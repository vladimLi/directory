using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Departments.Commands.CreateDepartment;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.IntegrationTests.Departments;

public class CreateDepartmentTests(DirectoryTestWebFactory factory) : DirectoryBaseTests(factory)
{
    [Fact]
    public async Task CreateDepartment_with_valid_data_should_succeed()
    {
        // Arrange
        var locationId = await CreateLocation();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await ExecuteHandler(sut =>
        {
            var command = new CreateDepartmentCommand(new CreateDepartmentRequest(
                "Name",
                "slug",
                null,
                [locationId.Value]));
            return sut.Handle(command, cancellationToken);
        });

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);

        await ExecuteInDb(async dbContext =>
        {
            var department = await dbContext.Departments
                .FirstOrDefaultAsync(d => d.Id == DepartmentId.Create(result.Value).Value, cancellationToken);

            Assert.NotNull(department);
            Assert.Equal("Name", department.Name.Value);
            Assert.Equal("slug", department.Slug.Value);
            Assert.Null(department.ParentId);
        });
    }

    [Fact]
    public async Task CreateDepartment_with_parent_should_set_parent_path_and_id()
    {
        // Arrange
        var locationId = await CreateLocation();
        var cancellationToken = CancellationToken.None;

        var parentResult = await ExecuteHandler(sut =>
        {
            var command = new CreateDepartmentCommand(new CreateDepartmentRequest(
                "Parent",
                "parent",
                null,
                [locationId.Value]));
            return sut.Handle(command, cancellationToken);
        });

        Assert.True(parentResult.IsSuccess);

        // Act
        var childResult = await ExecuteHandler(sut =>
        {
            var command = new CreateDepartmentCommand(new CreateDepartmentRequest(
                "Child",
                "child",
                parentResult.Value,
                [locationId.Value]));
            return sut.Handle(command, cancellationToken);
        });

        // Assert
        Assert.True(childResult.IsSuccess);

        await ExecuteInDb(async dbContext =>
        {
            var parent = await dbContext.Departments
                .FirstAsync(d => d.Id == DepartmentId.Create(parentResult.Value).Value, cancellationToken);
            var child = await dbContext.Departments
                .FirstAsync(d => d.Id == DepartmentId.Create(childResult.Value).Value, cancellationToken);

            Assert.Equal(parent.Id, child.ParentId);

            // Явно указываем StringComparison, чтобы показать намерение (Ordinal — для путей/идентификаторов)
            Assert.True(
                child.Path.Value.Contains(parent.Path.Value, StringComparison.Ordinal),
                $"Ожидалось, что путь ребёнка '{child.Path.Value}' содержит путь родителя '{parent.Path.Value}'");
        });
    }

    [Fact]
    public async Task CreateDepartment_with_non_existing_parent_should_fail()
    {
        // Arrange
        var locationId = await CreateLocation();
        var fakeParentId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await ExecuteHandler(sut =>
        {
            var command = new CreateDepartmentCommand(new CreateDepartmentRequest(
                "Name",
                "slug",
                fakeParentId,
                [locationId.Value]));
            return sut.Handle(command, cancellationToken);
        });

        // Assert
        Assert.True(result.IsFailure);

        await ExecuteInDb(async dbContext =>
        {
            Assert.False(await dbContext.Departments.AnyAsync(cancellationToken));
        });
    }

    [Fact]
    public async Task CreateDepartment_with_non_existing_location_should_fail()
    {
        // Arrange
        var fakeLocationId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await ExecuteHandler(sut =>
        {
            var command = new CreateDepartmentCommand(new CreateDepartmentRequest(
                "Name",
                "slug",
                null,
                [fakeLocationId]));
            return sut.Handle(command, cancellationToken);
        });

        // Assert
        Assert.True(result.IsFailure);

        await ExecuteInDb(async dbContext =>
        {
            Assert.False(await dbContext.Departments.AnyAsync(cancellationToken));
        });
    }

    [Theory]
    [InlineData("", "slug")]
    [InlineData("Name", "")]
    public async Task CreateDepartment_with_invalid_data_should_fail(string name, string slug)
    {
        // Arrange
        var locationId = await CreateLocation();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await ExecuteHandler(sut =>
        {
            var command = new CreateDepartmentCommand(new CreateDepartmentRequest(
                name,
                slug,
                null,
                [locationId.Value]));
            return sut.Handle(command, cancellationToken);
        });

        // Assert
        Assert.True(result.IsFailure);

        await ExecuteInDb(async dbContext =>
        {
            Assert.False(await dbContext.Departments.AnyAsync(cancellationToken));
        });
    }
    
    [Fact]
    public async Task CreateDepartment_with_empty_location_ids_should_fail()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await ExecuteHandler(sut =>
        {
            var command = new CreateDepartmentCommand(new CreateDepartmentRequest(
                "Name", "slug", null, []));
            return sut.Handle(command, cancellationToken);
        });

        // Assert
        Assert.True(result.IsFailure);

        await ExecuteInDb(async dbContext =>
        {
            Assert.False(await dbContext.Departments.AnyAsync(cancellationToken));
            Assert.False(await dbContext.DepartmentLocation.AnyAsync(cancellationToken));
        });
    }

    private async Task<LocationId> CreateLocation()
    {
        return await ExecuteInDb(async dbContext =>
        {
            var location = Domain.Locations.Location.Create(
                "name",
                "street",
                "city",
                "country");

            dbContext.Add(location.Value);
            await dbContext.SaveChangesAsync();
            return location.Value.Id;
        });
    }

    private async Task<T> ExecuteHandler<T>(Func<CreateDepartmentHandler, Task<T>> action)
    {
        await using var scope = Services.CreateAsyncScope();
        var sut = scope.ServiceProvider.GetRequiredService<CreateDepartmentHandler>();
        return await action(sut);
    }
}