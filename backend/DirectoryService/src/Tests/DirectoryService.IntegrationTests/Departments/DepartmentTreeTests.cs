using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Departments.Commands.CreateDepartment;
using DirectoryService.Core.Departments.Queries;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared;

namespace DirectoryService.IntegrationTests.Departments;

public class DepartmentTreeTests(DirectoryTestWebFactory factory) : DirectoryBaseTests(factory)
{
    private readonly CancellationToken _ct = CancellationToken.None;

    [Fact]
    public async Task GetTree_when_database_is_empty_returns_empty_list()
    {
        // Act
        var result = await ExecuteHandler<GetDepartmentTreeHandler, Result<IReadOnlyList<DepartmentTreeNodeDto>, Errors>>(
            handler => handler.Handle(_ct));

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task GetTree_returns_only_roots_with_has_children_flag()
    {
        // Arrange
        var rootId = await CreateDepartment("Company", "company");
        await CreateDepartment("Sales", "sales", rootId);
        await CreateDepartment("HR", "hr", rootId);
        await CreateDepartment("Partner", "partner");

        // Act
        var result = await ExecuteHandler<GetDepartmentTreeHandler, Result<IReadOnlyList<DepartmentTreeNodeDto>, Errors>>(
            handler => handler.Handle(_ct));

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count);

        var company = Assert.Single(result.Value, d => d.Id == rootId);
        Assert.Equal(0, company.Depth);
        Assert.True(company.HasChildren);

        var partner = Assert.Single(result.Value, d => string.Equals(d.Slug, "partner", StringComparison.Ordinal));
        Assert.False(partner.HasChildren);
    }

    [Fact]
    public async Task GetChildren_returns_only_direct_children()
    {
        // Arrange
        var rootId = await CreateDepartment("Company", "company");
        var salesId = await CreateDepartment("Sales", "sales", rootId);
        await CreateDepartment("HR", "hr", rootId);
        await CreateDepartment("Moscow", "moscow", salesId);

        // Act
        var result = await ExecuteHandler<GetDepartmentChildrenHandler, Result<IReadOnlyList<DepartmentTreeNodeDto>, Errors>>(
            handler => handler.Handle(rootId, _ct));

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count);
        Assert.DoesNotContain(result.Value, d => string.Equals(d.Slug, "moscow", StringComparison.Ordinal));

