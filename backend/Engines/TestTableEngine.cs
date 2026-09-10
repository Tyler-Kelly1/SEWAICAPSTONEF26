using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Engines;

public class TestTableEngine : ITestTableEngine
{
    private readonly AppDbContext _context;

    public TestTableEngine(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TestItem>> GetTestItemsAsync()
    {
        return await _context.TestTables
            .OrderByDescending(x => x.InsertTimeStamp)
            .ToListAsync();
    }

    public async Task<TestItem> CreateTestItemAsync(CreateTestItemDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Value))
        {
            throw new ArgumentException("Value cannot be empty.");
        }

        var item = new TestItem
        {
            Value = dto.Value.Trim(),
            InsertTimeStamp = DateTime.UtcNow
        };

        _context.TestTables.Add(item);
        await _context.SaveChangesAsync();

        return item;
    }
}
