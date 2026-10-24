using System.Diagnostics;
using Kernel.Function.Extensions;
using Kernel.Function.Extensions.Clipping;
using Kernel.Function.Extensions.Metric;
using Kernel.Function.Implementations;
using Shared.Deferred;
using Shared.Intervals.Implementations;
using Shared.Options.Extensions;
using Shared.Options.Factory;
using Shared.Options.Implementations;
using Utils.Shape;
using static System.Math;

namespace Kernel.Function.Abstractions;

public abstract class MeasurableFunction(string name, Interval universe, double uMax) : MembershipFunction(name, universe, uMax)
{
    private bool _isResolvingPrerequisites;

    protected Option<DeferredValue<double>> DeferredArea { get; private set; } = Option<DeferredValue<double>>.None();
    protected Option<DeferredValue<double>> ClippedArea { get; private set; } = Option<DeferredValue<double>>.None();
    protected Option<DeferredValue<double>> DeferredMomentX { get; private set; } = Option<DeferredValue<double>>.None();
    protected Option<DeferredValue<double>> ClippedMomentX { get; private set; } = Option<DeferredValue<double>>.None();
    protected Option<DeferredValue<double>> DeferredMomentY { get; private set; } = Option<DeferredValue<double>>.None();
    protected Option<DeferredValue<double>> ClippedMomentY { get; private set; } = Option<DeferredValue<double>>.None();
    protected Option<DeferredValue<double>> DeferredMomentXx { get; private set; } = Option<DeferredValue<double>>.None();
    protected Option<DeferredValue<double>> ClippedMomentXx { get; private set; } = Option<DeferredValue<double>>.None();
    protected Option<DeferredValue<double>> DeferredMomentXy { get; private set; } = Option<DeferredValue<double>>.None();
    protected Option<DeferredValue<double>> ClippedMomentXy { get; private set; } = Option<DeferredValue<double>>.None();
    protected Option<DeferredValue<double>> DeferredMomentYy { get; private set; } = Option<DeferredValue<double>>.None();
    protected Option<DeferredValue<double>> ClippedMomentYy { get; private set; } = Option<DeferredValue<double>>.None();

    protected virtual double CalculateOriginalMetric(MetricType metricType) =>
        this.CalculateIntegral(metricType, EffectiveSupportLeft, EffectiveSupportRight);

    internal void InitializeMetrics()
    {
        InitializeOriginalMetrics();
        InitializeClippedMetrics();
    }

    internal void InitializeOriginalMetrics()
    {
        var resolver = ResolveToOrder;

        DeferredArea = new DeferredValue<double>(() => CalculateOriginalMetric(MetricType.Area), () => resolver(MetricOrder.Zeroth));
        DeferredMomentX = new DeferredValue<double>(() => CalculateOriginalMetric(MetricType.MomentX), () => resolver(MetricOrder.First));
        DeferredMomentY = new DeferredValue<double>(() => CalculateOriginalMetric(MetricType.MomentY), () => resolver(MetricOrder.First));
        DeferredMomentXx = new DeferredValue<double>(() => CalculateOriginalMetric(MetricType.MomentXx), () => resolver(MetricOrder.Second));
        DeferredMomentXy = new DeferredValue<double>(() => CalculateOriginalMetric(MetricType.MomentXy), () => resolver(MetricOrder.Second));
        DeferredMomentYy = new DeferredValue<double>(() => CalculateOriginalMetric(MetricType.MomentYy), () => resolver(MetricOrder.Second));
    }

