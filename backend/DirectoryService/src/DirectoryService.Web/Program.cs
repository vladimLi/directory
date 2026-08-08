using System.Globalization;
using DirectoryService.Infrastructure.Postgres;
using DirectoryService.Web;
using DirectoryService.Web.Middlewares;
using Scalar.AspNetCore;
using Serilog;
using Shared;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting web application...");
    
    var builder = WebApplication.CreateBuilder(args);

    builder.Services
        .AddInfrastructure(builder.Configuration)
        .AddProgramDependencies(builder.Configuration);

    builder.Services.AddHealthChecks();

    var app = builder.Build();

    app.UseExceptionMiddleware();

    app.MapControllers();

    app.MapHealthChecks("/health");

    if (!app.Environment.IsProduction())
    {
        app.UseSerilogRequestLogging();
        app.MapOpenApi();              // /openapi/v1.json
        app.MapScalarApiReference(); // /scalar/v1
    }

    await app.RunAsync();
}
catch (Exception ex) when (ex is not OutOfMemoryException and not StackOverflowException)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}
