using Reasoning.Adaptation.Aggregator.Abstractions;
using Reasoning.Adaptation.Aggregator.Implementations;
using Reasoning.Adaptation.Aggregator.Policy.Abstractions;

namespace Reasoning.Adaptation.Aggregator.Policy.Implementations;

public readonly record struct ExponentialMovingAveragePolicy(double Alpha) : IWeightAggregatorPolicy
{
    private static readonly ExponentialMovingAveragePolicy DefaultInstance = new(ExponentialMovingAverage.DefaultAlpha);
    
    public IWeightAggregator Create() => 
        new ExponentialMovingAverage(Alpha);

    public static IWeightAggregatorPolicy Default =>
        DefaultInstance;
}