using Reasoning.Proposition.Abstractions;
using Reasoning.Rule.Functional.Policy.Abstractions;
using Reasoning.Rule.Functional.Policy.Factory;

namespace Reasoning.Rule.Functional.Policy.Implementations;

public class PolarityAwareInit(SkewnessMetric metric) : ICoefficientInitPolicy<PolarityAwareInit>
{
    public SkewnessMetric Metric { get; } = metric;
    
    public static PolarityAwareInit Default => new(SkewnessMetric.Centroid);

    private static readonly Dictionary<SkewnessMetric, CoefficientInitMethod> Dict = new()
    {
        {SkewnessMetric.Centroid, CoefficientInitMethod.Centroid},
        {SkewnessMetric.NormalizedCentroid, CoefficientInitMethod.NormalizedCentroid},
        {SkewnessMetric.MeanDeviation, CoefficientInitMethod.MeanDeviation},
        {SkewnessMetric.NormalizedMeanDeviation, CoefficientInitMethod.NormalizedMeanDeviation}
    };

    public (IList<double> Coefficients, double Bias) Initialize(IList<IProposition> propositions)
    {
        var coefficients = CoefficientInitFactory.GetInstance(Dict[Metric]).Initialize(propositions).Coefficients;
        var midpoints = CoefficientInitFactory.GetInstance(CoefficientInitMethod.SupportMidpoint).Initialize(propositions).Coefficients;
        return ([..coefficients.Zip(midpoints, (coefficient, midpoint) => coefficient < midpoint ? -coefficient : coefficient)], 0);
    }
}

public enum SkewnessMetric
{
    Centroid,
    NormalizedCentroid,
    MeanDeviation,
    NormalizedMeanDeviation
}