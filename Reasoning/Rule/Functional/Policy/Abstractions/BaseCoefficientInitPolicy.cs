using Reasoning.Proposition.Implementations;
using Reasoning.Rule.Functional.Abstractions;

namespace Reasoning.Rule.Functional.Policy.Abstractions;

public abstract class BaseCoefficientInitPolicy : ICoefficientInitPolicy
{
    public IReadOnlyDictionary<string, double> InitializeFromRule(IFunctionalRule rule)
    {
        var premiseDict = rule.PremiseDict
            .Where(pair => pair.Key.IsString)
            .ToDictionary(pair => pair.Key.AsString, pair => pair.Value.Cast<FuzzyProposition>().ToList());
        return premiseDict.Keys.ToDictionary(key => key, key => Aggregate(Initialize(premiseDict[key])));
    }

    protected virtual double Aggregate(IEnumerable<double> values) =>
        values.Max();

    public abstract IEnumerable<double> Initialize(IReadOnlyList<FuzzyProposition> premise);
}