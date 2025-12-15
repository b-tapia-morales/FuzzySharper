namespace Inference.Aggregator.Abstractions;

public interface IValueAggregator
{
    double Aggregate(IList<double> values);
}