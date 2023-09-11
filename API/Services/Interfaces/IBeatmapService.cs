using API.DTOs;
using API.Entities;

namespace API.Services.Interfaces;

public interface IBeatmapService : IService<Beatmap>
{
	Task<IEnumerable<BeatmapDTO>> GetByBeatmapIdsAsync(IEnumerable<long> beatmapIds);
	Task<IEnumerable<BeatmapDTO>> GetAllAsync();
	Task<BeatmapDTO?> GetByBeatmapIdAsync(long osuBeatmapId);
}