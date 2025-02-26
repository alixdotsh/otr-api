using API.DTOs;
using API.Services.Interfaces;
using API.Utilities;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion(1)]
[Authorize(Roles = $"{OtrClaims.System}, {OtrClaims.Admin}")] // Internal access only at this time
[Route("api/v{version:apiVersion}/[controller]")]
public class BeatmapsController(IBeatmapService beatmapService) : Controller
{
    /// <summary>
    /// List all beatmaps
    /// </summary>
    /// <response code="200">Returns all beatmaps</response>
    [HttpGet]
    [ProducesResponseType<IEnumerable<BeatmapDTO>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAsync() =>
        Ok(await beatmapService.ListAsync());

    /// <summary>
    /// Get a beatmap by versatile search
    /// </summary>
    /// <remarks>Get a beatmap searching first by id, then by osu! beatmap id</remarks>
    /// <param name="key">Search key</param>
    /// <response code="404">If a beatmap for the search key does not exist</response>
    /// <response code="200">Returns a beatmap</response>
    [HttpGet("{key:long}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<BeatmapDTO>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(long key)
    {
        BeatmapDTO? beatmap = await beatmapService.GetVersatileAsync(key);
        if (beatmap == null)
        {
            return NotFound();
        }

        return Ok(beatmap);
    }
}
