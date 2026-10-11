using Reasoning.Base.Abstractions;
using Reasoning.Rule.Abstractions;
using Shared.Options.Factory;

namespace Inference.Engine.Abstractions;

public abstract class AbstractInferenceEngine<T> : AbstractInferenceEngine, IInferenceEngine<T> 
    where T : class, IRule
{
    public required IRuleBase<T> RuleBase { get; init; }

    public override void ResetLearning()
    {
        RuleBase.ResetAdaptation();
        CurrentIteration = 0;
    }

    public void UpdateLearning()
    {
        if (!IsLearningEnabled || !AdaptationConfig.IsSome(out var config))
            return;
        RuleBase.RecomputeAdaptation(config);
    }
}