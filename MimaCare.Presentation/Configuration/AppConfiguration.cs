using Npgsql;

namespace MimaCare.Presentation.Configuration;

/// <summary>
/// Normaliza la configuración de la aplicación para que la API funcione igual
/// en local (PostgreSQL local) y en la nube (Supabase / Render).
///
/// Orden de resolución de la cadena de conexión:
///   1. ConnectionStrings:DefaultConnection  (appsettings.* o variable de entorno)
///   2. DATABASE_URL                         (formato URI: postgresql://usuario:pass@host:puerto/db)
///   3. SUPABASE_DB_*                        (host, port, name, user, password)
///
/// Orden de resolución de la clave JWT (Jwt:Key):
///   1. Jwt__Key / Jwt:Key
///   2. SUPABASE_JWT_SECRET
///   3. En Development: una clave de desarrollo por defecto.
/// </summary>
public static class AppConfiguration
{
    private const string DevJwtKey = "5e397eb14a994f2daeb0c7c6345a21e0";

    /// <summary>Longitud mínima exigida a la clave de firma del JWT (HS256: 256 bits).</summary>
    private const int MinJwtKeyLength = 32;

    public static void Apply(IConfiguration configuration, IHostEnvironment environment)
    {
        configuration["ConnectionStrings:DefaultConnection"] =
            ResolveConnectionString(configuration, environment);

        configuration["Jwt:Key"] = ResolveJwtKey(configuration, environment);
    }

    /// <summary>
    /// Describe el destino de la conexión (sin credenciales) para poder
    /// registrarlo en el arranque sin exponer la contraseña.
    /// </summary>
    public static string DescribeTarget(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return "(sin configurar)";
        }

        var builder = new NpgsqlConnectionStringBuilder(connectionString);

        return $"{builder.Host}:{builder.Port}/{builder.Database} (usuario {builder.Username})";
    }

    private static string ResolveConnectionString(
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var configured = configuration.GetConnectionString("DefaultConnection");

        if (!string.IsNullOrWhiteSpace(configured))
        {
            return configured.Trim();
        }

        var databaseUrl = configuration["DATABASE_URL"];

        if (!string.IsNullOrWhiteSpace(databaseUrl))
        {
            return FromUri(databaseUrl.Trim());
        }

        // Host/puerto/base/usuario NO son secretos: tienen valores por defecto en
        // appsettings.json (sección "Supabase") y se pueden sobrescribir con las
        // variables SUPABASE_DB_* o Supabase__*.
        var host = configuration["SUPABASE_DB_HOST"]
            ?? configuration["Supabase:Host"];

        if (!string.IsNullOrWhiteSpace(host))
        {
            var password = configuration["SUPABASE_DB_PASSWORD"]
                ?? configuration["Supabase:Password"];

            if (string.IsNullOrWhiteSpace(password))
            {
                // Mensaje explícito: sin contraseña la conexión fallaría con un
                // críptico "28P01 password authentication failed".
                throw new InvalidOperationException(
                    "Falta la contraseña de la base de datos. En el servicio de Render añade " +
                    "la variable de entorno SUPABASE_DB_PASSWORD (o ConnectionStrings__DefaultConnection " +
                    "con la cadena completa). " + DescribeDetectedKeys(configuration) +
                    $" Entorno actual: {environment.EnvironmentName}.");
            }

            return BuildFromParts(
                host!,
                configuration["SUPABASE_DB_PORT"] ?? configuration["Supabase:Port"],
                configuration["SUPABASE_DB_NAME"] ?? configuration["Supabase:Database"],
                configuration["SUPABASE_DB_USER"] ?? configuration["Supabase:User"],
                password);
        }

        throw new InvalidOperationException(
            "No hay base de datos configurada. Define ConnectionStrings__DefaultConnection, " +
            "DATABASE_URL o las variables SUPABASE_DB_* (SUPABASE_DB_HOST, SUPABASE_DB_PORT, " +
            "SUPABASE_DB_NAME, SUPABASE_DB_USER, SUPABASE_DB_PASSWORD). " +
            DescribeDetectedKeys(configuration) +
            $" Entorno actual: {environment.EnvironmentName}.");
    }

    /// <summary>
    /// Lista (solo los nombres, nunca los valores) las variables de configuración
    /// relacionadas con la BD y el JWT que el proceso ha recibido. Es la forma más
    /// rápida de ver en los logs de Render si faltó definir algo.
    /// </summary>
    private static string DescribeDetectedKeys(IConfiguration configuration)
    {
        var prefixes = new[]
        {
            "SUPABASE",
            "DATABASE",
            "POSTGRES",
            "CONNECTIONSTRINGS",
            "JWT",
            "Supabase:"
        };

        var keys = configuration.AsEnumerable()
            .Select(pair => pair.Key)
            .Where(key => prefixes.Any(prefix =>
                key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(key => key, StringComparer.OrdinalIgnoreCase)
            .ToList();

        return keys.Count == 0
            ? "Claves detectadas: ninguna (no defines ninguna variable de BD en el servicio)."
            : "Claves detectadas: " + string.Join(", ", keys) + ".";
    }

    private static string ResolveJwtKey(
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var key = configuration["Jwt:Key"];

        key = !string.IsNullOrWhiteSpace(key)
            ? key.Trim()
            : configuration["SUPABASE_JWT_SECRET"]?.Trim();

        if (string.IsNullOrWhiteSpace(key))
        {
            if (environment.IsDevelopment())
            {
                return DevJwtKey;
            }

            throw new InvalidOperationException(
                "Falta la clave JWT. Define la variable de entorno Jwt__Key " +
                "(cualquier cadena secreta, mínimo " + MinJwtKeyLength + " caracteres) " +
                "o SUPABASE_JWT_SECRET.");
        }

        if (key.Length < MinJwtKeyLength)
        {
            throw new InvalidOperationException(
                $"La clave JWT (Jwt__Key) es demasiado corta: {key.Length} caracteres. " +
                $"Usa al menos {MinJwtKeyLength} (HS256 requiere 256 bits).");
        }

        return key;
    }

    private static string BuildFromParts(
        string host,
        string? port,
        string? database,
        string? user,
        string? password)
    {
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = host,
            Port = int.TryParse(port, out var parsedPort) ? parsedPort : 5432,
            Database = string.IsNullOrWhiteSpace(database) ? "postgres" : database,
            Username = string.IsNullOrWhiteSpace(user) ? "postgres" : user,
            Password = password,
            // Supabase exige TLS.
            SslMode = SslMode.Require
        };

        return builder.ConnectionString;
    }

    private static string FromUri(string connectionUri)
    {
        var uri = new Uri(connectionUri);

        var credentials = uri.UserInfo.Split(':', 2);

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.Port > 0 ? uri.Port : 5432,
            Database = uri.AbsolutePath.TrimStart('/'),
            Username = Uri.UnescapeDataString(credentials[0]),
            Password = credentials.Length > 1
                ? Uri.UnescapeDataString(credentials[1])
                : string.Empty,
            SslMode = SslMode.Require
        };

        return builder.ConnectionString;
    }
}
