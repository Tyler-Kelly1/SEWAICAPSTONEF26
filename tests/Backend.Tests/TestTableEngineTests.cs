using Backend.Data;
using Backend.DTOs;
using Backend.Engines;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Backend.Tests;

[TestClass]
public class TestTableEngineTests
{
    private AppDbContext _context = null!;
    private TestTableEngine _engine = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _engine = new TestTableEngine(_context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [TestMethod]
    public async Task GetTestItemsAsync_ReturnsItemsOrderedByTimestampDescending()
    {
        // Arrange
        var item1 = await _engine.CreateTestItemAsync(new CreateTestItemDto { Value = "First Item" });
        await Task.Delay(10); // Ensure timestamp difference
        var item2 = await _engine.CreateTestItemAsync(new CreateTestItemDto { Value = "Second Item" });

        // Act
        var items = (await _engine.GetTestItemsAsync()).ToList();

        // Assert
        Assert.AreEqual(2, items.Count);
        Assert.AreEqual("Second Item", items[0].Value);
        Assert.AreEqual("First Item", items[1].Value);
    }

    [TestMethod]
    public async Task CreateTestItemAsync_ValidDto_CreatesAndReturnsItem()
    {
        // Arrange
        var dto = new CreateTestItemDto { Value = "Sample Test Entry" };

        // Act
        var created = await _engine.CreateTestItemAsync(dto);
        var allItems = (await _engine.GetTestItemsAsync()).ToList();

        // Assert
        Assert.IsNotNull(created);
        Assert.IsTrue(created.Id > 0);
        Assert.AreEqual("Sample Test Entry", created.Value);
        Assert.AreEqual(1, allItems.Count);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public async Task CreateTestItemAsync_EmptyValue_ThrowsArgumentException()
    {
        // Arrange
        var dto = new CreateTestItemDto { Value = "   " };

        // Act
        await _engine.CreateTestItemAsync(dto);
    }
}
