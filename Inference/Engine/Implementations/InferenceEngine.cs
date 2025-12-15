using Inference.Aggregator.Factory;
using Inference.Defuzzifier.Factory;
using Inference.Engine.Abstractions;
using Inference.Tree.Extensions;
using Inference.Tree.Implementations;
using Kernel.Function.Implication.Factory;
using Kernel.Operator.Family.Abstractions;
using Knowledge.Memory.Abstractions;
using Reasoning.Adaptation.Components;
using Reasoning.Base.Abstractions;
using Reasoning.Comparer.Implementations.Deterministic.Factory;
using Reasoning.Rule.Abstractions;
using Reasoning.Rule.Extensions;
using Shared.Options.Factory;
using Shared.Options.Implementations;

namespace Inference.Engine.Implementations;

public class InferenceEngine : IEngine
{
    public required IRuleBase RuleBase { get; init; }
    public required IWorkingMemory WorkingMemory { get; init; }
    public required IOperatorFamily OperatorFamily { get; set; }
    public ImplicationMethod ImplicationMethod { get; set; }
    public DefuzzificationMethod DefuzzificationMethod { get; set; }
    public DeterministicMethod DeterministicMethod { get; set; }
    public ValueAggregatorMethod AggregatorMethod { get; set; }
    public bool IsLearningEnabled { get; set; }
    public uint CurrentIteration { get; set; }
    public Option<AdaptationConfig> AdaptationConfig { get; set; } = OptionFactory.None<AdaptationConfig>();

    public void EnableLearning(AdaptationPolicy policy, bool resetPreviousHistory = true) =>
        EnableLearning(new AdaptationConfig(policy), resetPreviousHistory);

    public void EnableLearning(AdaptationConfig config, bool resetPreviousHistory = true)
    {
        if (resetPreviousHistory)
            ResetLearning();
        IsLearningEnabled = true;
        AdaptationConfig = config;
    }

    public void DisableLearning(bool resetPreviousHistory = true)
    {
        if (resetPreviousHistory)
            ResetLearning();
        IsLearningEnabled = false;
        AdaptationConfig = OptionFactory.None<AdaptationConfig>();
    }

    public void ResetLearning()
    {
        RuleBase.ResetLearning();
        CurrentIteration = 0;
    }

    private void UpdateLearning()
    {
        if (!IsLearningEnabled || !AdaptationConfig.IsSomeRef(out var config))
            return;
        RuleBase.UpdateLearning(config);
    }

    public Option<double> Defuzzify(string variableName, bool provideExplanation = true)
    {
        if (WorkingMemory.GetNumericFact(variableName).IsSomeVal(out var value))
            return value;
        var workingMemory = WorkingMemory.DeepCopy();
        var rules = new List<IFuzzySetRule>(RuleBase.ProductionRules);
        rules = rules.FilterFacts(workingMemory).ToList();
        rules = rules.FilterCircularDependencies(variableName).ToList();
        var operatorFamily = OperatorFamily.DeepCopy();
        var ruleComparer = DeterministicFactory.GetInstance(DeterministicMethod, workingMemory, operatorFamily);
        var defuzzifier = DefuzzificationFactory.GetInstance(DefuzzificationMethod);
        var aggregator = ValueAggregatorFactory.GetInstance(AggregatorMethod);
        var rootNode = DerivationTree.BuildTree(variableName, rules);
        var inferredValue = rootNode.InferFact(workingMemory, ruleComparer, defuzzifier, aggregator, operatorFamily, ImplicationMethod, CurrentIteration);
        if (IsLearningEnabled)
            UpdateLearning();
        if (!provideExplanation)
            return inferredValue;
        rootNode.PrettyWriteTree();
        Console.WriteLine();
        rootNode.WriteTree();
        return inferredValue;
    }
}