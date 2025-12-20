using Kernel.Operator.Family.Abstractions;
using Knowledge.Memory.Abstractions;
using Reasoning.Adaptation.Components;
using Reasoning.Base.FuzzySet.Abstractions;
using Shared.Options.Factory;
using Shared.Options.Implementations;

namespace Inference.Engine.Abstractions;

public abstract class AbstractInferenceEngine : IInferenceEngine
{
    public abstract required IFuzzySetRuleBase RuleBase { get; init; }
    public abstract required IWorkingMemory WorkingMemory { get; init; }
    public abstract required IOperatorFamily OperatorFamily { get; set; }
    public abstract bool IsLearningEnabled { get; set; }
    public abstract uint CurrentIteration { get; set; }
    public abstract Option<AdaptationConfig> AdaptationConfig { get; set; }

    public void EnableLearning(AdaptationPolicy policy, bool resetPreviousHistory = true) =>
        EnableLearning(new AdaptationConfig(policy), resetPreviousHistory);

    public void EnableLearning(AdaptationConfig config, bool resetPreviousHistory = true)
    {
        if (resetPreviousHistory)
            ResetLearning();
        IsLearningEnabled = true;
        AdaptationConfig = config;
    }

    public void DisableLearning(bool resetPreviousHistory = true)
    {
        if (resetPreviousHistory)
            ResetLearning();
        IsLearningEnabled = false;
        AdaptationConfig = Option<AdaptationConfig>.None();
    }

    public void ResetLearning()
    {
        RuleBase.ResetLearning();
        CurrentIteration = 0;
    }

    protected void UpdateLearning()
    {
        if (!IsLearningEnabled || !AdaptationConfig.IsSome(out var config))
            return;
        RuleBase.UpdateLearning(config);
    }
}