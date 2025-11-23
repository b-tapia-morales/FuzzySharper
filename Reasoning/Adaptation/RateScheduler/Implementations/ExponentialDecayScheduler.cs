using Reasoning.Adaptation.RateScheduler.Abstractions;

namespace Reasoning.Adaptation.RateScheduler.Implementations;

public class ExponentialDecayScheduler : IRateScheduler
{
    public const double DefaultTau = 10;
    private const double MinTau = 1;

    public ExponentialDecayScheduler() : this(DefaultTau)
    {
    }

    public ExponentialDecayScheduler(double tau)
    {
        IRateScheduler.CheckValue("Tau", tau, MinTau);
        Tau = tau;
    }

    private double Tau { get; }

    public double GetAlpha(uint iteration) =>
        1 - Math.Exp(-iteration / Tau);
}