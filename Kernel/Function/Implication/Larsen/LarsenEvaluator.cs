using Kernel.Function.Abstractions;
using Kernel.Function.Extensions.Metric;
using Kernel.Function.Implication.Abstractions;
using Kernel.Number;
using Shared.Approx;

namespace Kernel.Function.Implication.Larsen;

public class LarsenEvaluator : IImplicationEvaluator<LarsenEvaluator>
{
    private bool HasCutTop { get; init; }
    private FuzzyNumber TopCut { get; init; }

    private LarsenOutcome Outcome { get; init; }

    public static LarsenEvaluator BuildPlan(MeasurableFunction function, FuzzyNumber implicationParam, bool useUoD)
    {
        var hasTopCut = implicationParam.Value.IsRoughlyLesserThan(function.UMax);

        if (!hasTopCut)
            return new LarsenEvaluator
            {
                HasCutTop = false,
                Outcome = function.IsClipped ? LarsenOutcome.NoScalingClipped : LarsenOutcome.NoScalingUnclipped
            };

        return new LarsenEvaluator
        {
            HasCutTop = true,
            TopCut = Math.Min(implicationParam.Value, function.UMax),
            Outcome = function.IsClipped ? LarsenOutcome.ScaledDownClipped : LarsenOutcome.ScaledDownUnclipped,
        };
    }

    public double Evaluate(MetricType metricType, MeasurableFunction function, uint precision) => Outcome switch
    {
        LarsenOutcome.NoScalingUnclipped => metricType.GetOriginalMetric(function),
        LarsenOutcome.NoScalingClipped => metricType.GetClippedMetric(function).Get,
        LarsenOutcome.ScaledDownUnclipped => TopCut.Value * metricType.GetOriginalMetric(function),
        LarsenOutcome.ScaledDownClipped => TopCut.Value * metricType.GetClippedMetric(function).Get,
        _ => throw new ArgumentOutOfRangeException(nameof(Outcome), Outcome, null)
    };
}