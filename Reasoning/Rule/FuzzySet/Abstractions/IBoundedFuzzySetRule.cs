using Reasoning.Proposition.Components;
using Reasoning.Rule.Abstractions;

namespace Reasoning.Rule.FuzzySet.Abstractions;

public interface IBoundedFuzzySetRule<out T> : IFuzzySetRule, IContextBoundRule<T> where T: class, IBoundedFuzzySetRule<T>
{
    T Then(string variableName, string termName, HedgeType hedgeType = HedgeType.None);
}