using System.Security.Claims;
using AnimeList.Core.DTOs.AnimeList;
using AnimeList.Core.Enums;
using AnimeList.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnimeList.Api.Controllers;

[ApiController]
[Route("api/anime-list")]
[Authorize]
public class AnimeListController(IAnimeListService animeListService) : ControllerBase
{
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet("{entryId:int}")]
    public async Task<ActionResult<AnimeEntryResponse>> GetEntry(int entryId, CancellationToken ct)
    {
        var entry = await animeListService.GetEntryAsync(UserId, entryId, ct);
        if (entry is null) return NotFound();
        return Ok(entry);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AnimeEntryResponse>>> GetList(
        [FromQuery] WatchStatus? status, CancellationToken ct)
    {
        var list = await animeListService.GetListAsync(UserId, status, ct);
        return Ok(list);
    }

    [HttpPost]
    public async Task<ActionResult<AnimeEntryResponse>> Add(AddAnimeRequest request, CancellationToken ct)
    {
        var entry = await animeListService.AddAsync(UserId, request.MalId, ct);
        if (entry is null) return NotFound($"No se encontró el anime con MalId {request.MalId}.");
        return Ok(entry);
    }

    [HttpPut("{entryId:int}")]
    public async Task<ActionResult<AnimeEntryResponse>> Update(
        int entryId, UpdateAnimeEntryRequest request, CancellationToken ct)
    {
        var entry = await animeListService.UpdateAsync(UserId, entryId, request, ct);
        if (entry is null) return NotFound();
        return Ok(entry);
    }

    [HttpDelete("{entryId:int}")]
    public async Task<IActionResult> Remove(int entryId, CancellationToken ct)
    {
        var removed = await animeListService.RemoveAsync(UserId, entryId, ct);
        if (!removed) return NotFound();
        return NoContent();
    }
}
