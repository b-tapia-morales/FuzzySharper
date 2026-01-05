using Reasoning.Proposition.Implementations;
using Reasoning.Rule.Functional.Policy.Abstractions;
using Reasoning.Rule.Functional.Policy.Factory;

namespace Reasoning.Rule.Functional.Policy.Implementations;

public class PolarityAwareInit(SkewnessMetric metric) : BaseCoefficientInitPolicy
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

    public override IEnumerable<double> Initialize(IReadOnlyList<FuzzyProposition> premise)
    {
        var coefficients = ((BaseCoefficientInitPolicy) CoefficientInitFactory.GetInstance(Dict[Metric])).Initialize(premise);
        var midpoints = ((BaseCoefficientInitPolicy) CoefficientInitFactory.GetInstance(CoefficientInitMethod.SupportMidpoint)).Initialize(premise);
        return [..coefficients.Zip(midpoints, (coefficient, midpoint) => coefficient < midpoint ? -coefficient : coefficient)];
    }
}

public enum SkewnessMetric
{
    Centroid,
    NormalizedCentroid,
    MeanDeviation,
    NormalizedMeanDeviation
}