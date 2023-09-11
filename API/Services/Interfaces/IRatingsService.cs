using API.DTOs;
using API.Entities;

namespace API.Services.Interfaces;

public interface IRatingsService : IService<Rating>
{
	/// <summary>
	///  Returns a list of all ratings for a player, one for each game mode (if available)
	/// </summary>
	/// <param name="playerId"></param>
	/// <returns></returns>
	Task<IEnumerable<RatingDTO>> GetForPlayerAsync(long osuPlayerId);

	Task<int> InsertOrUpdateForPlayerAsync(int playerId, Rating rating);
	Task<int> BatchInsertAsync(IEnumerable<RatingDTO> ratings);
	Task<IEnumerable<RatingDTO>> GetAllAsync();
	Task TruncateAsync();
}