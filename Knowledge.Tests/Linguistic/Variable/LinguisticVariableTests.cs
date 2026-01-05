using Kernel.Function.Implementations;
using Knowledge.Linguistic.Variable.Exceptions;
using Knowledge.Linguistic.Variable.Implementations;
using Shared.Intervals.Implementations;

namespace Knowledge.Tests.Linguistic.Variable;

public class LinguisticVariableTests
{
    [Fact]
    public void InstantiationThrowsOnEmptyName()
    {
        Assert.Throws<ArgumentException>(() => LinguisticVariable.Create(""));
    }
    
    [Fact]
    public void InstantiationThrowsOnEntryCollision()
    {
        var f1 = TriangleFunction.Create("Bad", 0, 0, 5);
        var f2 = GaussianFunction.Create("bad", 5, 2.5);
        Assert.Throws<DuplicatedEntryException>(() => LinguisticVariable.Create("Food quality", f1, f2));
    }
    
    [Fact]
    public void InstantiationThrowsOnFunctionOutsideRange()
    {
        Assert.Throws<VariableRangeException>(() => LinguisticVariable
            .Create("Food quality", new Interval(0, 10))
            .AddTriangularFunction("Bad", -5, 0, 0));
    }

    [Fact]
    public void AddingThrowsOnEntryCollision()
    {
        var variable = LinguisticVariable.Create("Food quality");
        variable.AddTriangularFunction("Bad", 0, 0, 5);
        Assert.Throws<DuplicatedEntryException>(() => variable.AddTriangularFunction("Bad", 0, 2.5, 7.5));
    }
    
    [Fact]
    public void AddingThrowsOnFunctionOutsideRange()
    {
        var variable = LinguisticVariable.Create("Food quality", new Interval(0, 10));
        Assert.Throws<VariableRangeException>(() => variable.AddTriangularFunction("Bad", -5, 0, 0));
    }
    
    [Fact]
    public void AddingDoesNotThrowOnFunctionAlmostOutsideRange()
    {
        var variable = LinguisticVariable.Create("Food quality", new Interval(0, 10));
        var exception = Record.Exception(() => variable.AddTriangularFunction("Bad", -5, 0.01, 0.01));
        Assert.Null(exception);
    }

    [Fact]
    public void AddedFunctionsAreFoundSuccessfully()
    {
        var variable = LinguisticVariable.Create("Food quality", new Interval(0, 10))
            .AddTriangularFunction("Bad", 0, 0, 5)
            .AddTriangularFunction("Decent", 0, 2.5, 7.5)
            .AddTriangularFunction("Good", 0, 5, 10);
        Assert.True(variable.ContainsMapping("bad"));
        Assert.True(variable.ContainsMapping("decent"));
        Assert.True(variable.ContainsMapping("good"));
        Assert.True(variable.GetMapping("bad").IsSome);
        Assert.True(variable.GetMapping("decent").IsSome);
        Assert.True(variable.GetMapping("good").IsSome);
    }
}