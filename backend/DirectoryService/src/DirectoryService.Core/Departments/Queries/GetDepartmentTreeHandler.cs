using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Departments;
using Shared;

namespace DirectoryService.Core.Departments.Queries;

public class GetDepartmentTreeHandler
{
    private readonly IDepartmentTreeRepository _repository;

    public GetDepartmentTreeHandler(IDepartmentTreeRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IReadOnlyList<DepartmentTreeNodeDto>, Shared.Errors>> Handle(
        CancellationToken cancellationToken)
    {
        var roots = await _repository.GetRootsAsync(cancellationToken);

        return Result.Success<IReadOnlyList<DepartmentTreeNodeDto>, Shared.Errors>(roots);
    }
}
