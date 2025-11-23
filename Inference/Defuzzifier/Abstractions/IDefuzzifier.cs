using Inference.Defuzzifier.Exceptions;
using Kernel.Function.Abstractions;
using Kernel.Function.Extensions;
using Kernel.Number;
using Kernel.Operator.Conorm.Abstractions;
using Kernel.Operator.Conorm.Implementations.Canonical;
using Kernel.Operator.Family.Abstractions;
using Kernel.Operator.Negation.Abstractions;
using Kernel.Operator.Negation.Implementations.Canonical;
using Kernel.Operator.Norm.Abstractions;
using Kernel.Operator.Norm.Implementations.Canonical;
using Knowledge.Memory.Abstractions;
using Reasoning.Rule.Abstractions;
using Shared.Intervals.Implementations;
using Shared.Options.Extensions;
using Shared.Options.Implementations;

// ReSharper disable RedundantExplicitTupleComponentName

namespace Inference.Defuzzifier.Abstractions;

public interface IDefuzzifier
{
    internal static void RulesCheck(ICollection<IRule> rules, IWorkingMemory memory)
    {
        if (rules.Count == 0)
            throw new ArgumentException("The list of rules provided as a parameter contains no elements");

        if (!rules.Any(e => e.IsPremiseEvaluable(memory)))
            throw new InapplicableRulesException();

        var firstConsequent = rules.First().Consequent!.Variable;
        if (!rules.Select(e => e.Consequent!.Variable).All(e => string.Equals(e, firstConsequent, StringComparison.OrdinalIgnoreCase)))
            throw new ConsequentMismatchException();
    }

    internal static ICollection<(IRule Rule, IMembershipFunction Function, FuzzyNumber Weight)>
        GetWeightedTuples(ICollection<IRule> rules, IWorkingMemory memory, INegation negation, INorm norm, IConorm conorm) => rules
        .Select(rule => (
            Rule: rule,
            Function: rule.Consequent!.Function,
            Weight: rule.EvaluatePremiseWeight(memory, negation, norm, conorm).OrElse(0)))
        .Where(tuple => tuple.Weight > 0)
        .ToList();

    internal static Interval GetUniverse(ICollection<IRule> rules) =>
        rules.First().Consequent!.Function.UniverseOfDiscourse;

    Option<double> Defuzzify(ICollection<IRule> rules, IWorkingMemory memory,
        ImplicationMethod method, out ICollection<IRule> activatedRules) =>
        Defuzzify(rules, memory, Negation.Standard, Norm.Minimum, Conorm.Maximum, method, out activatedRules);

    Option<double> Defuzzify(ICollection<IRule> rules, IWorkingMemory memory,
        IOperatorFamily family, ImplicationMethod method, out ICollection<IRule> activatedRules) =>
        Defuzzify(rules, memory, family.Negation, family.Norm, family.Conorm, method, out activatedRules);

    Option<double> Defuzzify(ICollection<IRule> rules, IWorkingMemory memory,
        INegation negation, INorm norm, IConorm conorm, ImplicationMethod method, out ICollection<IRule> activatedRules);
}