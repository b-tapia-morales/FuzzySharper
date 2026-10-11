using Kernel.Operator.Family.Abstractions;
using Kernel.Operator.Family.Factory.Canonical;
using Knowledge.Memory.Abstractions;
using Reasoning.Adaptation.Components;
using Shared.Options.Factory;
using Shared.Options.Implementations;

namespace Inference.Builder.Abstractions;

public abstract class AbstractEngineBuilder : IEngineBuilder
{
    public IWorkingMemory? WorkingMemory { get; internal set; }
    public IOperatorFamily OperatorFamily { get; internal set; } = CanonicalFactory.UseFamily(CanonicalType.Godel);
    public bool IsLearningEnabled { get; internal set; }
    public Option<AdaptationPolicy> AdaptationPolicy { get; internal set; } = Option<AdaptationPolicy>.None();
}