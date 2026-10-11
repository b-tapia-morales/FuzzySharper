using Kernel.Operator.Family.Abstractions;
using Knowledge.Memory.Abstractions;
using Reasoning.Adaptation.Components;
using Shared.Options.Implementations;

namespace Inference.Builder.Abstractions;

public interface IEngineBuilder
{
    IWorkingMemory? WorkingMemory { get; }
    IOperatorFamily OperatorFamily { get; }
    bool IsLearningEnabled { get; }
    Option<AdaptationPolicy> AdaptationPolicy { get; }
}