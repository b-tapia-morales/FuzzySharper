using Kernel.Number;
using Kernel.Operator.Family.Abstractions;
using Kernel.Operator.Family.Factory.Canonical;
using Knowledge.Memory.Abstractions;
using Reasoning.Rule.FuzzySet.Abstractions;
using Reasoning.Rule.FuzzySet.Comparer.Abstractions;
using Shared.Options.Factory;
using Shared.Options.Implementations;

namespace Reasoning.Rule.FuzzySet.Comparer.Implementations.LearningBased;

public class LatestLearnedWeight(IWorkingMemory memory, IOperatorFamily operatorFamily) : IRuleComparer
{
    public IWorkingMemory Memory { get; } = memory;
    private IOperatorFamily OperatorFamily { get; } = operatorFamily;

    public LatestLearnedWeight(IWorkingMemory memory) : this(memory, CanonicalFactory.UseFamily(CanonicalType.Godel))
    {
    }

    public int ComparerMethod(IFuzzySetRule x, IFuzzySetRule y)
    {
        var (firstIsPresent, secondIsPresent) = (ResolveWeight(x).IsSome(out var w1), ResolveWeight(y).IsSome(out var w2));
        return (firstIsPresent, secondIsPresent) switch
        {
            (false, false) => +0,
            (true, false) => -1,
            (false, true) => +1,
            (true, true) => w2.CompareTo(w1)
        };
    }

    private Option<FuzzyNumber> ResolveWeight(IFuzzySetRule rule)
    {
        if (rule.AdaptationState.LatestWeight.IsSome(out var learnedWeight))
            return learnedWeight;
        return rule.EvaluatePremiseWeight(Memory, OperatorFamily).IsSome(out var premiseWeight)
            ? premiseWeight
            : Option<FuzzyNumber>.None();
    }
}