using Kernel.Function.Abstractions;
using Kernel.Function.Extensions.Metric;
using Kernel.Number;
using Shared.Approx;
using Shared.Intervals.Implementations;
using Shared.Options.Factory;
using Shared.Options.Implementations;
using static System.Math;

// ReSharper disable MemberCanBePrivate.Global

namespace Kernel.Function.Implementations;

public class LogisticFunction : SigmoidFunction
{
    internal static IMembershipFunction Create(string name, double a, double c, Interval universe, double uMax = 1)
    {
        CheckWidth(a);
        var function = new LogisticFunction(name, a, c, universe, uMax);
        function.InitializeMetrics();
        return function;

        static void CheckWidth(double a)
        {
            if (a.IsRoughlyZero())
                throw new ArgumentException($"Expected a != 0. Received a = {a}.");
        }
    }

    public static IMembershipFunction Create(string name, double a, double c, double uMax = 1) =>
        Create(name, a, c, Interval.Default, uMax);

    private LogisticFunction(string name, double a, double c, double uMax) :
        this(name, a, c, Interval.Default, uMax)
    {
    }

    private LogisticFunction(string name, double a, double c, Interval universe, double uMax) :
        base(name, universe, uMax)
    {
        A = a;
        C = c;
        var d = Log(1 / Q - 1) / Abs(a);
        EffectiveSupportLeft = C - d;
        EffectiveSupportRight = C + d;
    }

    public double A { get; }
    public double C { get; }

    private const double Q = 2 * DoubleApproxExt.DefaultTolerance;

    public override double Center => C;

    public override double Steepness => A;

    public override double EffectiveSupportLeft { get; }

    public override double EffectiveSupportRight { get; }

    public override Option<double> AlphaCutLeft(FuzzyNumber alpha)
    {
        if (alpha.Value.IsRoughlyGreaterOrEqualTo(UMax) || alpha.Value.IsRoughlyZero())
            return Option<double>.None();
        return SaturatesLeft ? double.NegativeInfinity : C - Log(UMax / alpha.Value - 1) / A;
    }

    public override Option<double> AlphaCutRight(FuzzyNumber alpha)
    {
        if (alpha.Value.IsRoughlyGreaterOrEqualTo(UMax) || alpha.Value.IsRoughlyZero())
            return Option<double>.None();
        return SaturatesRight ? double.PositiveInfinity : C - Log(UMax / alpha.Value - 1) / A;
    }

    public override Func<double, double> LarsenProduct(FuzzyNumber lambda) =>
        x => lambda.Value * UMax * (1 / (1 + Exp(-A * (x - C))));

    protected override double CalculateOriginalMetric(MetricType metricType) => metricType switch
    {
        MetricType.Area => CalculateOriginalArea(),
        MetricType.MomentY => CalculateOriginalMomentY(),
        MetricType.MomentYy => CalculateOriginalMomentYy(),
        _ => base.CalculateOriginalMetric(metricType)
    };

    // Area = μMax / a * [Log(1 + Exp(a * (x1 - c))) - Log(1 + Exp(a * (x0 - c)))]
    private double CalculateOriginalArea()
    {
        var (x0, x1) = EffectiveSupport.ToTuple();
        var l0 = Log(1 + Exp(A * (x0 - C)));
        var l1 = Log(1 + Exp(A * (x1 - C)));
        return (UMax / A) * (l1 - l0);
    }

    // My = (1/2) ⋅ ∫ [f(x)]² dx
    // s(x) = 1 / (1 + Exp(a * (x - c))), the complement of the normalized sigmoid.
    private double CalculateOriginalMomentY()
    {
        var (x0, x1) = EffectiveSupport.ToTuple();
        var s0 = 1 / (1 + Exp(A * (x0 - C)));
        var s1 = 1 / (1 + Exp(A * (x1 - C)));
        var l0 = Log(1 + Exp(A * (x0 - C)));
        var l1 = Log(1 + Exp(A * (x1 - C)));
        return (Pow(UMax, 2) / (2 * A)) * (l1 - l0 + s1 - s0);
    }

    // Myy = (1/3) ⋅ ∫ [f(x)]³ dx
    // s(x) uses the same complement convention as My.
    private double CalculateOriginalMomentYy()
    {
        var (x0, x1) = EffectiveSupport.ToTuple();
        var s0 = 1 / (1 + Exp(A * (x0 - C)));
        var s1 = 1 / (1 + Exp(A * (x1 - C)));
        var l0 = Log(1 + Exp(A * (x0 - C)));
        var l1 = Log(1 + Exp(A * (x1 - C)));
        return (Pow(UMax, 3) / (3 * A)) *
               (l1 - l0 + 2 * (s1 - s0) - (s1 * s1 - s0 * s0) / 2);
    }

    public override IMembershipFunction DeepCopy(bool copyState = true) =>
        DeepCopy(Name, copyState);

    public override IMembershipFunction DeepCopy(string name, bool copyState = true)
    {
        var copy = new LogisticFunction(name, A, C, copyState ? UniverseOfDiscourse : Interval.Default, UMax);
        copy.CopyMetrics(this, copyState);
        return copy;
    }

    #region ObjectOverrides

    public override string ToString() =>
        $"Linguistic term: {Name} - Membership Function: {GetType().Name} - Sides: (A: {A}, C: {C}) - μMax: {UMax}";

    #endregion

}
