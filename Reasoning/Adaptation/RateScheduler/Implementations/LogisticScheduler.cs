using Reasoning.Adaptation.RateScheduler.Abstractions;

namespace Reasoning.Adaptation.RateScheduler.Implementations;

public class LogisticScheduler : IRateScheduler
{
    public const double DefaultTau = 15;
    private const double MinTau = 1;
    public const double DefaultSteepness = 5;
    private const double MinSteepness = 0.5;

    public LogisticScheduler() : this(DefaultTau, DefaultSteepness)
    {
    }

    public LogisticScheduler(double tau, double steepness)
    {
        IRateScheduler.CheckValue("Tau", tau, MinTau);
        IRateScheduler.CheckValue("Steepness", steepness, MinSteepness);
        Tau = tau;
        Steepness = steepness;
    }

    private double Tau { get; }
    private double Steepness { get; }

    public double GetAlpha(uint iteration) =>
        1 / (1 + Math.Exp(-(iteration - Tau) / Steepness));
}