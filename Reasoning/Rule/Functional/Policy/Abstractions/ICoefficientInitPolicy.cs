using Reasoning.Proposition.Abstractions;
using Reasoning.Proposition.Implementations;

namespace Reasoning.Rule.Functional.Policy.Abstractions;

public interface ICoefficientInitPolicy
{
    (IList<double> Coefficients, double Bias) Initialize(IList<IProposition> propositions);

    static double EvaluateCoefficient(IProposition proposition, Func<FuzzyProposition, double> evaluator) =>
        proposition is FuzzyProposition prop ? evaluator(prop) : 0;
}

public interface ICoefficientInitPolicy<out T> : ICoefficientInitPolicy where T : class, ICoefficientInitPolicy
{
    static abstract T Default { get; }
}