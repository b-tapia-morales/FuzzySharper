using Reasoning.Adaptation.RateScheduler.Abstractions;

namespace Reasoning.Adaptation.RateScheduler.Policy.Abstractions;

public interface IRateSchedulerPolicy
{
    IRateScheduler Create();
}