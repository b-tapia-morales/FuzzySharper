using Kernel.Function.Extensions;
using Kernel.Function.Implementations;
using Shared.Deferred;
using Shared.Intervals.Implementations;
using Utils.Shape;
using static System.Math;

namespace Kernel.Function.Abstractions;

public abstract class MeasurableFunction : MembershipFunction
{
    protected DeferredValue<double> DeferredArea { get; }
    protected DeferredValue<double> DeferredMomentX { get; }
    protected DeferredValue<double> DeferredMomentY { get; }
    protected DeferredValue<double> DeferredMomentXx { get; }
    protected DeferredValue<double> DeferredMomentXy { get; }
    protected DeferredValue<double> DeferredMomentYy { get; }

    protected MeasurableFunction(string name, Interval universe, double uMax) : base(name, universe, uMax)
    {
        var resolver = ResolveToOrder;

        DeferredArea = new DeferredValue<double>(() => this.CalculateArea(), () => resolver(MetricOrder.Zeroth));
        DeferredMomentX = new DeferredValue<double>(() => this.CalculateFirstMoment(Axis.X), () => resolver(MetricOrder.First));
        DeferredMomentY = new DeferredValue<double>(() => this.CalculateFirstMoment(Axis.Y), () => resolver(MetricOrder.First));
        DeferredMomentXx = new DeferredValue<double>(() => this.CalculateSecondMoment(Axis.X, Axis.X), () => resolver(MetricOrder.Second));
        DeferredMomentXy = new DeferredValue<double>(() => this.CalculateSecondMoment(Axis.X, Axis.Y), () => resolver(MetricOrder.Second));
        DeferredMomentYy = new DeferredValue<double>(() => this.CalculateSecondMoment(Axis.Y, Axis.Y), () => resolver(MetricOrder.Second));
    }

    public virtual double Area() =>
        DeferredArea.Value;

    public virtual double MomentX() =>
        DeferredMomentX.Value;

    public virtual double MomentY() =>
        DeferredMomentY.Value;

    public virtual double MomentXx() =>
        DeferredMomentXx.Value;

    public virtual double MomentXy() =>
        DeferredMomentXy.Value;

    public virtual double MomentYy() =>
        DeferredMomentYy.Value;

    public virtual double EffectiveSupportLength() =>
        EffectiveSupportRight - EffectiveSupportLeft;

    public virtual double AverageHeight() =>
        Area() / EffectiveSupportLength();

    public virtual double CentroidX() =>
        MomentX() / Area();

    public virtual double CentroidY() =>
        MomentY() / Area();

    public virtual double VarianceX() =>
        MomentXx() / Area() - Pow(CentroidX(), 2);

    public virtual double VarianceY() =>
        MomentYy() / Area() - Pow(CentroidY(), 2);

    public virtual double Covariance() =>
        MomentXy() / Area() - CentroidX() * CentroidY();

    public virtual double StandardDeviationX() =>
        Sqrt(VarianceX());

    public virtual double StandardDeviationY() =>
        Sqrt(VarianceY());

    public virtual double EigenvalueMajor() =>
        (VarianceX() + VarianceY() + Sqrt(Pow(VarianceX() - VarianceY(), 2) + 4 * Pow(Covariance(), 2))) / 2;

    public virtual double EigenvalueMinor() =>
        (VarianceX() + VarianceY() - Sqrt(Pow(VarianceX() - VarianceY(), 2) + 4 * Pow(Covariance(), 2))) / 2;

    public virtual double PrincipalAxisAngle() =>
        (1 / 2.0) * Atan2(2 * Covariance(), VarianceX() - VarianceY());

    public virtual double Eccentricity() =>
        Sqrt(1 - EigenvalueMinor() / EigenvalueMajor());

    public virtual double AxisRatio() =>
        Sqrt(EigenvalueMinor() / EigenvalueMajor());

    public virtual double MajorMomentCompactness() =>
        Area() / Sqrt(PI * EigenvalueMajor());

    public virtual double InertiaRatio() =>
        EigenvalueMinor() / EigenvalueMajor();

    public virtual double ShearStrength() =>
        Covariance() / (VarianceX() + VarianceY());

    public virtual double OrientationStability() =>
        (EigenvalueMajor() - EigenvalueMinor()) / (EigenvalueMajor() + EigenvalueMinor());

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
            _ = DeferredArea.Value;
    }

    private void ResolveToFirstOrder()
    {
        if (!DeferredMomentX.HasValue)
            _ = DeferredMomentX.Value;
        if (!DeferredMomentY.HasValue)
            _ = DeferredMomentY.Value;
    }

    private void ResolveToSecondOrder()
    {
        if (!DeferredMomentXx.HasValue)
            _ = DeferredMomentXx.Value;
        if (!DeferredMomentXy.HasValue)
            _ = DeferredMomentXy.Value;
        if (!DeferredMomentYy.HasValue)
            _ = DeferredMomentYy.Value;
    }
}

public enum MetricOrder
{
    Zeroth,
    First,
    Second
}