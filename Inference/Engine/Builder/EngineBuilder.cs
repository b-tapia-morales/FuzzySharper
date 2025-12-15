using Inference.Aggregator.Factory;
using Inference.Defuzzifier.Factory;
using Inference.Engine.Abstractions;
using Inference.Engine.Implementations;
using Kernel.Function.Implication.Factory;
using Kernel.Operator.Conorm.Abstractions;
using Kernel.Operator.Family.Abstractions;
using Kernel.Operator.Family.Factory.Canonical;
using Kernel.Operator.Negation.Abstractions;
using Kernel.Operator.Norm.Abstractions;
using Kernel.Operator.Residuum.Abstractions;
using Knowledge.Memory.Abstractions;
using Reasoning.Adaptation.Components;
using Reasoning.Base.Abstractions;
using Reasoning.Comparer.Implementations.Deterministic.Factory;
using Shared.Options.Factory;
using Shared.Options.Implementations;

namespace Inference.Engine.Builder;

public class EngineBuilder
{
    private IRuleBase? RuleBase { get; set; }
    private IWorkingMemory? WorkingMemory { get; set; }
    private IOperatorFamily OperatorFamily { get; set; }
    private ImplicationMethod ImplicationMethod { get; set; }
    private DefuzzificationMethod DefuzzificationMethod { get; set; }
    private DeterministicMethod ComparerMethod { get; set; }
    private ValueAggregatorMethod AggregatorMethod { get; set; }
    private bool IsLearningEnabled { get; set; }
    private Option<AdaptationPolicy> AdaptationPolicy { get; set; }

    private EngineBuilder()
    {
        OperatorFamily = CanonicalFactory.UseFamily(CanonicalType.Godel);
        ImplicationMethod = ImplicationMethod.Mamdani;
        DefuzzificationMethod = DefuzzificationMethod.MeanOfMaxima;
        ComparerMethod = DeterministicMethod.PremiseWeight;
        AggregatorMethod = ValueAggregatorMethod.Mean;
        IsLearningEnabled = false;
        AdaptationPolicy = OptionFactory.None<AdaptationPolicy>();
    }

    public static EngineBuilder Create() =>
        new();

    public EngineBuilder WithRuleBase(IRuleBase ruleBase)
    {
        RuleBase = ruleBase;
        return this;
    }

    public EngineBuilder WithWorkingMemory(IWorkingMemory workingMemory)
    {
        WorkingMemory = workingMemory;
        return this;
    }
    
    public EngineBuilder WithOperatorFamily(IOperatorFamily operatorFamily)
    {
        OperatorFamily = operatorFamily.DeepCopy();
        return this;
    }

    public EngineBuilder WithImplication(ImplicationMethod method)
    {
        ImplicationMethod = method;
        return this;
    }

    public EngineBuilder WithDefuzzification(DefuzzificationMethod method)
    {
        DefuzzificationMethod = method;
        return this;
    }

    public EngineBuilder WithRuleComparer(DeterministicMethod method)
    {
        ComparerMethod = method;
        return this;
    }

    public EngineBuilder WithNegation(INegation negation)
    {
        OperatorFamily.Negation = negation;
        return this;
    }

    public EngineBuilder WithDisjunction(INorm norm)
    {
        OperatorFamily.Norm = norm;
        return this;
    }

    public EngineBuilder WithConjunction(IConorm conorm)
    {
        OperatorFamily.Conorm = conorm;
        return this;
    }

    public EngineBuilder WithResiduum(IResiduum residuum)
    {
        OperatorFamily.Residuum = residuum;
        return this;
    }

    public EngineBuilder WithOperators(INegation negation, INorm norm, IConorm conorm, IResiduum residuum)
    {
        OperatorFamily.Negation = negation;
        OperatorFamily.Norm = norm;
        OperatorFamily.Conorm = conorm;
        OperatorFamily.Residuum = residuum;
        return this;
    }

    public EngineBuilder WithCanonicalFamily(CanonicalType type)
    {
        OperatorFamily = CanonicalFactory.UseFamily(type);
        return this;
    }

    public EngineBuilder WithAdaptationPolicy(AdaptationPolicy adaptationPolicy)
    {
        IsLearningEnabled = true;
        AdaptationPolicy = adaptationPolicy;
        return this;
    }

    public EngineBuilder WithValueAggregator(ValueAggregatorMethod method)
    {
        AggregatorMethod = method;
        return this;
    }

    public IEngine Build()
    {
        ArgumentNullException.ThrowIfNull(WorkingMemory);
        ArgumentNullException.ThrowIfNull(RuleBase);
        return new InferenceEngine
        {
            WorkingMemory = WorkingMemory,
            RuleBase = RuleBase,
            OperatorFamily = OperatorFamily,
            ImplicationMethod = ImplicationMethod,
            DefuzzificationMethod = DefuzzificationMethod,
            DeterministicMethod = ComparerMethod,
            AggregatorMethod = AggregatorMethod,
            IsLearningEnabled = IsLearningEnabled,
            AdaptationConfig = AdaptationPolicy.IsSomeVal(out var policy)
                ? new AdaptationConfig(policy)
                : OptionFactory.None<AdaptationConfig>()
        };
    }
}