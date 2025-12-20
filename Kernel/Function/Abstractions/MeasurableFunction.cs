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

public abstract class MeasurableFunction : MembershipFunction
{
    protected virtual DeferredValue<double> DeferredArea { get; }
    protected virtual Option<DeferredValue<double>> ClippedArea { get; } = Option<DeferredValue<double>>.None<DeferredValue<double>>();
    protected virtual DeferredValue<double> DeferredMomentX { get; }
    protected virtual Option<DeferredValue<double>> ClippedMomentX { get; } = Option<DeferredValue<double>>.None<DeferredValue<double>>();
    protected virtual DeferredValue<double> DeferredMomentY { get; }
    protected virtual Option<DeferredValue<double>> ClippedMomentY { get; } = Option<DeferredValue<double>>.None<DeferredValue<double>>();
    protected virtual DeferredValue<double> DeferredMomentXx { get; }
    protected virtual Option<DeferredValue<double>> ClippedMomentXx { get; } = Option<DeferredValue<double>>.None<DeferredValue<double>>();
    protected virtual DeferredValue<double> DeferredMomentXy { get; }
    protected virtual Option<DeferredValue<double>> ClippedMomentXy { get; } = Option<DeferredValue<double>>.None<DeferredValue<double>>();
    protected virtual DeferredValue<double> DeferredMomentYy { get; }
    protected virtual Option<DeferredValue<double>> ClippedMomentYy { get; } = Option<DeferredValue<double>>.None<DeferredValue<double>>();
    internal bool IsClipped { get; }

    protected MeasurableFunction(string name, Interval universe, double uMax) : base(name, universe, uMax)
    {
        var resolver = ResolveToOrder;

        var clippingPlan = ClippingPlan.BuildPlan(this);
        IsClipped = clippingPlan.Mode != ClippingMode.None;

        DeferredArea = new DeferredValue<double>(() => this.CalculateArea(), () => resolver(MetricOrder.Zeroth));
        DeferredMomentX = new DeferredValue<double>(() => this.CalculateFirstMoment(Axis.X), () => resolver(MetricOrder.First));
        DeferredMomentY = new DeferredValue<double>(() => this.CalculateFirstMoment(Axis.Y), () => resolver(MetricOrder.First));
        DeferredMomentXx = new DeferredValue<double>(() => this.CalculateSecondMoment(Axis.X, Axis.X), () => resolver(MetricOrder.Second));
        DeferredMomentXy = new DeferredValue<double>(() => this.CalculateSecondMoment(Axis.X, Axis.Y), () => resolver(MetricOrder.Second));
        DeferredMomentYy = new DeferredValue<double>(() => this.CalculateSecondMoment(Axis.Y, Axis.Y), () => resolver(MetricOrder.Second));

        ClippedArea = IsClipped
            ? Option<DeferredValue<double>>.Some(new DeferredValue<double>(() => CalculateClippedMetric(clippingPlan, MetricType.Area), () => resolver(MetricOrder.Zeroth)))
            : Option<DeferredValue<double>>.None<DeferredValue<double>>();
        ClippedMomentX = IsClipped
            ? Option<DeferredValue<double>>.Some(new DeferredValue<double>(() => CalculateClippedMetric(clippingPlan, MetricType.MomentX), () => resolver(MetricOrder.First)))
            : Option<DeferredValue<double>>.None<DeferredValue<double>>();
        ClippedMomentY = IsClipped
            ? Option<DeferredValue<double>>.Some(new DeferredValue<double>(() => CalculateClippedMetric(clippingPlan, MetricType.MomentY), () => resolver(MetricOrder.First)))
            : Option<DeferredValue<double>>.None<DeferredValue<double>>();
        ClippedMomentXx = IsClipped
            ? Option<DeferredValue<double>>.Some(new DeferredValue<double>(() => CalculateClippedMetric(clippingPlan, MetricType.MomentXx), () => resolver(MetricOrder.Second)))
            : Option<DeferredValue<double>>.None<DeferredValue<double>>();
        ClippedMomentXy = IsClipped
            ? Option<DeferredValue<double>>.Some(new DeferredValue<double>(() => CalculateClippedMetric(clippingPlan, MetricType.MomentXy), () => resolver(MetricOrder.Second)))
            : Option<DeferredValue<double>>.None<DeferredValue<double>>();
        ClippedMomentYy = IsClipped
            ? Option<DeferredValue<double>>.Some(new DeferredValue<double>(() => CalculateClippedMetric(clippingPlan, MetricType.MomentYy), () => resolver(MetricOrder.Second)))
            : Option<DeferredValue<double>>.None<DeferredValue<double>>();
    }

    public virtual double Area(MetricScope scope = MetricScope.Clipped) =>
        IsClipped && scope == MetricScope.Clipped ? ClippedArea.Get.Value : DeferredArea.Value;

    public virtual double MomentX(MetricScope scope = MetricScope.Clipped) =>
        IsClipped && scope == MetricScope.Clipped ? ClippedMomentX.Get.Value : DeferredMomentX.Value;

    public virtual double MomentY(MetricScope scope = MetricScope.Clipped) =>
        IsClipped && scope == MetricScope.Clipped ? ClippedMomentY.Get.Value : DeferredMomentY.Value;

    public virtual double MomentXx(MetricScope scope = MetricScope.Clipped) =>
        IsClipped && scope == MetricScope.Clipped ? ClippedMomentXx.Get.Value : DeferredMomentXx.Value;

    public virtual double MomentXy(MetricScope scope = MetricScope.Clipped) =>
        IsClipped && scope == MetricScope.Clipped ? ClippedMomentXy.Get.Value : DeferredMomentXy.Value;

