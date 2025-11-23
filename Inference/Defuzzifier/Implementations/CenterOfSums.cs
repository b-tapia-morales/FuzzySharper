using Inference.Defuzzifier.Abstractions;
using Kernel.Function.Extensions;
using Kernel.Operator.Conorm.Abstractions;
using Kernel.Operator.Negation.Abstractions;
using Kernel.Operator.Norm.Abstractions;
using Knowledge.Memory.Abstractions;
using Reasoning.Rule.Abstractions;
using Shared.Approx;
using Shared.Options.Factory;
using Shared.Options.Implementations;
using Utils.Shape;

// ReSharper disable RedundantExplicitTupleComponentName

namespace Inference.Defuzzifier.Implementations;

public class CenterOfSums : IDefuzzifier
{
    public Option<double> Defuzzify(ICollection<IRule> rules, IWorkingMemory memory, INegation negation,
        INorm norm, IConorm conorm, ImplicationMethod method, out ICollection<IRule> activatedRules)
    {
        IDefuzzifier.RulesCheck(rules, memory);
        var (min, max) = IDefuzzifier.GetUniverse(rules).ToTuple();

        activatedRules = [];

        var applicable = IDefuzzifier.GetWeightedTuples(rules, memory, negation, norm, conorm);
        if (applicable.Count == 0)
            return OptionFactory.None<double>();

        var candidates = applicable.Select(tuple => (
                Rule: tuple.Rule,
                Function: tuple.Function,
                Area: tuple.Function.CalculateAreaAt(method, tuple.Weight, min, max),
                MomentumX: tuple.Function.CalculateFirstMomentAt(Axis.X, method, tuple.Weight, min, max)))
            .Where(tuple => tuple.Area.IsRoughlyGreaterThan(0))
            .ToList();
        if (candidates.Count == 0)
            return OptionFactory.None<double>();

        activatedRules = [..candidates.Select(tuple => tuple.Rule)];

        var centroid = candidates.Sum(t => t.MomentumX) / candidates.Sum(t => t.Area);
        return Math.Clamp(centroid, min, max);
    }
}