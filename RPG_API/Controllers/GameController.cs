using Microsoft.AspNetCore.Mvc;
using RPG_API.Models;
using RPG_API.Services;

namespace RPG_API.Controllers;

// De controller ontvangt HTTP-requests en geeft HTTP-responses terug.
// Hij bevat geen spelregels: dat werk doet de service.
[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private readonly IGameService _gameService;

    public GameController(IGameService gameService)
    {
        _gameService = gameService;
    }

    // POST /api/game  -> 201 Created
    [HttpPost]
    public async Task<ActionResult<GameResponse>> Start(StartGameRequest request)
    {
        var game = await _gameService.StartAsync(request);
        return CreatedAtAction(nameof(Get), new { id = game.Id }, game);
    }

    // GET /api/game
    [HttpGet]
    public async Task<ActionResult<List<GameSummaryResponse>>> GetGames()
    {
        var games = await _gameService.GetGamesAsync();
        return Ok(games);
    }

    // GET /api/game/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<GameResponse>> Get(int id)
    {
        var game = await _gameService.GetAsync(id);
        return Ok(game);
    }

    // POST /api/game/5/move
    [HttpPost("{id:int}/move")]
    public async Task<ActionResult<GameResponse>> Move(int id)
    {
        var game = await _gameService.MoveAsync(id);
        return Ok(game);
    }

    // POST /api/game/5/attack   body: { "type": "Light" } of { "type": "Heavy" }
    [HttpPost("{id:int}/attack")]
    public async Task<ActionResult<GameResponse>> Attack(int id, AttackRequest request)
    {
        var game = await _gameService.AttackAsync(id, request.Type);
        return Ok(game);
    }

    // POST /api/game/5/run
    [HttpPost("{id:int}/run")]
    public async Task<ActionResult<GameResponse>> Run(int id)
    {
        var game = await _gameService.RunAsync(id);
        return Ok(game);
    }

    // POST /api/game/5/potion   (eigen spelactie)
    [HttpPost("{id:int}/potion")]
    public async Task<ActionResult<GameResponse>> UsePotion(int id)
    {
        var game = await _gameService.UsePotionAsync(id);
        return Ok(game);
    }
}
