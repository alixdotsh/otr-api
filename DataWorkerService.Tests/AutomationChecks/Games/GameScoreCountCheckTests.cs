using Database.Entities;
using Database.Enums.Verification;
using DataWorkerService.AutomationChecks.Games;
using TestingUtils.SeededData;

namespace DataWorkerService.Tests.AutomationChecks.Games;

public class GameScoreCountCheckTests : AutomationChecksTestBase<GameScoreCountCheck>
{
    [Fact]
    public void Check_GivenNoScores_FailsWith_NoScores()
    {
        // Arrange
        Game game = SeededGame.Generate(rejectionReason: GameRejectionReason.None);

        // Act
        var actualPass = AutomationCheck.Check(game);

        // Assert
        Assert.False(actualPass);
        Assert.Empty(game.Scores);
        Assert.Equal(GameRejectionReason.NoScores, game.RejectionReason);
    }

    [Fact]
    public void Check_GivenOnlyPreRejectedAndRejectedScores_FailsWith_NoValidScores()
    {
        // Arrange
        Game game = SeededGame.Generate(rejectionReason: GameRejectionReason.None);

        foreach (var _ in Enumerable.Range(0, 2))
        {
            SeededScore.Generate(verificationStatus: VerificationStatus.Rejected, game: game);
        }

        foreach (var _ in Enumerable.Range(0, 2))
        {
            SeededScore.Generate(verificationStatus: VerificationStatus.PreRejected, game: game);
        }

        // Act
        var actualPass = AutomationCheck.Check(game);

        // Assert
        Assert.False(actualPass);
        Assert.DoesNotContain(game.Scores, score =>
            score.VerificationStatus is VerificationStatus.PreVerified or VerificationStatus.Verified
        );
        Assert.Equal(GameRejectionReason.NoValidScores, game.RejectionReason);
    }

    [Fact]
    public void Check_GivenPreVerifiedAndVerifiedScores_ConsidersValidScores()
    {
        // Arrange
        Game game = SeededGame.Generate(rejectionReason: GameRejectionReason.None);

        foreach (var _ in Enumerable.Range(1, 3))
        {
            SeededScore.Generate(verificationStatus: VerificationStatus.PreVerified, game: game);
        }

        foreach (var _ in Enumerable.Range(1, 3))
        {
            SeededScore.Generate(verificationStatus: VerificationStatus.Verified, game: game);
        }

        // Act
        AutomationCheck.Check(game);

        // Assert
        Assert.NotEqual(GameRejectionReason.NoScores, game.RejectionReason);
        Assert.NotEqual(GameRejectionReason.NoValidScores, game.RejectionReason);
    }

    [Fact]
    public void Check_GivenVerifiedScoresCount_LessThanTournamentTeamSize_FailsWith_TeamSizeMissMatch()
    {
        // Arrange
        Game game = SeededGame.Generate(rejectionReason: GameRejectionReason.None);

        SeededScore.Generate(verificationStatus: VerificationStatus.PreVerified, game: game);
        SeededScore.Generate(verificationStatus: VerificationStatus.Verified, game: game);

        game.Match.Tournament.LobbySize = 4;

        // Act
        var actualPass = AutomationCheck.Check(game);

        // Assert
        Assert.False(actualPass);
        Assert.Equal(GameRejectionReason.TeamSizeMismatch, game.RejectionReason);
    }

    [Theory]
    [InlineData(0, 0, 1, false, GameRejectionReason.NoScores)]
    [InlineData(0, 2, 1, false, GameRejectionReason.NoValidScores)]
    [InlineData(1, 3, 1, false, GameRejectionReason.TeamSizeMismatch)]
    [InlineData(9, 0, 4, false, GameRejectionReason.TeamSizeMismatch)]
    [InlineData(2, 7, 1, true, GameRejectionReason.None)]
    [InlineData(4, 1, 2, true, GameRejectionReason.None)]
    [InlineData(6, 0, 3, true, GameRejectionReason.None)]
    [InlineData(8, 0, 4, true, GameRejectionReason.None)]
    public void Check_PassesWhenExpected(
        int verifiedScores,
        int rejectedScores,
        int tournamentTeamSize,
        bool expectedPass,
        GameRejectionReason expectedRejectionReason
    )
    {
        // Arrange
        Game game = SeededGame.Generate(rejectionReason: GameRejectionReason.None);

        if (verifiedScores >= 1)
        {
            foreach (var _ in Enumerable.Range(1, verifiedScores))
            {
                SeededScore.Generate(verificationStatus: VerificationStatus.Verified, game: game);
            }
        }

        if (rejectedScores >= 1)
        {
            foreach (var _ in Enumerable.Range(1, rejectedScores))
            {
                SeededScore.Generate(verificationStatus: VerificationStatus.Rejected, game: game);
            }
        }

        game.Match.Tournament.LobbySize = tournamentTeamSize;

        // Act
        var actualPass = AutomationCheck.Check(game);

        // Assert
        Assert.Equal(expectedPass, actualPass);
        Assert.Equal(expectedRejectionReason, game.RejectionReason);
    }
}
