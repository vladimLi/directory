using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Departments.Commands.CreateDepartment;
using DirectoryService.Core.Departments.Commands.UpdateDepartmentSlug;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.IntegrationTests.Departments;

public class UpdateDepartmentSlugTests(DirectoryTestWebFactory factory) : DirectoryBaseTests(factory)
{
    [Fact]
    public async Task UpdateDepartmentSlug_with_valid_data_should_succeed()
    {
        // Arrange
        var departmentId = await CreateDepartment("Name", "old-slug");
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await ExecuteHandler(sut =>
            sut.Handle(
                new UpdateDepartmentSlugCommand(new UpdateDepartmentSlugRequest(departmentId, "new-slug")),
                cancellationToken));

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(departmentId, result.Value);

        await ExecuteInDb(async dbContext =>
        {
            var department = await dbContext.Departments
                .FirstAsync(d => d.Id == DepartmentId.Create(departmentId).Value, cancellationToken);

            Assert.Equal("new-slug", department.Slug.Value);
            // имя не изменилось
            Assert.Equal("Name", department.Name.Value);
        });
    }

    [Fact]
    public async Task UpdateDepartmentSlug_with_invalid_slug_should_fail()
    {
        // Arrange
        var departmentId = await CreateDepartment("Name", "valid-slug");
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await ExecuteHandler(sut =>
            sut.Handle(
                new UpdateDepartmentSlugCommand(new UpdateDepartmentSlugRequest(departmentId, "")),
                cancellationToken));

        // Assert
        Assert.True(result.IsFailure);

        await ExecuteInDb(async dbContext =>
        {
            var department = await dbContext.Departments
                .FirstAsync(d => d.Id == DepartmentId.Create(departmentId).Value, cancellationToken);
            Assert.Equal("valid-slug", department.Slug.Value); // slug не изменился
        });
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("UPPER CASE")]
    [InlineData("with space")]
    public async Task UpdateDepartmentSlug_with_invalid_format_should_fail(string invalidSlug)
    {
        // Arrange
        var departmentId = await CreateDepartment("Name", "valid-slug");
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await ExecuteHandler(sut =>
            sut.Handle(
                new UpdateDepartmentSlugCommand(new UpdateDepartmentSlugRequest(departmentId, invalidSlug)),
                cancellationToken));

        // Assert
        Assert.True(result.IsFailure);

        await ExecuteInDb(async dbContext =>
        {
            var department = await dbContext.Departments
                .FirstAsync(d => d.Id == DepartmentId.Create(departmentId).Value, cancellationToken);
            Assert.Equal("valid-slug", department.Slug.Value);
        });
    }

    [Fact]
    public async Task UpdateDepartmentSlug_non_existing_department_should_fail()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var fakeId = Guid.NewGuid();

        // Act
        var result = await ExecuteHandler(sut =>
            sut.Handle(
                new UpdateDepartmentSlugCommand(new UpdateDepartmentSlugRequest(fakeId, "new-slug")),
                cancellationToken));

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task UpdateDepartmentSlug_with_empty_guid_should_fail()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await ExecuteHandler(sut =>
            sut.Handle(
                new UpdateDepartmentSlugCommand(new UpdateDepartmentSlugRequest(Guid.Empty, "new-slug")),
                cancellationToken));

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task UpdateDepartmentSlug_with_same_slug_should_succeed()
    {
        // Arrange
        var departmentId = await CreateDepartment("Name", "same-slug");
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await ExecuteHandler(sut =>
            sut.Handle(
                new UpdateDepartmentSlugCommand(new UpdateDepartmentSlugRequest(departmentId, "same-slug")),
                cancellationToken));

        // Assert
        Assert.True(result.IsSuccess);

        await ExecuteInDb(async dbContext =>
        {
            var department = await dbContext.Departments
                .FirstAsync(d => d.Id == DepartmentId.Create(departmentId).Value, cancellationToken);
            Assert.Equal("same-slug", department.Slug.Value);
            Assert.Equal("same-slug", department.Path.Value);
        });
    }

    [Fact]
    public async Task UpdateDepartmentSlug_should_not_affect_other_departments()
    {
        // Arrange
        var firstId = await CreateDepartment("First", "first");
        var secondId = await CreateDepartment("Second", "second");
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await ExecuteHandler(sut =>
            sut.Handle(
                new UpdateDepartmentSlugCommand(new UpdateDepartmentSlugRequest(firstId, "first-updated")),
                cancellationToken));

        // Assert
        Assert.True(result.IsSuccess);

        await ExecuteInDb(async dbContext =>
        {
            var first = await dbContext.Departments
                .FirstAsync(d => d.Id == DepartmentId.Create(firstId).Value, cancellationToken);
            var second = await dbContext.Departments
                .FirstAsync(d => d.Id == DepartmentId.Create(secondId).Value, cancellationToken);

            Assert.Equal("first-updated", first.Slug.Value);
            Assert.Equal("second", second.Slug.Value); // не тронут
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

    private async Task<T> ExecuteHandler<T>(Func<UpdateDepartmentSlugHandler, Task<T>> action)
    {
        await using var scope = Services.CreateAsyncScope();
        var sut = scope.ServiceProvider.GetRequiredService<UpdateDepartmentSlugHandler>();
        return await action(sut);
    }
}