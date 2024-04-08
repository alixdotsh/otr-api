using API.DTOs;
using API.Repositories.Interfaces;
using API.Services.Interfaces;

namespace API.Services.Implementations;

public class SearchService(ITournamentsRepository tournamentsRepository, IMatchesRepository matchesRepository, IPlayerRepository playerRepository) : ISearchService
{
    public async Task<List<SearchResponseDTO>> SearchByNameAsync(string searchKey)
    {
        var returnList = (await SearchTournamentsByNameAsync(searchKey)).ToList();
        returnList.AddRange(await SearchMatchesByNameAsync(searchKey));
        returnList.AddRange(await SearchPlayersByNameAsync(searchKey));

        return returnList;
    }

    private async Task<IEnumerable<SearchResponseDTO>> SearchTournamentsByNameAsync(string tournamentName)
    {
        var tournaments = (await tournamentsRepository.SearchAsync(tournamentName)).ToList();
        return tournaments.Select(tournament => new SearchResponseDTO { Type = "Tournament", Text = tournament.Name, Url = $"/tournaments/{tournament.Id}" });
    }

    private async Task<IEnumerable<SearchResponseDTO>> SearchMatchesByNameAsync(string matchName)
    {
        var matches = (await matchesRepository.SearchAsync(matchName)).ToList();
        return matches.Select(match => new SearchResponseDTO { Type = "Match", Text = match.Name ?? match.MatchId.ToString(), Url = $"/matches/{match.Id}" });
    }

    private async Task<IEnumerable<SearchResponseDTO>> SearchPlayersByNameAsync(string username)
    {
        var players = (await playerRepository.SearchAsync(username)).ToList();
        return players.Select(player => new SearchResponseDTO { Type = "Player", Text = player.Username ?? "<Unknown>", Url = $"/users/{player.Id}", Thumbnail = $"a.ppy.sh/{player.OsuId}" });
    }
}
