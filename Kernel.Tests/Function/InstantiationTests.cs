using Kernel.Function.Implementations;

namespace Kernel.Tests.Function;

public class InstantiationTests
{
    [Fact]
    public void BellShapedFunctionInstantiationFailsWithBadValues()
    {
        // Null or whitespace string
        Assert.Throws<ArgumentException>(() => GeneralizedBellFunction.Create(name: string.Empty, a: 2, b: 4, c: 8));
        // h = 0
        Assert.Throws<ArgumentException>(() => GeneralizedBellFunction.Create(name: "Function.Create", a: 2, b: 4, c: 8, uMax: 0));
        // a = 0
        Assert.Throws<ArgumentException>(() => GeneralizedBellFunction.Create(name: "Function.Create", a: 0, b: 4, c: 8));
        // b < 1
        Assert.Throws<ArgumentException>(() => GeneralizedBellFunction.Create(name: "Function.Create", a: 2, b: 3 / 4.0, c: 8));
    }

    [Fact]
    public void GaussianFunctionCreateInstantiationFailsWithBadValues()
    {
        // Null or whitespace string
        Assert.Throws<ArgumentException>(() => GaussianFunction.Create(name: string.Empty, mu: 2, sigma: 4, uMax: 1));
        // h = 0
        Assert.Throws<ArgumentException>(() => GaussianFunction.Create(name: "Function.Create", mu: 2, sigma: 4, uMax: 0));
        // o = 0
        Assert.Throws<ArgumentException>(() => GaussianFunction.Create(name: "Function.Create", mu: 2, sigma: 0));
    }

    [Fact]
    public void TriangularFunctionCreateInstantiationFailsWithBadValues()
    {
        // Null or whitespace string
        Assert.Throws<ArgumentException>(() => TriangleFunction.Create(name: string.Empty, a: 2, b: 4, c: 8));
        // h = 0
        Assert.Throws<ArgumentException>(() => TriangleFunction.Create(name: "Function.Create", a: 2, b: 4, c: 8, uMax: 0));
        // a > b ∨ b > c
        Assert.Throws<ArgumentException>(() => TriangleFunction.Create(name: "Function.Create", a: 2, b: 4, c: 2));
        // Singleton function
        Assert.Throws<ArgumentException>(() => TriangleFunction.Create(name: "Function.Create", a: 2, b: 2, c: 2));
    }

    [Fact]
    public void TrapezoidalFunctionCreateInstantiationFailsWithBadValues()
    {
        // Null or whitespace string
        Assert.Throws<ArgumentException>(() => TrapezoidFunction.Create(name: string.Empty, a: 2, b: 4, c: 8, d: 10));
        // h = 0
        Assert.Throws<ArgumentException>(() =>
            TrapezoidFunction.Create(name: string.Empty, a: 2, b: 4, c: 8, d: 10, uMax: 0));
        // a > b ∨ b > c ∨ c > d
        Assert.Throws<ArgumentException>(() => TrapezoidFunction.Create(name: "Function.Create", a: 2, b: 4, c: 8, d: 4));
        // Rectangle shape
        Assert.Throws<ArgumentException>(() => TrapezoidFunction.Create(name: "Function.Create", a: 2, b: 2, c: 8, d: 8));
    }
    
    [Fact]
    public void SigmoidFunctionCreateInstantiationFailsWithBadValues()
    {
        // Null or whitespace string
        Assert.Throws<ArgumentException>(() => LogisticFunction.Create(name: string.Empty, a: 2, c: 8));
        // h = 0
        Assert.Throws<ArgumentException>(() => LogisticFunction.Create(name: "Function.Create", a: 2, c: 8, uMax: 0));
        // a = 0
        Assert.Throws<ArgumentException>(() => LogisticFunction.Create(name: "Function", a: 0, c: 8));
    }
}