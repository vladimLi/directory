using DirectoryService.Core.Database;
using DirectoryService.Core.Departments;
using DirectoryService.Core.Locations;
using DirectoryService.Core.Relationships;
using DirectoryService.Infrastructure.Postgres.Database;
using DirectoryService.Infrastructure.Postgres.Departments;
using DirectoryService.Infrastructure.Postgres.Locations;
using DirectoryService.Infrastructure.Postgres.Relationships;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.Infrastructure.Postgres;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("AppDbContext")!;
        
        services.AddScoped<AppDbContext>(_ => new AppDbContext(connectionString));
        
        services.AddSingleton<IDbConnectionFactory, NpgsqlConnectionFactory>();
        services.AddScoped<ITransactionManager, TransactionManager>();
        
        services.AddScoped<ILocationsRepository, EfCoreLocationsRepository>();
        services.AddScoped<IDepartmentsRepository, EfCoreDepartmentsRepository>();
        services.AddScoped<IDepartmentLocationRepository, EfCoreDepartmentLocationRepository>();
        //services.AddScoped<ILocationsRepository, NpgSqlLocationsRepository>();
        return services;
    }
}