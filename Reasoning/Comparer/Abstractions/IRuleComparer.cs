using Knowledge.Memory.Abstractions;
using Reasoning.Rule.Abstractions;
using Reasoning.Rule.Extensions;

namespace Reasoning.Comparer.Abstractions;

public interface IRuleComparer : IComparer<IRule>
{
    IWorkingMemory Memory { get; }

    int ComparerMethod(IRule x, IRule y);

    int IComparer<IRule>.Compare(IRule? x, IRule? y)
    {
        if (x == null && y == null)
            return +0;
        if (x != null && y == null)
            return -1;
        if (x == null && y != null)
            return +1;
        var a = x ?? throw new ArgumentNullException(nameof(x));
        var b = y ?? throw new ArgumentNullException(nameof(y));
        a.Validate();
        b.Validate();
        if (!a.IsPremiseEvaluable(Memory) && !b.IsPremiseEvaluable(Memory))
            return +0;
        if (a.IsPremiseEvaluable(Memory) && !b.IsPremiseEvaluable(Memory))
            return -1;
        if (!a.IsPremiseEvaluable(Memory) && b.IsPremiseEvaluable(Memory))
            return +1;
        return ComparerMethod(x, y);
    }
}