using Reasoning.Adaptation.Aggregator.Factory;
using Reasoning.Adaptation.Aggregator.Policy.Abstractions;
using Reasoning.Adaptation.Aggregator.Policy.Implementations;

namespace Reasoning.Adaptation.Aggregator.Policy.Factory;

public static class WeightAggregationPolicyFactory
{
    private static readonly IWeightAggregatorPolicy ArithmeticMean = new ArithmeticMeanPolicy();
    private static readonly IWeightAggregatorPolicy ExponentialMovingAverage = new ExponentialMovingAveragePolicy();
    private static readonly IWeightAggregatorPolicy RecencyWeightedAverage = new RecencyWeightedAveragePolicy();
    private static readonly IWeightAggregatorPolicy FrequencyWeightedAverage = new FrequencyWeightedAveragePolicy();
    private static readonly IWeightAggregatorPolicy LatestValue = new LatestValuePolicy();

    public static IWeightAggregatorPolicy GetInstance(WeightAggregationMethod method) =>
        method switch
        {
            WeightAggregationMethod.ArithmeticMean => ArithmeticMean,
            WeightAggregationMethod.ExponentialMovingAverage => ExponentialMovingAverage,
            WeightAggregationMethod.RecencyWeightedAverage => RecencyWeightedAverage,
            WeightAggregationMethod.FrequencyWeightedAverage => FrequencyWeightedAverage,
            WeightAggregationMethod.LatestValue => LatestValue,
            _ => throw new ArgumentOutOfRangeException(nameof(method), method, null)
        };
}