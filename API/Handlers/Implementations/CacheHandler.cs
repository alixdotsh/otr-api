using API.DTOs;
using API.Handlers.Interfaces;
using API.Utilities;
using CachingFramework.Redis;

namespace API.Handlers.Implementations;

public class CacheHandler(string configuration) : RedisContext(configuration), ICacheHandler
{
    private const int SearchResultLifetimeMinutes = 30;

    public async Task SetTournamentSearchResultAsync(IEnumerable<TournamentSearchResultDTO> result, string query) =>
        await Cache.SetObjectAsync(
            CacheUtils.TournamentSearchKey(query),
            result,
            [CacheUtils.TournamentSearchTag],
            TimeSpan.FromMinutes(SearchResultLifetimeMinutes)
        );

    public async Task SetMatchSearchResultAsync(IEnumerable<MatchSearchResultDTO> result, string query) =>
        await Cache.SetObjectAsync(
            CacheUtils.MatchSearchKey(query),
            result,
            [CacheUtils.MatchSearchTag],
            TimeSpan.FromMinutes(SearchResultLifetimeMinutes)
        );

    public async Task SetPlayerSearchResultAsync(IEnumerable<PlayerSearchResultDTO> result, string query) =>
        await Cache.SetObjectAsync(
            CacheUtils.PlayerSearchKey(query),
            result,
            [CacheUtils.PlayerSearchTag],
            TimeSpan.FromMinutes(SearchResultLifetimeMinutes)
        );
}
