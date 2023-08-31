using API.Entities.Bases;
using API.Osu.Multiplayer;
using Dapper;

namespace API.Entities;

/// <summary>
/// Represents a game as seen here: https://github.com/ppy/osu-api/wiki#response-5
/// </summary>
[Table("osugames")]
public class OsuGame : EntityBase
{
	/// <summary>
	/// The id of the match as seen from the database
	/// </summary>
	[Column("match_id")]
	public int MatchId { get; set; }
	
	[Column("game_id")]
	public long GameId { get; set; }
	
	[Column("start_time")]
	public DateTime StartTime { get; set; }
	
	[Column("end_time")]
	public DateTime EndTime { get; set; }
	
	[Column("beatmap_id")]
	public long BeatmapId { get; set; }
	
	[Column("play_mode")]
	public PlayMode PlayMode { get; set; }
	
	[Column("match_type")]
	public MatchType MatchType { get; set; }
	
	[Column("scoring_type")]
	public ScoringType ScoringType { get; set; }
	
	[Column("team_type")]
	public TeamType TeamType { get; set; }
	
	[Column("mods")]
	public Mods Mods { get; set; }
	
	public ICollection<OsuScore> Scores { get; set; } = null!;
}