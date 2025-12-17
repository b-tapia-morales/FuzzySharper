using Knowledge.Linguistic.Base.Abstractions;
using Knowledge.Linguistic.Base.Implementations;
using Knowledge.Linguistic.Variable.Implementations;
using Knowledge.Memory.Abstractions;
using Knowledge.Memory.Implementations;
using Microsoft.Extensions.DependencyInjection;
using Reasoning.Base.FuzzySet.Abstractions;
using Reasoning.Base.FuzzySet.Implementations;
using Reasoning.Rule.FuzzySet.Implementations;

namespace Inference.Tests;

public static class EngineProvider
{
    public static ServiceProvider ConfigureProvider()
    {
        var serviceCollection = new ServiceCollection();
        var linguisticBase = InitializeLinguisticBase();
        var ruleBase = InitializeRuleBase(linguisticBase);
        var workingMemory = InitializeWorkingMemory();
        serviceCollection.AddTransient<ILinguisticBase>(_ => linguisticBase);
        serviceCollection.AddTransient<IFuzzySetRuleBase>(_ => ruleBase);
        serviceCollection.AddTransient<IWorkingMemory>(_ => workingMemory);
        return serviceCollection.BuildServiceProvider();
    }
    
    private static ILinguisticBase InitializeLinguisticBase()
    {
        var foodQuality = LinguisticVariable
            .Create("Food quality")
            .AddTriangularFunction("Bad", 0, 0, 5)
            .AddTriangularFunction("Decent", 0, 5, 10)
            .AddTriangularFunction("Great", 5, 10, 10);
        var serviceQuality = LinguisticVariable
            .Create("Service quality")
            .AddTriangularFunction("Poor", 0, 0, 5)
            .AddTriangularFunction("Acceptable", 0, 5, 10)
            .AddTriangularFunction("Amazing", 5, 10, 10);
        var tip = LinguisticVariable
            .Create("Tip")
            .AddTriangularFunction("Low", 0, 0, 13)
            .AddTriangularFunction("Medium", 0, 13, 25)
            .AddTriangularFunction("High", 13, 25, 35);
        return LinguisticBase.Create(foodQuality, serviceQuality, tip);
    }

    private static IFuzzySetRuleBase InitializeRuleBase(ILinguisticBase linguisticBase)
    {
        var r1 = BoundFuzzySetRule.Create(linguisticBase)
            .If("food quality", "bad")
            .Or("service quality", "poor")
            .Then("tip", "low");
        var r2 = BoundFuzzySetRule.Create(linguisticBase)
            .If("service quality", "acceptable")
            .Then("tip", "medium");
        var r3 = BoundFuzzySetRule.Create(linguisticBase)
            .If("food quality", "great")
            .Or("service quality", "amazing")
            .Then("tip", "high");
        return FuzzySetRuleBase.Create(r1, r2, r3);
    }

    private static IWorkingMemory InitializeWorkingMemory() => 
        WorkingMemory.Create(("food quality", 6), ("service quality", 9.8));
}