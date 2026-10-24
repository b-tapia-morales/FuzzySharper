using Inference.Aggregator.Factory;
using Inference.Builder.Fuzzy.Implementations;
using Inference.Defuzzifier.Factory;
using Inference.Engine.Fuzzy.Implementations;
using Kernel.Function.Implication.Factory;
using Kernel.Operator.Family.Factory.Canonical;
using Knowledge.Memory.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Reasoning.Base.FuzzySet.Abstractions;
using Shared.Approx;
using Shared.Options.Factory;

namespace Inference.Tests.Inference;

[Trait("Category", "Integration")]
public sealed class MamdaniEngineTests : IDisposable
{
    private const uint Precision = DoubleApproxExt.DefaultPrecision;
    private static readonly double TipTolerance = Math.Pow(10, Precision * -2D);

    private readonly ServiceProvider _provider;
    private readonly IServiceScope _scope;
    private readonly IFuzzySetRuleBase _rules;
    private readonly IWorkingMemory _memory;

    public MamdaniEngineTests()
    {
        _provider = MamdaniTestProvider.ConfigureProvider();
        _scope = _provider.CreateScope();
        _rules = _scope.ServiceProvider.GetRequiredService<IFuzzySetRuleBase>();
        _memory = _scope.ServiceProvider.GetRequiredService<IWorkingMemory>();
    }

    [Theory]
    [ClassData(typeof(TippingCasesData))]
    public void Defuzzify_TippingInputs_ReturnsExpectedTip(
        ImplicationMethod implicationMethod, double foodRating, double serviceRating, double expectedTip)
    {
        _memory.AddNumericFact("Food quality", foodRating);
        _memory.AddNumericFact("Service quality", serviceRating);
        var engine = CreateEngine(implicationMethod: implicationMethod);

        var result = engine.Defuzzify("Tip", provideExplanation: false);

        Assert.True(result.IsSome(out var actualTip));
        Assert.True(double.IsFinite(actualTip));
        Assert.InRange(actualTip, expectedTip - TipTolerance, expectedTip + TipTolerance);
    }

    [Theory]
    [Trait("Category", "Configuration")]
    [ClassData(typeof(EngineConfigurationData))]
    public void Defuzzify_SupportedConfiguration_ReturnsTipInRange(
        CanonicalType canonicalType, 
        ImplicationMethod implicationMethod,
        DefuzzificationMethod defuzzificationMethod, 
        ValueAggregatorMethod aggregatorMethod)
    {
        _memory.AddNumericFact("Food quality", 6);
        _memory.AddNumericFact("Service quality", 9.8);
        var engine = CreateEngine(canonicalType, implicationMethod, defuzzificationMethod, aggregatorMethod);

        var result = engine.Defuzzify("Tip", provideExplanation: false);

        Assert.True(result.IsSome(out var actualTip));
        Assert.True(double.IsFinite(actualTip));
        Assert.InRange(actualTip, 0d, 35d);
    }

    [Theory]
    [Trait("Category", "Configuration")]
    [ClassData(typeof(TippingGridData))]
    public void Defuzzify_SupportedConfigurationAndRatings_ReturnsTipInRange(
        CanonicalType canonicalType, 
        ImplicationMethod implicationMethod,
        DefuzzificationMethod defuzzificationMethod, 
        ValueAggregatorMethod aggregatorMethod,
        double foodRating, 
        double serviceRating)
    {
        _memory.AddNumericFact("Food quality", foodRating);
        _memory.AddNumericFact("Service quality", serviceRating);
        var engine = CreateEngine(canonicalType, implicationMethod, defuzzificationMethod, aggregatorMethod);

        var result = engine.Defuzzify("Tip", provideExplanation: false);

        Assert.True(result.IsSome(out var actualTip));
        Assert.True(double.IsFinite(actualTip));
        Assert.InRange(actualTip, 0d, 35d);
    }

    [Theory]
    [InlineData(ImplicationMethod.Mamdani, 5d, 5d, 13d)]
    [InlineData(ImplicationMethod.Mamdani, 10d, 10d, 25d)]
    [InlineData(ImplicationMethod.Larsen, 5d, 5d, 13d)]
    [InlineData(ImplicationMethod.Larsen, 10d, 10d, 25d)]
    public void Defuzzify_MeanOfMaximaWithSinglePeak_ReturnsPeakPosition(
        ImplicationMethod implicationMethod, double foodRating, double serviceRating, double expectedTip)
    {
        _memory.AddNumericFact("Food quality", foodRating);
        _memory.AddNumericFact("Service quality", serviceRating);
        var engine = CreateEngine(
            implicationMethod: implicationMethod,
            defuzzificationMethod: DefuzzificationMethod.MeanOfMaxima);

        var result = engine.Defuzzify("Tip", provideExplanation: false);

        Assert.True(result.IsSome(out var actualTip));
        Assert.True(double.IsFinite(actualTip));
        Assert.InRange(actualTip, expectedTip - TipTolerance, expectedTip + TipTolerance);
    }

    [Fact]
    public void Defuzzify_TargetAlreadyKnown_ReturnsExistingFact()
    {
        _memory.AddNumericFact("Tip", 20d);
        var engine = CreateEngine();

        var result = engine.Defuzzify("Tip", provideExplanation: false);

        Assert.True(result.IsSome(out var actualTip));
        Assert.Equal(20d, actualTip);
    }

    [Theory]
    [InlineData(-1d, -1d)]
    [InlineData(11d, 11d)]
    public void Defuzzify_NoActivatedRules_ReturnsNone(double foodRating, double serviceRating)
    {
        _memory.AddNumericFact("Food quality", foodRating);
        _memory.AddNumericFact("Service quality", serviceRating);
        var engine = CreateEngine();

        var result = engine.Defuzzify("Tip", provideExplanation: false);

        Assert.True(result.IsNone);
    }

    private FuzzyConsequentEngine CreateEngine(
        CanonicalType canonicalType = CanonicalType.Godel,
        ImplicationMethod implicationMethod = ImplicationMethod.Mamdani,
        DefuzzificationMethod defuzzificationMethod = DefuzzificationMethod.CenterOfSums,
        ValueAggregatorMethod aggregatorMethod = ValueAggregatorMethod.Mean) =>
        FuzzyEngineBuilder.Create()
            .WithRuleBase(_rules)
            .WithWorkingMemory(_memory)
            .WithCanonicalFamily(canonicalType)
            .WithImplication(implicationMethod)
            .WithDefuzzification(defuzzificationMethod)
            .WithValueAggregator(aggregatorMethod)
            .Build();

    public void Dispose()
    {
        _scope.Dispose();
        _provider.Dispose();
    }
}
