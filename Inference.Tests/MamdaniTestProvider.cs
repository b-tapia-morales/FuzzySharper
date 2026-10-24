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

/// <summary>
/// Configures dependency scopes for Mamdani integration tests without constructing an inference engine.
/// </summary>
public static class MamdaniTestProvider
{
    /// <summary>
    /// Registers a linguistic base, a fuzzy-set rule base, and working memory with scoped lifetimes.
    /// Each scope creates its own instances and uses the same linguistic base to construct its rules.
    /// The default scenario uses the tipping model with empty working memory.
    /// </summary>
    /// <param name="linguisticBaseFactory">Creates the scope's linguistic definitions, or uses the tipping definitions when omitted.</param>
    /// <param name="ruleBaseFactory">Creates rules against the scope's linguistic base, or uses the tipping rules when omitted.</param>
    /// <param name="workingMemoryFactory">Creates the scope's facts, or creates empty working memory when omitted.</param>
    /// <returns>A service provider from which each test creates and disposes its own scope.</returns>
    /// <remarks>
    /// Custom factories should return new instances on each invocation. Scoped services cannot be resolved
    /// directly from the root provider. Dispose the provider after its scopes have been disposed.
    /// </remarks>
    public static ServiceProvider ConfigureProvider(
        Func<ILinguisticBase>? linguisticBaseFactory = null,
        Func<ILinguisticBase, IFuzzySetRuleBase>? ruleBaseFactory = null,
        Func<IWorkingMemory>? workingMemoryFactory = null)
    {
        var createLinguisticBase = linguisticBaseFactory ?? CreateTippingLinguisticBase;
        var createRuleBase = ruleBaseFactory ?? CreateTippingRuleBase;
        var createWorkingMemory = workingMemoryFactory ?? (static () => WorkingMemory.Create());

        var services = new ServiceCollection();
        services.AddScoped<ILinguisticBase>(_ => createLinguisticBase());
        services.AddScoped<IFuzzySetRuleBase>(provider => createRuleBase(provider.GetRequiredService<ILinguisticBase>()));
        services.AddScoped<IWorkingMemory>(_ => createWorkingMemory());

        return services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateScopes = true,
            ValidateOnBuild = true
        });
    }

    private static ILinguisticBase CreateTippingLinguisticBase()
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

    private static IFuzzySetRuleBase CreateTippingRuleBase(ILinguisticBase linguisticBase)
    {
        var lowTip = BoundedFuzzySetRule.Create(linguisticBase)
            .If("food quality", "bad")
            .Or("service quality", "poor")
            .Then("tip", "low");
        var mediumTip = BoundedFuzzySetRule.Create(linguisticBase)
            .If("service quality", "acceptable")
            .Then("tip", "medium");
        var highTip = BoundedFuzzySetRule.Create(linguisticBase)
            .If("food quality", "great")
            .Or("service quality", "amazing")
            .Then("tip", "high");

        return FuzzySetRuleBase.Create(lowTip, mediumTip, highTip);
    }
}
