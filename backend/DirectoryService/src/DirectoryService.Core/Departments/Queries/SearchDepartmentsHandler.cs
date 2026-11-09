using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Departments;
using Shared;

namespace DirectoryService.Core.Departments.Queries;

public class SearchDepartmentsHandler
{
    private const int MinQueryLength = 2;
    private const int ResultsLimit = 50;

    private readonly IDepartmentTreeRepository _repository;

    public SearchDepartmentsHandler(IDepartmentTreeRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IReadOnlyList<DepartmentSearchResultDto>, Shared.Errors>> Handle(
        string? query,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Trim().Length < MinQueryLength)
            return GeneralErrors.ValueLengthIsInvalid("q", $"минимум {MinQueryLength} символа");

        var results = await _repository.SearchAsync(
            query.Trim(),
            ResultsLimit,
            cancellationToken);

        return Result.Success<IReadOnlyList<DepartmentSearchResultDto>, Shared.Errors>(results);
    }
}
