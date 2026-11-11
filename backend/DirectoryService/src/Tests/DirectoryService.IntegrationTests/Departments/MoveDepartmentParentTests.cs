using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Departments.Commands.CreateDepartment;
using DirectoryService.Core.Departments.Commands.MoveDepartmentParent;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.IntegrationTests.Departments;

public class MoveDepartmentParentTests(DirectoryTestWebFactory factory) : DirectoryBaseTests(factory)
{
    private readonly CancellationToken _ct = CancellationToken.None;

    [Fact]
    public async Task MoveParent_with_descendants_updates_subtree_paths_and_depths()
    {
        var hqId = await CreateDepartment("HQ", "hq");
        var itId = await CreateDepartment("IT", "it", hqId);
        var devTeamId = await CreateDepartment("DevTeam", "devteam", itId);
        var qaId = await CreateDepartment("QA", "qa", devTeamId);

        var result = await Move(devTeamId, hqId);

        Assert.True(result.IsSuccess);
        Assert.Equal(devTeamId, result.Value.Id);
        Assert.Equal(hqId, result.Value.ParentId);
        Assert.Equal("hq/devteam", result.Value.Path);
        Assert.Equal(1, result.Value.Depth);

        await ExecuteInDb(async db =>
        {
            var dev = await db.Departments
                .IgnoreQueryFilters()
                .SingleAsync(d => d.Id == DepartmentId.Create(devTeamId).Value, _ct);
            var qa = await db.Departments
                .IgnoreQueryFilters()
                .SingleAsync(d => d.Id == DepartmentId.Create(qaId).Value, _ct);

            Assert.Equal(hqId, dev.ParentId!.Value);
            Assert.Equal("hq/devteam", dev.Path.Value);
            Assert.Equal(1, dev.Depth);

            Assert.Equal(devTeamId, qa.ParentId!.Value);
            Assert.Equal("hq/devteam/qa", qa.Path.Value);
            Assert.Equal(2, qa.Depth);
        });
    }

    [Fact]
    public async Task MoveParent_subtree_with_nested_child_moves_whole_subtree()
    {
        var hqId = await CreateDepartment("HQ", "hq");
        var salesId = await CreateDepartment("Sales", "sales", hqId);
        var regionalId = await CreateDepartment("Regional", "regional", salesId);
        var southId = await CreateDepartment("South", "south", regionalId);

        var result = await Move(regionalId, hqId);

        Assert.True(result.IsSuccess);
        Assert.Equal("hq/regional", result.Value.Path);
        Assert.Equal(1, result.Value.Depth);

        await ExecuteInDb(async db =>
        {
            var regional = await db.Departments.SingleAsync(d => d.Id == DepartmentId.Create(regionalId).Value, _ct);
            var south = await db.Departments.SingleAsync(d => d.Id == DepartmentId.Create(southId).Value, _ct);

            Assert.Equal("hq/regional", regional.Path.Value);
            Assert.Equal(1, regional.Depth);
            Assert.Equal("hq/regional/south", south.Path.Value);
            Assert.Equal(2, south.Depth);
        });
    }

    [Fact]
    public async Task MoveParent_to_root_makes_department_root()
    {
        var hqId = await CreateDepartment("HQ", "hq");
        var salesId = await CreateDepartment("Sales", "sales", hqId);
        var regionalId = await CreateDepartment("Regional", "regional", salesId);

        var result = await Move(regionalId, null);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value.ParentId);
        Assert.Equal("regional", result.Value.Path);
        Assert.Equal(0, result.Value.Depth);

