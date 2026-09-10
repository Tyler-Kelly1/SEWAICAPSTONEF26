using Backend.DTOs;
using Backend.Models;

namespace Backend.Engines;

public interface ITestTableEngine
{
    Task<IEnumerable<TestItem>> GetTestItemsAsync();
    Task<TestItem> CreateTestItemAsync(CreateTestItemDto dto);
}
