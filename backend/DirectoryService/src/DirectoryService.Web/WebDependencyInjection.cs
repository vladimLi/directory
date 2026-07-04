using DirectoryService.Core;

namespace DirectoryService.Web;

public static class WebDependencyInjection
{
    public static IServiceCollection AddProgramDependencies(this IServiceCollection services)
    {
        return services.AddWebDependencies()
            .AddApplication();
    }

    private static IServiceCollection AddWebDependencies(this IServiceCollection services)
    {
        services.AddOpenApi();
        services.AddControllers();
        
        return services;
    }
}