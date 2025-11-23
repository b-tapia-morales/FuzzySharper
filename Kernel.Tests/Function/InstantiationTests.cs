using Kernel.Function.Implementations;

namespace Kernel.Tests.Function;

public class InstantiationTests
{
    [Fact]
    public void BellShapedFunctionInstantiationFailsWithBadValues()
    {
        // Null or whitespace string
        Assert.Throws<ArgumentException>(() => new GeneralizedBellFunction(name: string.Empty, a: 2, b: 4, c: 8));
        // h = 0
        Assert.Throws<ArgumentException>(() => new GeneralizedBellFunction(name: "Function", a: 2, b: 4, c: 8, uMax: 0));
        // a = 0
        Assert.Throws<ArgumentException>(() => new GeneralizedBellFunction(name: "Function", a: 0, b: 4, c: 8));
        // b < 1
        Assert.Throws<ArgumentException>(() => new GeneralizedBellFunction(name: "Function", a: 2, b: 3 / 4.0, c: 8));
    }

    [Fact]
    public void GaussianFunctionInstantiationFailsWithBadValues()
    {
        // Null or whitespace string
        Assert.Throws<ArgumentException>(() => new GaussianFunction(name: string.Empty, mu: 2, sigma: 4, uMax: 1));
        // h = 0
        Assert.Throws<ArgumentException>(() => new GaussianFunction(name: "Function", mu: 2, sigma: 4, uMax: 0));
        // o = 0
        Assert.Throws<ArgumentException>(() => new GaussianFunction(name: "Function", mu: 2, sigma: 0));
    }

    [Fact]
    public void TriangularFunctionInstantiationFailsWithBadValues()
    {
        // Null or whitespace string
        Assert.Throws<ArgumentException>(() => new TriangleFunction(name: string.Empty, a: 2, b: 4, c: 8));
        // h = 0
        Assert.Throws<ArgumentException>(() => new TriangleFunction(name: "Function", a: 2, b: 4, c: 8, uMax: 0));
        // a > b ∨ b > c
        Assert.Throws<ArgumentException>(() => new TriangleFunction(name: "Function", a: 2, b: 4, c: 2));
        // Singleton function
        Assert.Throws<ArgumentException>(() => new TriangleFunction(name: "Function", a: 2, b: 2, c: 2));
    }

    [Fact]
    public void TrapezoidalFunctionInstantiationFailsWithBadValues()
    {
        // Null or whitespace string
        Assert.Throws<ArgumentException>(() => new TrapezoidFunction(name: string.Empty, a: 2, b: 4, c: 8, d: 10));
        // h = 0
        Assert.Throws<ArgumentException>(() =>
            new TrapezoidFunction(name: string.Empty, a: 2, b: 4, c: 8, d: 10, uMax: 0));
        // a > b ∨ b > c ∨ c > d
        Assert.Throws<ArgumentException>(() => new TrapezoidFunction(name: "Function", a: 2, b: 4, c: 8, d: 4));
        // Rectangle shape
        Assert.Throws<ArgumentException>(() => new TrapezoidFunction(name: "Function", a: 2, b: 2, c: 8, d: 8));
    }
    
    [Fact]
    public void SigmoidFunctionInstantiationFailsWithBadValues()
    {
        // Null or whitespace string
        Assert.Throws<ArgumentException>(() => new LogisticFunction(name: string.Empty, a: 2, c: 8));
        // h = 0
        Assert.Throws<ArgumentException>(() => new LogisticFunction(name: "Function", a: 2, c: 8, uMax: 0));
        // a = 0
        Assert.Throws<ArgumentException>(() => new LogisticFunction(name: "Function", a: 0, c: 8));
    }
}