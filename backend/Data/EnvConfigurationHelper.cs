namespace Backend.Data;

public static class EnvConfigurationHelper
{
    public static Dictionary<string, string?> LoadEnv(string? searchDirectory = null)
    {
        var loadedVariables = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        var candidatePaths = new List<string>();
        if (!string.IsNullOrWhiteSpace(searchDirectory))
        {
            candidatePaths.Add(Path.Combine(searchDirectory, ".env"));
        }

        candidatePaths.Add(Path.Combine(Directory.GetCurrentDirectory(), ".env"));
        candidatePaths.Add(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".env"));

        // Check parent directories up to 4 levels for solution root
        var currentDir = new DirectoryInfo(Directory.GetCurrentDirectory());
        for (int i = 0; i < 4 && currentDir != null; i++)
        {
            candidatePaths.Add(Path.Combine(currentDir.FullName, ".env"));
            currentDir = currentDir.Parent;
        }

        string? envFilePath = candidatePaths.FirstOrDefault(File.Exists);
        if (envFilePath == null)
        {
            return loadedVariables;
        }

        foreach (var rawLine in File.ReadAllLines(envFilePath))
        {
            var line = rawLine.Trim();
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#') || !line.Contains('='))
            {
                continue;
            }

            var separatorIndex = line.IndexOf('=');
            var key = line.Substring(0, separatorIndex).Trim();
            var value = line.Substring(separatorIndex + 1).Trim();

            // Strip surrounding double or single quotes if present
            if ((value.StartsWith('"') && value.EndsWith('"')) || (value.StartsWith('\'') && value.EndsWith('\'')))
            {
                value = value.Substring(1, value.Length - 2);
            }

            if (!string.IsNullOrWhiteSpace(key))
            {
                Environment.SetEnvironmentVariable(key, value);
                loadedVariables[key] = value;
            }
        }

        return loadedVariables;
    }

    public static string GetDatabaseConnectionString(IConfiguration configuration)
    {
        // 1. Direct connection string from configuration if fully defined
        var configuredString = configuration.GetConnectionString("DefaultConnection");

        // 2. Read individual DB parameters from environment / configuration
        var host = configuration["DB_HOST"] ?? Environment.GetEnvironmentVariable("DB_HOST");
        var port = configuration["DB_PORT"] ?? Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
        var database = configuration["DB_NAME"] ?? Environment.GetEnvironmentVariable("DB_NAME") ?? "overloaddb";
        var user = configuration["DB_USER"] ?? Environment.GetEnvironmentVariable("DB_USER") ?? "postgres";
        var password = configuration["DB_PASSWORD"] ?? Environment.GetEnvironmentVariable("DB_PASSWORD");

        if (!string.IsNullOrWhiteSpace(host) && !string.IsNullOrWhiteSpace(password))
        {
            return $"Host={host};Port={port};Database={database};Username={user};Password={password}";
        }

        if (!string.IsNullOrWhiteSpace(configuredString))
        {
            return configuredString;
        }

        // Default local dev fallback
        return "Host=localhost;Port=5432;Database=overloaddb;Username=postgres;Password=postgrespassword";
    }
}
