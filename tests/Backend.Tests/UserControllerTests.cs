using System.Security.Claims;
using Backend.Controllers;
using Backend.DTOs;
using Backend.Engines;
using Backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Backend.Tests;

[TestClass]
public class UserControllerTests
{
    private FakeUserEngine _fakeEngine = null!;
    private FakeAuthEngine _fakeAuth = null!;
    private UserController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _fakeEngine = new FakeUserEngine();
        _fakeAuth = new FakeAuthEngine();
        _controller = new UserController(_fakeEngine, _fakeAuth);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, "lifter1"),
            new Claim(ClaimTypes.NameIdentifier, "lifter1")
        }, "TestAuth"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
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
    public async Task GetUserById_WhenExistsAndAuthorized_ReturnsOkWithUser()
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
    public async Task GetUserById_WhenAccessingOtherUser_ReturnsForbid()
    {
        // Act
        var result = await _controller.GetUserById("other_lifter");

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(ForbidResult));
    }

    [TestMethod]
    public void CreateUser_ReturnsBadRequest_DirectingToRegister()
    {
        // Arrange
        var dto = new CreateUserDto { UserId = "new_user" };

        // Act
        var result = _controller.CreateUser(dto);

        // Assert
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
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
        var user = new User { UserId = dto.UserId };
        _users.Add(user);
        return Task.FromResult(user);
    }

    public Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return Task.FromResult<IEnumerable<User>>(_users);
    }
}
