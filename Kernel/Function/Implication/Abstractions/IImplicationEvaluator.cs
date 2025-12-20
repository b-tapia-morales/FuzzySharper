using Kernel.Function.Abstractions;
using Kernel.Function.Extensions.Metric;
using Kernel.Number;

namespace Kernel.Function.Implication.Abstractions;

public interface IImplicationEvaluator
{
    double Evaluate(MetricType metricType, MeasurableFunction function, uint precision);
}

public interface IImplicationEvaluator<out T> : IImplicationEvaluator where T : class, IImplicationEvaluator<T>
{
    static abstract T BuildPlan(MeasurableFunction function, FuzzyNumber implicationParam, bool useUoD);
}