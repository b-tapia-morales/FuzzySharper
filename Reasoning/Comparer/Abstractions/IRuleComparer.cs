using Knowledge.Memory.Abstractions;
using Reasoning.Rule.Extensions;
using Reasoning.Rule.FuzzySet.Abstractions;

namespace Reasoning.Comparer.Abstractions;

public interface IRuleComparer : IComparer<IFuzzySetRule>
{
    IWorkingMemory Memory { get; }

    int ComparerMethod(IFuzzySetRule x, IFuzzySetRule y);

    int IComparer<IFuzzySetRule>.Compare(IFuzzySetRule? x, IFuzzySetRule? y)
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