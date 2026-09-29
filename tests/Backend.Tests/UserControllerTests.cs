using Backend.Controllers;
using Backend.DTOs;
using Backend.Engines;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Backend.Tests;

[TestClass]
public class UserControllerTests
{
    private FakeUserEngine _fakeEngine = null!;
    private UserController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _fakeEngine = new FakeUserEngine();
        _controller = new UserController(_fakeEngine);
    }

    [TestMethod]
    public async Task CheckUserExists_WhenUserExists_ReturnsOkWithTrue()
    {
        // Arrange
        await _fakeEngine.CreateUserAsync(new CreateUserDto { UserId = "existing_user" });

        // Act
        var result = await _controller.CheckUserExists("existing_user");

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)result.Result!;
        var dto = okResult.Value as UserCheckResultDto;
        Assert.IsNotNull(dto);
        Assert.AreEqual("existing_user", dto.UserId);
        Assert.IsTrue(dto.Exists);
    }

    [TestMethod]
    public async Task CheckUserExists_WhenUserDoesNotExist_ReturnsOkWithFalse()
    {
        // Act
        var result = await _controller.CheckUserExists("unknown_user");

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)result.Result!;
        var dto = okResult.Value as UserCheckResultDto;
        Assert.IsNotNull(dto);
        Assert.AreEqual("unknown_user", dto.UserId);
        Assert.IsFalse(dto.Exists);
    }

    [TestMethod]
    public async Task GetUserById_WhenExists_ReturnsOkWithUser()
    {
        // Arrange
        await _fakeEngine.CreateUserAsync(new CreateUserDto { UserId = "lifter1" });

        // Act
        var result = await _controller.GetUserById("lifter1");

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)result.Result!;
        var user = okResult.Value as User;
        Assert.IsNotNull(user);
        Assert.AreEqual("lifter1", user.UserId);
    }

    [TestMethod]
    public async Task GetUserById_WhenDoesNotExist_ReturnsNotFound()
    {
        // Act
        var result = await _controller.GetUserById("nonexistent");

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
    }

    [TestMethod]
    public async Task CreateUser_ValidDto_ReturnsCreatedAtAction()
    {
        // Arrange
        var dto = new CreateUserDto { UserId = "new_user" };

        // Act
        var result = await _controller.CreateUser(dto);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
        var createdResult = (CreatedAtActionResult)result.Result!;
        var user = createdResult.Value as User;
        Assert.IsNotNull(user);
        Assert.AreEqual("new_user", user.UserId);
    }

    [TestMethod]
    public async Task CreateUser_WhenUserAlreadyExists_ReturnsConflict()
    {
        // Arrange
        await _fakeEngine.CreateUserAsync(new CreateUserDto { UserId = "duplicate" });
        var dto = new CreateUserDto { UserId = "duplicate" };

        // Act
        var result = await _controller.CreateUser(dto);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(ConflictObjectResult));
    }

    [TestMethod]
    public async Task CreateUser_EmptyUserId_ReturnsBadRequest()
    {
        // Arrange
        var dto = new CreateUserDto { UserId = "" };

        // Act
        var result = await _controller.CreateUser(dto);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
    }
}

public class FakeUserEngine : IUserEngine
{
    private readonly List<User> _users = new();

    public Task<bool> CheckUserExistsAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId)) return Task.FromResult(false);
        var exists = _users.Any(u => u.UserId.Equals(userId.Trim(), StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(exists);
    }

    public Task<User?> GetUserByIdAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId)) return Task.FromResult<User?>(null);
        var user = _users.FirstOrDefault(u => u.UserId.Equals(userId.Trim(), StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(user);
    }

    public Task<User> CreateUserAsync(CreateUserDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.UserId))
        {
            throw new ArgumentException("User ID is required.");
        }

        var clean = dto.UserId.Trim();
        if (_users.Any(u => u.UserId.Equals(clean, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"User with ID '{clean}' already exists.");
        }

        var user = new User { UserId = clean };
        _users.Add(user);
        return Task.FromResult(user);
    }

    public Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return Task.FromResult<IEnumerable<User>>(_users);
    }
}
