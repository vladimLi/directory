using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Departments.Errors;
using Shared;

namespace DirectoryService.Core.Departments.Queries;

public class GetDepartmentAncestorsHandler
{
    private readonly IDepartmentTreeRepository _repository;

    public GetDepartmentAncestorsHandler(IDepartmentTreeRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IReadOnlyList<DepartmentAncestorDto>, Shared.Errors>> Handle(
        Guid departmentId,
        CancellationToken cancellationToken)
    {
        var exists = await _repository.ExistsAsync(departmentId, cancellationToken);

        if (!exists)
            return Fails.DepartmentError.DepartmentNotFoundException(departmentId);

        var ancestors = await _repository.GetAncestorsAsync(departmentId, cancellationToken);

        return Result.Success<IReadOnlyList<DepartmentAncestorDto>, Shared.Errors>(ancestors);
    }
}
