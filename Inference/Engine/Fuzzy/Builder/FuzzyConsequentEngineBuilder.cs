using Inference.Aggregator.Factory;
using Inference.Defuzzifier.Factory;
using Inference.Engine.Fuzzy.Abstractions;
using Inference.Engine.Fuzzy.Implementations;
using Kernel.Function.Implication.Factory;
using Kernel.Operator.Conorm.Abstractions;
using Kernel.Operator.Family.Abstractions;
using Kernel.Operator.Family.Factory.Canonical;
using Kernel.Operator.Negation.Abstractions;
using Kernel.Operator.Norm.Abstractions;
using Kernel.Operator.Residuum.Abstractions;
using Knowledge.Memory.Abstractions;
using Reasoning.Adaptation.Components;
using Reasoning.Base.FuzzySet.Abstractions;
using Reasoning.Rule.FuzzySet.Comparer.Implementations.Deterministic.Factory;
using Shared.Options.Factory;
using Shared.Options.Implementations;

namespace Inference.Engine.Fuzzy.Builder;

public class FuzzyConsequentEngineBuilder
{
    private IFuzzySetRuleBase? RuleBase { get; set; }
    private IWorkingMemory? WorkingMemory { get; set; }
    private IOperatorFamily OperatorFamily { get; set; }
    private ImplicationMethod ImplicationMethod { get; set; }
    private DefuzzificationMethod DefuzzificationMethod { get; set; }
    private DeterministicMethod ComparerMethod { get; set; }
    private ValueAggregatorMethod AggregatorMethod { get; set; }
    private bool IsLearningEnabled { get; set; }
    private Option<AdaptationPolicy> AdaptationPolicy { get; set; }

    private FuzzyConsequentEngineBuilder()
    {
        OperatorFamily = CanonicalFactory.UseFamily(CanonicalType.Godel);
        ImplicationMethod = ImplicationMethod.Mamdani;
        DefuzzificationMethod = DefuzzificationMethod.MeanOfMaxima;
        ComparerMethod = DeterministicMethod.PremiseWeight;
        AggregatorMethod = ValueAggregatorMethod.Mean;
        IsLearningEnabled = false;
        AdaptationPolicy = Option<AdaptationPolicy>.None();
    }

    public static FuzzyConsequentEngineBuilder Create() =>
        new();

    public FuzzyConsequentEngineBuilder WithRuleBase(IFuzzySetRuleBase ruleBase)
    {
        RuleBase = ruleBase;
        return this;
    }

    public FuzzyConsequentEngineBuilder WithWorkingMemory(IWorkingMemory workingMemory)
    {
        WorkingMemory = workingMemory;
        return this;
    }

    public FuzzyConsequentEngineBuilder WithOperatorFamily(IOperatorFamily operatorFamily)
    {
        OperatorFamily = operatorFamily.DeepCopy();
        return this;
    }

    public FuzzyConsequentEngineBuilder WithImplication(ImplicationMethod method)
    {
        ImplicationMethod = method;
        return this;
    }

    public FuzzyConsequentEngineBuilder WithDefuzzification(DefuzzificationMethod method)
    {
        DefuzzificationMethod = method;
        return this;
    }

    public FuzzyConsequentEngineBuilder WithRuleComparer(DeterministicMethod method)
    {
        ComparerMethod = method;
        return this;
    }

    public FuzzyConsequentEngineBuilder WithNegation(INegation negation)
    {
        OperatorFamily.Negation = negation;
        return this;
    }

    public FuzzyConsequentEngineBuilder WithDisjunction(INorm norm)
    {
        OperatorFamily.Norm = norm;
        return this;
    }

    public FuzzyConsequentEngineBuilder WithConjunction(IConorm conorm)
    {
        OperatorFamily.Conorm = conorm;
        return this;
    }

    public FuzzyConsequentEngineBuilder WithResiduum(IResiduum residuum)
    {
        OperatorFamily.Residuum = residuum;
        return this;
    }

    public FuzzyConsequentEngineBuilder WithOperators(INegation negation, INorm norm, IConorm conorm, IResiduum residuum)
    {
        OperatorFamily.Negation = negation;
        OperatorFamily.Norm = norm;
        OperatorFamily.Conorm = conorm;
        OperatorFamily.Residuum = residuum;
        return this;
    }

    public FuzzyConsequentEngineBuilder WithCanonicalFamily(CanonicalType type)
    {
        OperatorFamily = CanonicalFactory.UseFamily(type);
        return this;
    }

    public FuzzyConsequentEngineBuilder WithAdaptationPolicy(AdaptationPolicy adaptationPolicy)
    {
        IsLearningEnabled = true;
        AdaptationPolicy = adaptationPolicy;
        return this;
    }

    public FuzzyConsequentEngineBuilder WithValueAggregator(ValueAggregatorMethod method)
    {
        AggregatorMethod = method;
        return this;
    }

    public IFuzzyConsequentEngine Build()
    {
        ArgumentNullException.ThrowIfNull(WorkingMemory);
        ArgumentNullException.ThrowIfNull(RuleBase);
        return new FuzzyConsequentEngine
        {
            WorkingMemory = WorkingMemory,
            RuleBase = RuleBase,
            OperatorFamily = OperatorFamily,
            ImplicationMethod = ImplicationMethod,
            DefuzzificationMethod = DefuzzificationMethod,
            DeterministicMethod = ComparerMethod,
            AggregatorMethod = AggregatorMethod,
            IsLearningEnabled = IsLearningEnabled,
            AdaptationConfig = AdaptationPolicy.IsSome(out var policy)
                ? new AdaptationConfig(policy)
                : Option<AdaptationConfig>.None()
        };
    }
}