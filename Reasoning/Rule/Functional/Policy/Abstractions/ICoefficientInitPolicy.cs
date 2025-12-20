using Reasoning.Rule.Functional.Abstractions;

namespace Reasoning.Rule.Functional.Policy.Abstractions;

public interface ICoefficientInitPolicy
{
    IReadOnlyDictionary<string, double> Initialize(IFunctionalRule rule);
}

public interface ICoefficientInitPolicy<out T> : ICoefficientInitPolicy where T : class, ICoefficientInitPolicy
{
    static abstract T Default { get; }
}