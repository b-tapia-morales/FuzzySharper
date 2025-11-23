using Inference.Defuzzifier.Abstractions;
using Kernel.Function.Extensions;
using Kernel.Operator.Conorm.Abstractions;
using Kernel.Operator.Negation.Abstractions;
using Kernel.Operator.Norm.Abstractions;
using Knowledge.Memory.Abstractions;
using Reasoning.Rule.Abstractions;
using Shared.Options.Factory;
using Shared.Options.Implementations;

// ReSharper disable RedundantExplicitTupleComponentName

namespace Inference.Defuzzifier.Implementations;

public class FirstOfMaxima : IDefuzzifier
{
    public Option<double> Defuzzify(ICollection<IRule> rules, IWorkingMemory memory, INegation negation,
        INorm norm, IConorm conorm, ImplicationMethod method, out ICollection<IRule> activatedRules)
    {
        IDefuzzifier.RulesCheck(rules, memory);
        var (min, max) = IDefuzzifier.GetUniverse(rules).ToTuple();

        var applicable = IDefuzzifier.GetWeightedTuples(rules, memory, negation, norm, conorm);
        activatedRules = [..applicable.Select(tuple => tuple.Rule)];

        var best = applicable.MaxBy(tuple => tuple.Weight);
        if (best.Weight == 0)
            return OptionFactory.None<double>();

        var (function, weight) = (best.Function, best.Weight);
        var lastOfMaxima = method == ImplicationMethod.Mamdani ? function.AlphaCutLeft(weight).Get : function.PeakLeft.Get;
        return Math.Clamp(lastOfMaxima, min, max);
    }
}