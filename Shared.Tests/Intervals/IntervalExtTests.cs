using Shared.Intervals.Extensions;
using Shared.Intervals.Implementations;
using Xunit.Abstractions;

// ReSharper disable CollectionNeverUpdated.Local

namespace Shared.Tests.Intervals;

public class IntervalExtTests(ITestOutputHelper output)
{
    private static readonly List<Interval> Intervals =
    [
        new(0, 1),
        new(2, 3),
        new(3, 6),
        new(4, 8),
        new(9, 12),
        new(11, 15),
        new(16, 20),
        new(18, 22),
        new(23, 27),
        new(28, 30),
        new(29, 35),
        new(36, 40)
    ];

    private static readonly List<(double, double)> MergedIntervals =
    [
        (0, 1),
        (2, 8),
        (9, 15),
        (16, 22),
        (23, 27),
        (28, 35),
        (36, 40)
    ];

    private static readonly List<(double, double)> Gaps =
    [
        (1, 2),
        (8, 9),
        (15, 16),
        (22, 23),
        (27, 28),
        (35, 36)
    ];

    [Fact]
    public void FindIntersectionsReturnsMergedCoverage()
    {
        var expected = MergedIntervals;

        var actual = Intervals.FindIntersections().Select(interval => interval.ToTuple()).ToList();

        output.WriteLine(string.Join(", ", expected));
        output.WriteLine(string.Join(", ", actual));
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ClipToRestrictsMergedIntervalsToBounds()
    {
        var bounds = new Interval(2, 37);
        List<(double, double)> expected =
        [
            (2, 8),
            (9, 15),
            (16, 22),
            (23, 27),
            (28, 35),
            (36, 37)
        ];

        var actual = Intervals
            .FindIntersections()
            .ClipTo(bounds)
            .Select(interval => interval.ToTuple())
            .ToList();

        output.WriteLine(string.Join(", ", actual));
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FindGapsReturnsInternalGapsBetweenGroups()
    {
        var expected = Gaps;

        var actual = Intervals.FindGaps().Select(interval => interval.ToTuple()).ToList();

        output.WriteLine(string.Join(", ", expected));
        output.WriteLine(string.Join(", ", actual));
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FindIntersectionsReturnsEmptyForEmptyInput()
    {
        List<Interval> intervals = [];
        List<(double, double)> expected = [];

        var actual = intervals.FindIntersections().Select(interval => interval.ToTuple()).ToList();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FindIntersectionsPreservesSingleInterval()
    {
        List<Interval> intervals = [new(2, 8)];
        (double, double) expected = (2, 8);

        var actual = Assert.Single(intervals.FindIntersections()).ToTuple();

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(0, 4, 2, 6, 6)]
    [InlineData(0, 2, 2, 5, 5)]
    [InlineData(0, 10, 2, 3, 10)]
    [InlineData(0, 4, 0, 4, 4)]
    public void FindIntersectionsMergesOverlappingOrTouchingIntervals(
        double firstLower, double firstUpper, double secondLower, double secondUpper, double expectedUpper)
    {
        List<Interval> intervals =
        [
            new(firstLower, firstUpper),
            new(secondLower, secondUpper)
        ];
        (double, double) expected = (firstLower, expectedUpper);

        var actual = Assert.Single(intervals.FindIntersections()).ToTuple();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FindIntersectionsSortsInputAndPreservesSeparatedGroups()
    {
        List<Interval> intervals =
        [
            new(5, 8),
            new(0, 2),
            new(10, 11),
            new(1, 4)
        ];

        List<(double, double)> expected =
        [
            (0, 4),
            (5, 8),
            (10, 11)
        ];

        var actual = intervals.FindIntersections().Select(interval => interval.ToTuple()).ToList();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FindIntersectionsIsIndependentOfInputOrder()
    {
        List<Interval> intervals =
        [
            new(5, 8),
            new(0, 2),
            new(10, 11),
            new(1, 4)
        ];

        var expected = intervals.FindIntersections().Select(interval => interval.ToTuple()).ToList();
        var actual = intervals.AsEnumerable().Reverse().FindIntersections().Select(interval => interval.ToTuple()).ToList();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FindIntersectionsProducesSameResultWhenRepeated()
    {
        List<Interval> intervals =
        [
            new(5, 8),
            new(0, 2),
            new(10, 11),
            new(1, 4)
        ];

        var merged = intervals.FindIntersections().ToList();
        var expected = merged.Select(interval => interval.ToTuple()).ToList();

        var actual = merged.FindIntersections().Select(interval => interval.ToTuple()).ToList();

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void FindGapsHandlesEmptyCoverage(bool includeInfinity)
    {
        List<Interval> intervals = [];
        List<(double, double)> expected = includeInfinity
            ? [(double.NegativeInfinity, double.PositiveInfinity)]
            : [];

        var actual = intervals.FindGaps(includeInfinity).Select(interval => interval.ToTuple()).ToList();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FindGapsReturnsEmptyForSingleInterval()
    {
        List<Interval> intervals = [new(2, 8)];
        List<(double, double)> expected = [];

        var actual = intervals.FindGaps().Select(interval => interval.ToTuple()).ToList();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FindGapsIncludesBothExteriorGapsForSingleBoundedInterval()
    {
        List<Interval> intervals = [new(2, 8)];
        List<(double, double)> expected =
        [
            (double.NegativeInfinity, 2),
            (8, double.PositiveInfinity)
        ];

        var actual = intervals.FindGaps(includeInfinity: true).Select(interval => interval.ToTuple()).ToList();

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(1, 3)]
    [InlineData(2, 4)]
    public void FindGapsReturnsEmptyForOverlappingOrTouchingIntervals(double lower, double upper)
    {
        List<Interval> intervals =
        [
            new(0, 2),
            new(lower, upper)
        ];
        List<(double, double)> expected = [];

        var actual = intervals.FindGaps().Select(interval => interval.ToTuple()).ToList();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FindGapsUsesCoverageBoundsForUnsortedInput()
    {
        List<Interval> intervals =
        [
            new(8, 10),
            new(0, 4),
            new(2, 3)
        ];

        List<(double, double)> expected =
        [
            (double.NegativeInfinity, 0),
            (4, 8),
            (10, double.PositiveInfinity)
        ];

        var actual = intervals.FindGaps(includeInfinity: true).Select(interval => interval.ToTuple()).ToList();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FindGapsFindsInternalGapsInUnsortedInput()
    {
        List<Interval> intervals =
        [
            new(8, 10),
            new(0, 4),
            new(2, 3)
        ];

        (double, double) expected = (4, 8);

        var actual = Assert.Single(intervals.FindGaps()).ToTuple();

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(double.NegativeInfinity, 2, 2, double.PositiveInfinity)]
    [InlineData(0, double.PositiveInfinity, double.NegativeInfinity, 0)]
    public void FindGapsOmitsExteriorGapsOnUnboundedSides(
        double lower, double upper, double expectedLower, double expectedUpper)
    {
        List<Interval> intervals = [new(lower, upper)];
        (double, double) expected = (expectedLower, expectedUpper);

        var actual = Assert.Single(intervals.FindGaps(includeInfinity: true)).ToTuple();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FindGapsReturnsEmptyForFullyUnboundedCoverage()
    {
        List<Interval> intervals =
        [
            new(double.NegativeInfinity, 3),
            new(2, double.PositiveInfinity)
        ];
        List<(double, double)> expected = [];

        var actual = intervals.FindGaps(includeInfinity: true).Select(interval => interval.ToTuple()).ToList();

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void FindGapsPreservesInternalGapsBetweenUnboundedIntervals(bool includeInfinity)
    {
        List<Interval> intervals =
        [
            new(double.NegativeInfinity, 0),
            new(2, double.PositiveInfinity)
        ];
        (double, double) expected = (0, 2);

        var actual = Assert.Single(intervals.FindGaps(includeInfinity)).ToTuple();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ClipToReturnsEmptyForEmptyInput()
    {
        List<Interval> intervals = [];
        List<(double, double)> expected = [];

        var actual = intervals.ClipTo(new Interval(2, 8)).Select(interval => interval.ToTuple()).ToList();

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(9, 10)]
    public void ClipToOmitsIntervalsFullyOutsideBounds(double lower, double upper)
    {
        List<Interval> intervals = [new(lower, upper)];
        List<(double, double)> expected = [];

        var actual = intervals.ClipTo(new Interval(2, 8)).Select(interval => interval.ToTuple()).ToList();

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(3, 6, 3, 6)]
    [InlineData(0, 4, 2, 4)]
    [InlineData(6, 10, 6, 8)]
    [InlineData(0, 10, 2, 8)]
    [InlineData(0, 2, 2, 2)]
    [InlineData(8, 10, 8, 8)]
    [InlineData(4, 4, 4, 4)]
    [InlineData(double.NegativeInfinity, double.PositiveInfinity, 2, 8)]
    public void ClipToRestrictsIntervalsToBounds(
        double lower, double upper, double expectedLower, double expectedUpper)
    {
        List<Interval> intervals = [new(lower, upper)];
        (double, double) expected = (expectedLower, expectedUpper);

        var actual = Assert.Single(intervals.ClipTo(new Interval(2, 8))).ToTuple();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ClipToPreservesRetainedIntervalOrder()
    {
        List<Interval> intervals =
        [
            new(9, 10),
            new(6, 10),
            new(3, 4),
            new(0, 1),
            new(0, 3)
        ];

        List<(double, double)> expected =
        [
            (6, 8),
            (3, 4),
            (2, 3)
        ];

        var actual = intervals.ClipTo(new Interval(2, 8)).Select(interval => interval.ToTuple()).ToList();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ClipToPreservesIntervalsForFullyUnboundedBounds()
    {
        List<Interval> intervals = [new(2, 8)];
        var bounds = Interval.Default;
        (double, double) expected = (2, 8);

        var actual = Assert.Single(intervals.ClipTo(bounds)).ToTuple();

        Assert.Equal(expected, actual);
    }
}