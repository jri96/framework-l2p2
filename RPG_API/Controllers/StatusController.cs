using Microsoft.AspNetCore.Mvc;
using RPG_API.Models;
using RPG_API.Services;

namespace RPG_API.Controllers;

// Week 1: eerste endpoint om te testen of de API draait.
// Week 3: verplaatst naar een controller, het bericht komt uit IStatusService.
[ApiController]
[Route("api/status")]
public class StatusController : ControllerBase
{
    private readonly IStatusService _statusService;

    public StatusController(IStatusService statusService)
    {
        _statusService = statusService;
    }

    [HttpGet]
    public ActionResult<StatusResponse> Get()
    {
        return Ok(new StatusResponse
        {
            Message = _statusService.GetMessage(),
            ServerTime = DateTime.Now
        });
    }
}
