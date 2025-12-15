using Kernel.Function.Abstractions;
using Kernel.Number;
using MathNet.Numerics;
using Shared.Approx;
using Shared.Deferred;
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
        return new GaussianFunction(name, mu, sigma, universe, uMax);
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

    // μMax * σ * Sqrt(2 * π) * Erf(2 * Sqrt(2))
    override protected DeferredValue<double> DeferredArea =>
        new(UMax * Sigma * Sqrt(2 * PI) * SpecialFunctions.Erf(2 * Sqrt(2)));

    // μMax^2 * σ * Sqrt(π) * Erf(4)
    override protected DeferredValue<double> DeferredMomentX =>
        new(Pow(UMax, 2) * Sigma * Sqrt(PI) * SpecialFunctions.Erf(4));

    // μMax * σ * Sqrt(π / 2) * μ * Erf(2 * Sqrt(2))
    override protected DeferredValue<double> DeferredMomentY =>
        new(UMax * Sigma * Sqrt(PI / 2) * Mu * SpecialFunctions.Erf(2 * Sqrt(2)));

    // (1/3) * μMax^3 * σ * Sqrt((2/3) * π) * Erf(2 * Sqrt(6))
    override protected DeferredValue<double> DeferredMomentXx =>
        new((1 / 3.0) * Pow(UMax, 3) * Sigma * Sqrt((2 / 3.0) * PI) * SpecialFunctions.Erf(2 * Sqrt(6)));

    // μMax * [-8 * σ^3/ℇ^8 + σ * Sqrt(2 * π) * (σ^2 + μ^2) * Erf(2 * Sqrt(2))]
    override protected DeferredValue<double> DeferredMomentXy =>
        new(UMax * (-8 * (Pow(Sigma, 3) / Pow(E, 8)) + Sigma * Sqrt(2 * PI) * (Pow(Sigma, 2) + Pow(Mu, 2)) * SpecialFunctions.Erf(2 * Sqrt(2))));

    // (1/2) * μMax^2 * σ * Sqrt(π) * u * Erf(4)
    override protected DeferredValue<double> DeferredMomentYy =>
        new((1 / 2.0) * Pow(UMax, 2) * Sigma * Sqrt(PI) * Mu * SpecialFunctions.Erf(4));

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