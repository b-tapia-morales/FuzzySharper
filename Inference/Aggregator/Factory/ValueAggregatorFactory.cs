using Inference.Aggregator.Abstractions;
using Inference.Aggregator.Implementations;

namespace Inference.Aggregator.Factory;

public static class ValueAggregatorFactory
{
    private static readonly LeftmostAggregator LeftmostAggregator = new();
    private static readonly MeanAggregator MeanAggregator = new();
    private static readonly RightmostAggregator RightmostAggregator = new();

    public static IValueAggregator GetInstance(ValueAggregatorMethod method) => method switch
    {
        ValueAggregatorMethod.Leftmost => LeftmostAggregator,
        ValueAggregatorMethod.Mean => MeanAggregator,
        ValueAggregatorMethod.Rightmost => RightmostAggregator,
        _ => throw new ArgumentOutOfRangeException(nameof(method), method, null)
    };
}