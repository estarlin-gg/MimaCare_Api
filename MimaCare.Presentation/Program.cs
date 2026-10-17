using FluentValidation;
using FluentValidation.AspNetCore;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MimaCare.Application.Common;
using MimaCare.Application.Interface;
using MimaCare.Application.Mappers;
using MimaCare.Application.Services;
using MimaCare.Application.Validators.Clinic;
using MimaCare.Domain.Entities;
using MimaCare.Infrastructure.Context;
using MimaCare.Infrastructure.Repositories;
using MimaCare.Infrastructure.Repositories.Abstraction;
using MimaCare.Infrastructure.Seed;
using Scalar.AspNetCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Npgsql;
using MimaCare.Presentation.Middlewares;
using MimaCare.Domain.Interfaces;
using MimaCare.Infrastructure.Persistence;
using MimaCare.Presentation.Configuration;

var builder = WebApplication.CreateBuilder(args);


// CONFIGURATION (local PostgreSQL en Development, Supabase en Production)

// Resuelve la cadena de conexión (ConnectionStrings:DefaultConnection, DATABASE_URL
// o SUPABASE_DB_*) y la clave Jwt:Key (Jwt__Key o SUPABASE_JWT_SECRET).
AppConfiguration.Apply(builder.Configuration, builder.Environment);


// DATABASE

builder.Services.AddDbContext<MimaCareContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString(
            "DefaultConnection"));
});


// IDENTITY

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireDigit = false;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddEntityFrameworkStores<MimaCareContext>()
    .AddDefaultTokenProviders();


// JWT

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;

        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            builder.Configuration["Jwt:Key"]!
                        )
                    ),

                ClockSkew = TimeSpan.Zero,

                RoleClaimType = ClaimTypes.Role
            };

        // Reject JWTs revoked through the logout endpoint.
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var principal = context.Principal;

                if (principal == null)
                {
                    return;
                }

                var jti =
                    principal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value
                    ?? principal.FindFirst(ClaimTypes.SerialNumber)?.Value;

                if (string.IsNullOrEmpty(jti))
                {
                    // Legacy token issued before revocation support.
                    return;
                }

                var revokedTokens = context.HttpContext.RequestServices
                    .GetRequiredService<IRevokedTokenRepository>();

                if (await revokedTokens.IsRevokedAsync(jti))
                {
                    context.Fail("Token has been revoked.");
                }
            }
        };
    });
builder.Services.AddAuthorization();


// PASSWORD RESET OPTIONS

builder.Services.Configure<PasswordResetOptions>(options =>
{
    // No email provider is configured: expose reset tokens in Development only.
    options.ExposeResetToken = builder.Environment.IsDevelopment();
});


// MAPSTER

TypeAdapterConfig.GlobalSettings.Scan(
    typeof(ClinicMapper).Assembly
);


// FLUENT VALIDATION

builder.Services
    .AddFluentValidationAutoValidation();

builder.Services
    .AddValidatorsFromAssemblyContaining<
        RegisterClinicDtoValidator>();


// SERVICES

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IDoctorService, DoctorService>();

builder.Services.AddScoped<IAppointmentService, AppointmentService>();

builder.Services.AddScoped<IPatientService, PatientService>();

builder.Services.AddScoped<IDoctorScheduleService, DoctorScheduleService>();

builder.Services.AddScoped<ISpecialtyService, SpecialtyService>();

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<ISubscriptionPlanService, SubscriptionPlanService>();

builder.Services.AddScoped<IBranchService, BranchService>();


// REPOSITORIES

builder.Services.AddScoped<IClinicRepository, ClinicRepository>();

builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();

builder.Services.AddScoped<IPatientRepository,PatientRepository>();

builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();

builder.Services.AddScoped<IDoctorScheduleRepository, DoctorScheduleRepository>();

builder.Services.AddScoped<ISpecialtyRepository, SpecialtyRepository>();

builder.Services.AddScoped<IRevokedTokenRepository, RevokedTokenRepository>();

builder.Services.AddScoped<ISubscriptionPlanRepository, SubscriptionPlanRepository>();

builder.Services.AddScoped<IBranchRepository, BranchRepository>();

builder.Services.AddScoped<IDoctorBranchRepository, DoctorBranchRepository>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped(
    typeof(IRepository<>),
    typeof(Repository<>)
);


// HTTP CONTEXT

builder.Services.AddHttpContextAccessor();


// CONTROLLERS

builder.Services.AddControllers();


// CORS (opcional). Se habilita solo si existe Cors:AllowedOrigins
// (lista separada por comas), por ejemplo:
//   Cors__AllowedOrigins=https://mi-frontend.vercel.app,http://localhost:3000

var allowedOrigins = builder.Configuration["Cors:AllowedOrigins"]?
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

if (allowedOrigins is { Length: > 0 })
{
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy => policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
    });
}


// OPEN API

builder.Services.AddOpenApi();


var app = builder.Build();


// MIGRATE DATABASE

var startupLogger = app.Services
    .GetRequiredService<ILoggerFactory>()
    .CreateLogger("MimaCare.Startup");

startupLogger.LogInformation(
    "Entorno {Environment} | Base de datos {Database}",
    app.Environment.EnvironmentName,
    AppConfiguration.DescribeTarget(
        app.Configuration.GetConnectionString("DefaultConnection")!));

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<MimaCareContext>();

    await DatabaseBootstrapper.ApplyMigrationsAsync(context, startupLogger);
}


// SEED ROLES

using (var scope = app.Services.CreateScope())
{
    var roleManager =
        scope.ServiceProvider
            .GetRequiredService<
                RoleManager<IdentityRole<Guid>>>();

    await DatabaseBootstrapper.RunAsync(
        "RoleSeeder",
        () => RoleSeeder.SeedAsync(roleManager),
        startupLogger);
}

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<MimaCareContext>();

    await DatabaseBootstrapper.RunAsync(
        "SpecialtySeeder",
        () => SpecialtySeeder.SeedAsync(context),
        startupLogger);
}

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<MimaCareContext>();

    await DatabaseBootstrapper.RunAsync(
        "SubscriptionPlanSeeder",
        () => SubscriptionPlanSeeder.SeedAsync(context),
        startupLogger);
}


// DEVELOPMENT

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference();
}


// MIDDLEWARE

// NOTE: UseHttpsRedirection() was removed: the 307 redirect to the HTTPS port
// makes clients drop the Authorization header and fail with 401 on HTTP.

app.UseExceptionMiddleware();

if (allowedOrigins is { Length: > 0 })
{
    app.UseCors();
}

app.UseAuthentication();

app.UseAuthorization();


// ENDPOINTS

app.MapGet("/health", async (MimaCareContext context) =>
{
    var canConnect = await context.Database.CanConnectAsync();

    return canConnect
        ? Results.Ok(new { status = "ok", database = "up" })
        : Results.Json(new { status = "degraded", database = "down" }, statusCode: 503);
});

app.MapControllers();

app.Run();