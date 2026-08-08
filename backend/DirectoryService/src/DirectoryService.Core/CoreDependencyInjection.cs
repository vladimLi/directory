using DirectoryService.Core.Departments;
using DirectoryService.Core.Locations;
using DirectoryService.Core.Relationships;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Exceptions;

namespace DirectoryService.Core;

public static class CoreDependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddValidatorsFromAssembly(typeof(CoreDependencyInjection).Assembly)
            .AddSerilogLogging(configuration);
        

        services.AddScoped<ILocationsService, LocationsService>();
        services.AddScoped<IDepartmentsService, DepartmentsService>();
        services.AddScoped<IDepartmentLocationService, DepartmentLocationService>();
        
        return services;
    }
    private static IServiceCollection AddSerilogLogging(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSerilog((sp, lc) => lc
            .ReadFrom.Configuration(configuration)
            .ReadFrom.Services(sp)
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .Enrich.WithProperty("ServiceName", "DirectoryService"));
        
        return services;
    }
}