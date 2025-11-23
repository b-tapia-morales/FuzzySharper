using Kernel.Number;
using Shared.Approx;

namespace Kernel.Tests.Number;

public class InstantiationTests
{
    private static readonly Random Random = new();
    private const double Tolerance = DoubleApproxExt.DefaultTolerance;
    private static readonly double MinValue = Math.BitDecrement(-Tolerance);
    private static readonly double MaxValue = Math.BitIncrement(1 + Tolerance);

    [Fact]
    public void InstantiationThrowsExceptionOnValueOutOfRangeHelper()
    {
        Assert.Throws<ArgumentException>(() => FuzzyNumber.Of(MinValue));
        Assert.Throws<ArgumentException>(() => FuzzyNumber.Of(MaxValue));
        Assert.Throws<ArgumentException>(() => FuzzyNumber.Of(double.MaxValue));
    }

    [Fact]
    public void TryCreateFailsOnValueOutOfRangeHelper()
    {
        Assert.False(FuzzyNumber.TryCreate(MinValue, out _));
        Assert.False(FuzzyNumber.TryCreate(MaxValue, out _));
        Assert.False(FuzzyNumber.TryCreate(double.MaxValue, out _));
    }

    [Fact]
    public void NegationOperatorYieldsExpectedValuesHelper()
    {
        Assert.Equal(FuzzyNumber.Min, !FuzzyNumber.Max, Tolerance);
        Assert.StrictEqual(FuzzyNumber.Min, !FuzzyNumber.Max);
        Assert.True(FuzzyNumber.Min == !FuzzyNumber.Max);
        Assert.Equal(FuzzyNumber.Max, !FuzzyNumber.Min, Tolerance);
        Assert.StrictEqual(FuzzyNumber.Max, !FuzzyNumber.Min);
        Assert.True(FuzzyNumber.Max == !FuzzyNumber.Min);
    }

    [Theory]
    [MemberData(nameof(GenerateRandomValues), parameters: 100)]
    public void NegationOperatorIsEquivalent(double x) =>
        NegationOperatorIsEquivalentHelper(x);

    [Theory]
    [MemberData(nameof(GenerateRandomValuesIncludingTolerance), parameters: 100)]
    public void ValuesYieldEqualityForDifferenceUnderTolerance(double first, double second) =>
        ValuesYieldEqualityForDifferenceUnderToleranceHelper(first, second);

    [Theory]
    [InlineData(1e-1 + Tolerance, 1e-1 + Tolerance + 1e-15)]
    [InlineData(0 - Tolerance * 1e-1, 0)]
    [InlineData(0 + Tolerance * 1e-1, 0)]
    [InlineData(1 - Tolerance * 1e-1, 1)]
    [InlineData(1 + Tolerance * 1e-1, 1)]
    public void EdgeCasesYieldEqualityForDifferenceUnderTolerance(double first, double second) =>
        ValuesYieldEqualityForDifferenceUnderTolerance(first, second);

    private static void NegationOperatorIsEquivalentHelper(double x)
    {
        Assert.Equal(1 - x, !FuzzyNumber.Of(x), Tolerance);
        Assert.True(FuzzyNumber.Of(1 - x) == !FuzzyNumber.Of(x));
    }

    private static void ValuesYieldEqualityForDifferenceUnderToleranceHelper(double a, double b)
    {
        Assert.Equal(FuzzyNumber.Of(a), FuzzyNumber.Of(b), Tolerance);
        Assert.True(FuzzyNumber.Of(a) == FuzzyNumber.Of(b));
    }

    public static TheoryData<double> GenerateRandomValues(int n)
    {
        var data = new TheoryData<double>();
        for (var i = 1; i <= n; i++)
        {
            data.Add(Random.NextDouble());
        }

        return data;
    }

    public static IEnumerable<object[]> GenerateRandomValuesIncludingTolerance(int n)
    {
        for (var i = 1; i <= n; i++)
        {
            var x = Random.NextDouble();
            yield return [x, x - Random.Next(1, 9) * Tolerance * 1e-1];
        }
    }
}