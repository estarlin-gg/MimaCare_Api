using Microsoft.EntityFrameworkCore;
using MimaCare.Infrastructure.Context;
using Npgsql;

namespace MimaCare.Presentation.Configuration;

/// <summary>
/// Arranque de la base de datos (migraciones + seeders) tolerante a fallos
/// transitorios de red, típicos de un PostgreSQL gestionado en la nube
/// (por ejemplo el pooler de Supabase, que puede rechazar conexiones de
/// forma puntual mientras el proyecto despierta o el pool se recicla).
/// </summary>
public static class DatabaseBootstrapper
{
    private const int MaxAttempts = 5;

    private static readonly TimeSpan Delay = TimeSpan.FromSeconds(5);

    public static async Task ApplyMigrationsAsync(
        MimaCareContext context,
        ILogger logger)
    {
        for (var attempt = 1; ; attempt++)
        {
            try
            {
                await context.Database.MigrateAsync();

                logger.LogInformation("Migraciones aplicadas correctamente.");

                return;
            }
            catch (PostgresException ex) when (ex.SqlState == "42P07")
            {
                // La base ya tenía tablas creadas por fuera de EF (por ejemplo al
                // aplicar MimaCare.infrastructure/migration_script.sql a mano).
                // Se marcan todas las migraciones definidas como aplicadas y se reintenta.
                logger.LogWarning(
                    "La base de datos ya contenía tablas sin historial de migraciones. " +
                    "Se marcan las migraciones como aplicadas.");

                await MarkMigrationsAsAppliedAsync(context);

                continue;
            }
            catch (Exception ex) when (attempt < MaxAttempts)
            {
                logger.LogWarning(
                    ex,
                    "Fallo aplicando migraciones (intento {Attempt}/{Max}). " +
                    "Reintentando en {Seconds}s...",
                    attempt,
                    MaxAttempts,
                    Delay.TotalSeconds);

                await Task.Delay(Delay);
            }
        }
    }

    /// <summary>
    /// Ejecuta una tarea de arranque (por ejemplo un seeder) reintentando ante
    /// fallos transitorios. Si se agotan los intentos, la excepción se propaga.
    /// </summary>
    public static async Task RunAsync(
        string name,
        Func<Task> action,
        ILogger logger)
    {
        for (var attempt = 1; ; attempt++)
        {
            try
            {
                await action();

                return;
            }
            catch (Exception ex) when (attempt < MaxAttempts)
            {
                logger.LogWarning(
                    ex,
                    "Fallo ejecutando {Name} (intento {Attempt}/{Max}). " +
                    "Reintentando en {Seconds}s...",
                    name,
                    attempt,
                    MaxAttempts,
                    Delay.TotalSeconds);

                await Task.Delay(Delay);
            }
        }
    }

    private static async Task MarkMigrationsAsAppliedAsync(MimaCareContext context)
    {
        var defined = context.Database.GetMigrations().ToList();

        var applied = (await context.Database.GetAppliedMigrationsAsync()).ToHashSet();

        var productVersion = typeof(DbContext).Assembly
            .GetName().Version?.ToString() ?? "10.0.7";

        foreach (var migration in defined.Where(m => !applied.Contains(m)))
        {
            await context.Database.ExecuteSqlRawAsync(
                "INSERT INTO \"__EFMigrationsHistory\" (\"MigrationId\", \"ProductVersion\") " +
                "VALUES ({0}, {1})",
                migration,
                productVersion);
        }
    }
}
