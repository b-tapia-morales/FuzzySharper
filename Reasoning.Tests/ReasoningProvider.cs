using Knowledge.Linguistic.Base.Abstractions;
using Knowledge.Linguistic.Base.Implementations;
using Knowledge.Linguistic.Variable.Implementations;
using Knowledge.Memory.Abstractions;
using Knowledge.Memory.Implementations;
using Microsoft.Extensions.DependencyInjection;
using Reasoning.Base.FuzzySet.Abstractions;
using Reasoning.Base.FuzzySet.Implementations;
using Reasoning.Rule.FuzzySet.Implementations;

namespace Reasoning.Tests;

public static class ReasoningProvider
{
    public static ServiceProvider ConfigureProvider()
    {
        var serviceCollection = new ServiceCollection();
        var linguisticBase = InitializeLinguistics();
        var workingMemory = InitializeMemory();
        var ruleBase = InitializeRules(linguisticBase);
        serviceCollection.AddTransient<ILinguisticBase>(_ => linguisticBase);
        serviceCollection.AddTransient<IFuzzySetRuleBase>(_ => ruleBase);
        serviceCollection.AddTransient<IWorkingMemory>(_ => workingMemory);
        return serviceCollection.BuildServiceProvider();
    }

    private static ILinguisticBase InitializeLinguistics()
    {
        var roomTemperature = LinguisticVariable
            .Create("Room Temperature")
            .AddTriangularFunction("Cold", 0, 8, 15)
            .AddTriangularFunction("Cool", 10, 17, 22)
            .AddTriangularFunction("Comfortable", 18, 22, 26)
            .AddTriangularFunction("Warm", 24, 28, 33)
            .AddTriangularFunction("Hot", 30, 35, 40);
        var humidity = LinguisticVariable
            .Create("Humidity")
            .AddTrapezoidFunction("Dry", 0, 0, 20, 35)
            .AddTriangularFunction("Normal", 30, 45, 60)
            .AddTrapezoidFunction("Humid", 55, 70, 100, 100);
        var outsideTemperature = LinguisticVariable
            .Create("Outside Temperature")
            .AddTriangularFunction("Freezing", -10, 0, 8)
            .AddTriangularFunction("Chilly", 5, 12, 18)
            .AddTriangularFunction("Mild", 15, 20, 25)
            .AddTriangularFunction("Warm", 22, 27, 35);
        var heatingPower = LinguisticVariable
            .Create("Heating Power")
            .AddTrapezoidFunction("Low", 0, 0, 20, 40)
            .AddTriangularFunction("Medium", 30, 50, 70)
            .AddTrapezoidFunction("High", 60, 80, 100, 100);
        return LinguisticBase.Create(roomTemperature, humidity, outsideTemperature, heatingPower);
    }

    private static IWorkingMemory InitializeMemory()
    {
        var memory = WorkingMemory.Create();
        memory.AddNumericFacts(("Room Temperature", 24), ("Humidity", 45), ("Outside Temperature", 22));
        memory.AddCategoricalFact(Window.Closed);
        memory.AddCategoricalFact(Occupancy.Occupied);
        return memory;
    }

    private static IFuzzySetRuleBase InitializeRules(ILinguisticBase linguisticBase)
    {
        var a = BoundFuzzySetRule.Create(linguisticBase)
            .If("Room temperature", "Cold")
            .And(Occupancy.Occupied)
            .Then("Heating power", "High");
        var b = BoundFuzzySetRule.Create(linguisticBase)
            .If("Room Temperature", "Cool")
            .And("Outside Temperature", "Freezing")
            .Then("Heating power", "High");
        var c = BoundFuzzySetRule.Create(linguisticBase)
            .If("Room temperature", "Comfortable")
            .And("Humidity", "Normal")
            .Then("Heating power", "Low");
        var d = BoundFuzzySetRule.Create(linguisticBase)
            .If("Room temperature", "Comfortable")
            .AndNot(Window.Open)
            .Then("Heating power", "Low");
        var e = BoundFuzzySetRule.Create(linguisticBase)
            .If("Room temperature", "Warm")
            .Or("Room temperature", "Hot")
            .Then("Heating power", "Low");
        var f = BoundFuzzySetRule.Create(linguisticBase)
            .If("Room temperature", "Cold")
            .And(Window.Open)
            .Then("Heating power", "Medium");
        var g = BoundFuzzySetRule.Create(linguisticBase)
            .If("Room temperature", "Cold")
            .And("Humidity", "Humid")
            .Then("Heating power", "Medium");
        var h = BoundFuzzySetRule.Create(linguisticBase)
            .If("Outside temperature", "Chilly")
            .And("Room temperature", "Cool")
            .Then("Heating power", "Medium");
        var i = BoundFuzzySetRule.Create(linguisticBase)
            .If("Room temperature", "Cold")
            .And(Occupancy.Occupied)
            .Then("Heating power", "Medium");
        var j = BoundFuzzySetRule.Create(linguisticBase)
            .If("Outside temperature", "Mild")
            .And("Room temperature", "Comfortable")
            .Then("Heating power", "Low");
        var k = BoundFuzzySetRule.Create(linguisticBase)
            .If("Humidity", "Dry")
            .And("Room temperature", "Cool")
            .Then("Heating power", "Medium");
        var l = BoundFuzzySetRule.Create(linguisticBase)
            .If("Room temperature", "Hot")
            .And(Window.Open)
            .Then("Heating power", "Low");
        return FuzzySetRuleBase.Create(a, b, c, d, e, f, g, h, i, j, k, l);
    }
}

internal enum Window
{
    Open,
    Closed
}

internal enum Occupancy
{
    Occupied,
    Empty
}