using Inference.Aggregator.Factory;
using Inference.Builder.Abstractions;
using Inference.Builder.Fuzzy.Abstractions;
using Inference.Defuzzifier.Factory;
using Inference.Engine.Fuzzy.Implementations;
using Kernel.Function.Implication.Factory;
using Kernel.Operator.Family.Factory.Canonical;
using Reasoning.Adaptation.Components;
using Reasoning.Base.FuzzySet.Abstractions;
using Reasoning.Rule.FuzzySet.Comparer.Implementations.Deterministic.Factory;
using Shared.Options.Factory;
using Shared.Options.Implementations;

namespace Inference.Builder.Fuzzy.Implementations;

public class FuzzyEngineBuilder :
    AbstractEngineBuilder<FuzzyEngineBuilder>, IFuzzyEngineBuilder<FuzzyEngineBuilder>, IFuzzyEngineCreator<FuzzyConsequentEngine>
{
    public IFuzzySetRuleBase? RuleBase { get; private set; }
    public ImplicationMethod ImplicationMethod { get; private set; }
    public DefuzzificationMethod DefuzzificationMethod { get; private set; }
    public DeterministicMethod ComparerMethod { get; private set; }
    public ValueAggregatorMethod AggregatorMethod { get; private set; }

    private FuzzyEngineBuilder()
    {
        OperatorFamily = CanonicalFactory.UseFamily(CanonicalType.Godel);
        ImplicationMethod = ImplicationMethod.Mamdani;
        DefuzzificationMethod = DefuzzificationMethod.MeanOfMaxima;
        ComparerMethod = DeterministicMethod.PremiseWeight;
        AggregatorMethod = ValueAggregatorMethod.Mean;
        IsLearningEnabled = false;
        AdaptationPolicy = Option<AdaptationPolicy>.None();
    }

    public static FuzzyEngineBuilder Create() =>
        new();

    public FuzzyEngineBuilder WithRuleBase(IFuzzySetRuleBase ruleBase)
    {
        RuleBase = ruleBase;
        return this;
    }

    public FuzzyEngineBuilder WithImplication(ImplicationMethod method)
    {
        ImplicationMethod = method;
        return this;
    }

    public FuzzyEngineBuilder WithDefuzzification(DefuzzificationMethod method)
    {
        DefuzzificationMethod = method;
        return this;
    }

    public FuzzyEngineBuilder WithRuleComparer(DeterministicMethod method)
    {
        ComparerMethod = method;
        return this;
    }

    public FuzzyEngineBuilder WithValueAggregator(ValueAggregatorMethod method)
    {
        AggregatorMethod = method;
        return this;
    }

    public FuzzyConsequentEngine Build()
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