using API.Entities;
using API.Enums;
using API.Osu;

namespace API.Utilities;

public static class QueryExtensions
{
	// Match
	public static IQueryable<Match> WhereVerified(this IQueryable<Match> query) => query.AsQueryable().Where(x => x.VerificationStatus == (int)MatchVerificationStatus.Verified);

	// Game
	public static IQueryable<Game> WhereVerified(this IQueryable<Game> query) => query.AsQueryable().Where(x => x.Match.VerificationStatus == (int)MatchVerificationStatus.Verified);
	public static IQueryable<Game> WhereTeamVsTeam(this IQueryable<Game> query) => query.AsQueryable().Where(x => x.TeamType == (int)OsuEnums.TeamType.TeamVs);
	public static IQueryable<Game> WhereHeadToHead(this IQueryable<Game> query) => query.AsQueryable().Where(x => x.TeamType == (int)OsuEnums.TeamType.HeadToHead);

	// MatchScore
	/// <summary>
	/// Selects scores that are verified
	/// </summary>
	/// <param name="query"></param>
	/// <returns></returns>
	public static IQueryable<MatchScore> WhereVerified(this IQueryable<MatchScore> query) => query.AsQueryable().Where(x => x.Game.Match.VerificationStatus == (int)MatchVerificationStatus.Verified);
	/// <summary>
	/// Selects all HeadToHead scores
	/// </summary>
	/// <param name="query"></param>
	/// <returns></returns>
	public static IQueryable<MatchScore> WhereHeadToHead(this IQueryable<MatchScore> query) => query.AsQueryable().Where(x => x.Game.TeamType == (int)OsuEnums.TeamType.HeadToHead);
	public static IQueryable<MatchScore> WhereNotHeadToHead(this IQueryable<MatchScore> query) => query.AsQueryable().Where(x => x.Game.TeamType != (int)OsuEnums.TeamType.HeadToHead);
	/// <summary>
	/// Selects all TeamVs match scores
	/// </summary>
	/// <param name="query"></param>
	/// <returns></returns>
	public static IQueryable<MatchScore> WhereTeamVs(this IQueryable<MatchScore> query) => query.AsQueryable().Where(x => x.Game.TeamType == (int)OsuEnums.TeamType.TeamVs);
	/// <summary>
	/// Selects all match scores, other than the provided player's, that are on the opposite team as the provided player. Excludes HeadToHead scores
	/// </summary>
	/// <param name="query"></param>
	/// <param name="osuPlayerId"></param>
	/// <returns></returns>
	public static IQueryable<MatchScore> WhereOpponent(this IQueryable<MatchScore> query, long osuPlayerId) => query.AsQueryable().Where(x => x.Player.OsuId != osuPlayerId);
	/// <summary>
	/// Selects all match scores, other than the provided player's, that are on the same team as the provided player. Excludes HeadToHead scores
	/// </summary>
	/// <param name="query"></param>
	/// <param name="osuPlayerId"></param>
	/// <returns></returns>
	public static IQueryable<MatchScore> WhereTeammate(this IQueryable<MatchScore> query, long osuPlayerId) => query.AsQueryable().Where(x => x.Player.OsuId != osuPlayerId && x.Game.TeamType != (int)OsuEnums.TeamType.HeadToHead && x.Team == x.Game.MatchScores.First(y => y.Player.OsuId == osuPlayerId).Team);
	/// <summary>
	/// Selects all MatchScores for a given playMode (e.g. mania)
	/// </summary>
	/// <param name="query"></param>
	/// <param name="playMode"></param>
	/// <returns></returns>
	public static IQueryable<MatchScore> WhereMode(this IQueryable<MatchScore> query, int playMode) => query.AsQueryable().Where(x => x.Game.PlayMode == playMode);
	public static IQueryable<MatchScore> WherePlayer(this IQueryable<MatchScore> query, long osuPlayerId) => query.AsQueryable().Where(x => x.Player.OsuId == osuPlayerId);
	// Rating
	public static IQueryable<Rating> WhereMode(this IQueryable<Rating> query, int playMode) => query.AsQueryable().Where(x => x.Mode == playMode);
	public static IQueryable<Rating> WherePlayer(this IQueryable<Rating> query, long osuPlayerId) => query.AsQueryable().Where(x => x.Player.OsuId == osuPlayerId);
	public static IQueryable<Rating> OrderByMuDescending(this IQueryable<Rating> query) => query.AsQueryable().OrderByDescending(x => x.Mu);
	
	// Rating Histories
	public static IQueryable<RatingHistory> WherePlayer(this IQueryable<RatingHistory> query, long osuPlayerId) => query.AsQueryable().Where(x => x.Player.OsuId == osuPlayerId);
	public static IQueryable<RatingHistory> OrderByMuDescending(this IQueryable<RatingHistory> query) => query.AsQueryable().OrderByDescending(x => x.Mu);
}