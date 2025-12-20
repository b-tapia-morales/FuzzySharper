using Inference.Aggregator.Abstractions;
using Inference.Aggregator.Components;
using Inference.Aggregator.Extensions;
using Inference.Defuzzifier.Abstractions;
using Kernel.Function.Implication.Factory;
using Kernel.Operator.Family.Abstractions;
using Knowledge.Memory.Abstractions;
using Reasoning.Rule.FuzzySet.Abstractions;
using Shared.Options.Factory;
using Shared.Options.Implementations;

// ReSharper disable RedundantExplicitTupleComponentName

namespace Inference.Defuzzifier.Implementations;

public class MeanOfMaxima : BaseDefuzzifier
{
    override protected Option<double> DefuzzifyMethod(ICollection<IFuzzySetRule> rules, IWorkingMemory memory,
        IOperatorFamily family, IValueAggregator aggregator, ImplicationMethod method,
        out ICollection<IFuzzySetRule> activatedRules)
    {
        activatedRules = [];

        var applicable = EvaluateFiringStrengths(rules, memory, family);
        if (applicable.Count == 0)
            return Option<double>.None();

        activatedRules = [..applicable.Select(firing => firing.Rule)];
        return FiringStrengthAggregator.AggregateFirings(applicable, Selector, aggregator);

        double Selector(FiringStrength firing) =>
            method == ImplicationMethod.Mamdani ? firing.Function.AlphaCutClipped(firing.Weight).Get.Width.Get : firing.Function.PeakClipped.Get.Width.Get;
    }
}