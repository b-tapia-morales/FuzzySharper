using Kernel.Operator.Conorm.Abstractions;
using Kernel.Operator.Family.Abstractions;
using Kernel.Operator.Family.Factory.Canonical;
using Kernel.Operator.Negation.Abstractions;
using Kernel.Operator.Norm.Abstractions;
using Kernel.Operator.Residuum.Abstractions;
using Knowledge.Memory.Abstractions;
using Reasoning.Adaptation.Components;

namespace Inference.Builder.Abstractions;

public interface IEngineBuilder<out T> : IEngineBuilder where T : IEngineBuilder<T>
{
    T WithWorkingMemory(IWorkingMemory workingMemory);

    T WithOperatorFamily(IOperatorFamily operatorFamily);

    T WithNegation(INegation negation);

    T WithDisjunction(INorm norm);

    T WithConjunction(IConorm conorm);

    T WithResiduum(IResiduum residuum);

    T WithOperators(INegation negation, INorm norm, IConorm conorm, IResiduum residuum);

    T WithCanonicalFamily(CanonicalType type);

    T WithAdaptationPolicy(AdaptationPolicy adaptationPolicy);
}