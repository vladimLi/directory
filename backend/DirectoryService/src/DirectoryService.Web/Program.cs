using DirectoryService.Infrastructure.Postgres;
using DirectoryService.Web;
using DirectoryService.Web.Middlewares;
using Scalar.AspNetCore;
using Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddProgramDependencies();

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseExceptionMiddleware();

app.MapControllers();

app.MapHealthChecks("/health");

if (!app.Environment.IsProduction())
{
    app.MapOpenApi();              // /openapi/v1.json
    app.MapScalarApiReference(); // /scalar/v1
}

await app.RunAsync();