using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Serilog;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using Neocare.Infrastructure.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

// Configurar Serilog para logging estruturado
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/neocare-.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "NeoCare")
    .CreateLogger();

try
{
    Log.Information("Iniciando aplicação NeoCare");

    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

    // Configurar Health Checks
    builder.Services
        .AddHealthChecks()
        .AddCheck<DatabaseHealthCheck>("database")
        .AddCheck<ExternalServiceHealthCheck>("external_services")
        .AddCheck("api_health", () => HealthCheckResult.Healthy("API está saudável"));

    // Configurar OpenTelemetry para tracing e métricas
    builder.Services.AddOpenTelemetry()
        .WithTracing(tracing =>
        {
            tracing
                .AddAspNetCoreInstrumentation()
                .AddConsoleExporter();
        })
        .WithMetrics(metrics =>
        {
            metrics
                .AddAspNetCoreInstrumentation()
                .AddRuntimeInstrumentation()
                .AddConsoleExporter();
        });

    builder.Services.AddHttpClient();

    builder.Services.AddRazorPages()
        .AddRazorPagesOptions(options =>
        {
            options.Conventions.AddPageRoute("/StressEntries", "/stress");
            options.Conventions.AddPageRoute("/CreateStressEntry", "/stress/new");
        });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "NeoCare API",
            Version = "v1",
            Description = "API para gerenciamento de registros de estresse mental",
            Contact = new OpenApiContact
            {
                Name = "Equipe NeoCare",
                Email = "contato@neocare.com"
            }
        });
    });

    builder.Services.AddMemoryCache();
    builder.Services.AddResponseCaching();
    builder.Services.AddSingleton<Neocare.Domain.Interfaces.IStressEntryRepository, Neocare.Infrastructure.Repositories.InMemoryStressEntryRepository>();
    builder.Services.AddScoped<Neocare.Application.Services.StressEntryService>();

    var app = builder.Build();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "NeoCare API V1");
            c.RoutePrefix = "api/docs";
        });
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseAuthorization();
    app.UseResponseCaching();

    // Adicionar Health Checks Endpoint
    app.MapHealthChecks("/health");
    app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
    {
        Predicate = _ => true
    });

    app.MapRazorPages();

    app.MapGet("/api/stress", async (Neocare.Application.Services.StressEntryService service, [AsParameters] Neocare.Application.DTOs.SearchParams searchParams) =>
    {
        var result = await service.SearchStressEntries(searchParams);
        var baseUrl = $"/api/stress";

        return Results.Ok(new
        {
            Data = result.Items,
            Links = new
            {
                Self = $"{baseUrl}?page={searchParams.Page}&pageSize={searchParams.PageSize}&sortBy={searchParams.SortBy}&sortDirection={searchParams.SortDirection}",
                First = $"{baseUrl}?page=1&pageSize={searchParams.PageSize}&sortBy={searchParams.SortBy}&sortDirection={searchParams.SortDirection}",
                Previous = searchParams.Page > 1
                    ? $"{baseUrl}?page={searchParams.Page - 1}&pageSize={searchParams.PageSize}&sortBy={searchParams.SortBy}&sortDirection={searchParams.SortDirection}"
                    : null,
                Next = searchParams.Page < result.TotalPages
                    ? $"{baseUrl}?page={searchParams.Page + 1}&pageSize={searchParams.PageSize}&sortBy={searchParams.SortBy}&sortDirection={searchParams.SortDirection}"
                    : null,
                Last = $"{baseUrl}?page={result.TotalPages}&pageSize={searchParams.PageSize}&sortBy={searchParams.SortBy}&sortDirection={searchParams.SortDirection}"
            },
            Meta = new
            {
                result.CurrentPage,
                result.TotalPages,
                result.TotalItems,
                PageSize = searchParams.PageSize,
                SortBy = searchParams.SortBy,
                SortDirection = searchParams.SortDirection
            }
        });
    });

    app.MapPost("/api/stress", async (Neocare.Application.Services.StressEntryService service, Neocare.Application.DTOs.CreateStressEntryDto entry) =>
    {
        var result = await service.CreateAsync(entry);
        return Results.Created($"/api/stress/{result.Id}", new
        {
            Data = result,
            Links = new
            {
                Self = $"/api/stress/{result.Id}",
                Collection = "/api/stress"
            }
        });
    });

    app.MapPut("/api/stress/{id}", async (Neocare.Application.Services.StressEntryService service, Guid id, Neocare.Application.DTOs.StressEntryDto entry) =>
    {
        var result = await service.UpdateStressEntry(id, entry);
        if (result == null) return Results.NotFound();

        return Results.Ok(new
        {
            Data = result,
            Links = new
            {
                Self = $"/api/stress/{result.Id}",
                Collection = "/api/stress"
            }
        });
    });

    app.MapDelete("/api/stress/{id}", async (Neocare.Application.Services.StressEntryService service, Guid id) =>
    {
        var result = await service.DeleteStressEntry(id);
        return result ? Results.NoContent() : Results.NotFound();
    });

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Erro crítico na aplicação");
    Environment.Exit(1);
}
finally
{
    Log.CloseAndFlush();
}

public class PaginationParams
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SortBy { get; set; } = "date";
}

// Tornar Program público para testes de integração
public partial class Program { }
