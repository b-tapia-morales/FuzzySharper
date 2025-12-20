using Reasoning.Adaptation.RateScheduler.Abstractions;
using Reasoning.Adaptation.RateScheduler.Implementations;
using Reasoning.Adaptation.RateScheduler.Policy.Abstractions;

namespace Reasoning.Adaptation.RateScheduler.Policy.Implementations;

public readonly record struct LogisticPolicy(double Tau, double Steepness) : IRateSchedulerPolicy
{
    private static readonly LogisticScheduler DefaultInstance = new(LogisticScheduler.DefaultTau, LogisticScheduler.DefaultSteepness);

    public static IRateScheduler Default =>
        DefaultInstance;

    public IRateScheduler Create() =>
        new LogisticScheduler(Tau, Steepness);
}