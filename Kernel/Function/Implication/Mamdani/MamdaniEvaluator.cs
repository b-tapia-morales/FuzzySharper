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
using Utils.Shape;

namespace Kernel.Function.Implication.Mamdani;

public class MamdaniEvaluator : IImplicationEvaluator<MamdaniEvaluator>
{
    private bool HasCutTop { get; init; }
    private FuzzyNumber TopCut { get; init; }

    private bool HasLeftRegion { get; init; }
    private bool HasLeftCut { get; init; }
    private Option<Interval> LeftRegion { get; init; } = Option<Interval>.None();

    private bool HasRightRegion { get; init; }
    private bool HasRightCut { get; init; }
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
                Outcome = useUoD && function.IsClippedByUoD ? MamdaniOutcome.ClippedByUoD : MamdaniOutcome.NoCut
            };

        var height = Math.Min(implicationParam.Value, function.UMax);

        var (fx0, fx1) = useUoD ? function.RestrictedSupport.ToTuple() : function.EffectiveSupport.ToTuple();
        var alphaCut = useUoD ? function.AlphaCutClipped(height) : function.AlphaCut(height);

        if (alphaCut.IsNone)
            return new MamdaniEvaluator
            {
                HasCutTop = false,
                Outcome = useUoD && function.IsClippedByUoD ? MamdaniOutcome.ClippedByUoD : MamdaniOutcome.NoCut
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
            HasLeftCut = useUoD && hasLeftRegion && fx0.IsRoughlyGreaterThan(function.EffectiveSupportLeft),
            LeftRegion = leftRegion,
            HasRightRegion = hasRightRegion,
            HasRightCut = useUoD && hasRightRegion && fx1.IsRoughlyLesserThan(function.EffectiveSupportRight),
            RightRegion = rightRegion,
            MiddleRegion = Option<List<(double, double)>>.Some(vertices),
            Outcome = mode,
        };
    }

    public double Evaluate(MetricType metricType, MeasurableFunction function, uint precision)
    {
        var totalArea = CalculateTotalArea(metricType, function, precision);
        var leftRegion = HasLeftRegion
            ? CalculateLeft(metricType, function, precision, totalArea)
            : 0D;

        var rightRegion = HasRightRegion
            ? CalculateRight(metricType, function, precision, totalArea)
            : 0D;

        var middleRegion = Outcome >= MamdaniOutcome.MiddleOnly
            ? CalculateMiddle(metricType, totalArea)
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

    private Option<double> CalculateTotalArea(MetricType metricType, MeasurableFunction function, uint precision)
    {
        if (metricType is not (MetricType.CentroidX or MetricType.CentroidY) || MiddleRegion.IsNone)
            return Option<double>.None();
        var leftArea = HasLeftRegion ? CalculateLeft(MetricType.Area, function, precision, Option<double>.None()) : 0D;
        var rightArea = HasRightRegion ? CalculateRight(MetricType.Area, function, precision, Option<double>.None()) : 0D;
        var middleArea = PolygonUtils.CalculateArea(MiddleRegion.Get);
        return leftArea + middleArea + rightArea;
    }

    private double CalculateSide(MetricType metricType, MeasurableFunction function, uint precision, bool isLeft, Option<double> totalArea)
    {
        var isPiecewise = function is LinearPiecewiseFunction;
        var isCentroid = metricType is MetricType.CentroidX or MetricType.CentroidY;
        var axis = metricType == MetricType.CentroidX ? Axis.X : Axis.Y;
        var region = isLeft ? LeftRegion.Get : RightRegion.Get;
        var hasCut = isLeft ? HasLeftCut : HasRightCut;

        LinearPiecewiseFunction piecewise;
        List<(double X, double Y)> vertices;
        switch (isPiecewise, isCentroid)
        {
            case (true, true):
                piecewise = (LinearPiecewiseFunction) function;
                vertices = isLeft ? piecewise.AlphaCutLeftRegion(TopCut, hasCut) : piecewise.AlphaCutRightRegion(TopCut, hasCut);
                return PolygonUtils.CalculateFirstMoment(vertices, axis) / totalArea.Get;
            case (true, false):
                piecewise = (LinearPiecewiseFunction) function;
                vertices = isLeft ? piecewise.AlphaCutLeftRegion(TopCut, hasCut) : piecewise.AlphaCutRightRegion(TopCut, hasCut);
                return metricType.CalculateMetric(vertices);
            case (false, true):
                return function.CalculateFirstMomentAt(axis, region.LowerBound, region.UpperBound, precision) / totalArea.Get;
            case (false, false):
                return function.CalculateIntegral(metricType, region.LowerBound, region.UpperBound, precision);
        }
    }

    private double CalculateMiddle(MetricType metricType, Option<double> totalArea)
    {
        if (metricType is not (MetricType.CentroidX or MetricType.CentroidY))
            return metricType.CalculateMetric(MiddleRegion.Get);
        var axis = metricType == MetricType.CentroidX ? Axis.X : Axis.Y;
        return PolygonUtils.CalculateFirstMoment(MiddleRegion.Get, axis) / totalArea.Get;
    }

    private double CalculateLeft(MetricType metricType, MeasurableFunction function, uint precision, Option<double> totalArea) =>
        CalculateSide(metricType, function, precision, true, totalArea);

    private double CalculateRight(MetricType metricType, MeasurableFunction function, uint precision, Option<double> totalArea) =>
        CalculateSide(metricType, function, precision, false, totalArea);
}
