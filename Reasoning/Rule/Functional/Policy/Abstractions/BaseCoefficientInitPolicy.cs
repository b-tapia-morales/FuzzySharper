using Reasoning.Proposition.Abstractions;
using Reasoning.Proposition.Implementations;
using Reasoning.Rule.Functional.Abstractions;

namespace Reasoning.Rule.Functional.Policy.Abstractions;

public abstract class BaseCoefficientInitPolicy : ICoefficientInitPolicy
{
    public IReadOnlyDictionary<string, double> Initialize(IFunctionalRule rule)
    {
        var premiseDict = rule.PremiseDict
            .Where(pair => pair.Key.IsString)
            .ToDictionary(pair => pair.Key.AsString, pair => pair.Value);
        var premiseVariables = premiseDict.Keys.ToList();
        return premiseDict.Count == 0
            ? new Dictionary<string, double>()
            : premiseVariables.ToDictionary(key => key, key => Aggregate(Initialize(premiseDict[key])));
    }

    protected virtual double Aggregate(IEnumerable<double> values)
        => values.Max();

    public abstract IEnumerable<double> Initialize(IReadOnlyList<IProposition> premise);

    protected static double EvaluateCoefficient(IProposition proposition, Func<FuzzyProposition, double> evaluator) =>
        proposition is FuzzyProposition fuzzyProposition ? evaluator(fuzzyProposition) : 0;
}