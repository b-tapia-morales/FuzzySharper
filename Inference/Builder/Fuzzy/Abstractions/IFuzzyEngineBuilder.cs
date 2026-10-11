using Inference.Aggregator.Factory;
using Inference.Builder.Abstractions;
using Inference.Defuzzifier.Factory;
using Kernel.Function.Implication.Factory;
using Reasoning.Base.FuzzySet.Abstractions;
using Reasoning.Rule.FuzzySet.Comparer.Implementations.Deterministic.Factory;

namespace Inference.Builder.Fuzzy.Abstractions;

public interface IFuzzyEngineBuilder : IEngineBuilder
{
    IFuzzySetRuleBase? RuleBase { get; }
    ImplicationMethod ImplicationMethod { get; }
    DefuzzificationMethod DefuzzificationMethod { get; }
    DeterministicMethod ComparerMethod { get; }
    ValueAggregatorMethod AggregatorMethod { get; }
}