using System.Security.Claims;
using Backend.Controllers;
using Backend.DTOs;
using Backend.Engines;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Backend.Tests;

[TestClass]
public class AuthControllerTests
{
    private FakeAuthEngine _fakeAuthEngine = null!;
    private AuthController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _fakeAuthEngine = new FakeAuthEngine();
        _controller = new AuthController(_fakeAuthEngine)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    [TestMethod]
    public async Task Register_ValidDto_ReturnsOk()
    {
        var dto = new RegisterDto
        {
            UserName = "newuser",
            Email = "newuser@example.com",
            Password = "Password123!"
        };

        var result = await _controller.Register(dto);

        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)result.Result!;
        var response = (AuthResponseDto)okResult.Value!;
        Assert.IsTrue(response.Success);
        Assert.AreEqual("newuser", response.UserName);
    }

    [TestMethod]
    public async Task Register_DuplicateUser_ReturnsBadRequest()
    {
        var dto = new RegisterDto
        {
            UserName = "taken",
            Email = "taken@example.com",
            Password = "Password123!"
        };

        var result = await _controller.Register(dto);

        Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task Login_ValidCredentials_ReturnsOk()
    {
        var dto = new LoginDto
        {
            Email = "valid@example.com",
            Password = "Password123!"
        };

        var result = await _controller.Login(dto);

        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)result.Result!;
        var response = (AuthResponseDto)okResult.Value!;
        Assert.IsTrue(response.Success);
    }

    [TestMethod]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        var dto = new LoginDto
        {
            Email = "wrong@example.com",
            Password = "BadPassword"
        };

        var result = await _controller.Login(dto);

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedObjectResult));
    }

    [TestMethod]
    public async Task Logout_ReturnsOk()
    {
        var result = await _controller.Logout();
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
    }

    [TestMethod]
    public async Task GetCurrentUser_WhenAuthenticated_ReturnsUser()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, "current_user"),
            new Claim(ClaimTypes.NameIdentifier, "current_user")
        }, "TestAuth"));

        _controller.ControllerContext.HttpContext.User = user;

        var result = await _controller.GetCurrentUser();

        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)result.Result!;
        var authUser = (AuthUserDto)okResult.Value!;
        Assert.AreEqual("current_user", authUser.UserName);
    }

    [TestMethod]
    public async Task GetCurrentUser_WhenNotAuthenticated_ReturnsUnauthorized()
    {
        _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

        var result = await _controller.GetCurrentUser();

        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedObjectResult));
    }
}

public class FakeAuthEngine : IAuthEngine
{
    public Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        if (dto.UserName == "taken" || dto.Email == "taken@example.com")
        {
            return Task.FromResult(new AuthResponseDto
            {
                Success = false,
                Message = "Username or email is already taken."
            });
        }

        return Task.FromResult(new AuthResponseDto
        {
            Success = true,
            Message = "Account registered successfully.",
            UserId = dto.UserName,
            UserName = dto.UserName,
            Email = dto.Email
        });
    }

    public Task<AuthResponseDto> LoginAsync(LoginDto dto, HttpContext httpContext)
    {
        if (dto.Email == "valid@example.com" && dto.Password == "Password123!")
        {
            return Task.FromResult(new AuthResponseDto
            {
                Success = true,
                Message = "Login successful.",
                UserId = "valid_user",
                UserName = "valid_user",
                Email = dto.Email
            });
        }

        return Task.FromResult(new AuthResponseDto
        {
            Success = false,
            Message = "Invalid credentials."
        });
    }

    public Task LogoutAsync(HttpContext httpContext)
    {
        return Task.CompletedTask;
    }

    public Task<AuthUserDto?> GetCurrentUserAsync(ClaimsPrincipal principal)
    {
        if (principal?.Identity?.IsAuthenticated == true)
        {
            return Task.FromResult<AuthUserDto?>(new AuthUserDto
            {
                UserId = principal.Identity.Name ?? "user1",
                UserName = principal.Identity.Name ?? "user1",
                Email = $"{principal.Identity.Name}@example.com"
            });
        }

        return Task.FromResult<AuthUserDto?>(null);
    }

    public string? GetAuthenticatedUserId(ClaimsPrincipal principal)
    {
        return principal?.Identity?.IsAuthenticated == true ? (principal.Identity.Name ?? "user1") : null;
    }

    public bool ValidateUserAccess(ClaimsPrincipal principal, string? requestedUserId)
    {
        if (principal?.Identity?.IsAuthenticated != true) return false;
        if (string.IsNullOrEmpty(requestedUserId)) return true;
        return string.Equals(principal.Identity.Name, requestedUserId, StringComparison.OrdinalIgnoreCase);
    }
}
