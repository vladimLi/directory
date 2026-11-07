using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Departments.Commands.CreateDepartment;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Infrastructure.Postgres.SoftDelete;
using DirectoryService.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.IntegrationTests.Departments;

public class SoftDeleteDepartmentCleanupTests(DirectoryTestWebFactory factory) : DirectoryBaseTests(factory)
{
    [Fact]
    public async Task Cleanup_removes_only_old_soft_deleted()
    {
        var ct = CancellationToken.None;

        // Arrange: три департамента
        var activeId = await CreateDepartment("Active", "active");
        var freshDeletedId = await CreateDepartment("Fresh", "fresh");
        var oldDeletedId = await CreateDepartment("Old", "old");

        await ExecuteInDb(async db =>
        {
            // свежий удалённый — удалён вчера (в пределах Retention=30 дней)
            await db.Departments
                .Where(d => d.Id == DepartmentId.Create(freshDeletedId).Value)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(d => d.IsDeleted, true)
                    .SetProperty(d => d.DeletedAt, DateTime.UtcNow.AddDays(-1)), ct);

            // старый удалённый — 31 день назад (за пределами Retention)
            await db.Departments
                .Where(d => d.Id == DepartmentId.Create(oldDeletedId).Value)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(d => d.IsDeleted, true)
                    .SetProperty(d => d.DeletedAt, DateTime.UtcNow.AddDays(-31)), ct);
        });

        // Act
        await using var scope = Services.CreateAsyncScope();
        var processor = scope.ServiceProvider.GetRequiredService<SoftDeleteCleanupProcessor>();
        await processor.RunAsync(ct);

        // Assert
        await ExecuteInDb(async db =>
        {
            var remaining = await db.Departments
                .IgnoreQueryFilters()
                .Select(d => d.Id)
                .ToListAsync(ct);

            Assert.Contains(DepartmentId.Create(activeId).Value, remaining);
            Assert.Contains(DepartmentId.Create(freshDeletedId).Value, remaining);
            Assert.DoesNotContain(DepartmentId.Create(oldDeletedId).Value, remaining);
        });
    }

    private async Task<Guid> CreateDepartment(string name, string slug)
    {
        var locationId = await CreateLocation();
        var ct = CancellationToken.None;

        await using var scope = Services.CreateAsyncScope();
        var handler = scope.ServiceProvider.GetRequiredService<CreateDepartmentHandler>();
        var result = await handler.Handle(
            new CreateDepartmentCommand(new CreateDepartmentRequest(name, slug, null, [locationId])),
            ct);

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
}