using System.Collections;
using Kernel.Number;
using Kernel.Operator.Conorm.Abstractions;
using Kernel.Operator.Conorm.Implementations.Canonical;
using Kernel.Operator.Negation.Abstractions;
using Kernel.Operator.Negation.Implementations.Canonical;
using Kernel.Operator.Norm.Abstractions;
using Kernel.Operator.Norm.Implementations.Canonical;
using Utils.Numbers;

namespace Kernel.Tests.Operator;

public static class GlobalConst
{
    public const int N = 100;
}

public class GodelOperatorsTest
{
    private static readonly Norm Minimum = Norm.Minimum;
    private static readonly Conorm Maximum = Conorm.Maximum;

    [Theory]
    [ClassData(typeof(NegationDataOperator))]
    public void ComplementSatisfiesFundamentalProperty(INegation negation)
    {
        Assert.StrictEqual(negation.Complement(0), FuzzyNumber.Max);
        Assert.StrictEqual(negation.Complement(1), FuzzyNumber.Min);
    }

    [Theory]
    [ClassData(typeof(IntersectionDataOperator))]
    public void AssociativityHoldsForIntersection(INorm norm, FuzzyNumber x, FuzzyNumber y, FuzzyNumber z)
    {
        Assert.Equal(norm.Intersection(x, norm.Intersection(y, z)), norm.Intersection(norm.Intersection(x, y), z));
    }

    [Theory]
    [ClassData(typeof(UnionDataOperator))]
    public void AssociativityHoldsForUnion(IConorm conorm, FuzzyNumber x, FuzzyNumber y, FuzzyNumber z)
    {
        Assert.Equal(conorm.Union(x, conorm.Union(y, z)), conorm.Union(conorm.Union(x, y), z));
    }

    [Theory]
    [ClassData(typeof(IntersectionDataOperator))]
    public void CommutativityHoldsForIntersection(INorm norm, FuzzyNumber x, FuzzyNumber y, FuzzyNumber _)
    {
        Assert.Equal(norm.Intersection(x, y), norm.Intersection(y, x));
        Assert.StrictEqual(norm.Intersection(x, y), norm.Intersection(y, x));
    }

    [Theory]
    [ClassData(typeof(UnionDataOperator))]
    public void CommutativityHoldsForUnion(IConorm conorm, FuzzyNumber x, FuzzyNumber y, FuzzyNumber _)
    {
        Assert.Equal(conorm.Union(x, y), conorm.Union(y, x));
        Assert.StrictEqual(conorm.Union(x, y), conorm.Union(y, x));
    }

    [Theory]
    [ClassData(typeof(IntersectionDataOperator))]
    public void MonotonicityHoldsForIntersection(INorm norm, FuzzyNumber x, FuzzyNumber y, FuzzyNumber z)
    {
        var (min, median, max) = (NumberUtils.Min<double>(x, y, z), NumberUtils.Median<double>(x, y, z), NumberUtils.Max<double>(x, y, z));
        Assert.True(min < median && max >= median);
        Assert.True(norm.Intersection(min, median) <= norm.Intersection(min, max));
    }

    [Theory]
    [ClassData(typeof(UnionDataOperator))]
    public void MonotonicityHoldsForUnion(IConorm conorm, FuzzyNumber x, FuzzyNumber y, FuzzyNumber z)
    {
        var (min, median, max) = (NumberUtils.Min<double>(x, y, z), NumberUtils.Median<double>(x, y, z), NumberUtils.Max<double>(x, y, z));
        Assert.True(min <= median && max > median);
        Assert.True(conorm.Union(min, median) <= conorm.Union(min, max));
    }

    [Theory]
    [ClassData(typeof(GodelDataOperators))]
    public void DistributivityHolds(FuzzyNumber x, FuzzyNumber y, FuzzyNumber z)
    {
        Assert.Equal(Maximum.Union(x, Minimum.Intersection(y, z)), Minimum.Intersection(Maximum.Union(x, y), Maximum.Union(x, z)));
        Assert.Equal(Minimum.Intersection(x, Maximum.Union(y, z)), Maximum.Union(Minimum.Intersection(x, y), Minimum.Intersection(x, z)));
    }

    [Theory]
    [ClassData(typeof(GodelDataOperators))]
    public void AbsorptionHolds(FuzzyNumber x, FuzzyNumber y, FuzzyNumber _)
    {
        Assert.Equal(Minimum.Intersection(x, Maximum.Union(x, y)), x);
        Assert.Equal(Maximum.Union(x, Minimum.Intersection(x, y)), x);
    }

    [Theory]
    [ClassData(typeof(GodelDataOperators))]
    public void NonCompensationHolds(FuzzyNumber x, FuzzyNumber y, FuzzyNumber z)
    {
        var (min, max, median) = (NumberUtils.Min<double>(x, y, z), NumberUtils.Median<double>(x, y, z), NumberUtils.Max<double>(x, y, z));
        var (a, b, c) = (FuzzyNumber.Of(min), FuzzyNumber.Of(median), FuzzyNumber.Of(max));
        Assert.NotEqual(Minimum.Intersection(a, c), Minimum.Intersection(b, b));
        Assert.NotEqual(Maximum.Union(a, c), Maximum.Union(b, b));
    }
}

file class NegationDataOperator : IEnumerable<object[]>
{
    private static readonly IEnumerable<object[]> Negators = Negation.GetValues().Select(e => new object[] {e});

    public IEnumerator<object[]> GetEnumerator() => Negators.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

file class IntersectionDataOperator : IEnumerable<object[]>
{
    private static readonly Random Random = new();

    private static readonly IEnumerable<object[]> Intersectors =
        Enumerable
            .Repeat(Norm.GetValues(), GlobalConst.N)
            .SelectMany(e => e)
            .Select(e => new object[] {e, FuzzyNumber.Of(Random.NextDouble()), FuzzyNumber.Of(Random.NextDouble()), FuzzyNumber.Of(Random.NextDouble())});

    public IEnumerator<object[]> GetEnumerator() => Intersectors.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

file class UnionDataOperator : IEnumerable<object[]>
{
    private static readonly Random Random = new();

    private static readonly IEnumerable<object[]> Unitors =
        Enumerable
            .Repeat(Conorm.GetValues(), GlobalConst.N)
            .SelectMany(e => e)
            .Select(e => new object[] {e, FuzzyNumber.Of(Random.NextDouble()), FuzzyNumber.Of(Random.NextDouble()), FuzzyNumber.Of(Random.NextDouble())});

    public IEnumerator<object[]> GetEnumerator() => Unitors.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

file class GodelDataOperators : IEnumerable<object[]>
{
    private static readonly Random Random = new();

    private static readonly IEnumerable<object[]> GodelOperators =
        Enumerable
            .Range(0, GlobalConst.N)
            .Select(_ => new object[] {FuzzyNumber.Of(Random.NextDouble()), FuzzyNumber.Of(Random.NextDouble()), FuzzyNumber.Of(Random.NextDouble())});

    public IEnumerator<object[]> GetEnumerator() => GodelOperators.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}