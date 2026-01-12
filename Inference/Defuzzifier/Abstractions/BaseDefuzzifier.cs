using Inference.Aggregator.Abstractions;
using Inference.Aggregator.Components;
using Inference.Aggregator.Factory;
using Inference.Defuzzifier.Exceptions;
using Kernel.Function.Implication.Factory;
using Kernel.Operator.Family.Abstractions;
using Kernel.Operator.Family.Factory.Canonical;
using Knowledge.Memory.Abstractions;
using Reasoning.Rule.FuzzySet.Abstractions;
using Reasoning.Rule.FuzzySet.Components;
using Shared.Options.Extensions;
using Shared.Options.Implementations;

// ReSharper disable RedundantExplicitTupleComponentName

namespace Inference.Defuzzifier.Abstractions;

public abstract class BaseDefuzzifier : IDefuzzifier
{
    protected abstract Option<double> DefuzzifyMethod(ICollection<IFuzzySetRule> rules, IWorkingMemory memory,
        IOperatorFamily family, IValueAggregator aggregator, ImplicationMethod method,
        out ICollection<IFuzzySetRule> activatedRules);

    public Option<double> Defuzzify(ICollection<IFuzzySetRule> rules, IWorkingMemory memory,
        IOperatorFamily family, IValueAggregator aggregator, ImplicationMethod method,
        out ICollection<IFuzzySetRule> activatedRules)
    {
        if (rules.Count == 0)
            throw new ArgumentException("The list of rules provided as a parameter contains no elements");

        if (!rules.Any(e => e.IsPremiseEvaluable(memory)))
            throw new InapplicableRulesException();

        var firstConsequent = rules.First().Consequent!.Identifier;
        if (!rules.Select(e => e.Consequent!.Identifier).All(e => string.Equals(e, firstConsequent, StringComparison.OrdinalIgnoreCase)))
            throw new ConsequentMismatchException();

        return DefuzzifyMethod(rules, memory, family, aggregator, method, out activatedRules);
    }

    public Option<double> Defuzzify(ICollection<IFuzzySetRule> rules, IWorkingMemory memory,
        IOperatorFamily family, ImplicationMethod method,
        out ICollection<IFuzzySetRule> activatedRules) =>
        Defuzzify(rules, memory, family, ValueAggregatorFactory.GetInstance(ValueAggregatorMethod.Mean), method, out activatedRules);
    
    public Option<double> Defuzzify(ICollection<IFuzzySetRule> rules, IWorkingMemory memory,
        IValueAggregator aggregator, ImplicationMethod method,
        out ICollection<IFuzzySetRule> activatedRules) =>
        Defuzzify(rules, memory, CanonicalFactory.UseFamily(CanonicalType.Godel), aggregator, method, out activatedRules);

    public Option<double> Defuzzify(ICollection<IFuzzySetRule> rules, IWorkingMemory memory, 
        ImplicationMethod method,
        out ICollection<IFuzzySetRule> activatedRules) => 
        Defuzzify(rules, memory, ValueAggregatorFactory.GetInstance(ValueAggregatorMethod.Mean), method, out activatedRules);
    
    

    protected static IList<FiringStrength> EvaluateFiringStrengths(ICollection<IFuzzySetRule> rules, IWorkingMemory memory,
        IOperatorFamily family) =>
        rules
            .Select(rule => new FiringStrength(rule, ((FuzzySetConsequent) rule.Consequent!).Proposition.Function, rule.EvaluatePremiseWeight(memory, family).OrElse(0)))
            .Where(tuple => tuple.Weight > 0)
            .ToList();
}