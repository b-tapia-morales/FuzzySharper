using Shared.Approx;
using Shared.Intervals.Implementations;

namespace Shared.Tests.Approx;

public class DoubleApproxExtTests
{
    [Theory]
    [InlineData(0, 0, true)]
    [InlineData(2, 2, true)]
    [InlineData(0, 0.00005, true)]
    [InlineData(0, 0.0001, true)]
    [InlineData(-0.0001, 0, true)]
    [InlineData(0, 0.0002, false)]
    [InlineData(-2, 2, false)]
    public void RoughlyEqualsUsesDefaultToleranceAndIsSymmetric(double first, double second, bool expected)
    {
        Assert.Equal(expected, first.RoughlyEquals(second));
        Assert.Equal(expected, second.RoughlyEquals(first));
    }

    [Theory]
    [InlineData(1u)]
    [InlineData(4u)]
    [InlineData(15u)]
    public void RoughlyEqualsIncludesToleranceBoundary(uint precision)
    {
        var tolerance = Math.Pow(10, -(double)precision);
        var outside = Math.BitIncrement(tolerance);

        Assert.True(tolerance.RoughlyEquals(0, precision));
        Assert.True((-tolerance).RoughlyEquals(0, precision));
        Assert.False(outside.RoughlyEquals(0, precision));
        Assert.False((-outside).RoughlyEquals(0, precision));
    }

    [Theory]
    [InlineData(3u, true)]
    [InlineData(4u, false)]
    public void RoughlyEqualsHonorsPrecision(uint precision, bool expected)
    {
        Assert.Equal(expected, 0.0005.RoughlyEquals(0, precision));
    }

    [Theory]
    [InlineData(double.NaN, 0)]
    [InlineData(0, double.NaN)]
    [InlineData(double.NaN, double.NaN)]
    [InlineData(double.PositiveInfinity, double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity, double.NegativeInfinity)]
    [InlineData(double.PositiveInfinity, double.NegativeInfinity)]
    [InlineData(0, double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity, 0)]
    public void RoughlyEqualsRejectsNonFiniteComparisons(double first, double second)
    {
        Assert.False(first.RoughlyEquals(second));
    }

    [Theory]
    [InlineData(0, true)]
    [InlineData(0.00005, true)]
    [InlineData(-0.00005, true)]
    [InlineData(0.0001, true)]
    [InlineData(-0.0001, true)]
    [InlineData(0.0002, false)]
    [InlineData(-0.0002, false)]
    [InlineData(double.NaN, false)]
    [InlineData(double.PositiveInfinity, false)]
    [InlineData(double.NegativeInfinity, false)]
    public void IsRoughlyZeroUsesDefaultTolerance(double value, bool expected)
    {
        Assert.Equal(expected, value.IsRoughlyZero());
    }

    [Theory]
    [InlineData(1u)]
    [InlineData(4u)]
    [InlineData(15u)]
    public void IsRoughlyZeroIncludesToleranceBoundary(uint precision)
    {
        var tolerance = Math.Pow(10, -(double)precision);
        var outside = Math.BitIncrement(tolerance);

        Assert.True(tolerance.IsRoughlyZero(precision));
        Assert.True((-tolerance).IsRoughlyZero(precision));
        Assert.False(outside.IsRoughlyZero(precision));
        Assert.False((-outside).IsRoughlyZero(precision));
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(1.00005, true)]
    [InlineData(0.99995, true)]
    [InlineData(1.0002, false)]
    [InlineData(0.9998, false)]
    [InlineData(0.5, false)]
    [InlineData(1.5, false)]
    [InlineData(0, false)]
    [InlineData(2, false)]
    [InlineData(-2147483647d, false)]
    [InlineData(double.MinValue, false)]
    [InlineData(double.MaxValue, false)]
    [InlineData(double.NaN, false)]
    [InlineData(double.PositiveInfinity, false)]
    [InlineData(double.NegativeInfinity, false)]
    public void IsRoughlyOneUsesTheFullDifferenceWithoutIntegerConversion(double value, bool expected)
    {
        Assert.Equal(expected, value.IsRoughlyOne());
    }

