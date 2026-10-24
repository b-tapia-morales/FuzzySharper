using Kernel.Function.Abstractions;
using Kernel.Function.Extensions.Metric;
using Kernel.Number;
using MathNet.Numerics;
using Shared.Approx;
using Shared.Intervals.Implementations;
using Shared.Options.Factory;
using Shared.Options.Implementations;
using static System.Math;

// ReSharper disable MemberCanBePrivate.Global

namespace Kernel.Function.Implementations;

public class GaussianFunction : BellShapedFunction
{
    internal static IMembershipFunction Create(string name, double mu, double sigma, Interval universe, double uMax = 1)
    {
        CheckSigma(sigma);
        CheckMu(mu);
        var function = new GaussianFunction(name, mu, sigma, universe, uMax);
        function.InitializeMetrics();
        return function;

        static void CheckSigma(double sigma)
        {
            if (!double.IsFinite(sigma) || sigma.IsRoughlyLesserOrEqualTo(0))
                throw new ArgumentException($"Expected sigma > 0 and finite. Received sigma = {sigma}.", nameof(sigma));
        }

        static void CheckMu(double mu)
        {
            if (!double.IsFinite(mu)) 
                throw new ArgumentException($"Expected mu to be finite. Received mu = {mu}.", nameof(mu));
        }

    }

    public static IMembershipFunction Create(string name, double mu, double sigma, double uMax = 1) =>
        Create(name, mu, sigma, Interval.Default, uMax);
    
    private GaussianFunction(string name, double mu, double sigma, double uMax) :
        this(name, mu, sigma, Interval.Default, uMax)
    {
    }
    
    private GaussianFunction(string name, double mu, double sigma, Interval universe, double uMax) : 
        base(name, universe, uMax)
    {
        Mu = mu;
        Sigma = sigma;
    }

    public double Mu { get; }
    public double Sigma { get; }

    #region BellShapedFunctionProperties

    public override double Center => Mu;

    #endregion

    #region MeasurableFunctionProperties

    protected override double CalculateOriginalMetric(MetricType metricType) => metricType switch
    {
        // μMax * σ * Sqrt(2 * π) * Erf(2 * Sqrt(2))
        MetricType.Area => UMax * Sigma * Sqrt(2 * PI) * SpecialFunctions.Erf(2 * Sqrt(2)),
        // Mx = μ ⋅ Area
        MetricType.MomentX => Mu * Area(MetricScope.Original),
        // My = (1/2) * μMax^2 * σ * Sqrt(π) * Erf(4)
        MetricType.MomentY => (1 / 2D) * Pow(UMax, 2) * Sigma * Sqrt(PI) * SpecialFunctions.Erf(4),
        // Mxx = (μ^2 + σ^2) ⋅ Area - 8 * μMax * σ^3 * Exp(-8)
        MetricType.MomentXx => (Pow(Mu, 2) + Pow(Sigma, 2)) * Area(MetricScope.Original) - 8 * UMax * Pow(Sigma, 3) * Exp(-8),
        // Mxy = μ ⋅ My
        MetricType.MomentXy => Mu * MomentY(MetricScope.Original),
        // Myy = (1/3) * μMax^3 * σ * Sqrt((2/3) * π) * Erf(2 * Sqrt(6))
        MetricType.MomentYy => (1 / 3D) * Pow(UMax, 3) * Sigma * Sqrt((2 / 3D) * PI) * SpecialFunctions.Erf(2 * Sqrt(6)),
        _ => base.CalculateOriginalMetric(metricType)
    };

    #endregion

    #region MembershipFunctionIntervals

    public override double EffectiveSupportLeft => Mu - 4 * Sigma;

    public override double EffectiveSupportRight => Mu + 4 * Sigma;

    #endregion

    #region AlphaCuts

    public override Option<double> AlphaCutLeft(FuzzyNumber alpha)
    {
        if (alpha.Value.IsRoughlyZero() || alpha.Value.IsRoughlyGreaterThan(UMax))
            return Option<double>.None();
        if (alpha.Value.RoughlyEquals(UMax))
            return Mu;
        return Mu - Sigma * Sqrt(2 * Log(UMax / alpha.Value));
    }

    public override Option<double> AlphaCutRight(FuzzyNumber alpha)
    {
        if (alpha.Value.IsRoughlyZero() || alpha.Value.IsRoughlyGreaterThan(UMax))
            return Option<double>.None();
        if (alpha.Value.RoughlyEquals(UMax))
            return Mu;
        return Mu + Sigma * Sqrt(2 * Log(UMax / alpha.Value));
    }

    #endregion

    #region ImplicationMethods

    public override Func<double, double> LarsenProduct(FuzzyNumber lambda) =>
        x => lambda.Value * UMax * Exp(-(1 / 2.0) * Pow((x - Mu) / Sigma, 2));

    #endregion

    #region CloningMethods

    public override IMembershipFunction DeepCopy(bool copyState = true) =>
        DeepCopy(Name, copyState);

    public override IMembershipFunction DeepCopy(string name, bool copyState = true)
    {
        var copy = new GaussianFunction(name, Mu, Sigma, copyState ? UniverseOfDiscourse : Interval.Default, UMax);
        copy.CopyMetrics(this, copyState);
        return copy;
    }

    #endregion

    #region ObjectOverrides

    public override string ToString() =>
        $"Linguistic term: {Name} - Membership Function: Gaussian - Sides: (μ: {Mu}, σ: {Sigma}) - μMax: {UMax}";

    #endregion

}
