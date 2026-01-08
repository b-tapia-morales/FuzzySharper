using Inference.Aggregator.Abstractions;
using Inference.Aggregator.Components;
using Inference.Aggregator.Extensions;
using Inference.Defuzzifier.Abstractions;
using Kernel.Function.Extensions;
using Kernel.Function.Implication.Factory;
using Kernel.Operator.Family.Abstractions;
using Knowledge.Memory.Abstractions;
using Reasoning.Rule.FuzzySet.Abstractions;
using Shared.Options.Factory;
using Shared.Options.Implementations;
using Utils.Shape;

// ReSharper disable RedundantExplicitTupleComponentName

namespace Inference.Defuzzifier.Implementations;

public class CenterOfLargestArea : BaseDefuzzifier
{
    protected override Option<double> DefuzzifyMethod(ICollection<IFuzzySetRule> rules, IWorkingMemory memory,
        IOperatorFamily family, IValueAggregator aggregator, ImplicationMethod method,
        out ICollection<IFuzzySetRule> activatedRules)
    {
        activatedRules = [];

        var applicable = EvaluateFiringStrengths(rules, memory, family);
        if (applicable.Count == 0)
            return Option<double>.None();

        var candidates = FiringStrengthAggregator.SelectFirings(applicable);
        if (candidates.Count == 0)
            return Option<double>.None();

        activatedRules = [..candidates.Select(tuple => tuple.Rule)];
        return FiringStrengthAggregator.AggregateFirings(candidates, Selector, aggregator);

        double Selector(FiringStrength firing) =>
            firing.Function.CalculateFirstMoment(Axis.X, method, firing.Weight) / firing.Function.CalculateArea(method, firing.Weight);
        
    }
}