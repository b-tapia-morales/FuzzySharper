using Kernel.Operator.Conorm.Abstractions;
using Kernel.Operator.Family.Abstractions;
using Kernel.Operator.Family.Factory.Canonical;
using Kernel.Operator.Negation.Abstractions;
using Kernel.Operator.Norm.Abstractions;
using Kernel.Operator.Residuum.Abstractions;
using Knowledge.Memory.Abstractions;
using Reasoning.Adaptation.Components;

namespace Inference.Builder.Abstractions;

public abstract class AbstractEngineBuilder<T> : AbstractEngineBuilder, IEngineBuilder<T>
    where T : AbstractEngineBuilder<T>
{
    public T WithWorkingMemory(IWorkingMemory workingMemory)
    {
        WorkingMemory = workingMemory;
        return (T) this;
    }

    public T WithOperatorFamily(IOperatorFamily operatorFamily)
    {
        OperatorFamily = operatorFamily.DeepCopy();
        return (T) this;
    }

    public T WithNegation(INegation negation)
    {
        OperatorFamily.Negation = negation;
        return (T) this;
    }

    public T WithDisjunction(INorm norm)
    {
        OperatorFamily.Norm = norm;
        return (T) this;
    }

    public T WithConjunction(IConorm conorm)
    {
        OperatorFamily.Conorm = conorm;
        return (T) this;
    }

    public T WithResiduum(IResiduum residuum)
    {
        OperatorFamily.Residuum = residuum;
        return (T) this;
    }

    public T WithOperators(INegation negation, INorm norm, IConorm conorm,
        IResiduum residuum)
    {
        OperatorFamily.Negation = negation;
        OperatorFamily.Norm = norm;
        OperatorFamily.Conorm = conorm;
        OperatorFamily.Residuum = residuum;
        return (T) this;
    }

    public T WithCanonicalFamily(CanonicalType type)
    {
        OperatorFamily = CanonicalFactory.UseFamily(type);
        return (T) this;
    }

    public T WithAdaptationPolicy(AdaptationPolicy adaptationPolicy)
    {
        IsLearningEnabled = true;
        AdaptationPolicy = adaptationPolicy;
        return (T) this;
    }
}