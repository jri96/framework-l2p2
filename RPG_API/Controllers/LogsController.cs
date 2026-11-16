using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPG_API.Database;
using RPG_API.Entities;

namespace RPG_API.Controllers;

// Laat de laatste request-logs zien. Handig tijdens debuggen en het beoordelingsgesprek.
[ApiController]
[Route("api/logs")]
public class LogsController : ControllerBase
{
    private readonly GameDbContext _context;

    public LogsController(GameDbContext context)
    {
        _context = context;
    }

    // GET /api/logs?take=20
    [HttpGet]
    public async Task<ActionResult<List<ApiRequestLog>>> Get(int take = 20)
    {
        take = Math.Clamp(take, 1, 100);

        var logs = await _context.ApiRequestLogs
            .OrderByDescending(log => log.Id)
            .Take(take)
            .ToListAsync();

        return Ok(logs);
    }
}
