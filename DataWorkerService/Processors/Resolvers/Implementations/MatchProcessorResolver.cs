using Database.Entities;
using DataWorkerService.Processors.Matches;
using DataWorkerService.Processors.Resolvers.Interfaces;

namespace DataWorkerService.Processors.Resolvers.Implementations;

public class MatchProcessorResolver(
    IEnumerable<IProcessor<Match>> processors
) : ProcessorResolver<Match>(processors), IMatchProcessorResolver
{
    public IProcessor<Match> GetDataProcessor() =>
        Processors.FirstOrDefault(p => p is MatchDataProcessor)
            ?? throw new InvalidOperationException($"Processor was not registered: {nameof(MatchDataProcessor)}");

    public override IProcessor<Match> GetAutomationChecksProcessor() =>
        Processors.FirstOrDefault(p => p is MatchAutomationChecksProcessor)
        ?? throw new InvalidOperationException($"Processor was not registered: {nameof(MatchAutomationChecksProcessor)}");

    public IProcessor<Match> GetStatsProcessor() =>
        Processors.FirstOrDefault(p => p is MatchStatsProcessor)
        ?? throw new InvalidOperationException($"Processor was not registered: {nameof(MatchStatsProcessor)}");
}
