using Inference.Tree.Functional.Abstractions;
using Inference.Tree.Functional.Implementations;
using Kernel.Function.Abstractions;
using Kernel.Function.Implementations;
using Kernel.Operator.Family.Factory.Canonical;
using Knowledge.Memory.Implementations;
using Reasoning.Proposition.Components;
using Reasoning.Proposition.Implementations;
using Reasoning.Rule.Functional.Abstractions;
using Reasoning.Rule.Functional.Implementations;
using Shared.Options.Factory;
using Shared.Options.Implementations;

namespace Inference.Tests.Tree.Functional;

public class SugenoGoalTreeTests
{
    private static readonly Kernel.Operator.Family.Abstractions.IOperatorFamily Operators =
        CanonicalFactory.UseFamily(CanonicalType.Godel);

    [Fact]
    public void InferFactAggregatesRulesUsingTheirPremiseWeights()
    {
        var memory = WorkingMemory.Create(("A", 0.5));
        ICollection<IFunctionalRule> rules =
        [
            Rule("Target", "A", 20, TriangleFunction.Create("Wide", 0, 1, 2)),
            Rule("Target", "A", 60)
        ];
        ISugenoGoalTree<SugenoGoalTree> tree = SugenoGoalTree.BuildTree("Target", rules);

        Assert.True(tree.InferFact(memory, Operators).IsSome(out var value));
        Assert.Equal(23.333333333333332, value, 10);
        Assert.Equal(value, memory.GetNumericFact("Target").Get);
        Assert.True(tree.IsProven);
        Assert.All(rules, rule => Assert.Empty(rule.AdaptationState.LearningHistory));
    }

    [Fact]
    public void InferFactDerivesDependenciesBeforeTheTarget()
    {
        var memory = WorkingMemory.Create(("A", 0.25));
        ICollection<IFunctionalRule> rules = [Rule("C", "B", 4), Rule("B", "A", 2)];
        var tree = SugenoGoalTree.BuildTree("C", rules);

        Assert.True(tree.InferFact(memory, Operators).IsSome(out var value));
        Assert.Equal(2, value);
        Assert.Equal(0.5, memory.GetNumericFact("B").Get);
        Assert.True(tree.Children.Single().IsProven);
        Assert.True(tree.Children.Single().Children.Single().IsProven);
    }

    [Fact]
    public void InferFactUsesEligibleRulesWhenAnotherDependencyIsUnavailable()
    {
        var memory = WorkingMemory.Create(("A", 0.5));
        ICollection<IFunctionalRule> rules = [Rule("C", "A", 2), Rule("C", "Missing", 100)];
        var tree = SugenoGoalTree.BuildTree("C", rules);

        Assert.True(tree.InferFact(memory, Operators).IsSome(out var value));
        Assert.Equal(1, value);
        Assert.True(tree.IsProven);
        Assert.False(tree.Children.Single(node => node.Identifier.AsString == "Missing").IsProven);
        Assert.False(memory.ContainsNumericFact("Missing"));
    }

    [Fact]
    public void InferFactReusesKnownDependencyWithoutItsProducingInputs()
    {
        var memory = WorkingMemory.Create(("B", 0.75));
        ICollection<IFunctionalRule> rules = [Rule("C", "B", 4), Rule("B", "A", 2)];
        var tree = SugenoGoalTree.BuildTree("C", rules);

        Assert.True(tree.InferFact(memory, Operators).IsSome(out var value));
        Assert.Equal(3, value);
        Assert.Equal(0.75, memory.GetNumericFact("B").Get);
        Assert.False(memory.ContainsNumericFact("A"));
    }

