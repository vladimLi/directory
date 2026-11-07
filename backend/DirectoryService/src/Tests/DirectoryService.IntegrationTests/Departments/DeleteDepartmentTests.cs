using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Departments.Commands.CreateDepartment;
using DirectoryService.Core.Departments.Commands.DeleteDepartment;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.IntegrationTests.Departments;

public class DeleteDepartmentTests(DirectoryTestWebFactory factory) : DirectoryBaseTests(factory)
{
    [Fact]
    public async Task DeleteDepartment_after_removing_links_should_succeed()
    {
        // Arrange: создаём департамент с локацией (связь DepartmentLocation существует)
        var departmentId = await CreateDepartment("Name", "slug");
        var cancellationToken = CancellationToken.None;
        var departmentGuid = DepartmentId.Create(departmentId).Value;

        // Удаляем связи напрямую через DbContext (в обход хендлера)
        await ExecuteInDb(async dbContext =>
        {
            var links = await dbContext.DepartmentLocation
                .Where(dl => dl.DepartmentId == departmentGuid)
                .ToListAsync(cancellationToken);

            Assert.NotEmpty(links); // убеждаемся, что связь действительно была

            dbContext.DepartmentLocation.RemoveRange(links);
            await dbContext.SaveChangesAsync(cancellationToken);
        });

        // Act: теперь пытаемся удалить департамент
        var result = await ExecuteHandler(sut =>
            sut.Handle(new DeleteDepartmentCommand(departmentId), cancellationToken));

        // Assert
        Assert.True(result.IsSuccess);

        await ExecuteInDb(async dbContext =>
        {
            Assert.False(await dbContext.Departments
                .AnyAsync(d => d.Id == departmentGuid, cancellationToken));
        });
    }

    [Fact]
    public async Task DeleteDepartment_non_existing_should_fail()
    {
        // Arrange
        var fakeId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await ExecuteHandler(sut =>
            sut.Handle(new DeleteDepartmentCommand(fakeId), cancellationToken));

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task DeleteDepartment_with_empty_guid_should_fail()
    {
        var cancellationToken = CancellationToken.None;

        var result = await ExecuteHandler(sut =>
            sut.Handle(new DeleteDepartmentCommand(Guid.Empty), cancellationToken));

        Assert.True(result.IsFailure);
    }

    private async Task<Guid> CreateDepartment(string name, string slug)
    {
        var locationId = await CreateLocation();
        var cancellationToken = CancellationToken.None;

        await using var scope = Services.CreateAsyncScope();
        var handler = scope.ServiceProvider.GetRequiredService<CreateDepartmentHandler>();
        var result = await handler.Handle(
            new CreateDepartmentCommand(new CreateDepartmentRequest(name, slug, null, [locationId])),
            cancellationToken);

        Assert.True(result.IsSuccess);
        return result.Value;
    }

    private async Task<Guid> CreateLocation()
    {
        return await ExecuteInDb(async dbContext =>
        {
            var location = Domain.Locations.Location.Create("name", "street", "city", "country");
            dbContext.Add(location.Value);
            await dbContext.SaveChangesAsync();
            return location.Value.Id.Value;
        });
    }

    private async Task<T> ExecuteHandler<T>(Func<DeleteDepartmentHandler, Task<T>> action)
    {
        await using var scope = Services.CreateAsyncScope();
        var sut = scope.ServiceProvider.GetRequiredService<DeleteDepartmentHandler>();
        return await action(sut);
    }
}