        var sales = Assert.Single(result.Value, d => d.Id == salesId);
        Assert.True(sales.HasChildren);
        Assert.Equal(1, sales.Depth);
    }

    [Fact]
    public async Task GetChildren_for_node_without_children_returns_empty_list()
    {
        // Arrange
        var rootId = await CreateDepartment("Company", "company");

        // Act
        var result = await ExecuteHandler<GetDepartmentChildrenHandler, Result<IReadOnlyList<DepartmentTreeNodeDto>, Errors>>(
            handler => handler.Handle(rootId, _ct));

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task GetChildren_for_missing_node_returns_not_found()
    {
        // Act
        var result = await ExecuteHandler<GetDepartmentChildrenHandler, Result<IReadOnlyList<DepartmentTreeNodeDto>, Errors>>(
            handler => handler.Handle(Guid.NewGuid(), _ct));

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Error, e => e.Type == ErrorType.NOT_FOUND);
    }

    [Fact]
    public async Task GetAncestors_for_root_returns_empty_list()
    {
        // Arrange
        var rootId = await CreateDepartment("Company", "company");

        // Act
        var result = await ExecuteHandler<GetDepartmentAncestorsHandler, Result<IReadOnlyList<DepartmentAncestorDto>, Errors>>(
            handler => handler.Handle(rootId, _ct));

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task GetAncestors_returns_chain_from_root_to_parent()
    {
        // Arrange
        var rootId = await CreateDepartment("Company", "company");
        var salesId = await CreateDepartment("Sales", "sales", rootId);
        var moscowId = await CreateDepartment("Moscow", "moscow", salesId);

        // Act
        var result = await ExecuteHandler<GetDepartmentAncestorsHandler, Result<IReadOnlyList<DepartmentAncestorDto>, Errors>>(
            handler => handler.Handle(moscowId, _ct));

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Collection(
            result.Value,
            company =>
            {
                Assert.Equal(rootId, company.Id);
                Assert.Equal(0, company.Depth);
            },
            sales =>
            {
                Assert.Equal(salesId, sales.Id);
                Assert.Equal(1, sales.Depth);
            });
    }

    [Fact]
    public async Task GetAncestors_for_missing_node_returns_not_found()
    {
        // Act
        var result = await ExecuteHandler<GetDepartmentAncestorsHandler, Result<IReadOnlyList<DepartmentAncestorDto>, Errors>>(
            handler => handler.Handle(Guid.NewGuid(), _ct));

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Error, e => e.Type == ErrorType.NOT_FOUND);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("a")]
    [InlineData(" a ")]
    public async Task Search_with_too_short_query_returns_validation_error(string? query)
    {
        // Act
        var result = await ExecuteHandler<SearchDepartmentsHandler, Result<IReadOnlyList<DepartmentSearchResultDto>, Errors>>(
            handler => handler.Handle(query, _ct));

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Error, e => e.Type == ErrorType.VALIDATION);
    }

    [Fact]
    public async Task Search_without_matches_returns_empty_list()
    {
        // Arrange
        await CreateDepartment("Company", "company");

        // Act
        var result = await ExecuteHandler<SearchDepartmentsHandler, Result<IReadOnlyList<DepartmentSearchResultDto>, Errors>>(
            handler => handler.Handle("zzz", _ct));

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task Search_is_case_insensitive_and_returns_ancestors()
    {
        // Arrange
        var rootId = await CreateDepartment("Company", "company");
        var salesId = await CreateDepartment("Sales", "sales", rootId);
        await CreateDepartment("Moscow Office", "moscow", salesId);

        // Act
        var result = await ExecuteHandler<SearchDepartmentsHandler, Result<IReadOnlyList<DepartmentSearchResultDto>, Errors>>(
            handler => handler.Handle("MOSCOW", _ct));

        // Assert
        Assert.True(result.IsSuccess);

        var match = Assert.Single(result.Value);
        Assert.Equal("Moscow Office", match.Department.Name);
        Assert.Equal(2, match.Department.Depth);

        Assert.Collection(
            match.Ancestors,
            company => Assert.Equal(rootId, company.Id),
            sales => Assert.Equal(salesId, sales.Id));
    }

    [Fact]
    public async Task Soft_deleted_departments_are_hidden_everywhere()
    {
        // Arrange
        var rootId = await CreateDepartment("Company", "company");
        var salesId = await CreateDepartment("Sales", "sales", rootId);
        var moscowId = await CreateDepartment("Moscow", "moscow", salesId);

        await ExecuteInDb(async dbContext =>
        {
            await dbContext.Departments
                .Where(d => d.Id == DepartmentId.Create(salesId).Value)
                .ExecuteUpdateAsync(s => s.SetProperty(d => d.IsDeleted, true), _ct);
        });

        // Act
        var tree = await ExecuteHandler<GetDepartmentTreeHandler, Result<IReadOnlyList<DepartmentTreeNodeDto>, Errors>>(
            handler => handler.Handle(_ct));
        var children = await ExecuteHandler<GetDepartmentChildrenHandler, Result<IReadOnlyList<DepartmentTreeNodeDto>, Errors>>(
            handler => handler.Handle(rootId, _ct));
        var ancestors = await ExecuteHandler<GetDepartmentAncestorsHandler, Result<IReadOnlyList<DepartmentAncestorDto>, Errors>>(
            handler => handler.Handle(moscowId, _ct));
        var search = await ExecuteHandler<SearchDepartmentsHandler, Result<IReadOnlyList<DepartmentSearchResultDto>, Errors>>(
            handler => handler.Handle("sales", _ct));

        // Assert
        Assert.True(tree.IsSuccess);
        var company = Assert.Single(tree.Value);
        Assert.Equal(rootId, company.Id);
        Assert.False(company.HasChildren);

        Assert.True(children.IsSuccess);
        Assert.Empty(children.Value);

        Assert.True(ancestors.IsSuccess);
        var ancestor = Assert.Single(ancestors.Value);
        Assert.Equal(rootId, ancestor.Id);

        Assert.True(search.IsSuccess);
        Assert.Empty(search.Value);
    }

    [Fact]
    public async Task CreateDepartment_sets_path_and_depth_for_root_and_children()
    {
        // Arrange
        var rootId = await CreateDepartment("Company", "company");
        var salesId = await CreateDepartment("Sales", "sales", rootId);
        var moscowId = await CreateDepartment("Moscow", "moscow", salesId);

        // Act & Assert
        await ExecuteInDb(async dbContext =>
        {
            var root = await dbContext.Departments
                .SingleAsync(d => d.Id == DepartmentId.Create(rootId).Value, _ct);
            var sales = await dbContext.Departments
                .SingleAsync(d => d.Id == DepartmentId.Create(salesId).Value, _ct);
            var moscow = await dbContext.Departments
                .SingleAsync(d => d.Id == DepartmentId.Create(moscowId).Value, _ct);

            Assert.Equal(0, root.Depth);
            Assert.Equal("company", root.Path.Value);

            Assert.Equal(1, sales.Depth);
            Assert.Equal("company/sales", sales.Path.Value);

            Assert.Equal(2, moscow.Depth);
            Assert.Equal("company/sales/moscow", moscow.Path.Value);
        });
    }

    [Fact]
    public async Task Path_column_is_stored_as_ltree()
    {
        // Arrange
        await CreateDepartment("Company", "company");

        // Act
        var columnType = await ExecuteInDb(async dbContext =>
            await dbContext.Database
                .SqlQueryRaw<string>("""
                    SELECT udt_name::text AS "Value"
                    FROM information_schema.columns
                    WHERE table_name = 'departments' AND column_name = 'path'
                    """)
                .SingleAsync(_ct));

        // Assert
        Assert.Equal("ltree", columnType);
    }

    private async Task<Guid> CreateDepartment(string name, string slug, Guid? parentId = null)
    {
        var locationId = await CreateLocation();

        var result = await ExecuteHandler<CreateDepartmentHandler, Result<Guid, Errors>>(handler =>
            handler.Handle(
                new CreateDepartmentCommand(new CreateDepartmentRequest(
                    name,
                    slug,
                    parentId,
                    [locationId])),
                _ct));

        Assert.True(result.IsSuccess);
        return result.Value;
    }

    private async Task<Guid> CreateLocation()
    {
        return await ExecuteInDb(async dbContext =>
        {
            var location = Domain.Locations.Location.Create(
                "name",
                "street",
                "city",
                "country");

            dbContext.Add(location.Value);
            await dbContext.SaveChangesAsync(_ct);

            return location.Value.Id.Value;
        });
    }

    private async Task<TResult> ExecuteHandler<THandler, TResult>(
        Func<THandler, Task<TResult>> action)
        where THandler : class
    {
        await using var scope = Services.CreateAsyncScope();
        var handler = scope.ServiceProvider.GetRequiredService<THandler>();

        return await action(handler);
    }
}
