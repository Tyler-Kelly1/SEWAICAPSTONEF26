using Backend.Data;
using Backend.DTOs;
using Backend.Engines;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Backend.Tests;

[TestClass]
public class UserEngineTests
{
    private AppDbContext _context = null!;
    private UserEngine _engine = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _engine = new UserEngine(_context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [TestMethod]
    public async Task CheckUserExistsAsync_WhenUserExists_ReturnsTrue()
    {
        // Arrange
        _context.Users.Add(new User { UserId = "existing_user" });
        await _context.SaveChangesAsync();

        // Act
        var result = await _engine.CheckUserExistsAsync("existing_user");

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task CheckUserExistsAsync_CaseInsensitiveMatch_ReturnsTrue()
    {
        // Arrange
        _context.Users.Add(new User { UserId = "Tyler_Dev" });
        await _context.SaveChangesAsync();

        // Act
        var result = await _engine.CheckUserExistsAsync("tyler_dev");

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task CheckUserExistsAsync_WhenUserDoesNotExist_ReturnsFalse()
    {
        // Act
        var result = await _engine.CheckUserExistsAsync("non_existent_user");

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task CheckUserExistsAsync_EmptyOrNull_ReturnsFalse()
    {
        // Act & Assert
        Assert.IsFalse(await _engine.CheckUserExistsAsync(""));
        Assert.IsFalse(await _engine.CheckUserExistsAsync("   "));
        Assert.IsFalse(await _engine.CheckUserExistsAsync(null!));
    }

    [TestMethod]
    public async Task GetUserByIdAsync_WhenExists_ReturnsUser()
    {
        // Arrange
        _context.Users.Add(new User { UserId = "test_athlete" });
        await _context.SaveChangesAsync();

        // Act
        var user = await _engine.GetUserByIdAsync("test_athlete");

        // Assert
        Assert.IsNotNull(user);
        Assert.AreEqual("test_athlete", user.UserId);
    }

    [TestMethod]
    public async Task GetUserByIdAsync_WhenNotFound_ReturnsNull()
    {
        // Act
        var user = await _engine.GetUserByIdAsync("ghost");

        // Assert
        Assert.IsNull(user);
    }

    [TestMethod]
    public async Task CreateUserAsync_ValidDto_CreatesAndPersistsUser()
    {
        // Arrange
        var dto = new CreateUserDto { UserId = "new_lifter" };

        // Act
        var created = await _engine.CreateUserAsync(dto);

        // Assert
        Assert.IsNotNull(created);
        Assert.AreEqual("new_lifter", created.UserId);

        var inDb = await _context.Users.FindAsync("new_lifter");
        Assert.IsNotNull(inDb);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public async Task CreateUserAsync_UserAlreadyExists_ThrowsInvalidOperationException()
    {
        // Arrange
        _context.Users.Add(new User { UserId = "already_here" });
        await _context.SaveChangesAsync();

        var dto = new CreateUserDto { UserId = "already_here" };

        // Act
        await _engine.CreateUserAsync(dto);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public async Task CreateUserAsync_EmptyUserId_ThrowsArgumentException()
    {
        // Arrange
        var dto = new CreateUserDto { UserId = "   " };

        // Act
        await _engine.CreateUserAsync(dto);
    }

    [TestMethod]
    public async Task GetAllUsersAsync_ReturnsAllUsersOrdered()
    {
        // Arrange
        _context.Users.AddRange(
            new User { UserId = "charlie" },
            new User { UserId = "alice" },
            new User { UserId = "bob" }
        );
        await _context.SaveChangesAsync();

        // Act
        var users = (await _engine.GetAllUsersAsync()).ToList();

        // Assert
        Assert.AreEqual(3, users.Count);
        Assert.AreEqual("alice", users[0].UserId);
        Assert.AreEqual("bob", users[1].UserId);
        Assert.AreEqual("charlie", users[2].UserId);
    }
}