    [Theory]
    [InlineData(1.005, 2u, true)]
    [InlineData(1.005, 3u, false)]
    [InlineData(0.995, 2u, true)]
    [InlineData(0.995, 3u, false)]
    public void IsRoughlyOneCompliesWithDefaultPrecision(double value, uint precision, bool expected)
    {
        Assert.Equal(expected, value.IsRoughlyOne(precision));
    }

    [Theory]
    [InlineData(double.MinValue, true, false)]
    [InlineData(double.MaxValue, false, true)]
    [InlineData(-1, false, false)]
    [InlineData(0, false, false)]
    [InlineData(1, false, false)]
    [InlineData(double.NaN, false, false)]
    [InlineData(double.NegativeInfinity, false, false)]
    [InlineData(double.PositiveInfinity, false, false)]
    public void ExtremeValueChecksRecognizeFiniteEndpoints(double value, bool isMinimum, bool isMaximum)
    {
        Assert.Equal(isMinimum, value.IsRoughlyMinValue());
        Assert.Equal(isMaximum, value.IsRoughlyMaxValue());
    }

    [Fact]
    public void ExtremeValueChecksRejectAdjacentRepresentableValues()
    {
        Assert.False(Math.BitIncrement(double.MinValue).IsRoughlyMinValue());
        Assert.False(Math.BitDecrement(double.MaxValue).IsRoughlyMaxValue());
    }

    [Theory]
    [InlineData(-1, 0, true, false)]
    [InlineData(1, 0, false, true)]
    [InlineData(0, 0, false, false)]
    [InlineData(-0.00005, 0, false, false)]
    [InlineData(0.00005, 0, false, false)]
    [InlineData(-0.0002, 0, true, false)]
    [InlineData(0.0002, 0, false, true)]
    [InlineData(double.NaN, 0, false, false)]
    [InlineData(0, double.NaN, false, false)]
    [InlineData(0, double.PositiveInfinity, true, false)]
    [InlineData(0, double.NegativeInfinity, false, true)]
    [InlineData(double.PositiveInfinity, double.PositiveInfinity, false, false)]
    public void StrictComparisonsUseDefaultTolerance(double first, double second, bool isLess, bool isGreater)
    {
        Assert.Equal(isLess, first.IsRoughlyLesserThan(second));
        Assert.Equal(isGreater, first.IsRoughlyGreaterThan(second));
    }

    [Theory]
    [InlineData(1u)]
    [InlineData(4u)]
    [InlineData(15u)]
    public void StrictComparisonsExcludeToleranceBoundary(uint precision)
    {
        var tolerance = Math.Pow(10, -(double)precision);
        var outside = Math.BitIncrement(tolerance);

        Assert.False((-tolerance).IsRoughlyLesserThan(0, precision));
        Assert.True((-outside).IsRoughlyLesserThan(0, precision));
        Assert.False(tolerance.IsRoughlyGreaterThan(0, precision));
        Assert.True(outside.IsRoughlyGreaterThan(0, precision));
    }

    [Theory]
    [InlineData(-1, 0, true, false)]
    [InlineData(1, 0, false, true)]
    [InlineData(0, 0, true, true)]
    [InlineData(-0.00005, 0, true, true)]
    [InlineData(0.00005, 0, true, true)]
    [InlineData(-0.0002, 0, true, false)]
    [InlineData(0.0002, 0, false, true)]
    [InlineData(double.NaN, 0, false, false)]
    [InlineData(0, double.NaN, false, false)]
    [InlineData(0, double.PositiveInfinity, true, false)]
    [InlineData(0, double.NegativeInfinity, false, true)]
    [InlineData(double.PositiveInfinity, double.PositiveInfinity, false, false)]
    public void NonStrictComparisonsUseDefaultTolerance(double first, double second, bool isLessOrEqual, bool isGreaterOrEqual)
    {
        Assert.Equal(isLessOrEqual, first.IsRoughlyLesserOrEqualTo(second));
        Assert.Equal(isGreaterOrEqual, first.IsRoughlyGreaterOrEqualTo(second));
    }

