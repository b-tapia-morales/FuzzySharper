using Inference.Aggregator.Factory;
using Inference.Defuzzifier.Factory;
using Inference.Engine.Abstractions;
using Kernel.Function.Implication.Factory;
using Reasoning.Base.FuzzySet.Abstractions;
using Reasoning.Rule.FuzzySet.Comparer.Implementations.Deterministic.Factory;
using Shared.Options.Implementations;

namespace Inference.Engine.Fuzzy.Abstractions;

public interface IFuzzyConsequentEngine : IInferenceEngine
{
    IFuzzySetRuleBase RuleBase { get; }
    ImplicationMethod ImplicationMethod { get; internal set; }
    DefuzzificationMethod DefuzzificationMethod { get; internal set; }
    DeterministicMethod DeterministicMethod { get; internal set; }
    ValueAggregatorMethod AggregatorMethod { get; internal set; }

    Option<double> Defuzzify(string target, bool provideExplanation = true);
}