    internal void InitializeClippedMetrics()
    {
        var resolver = ResolveToOrder;

        var clippingPlan = ClippingPlan.BuildPlan(this);
        var isClipped = clippingPlan.Mode != ClippingMode.None;
        if (!isClipped)
        {
            InvalidateClippedMetrics();
            return;
        }

        ClippedArea = new DeferredValue<double>(() => CalculateClippedMetric(clippingPlan, MetricType.Area), () => resolver(MetricOrder.Zeroth));
        ClippedMomentX = new DeferredValue<double>(() => CalculateClippedMetric(clippingPlan, MetricType.MomentX), () => resolver(MetricOrder.First));
        ClippedMomentY = new DeferredValue<double>(() => CalculateClippedMetric(clippingPlan, MetricType.MomentY), () => resolver(MetricOrder.First));
        ClippedMomentXx = new DeferredValue<double>(() => CalculateClippedMetric(clippingPlan, MetricType.MomentXx), () => resolver(MetricOrder.Second));
        ClippedMomentXy = new DeferredValue<double>(() => CalculateClippedMetric(clippingPlan, MetricType.MomentXy), () => resolver(MetricOrder.Second));
        ClippedMomentYy = new DeferredValue<double>(() => CalculateClippedMetric(clippingPlan, MetricType.MomentYy), () => resolver(MetricOrder.Second));
    }

    internal void InvalidateClippedMetrics()
    {
        ClippedArea = Option<DeferredValue<double>>.None();
        ClippedMomentX = Option<DeferredValue<double>>.None();
        ClippedMomentY = Option<DeferredValue<double>>.None();
        ClippedMomentXx = Option<DeferredValue<double>>.None();
        ClippedMomentXy = Option<DeferredValue<double>>.None();
        ClippedMomentYy = Option<DeferredValue<double>>.None();
    }

    protected void CopyMetrics(MeasurableFunction source, bool copyState = true)
    {
        CopyOriginalMetrics(source);
        if (!copyState)
        {
            InvalidateClippedMetrics();
            return;
        }
        CopyClippedMetrics(source);
    }

    private void CopyOriginalMetrics(MeasurableFunction source)
    {
        DeferredArea = Copy(source.DeferredArea, MetricType.Area, MetricOrder.Zeroth);
        DeferredMomentX = Copy(source.DeferredMomentX, MetricType.MomentX, MetricOrder.First);
        DeferredMomentY = Copy(source.DeferredMomentY, MetricType.MomentY, MetricOrder.First);
        DeferredMomentXx = Copy(source.DeferredMomentXx, MetricType.MomentXx, MetricOrder.Second);
        DeferredMomentXy = Copy(source.DeferredMomentXy, MetricType.MomentXy, MetricOrder.Second);
        DeferredMomentYy = Copy(source.DeferredMomentYy, MetricType.MomentYy, MetricOrder.Second);
        return;

        Option<DeferredValue<double>> Copy(Option<DeferredValue<double>> sourceMetric, MetricType metricType, MetricOrder order) =>
            CopyMetric(sourceMetric, () => CalculateOriginalMetric(metricType), () => ResolveToOrder(order));
    }

    private void CopyClippedMetrics(MeasurableFunction source)
    {
        var plan = new Lazy<ClippingPlan>(() => ClippingPlan.BuildPlan(this));

        ClippedArea = Copy(source.ClippedArea, MetricType.Area, MetricOrder.Zeroth);
        ClippedMomentX = Copy(source.ClippedMomentX, MetricType.MomentX, MetricOrder.First);
        ClippedMomentY = Copy(source.ClippedMomentY, MetricType.MomentY, MetricOrder.First);
        ClippedMomentXx = Copy(source.ClippedMomentXx, MetricType.MomentXx, MetricOrder.Second);
        ClippedMomentXy = Copy(source.ClippedMomentXy, MetricType.MomentXy, MetricOrder.Second);
        ClippedMomentYy = Copy(source.ClippedMomentYy, MetricType.MomentYy, MetricOrder.Second);
        return;

        Option<DeferredValue<double>> Copy(Option<DeferredValue<double>> sourceMetric, MetricType metricType, MetricOrder order) =>
            CopyMetric(sourceMetric, () => CalculateClippedMetric(plan.Value, metricType), () => ResolveToOrder(order));
    }