    [Theory]
    [InlineData(1u)]
    [InlineData(4u)]
    [InlineData(15u)]
    public void NonStrictComparisonsIncludeToleranceBoundary(uint precision)
    {
        var tolerance = Math.Pow(10, -(double)precision);
        var outside = Math.BitIncrement(tolerance);

        Assert.True(tolerance.IsRoughlyLesserOrEqualTo(0, precision));
        Assert.False(outside.IsRoughlyLesserOrEqualTo(0, precision));
        Assert.True((-tolerance).IsRoughlyGreaterOrEqualTo(0, precision));
        Assert.False((-outside).IsRoughlyGreaterOrEqualTo(0, precision));
    }

    [Theory]
    [InlineData(0, true)]
    [InlineData(0.5, true)]
    [InlineData(1, true)]
    [InlineData(-0.00005, true)]
    [InlineData(1.00005, true)]
    [InlineData(-0.0002, false)]
    [InlineData(1.0002, false)]
    [InlineData(-1, false)]
    [InlineData(2, false)]
    [InlineData(double.NaN, false)]
    [InlineData(double.NegativeInfinity, false)]
    [InlineData(double.PositiveInfinity, false)]
    public void IsRoughlyBetweenIncludesInteriorAndEndpoints(double value, bool expected)
    {
        Assert.Equal(expected, value.IsRoughlyBetween(0, 1));
    }

    [Theory]
    [InlineData(-0.0005, 3u, true)]
    [InlineData(-0.0005, 4u, false)]
    [InlineData(1.0005, 3u, true)]
    [InlineData(1.0005, 4u, false)]
    public void IsRoughlyBetweenHonorsPrecision(double value, uint precision, bool expected)
    {
        Assert.Equal(expected, value.IsRoughlyBetween(0, 1, precision));
    }

    [Theory]
    [InlineData(0, true)]
    [InlineData(0.0001, true)]
    [InlineData(-0.0001, true)]
    [InlineData(0.0002, false)]
    [InlineData(-0.0002, false)]
    public void IsRoughlyBetweenHandlesSingletonBounds(double value, bool expected)
    {
        Assert.Equal(expected, value.IsRoughlyBetween(0, 0));
    }

    [Theory]
    [InlineData(0, true)]
    [InlineData(0.5, true)]
    [InlineData(1, true)]
    [InlineData(-0.00005, true)]
    [InlineData(1.00005, true)]
    [InlineData(-0.0002, false)]
    [InlineData(1.0002, false)]
    [InlineData(double.NaN, false)]
    public void IsRoughlyInRangeIncludesInteriorAndEndpoints(double value, bool expected)
    {
        var interval = new Interval(0, 1);

        Assert.Equal(expected, value.IsRoughlyInRange(interval));
    }

    [Theory]
    [InlineData(double.NegativeInfinity, 1, -2, true)]
    [InlineData(double.NegativeInfinity, 1, 2, false)]
    [InlineData(0, double.PositiveInfinity, 2, true)]
    [InlineData(0, double.PositiveInfinity, -2, false)]
    [InlineData(double.NegativeInfinity, double.PositiveInfinity, 0, true)]
    [InlineData(double.NegativeInfinity, double.PositiveInfinity, double.NaN, false)]
    public void IsRoughlyInRangeHandlesUnboundedIntervals(double lower, double upper, double value, bool expected)
    {
        var interval = new Interval(lower, upper);

        Assert.Equal(expected, value.IsRoughlyInRange(interval));
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(0.00005, 0, 0)]
    [InlineData(0.0001, 0, 0)]
    [InlineData(-0.0001, 0, 0)]
    [InlineData(0.0002, 0, 0.0002)]
    [InlineData(-0.0002, 0, -0.0002)]
    [InlineData(1.00005, 1, 1)]
    [InlineData(0.5, 1, 0.5)]
    public void SnapToReplacesOnlyValuesWithinDefaultTolerance(double value, double target, double expected)
    {
        Assert.Equal(expected, value.SnapTo(target));
    }

