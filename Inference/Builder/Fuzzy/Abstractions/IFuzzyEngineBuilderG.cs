using Inference.Aggregator.Factory;
using Inference.Builder.Abstractions;
using Inference.Defuzzifier.Factory;
using Inference.Engine.Fuzzy.Abstractions;
using Kernel.Function.Implication.Factory;
using Reasoning.Base.FuzzySet.Abstractions;
using Reasoning.Rule.FuzzySet.Comparer.Implementations.Deterministic.Factory;

namespace Inference.Builder.Fuzzy.Abstractions;

public interface IFuzzyEngineBuilder<out T> : IFuzzyEngineBuilder, IEngineBuilder<T>
    where T : class, IFuzzyEngineBuilder<T>
{
    T WithRuleBase(IFuzzySetRuleBase ruleBase);

    T WithImplication(ImplicationMethod method);

    T WithDefuzzification(DefuzzificationMethod method);

    T WithRuleComparer(DeterministicMethod method);

    T WithValueAggregator(ValueAggregatorMethod method);

    static abstract T Create();
}

public interface IFuzzyEngineCreator<out T> : IEngineBuilder 
    where T : class, IFuzzyConsequentEngine
{
    T Build();
}