using Inference.Aggregator.Abstractions;

namespace Inference.Aggregator.Implementations;

public class RightmostAggregator: IValueAggregator
{
    public double Aggregate(IList<double> values) => 
        values[^1];
}