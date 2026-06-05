using AnimeList.Core.DTOs.Anime;
using AnimeList.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnimeList.Api.Controllers;

[ApiController]
[Route("api/animes")]
[Authorize]
public class AnimesController(IJikanService jikanService) : ControllerBase
{
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<AnimeSearchResult>>> Search(
        [FromQuery] string q, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest("El parámetro 'q' es requerido.");

        var animes = await jikanService.BuscarAnimesAsync(q, ct);
        var results = animes.Select(a => new AnimeSearchResult(
            a.MalId, a.Title, a.TitleEnglish, a.ImageUrl, a.Synopsis, a.EpisodeCount, a.AirStatus));
        return Ok(results);
    }
}
