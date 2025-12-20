using Reasoning.Adaptation.Aggregator.Abstractions;

namespace Reasoning.Adaptation.Aggregator.Policy.Abstractions;

public interface IWeightAggregatorPolicy
{
    IWeightAggregator Create();
}