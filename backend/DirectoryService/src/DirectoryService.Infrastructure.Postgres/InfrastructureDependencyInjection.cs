using DirectoryService.Core.Locations;
using DirectoryService.Infrastructure.Postgres.Database;
using DirectoryService.Infrastructure.Postgres.Locations;
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
        
        services.AddScoped<ILocationsRepository, EfCoreLocationsRepository>();
        //services.AddScoped<ILocationsRepository, NpgSqlLocationsRepository>();
        return services;
    }
}