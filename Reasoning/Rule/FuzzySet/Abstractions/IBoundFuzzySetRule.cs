using Reasoning.Proposition.Components;
using Reasoning.Rule.Abstractions;

namespace Reasoning.Rule.FuzzySet.Abstractions;

public interface IBoundFuzzySetRule<out T> : IFuzzySetRule, IContextBoundRule<T> where T: class, IBoundFuzzySetRule<T>
{
    T Then(string variableName, string termName, HedgeType hedgeType = HedgeType.None);
}