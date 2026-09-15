using Backend.Controllers;
using Backend.DTOs;
using Backend.Engines;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Backend.Tests;

[TestClass]
public class TestTableControllerTests
{
    private FakeTestTableEngine _fakeEngine = null!;
    private TestTableController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _fakeEngine = new FakeTestTableEngine();
        _controller = new TestTableController(_fakeEngine);
    }

    [TestMethod]
    public async Task GetTestItems_ReturnsOkResultWithItems()
    {
        // Act
        var result = await _controller.GetTestItems();

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)result.Result!;
        var items = okResult.Value as IEnumerable<TestItem>;
        Assert.IsNotNull(items);
    }

    [TestMethod]
    public async Task CreateTestItem_ValidDto_ReturnsCreatedAtActionResult()
    {
        // Arrange
        var dto = new CreateTestItemDto { Value = "New Record" };

        // Act
        var result = await _controller.CreateTestItem(dto);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
        var createdResult = (CreatedAtActionResult)result.Result!;
        var item = createdResult.Value as TestItem;
        Assert.IsNotNull(item);
        Assert.AreEqual("New Record", item.Value);
    }

    [TestMethod]
    public async Task CreateTestItem_EngineThrowsArgumentException_ReturnsBadRequest()
    {
        // Arrange
        var dto = new CreateTestItemDto { Value = "" };

        // Act
        var result = await _controller.CreateTestItem(dto);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
    }
}

public class FakeTestTableEngine : ITestTableEngine
{
    private readonly List<TestItem> _items = new();
    private int _nextId = 1;

    public Task<IEnumerable<TestItem>> GetTestItemsAsync()
    {
        return Task.FromResult<IEnumerable<TestItem>>(_items.OrderByDescending(x => x.InsertTimeStamp).ToList());
    }

    public Task<TestItem> CreateTestItemAsync(CreateTestItemDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Value))
        {
            throw new ArgumentException("Value cannot be empty.");
        }

        var item = new TestItem
        {
            Id = _nextId++,
            Value = dto.Value.Trim(),
            InsertTimeStamp = DateTime.UtcNow
        };
        _items.Add(item);

        return Task.FromResult(item);
    }
}
