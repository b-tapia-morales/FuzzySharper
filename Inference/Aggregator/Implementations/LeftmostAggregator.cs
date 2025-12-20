using Inference.Aggregator.Abstractions;

namespace Inference.Aggregator.Implementations;

public class LeftmostAggregator: IValueAggregator
{
    public double Aggregate(IList<double> values) => 
        values[0];
}