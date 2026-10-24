using Knowledge.Linguistic.Base.Abstractions;
using Knowledge.Memory.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Reasoning.Base.Extensions;
using Reasoning.Base.FuzzySet.Abstractions;
using Reasoning.Rule.FuzzySet.Implementations;
using Shared.Primitives.Implementation;

namespace Reasoning.Tests.Rule.Base;

public sealed class RuleBaseTests : IDisposable
{
    private readonly ServiceProvider _provider;
    private readonly IServiceScope _scope;
    private readonly IWorkingMemory _memory;
    private readonly ILinguisticBase _linguistics;
    private readonly IFuzzySetRuleBase _rules;

    public RuleBaseTests()
    {
        _provider = ReasoningProvider.ConfigureProvider();
        _scope = _provider.CreateScope();
        _memory = _scope.ServiceProvider.GetRequiredService<IWorkingMemory>();
        _linguistics = _scope.ServiceProvider.GetRequiredService<ILinguisticBase>();
        _rules = _scope.ServiceProvider.GetRequiredService<IFuzzySetRuleBase>();
    }

    [Theory]
    [ClassData(typeof(BaseVariables))]
    public void BaseVariablesAreAsSpecified(StringOrType variable)
    {
        var baseVariables = _rules.GetBaseVariables();
        Assert.Equal(5, baseVariables.Count);
        Assert.Contains(variable, baseVariables);
    }

    [Fact]
    public void InferredVariablesIsSingle()
    {
        var inferredVariables = _rules.GetInferredVariables();
        Assert.NotEmpty(inferredVariables);
        Assert.Single(inferredVariables);
        Assert.Contains("Heating power", inferredVariables);
    }

    [Theory]
    [ClassData(typeof(BaseVariables))]
    public void BaseVariablesHaveNoDependencies(StringOrType variable)
    {
        var graph = _rules.BuildDependencyGraph();
        Assert.Contains(variable, graph);
        Assert.Empty(graph[variable]);
    }

    [Theory]
    [ClassData(typeof(BaseVariables))]
    public void PremiseVariablesAreFound(StringOrType variable)
    {
        var rules = _rules.FindByPremise(variable).ToList();
        Assert.NotEmpty(rules);
    }

    [Fact]
    public void AllRulesAreApplicable()
    {
        Assert.All(_rules.ProductionRules, rule =>
        {
            Assert.True(rule.IsPremiseEvaluable(_memory));
            Assert.True(rule.EvaluatePremiseWeight(_memory).IsSome);
        });
        Assert.Equal(12, _rules.ProductionRules.FindByConclusion("Heating power").Count());
    }

    [Fact]
    public void VariableKnownAsFactIsFilteredOut()
    {
        var ruleBase = _rules.DeepCopy();
        var factRule = BoundedFuzzySetRule.Create(_linguistics)
            .If("Humidity", "humid")
            .Then("Room temperature", "cold");
        ruleBase.Add(factRule);
        var filteredRules = ruleBase.ProductionRules.FilterFacts(_memory).ToList();
        Assert.DoesNotContain(factRule, filteredRules);
        Assert.Equal(_rules.ProductionRules, filteredRules);
    }

    [Fact]
    public void CircularDependencyIsFilteredOut()
    {
        var ruleBase = _rules.DeepCopy();
        var circularRule = BoundedFuzzySetRule.Create(_linguistics)
            .If("Heating power", "Low")
            .Then("Room temperature", "cold");
        ruleBase.Add(circularRule);
        var filteredRules = ruleBase.FilterCircularDependencies("Heating power").ToList();
        Assert.DoesNotContain(circularRule, filteredRules);
        Assert.Equal(_rules.ProductionRules, filteredRules);
    }

    [Fact]
    public void DeepCopy_PreservesRulesWithoutSharingRuleOrAdaptationInstances()
    {
        var copy = _rules.DeepCopy();

        Assert.Equal(_rules.ProductionRules, copy.ProductionRules);
        foreach (var (source, copied) in _rules.ProductionRules.Zip(copy.ProductionRules))
        {
            Assert.NotSame(source, copied);
            Assert.NotSame(source.AdaptationState, copied.AdaptationState);
        }
    }

    [Fact]
    public void SeparateScopes_HaveIndependentFactsLinguisticsAndRules()
    {
        using var otherScope = _provider.CreateScope();
        var otherMemory = otherScope.ServiceProvider.GetRequiredService<IWorkingMemory>();
        var otherLinguistics = otherScope.ServiceProvider.GetRequiredService<ILinguisticBase>();
        var otherRules = otherScope.ServiceProvider.GetRequiredService<IFuzzySetRuleBase>();

        _memory.UpdateNumericFact("Room temperature", 0);

        Assert.NotSame(_memory, otherMemory);
        Assert.NotSame(_linguistics, otherLinguistics);
        Assert.NotSame(_rules, otherRules);
        Assert.Equal(24, otherMemory.GetNumericFact("Room temperature").Get);
        foreach (var (rule, otherRule) in _rules.ProductionRules.Zip(otherRules.ProductionRules))
        {
            Assert.NotSame(rule, otherRule);
            Assert.NotSame(rule.AdaptationState, otherRule.AdaptationState);
        }
    }

    public void Dispose()
    {
        _scope.Dispose();
        _provider.Dispose();
    }
}