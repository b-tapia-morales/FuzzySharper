using Knowledge.Linguistic.Base.Abstractions;
using Knowledge.Linguistic.Base.Exceptions;
using Knowledge.Linguistic.Variable.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace Knowledge.Tests.Linguistic.Base;

public class LinguisticBaseTests
{
    private static readonly IServiceProvider ServiceProvider = KnowledgeProvider.ConfigureProvider();
    private static readonly ILinguisticBase LinguisticBase = ServiceProvider.GetService<ILinguisticBase>()!;

    [Theory]
    [InlineData("food quality")]
    [InlineData("service quality")]
    [InlineData("tip")]
    public void VariablesAreFoundSuccessfully(string variable)
    {
        Assert.True(LinguisticBase.ContainsVariable(variable));
        Assert.True(LinguisticBase.GetVariable(variable).IsSome);
        var exception = Record.Exception(() => LinguisticBase.GetVariable(variable).Get);
        Assert.Null(exception);
    }

    [Theory]
    [InlineData("food quality")]
    [InlineData("service quality")]
    [InlineData("tip")]
    public void AddingThrowsOnVariableCollision(string name)
    {
        var variable = LinguisticVariable.Create(name);
        Assert.Throws<DuplicateVariableException>(() => LinguisticBase.Add(variable));
    }

    [Theory]
    [InlineData("food quality", new[] {"bad", "decent", "great"})]
    [InlineData("service quality", new[] {"poor", "acceptable", "amazing"})]
    [InlineData("tip", new[] {"low", "medium", "high"})]
    public void FunctionsAreFoundSuccessfully(string variable, string[] terms)
    {
        foreach (var term in terms)
        {
            Assert.True(LinguisticBase.ContainsFunction(variable, term));
            Assert.True(LinguisticBase.GetFunction(variable, term).IsSome);
            var exception = Record.Exception(() => LinguisticBase.GetFunction(variable, term).Get);
            Assert.Null(exception);
        }
    }
}