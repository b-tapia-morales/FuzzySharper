using Kernel.Function.Abstractions;
using Kernel.Function.Extensions.Metric;
using Kernel.Function.Implication.Abstractions;
using Kernel.Number;

namespace Kernel.Function.Implication.Larsen;

public class LarsenEvaluator : IImplicationEvaluator<LarsenEvaluator>
{
    private double ScaleFactor { get; init; } = 1;
    private LarsenOutcome Outcome { get; init; }

    public static LarsenEvaluator BuildPlan(MeasurableFunction function, FuzzyNumber implicationParam, bool useUoD)
    {
        var hasScaling = implicationParam.Value < 1;
        var isClipped = useUoD && function.IsClippedByUoD;

        return new LarsenEvaluator
        {
            ScaleFactor = implicationParam.Value,
            Outcome = (hasScaling, isClipped) switch
            {
                (false, false) => LarsenOutcome.NoScalingUnclipped,
                (false, true) => LarsenOutcome.NoScalingClipped,
                (true, false) => LarsenOutcome.ScaledDownUnclipped,
                (true, true) => LarsenOutcome.ScaledDownClipped
            }
        };
    }

    public double Evaluate(MetricType metricType, MeasurableFunction function, uint precision)
    {
        if (ScaleFactor == 0)
            return 0;

        var unscaledMetric = GetUnscaledMetric(metricType, function, Outcome);
        var multiplier = CalculateMultiplier(metricType, ScaleFactor);
        return multiplier * unscaledMetric;
    }

    private static double GetUnscaledMetric(MetricType metricType, MeasurableFunction function, LarsenOutcome outcome) => outcome switch
    {
        LarsenOutcome.NoScalingUnclipped or LarsenOutcome.ScaledDownUnclipped => metricType.GetOriginalMetric(function),
        LarsenOutcome.NoScalingClipped or LarsenOutcome.ScaledDownClipped => metricType.GetClippedMetric(function).Get,
        _ => throw new ArgumentOutOfRangeException(nameof(outcome), outcome, null)
    };

    private static double CalculateMultiplier(MetricType metricType, double scale) => metricType switch
    {
        MetricType.Area or MetricType.MomentX or MetricType.MomentXx or MetricType.CentroidY => scale,
        MetricType.MomentY or MetricType.MomentXy => Math.Pow(scale, 2),
        MetricType.MomentYy => Math.Pow(scale, 3),
        MetricType.CentroidX => 1D,
        _ => throw new ArgumentOutOfRangeException(nameof(metricType), metricType, null)
    };
}