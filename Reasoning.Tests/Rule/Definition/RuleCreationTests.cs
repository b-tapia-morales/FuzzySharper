using Knowledge.Linguistic.Base.Abstractions;
using Knowledge.Linguistic.Base.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Reasoning.Rule.Exceptions;
using Reasoning.Rule.FuzzySet.Exceptions;
using Reasoning.Rule.FuzzySet.Implementations;

namespace Reasoning.Tests.Rule.Definition;

public sealed class RuleCreationTests : IDisposable
{
    private readonly ServiceProvider _provider;
    private readonly IServiceScope _scope;
    private readonly ILinguisticBase _linguistics;

    public RuleCreationTests()
    {
        _provider = ReasoningProvider.ConfigureProvider();
        _scope = _provider.CreateScope();
        _linguistics = _scope.ServiceProvider.GetRequiredService<ILinguisticBase>();
    }
    
    [Fact]
    public void CreationThrowsOnMissingConditional()
    {
        Assert.Throws<MissingAntecedentException>(() => BoundedFuzzySetRule.Create(_linguistics).And("Room Temperature", "Warm"));
        Assert.Throws<MissingAntecedentException>(() => BoundedFuzzySetRule.Create(_linguistics).Or("Room Temperature", "Warm"));
        Assert.Throws<MissingAntecedentException>(() => BoundedFuzzySetRule.Create(_linguistics).AndNot("Room Temperature", "Hot"));
        Assert.Throws<MissingAntecedentException>(() => BoundedFuzzySetRule.Create(_linguistics).OrNot("Room Temperature", "Hot"));
        Assert.Throws<MissingAntecedentException>(() => BoundedFuzzySetRule.Create(_linguistics).And(Window.Open));
        Assert.Throws<MissingAntecedentException>(() => BoundedFuzzySetRule.Create(_linguistics).Or(Window.Open));
        Assert.Throws<MissingAntecedentException>(() => BoundedFuzzySetRule.Create(_linguistics).AndNot(Occupancy.Occupied));
        Assert.Throws<MissingAntecedentException>(() => BoundedFuzzySetRule.Create(_linguistics).OrNot(Occupancy.Occupied));
    }

    [Fact]
    public void CreationThrowsOnDuplicatedConditional()
    {
        Assert.Throws<DuplicatedAntecedentException>(() => BoundedFuzzySetRule.Create(_linguistics).If("room temperature", "Warm").If("outside temperature", "freezing"));
        Assert.Throws<DuplicatedAntecedentException>(() => BoundedFuzzySetRule.Create(_linguistics).If(Window.Open).If(Occupancy.Occupied));
    }
    
    [Fact]
    public void CreationThrowsOnVariableNotFound()
    {
        Assert.Throws<VariableNotFoundException>(() => BoundedFuzzySetRule.Create(_linguistics).If("Air conditioner temperature", "Low"));
        Assert.Throws<VariableNotFoundException>(() => BoundedFuzzySetRule.Create(_linguistics).If("Thermostat temperature", "Low"));
    }

    [Fact]
    public void CreationThrowsOnFunctionNotFound()
    {
        Assert.Throws<EntryNotFoundException>(() => BoundedFuzzySetRule.Create(_linguistics).If("Room Temperature", "Freezing"));
        Assert.Throws<EntryNotFoundException>(() => BoundedFuzzySetRule.Create(_linguistics).If("Outside Temperature", "Hell on earth"));
    }

    [Fact]
    public void CreationThrowsOnVariableOverlap()
    {
        Assert.Throws<VariableOverlapException>(() => BoundedFuzzySetRule.Create(_linguistics).If("Room Temperature", "Warm").Then("Room Temperature", "Cold"));
        Assert.Throws<VariableOverlapException>(() => BoundedFuzzySetRule.Create(_linguistics).If("Humidity", "Dry").Then("humidity", "humid"));
    }

    [Fact]
    public void CreationThrowsOnAdditionsInFinalizedRule()
    {
        Assert.Throws<FinalizedRuleException>(() => BoundedFuzzySetRule.Create(_linguistics)
            .If("Room Temperature", "Warm")
            .Then("Heating power", "Low")
            .And(Window.Open));
        Assert.Throws<FinalizedRuleException>(() => BoundedFuzzySetRule.Create(_linguistics).If("Room Temperature", "Warm")
            .Then("Heating power", "Low")
            .And("Outside Temperature", "Freezing"));
    }

    public void Dispose()
    {
        _scope.Dispose();
        _provider.Dispose();
    }
}