using Shared.Intervals.Implementations;
using Shared.Options.Factory;

namespace Shared.Tests.Intervals;

public class IntervalTests
{
    [Theory]
    [InlineData(-2, 4)]
    [InlineData(3, 3)]
    [InlineData(double.NegativeInfinity, 4)]
    [InlineData(-2, double.PositiveInfinity)]
    public void ConstructorPreservesBounds(double lower, double upper)
    {
        (double Lower, double Upper) expected = (lower, upper);

        var interval = new Interval(lower, upper);

        Assert.Equal(expected.Lower, interval.LowerBound);
        Assert.Equal(expected.Upper, interval.UpperBound);
        Assert.Equal(expected, interval.ToTuple());
    }

    [Fact]
    public void TupleConstructionPreservesBounds()
    {
        (double, double) expected = (-2, 4);

        var constructed = new Interval(expected);
        Interval converted = expected;

        Assert.Equal(expected, constructed.ToTuple());
        Assert.Equal(expected, converted.ToTuple());
    }

    [Fact]
    public void ConstructorRejectsReversedBounds()
    {
        Assert.Throws<ArgumentException>(() => new Interval(3, 2));
        Assert.Throws<ArgumentException>(() => new Interval((3, 2)));
    }

    [Theory]
    [InlineData(0, 1, true, true, true, false)]
    [InlineData(double.NegativeInfinity, 1, false, true, false, false)]
    [InlineData(0, double.PositiveInfinity, true, false, false, false)]
    [InlineData(double.NegativeInfinity, double.PositiveInfinity, false, false, false, true)]
    public void BoundednessFlagsDescribeBothEnds(
        double lower, double upper, bool boundedLeft, bool boundedRight, bool fullyBounded, bool fullyUnbounded)
    {
        var expected = (
            BoundedLeft: boundedLeft,
            BoundedRight: boundedRight,
            FullyBounded: fullyBounded,
            FullyUnbounded: fullyUnbounded,
            Unbounded: !fullyBounded);

        var interval = new Interval(lower, upper);

        Assert.Equal(expected.BoundedLeft, interval.IsBoundedLeft);
        Assert.Equal(expected.BoundedRight, interval.IsBoundedRight);
        Assert.Equal(expected.FullyBounded, interval.IsFullyBounded);
        Assert.Equal(expected.FullyUnbounded, interval.IsFullyUnbounded);
        Assert.Equal(expected.Unbounded, interval.IsUnbounded);
    }

    [Fact]
    public void DefaultIntervalIsFullyUnbounded()
    {
        (double, double) expected = (double.NegativeInfinity, double.PositiveInfinity);

        var actual = Interval.Default;

        Assert.Equal(expected, actual.ToTuple());
        Assert.True(actual.IsFullyUnbounded);
    }

    [Fact]
    public void DefaultStructValueIsZeroSingleton()
    {
        (double, double) expected = (0, 0);

        var actual = default(Interval);

        Assert.Equal(expected, actual.ToTuple());
        Assert.True(actual.IsSingleton);
    }

    [Theory]
    [InlineData(3, 3, true)]
    [InlineData(2, 8, false)]
    [InlineData(double.NegativeInfinity, 3, false)]
    [InlineData(3, double.PositiveInfinity, false)]
    [InlineData(double.NegativeInfinity, double.PositiveInfinity, false)]
    public void SingletonRecognizesEqualFiniteBounds(double lower, double upper, bool isSingleton)
    {
        var interval = new Interval(lower, upper);

        Assert.Equal(isSingleton, interval.IsSingleton);
    }

    [Theory]
    [InlineData(-2, 4, 6, 1)]
    [InlineData(2, 8, 6, 5)]
    [InlineData(3, 3, 0, 3)]
    public void FullyBoundedIntervalsHaveWidthAndMidpoint(double lower, double upper, double width, double midpoint)
    {
        var expected = (Width: width, Midpoint: midpoint);

        var interval = new Interval(lower, upper);

        Assert.True(interval.Width.IsSome(out var actualWidth));
        Assert.Equal(expected.Width, actualWidth);
        Assert.True(interval.Midpoint.IsSome(out var actualMidpoint));
        Assert.Equal(expected.Midpoint, actualMidpoint);
    }

