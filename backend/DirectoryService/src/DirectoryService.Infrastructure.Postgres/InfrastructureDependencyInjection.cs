using DirectoryService.Core.Locations;
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
        services.AddScoped<ILocationsRepository, LocationsRepository>();
        return services;
    }
}