using API.DTOs;
using API.Entities;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class MeController : Controller
{
	private readonly IPlayerService _playerService;
	private readonly IRatingsService _ratingsService;
	private readonly IRatingHistoryService _ratingHistoryService;
	private readonly IUserService _userService;
	private readonly IDistributedCache _cache;
	public MeController(IPlayerService playerService, IRatingsService ratingsService, IRatingHistoryService ratingHistoryService, IUserService userService, IDistributedCache cache)
	{
		_playerService = playerService;
		_ratingsService = ratingsService;
		_ratingHistoryService = ratingHistoryService;
		_userService = userService;
		_cache = cache;
	}
	
	private long? GetOsuId()
	{
		string? osuId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Name)?.Value;
		if (osuId == null)
		{
			return null;
		}
		
		if(!long.TryParse(osuId, out long osuPlayerId))
		{
			return null;
		}

		return osuPlayerId;
	}

	[HttpGet]
	public async Task<ActionResult<User>> GetLoggedInUserAsync()
	{
		long? osuId = GetOsuId();
		
		if(!osuId.HasValue)
		{
			return BadRequest("User's login seems corrupted, couldn't identify osuId.");
		}

		var user = await _userService.GetForPlayerAsync(osuId.Value);
		return Ok(user);
	}

	// [HttpGet("statistics")]
	// public async Task<ActionResult<Unmapped_PlayerStatisticsDTO>> GetAsync([FromQuery]int offsetDays = -1, [FromQuery]int mode = 0)
	// {
	// 	long? osuId = GetOsuId();
	// 	
	// 	if(!osuId.HasValue)
	// 	{
	// 		return BadRequest("User's login seems corrupted, couldn't identify osuId.");
	// 	}
	//
	// 	string osuIdKey = HttpContext.User.Claims.First(x => x.Type == JwtRegisteredClaimNames.Name).Value;
	// 	string key = $"stats_{osuIdKey}_{offsetDays}_{mode}";
	// 	
	// 	byte[]? cachedStatsBytes = await _cache.GetAsync(key);
	// 	var recentCreatedDate = await _ratingsService.GetRecentCreatedDate(osuId.Value);
	//
	// 	if (cachedStatsBytes == null)
	// 	{
	// 		// Invalidate cache if the player's ratings have been updated since the cache was created.
	// 		await _cache.RemoveAsync(key);
	//
	// 		var freshStats = await _playerService.GetVerifiedPlayerStatisticsAsync(osuId.Value, (OsuEnums.Mode) mode, FromTime(offsetDays));
	// 		await _cache.SetStringAsync(key, JsonConvert.SerializeObject(freshStats));
	//
	// 		return Ok(freshStats);
	// 	}
	// 	
	// 	var cachedObj = JsonConvert.DeserializeObject<Unmapped_PlayerStatisticsDTO>(Encoding.UTF8.GetString(cachedStatsBytes));
	// 	if (cachedObj!.Created < recentCreatedDate)
	// 	{
	// 		// Invalidate cache if the player's ratings have been updated since the cache was created.
	// 		await _cache.RemoveAsync(key);
	//
	// 		var freshStats = await _playerService.GetVerifiedPlayerStatisticsAsync(osuId.Value, (OsuEnums.Mode) mode, FromTime(offsetDays));
	// 		await _cache.SetStringAsync(key, JsonConvert.SerializeObject(freshStats));
	//
	// 		return Ok(freshStats);
	// 	}
	// 	
	// 	return Ok(cachedObj);
	// }
	
	[HttpGet("ratinghistories")]
	public async Task<ActionResult<IEnumerable<RatingHistoryDTO>>> GetRatingHistoriesAsync([FromQuery]int offsetDays = -1, [FromQuery]int mode = 0)
	{
		long? osuId = GetOsuId();
		
		if(!osuId.HasValue)
		{
			return BadRequest("User's login seems corrupted, couldn't identify osuId.");
		}

		string osuIdKey = HttpContext.User.Claims.First(x => x.Type == JwtRegisteredClaimNames.Name).Value;
		string key = $"ratinghistories_{osuIdKey}_{offsetDays}_{mode}";
		
		byte[]? cachedStatsBytes = await _cache.GetAsync(key);

		if (cachedStatsBytes == null)
		{
			// Invalidate cache if the player's ratings have been updated since the cache was created.
			await _cache.RemoveAsync(key);

			var freshHistories = await _ratingHistoryService.GetForPlayerAsync(osuId.Value, mode, DateTime.MinValue);
			await _cache.SetStringAsync(key, JsonConvert.SerializeObject(freshHistories));
			cachedStatsBytes = await _cache.GetAsync(key);
		}
		
		var recentCreatedDate = await _ratingsService.GetRecentCreatedDate(osuId.Value);
		var cachedObj = JsonConvert.DeserializeObject<IEnumerable<RatingHistoryDTO>>(Encoding.UTF8.GetString(cachedStatsBytes!))?.ToList();

		if (cachedObj == null || cachedObj.MaxBy(x => x.Created)?.Created < recentCreatedDate)
		{
			// Invalidate cache if the player's ratings have been updated since the cache was created.
			await _cache.RemoveAsync(key);

			var freshHistories = await _ratingHistoryService.GetForPlayerAsync(osuId.Value, mode, DateTime.MinValue);
			await _cache.SetStringAsync(key, JsonConvert.SerializeObject(freshHistories));

			cachedObj = freshHistories.ToList();
		}
		
		// Filter by fromTime and return
		return Ok(cachedObj.Where(x => x.Created >= FromTime(offsetDays)));
	}

	private DateTime FromTime(int offsetDays)
	{
		return offsetDays < 0 ? DateTime.MinValue : DateTime.UtcNow.AddDays(-offsetDays);
	}
}