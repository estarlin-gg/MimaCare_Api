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

var builder = WebApplication.CreateBuilder(args);


// DATABASE

builder.Services.AddDbContext<MimaCareContext>(options =>
{
    //options.UseSqlite(
    //    builder.Configuration.GetConnectionString(
    //        "DefaultConnection"));

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


// REPOSITORIES

builder.Services.AddScoped<IClinicRepository, ClinicRepository>();

builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();

builder.Services.AddScoped<IPatientRepository,PatientRepository>();

builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();

builder.Services.AddScoped<IDoctorScheduleRepository, DoctorScheduleRepository>();

builder.Services.AddScoped<ISpecialtyRepository, SpecialtyRepository>();

builder.Services.AddScoped<IRevokedTokenRepository, RevokedTokenRepository>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped(
    typeof(IRepository<>),
    typeof(Repository<>)
);


// HTTP CONTEXT

builder.Services.AddHttpContextAccessor();


// CONTROLLERS

builder.Services.AddControllers();


// OPEN API

builder.Services.AddOpenApi();


var app = builder.Build();


// MIGRATE DATABASE

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<MimaCareContext>();

    try
    {
        await context.Database.MigrateAsync();
    }
    catch (PostgresException ex) when (ex.SqlState == "42P07")
    {
        // Tables already exist — mark InitialCreate as applied and retry
        await context.Database.ExecuteSqlRawAsync(
            "INSERT INTO \"__EFMigrationsHistory\" (\"MigrationId\", \"ProductVersion\") " +
            "VALUES ('20260523164337_InitialCreate', '10.0.7')");
        await context.Database.MigrateAsync();
    }
}


// SEED ROLES

using (var scope = app.Services.CreateScope())
{
    var roleManager =
        scope.ServiceProvider
            .GetRequiredService<
                RoleManager<IdentityRole<Guid>>>();

    await RoleSeeder.SeedAsync(roleManager);
}

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<MimaCareContext>();

    await SpecialtySeeder.SeedAsync(context);
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

app.UseAuthentication();

app.UseAuthorization();


// ENDPOINTS

app.MapControllers();

app.Run();