using Inference.Aggregator.Abstractions;
using Inference.Defuzzifier.Abstractions;
using Kernel.Function.Implication.Factory;
using Kernel.Operator.Family.Abstractions;
using Knowledge.Memory.Abstractions;
using Reasoning.Rule.FuzzySet.Abstractions;
using Shared.Options.Implementations;
using Shared.Primitives.Implementation;

namespace Inference.Tree.Abstractions;

public interface ITree<T> where T : class, ITree<T>
{
    StringOrType Identifier { get; }
    ICollection<T> Children { get; }
    ICollection<IFuzzySetRule> Rules { get; }
    bool IsProven { get; }

    bool IsLeaf();

    void AddChild(T child);

    void AddChildren(params IEnumerable<T> children);

    void AddRules(params IEnumerable<IFuzzySetRule> rules);
    
    Option<double> InferFact(IWorkingMemory memory, IComparer<IFuzzySetRule> comparer,
        IDefuzzifier defuzzifier, IValueAggregator aggregator, IOperatorFamily operatorFamily, ImplicationMethod implicationMethod);

    Option<double> InferFact(IWorkingMemory memory, IComparer<IFuzzySetRule> comparer,
        IDefuzzifier defuzzifier, IValueAggregator aggregator, IOperatorFamily operatorFamily, ImplicationMethod implicationMethod, Option<uint> iteration);

    static abstract T BuildTree(StringOrType rootIdentifier, ICollection<IFuzzySetRule> rules);
}