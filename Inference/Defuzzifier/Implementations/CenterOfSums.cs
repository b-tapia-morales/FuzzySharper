using Inference.Aggregator.Abstractions;
using Inference.Defuzzifier.Abstractions;
using Kernel.Function.Extensions;
using Kernel.Function.Implication.Factory;
using Kernel.Operator.Family.Abstractions;
using Knowledge.Memory.Abstractions;
using Reasoning.Rule.FuzzySet.Abstractions;
using Shared.Approx;
using Shared.Options.Factory;
using Shared.Options.Implementations;
using Utils.Shape;

// ReSharper disable RedundantExplicitTupleComponentName

namespace Inference.Defuzzifier.Implementations;

public class CenterOfSums : BaseDefuzzifier
{
    override protected Option<double> DefuzzifyMethod(ICollection<IFuzzySetRule> rules, IWorkingMemory memory,
        IOperatorFamily family, IValueAggregator aggregator, ImplicationMethod method,
        out ICollection<IFuzzySetRule> activatedRules)
    {
        activatedRules = [];

        var applicable = EvaluateFiringStrengths(rules, memory, family);
        if (applicable.Count == 0)
            return Option<double>.None();

        var candidates = applicable.Select(tuple => (
                Rule: tuple.Rule,
                Function: tuple.Function,
                Area: tuple.Function.CalculateArea(method, tuple.Weight),
                MomentumX: tuple.Function.CalculateFirstMoment(Axis.X, method, tuple.Weight)))
            .Where(tuple => tuple.Area.IsRoughlyGreaterThan(0))
            .ToList();
        if (candidates.Count == 0)
            return Option<double>.None();

        activatedRules = [..candidates.Select(tuple => tuple.Rule)];

        return candidates.Sum(t => t.MomentumX) / candidates.Sum(t => t.Area);
    }
}