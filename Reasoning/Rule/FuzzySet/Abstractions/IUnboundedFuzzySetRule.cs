using Knowledge.Linguistic.Base.Abstractions;
using Reasoning.Proposition.Components;
using Reasoning.Rule.Abstractions;

namespace Reasoning.Rule.FuzzySet.Abstractions;

public interface IUnboundedFuzzySetRule<out T> : IFuzzySetRule, IContextFreeRule<T> where T: class, IUnboundedFuzzySetRule<T>
{
    T Then(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None);
}