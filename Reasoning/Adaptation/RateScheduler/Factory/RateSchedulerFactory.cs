using Reasoning.Adaptation.RateScheduler.Abstractions;
using Reasoning.Adaptation.RateScheduler.Implementations;

namespace Reasoning.Adaptation.RateScheduler.Factory;

public static class RateSchedulerFactory
{
    private static readonly IRateScheduler Linear = new LinearScheduler();
    private static readonly IRateScheduler Logistic = new LogisticScheduler();
    private static readonly IRateScheduler ExponentialDecay = new ExponentialDecayScheduler();

    public static IRateScheduler GetInstance(RateSchedulerMethod method) =>
        method switch
        {
            RateSchedulerMethod.Linear => Linear,
            RateSchedulerMethod.Logistic => Logistic,
            RateSchedulerMethod.ExponentialDecay => ExponentialDecay,
            _ => throw new ArgumentOutOfRangeException(nameof(method), method, null)
        };
}