    private static Option<DeferredValue<double>> CopyMetric(Option<DeferredValue<double>> source, Func<double> compute, Action resolvePrerequisites)
    {
        if (!source.IsSome(out var original))
            return Option<DeferredValue<double>>.None();

        var copy = new DeferredValue<double>(compute, resolvePrerequisites);
        if (original.TryGetValue(out var value))
            copy.OverrideValue(value);

        return copy;
    }

    public double Area(MetricScope scope = MetricScope.Clipped) =>
        IsClippedByUoD && scope == MetricScope.Clipped ? ClippedArea.Get.Value : DeferredArea.Get.Value;

    public double MomentX(MetricScope scope = MetricScope.Clipped) =>
        IsClippedByUoD && scope == MetricScope.Clipped ? ClippedMomentX.Get.Value : DeferredMomentX.Get.Value;

    public double MomentY(MetricScope scope = MetricScope.Clipped) =>
        IsClippedByUoD && scope == MetricScope.Clipped ? ClippedMomentY.Get.Value : DeferredMomentY.Get.Value;

    public double MomentXx(MetricScope scope = MetricScope.Clipped) =>
        IsClippedByUoD && scope == MetricScope.Clipped ? ClippedMomentXx.Get.Value : DeferredMomentXx.Get.Value;

    public double MomentXy(MetricScope scope = MetricScope.Clipped) =>
        IsClippedByUoD && scope == MetricScope.Clipped ? ClippedMomentXy.Get.Value : DeferredMomentXy.Get.Value;

    public double MomentYy(MetricScope scope = MetricScope.Clipped) =>
        IsClippedByUoD && scope == MetricScope.Clipped ? ClippedMomentYy.Get.Value : DeferredMomentYy.Get.Value;

    public double EffectiveSupportLength() =>
        EffectiveSupportRight - EffectiveSupportLeft;

    public double AverageHeight(MetricScope scope = MetricScope.Clipped) =>
        Area(scope) / EffectiveSupportLength();

    public double CentroidX(MetricScope scope = MetricScope.Clipped) =>
        MomentX(scope) / Area(scope);

    public double CentroidY(MetricScope scope = MetricScope.Clipped) =>
        MomentY(scope) / Area(scope);

    public double VarianceX(MetricScope scope = MetricScope.Clipped) =>
        MomentXx(scope) / Area(scope) - Pow(CentroidX(scope), 2);

    public double VarianceY(MetricScope scope = MetricScope.Clipped) =>
        MomentYy(scope) / Area(scope) - Pow(CentroidY(scope), 2);

    public double Covariance(MetricScope scope = MetricScope.Clipped) =>
        MomentXy(scope) / Area(scope) - CentroidX(scope) * CentroidY(scope);

    public double StandardDeviationX(MetricScope scope = MetricScope.Clipped) =>
        Sqrt(VarianceX(scope));

    public double StandardDeviationY(MetricScope scope = MetricScope.Clipped) =>
        Sqrt(VarianceY(scope));

    public double EigenvalueMajor(MetricScope scope = MetricScope.Clipped) =>
        (VarianceX(scope) + VarianceY(scope) + Sqrt(Pow(VarianceX(scope) - VarianceY(scope), 2) + 4 * Pow(Covariance(scope), 2))) / 2;

    public double EigenvalueMinor(MetricScope scope = MetricScope.Clipped) =>
        (VarianceX(scope) + VarianceY(scope) - Sqrt(Pow(VarianceX(scope) - VarianceY(scope), 2) + 4 * Pow(Covariance(scope), 2))) / 2;

    public double PrincipalAxisAngle(MetricScope scope = MetricScope.Clipped) =>
        (1 / 2.0) * Atan2(2 * Covariance(scope), VarianceX(scope) - VarianceY(scope));

    public double Eccentricity(MetricScope scope = MetricScope.Clipped) =>
        Sqrt(1 - EigenvalueMinor(scope) / EigenvalueMajor(scope));

