using System.Security.Claims;
using AnimeList.Core.DTOs.Reviews;
using AnimeList.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnimeList.Api.Controllers;

[ApiController]
[Route("api/reviews")]
[Authorize]
public class ReviewsController(IReviewService reviewService) : ControllerBase
{
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet("{malId:int}")]
    public async Task<ActionResult<ReviewResponse>> Get(int malId, CancellationToken ct)
    {
        var review = await reviewService.GetByMalIdAsync(UserId, malId, ct);
        if (review is null) return NotFound();
        return Ok(review);
    }

    [HttpPut("{malId:int}")]
    public async Task<ActionResult<ReviewResponse>> Upsert(int malId, UpsertReviewRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
            return BadRequest("El contenido de la reseña no puede estar vacío.");

        var review = await reviewService.UpsertAsync(UserId, malId, request.Content, ct);
        if (review is null) return NotFound($"No se encontró el anime con MalId {malId}.");
        return Ok(review);
    }

    [HttpDelete("{malId:int}")]
    public async Task<IActionResult> Delete(int malId, CancellationToken ct)
    {
        var deleted = await reviewService.DeleteAsync(UserId, malId, ct);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