    public virtual double MomentYy(MetricScope scope = MetricScope.Clipped) =>
        IsClipped && scope == MetricScope.Clipped ? ClippedMomentYy.Get.Value : DeferredMomentYy.Value;

    public virtual double EffectiveSupportLength() =>
        EffectiveSupportRight - EffectiveSupportLeft;

    public virtual double AverageHeight(MetricScope scope = MetricScope.Clipped) =>
        Area(scope) / EffectiveSupportLength();

    public virtual double CentroidX(MetricScope scope = MetricScope.Clipped) =>
        MomentX(scope) / Area(scope);

    public virtual double CentroidY(MetricScope scope = MetricScope.Clipped) =>
        MomentY(scope) / Area(scope);

    public virtual double VarianceX(MetricScope scope = MetricScope.Clipped) =>
        MomentXx(scope) / Area(scope) - Pow(CentroidX(scope), 2);

    public virtual double VarianceY(MetricScope scope = MetricScope.Clipped) =>
        MomentYy(scope) / Area(scope) - Pow(CentroidY(scope), 2);

    public virtual double Covariance(MetricScope scope = MetricScope.Clipped) =>
        MomentXy(scope) / Area(scope) - CentroidX(scope) * CentroidY(scope);

    public virtual double StandardDeviationX(MetricScope scope = MetricScope.Clipped) =>
        Sqrt(VarianceX(scope));

    public virtual double StandardDeviationY(MetricScope scope = MetricScope.Clipped) =>
        Sqrt(VarianceY(scope));

    public virtual double EigenvalueMajor(MetricScope scope = MetricScope.Clipped) =>
        (VarianceX(scope) + VarianceY(scope) + Sqrt(Pow(VarianceX(scope) - VarianceY(scope), 2) + 4 * Pow(Covariance(scope), 2))) / 2;

    public virtual double EigenvalueMinor(MetricScope scope = MetricScope.Clipped) =>
        (VarianceX(scope) + VarianceY(scope) - Sqrt(Pow(VarianceX(scope) - VarianceY(scope), 2) + 4 * Pow(Covariance(scope), 2))) / 2;

    public virtual double PrincipalAxisAngle(MetricScope scope = MetricScope.Clipped) =>
        (1 / 2.0) * Atan2(2 * Covariance(scope), VarianceX(scope) - VarianceY(scope));

    public virtual double Eccentricity(MetricScope scope = MetricScope.Clipped) =>
        Sqrt(1 - EigenvalueMinor(scope) / EigenvalueMajor(scope));

    public virtual double AxisRatio(MetricScope scope = MetricScope.Clipped) =>
        Sqrt(EigenvalueMinor(scope) / EigenvalueMajor(scope));

    public virtual double MajorMomentCompactness(MetricScope scope = MetricScope.Clipped) =>
        Area(scope) / Sqrt(PI * EigenvalueMajor(scope));

    public virtual double InertiaRatio(MetricScope scope = MetricScope.Clipped) =>
        EigenvalueMinor(scope) / EigenvalueMajor(scope);

    public virtual double ShearStrength(MetricScope scope = MetricScope.Clipped) =>
        Covariance(scope) / (VarianceX(scope) + VarianceY(scope));

    public virtual double OrientationStability(MetricScope scope = MetricScope.Clipped) =>
        (EigenvalueMajor(scope) - EigenvalueMinor(scope)) / (EigenvalueMajor(scope) + EigenvalueMinor(scope));

    private void ResolveToOrder(MetricOrder order)
    {
        if (order >= MetricOrder.Zeroth)
            ResolveToZerothOrder();
        if (order >= MetricOrder.First)
            ResolveToFirstOrder();
        if (order >= MetricOrder.Second)
            ResolveToSecondOrder();
    }

    private void ResolveToZerothOrder()
    {
        if (!DeferredArea.HasValue)
            DeferredArea.Compute();
        if (ClippedArea.IsSome(out var deferredArea) && !deferredArea.HasValue)
            deferredArea.Compute();
    }

    private void ResolveToFirstOrder()
    {
        if (!DeferredMomentX.HasValue)
            DeferredMomentX.Compute();
        if (ClippedMomentX.IsSome(out var deferredMomentX) && !deferredMomentX.HasValue)
            deferredMomentX.Compute();
        if (!DeferredMomentY.HasValue)
            DeferredMomentY.Compute();
        if (ClippedMomentY.IsSome(out var deferredMomentY) && !deferredMomentY.HasValue)
            deferredMomentY.Compute();
    }

    private void ResolveToSecondOrder()
    {
        if (!DeferredMomentXx.HasValue)
            DeferredMomentXx.Compute();
        if (ClippedMomentXx.IsSome(out var deferredMomentXx) && !deferredMomentXx.HasValue)
            deferredMomentXx.Compute();
        if (!DeferredMomentXy.HasValue)
            DeferredMomentXy.Compute();
        if (ClippedMomentXy.IsSome(out var deferredMomentXy) && !deferredMomentXy.HasValue)
            deferredMomentXy.Compute();
        if (!DeferredMomentYy.HasValue)
            DeferredMomentYy.Compute();
        if (ClippedMomentYy.IsSome(out var deferredMomentYy) && !deferredMomentYy.HasValue)
            deferredMomentYy.Compute();
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
        Debug.Assert(clippingPlan is {HasLeftCut: true, LeftCut.IsSome: true} and {HasRightCut: true, RightCut.IsSome: true});
        return metricType.CalculateMetric(this, clippingPlan.LeftCut.Get, clippingPlan.RightCut.Get);
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
}

public enum MetricOrder
{
    Zeroth,
    First,
    Second
}