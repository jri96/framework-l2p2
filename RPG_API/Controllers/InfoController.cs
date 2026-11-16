using Microsoft.AspNetCore.Mvc;
using RPG_API.Models;
using RPG_API.Services;

namespace RPG_API.Controllers;

// Week 2: tweede GET-endpoint met spelinformatie.
// Week 3: controller + service via dependency injection.
[ApiController]
[Route("api/info")]
public class InfoController : ControllerBase
{
    private readonly IGameInfoService _gameInfoService;

    public InfoController(IGameInfoService gameInfoService)
    {
        _gameInfoService = gameInfoService;
    }

    // GET /api/info
    [HttpGet]
    public ActionResult<GameInfoResponse> Get()
    {
        return Ok(_gameInfoService.GetInfo());
    }
}
