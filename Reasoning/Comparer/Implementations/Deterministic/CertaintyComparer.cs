using Kernel.Operator.Family.Abstractions;
using Kernel.Operator.Family.Factory.Canonical;
using Knowledge.Memory.Abstractions;
using Reasoning.Comparer.Abstractions;
using Reasoning.Rule.Abstractions;
using Shared.Options.Extensions;
using Shared.Options.Factory;

namespace Reasoning.Comparer.Implementations.Deterministic;

public class CertaintyComparer(IWorkingMemory memory, IOperatorFamily family) : IRuleComparer
{
    public IWorkingMemory Memory { get; } = memory;
    private IOperatorFamily Family { get; } = family;

    public CertaintyComparer(IWorkingMemory memory) : this(memory, CanonicalFactory.UseFamily(CanonicalType.Godel))
    {
    }

    public int ComparerMethod(IRule x, IRule y)
    {
        var (cf1, firstIsPresent) = (x.CertaintyFactor, x.EvaluatePremiseWeight(Memory, Family).IsSomeVal(out var w1));
        var (cf2, secondIsPresent) = (y.CertaintyFactor, y.EvaluatePremiseWeight(Memory, Family).IsSomeVal(out var w2));
        return (firstIsPresent, secondIsPresent) switch
        {
            (false, false) => +0,
            (true, false) => -1,
            (false, true) => +1,
            (true, true) => (cf2.OrElse(1) * w2).CompareTo(cf1.OrElse(1) * w1)
        };
    }
}