using Kernel.Function.Abstractions;
using Kernel.Number;
using Shared.Approx;
using Shared.Intervals.Implementations;
using Shared.Options.Factory;
using Shared.Options.Implementations;
using static System.Math;

// ReSharper disable MemberCanBePrivate.Global

namespace Kernel.Function.Implementations;

public class GaussianFunction : BellShapedFunction
{
    public GaussianFunction(string name, double mu, double sigma, double uMax = 1) :
        this(name, mu, sigma, Interval.Default, uMax)
    {
    }

    public GaussianFunction(string name, double mu, double sigma, Interval universe, double uMax = 1) : base(name, universe, uMax)
    {
        CheckSigma(sigma);
        Mu = mu;
        Sigma = sigma;
    }

    public double Mu { get; }
    public double Sigma { get; }

    #region BellShapedFunctionProperties

    public override double Center => Mu;

    #endregion

    #region MembershipFunctionIntervals

    public override double EffectiveSupportLeft => Mu - 4 * Sigma;

    public override double EffectiveSupportRight => Mu + 4 * Sigma;

    #endregion

    #region AlphaCuts

    public override Option<double> AlphaCutLeft(FuzzyNumber alpha)
    {
        if (alpha.Value.IsRoughlyZero() || alpha.Value.IsRoughlyGreaterThan(UMax))
            return OptionFactory.None<double>();
        if (alpha.Value.RoughlyEquals(UMax))
            return Mu;
        return Mu - Sigma * Sqrt(2 * Log(1 / alpha.Value));
    }

    public override Option<double> AlphaCutRight(FuzzyNumber alpha)
    {
        if (alpha.Value.IsRoughlyZero() || alpha.Value.IsRoughlyGreaterThan(UMax))
            return OptionFactory.None<double>();
        if (alpha.Value.RoughlyEquals(UMax))
            return Mu;
        return Mu + Sigma * Sqrt(2 * Log(1 / alpha.Value));
    }

    #endregion

    #region ImplicationMethods

    public override Func<double, double> LarsenProduct(FuzzyNumber lambda) =>
        x => lambda.Value * Exp(-(1 / 2.0) * Pow((x - Mu) / Sigma, 2));

    #endregion

    #region CloningMethods

    public override IMembershipFunction DeepCopy() =>
        new GaussianFunction(Name, Mu, Sigma, UMax);

    public override IMembershipFunction DeepCopy(string name) =>
        new GaussianFunction(name, Mu, Sigma, UMax);

    #endregion

    #region ObjectOverrides

    public override string ToString() =>
        $"Linguistic term: {Name} - Membership Function: Gaussian - Sides: (μ: {Mu}, σ: {Sigma}) - μMax: {UMax}";

    #endregion

    private static void CheckSigma(double sigma)
    {
        if (sigma.IsRoughlyZero())
            throw new ArgumentException("The value for Sigma cannot be equal to 0");
    }
}