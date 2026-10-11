using Inference.Aggregator.Factory;
using Inference.Defuzzifier.Factory;
using Inference.Engine.Abstractions;
using Kernel.Function.Implication.Factory;
using Reasoning.Rule.FuzzySet.Abstractions;
using Reasoning.Rule.FuzzySet.Comparer.Implementations.Deterministic.Factory;
using Shared.Options.Implementations;

namespace Inference.Engine.Fuzzy.Abstractions;

public interface IFuzzyConsequentEngine : IInferenceEngine<IFuzzySetRule>
{
    ImplicationMethod ImplicationMethod { get; }
    DefuzzificationMethod DefuzzificationMethod { get; }
    DeterministicMethod DeterministicMethod { get; }
    ValueAggregatorMethod AggregatorMethod { get; }

    Option<double> Defuzzify(string target, bool provideExplanation = true);
}