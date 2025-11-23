using Reasoning.Adaptation.RateScheduler.Abstractions;
using Reasoning.Adaptation.RateScheduler.Implementations;
using Reasoning.Adaptation.RateScheduler.Policy.Abstractions;

namespace Reasoning.Adaptation.RateScheduler.Policy.Implementations;

public readonly record struct ExponentialDecayPolicy(double Tau) : IRateSchedulerPolicy
{
    private static readonly ExponentialDecayScheduler DefaultInstance = new(ExponentialDecayScheduler.DefaultTau);

    public static IRateScheduler Default =>
        DefaultInstance;

    public IRateScheduler Create() =>
        new ExponentialDecayScheduler(Tau);
}