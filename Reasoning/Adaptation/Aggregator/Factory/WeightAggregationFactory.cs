using Reasoning.Adaptation.Aggregator.Abstractions;
using Reasoning.Adaptation.Aggregator.Implementations;

namespace Reasoning.Adaptation.Aggregator.Factory;

public static class WeightAggregationFactory
{
    private static readonly IWeightAggregator ArithmeticMean = new ArithmeticMean();
    private static readonly IWeightAggregator ExponentialMovingAverage = new ExponentialMovingAverage();
    private static readonly IWeightAggregator RecencyWeightedAverage = new RecencyWeightedAverage();
    private static readonly IWeightAggregator FrequencyWeightedAverage = new FrequencyWeightedAverage();
    private static readonly IWeightAggregator LatestValue = new LatestValue();

    public static IWeightAggregator GetInstance(WeightAggregationMethod method) =>
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