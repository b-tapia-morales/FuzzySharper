using Knowledge.Linguistic.Base.Abstractions;
using Knowledge.Linguistic.Base.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Reasoning.Rule.Exceptions;
using Reasoning.Rule.FuzzySet.Exceptions;
using Reasoning.Rule.FuzzySet.Implementations;

namespace Reasoning.Tests.Rule.Definition;

public class RuleCreationTests
{
    private static readonly IServiceProvider ServiceProvider = ReasoningProvider.ConfigureProvider();
    private static readonly ILinguisticBase Linguistics = ServiceProvider.GetService<ILinguisticBase>()!;
    
    [Fact]
    public void CreationThrowsOnMissingConditional()
    {
        Assert.Throws<MissingAntecedentException>(() => BoundedFuzzySetRule.Create(Linguistics).And("Room Temperature", "Warm"));
        Assert.Throws<MissingAntecedentException>(() => BoundedFuzzySetRule.Create(Linguistics).Or("Room Temperature", "Warm"));
        Assert.Throws<MissingAntecedentException>(() => BoundedFuzzySetRule.Create(Linguistics).AndNot("Room Temperature", "Hot"));
        Assert.Throws<MissingAntecedentException>(() => BoundedFuzzySetRule.Create(Linguistics).OrNot("Room Temperature", "Hot"));
        Assert.Throws<MissingAntecedentException>(() => BoundedFuzzySetRule.Create(Linguistics).And(Window.Open));
        Assert.Throws<MissingAntecedentException>(() => BoundedFuzzySetRule.Create(Linguistics).Or(Window.Open));
        Assert.Throws<MissingAntecedentException>(() => BoundedFuzzySetRule.Create(Linguistics).AndNot(Occupancy.Occupied));
        Assert.Throws<MissingAntecedentException>(() => BoundedFuzzySetRule.Create(Linguistics).OrNot(Occupancy.Occupied));
    }

    [Fact]
    public void CreationThrowsOnDuplicatedConditional()
    {
        Assert.Throws<DuplicatedAntecedentException>(() => BoundedFuzzySetRule.Create(Linguistics).If("room temperature", "Warm").If("outside temperature", "freezing"));
        Assert.Throws<DuplicatedAntecedentException>(() => BoundedFuzzySetRule.Create(Linguistics).If(Window.Open).If(Occupancy.Occupied));
    }
    
    [Fact]
    public void CreationThrowsOnVariableNotFound()
    {
        Assert.Throws<VariableNotFoundException>(() => BoundedFuzzySetRule.Create(Linguistics).If("Air conditioner temperature", "Low"));
        Assert.Throws<VariableNotFoundException>(() => BoundedFuzzySetRule.Create(Linguistics).If("Thermostat temperature", "Low"));
    }

    [Fact]
    public void CreationThrowsOnFunctionNotFound()
    {
        Assert.Throws<EntryNotFoundException>(() => BoundedFuzzySetRule.Create(Linguistics).If("Room Temperature", "Freezing"));
        Assert.Throws<EntryNotFoundException>(() => BoundedFuzzySetRule.Create(Linguistics).If("Outside Temperature", "Hell on earth"));
    }

    [Fact]
    public void CreationThrowsOnVariableOverlap()
    {
        Assert.Throws<VariableOverlapException>(() => BoundedFuzzySetRule.Create(Linguistics).If("Room Temperature", "Warm").Then("Room Temperature", "Cold"));
        Assert.Throws<VariableOverlapException>(() => BoundedFuzzySetRule.Create(Linguistics).If("Humidity", "Dry").Then("humidity", "humid"));
    }

    [Fact]
    public void CreationThrowsOnAdditionsInFinalizedRule()
    {
        Assert.Throws<FinalizedRuleException>(() => BoundedFuzzySetRule.Create(Linguistics)
            .If("Room Temperature", "Warm")
            .Then("Heating power", "Low")
            .And(Window.Open));
        Assert.Throws<FinalizedRuleException>(() => BoundedFuzzySetRule.Create(Linguistics).If("Room Temperature", "Warm")
            .Then("Heating power", "Low")
            .And("Outside Temperature", "Freezing"));
    }
    
}