    [Theory]
    [InlineData(1u)]
    [InlineData(4u)]
    [InlineData(15u)]
    public void SnapToIncludesToleranceBoundary(uint precision)
    {
        var tolerance = Math.Pow(10, -(double)precision);
        var outside = Math.BitIncrement(tolerance);

        Assert.Equal(0, tolerance.SnapTo(0, precision));
        Assert.Equal(0, (-tolerance).SnapTo(0, precision));
        Assert.Equal(outside, outside.SnapTo(0, precision));
        Assert.Equal(-outside, (-outside).SnapTo(0, precision));
    }

    [Theory]
    [InlineData(3u, 0)]
    [InlineData(4u, 0.0005)]
    public void SnapToHonorsPrecision(uint precision, double expected)
    {
        Assert.Equal(expected, 0.0005.SnapTo(0, precision));
    }

    [Fact]
    public void SnapToPreservesValuesWhenComparisonInvolvesNaNOrInfinity()
    {
        Assert.True(double.IsNaN(double.NaN.SnapTo(0)));
        Assert.Equal(double.PositiveInfinity, double.PositiveInfinity.SnapTo(0));
        Assert.Equal(double.NegativeInfinity, double.NegativeInfinity.SnapTo(0));
        Assert.Equal(1, 1d.SnapTo(double.NaN));
        Assert.Equal(1, 1d.SnapTo(double.PositiveInfinity));
    }

    [Theory]
    [InlineData(-0.00005, 0)]
    [InlineData(0, 0)]
    [InlineData(0.00005, 0)]
    [InlineData(0.99995, 1)]
    [InlineData(1, 1)]
    [InlineData(1.00005, 1)]
    [InlineData(0.5, 0.5)]
    [InlineData(-1, -1)]
    [InlineData(2, 2)]
    [InlineData(double.PositiveInfinity, double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity, double.NegativeInfinity)]
    public void SnapToBoundsSnapsNearEndpointsWithoutClamping(double value, double expected)
    {
        Assert.Equal(expected, value.SnapToBounds(0, 1));
    }

    [Theory]
    [InlineData(1u)]
    [InlineData(4u)]
    [InlineData(15u)]
    public void SnapToBoundsIncludesToleranceBoundary(uint precision)
    {
        var tolerance = Math.Pow(10, -(double)precision);
        var outside = Math.BitIncrement(tolerance);

        Assert.Equal(0, tolerance.SnapToBounds(0, 1, precision));
        Assert.Equal(outside, outside.SnapToBounds(0, 1, precision));
        Assert.Equal(0, (-tolerance).SnapToBounds(-1, 0, precision));
        Assert.Equal(-outside, (-outside).SnapToBounds(-1, 0, precision));
    }

    [Theory]
    [InlineData(3u, 0)]
    [InlineData(4u, 0.0005)]
    public void SnapToBoundsHonorsPrecision(uint precision, double expected)
    {
        Assert.Equal(expected, 0.0005.SnapToBounds(0, 1, precision));
    }

    [Fact]
    public void SnapToBoundsPrefersLowerBoundWhenBothMatch()
    {
        Assert.Equal(0, 0.000075.SnapToBounds(0, 0.0001));
    }

    [Fact]
    public void SnapToBoundsPreservesNaN()
    {
        Assert.True(double.IsNaN(double.NaN.SnapToBounds(0, 1)));
    }
}
