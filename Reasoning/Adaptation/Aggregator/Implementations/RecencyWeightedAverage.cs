using Kernel.Number;
using Reasoning.Adaptation.Aggregator.Abstractions;
using Reasoning.Adaptation.Components;
using Shared.Options.Factory;
using Shared.Options.Implementations;

namespace Reasoning.Adaptation.Aggregator.Implementations;

public class RecencyWeightedAverage : IWeightAggregator
{
    public Option<FuzzyNumber> Aggregate(IReadOnlyCollection<AdaptationRecord> records)
    {
        if (records.Count == 0)
            return Option<FuzzyNumber>.None();

        var numerator = records.Sum(e => (int) e.Iteration * e.Weight.Value);
        var denominator = records.Select(e => (int) e.Iteration).Average();
        return FuzzyNumber.Of(numerator / denominator);
    }
}