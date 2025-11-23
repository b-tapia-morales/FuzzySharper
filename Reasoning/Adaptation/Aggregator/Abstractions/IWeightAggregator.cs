using Kernel.Number;
using Reasoning.Adaptation.Components;
using Shared.Options.Implementations;

namespace Reasoning.Adaptation.Aggregator.Abstractions;

public interface IWeightAggregator
{
    Option<FuzzyNumber> Aggregate(IReadOnlyCollection<AdaptationRecord> records);
}