using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Departments.Commands;
using DirectoryService.Core.Departments.Commands.CreateDepartment;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.IntegrationTests.Departments;

public class UpdateDepartmentNameTests(DirectoryTestWebFactory factory) : DirectoryBaseTests(factory)
{
    [Fact]
    public async Task UpdateDepartmentName_with_valid_data_should_succeed()
    {
        // Arrange
        var departmentId = await CreateDepartment("OldName", "slug");
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await ExecuteHandler(sut =>
            sut.Handle(
                new UpdateDepartmentNameCommand(new UpdateDepartmentNameRequest(departmentId, "NewName")),
                cancellationToken));

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(departmentId, result.Value);

        await ExecuteInDb(async dbContext =>
        {
            var department = await dbContext.Departments
                .FirstAsync(d => d.Id == DepartmentId.Create(departmentId).Value, cancellationToken);

            Assert.Equal("NewName", department.Name.Value);
            // slug и path не должны измениться
            Assert.Equal("slug", department.Slug.Value);
        });
    }

    [Fact]
    public async Task UpdateDepartmentName_non_existing_department_should_fail()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var fakeId = Guid.NewGuid();

        // Act
        var result = await ExecuteHandler(sut =>
            sut.Handle(
                new UpdateDepartmentNameCommand(new UpdateDepartmentNameRequest(fakeId, "NewName")),
                cancellationToken));

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task UpdateDepartmentName_with_empty_guid_should_fail()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await ExecuteHandler(sut =>
            sut.Handle(
                new UpdateDepartmentNameCommand(new UpdateDepartmentNameRequest(Guid.Empty, "NewName")),
                cancellationToken));

        // Assert
        Assert.True(result.IsFailure);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task UpdateDepartmentName_with_invalid_name_should_fail(string invalidName)
    {
        // Arrange
        var departmentId = await CreateDepartment("OldName", "slug");
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await ExecuteHandler(sut =>
            sut.Handle(
                new UpdateDepartmentNameCommand(new UpdateDepartmentNameRequest(departmentId, invalidName)),
                cancellationToken));

        // Assert
        Assert.True(result.IsFailure);

        await ExecuteInDb(async dbContext =>
        {
            var department = await dbContext.Departments
                .FirstAsync(d => d.Id == DepartmentId.Create(departmentId).Value, cancellationToken);

            // Имя не изменилось
            Assert.Equal("OldName", department.Name.Value);
        });
    }

    [Fact]
    public async Task UpdateDepartmentName_should_not_change_slug_and_path()
    {
        // Arrange
        var parentId = await CreateDepartment("Parent", "parent");
        var childId = await CreateDepartment("Child", "child", parentId);
        var cancellationToken = CancellationToken.None;

        var childGuid = DepartmentId.Create(childId).Value;

        string? slugBefore = null;
        string? pathBefore = null;

        await ExecuteInDb(async dbContext =>
        {
            var child = await dbContext.Departments
                .FirstAsync(d => d.Id == childGuid, cancellationToken);
            slugBefore = child.Slug.Value;
            pathBefore = child.Path.Value;
        });

        // Act
        var result = await ExecuteHandler(sut =>
            sut.Handle(
                new UpdateDepartmentNameCommand(new UpdateDepartmentNameRequest(childId, "NewChildName")),
                cancellationToken));

        // Assert
        Assert.True(result.IsSuccess);

        await ExecuteInDb(async dbContext =>
        {
            var child = await dbContext.Departments
                .FirstAsync(d => d.Id == childGuid, cancellationToken);

            Assert.Equal("NewChildName", child.Name.Value);
            Assert.Equal(slugBefore, child.Slug.Value);
            Assert.Equal(pathBefore, child.Path.Value);
        });
    }

    [Fact]
    public async Task UpdateDepartmentName_should_not_affect_other_departments()
    {
        // Arrange
        var firstId = await CreateDepartment("First", "first");
        var secondId = await CreateDepartment("Second", "second");
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await ExecuteHandler(sut =>
            sut.Handle(
                new UpdateDepartmentNameCommand(new UpdateDepartmentNameRequest(firstId, "FirstUpdated")),
                cancellationToken));

        // Assert
        Assert.True(result.IsSuccess);

        await ExecuteInDb(async dbContext =>
        {
            var first = await dbContext.Departments
                .FirstAsync(d => d.Id == DepartmentId.Create(firstId).Value, cancellationToken);
            var second = await dbContext.Departments
                .FirstAsync(d => d.Id == DepartmentId.Create(secondId).Value, cancellationToken);

            Assert.Equal("FirstUpdated", first.Name.Value);
            Assert.Equal("Second", second.Name.Value); // не тронут
        });
    }

    [Fact]
    public async Task UpdateDepartmentName_with_same_name_should_succeed()
    {
        // Arrange
        var departmentId = await CreateDepartment("SameName", "slug");
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await ExecuteHandler(sut =>
            sut.Handle(
                new UpdateDepartmentNameCommand(new UpdateDepartmentNameRequest(departmentId, "SameName")),
                cancellationToken));

        // Assert
        Assert.True(result.IsSuccess);

        await ExecuteInDb(async dbContext =>
        {
            var department = await dbContext.Departments
                .FirstAsync(d => d.Id == DepartmentId.Create(departmentId).Value, cancellationToken);
            Assert.Equal("SameName", department.Name.Value);
        });
    }

    private async Task<Guid> CreateDepartment(string name, string slug, Guid? parentId = null)
    {
        var locationId = await CreateLocation();
        var cancellationToken = CancellationToken.None;

        await using var scope = Services.CreateAsyncScope();
        var handler = scope.ServiceProvider.GetRequiredService<CreateDepartmentHandler>();

        var result = await handler.Handle(
            new CreateDepartmentCommand(new CreateDepartmentRequest(name, slug, parentId, [locationId])),
            cancellationToken);

        Assert.True(result.IsSuccess);
        return result.Value;
    }

    private async Task<Guid> CreateLocation()
    {
        return await ExecuteInDb(async dbContext =>
        {
            var unique = Guid.NewGuid().ToString("N");
            var location = Domain.Locations.Location.Create(
                $"name-{unique}", "street", "city", "country");

            dbContext.Add(location.Value);
            await dbContext.SaveChangesAsync();
            return location.Value.Id.Value;
        });
    }

    private async Task<T> ExecuteHandler<T>(Func<UpdateDepartmentNameHandler, Task<T>> action)
    {
        await using var scope = Services.CreateAsyncScope();
        var sut = scope.ServiceProvider.GetRequiredService<UpdateDepartmentNameHandler>();
        return await action(sut);
    }
}