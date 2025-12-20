using Reasoning.Adaptation.RateScheduler.Factory;
using Reasoning.Adaptation.RateScheduler.Policy.Abstractions;
using Reasoning.Adaptation.RateScheduler.Policy.Implementations;

namespace Reasoning.Adaptation.RateScheduler.Policy.Factory;

public static class RateSchedulerPolicyFactory
{
    private static readonly IRateSchedulerPolicy Linear = new LinearPolicy();
    private static readonly IRateSchedulerPolicy Logistic = new LogisticPolicy();
    private static readonly IRateSchedulerPolicy ExponentialDecay = new ExponentialDecayPolicy();

    public static IRateSchedulerPolicy GetInstance(RateSchedulerMethod method) =>
        method switch
        {
            RateSchedulerMethod.Linear => Linear,
            RateSchedulerMethod.Logistic => Logistic,
            RateSchedulerMethod.ExponentialDecay => ExponentialDecay,
            _ => throw new ArgumentOutOfRangeException(nameof(method), method, null)
        };
}