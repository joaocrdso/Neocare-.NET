using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Neocare.Infrastructure.Data;
using Neocare.Infrastructure.Repositories;
using Neocare.Infrastructure.Persistence;
using Neocare.Application.Services;
using Neocare.Application.Interfaces;
using Neocare.Domain.Interfaces;
using Neocare.Infrastructure.HealthChecks;
using Neocare.Infrastructure.Middleware;
using Swashbuckle.AspNetCore.SwaggerGen;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/neocare-.log", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "Neocare")
    .CreateLogger();

try
{
    Log.Information("Iniciando aplicação Neocare");

    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]!);

    builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

    builder.Services.AddAuthorization();

    builder.Services.AddDbContext<NeocareDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddIdentity<IdentityUser, IdentityRole>()
        .AddEntityFrameworkStores<NeocareDbContext>()
        .AddDefaultTokenProviders();

    builder.Services.Configure<JwtSettings>(jwtSettings);
    builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));

    builder.Services.AddScoped<IPatientRepository, PatientRepository>();
    builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
    builder.Services.AddScoped<IHealthProfessionalRepository, HealthProfessionalRepository>();
    builder.Services.AddScoped<ITreatmentRepository, TreatmentRepository>();
    builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
    builder.Services.AddSingleton<IStressEntryRepository, InMemoryStressEntryRepository>();

    builder.Services.AddScoped<IPatientService, PatientService>();
    builder.Services.AddScoped<IAppointmentService, AppointmentService>();
    builder.Services.AddScoped<IHealthProfessionalService, HealthProfessionalService>();
    builder.Services.AddScoped<ITreatmentService, TreatmentService>();
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<StressEntryService>();

    builder.Services.AddMemoryCache();
    builder.Services.AddSingleton<MongoDbContext>();

    builder.Services
        .AddHealthChecks()
        .AddSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")!, name: "sqlserver")
        .AddMongoDb(builder.Configuration["MongoDbSettings:ConnectionString"]!, name: "mongodb");

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddRazorPages();

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<NeocareDbContext>();
        context.Database.Migrate();
    }

    app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Neocare API V1");
        });
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.MapHealthChecks("/health");

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    app.MapRazorPages();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Aplicação encerrada com erro");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program { }
