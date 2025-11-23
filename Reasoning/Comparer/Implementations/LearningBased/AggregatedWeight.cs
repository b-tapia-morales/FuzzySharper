using Kernel.Number;
using Kernel.Operator.Family.Abstractions;
using Kernel.Operator.Family.Factory.Canonical;
using Knowledge.Memory.Abstractions;
using Reasoning.Comparer.Abstractions;
using Reasoning.Rule.Abstractions;
using Shared.Options.Factory;
using Shared.Options.Implementations;

namespace Reasoning.Comparer.Implementations.LearningBased;

public class AggregatedWeight(IWorkingMemory memory, IOperatorFamily operatorFamily) : IRuleComparer
{
    public IWorkingMemory Memory { get; } = memory;
    private IOperatorFamily OperatorFamily { get; } = operatorFamily;
    
    public AggregatedWeight(IWorkingMemory memory) : this(memory, CanonicalFactory.UseFamily(CanonicalType.Godel)) {}

    public int ComparerMethod(IRule x, IRule y)
    {
        var (firstIsPresent, secondIsPresent) = (ResolveWeight(x).IsSomeVal(out var w1), ResolveWeight(y).IsSomeVal(out var w2));
        return (firstIsPresent, secondIsPresent) switch
        {
            (false, false) => +0,
            (true, false) => -1,
            (false, true) => +1,
            (true, true) => w2.CompareTo(w1)
        };
    }

    private Option<FuzzyNumber> ResolveWeight(IRule rule)
    {
        if (rule.AdaptationState.AggregatedWeight.IsSomeVal(out var learnedWeight))
            return learnedWeight;
        return rule.EvaluatePremiseWeight(Memory, OperatorFamily).IsSomeVal(out var premiseWeight)
            ? premiseWeight
            : OptionFactory.None<FuzzyNumber>();
    }
}