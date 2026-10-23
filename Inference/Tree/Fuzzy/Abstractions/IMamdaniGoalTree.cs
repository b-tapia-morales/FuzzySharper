using Inference.Aggregator.Abstractions;
using Inference.Defuzzifier.Abstractions;
using Inference.Tree.Abstractions;
using Kernel.Function.Implication.Factory;
using Kernel.Operator.Family.Abstractions;
using Knowledge.Memory.Abstractions;
using Reasoning.Rule.FuzzySet.Abstractions;
using Shared.Options.Factory;
using Shared.Options.Implementations;
using Shared.Primitives.Implementation;

namespace Inference.Tree.Fuzzy.Abstractions;

public interface IMamdaniGoalTree<T> : IGoalTree<T, IFuzzySetRule> where T : class, IMamdaniGoalTree<T>
{
    public Option<double> InferFact(IWorkingMemory memory, IComparer<IFuzzySetRule> comparer,
        IDefuzzifier defuzzifier, IValueAggregator aggregator, IOperatorFamily operatorFamily,
        ImplicationMethod implicationMethod);

    public Option<double> InferFact(IWorkingMemory memory, IComparer<IFuzzySetRule> comparer,
        IDefuzzifier defuzzifier, IValueAggregator aggregator, IOperatorFamily operatorFamily,
        ImplicationMethod implicationMethod, Option<uint> iteration);

    static abstract T BuildTree(StringOrType rootIdentifier, ICollection<IFuzzySetRule> rules);
}