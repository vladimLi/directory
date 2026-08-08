using DirectoryService.Core;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Web;

public static class WebDependencyInjection
{
    public static IServiceCollection AddProgramDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddWebDependencies()
            .AddApplication(configuration);
    }

    private static IServiceCollection AddWebDependencies(this IServiceCollection services)
    {
        services.AddOpenApi();
        services.AddControllers();
        //дефолтные настройки
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });
        return services;
    }
}