    [Theory]
    [InlineData(double.NegativeInfinity, 3)]
    [InlineData(3, double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity, double.PositiveInfinity)]
    public void UnboundedIntervalsHaveNoWidthOrMidpoint(double lower, double upper)
    {
        var interval = new Interval(lower, upper);

        Assert.True(interval.Width.IsNone);
        Assert.True(interval.Midpoint.IsNone);
    }

    [Theory]
    [InlineData(1, false)]
    [InlineData(2, true)]
    [InlineData(5, true)]
    [InlineData(8, true)]
    [InlineData(9, false)]
    [InlineData(double.NaN, false)]
    public void ContainsChecksBothBounds(double value, bool isInBounds)
    {
        var interval = new Interval(2, 8);

        Assert.Equal(isInBounds, interval.Contains(value));
    }

    [Theory]
    [InlineData(double.NegativeInfinity, 8, -100, true)]
    [InlineData(double.NegativeInfinity, 8, 9, false)]
    [InlineData(2, double.PositiveInfinity, 100, true)]
    [InlineData(2, double.PositiveInfinity, 1, false)]
    [InlineData(double.NegativeInfinity, double.PositiveInfinity, 0, true)]
    public void ContainsChecksFiniteBoundsForUnboundedIntervals(double lower, double upper, double value, bool isInBounds)
    {
        var interval = new Interval(lower, upper);

        Assert.Equal(isInBounds, interval.Contains(value));
    }

    [Theory]
    [InlineData(2, 2)]
    [InlineData(5, 5)]
    [InlineData(8, 8)]
    public void ClampPreservesContainedValues(double value, double expected)
    {
        var interval = new Interval(2, 8);

        var actual = interval.Clamp(value);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(double.NegativeInfinity, 8, -100, -100)]
    [InlineData(double.NegativeInfinity, 8, 9, 8)]
    [InlineData(2, double.PositiveInfinity, 100, 100)]
    [InlineData(2, double.PositiveInfinity, 1, 2)]
    [InlineData(double.NegativeInfinity, double.PositiveInfinity, 7, 7)]
    public void ClampUsesFiniteBoundsForUnboundedIntervals(double lower, double upper, double value, double expected)
    {
        var interval = new Interval(lower, upper);

        var actual = interval.Clamp(value);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(2, 8, true)]
    [InlineData(1, 8, false)]
    [InlineData(2, 9, false)]
    public void EqualityComparesBothBounds(double otherLower, double otherUpper, bool equals)
    {
        var interval = new Interval(2, 8);
        var other = new Interval(otherLower, otherUpper);
        var comparer = EqualityComparer<Interval>.Default;

        Assert.Equal(equals, interval.Equals(other));
        Assert.Equal(equals, interval.Equals((object) other));
        Assert.Equal(equals, comparer.Equals(interval, other));
        Assert.Equal(equals, interval == other);
    }

    [Fact]
    public void EqualityRejectsNullAndUnrelatedObjects()
    {
        (double, double) unrelated = (2, 8);
        var interval = new Interval(2, 8);

        Assert.False(interval.Equals(null));
        // ReSharper disable once SuspiciousTypeConversion.Global
        Assert.False(interval.Equals((object) unrelated));
    }

    [Fact]
    public void IdenticalFiniteBoundsHaveEqualHashes()
    {
        var first = new Interval(2, 8);
        var second = new Interval(2, 8);
        var comparer = EqualityComparer<Interval>.Default;

        Assert.True(first.Equals(second));
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
        Assert.Equal(comparer.GetHashCode(first), comparer.GetHashCode(second));
    }
}