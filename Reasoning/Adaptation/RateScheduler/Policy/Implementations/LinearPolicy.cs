using Reasoning.Adaptation.RateScheduler.Abstractions;
using Reasoning.Adaptation.RateScheduler.Implementations;
using Reasoning.Adaptation.RateScheduler.Policy.Abstractions;

namespace Reasoning.Adaptation.RateScheduler.Policy.Implementations;

public readonly record struct LinearPolicy : IRateSchedulerPolicy
{
    public IRateScheduler Create() => new LinearScheduler();
}