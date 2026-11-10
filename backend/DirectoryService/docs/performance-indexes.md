# DS: Индексы производительности read-запросов (ltree, дерево, связи)

## Контекст
После DS-21 появились tree-запросы через `ltree` (дети, предки, поиск). На пустой БД планы не показательные, поэтому анализ выполнен на dev-БД с реалистичным объёмом.

## Объём данных (dev-БД)

Сид выполнен скриптом [`scripts/seed-performance.sql`](../scripts/seed-performance.sql) после применения всех миграций.

| Таблица | Кол-во строк |
|---|---:|
| departments | 9 560 |
| locations | 300 |
| positions | 300 |
| department_locations | 14 403 |
| department_positions | 14 403 |

Структура дерева: 20 корней, 4 уровня (depth 0..3), ~10k подразделений. Связи: 1–2 локации и 1–2 должности на подразделение.

## Методика
- PostgreSQL 18 (`postgres:18`, dev docker-compose, порт 5434).
- `EXPLAIN (ANALYZE, BUFFERS, VERBOSE)` для ключевых запросов.
- Baseline зафиксирован до применения миграции `20261110081154_AddPerformanceIndexes` (после `AddDepartmentDepthAndLtree`).
- После применения миграции — те же запросы на тех же данных (`ANALYZE` выполнен сидом).
- Для `departments.parent_id` и `departments.path` индексы (`ix_departments_parent_id`, `ix_departments_path_gist`) были добавлены ранее в DS-21; baseline по ним показан в текущем (проиндексированном DS-21) состоянии и дополнительно подтверждён эквивалентностью операторов плана (Bitmap/Index Scan). Для поиска по имени и проверки slug — чистый baseline без индекса.
- Время выполнения зависит от кэша/окружения; важнее оператор плана и объём чтений.

## Таблица до/после

| Endpoint / сценарий | Запрос (суть) | План до | План после | Индекс | Комментарий |
|---|---|---|---|---|---|
| `GET /departments/{id}/children` | `WHERE d.parent_id = $1 AND d.is_deleted = false ORDER BY d.name` + correlated `EXISTS` для `has_children` | `Bitmap Index Scan on ix_departments_parent_id` → `Bitmap Heap Scan` (10 rows), подзапрос HasChildren: `Index Scan using ix_departments_parent_id` (loops=10). **0.217 ms**, shared hit=37 | Без изменений (индекс уже был, переиспользуется). **~0.14–0.22 ms** | `btree(parent_id)` — `ix_departments_parent_id` (DS-21) | Ускоряет выборку прямых детей и проверку наличия детей. Корневой уровень (`parent_id IS NULL`) покрывается тем же btree. |
| `GET /departments/{id}/ancestors` | `a.path @> target.path AND a.path <> target.path`, `JOIN target ON target.id = $id`, `ORDER BY nlevel(a.path)` | `Bitmap Index Scan on ix_departments_path_gist` (Index Cond `a.path @> target.path`) + `Index Scan pk_departments` по target. **0.351 ms**, shared hit=10 | Без изменений (индекс уже был). **~0.18–0.35 ms** | `GiST(path)` — `ix_departments_path_gist` (DS-21) | Поддерживает ltree-операторы предков/поддерева (`@>`, `<@` и др.) и сценарии cycle-check на базе предков/потомков. |
| `GET /departments/tree/search?q=...` | `WHERE d.is_deleted = false AND d.name ILIKE '%L3 1-1-1%' ORDER BY d.name LIMIT 50` | `Seq Scan on departments` (Rows Removed by Filter: 9551) → `Sort` → `Limit`. **4.019 ms**, shared hit=262 | `Bitmap Index Scan on ix_departments_name_trgm` (Index Cond `name ~~* ...`, rows=3152) → `Bitmap Heap Scan` (Recheck, Rows Removed by Index Recheck: 3143, точных 9) → `Sort` → `Limit`. **2.284 ms**, shared hit=272 | `GIN(name gin_trgm_ops)` — `ix_departments_name_trgm` + extension `pg_trgm` | Выбран `gin_trgm`, т.к. поиск — contains + регистронезависимый (`ILIKE '%q%'`). Функциональный btree `LOWER(name)` не ускоряет `%...%`. На большем объёме/более редких паттернах выигрыш от отказа от Seq Scan возрастает. |
| Проверка уникальности/поиска по `slug` (активные) | `WHERE slug = $1 AND is_deleted = false` | Отдельного индекса не было (baseline: поиск по slug шёл бы сканом/без специализированного индекса уникальности) | `Index Scan using ix_departments_slug_active`. **0.050 ms**, shared hit=3 | `UNIQUE btree(slug) WHERE is_deleted = false` — `ix_departments_slug_active` | Обеспечивает проверку уникальности slug среди активных записей; partial позволяет переиспользовать slug после soft delete. |
| `GET /locations/top` | `LEFT JOIN department_locations dl ON dl.location_id = l.id` + `count` + `GROUP BY` + `ORDER BY count DESC LIMIT 5` | `Seq Scan on department_locations` (14403 rows) + `Seq Scan on locations` → `Hash Join` → `HashAggregate`. **4.028 ms**, shared hit=160 | Аналогичный план (seq scan по всем связям/локациям), т.к. запрос агрегирует весь набор для топа. **3.298 ms**, shared hit=160 | `btree(location_id)` — `ix_department_locations_location_id`; `btree(department_id)` — `ix_department_locations_department_id` (существовали как `IX_*`, в миграции приведены к `ix_*`) | Индексы связей критичны для сценариев «локации/должности конкретного подразделения» и join/фильтра по конкретной локации/должности. Для полного агрегата по всем локациям (топ-N без фильтра) планировщик обоснованно выбирает seq scan + hash aggregate. |
| Join-сценарии по должностям | `WHERE department_id = $1` / `WHERE position_id = $1` на `department_positions` | Индексы FK-колонок присутствовали (`IX_department_positions_*`) | Те же индексы под единым именованием (`ix_department_positions_department_id`, `ix_department_positions_position_id`) | `btree(department_id)`, `btree(position_id)` | Симметрично связям с локациями; ускоряют выборки и join по подразделению/должности. |

