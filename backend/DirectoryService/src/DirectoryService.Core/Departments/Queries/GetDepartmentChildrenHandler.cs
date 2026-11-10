using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Departments.Errors;
using Shared;

namespace DirectoryService.Core.Departments.Queries;

public class GetDepartmentChildrenHandler
{
    private readonly IDepartmentTreeRepository _repository;

    public GetDepartmentChildrenHandler(IDepartmentTreeRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IReadOnlyList<DepartmentTreeNodeDto>, Shared.Errors>> Handle(
        Guid parentId,
        CancellationToken cancellationToken)
    {
        var exists = await _repository.ExistsAsync(parentId, cancellationToken);

        if (!exists)
            return Fails.DepartmentError.DepartmentNotFoundException(parentId);

        var children = await _repository.GetChildrenAsync(parentId, cancellationToken);

        return Result.Success<IReadOnlyList<DepartmentTreeNodeDto>, Shared.Errors>(children);
    }
}
