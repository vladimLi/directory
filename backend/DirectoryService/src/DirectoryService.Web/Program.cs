using DirectoryService.Infrastructure.Postgres;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddControllers();

builder.Services.AddScoped<AppDbContext>(_ =>
    new AppDbContext(builder.Configuration.GetConnectionString("AppDbContext")!));

builder.Services.AddHealthChecks();

builder.Services.AddRouting(options => options.LowercaseUrls = true);

var app = builder.Build();

app.MapControllers();

app.MapHealthChecks("/health");

if (!app.Environment.IsProduction())
{
    app.MapOpenApi();              // /openapi/v1.json
    app.MapScalarApiReference(); // /scalar/v1
}


await app.RunAsync();