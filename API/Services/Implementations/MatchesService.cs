using API.Controllers;
using API.DTOs;
using API.DTOs.Processor;
using API.Services.Interfaces;
using AutoMapper;
using Database.Entities;
using Database.Enums;
using Database.Enums.Verification;
using Database.Repositories.Interfaces;

namespace API.Services.Implementations;

public class MatchesService(
    IMatchesRepository matchesRepository,
    ITournamentsRepository tournamentsRepository,
    IUrlHelperService urlHelperService,
    IMapper mapper
) : IMatchesService
{
    // TODO: Refactor to use enums for param "verificationSource"
    public async Task<IEnumerable<MatchCreatedResultDTO>?> CreateAsync(int tournamentId,
        int submitterId,
        IEnumerable<long> matchIds,
        bool verify)
    {
        Tournament? tournament = await tournamentsRepository.GetAsync(tournamentId);
        if (tournament is null)
        {
            return null;
        }

        // Only create matches that dont already exist
        IEnumerable<long> enumerableMatchIds = matchIds.ToList();
        IEnumerable<long> existingMatchIds = (await matchesRepository.GetAsync(enumerableMatchIds))
            .Select(m => m.OsuId)
            .ToList();

        // Create matches directly on the tournament because we can't access their ids until after the entity is updated
        IEnumerable<long> createdMatchIds = enumerableMatchIds.Except(existingMatchIds).ToList();
        foreach (var matchId in createdMatchIds)
        {
            tournament.Matches.Add(new Match
            {
                OsuId = matchId,
                VerificationStatus = VerificationStatus.None,
                VerifiedByUserId = verify ? submitterId : null,
                SubmittedByUserId = submitterId
            });
        }

        await tournamentsRepository.UpdateAsync(tournament);
        IEnumerable<Match> createdMatches = tournament.Matches.Where(m => createdMatchIds.Contains(m.OsuId));

        return mapper.Map<IEnumerable<MatchCreatedResultDTO>>(createdMatches);
    }

    public async Task<PagedResultDTO<MatchDTO>> GetAsync(int limit, int page, bool filterUnverified = true)
    {
        IEnumerable<Match> result = await matchesRepository.GetAsync(limit, page, filterUnverified);
        var count = result.Count();

        return new PagedResultDTO<MatchDTO>
        {
            Next = count == limit
                ? urlHelperService.Action(new CreatedAtRouteValues
                {
                    Controller = nameof(MatchesController),
                    Action = nameof(MatchesController.ListAsync),
                    RouteValues = new { limit, page = page + 1, filterUnverified }
                })
                : null,
            Previous = page > 1
                ? urlHelperService.Action(new CreatedAtRouteValues
                {
                    Controller = nameof(MatchesController),
                    Action = nameof(MatchesController.ListAsync),
                    RouteValues = new { limit, page = page - 1, filterUnverified }
                })
                : null,
            Count = count,
            Results = mapper.Map<IEnumerable<MatchDTO>>(result).ToList()
        };
    }

    public async Task<MatchDTO?> GetAsync(int id, bool filterInvalid = true) =>
            mapper.Map<MatchDTO?>(await matchesRepository.GetAsync(id, filterInvalid));

    public async Task<IEnumerable<MatchDTO>> GetAllForPlayerAsync(
        long osuPlayerId,
        int mode,
        DateTime start,
        DateTime end
    )
    {
        IEnumerable<Match> matches = await matchesRepository.GetPlayerMatchesAsync(osuPlayerId, mode, start, end);
        return mapper.Map<IEnumerable<MatchDTO>>(matches);
    }

    public async Task<IEnumerable<MatchSearchResultDTO>> SearchAsync(string name) => mapper.Map<IEnumerable<MatchSearchResultDTO>>(
        await matchesRepository.SearchAsync(name)
    );

    public async Task<MatchDTO?> GetByOsuIdAsync(long osuMatchId)
    {
        Match? match = await matchesRepository.GetByMatchIdAsync(osuMatchId);
        return mapper.Map<MatchDTO?>(match);
    }

    public async Task<MatchDTO?> UpdateVerificationStatusAsync(
        int id,
        VerificationStatus verificationStatus,
        int? verifierId = null
    ) =>
        mapper.Map<MatchDTO?>(await matchesRepository.UpdateVerificationStatusAsync(id, verificationStatus, verifierId));
}
