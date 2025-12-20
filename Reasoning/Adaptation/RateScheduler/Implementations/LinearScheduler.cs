using Reasoning.Adaptation.RateScheduler.Abstractions;

namespace Reasoning.Adaptation.RateScheduler.Implementations;

public class LinearScheduler : IRateScheduler
{
    public const double DefaultTau = 10;
    private const double MinTau = 1;

    public LinearScheduler(double tau)
    {
        IRateScheduler.CheckValue("Tau", tau, MinTau);
        Tau = tau;
    }

    public LinearScheduler() : this(DefaultTau)
    {
    }

    private double Tau { get; }

    public double GetAlpha(uint iteration) =>
        Math.Clamp(1 - (iteration / Tau), 0, 1);
}