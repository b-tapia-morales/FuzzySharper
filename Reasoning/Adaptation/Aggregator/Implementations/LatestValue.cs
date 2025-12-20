using Kernel.Number;
using Reasoning.Adaptation.Aggregator.Abstractions;
using Reasoning.Adaptation.Components;
using Shared.Options.Factory;
using Shared.Options.Implementations;

namespace Reasoning.Adaptation.Aggregator.Implementations;

public class LatestValue : IWeightAggregator
{
    public Option<FuzzyNumber> Aggregate(IReadOnlyCollection<AdaptationRecord> records) =>
        records.Count == 0
            ? Option<FuzzyNumber>.None()
            : records.Last().Weight;
}