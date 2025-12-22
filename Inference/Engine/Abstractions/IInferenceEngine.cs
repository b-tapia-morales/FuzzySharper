using Kernel.Operator.Family.Abstractions;
using Knowledge.Memory.Abstractions;
using Reasoning.Adaptation.Components;
using Reasoning.Base.FuzzySet.Abstractions;
using Shared.Options.Implementations;

namespace Inference.Engine.Abstractions;

public interface IInferenceEngine
{
    IWorkingMemory WorkingMemory { get; }
    IOperatorFamily OperatorFamily { get; internal set; }
    bool IsLearningEnabled { get; internal set; }
    uint CurrentIteration { get; internal set; }
    Option<AdaptationConfig> AdaptationConfig { get; internal set; }

    void EnableLearning(AdaptationPolicy policy, bool resetPreviousHistory = true);

    void EnableLearning(AdaptationConfig config, bool resetPreviousHistory = true);

    void DisableLearning(bool resetPreviousHistory = true);

    void ResetLearning();
}