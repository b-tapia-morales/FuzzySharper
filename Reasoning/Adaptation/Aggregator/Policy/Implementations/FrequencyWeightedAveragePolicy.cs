using Reasoning.Adaptation.Aggregator.Abstractions;
using Reasoning.Adaptation.Aggregator.Implementations;
using Reasoning.Adaptation.Aggregator.Policy.Abstractions;

namespace Reasoning.Adaptation.Aggregator.Policy.Implementations;

public readonly record struct FrequencyWeightedAveragePolicy(uint DecimalPlaces) : IWeightAggregatorPolicy
{
    private static readonly FrequencyWeightedAveragePolicy DefaultInstance = new(FrequencyWeightedAverage.DefaultDecimalPlaces);

    public IWeightAggregator Create() =>
        new FrequencyWeightedAverage(DecimalPlaces);

    public static IWeightAggregatorPolicy Default =>
        DefaultInstance;
}