using API.DTOs;
using Database.Enums;

namespace API.Services.Interfaces;

public interface ITournamentsService
{
    /// <summary>
    /// Creates a tournament from a <see cref="TournamentSubmissionDTO"/>.
    /// </summary>
    /// <param name="submission">Tournament submission data</param>
    /// <param name="submitterUserId">Id of the User that created the submission</param>
    /// <param name="preApprove">Denotes if the tournament should be pre-approved</param>
    /// <returns>Location information for the created tournament</returns>
    Task<TournamentCreatedResultDTO> CreateAsync(
        TournamentSubmissionDTO submission,
        int submitterUserId,
        bool preApprove
    );

    /// <summary>
    /// Denotes a tournament with matching name and ruleset exists
    /// </summary>
    Task<bool> ExistsAsync(string name, Ruleset ruleset);

    /// <summary>
    /// Gets all tournaments
    /// </summary>
    Task<IEnumerable<TournamentDTO>> ListAsync();

    /// <summary>
    /// Gets a tournament by id
    /// </summary>
    /// <param name="id">Primary key</param>
    /// <param name="eagerLoad">Whether to include child resources of the tournament</param>
    /// <returns>The tournament, or null if not found</returns>
    Task<TournamentDTO?> GetAsync(int id, bool eagerLoad = true);

    /// <summary>
    /// Gets the number of tournaments played by the given player
    /// </summary>
    Task<int> CountPlayedAsync(int playerId, Ruleset ruleset, DateTime? dateMin = null, DateTime? dateMax = null);

    /// <summary>
    /// Updates a tournament entity with values from a <see cref="TournamentDTO"/>
    /// </summary>
    Task<TournamentDTO?> UpdateAsync(int id, TournamentDTO wrapper);
}
