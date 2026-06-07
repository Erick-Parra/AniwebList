using System.Security.Claims;
using AnimeList.Core.DTOs.Stats;
using AnimeList.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnimeList.Api.Controllers;

[ApiController]
[Route("api/stats")]
[Authorize]
public class StatsController(IStatsService statsService) : ControllerBase
{
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet]
    public async Task<ActionResult<StatsResponse>> GetStats(CancellationToken ct)
    {
        var stats = await statsService.GetStatsAsync(UserId, ct);
        return Ok(stats);
    }
}