    public double AxisRatio(MetricScope scope = MetricScope.Clipped) =>
        Sqrt(EigenvalueMinor(scope) / EigenvalueMajor(scope));

    public double MajorMomentCompactness(MetricScope scope = MetricScope.Clipped) =>
        Area(scope) / Sqrt(PI * EigenvalueMajor(scope));

    public double InertiaRatio(MetricScope scope = MetricScope.Clipped) =>
        EigenvalueMinor(scope) / EigenvalueMajor(scope);

    public double ShearStrength(MetricScope scope = MetricScope.Clipped) =>
        Covariance(scope) / (VarianceX(scope) + VarianceY(scope));

    public double OrientationStability(MetricScope scope = MetricScope.Clipped) =>
        (EigenvalueMajor(scope) - EigenvalueMinor(scope)) / (EigenvalueMajor(scope) + EigenvalueMinor(scope));

    private void ResolveToOrder(MetricOrder order)
    {
        if (_isResolvingPrerequisites)
            return;

        _isResolvingPrerequisites = true;

        if (order >= MetricOrder.Zeroth)
            ResolveToZerothOrder();
        if (order >= MetricOrder.First)
            ResolveToFirstOrder();
        if (order >= MetricOrder.Second)
            ResolveToSecondOrder();

        _isResolvingPrerequisites = false;
    }

    private void ResolveToZerothOrder() =>
        ResolveMetric(DeferredArea, ClippedArea);

    private void ResolveToFirstOrder()
    {
        ResolveMetric(DeferredMomentX, ClippedMomentX);
        ResolveMetric(DeferredMomentY, ClippedMomentY);
    }

    private void ResolveToSecondOrder()
    {
        ResolveMetric(DeferredMomentXx, ClippedMomentXx);
        ResolveMetric(DeferredMomentXy, ClippedMomentXy);
        ResolveMetric(DeferredMomentYy, ClippedMomentYy);
    }

    private double CalculateClippedMetric(ClippingPlan clippingPlan, MetricType metricType)
    {
        Debug.Assert(clippingPlan.Mode != ClippingMode.None);
        return clippingPlan.Mode == ClippingMode.Inner
            ? CalculateInnerClipping(clippingPlan, metricType)
            : CalculateOuterClipping(clippingPlan, metricType);
    }

    private double CalculateInnerClipping(ClippingPlan clippingPlan, MetricType metricType)
    {
        Debug.Assert(clippingPlan.Mode != ClippingMode.None);
        var x0 = clippingPlan.LeftCut.OrElse(EffectiveSupportLeft);
        var x1 = clippingPlan.RightCut.OrElse(EffectiveSupportRight);
        return metricType.CalculateMetric(this, x0, x1);
    }

    private double CalculateOuterClipping(ClippingPlan clippingPlan, MetricType metricType)
    {
        Debug.Assert(clippingPlan.Mode != ClippingMode.None);
        var survivingRegion = metricType.GetOriginalMetric(this);
        var (fx0, fx1) = EffectiveSupport.ToTuple();
        var (ux0, ux1) = (clippingPlan.LeftCut.OrElse(0), clippingPlan.RightCut.OrElse(0));
        var leftClippedRegion = clippingPlan.HasLeftCut ? metricType.CalculateMetric(this, fx0, ux0) : 0D;
        var rightClippedRegion = clippingPlan.HasRightCut ? metricType.CalculateMetric(this, ux1, fx1) : 0D;
        return survivingRegion - (leftClippedRegion + rightClippedRegion);
    }

    private static void ResolveMetric(Option<DeferredValue<double>> original, Option<DeferredValue<double>> clipped)
    {
        if (original.IsSome(out var originalMetric) && !originalMetric.HasValue)
            originalMetric.Compute();
        if (clipped.IsSome(out var clippedMetric) && !clippedMetric.HasValue)
            clippedMetric.Compute();
    }
}

internal enum MetricOrder
{
    Zeroth,
    First,
    Second
}