using Kernel.Function.Abstractions;
using Kernel.Function.Extensions.Metric;

namespace Kernel.Function.Implication.Abstractions;

public interface IImplicationEvaluator
{
    double Evaluate(MetricType metricType, MeasurableFunction function, uint precision);
}