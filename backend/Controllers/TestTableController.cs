using Backend.DTOs;
using Backend.Engines;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestTableController : ControllerBase
{
    private readonly ITestTableEngine _engine;

    public TestTableController(ITestTableEngine engine)
    {
        _engine = engine;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TestItem>>> GetTestItems()
    {
        var items = await _engine.GetTestItemsAsync();
        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<TestItem>> CreateTestItem([FromBody] CreateTestItemDto dto)
    {
        try
        {
            var item = await _engine.CreateTestItemAsync(dto);
            return CreatedAtAction(nameof(GetTestItems), new { id = item.Id }, item);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
