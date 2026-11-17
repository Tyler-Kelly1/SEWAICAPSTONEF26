using Backend.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Backend.Tests;

[TestClass]
public class EnvConfigurationTests
{
    [TestMethod]
    public void GetDatabaseConnectionString_BuildsFromIndividualVariables()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string?>
        {
            { "DB_HOST", "db.example.com" },
            { "DB_PORT", "5433" },
            { "DB_NAME", "custom_overload" },
            { "DB_USER", "custom_user" },
            { "DB_PASSWORD", "SecretPass123!" }
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        // Act
        var connectionString = EnvConfigurationHelper.GetDatabaseConnectionString(configuration);

        // Assert
        Assert.IsTrue(connectionString.Contains("Host=db.example.com"));
        Assert.IsTrue(connectionString.Contains("Port=5433"));
        Assert.IsTrue(connectionString.Contains("Database=custom_overload"));
        Assert.IsTrue(connectionString.Contains("Username=custom_user"));
        Assert.IsTrue(connectionString.Contains("Password=SecretPass123!"));
    }

    [TestMethod]
    public void GetDatabaseConnectionString_WithConfiguredConnectionString_UsesConfiguredString()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string?>
        {
            { "ConnectionStrings:DefaultConnection", "Host=direct.example.com;Port=5432;Database=directdb;Username=u;Password=p" }
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        // Act
        var connectionString = EnvConfigurationHelper.GetDatabaseConnectionString(configuration);

        // Assert
        Assert.AreEqual("Host=direct.example.com;Port=5432;Database=directdb;Username=u;Password=p", connectionString);
    }
}
