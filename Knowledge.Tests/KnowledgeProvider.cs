using Knowledge.Linguistic.Base.Abstractions;
using Knowledge.Linguistic.Base.Implementations;
using Knowledge.Linguistic.Variable.Implementations;
using Knowledge.Memory.Abstractions;
using Knowledge.Memory.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace Knowledge.Tests;

public static class KnowledgeProvider
{
    public static ServiceProvider ConfigureProvider()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<ILinguisticBase>(_ => InitializeBase());
        serviceCollection.AddTransient<IWorkingMemory>(_ => InitializeMemory());
        return serviceCollection.BuildServiceProvider();
    }

    private static ILinguisticBase InitializeBase()
    {
        var food = LinguisticVariable
            .Create("Food quality")
            .AddTriangularFunction("Bad", 0, 0, 5)
            .AddTriangularFunction("Decent", 0, 5, 10)
            .AddTriangularFunction("Great", 5, 10, 10);
        var service = LinguisticVariable
            .Create("Service quality")
            .AddTriangularFunction("Poor", 0, 0, 5)
            .AddTriangularFunction("Acceptable", 0, 5, 10)
            .AddTriangularFunction("Amazing", 5, 10, 10);
        var tip = LinguisticVariable
            .Create("Tip")
            .AddTriangularFunction("Low", 0, 0, 13)
            .AddTriangularFunction("Medium", 0, 13, 25)
            .AddTriangularFunction("High", 13, 25, 35);
        return LinguisticBase.Create(food, service, tip);
    }

    private static IWorkingMemory InitializeMemory()
    {
        var workingMemory = WorkingMemory.Create();
        workingMemory.AddNumericFacts(("food quality", 6), ("service quality", 9.8));
        workingMemory.AddCategoricalFact(MichelinStars.None);
        return workingMemory;
    }
}

internal enum MichelinStars
{
    None,
    One,
    Two,
    Three
}