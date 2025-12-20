using Reasoning.Adaptation.Aggregator.Abstractions;
using Reasoning.Adaptation.Aggregator.Implementations;
using Reasoning.Adaptation.Aggregator.Policy.Abstractions;

namespace Reasoning.Adaptation.Aggregator.Policy.Implementations;

public readonly record struct ArithmeticMeanPolicy: IWeightAggregatorPolicy
{
    public IWeightAggregator Create() => new ArithmeticMean();
}