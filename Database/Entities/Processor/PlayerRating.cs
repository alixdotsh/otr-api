﻿using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Database.Enums;

namespace Database.Entities.Processor;

/// <summary>
/// Represents a summary of current rating data for a <see cref="Player"/> in a <see cref="Enums.Ruleset"/>
/// </summary>
/// <remarks>
/// Generated by the <a href="https://github.com/osu-tournament-rating/otr-processor">o!TR Processor</a>
/// <br/><br/>
/// A <see cref="Player"/> may only have one <see cref="PlayerRating"/> per <see cref="Enums.Ruleset"/> at any given time.
/// <br/><br/>
/// For more in depth documentation, see
/// <a href="https://github.com/osu-tournament-rating/otr-wiki/blob/master/algorithm/ratings/en.md">
/// o!TR Ratings Wiki
/// </a>
/// </remarks>
[Table("player_ratings")]
[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.CircularDependency")]
public class PlayerRating : EntityBase
{
    /// <summary>
    /// The <see cref="Enums.Ruleset"/> that the <see cref="PlayerRating"/> was generated for
    /// </summary>
    [Column("ruleset")]
    public Ruleset Ruleset { get; init; }

    /// <summary>
    /// osu! Tournament Rating... The number we're all here for!
    /// </summary>
    [Column("rating")]
    public double Rating { get; set; }

    /// <summary>
    /// Measure of how "strong" a single change in <see cref="Rating"/> can be
    /// </summary>
    [Column("volatility")]
    public double Volatility { get; init; }

    /// <summary>
    /// Global <see cref="Rating"/> percentile
    /// </summary>
    [Column("percentile")]
    public double Percentile { get; init; }

    /// <summary>
    /// Global rank
    /// </summary>
    [Column("global_rank")]
    public int GlobalRank { get; init; }

    /// <summary>
    /// Country rank
    /// </summary>
    [Column("country_rank")]
    public int CountryRank { get; init; }

    /// <summary>
    /// Id of the <see cref="Player"/> that the <see cref="PlayerRating"/> was generated for
    /// </summary>
    [Column("player_id")]
    public int PlayerId { get; init; }

    /// <summary>
    /// The <see cref="Player"/> that the <see cref="PlayerRating"/> was generated for
    /// </summary>
    public Player Player { get; init; } = null!;

    /// <summary>
    /// A collection of <see cref="RatingAdjustment"/>s that represent
    /// the individual changes to the <see cref="PlayerRating"/> over time
    /// </summary>
    public ICollection<RatingAdjustment> Adjustments { get; init; } = new List<RatingAdjustment>();
}
