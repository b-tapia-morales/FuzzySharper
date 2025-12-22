using Inference.Aggregator.Factory;
using Inference.Defuzzifier.Factory;
using Inference.Engine.Abstractions;
using Inference.Engine.Fuzzy.Abstractions;
using Inference.Tree.Extensions;
using Inference.Tree.Implementations;
using Kernel.Function.Implication.Factory;
using Kernel.Operator.Family.Abstractions;
using Knowledge.Memory.Abstractions;
using Reasoning.Adaptation.Components;
using Reasoning.Base.FuzzySet.Abstractions;
using Reasoning.Base.FuzzySet.Extensions;
using Reasoning.Rule.FuzzySet.Abstractions;
using Reasoning.Rule.FuzzySet.Comparer.Implementations.Deterministic.Factory;
using Shared.Options.Factory;
using Shared.Options.Implementations;

namespace Inference.Engine.Fuzzy.Implementations;

public class FuzzyConsequentEngine : AbstractInferenceEngine, IFuzzyConsequentEngine
{
    public override required IFuzzySetRuleBase RuleBase { get; init; }
    public override required IWorkingMemory WorkingMemory { get; init; }
    public override required IOperatorFamily OperatorFamily { get; set; }
    public ImplicationMethod ImplicationMethod { get; set; }
    public DefuzzificationMethod DefuzzificationMethod { get; set; }
    public DeterministicMethod DeterministicMethod { get; set; }
    public ValueAggregatorMethod AggregatorMethod { get; set; }
    public override bool IsLearningEnabled { get; set; }
    public override uint CurrentIteration { get; set; }
    public override Option<AdaptationConfig> AdaptationConfig { get; set; } = Option<AdaptationConfig>.None();

    public Option<double> Defuzzify(string target, bool provideExplanation = true)
    {
        if (WorkingMemory.GetNumericFact(target).IsSome(out var value))
            return value;
        var workingMemory = WorkingMemory.DeepCopy();
        var rules = new List<IFuzzySetRule>(RuleBase.ProductionRules);
        rules = rules.FilterFacts(workingMemory).ToList();
        rules = rules.FilterCircularDependencies(target).ToList();
        var operatorFamily = OperatorFamily.DeepCopy();
        var ruleComparer = DeterministicFactory.GetInstance(DeterministicMethod, workingMemory, operatorFamily);
        var defuzzifier = DefuzzificationFactory.GetInstance(DefuzzificationMethod);
        var aggregator = ValueAggregatorFactory.GetInstance(AggregatorMethod);
        var rootNode = DerivationTree.BuildTree(target, rules);
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