    [Fact]
    public void InferFactReturnsKnownRootWithoutDerivingItsDependenciesOrRecording()
    {
        var memory = WorkingMemory.Create(("C", 99), ("A", 0.25));
        ICollection<IFunctionalRule> rules = [Rule("C", "B", 4), Rule("B", "A", 2)];
        var tree = SugenoGoalTree.BuildTree("C", rules);

        Assert.True(tree.InferFact(memory, Operators, Option<uint>.Some(7U)).IsSome(out var value));
        Assert.Equal(99, value);
        Assert.True(tree.IsProven);
        Assert.False(memory.ContainsNumericFact("B"));
        Assert.All(rules, rule => Assert.Equal(0U, rule.AdaptationState.FireCount));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void InferFactRequiresPositiveActivationAndAvailableFacts(bool provideZeroActivationFact)
    {
        var memory = WorkingMemory.Create();
        if (provideZeroActivationFact)
            memory.AddNumericFact("A", 0);
        ICollection<IFunctionalRule> rules = [Rule("C", "A", 2)];
        var tree = SugenoGoalTree.BuildTree("C", rules);

        Assert.False(tree.InferFact(memory, Operators).IsSome);
        Assert.False(tree.IsProven);
        Assert.False(memory.ContainsNumericFact("C"));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void InferFactUsesBooleanFactsAndPreservesAValidZeroOutput(bool provideBooleanFact)
    {
        var memory = WorkingMemory.Create();
        if (provideBooleanFact)
            memory.AddCategoricalFact(DayOfWeek.Monday);
        ICollection<IFunctionalRule> rules =
        [
            new UnboundedFunctionalRule
            {
                Conditional = new BooleanProposition<DayOfWeek>(DayOfWeek.Monday, Connective.If, Literal.Is),
                Consequent = new FunctionalConsequent
                {
                    Identifier = "Target",
                    CoefficientDict = new Dictionary<string, double>()
                },
                IsFinalized = true
            }
        ];
        var tree = SugenoGoalTree.BuildTree("Target", rules);

        var result = tree.InferFact(memory, Operators);

        Assert.Equal(provideBooleanFact, result.IsSome);
        Assert.Equal(provideBooleanFact, tree.IsProven);
        Assert.Equal(provideBooleanFact, tree.Children.Single().IsProven);
        Assert.Equal(provideBooleanFact, memory.ContainsNumericFact("Target"));
        if (provideBooleanFact)
            Assert.Equal(0, result.Get);
    }

    [Fact]
    public void InferFactLeavesAnUnknownTargetWithoutRulesUnproven()
    {
        var tree = SugenoGoalTree.BuildTree("Unknown", new List<IFunctionalRule>());
        tree.IsProven = true;

        Assert.False(tree.InferFact(WorkingMemory.Create(), Operators).IsSome);
        Assert.False(tree.IsProven);
    }

    [Fact]
    public void InferFactDoesNotTreatABooleanRootAsANumericTarget()
    {
        var memory = WorkingMemory.Create();
        memory.AddCategoricalFact(DayOfWeek.Monday);
        var tree = new SugenoGoalTree(typeof(DayOfWeek));

        Assert.False(tree.InferFact(memory, Operators).IsSome);
        Assert.True(tree.IsProven);
    }

    [Fact]
    public void InferFactWithAnIterationReportsUnimplementedRecordingBeforeAddingTheFact()
    {
        var memory = WorkingMemory.Create(("A", 0.5));
        var rule = Rule("C", "A", 2);
        var tree = SugenoGoalTree.BuildTree("C", new List<IFunctionalRule> { rule });

        Assert.Throws<NotImplementedException>(() => tree.InferFact(memory, Operators, Option<uint>.Some(7U)));
        Assert.False(memory.ContainsNumericFact("C"));
        Assert.False(tree.IsProven);
        Assert.Equal(0U, rule.AdaptationState.FireCount);
    }

    private static IFunctionalRule Rule(string target, string input, double coefficient, IMembershipFunction? function = null) =>
        new UnboundedFunctionalRule
        {
            Conditional = new FuzzyProposition(input, Connective.If, Literal.Is, LinguisticHedge.None,
                function ?? TriangleFunction.Create("Input", 0, 0.5, 1)),
            Consequent = new FunctionalConsequent
            {
                Identifier = target,
                CoefficientDict = new Dictionary<string, double> { [input] = coefficient }
            },
            IsFinalized = true
        };
}
