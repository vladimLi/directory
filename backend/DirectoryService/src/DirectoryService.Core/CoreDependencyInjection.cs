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
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services
            .AddValidatorsFromAssembly(typeof(CoreDependencyInjection).Assembly);
        

        services.AddScoped<ILocationsService, LocationsService>();
        services.AddScoped<IDepartmentsService, DepartmentsService>();
        services.AddScoped<IDepartmentLocationService, DepartmentLocationService>();
        
        return services;
    }
}