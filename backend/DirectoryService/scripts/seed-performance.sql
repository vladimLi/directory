-- Seed for DS performance/index analysis.
-- Run on dev DB after migrations.

BEGIN;

TRUNCATE TABLE department_positions, department_locations, departments, positions, locations RESTART IDENTITY CASCADE;

-- Locations (300)
INSERT INTO locations (id, location_name, location_address, created_at, updated_at, is_deleted, deleted_at)
SELECT
  gen_random_uuid(),
  'Location ' || i,
  'Address ' || i || ', City, Country',
  NOW(), NOW(), false, '-infinity'::timestamptz
FROM generate_series(1, 300) AS s(i);

-- Positions (300)
INSERT INTO positions (id, name, created_at, updated_at, is_deleted, deleted_at)
SELECT
  gen_random_uuid(),
  'Position ' || i,
  NOW(), NOW(), false, '-infinity'::timestamptz
FROM generate_series(1, 300) AS s(i);

-- Departments: ~10,000 in 4 levels (20 roots). Labels use [A-Za-z0-9_], safe for ltree.
DO $$
DECLARE
  root_id uuid;
  l1_id uuid;
  l2_id uuid;
  l3_id uuid;
  root_slug text;
  l1_slug text;
  l2_slug text;
  l3_slug text;
  root_path ltree;
  l1_path ltree;
  l2_path ltree;
  l3_path ltree;
  n int := 0;
BEGIN
  FOR r IN 1..20 LOOP
    root_id := gen_random_uuid();
    root_slug := 'dept_r' || lpad(r::text, 2, '0');
    root_path := root_slug::ltree;
    INSERT INTO departments (id, name, slug, path, parent_id, depth, created_at, updated_at, is_deleted, deleted_at)
    VALUES (root_id, 'Department Root ' || r, root_slug, root_path, NULL, 0, NOW(), NOW(), false, '-infinity'::timestamptz);
    n := n + 1;

    FOR c1 IN 1..9 LOOP
      l1_id := gen_random_uuid();
      l1_slug := 'dept_r' || lpad(r::text,2,'0') || '_a' || lpad(c1::text,2,'0');
      l1_path := (root_path::text || '.' || l1_slug)::ltree;
      INSERT INTO departments (id, name, slug, path, parent_id, depth, created_at, updated_at, is_deleted, deleted_at)
      VALUES (l1_id, 'Department L1 ' || r || '-' || c1, l1_slug, l1_path, root_id, 1, NOW(), NOW(), false, '-infinity'::timestamptz);
      n := n + 1;

      FOR c2 IN 1..10 LOOP
        l2_id := gen_random_uuid();
        l2_slug := l1_slug || '_b' || lpad(c2::text,2,'0');
        l2_path := (l1_path::text || '.' || l2_slug)::ltree;
        INSERT INTO departments (id, name, slug, path, parent_id, depth, created_at, updated_at, is_deleted, deleted_at)
        VALUES (l2_id, 'Department L2 ' || r || '-' || c1 || '-' || c2, l2_slug, l2_path, l1_id, 2, NOW(), NOW(), false, '-infinity'::timestamptz);
        n := n + 1;

        FOR c3 IN 1..(CASE WHEN c2 <= 8 THEN 4 ELSE 5 END) LOOP
          l3_id := gen_random_uuid();
          l3_slug := l2_slug || '_c' || lpad(c3::text,2,'0');
          l3_path := (l2_path::text || '.' || l3_slug)::ltree;
          INSERT INTO departments (id, name, slug, path, parent_id, depth, created_at, updated_at, is_deleted, deleted_at)
          VALUES (l3_id, 'Department L3 ' || r || '-' || c1 || '-' || c2 || '-' || c3, l3_slug, l3_path, l2_id, 3, NOW(), NOW(), false, '-infinity'::timestamptz);
          n := n + 1;
        END LOOP;
      END LOOP;
    END LOOP;
  END LOOP;

  RAISE NOTICE 'Inserted departments: %', n;
END $$;

-- Links: each department -> 1-2 locations, 1-2 positions
INSERT INTO department_locations (id, department_id, location_id, is_primary)
SELECT gen_random_uuid(), d.id, l.id, (rn = 1)
FROM departments d
JOIN LATERAL (
  SELECT id, row_number() OVER () AS rn
  FROM locations
  ORDER BY md5(d.id::text || id::text)
  LIMIT 1 + (abs(hashtext(d.id::text)) % 2)
) l ON true;

INSERT INTO department_positions (id, department_id, position_id)
SELECT gen_random_uuid(), d.id, p.id
FROM departments d
JOIN LATERAL (
  SELECT id
  FROM positions
  ORDER BY md5(d.id::text || id::text)
  LIMIT 1 + (abs(hashtext(d.id::text)) % 2)
) p ON true;

ANALYZE;

COMMIT;

SELECT 'departments' AS tbl, count(*) AS cnt FROM departments
UNION ALL SELECT 'locations', count(*) FROM locations
UNION ALL SELECT 'positions', count(*) FROM positions
UNION ALL SELECT 'department_locations', count(*) FROM department_locations
UNION ALL SELECT 'department_positions', count(*) FROM department_positions;
