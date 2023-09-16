using API.Entities;

namespace API.Services.Interfaces;

public interface IMatchScoresService : IService<MatchScore>
{
	public Task<int> AverageTeammateScore(long osuPlayerId, int mode);
	public Task<int> AverageOpponentScore(long osuPlayerId, int mode);
}