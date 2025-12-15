using Inference.Aggregator.Abstractions;

namespace Inference.Aggregator.Implementations;

public class MeanAggregator: IValueAggregator
{
    public double Aggregate(IList<double> values) => 
        values.Average();
}