using Knowledge.Linguistic.Base.Abstractions;
using Knowledge.Memory.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Reasoning.Base.Extensions;
using Reasoning.Base.FuzzySet.Abstractions;
using Reasoning.Base.FuzzySet.Extensions;
using Reasoning.Rule.FuzzySet.Implementations;
using Shared.Primitives.Implementation;
using Xunit.Abstractions;

namespace Reasoning.Tests.Rule.Base;

public class RuleBaseTests(ITestOutputHelper output)
{
    private static readonly IServiceProvider ServiceProvider = ReasoningProvider.ConfigureProvider();
    private static readonly IWorkingMemory Memory = ServiceProvider.GetService<IWorkingMemory>()!;
    private static readonly ILinguisticBase Linguistics = ServiceProvider.GetService<ILinguisticBase>()!;
    private static readonly IFuzzySetRuleBase Rules = ServiceProvider.GetService<IFuzzySetRuleBase>()!;

    [Theory]
    [ClassData(typeof(BaseVariables))]
    public void BaseVariablesAreAsSpecified(StringOrType variable)
    {
        var baseVariables = Rules.GetBaseVariables();
        output.WriteLine(string.Join(Environment.NewLine, baseVariables));
        output.WriteLine(baseVariables.Count.ToString());
        Assert.True(baseVariables.Contains(variable));
    }

    [Fact]
    public void InferredVariablesIsSingle()
    {
        var inferredVariables = Rules.GetInferredVariables();
        Assert.NotEmpty(inferredVariables);
        Assert.Single(inferredVariables);
        Assert.Contains("Heating power", inferredVariables);
    }

    [Theory]
    [ClassData(typeof(BaseVariables))]
    public void BaseVariablesHaveNoDependencies(StringOrType variable)
    {
        var graph = Rules.BuildDependencyGraph();
        Assert.Contains(variable, graph);
        Assert.Empty(graph[variable]);
    }

    [Theory]
    [ClassData(typeof(BaseVariables))]
    public void PremiseVariablesAreFound(StringOrType variable)
    {
        var rules = Rules.FindByPremise(variable).ToList();
        output.WriteLine(string.Join(Environment.NewLine, rules));
        Assert.NotEmpty(rules);
    }

    [Fact]
    public void AllRulesAreApplicable()
    {
        Assert.True(Rules.ProductionRules.All(r => r.IsPremiseEvaluable(Memory)));
        Assert.True(Rules.ProductionRules.All(r => r.EvaluatePremiseWeight(Memory).IsSome));
        Assert.Equal(12, Rules.ProductionRules.FindByConclusion("Heating power").Count());
    }

    [Fact]
    public void VariableKnownAsFactIsFilteredOut()
    {
        var ruleBase = Rules.DeepCopy();
        var factRule = BoundFuzzySetRule.Create(Linguistics)
            .If("Humidity", "humid")
            .Then("Room temperature", "cold");
        ruleBase.Add(factRule);
        var filteredRules = ruleBase.ProductionRules.FilterFacts(Memory).ToList();
        output.WriteLine(string.Join(Environment.NewLine, filteredRules));
        Assert.DoesNotContain(factRule, filteredRules);
        Assert.True(filteredRules.SequenceEqual(Rules.ProductionRules));
    }

    [Fact]
    public void CircularDependencyIsFilteredOut()
    {
        var ruleBase = Rules.DeepCopy();
        var circularRule = BoundFuzzySetRule.Create(Linguistics)
            .If("Heating power", "Low")
            .Then("Room temperature", "cold");
        ruleBase.Add(circularRule);
        var filteredRules = ruleBase.FilterCircularDependencies("Heating power").ToList();
        output.WriteLine(string.Join(Environment.NewLine, filteredRules));
        Assert.DoesNotContain(circularRule, filteredRules);
        Assert.True(filteredRules.SequenceEqual(Rules.ProductionRules));
    }
}

file class BaseVariables : TheoryData<StringOrType>
{
    public BaseVariables()
    {
        Add("Room temperature");
        Add("Humidity");
        Add("Outside temperature");
        Add(typeof(Window));
        Add(typeof(Occupancy));
    }
}