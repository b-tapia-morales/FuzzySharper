namespace Reasoning.Adaptation.Aggregator.Factory;

public enum WeightAggregationMethod
{
    ArithmeticMean,
    ExponentialMovingAverage,
    RecencyWeightedAverage,
    FrequencyWeightedAverage,
    LatestValue
}