using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Database;
using DirectoryService.Core.Departments;
using DirectoryService.Core.Departments.Queries;
using DirectoryService.Core.Locations;
using DirectoryService.Core.Locations.Queries;
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
        var assembly = typeof(CoreDependencyInjection).Assembly;
        
        services.AddValidatorsFromAssembly(assembly);
        
        services.Scan(scan => scan.FromAssemblies(assembly)
            .AddClasses(classes => classes.AssignableToAny(
                typeof(ICommandHandler<,>),
                typeof(ICommandHandler<>)))
            .AsSelfWithInterfaces()
            .WithScopedLifetime());
        services.AddScoped<GetDepartmentByIdHandler>();
        services.AddScoped<GetLocationByIdHandler>();
        return services;
    }
}