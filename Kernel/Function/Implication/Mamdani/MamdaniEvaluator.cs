using Kernel.Function.Abstractions;
using Kernel.Function.Extensions;
using Kernel.Function.Extensions.Metric;
using Kernel.Function.Implication.Abstractions;
using Kernel.Number;
using Shared.Approx;
using Shared.Intervals.Implementations;
using Shared.Options.Extensions;
using Shared.Options.Factory;
using Shared.Options.Implementations;

namespace Kernel.Function.Implication.Mamdani;

public class MamdaniEvaluator : IImplicationEvaluator<MamdaniEvaluator>
{
    private bool HasCutTop { get; init; }
    private FuzzyNumber TopCut { get; init; }

    private bool HasLeftRegion { get; init; }
    private Option<Interval> LeftRegion { get; init; } = Option<Interval>.None();

    private bool HasRightRegion { get; init; }
    private Option<Interval> RightRegion { get; init; } = Option<Interval>.None();

    private Option<List<(double X, double Y)>> MiddleRegion { get; init; } = Option<List<(double, double)>>.None();

    private MamdaniOutcome Outcome { get; init; }

    public static MamdaniEvaluator BuildPlan(MeasurableFunction function, FuzzyNumber implicationParam, bool useUoD)
    {
        var hasTopCut = implicationParam.Value.IsRoughlyLesserThan(function.UMax);

        if (!hasTopCut)
            return new MamdaniEvaluator
            {
                HasCutTop = false,
                Outcome = MamdaniOutcome.NoCut
            };

        var height = Math.Min(implicationParam.Value, function.UMax);

        var (fx0, fx1) = useUoD ? function.RestrictedSupport.ToTuple() : function.EffectiveSupport.ToTuple();
        var alphaCut = useUoD ? function.AlphaCutClipped(height) : function.AlphaCut(height);

        if (alphaCut.IsNone)
            return new MamdaniEvaluator
            {
                HasCutTop = false,
                Outcome = MamdaniOutcome.ClippedByUoD
            };

        var (a0, a1) = alphaCut.Get.ToTuple();

        var hasLeftRegion = fx0.IsRoughlyLesserThan(a0);
        var leftRegion = hasLeftRegion ? new Interval(fx0, a0) : Option<Interval>.None();

        var hasRightRegion = fx1.IsRoughlyGreaterThan(a1);
        var rightRegion = hasRightRegion ? new Interval(a1, fx1) : Option<Interval>.None();

        var mode = (hasLeftRegion, hasRightRegion) switch
        {
            (true, true) => MamdaniOutcome.LeftPlusRight,
            (true, false) => MamdaniOutcome.Left,
            (false, true) => MamdaniOutcome.Right,
            (false, false) => MamdaniOutcome.MiddleOnly
        };

        List<(double, double)> vertices = [(a0, 0), (a0, height), (a1, height), (a1, 0)];

        return new MamdaniEvaluator
        {
            HasCutTop = true,
            TopCut = height,
            HasLeftRegion = hasLeftRegion,
            LeftRegion = leftRegion,
            HasRightRegion = hasRightRegion,
            RightRegion = rightRegion,
            MiddleRegion = Option<List<(double, double)>>.Some(vertices),
            Outcome = mode,
        };
    }

    public double Evaluate(MetricType metricType, MeasurableFunction function, uint precision)
    {
        var leftInterval = LeftRegion.OrElse(Interval.Default);
        var leftRegion = HasLeftRegion
            ? function.CalculateIntegral(metricType, leftInterval.LowerBound, leftInterval.UpperBound, precision)
            : 0D;

        var rightInterval = RightRegion.OrElse(Interval.Default);
        var rightRegion = HasRightRegion
            ? function.CalculateIntegral(metricType, rightInterval.LowerBound, rightInterval.UpperBound, precision)
            : 0D;

        var middleRegion = Outcome >= MamdaniOutcome.MiddleOnly
            ? metricType.CalculateMetric(MiddleRegion.Get)
            : 0D;

        return Outcome switch
        {
            MamdaniOutcome.NoCut => metricType.GetOriginalMetric(function),
            MamdaniOutcome.ClippedByUoD => metricType.GetClippedMetric(function).Get,
            MamdaniOutcome.MiddleOnly => middleRegion,
            MamdaniOutcome.Left => leftRegion + middleRegion,
            MamdaniOutcome.Right => middleRegion + rightRegion,
            MamdaniOutcome.LeftPlusRight => leftRegion + middleRegion + rightRegion,
            _ => throw new ArgumentOutOfRangeException(nameof(metricType), metricType, null)
        };
    }
}