## Добавленные/актуализированные индексы (миграция `AddPerformanceIndexes`)

| Индекс | Тип / предикат | Зачем |
|---|---|---|
| `ix_departments_name_trgm` | GIN (`gin_trgm_ops`) на `departments(name)` | Поиск `/departments/tree/search` по имени, `ILIKE '%q%'` без Seq Scan |
| `ix_departments_slug_active` | UNIQUE btree на `departments(slug)` WHERE `is_deleted = false` | Уникальность slug среди активных, быстрый lookup по slug |
| `ix_department_locations_department_id` | btree | Список/фильтр связей по подразделению (переименование из `IX_*`) |
| `ix_department_locations_location_id` | btree | Join/фильтр/агрегации по локации (переименование из `IX_*`) |
| `ix_department_positions_department_id` | btree | Связи по подразделению (переименование из `IX_*`) |
| `ix_department_positions_position_id` | btree | Связи по должности (переименование из `IX_*`) |

Расширение: `pg_trgm` (добавлено в `AppDbContext` и миграции).

Существующие (DS-21, без изменений в этой задаче): `ix_departments_path_gist` (GiST, `path`), `ix_departments_parent_id` (btree, `parent_id`).

## Partial index для активных записей — оценка

- **Принято:** `ix_departments_slug_active` — partial unique (`is_deleted = false`), т.к. даёт и семантику (повторное использование slug удалённого подразделения), и меньший/целевой индекс при накоплении удалённых строк.
- **Не добавлялись отдельно:** partial-аналоги для `parent_id`/`path` (`WHERE is_deleted = false`). Проверка на dev-наборе (9560 активных, 0 удалённых) через временные индексы показала сопоставимые планы/стоимость с уже существующими индексами; при текущем паттерне запросов и отсутствии большого объёма soft-deleted данных отдельный выигрыш не подтверждён. При появлении значительной доли удалённых записей имеет смысл пересмотреть (отдельный partial GiST/btree может уменьшить размер индекса и объём сканирования).

## Воспроизводимость

```powershell
# из backend/DirectoryService
docker compose up -d
dotnet ef database update --project src/DirectoryService.Infrastructure.Postgres/DirectoryService.Infrastructure.Postgres.csproj --startup-project src/DirectoryService.Web/DirectoryService.Web.csproj

# сид (PowerShell)
Get-Content scripts/seed-performance.sql -Raw | docker exec -i postgres psql -U postgres -d directory_service_db -v ON_ERROR_STOP=1

# пример EXPLAIN
docker exec postgres psql -U postgres -d directory_service_db -c "EXPLAIN (ANALYZE, BUFFERS) SELECT ...;"
```

## Проверки
- Миграции применяются на существующей dev-БД и проектируются для чистой БД (расширения `ltree`, `pg_trgm` декларированы; индексы создаются/переименовываются миграцией).
- После изменений: `dotnet build DirectoryService.slnx` и интеграционные тесты должны проходить (Testcontainers поднимают PostgreSQL с нужными расширениями).
