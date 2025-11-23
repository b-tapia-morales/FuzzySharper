using Knowledge.FactStorage.Exceptions;
using Knowledge.Memory.Abstractions;
using Knowledge.Memory.Exceptions;
using Knowledge.Memory.Implementations;
using Microsoft.Extensions.DependencyInjection;
using Shared.Options.Factory;
using Shared.Primitives.Extensions;
using Xunit.Abstractions;

namespace Knowledge.Tests.Memory;

public class WorkingMemoryTests(ITestOutputHelper output)
{
    private static readonly string CategoricalFactsPath = Path.Combine(AppContext.BaseDirectory, "Csv", "CategoricalFacts.csv");
    private static readonly string NumericFactsPath = Path.Combine(AppContext.BaseDirectory, "Csv", "NumericFacts.csv");
    
    private static readonly IServiceProvider ServiceProvider = KnowledgeProvider.ConfigureProvider();
    private static readonly IWorkingMemory Memory = ServiceProvider.GetService<IWorkingMemory>()!;

    [Theory]
    [InlineData("food quality")]
    [InlineData("service quality")]
    public void NumericFactsAreFoundSuccessfully(string variable)
    {
        Assert.True(Memory.NumericStorage.Contains(variable));
        Assert.True(Memory.NumericStorage.GetValue(variable).IsSome);
        Memory.GetNumericFact(variable).IsSomeVal(out var value);
        Assert.NotEqual(0, value);
        Assert.True(Memory.ContainsNumericFact(variable));
        Assert.True(Memory.GetNumericFact(variable).IsSome);
        Memory.GetNumericFact(variable).IsSomeVal(out value);
        Assert.NotEqual(0, value);
    }

    [Fact]
    public void CategoricalFactIsFoundSuccessfully()
    {
        var type = typeof(MichelinStars);
        Assert.True(Memory.CategoricalStorage.Contains(type));
        Assert.True(Memory.CategoricalStorage.GetValue(type).IsSome);
        Assert.True(Memory.ContainsCategoricalFact<MichelinStars>());
        Assert.True(Memory.GetCategoricalFact<MichelinStars>().IsSome);
        Memory.GetCategoricalFact<MichelinStars>().IsSomeVal(out var e1);
        Memory.CategoricalStorage.GetValue(type).IsSomeRef(out var e2);
        Assert.InRange((int) e1, 0, 3);
        Assert.Equal(e2.AsTypedEnum<MichelinStars>(), e1);
    }
    
    [Theory]
    [InlineData("age")]
    [InlineData("height")]
    [InlineData("weight")]
    public void NumericFactsAreLoadedSuccessfully(string key)
    {
        var memory = WorkingMemory.Create();
        memory.ReadNumericFactsFromFile(NumericFactsPath);
        Assert.True(memory.ContainsNumericFact(key));
        Assert.True(memory.NumericStorage.Contains(key));
        output.WriteLine(memory.ToString());
    }

    [Fact]
    public void CategoricalFactsAreLoadedSuccessfully()
    {
        var memory = WorkingMemory.Create();
        memory.ReadCategoricalFactsFromFile(CategoricalFactsPath);
        Assert.True(memory.ContainsCategoricalFact<Some>());
        Assert.True(memory.CategoricalStorage.Contains(typeof(Some)));
        Assert.True(memory.ContainsCategoricalFact<Other>());
        Assert.True(memory.CategoricalStorage.Contains(typeof(Other)));
        output.WriteLine(memory.ToString());
    }

    [Fact]
    public void AddingNonCategoricalFactThrowsException()
    {
        Assert.Throws<InvalidKeyException>(() => Memory.CategoricalStorage.AddValue("food quality", MichelinStars.None));
        Assert.Throws<InvalidValueException>(() => Memory.CategoricalStorage.AddValue(typeof(MichelinStars), 0));
        Assert.Throws<InvalidPairException>(() => Memory.AddValue("food quality", MichelinStars.None));
    }

    [Fact]
    public void AddingNonNumericFactThrowsException()
    {
        Assert.Throws<InvalidKeyException>(() => Memory.NumericStorage.AddValue(typeof(MichelinStars), 0));
        Assert.Throws<InvalidValueException>(() => Memory.NumericStorage.AddValue("food quality", MichelinStars.None));
        Assert.Throws<InvalidPairException>(() => Memory.AddValue(typeof(MichelinStars), 0));
    }
}

internal enum Some
{
    A,
    B,
    C
}

internal enum Other
{
    A,
    B,
    C
}