        await ExecuteInDb(async db =>
        {
            var regional = await db.Departments.SingleAsync(d => d.Id == DepartmentId.Create(regionalId).Value, _ct);
            Assert.Null(regional.ParentId);
            Assert.Equal("regional", regional.Path.Value);
            Assert.Equal(0, regional.Depth);
        });
    }

    [Fact]
    public async Task MoveParent_to_descendant_returns_cycle_conflict()
    {
        var hqId = await CreateDepartment("HQ", "hq");
        var itId = await CreateDepartment("IT", "it", hqId);
        var devTeamId = await CreateDepartment("DevTeam", "devteam", itId);

        var result = await Move(itId, devTeamId);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Error, e => e.Type == ErrorType.CONFLICT && string.Equals(e.Code, "department.move.cycle", StringComparison.Ordinal));
    }

    [Fact]
    public async Task MoveParent_to_self_returns_validation_error()
    {
        var hqId = await CreateDepartment("HQ", "hq");

        var result = await Move(hqId, hqId);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Error, e => e.Type == ErrorType.VALIDATION && string.Equals(e.Code, "department.move.parent_is_self", StringComparison.Ordinal));
    }

    [Fact]
    public async Task MoveParent_to_soft_deleted_parent_returns_conflict()
    {
        var hqId = await CreateDepartment("HQ", "hq");
        var itId = await CreateDepartment("IT", "it", hqId);
        var devTeamId = await CreateDepartment("DevTeam", "devteam", itId);

        await ExecuteInDb(async db =>
        {
            await db.Departments
                .Where(d => d.Id == DepartmentId.Create(itId).Value)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(d => d.IsDeleted, true)
                    .SetProperty(d => d.DeletedAt, DateTime.UtcNow), _ct);
        });

        var result = await Move(devTeamId, itId);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Error, e => e.Type == ErrorType.CONFLICT && string.Equals(e.Code, "department.move.parent_deleted", StringComparison.Ordinal));
    }

    [Fact]
    public async Task MoveParent_when_parent_not_found_returns_not_found()
    {
        var hqId = await CreateDepartment("HQ", "hq");

        var result = await Move(hqId, Guid.NewGuid());

        Assert.True(result.IsFailure);
        Assert.Contains(result.Error, e => e.Type == ErrorType.NOT_FOUND && string.Equals(e.Code, "department.parent.not_found", StringComparison.Ordinal));
    }

    [Fact]
    public async Task MoveParent_when_department_not_found_returns_not_found()
    {
        var result = await Move(Guid.NewGuid(), null);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Error, e => e.Type == ErrorType.NOT_FOUND && string.Equals(e.Code, "department.not_found", StringComparison.Ordinal));
    }

    [Fact]
    public async Task MoveParent_with_same_parent_is_noop_and_does_not_change_updated_at()
    {
        var hqId = await CreateDepartment("HQ", "hq");
        var itId = await CreateDepartment("IT", "it", hqId);
        var devTeamId = await CreateDepartment("DevTeam", "devteam", itId);

        var before = await ExecuteInDb(async db =>
            await db.Departments.AsNoTracking()
                .Where(d => d.Id == DepartmentId.Create(devTeamId).Value)
                .Select(d => new { d.UpdatedAt, d.Path.Value, d.Depth, d.ParentId })
                .SingleAsync(_ct));

        var result = await Move(devTeamId, itId);

        Assert.True(result.IsSuccess);
        Assert.Equal(before.UpdatedAt, result.Value.UpdatedAt);
        Assert.Equal(itId, result.Value.ParentId);
        Assert.Equal("hq/it/devteam", result.Value.Path);
        Assert.Equal(2, result.Value.Depth);

        var after = await ExecuteInDb(async db =>
            await db.Departments.AsNoTracking()
                .Where(d => d.Id == DepartmentId.Create(devTeamId).Value)
                .Select(d => new { d.UpdatedAt })
                .SingleAsync(_ct));

        Assert.Equal(before.UpdatedAt, after.UpdatedAt);
    }

    [Fact]
    public async Task MoveParent_large_subtree_updates_with_single_bulk_update_command()
    {
        var rootA = await CreateDepartment("RootA", "roota");
        var rootB = await CreateDepartment("RootB", "rootb");
        var moving = await CreateDepartment("Moving", "moving", rootA);

        var currentParent = moving;
        for (var i = 0; i < 60; i++)
        {
            currentParent = await CreateDepartment($"Node{i}", $"node{i}", currentParent);
        }

        var leaf = currentParent;

        var connectionString = await ExecuteInDb(db => Task.FromResult(db.Database.GetConnectionString()!));
        var interceptor = new CountingUpdateInterceptor();

        await using var scope = Services.CreateAsyncScope();
        var loggerFactory = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Logging.ILoggerFactory>();

        await using (var db = new InterceptedAppDbContext(connectionString, interceptor))
        {
            var repo = new DirectoryService.Infrastructure.Postgres.Departments.EfCoreDepartmentsRepository(
                db,
                loggerFactory.CreateLogger<DirectoryService.Infrastructure.Postgres.Departments.EfCoreDepartmentsRepository>());
            var txManager = new DirectoryService.Infrastructure.Postgres.Database.TransactionManager(
                db,
                loggerFactory.CreateLogger<DirectoryService.Infrastructure.Postgres.Database.TransactionManager>(),
                loggerFactory);
            var validator = new MoveDepartmentParentValidator();
            var handlerLogger = loggerFactory.CreateLogger<MoveDepartmentParentHandler>();
            var handler = new MoveDepartmentParentHandler(repo, txManager, validator, handlerLogger);

            var result = await handler.Handle(new MoveDepartmentParentCommand(moving, rootB), _ct);

            Assert.True(result.IsSuccess);
            Assert.Equal("rootb/moving", result.Value.Path);
            Assert.Equal(1, result.Value.Depth);
        }

        Assert.Equal(1, interceptor.DepartmentUpdateCount);

        await ExecuteInDb(async db =>
        {
            var moved = await db.Departments.AsNoTracking()
                .SingleAsync(d => d.Id == DepartmentId.Create(moving).Value, _ct);
            var deepLeaf = await db.Departments.AsNoTracking()
                .SingleAsync(d => d.Id == DepartmentId.Create(leaf).Value, _ct);

            Assert.Equal("rootb/moving", moved.Path.Value);
            Assert.Equal(1, moved.Depth);
            Assert.StartsWith("rootb/moving/", deepLeaf.Path.Value, StringComparison.Ordinal);
            Assert.Equal(1 + 60, deepLeaf.Depth);
        });
    }

    private async Task<Result<MoveDepartmentParentResponse, Errors>> Move(Guid departmentId, Guid? parentId)
    {
        await using var scope = Services.CreateAsyncScope();
        var handler = scope.ServiceProvider.GetRequiredService<MoveDepartmentParentHandler>();
        return await handler.Handle(new MoveDepartmentParentCommand(departmentId, parentId), _ct);
    }

    private async Task<Guid> CreateDepartment(string name, string slug, Guid? parentId = null)
    {
        var locationId = await CreateLocation();

        await using var scope = Services.CreateAsyncScope();
        var handler = scope.ServiceProvider.GetRequiredService<CreateDepartmentHandler>();

        var result = await handler.Handle(
            new CreateDepartmentCommand(new CreateDepartmentRequest(
                name,
                slug,
                parentId,
                [locationId])),
            _ct);

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

    private sealed class CountingUpdateInterceptor : DbCommandInterceptor
    {
        public int DepartmentUpdateCount { get; private set; }

        public override InterceptionResult<int> NonQueryExecuting(
            System.Data.Common.DbCommand command,
            CommandEventData eventData,
            InterceptionResult<int> result)
        {
            if (IsDepartmentSubtreeUpdate(command))
                DepartmentUpdateCount++;

            return base.NonQueryExecuting(command, eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(
            System.Data.Common.DbCommand command,
            CommandEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            if (IsDepartmentSubtreeUpdate(command))
                DepartmentUpdateCount++;

            return base.NonQueryExecutingAsync(command, eventData, result, cancellationToken);
        }

        private static bool IsDepartmentSubtreeUpdate(System.Data.Common.DbCommand command)
        {
            var text = command.CommandText;
            return text.Contains("UPDATE departments", StringComparison.OrdinalIgnoreCase)
                   && text.Contains("path <@", StringComparison.OrdinalIgnoreCase);
        }
    }

    private sealed class InterceptedAppDbContext(string connectionString, DbCommandInterceptor interceptor)
        : DirectoryService.Infrastructure.Postgres.AppDbContext(connectionString)
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.AddInterceptors(interceptor);
        }
    }
}
