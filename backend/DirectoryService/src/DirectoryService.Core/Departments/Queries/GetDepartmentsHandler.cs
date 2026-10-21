using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Database;
using DirectoryService.Core.Departments.Queries.Enumerations;
using DirectoryService.Core.Enumerations;
using DirectoryService.Domain;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace DirectoryService.Core.Departments.Queries;

public class GetDepartmentsHandler
{
    private readonly IReadDbContext _readDbContext;

    public GetDepartmentsHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }
    public async Task<Result<DepartmentListItemDto, Shared.Errors>> Handle(
        GetDepartmentsRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Search?.Length > LengthConstants.Length50)
            return GeneralErrors.ValueLengthIsInvalid("search");
        
        if (request.Pagination.Page < LengthConstants.Length1)
            return GeneralErrors.ValueLengthIsInvalid("pagination.page");
        
        if (request.Pagination.PageSize > LengthConstants.Length50)
            return GeneralErrors.ValueLengthIsInvalid("pagination.page.size");
        
        var sortByRaw  = string.IsNullOrWhiteSpace(request.SortBy)  ? "name" : request.SortBy;
        var sortDirRaw = string.IsNullOrWhiteSpace(request.SortDirection) ? "asc"  : request.SortDirection;

        if (!Enum.TryParse<DepartmentSortBy>(sortByRaw, ignoreCase: true, out var sortBy))
            return GeneralErrors.ConditionIsInvalid("нет доступных перечислений","sortBy");
        
        if (!Enum.TryParse<SortDirection>(sortDirRaw, ignoreCase: true, out var sortDir))
            return GeneralErrors.ConditionIsInvalid("нет доступных перечислений", "sortDir");
        
        var query = _readDbContext.DepartmentsRead;
        
        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(d => 
                EF.Functions.Like(d.Name, $"%{request.Search.ToLowerInvariant()}%"));
        
        query = (sortBy, sortDir) switch
        {
            (DepartmentSortBy.Name, SortDirection.Asc) =>
                query.OrderBy(d => d.Name).ThenBy(d => d.Id),

            (DepartmentSortBy.Name, SortDirection.Desc) =>
                query.OrderByDescending(d => d.Name).ThenByDescending(d => d.Id),

            (DepartmentSortBy.CreatedAt, SortDirection.Asc) =>
                query.OrderBy(d => d.CreatedAt).ThenBy(d => d.Id),

            (DepartmentSortBy.CreatedAt, SortDirection.Desc) =>
                query.OrderByDescending(d => d.CreatedAt).ThenByDescending(d => d.Id),

            _ => query.OrderBy(d => d.Name).ThenBy(d => d.Id)
        };
        
        var totalCount = await query.LongCountAsync(cancellationToken);
        
        query = query
            .Skip((request.Pagination.Page - 1) * request.Pagination.PageSize)
            .Take(request.Pagination.PageSize);
        
        var departments = await query
            .Select(e => new DepartmentDto
            {
                Id = e.Id.Value,
                Name = e.Name.Value,
                Slug = e.Slug.Value,
                Path = e.Path.Value,
                CreatedAt = e.CreatedAt,
            })
            .ToListAsync(cancellationToken);

        return new DepartmentListItemDto(departments, totalCount);
    }
}