using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Departments;
using DirectoryService.Domain.Departments.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Postgres.Departments;

public sealed class EfCoreDepartmentTreeRepository : IDepartmentTreeRepository
{
    private readonly AppDbContext _context;

    public EfCoreDepartmentTreeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<DepartmentTreeNodeDto>> GetRootsAsync(
        CancellationToken cancellationToken)
    {
        return await _context.Departments
            .AsNoTracking()
            .Where(d => d.ParentId == null)
            .OrderBy(d => d.Name)
            .Select(d => new DepartmentTreeNodeDto
            {
                Id = d.Id.Value,
                Name = d.Name.Value,
                Slug = d.Slug.Value,
                Path = d.Path.Value,
                Depth = d.Depth,
                HasChildren = _context.Departments.Any(c => c.ParentId == d.Id)
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid departmentId, CancellationToken cancellationToken)
    {
        var id = DepartmentId.Create(departmentId);
        if (id.IsFailure)
            return false;

        return await _context.Departments
            .AnyAsync(d => d.Id == id.Value, cancellationToken);
    }

    public async Task<IReadOnlyList<DepartmentTreeNodeDto>> GetChildrenAsync(
        Guid parentId,
        CancellationToken cancellationToken)
    {
        var id = DepartmentId.Create(parentId);
        if (id.IsFailure)
            return [];

        return await _context.Departments
            .AsNoTracking()
            .Where(d => d.ParentId == id.Value)
            .OrderBy(d => d.Name)
            .Select(d => new DepartmentTreeNodeDto
            {
                Id = d.Id.Value,
                Name = d.Name.Value,
                Slug = d.Slug.Value,
                Path = d.Path.Value,
                Depth = d.Depth,
                HasChildren = _context.Departments.Any(c => c.ParentId == d.Id)
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DepartmentAncestorDto>> GetAncestorsAsync(
        Guid departmentId,
        CancellationToken cancellationToken)
    {
        return await _context.Departments
            .FromSqlInterpolated($"""
                SELECT a.*
                FROM departments AS a
                JOIN departments AS target ON target.id = {departmentId}
                WHERE a.is_deleted = false
                  AND target.is_deleted = false
                  AND a.path @> target.path
                  AND a.path <> target.path
                ORDER BY nlevel(a.path)
                """)
            .IgnoreQueryFilters()
            .AsNoTracking()
            .OrderBy(d => d.Depth)
            .Select(d => new DepartmentAncestorDto
            {
                Id = d.Id.Value,
                Name = d.Name.Value,
                Slug = d.Slug.Value,
                Path = d.Path.Value,
                Depth = d.Depth
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DepartmentSearchResultDto>> SearchAsync(
        string query,
        int limit,
        CancellationToken cancellationToken)
    {
        var pattern = $"%{EscapeLikePattern(query)}%";

        var matches = await _context.Departments
            .FromSqlInterpolated($"""
                SELECT d.*
                FROM departments AS d
                WHERE d.is_deleted = false
                  AND d.name ILIKE {pattern}
                ORDER BY d.name
                LIMIT {limit}
                """)
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Select(d => new DepartmentTreeNodeDto
            {
                Id = d.Id.Value,
                Name = d.Name.Value,
                Slug = d.Slug.Value,
                Path = d.Path.Value,
                Depth = d.Depth,
                HasChildren = _context.Departments.Any(c => c.ParentId == d.Id)
            })
            .ToListAsync(cancellationToken);

        if (matches.Count == 0)
            return [];

        var ids = matches.Select(m => m.Id).ToArray();

        var ancestors = await _context.Departments
            .FromSqlInterpolated($"""
                SELECT a.*
                FROM departments AS a
                WHERE a.is_deleted = false
                  AND EXISTS (
                      SELECT 1
                      FROM departments AS m
                      WHERE m.id = ANY({ids}::uuid[])
                        AND m.is_deleted = false
                        AND a.path @> m.path
                        AND a.path <> m.path
                  )
                ORDER BY nlevel(a.path)
                """)
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Select(d => new DepartmentAncestorDto
            {
                Id = d.Id.Value,
                Name = d.Name.Value,
                Slug = d.Slug.Value,
                Path = d.Path.Value,
                Depth = d.Depth
            })
            .ToListAsync(cancellationToken);

        var ancestorsByPath = ancestors
            .GroupBy(a => a.Path, StringComparer.Ordinal)
            .ToDictionary(g => g.Key, g => g.First(), StringComparer.Ordinal);

        return matches
            .Select(match => new DepartmentSearchResultDto
            {
                Department = match,
                Ancestors = GetAncestorPaths(match.Path)
                    .Where(ancestorsByPath.ContainsKey)
                    .Select(path => ancestorsByPath[path])
                    .ToList()
            })
            .ToList();
    }

    private static IEnumerable<string> GetAncestorPaths(string path)
    {
        var segments = path.Split('/');

        var prefix = string.Empty;

        for (var i = 0; i < segments.Length - 1; i++)
        {
            prefix = i == 0 ? segments[i] : $"{prefix}/{segments[i]}";
            yield return prefix;
        }
    }

    private static string EscapeLikePattern(string value)
    {
        return value
            .Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("%", "\\%", StringComparison.Ordinal)
            .Replace("_", "\\_", StringComparison.Ordinal);
    }
}
