namespace API.DTOs;

public class PlayerRanksDTO
{
	public long OsuId { get; set; }
	public int? RankStandard { get; set; }
	public int? RankTaiko { get; set; }
	public int? RankCatch { get; set; }
	public int? RankMania { get; set; }
}