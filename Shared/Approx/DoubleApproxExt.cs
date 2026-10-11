using Shared.Intervals.Implementations;
using static System.Math;

namespace Shared.Approx;

/// <summary>Provides approximate comparisons and snapping operations for double values.</summary>
/// <remarks>
/// Precision sets the absolute tolerance to 10 raised to the negative precision.
/// The limits defined by <see cref="MinPrecision"/> and <see cref="MaxPrecision"/> are not enforced.
/// </remarks>
public static class DoubleApproxExt
{
    /// <summary>The lower precision bound: 1 decimal place.</summary>
    public const uint MinPrecision = 1;

    /// <summary>The upper precision bound: 15 decimal places.</summary>
    public const uint MaxPrecision = 15;

    /// <summary>The default precision: 4 decimal places.</summary>
    public const uint DefaultPrecision = 4;

    /// <summary>The default maximum absolute difference: 0.0001.</summary>
    public const double DefaultTolerance = 1e-4;

    /// <param name="a">The value to compare or snap.</param>
    extension(double a)
    {
        /// <summary>
        /// Two numbers are considered to be roughly equal if the absolute difference between them is less than or equal to the
        /// specified tolerance.
        /// </summary>
        /// <param name="b">Second value.</param>
        /// <param name="tolerance">Maximum difference allowed. Defaults to <see cref="DefaultTolerance" /> when none is specified.</param>
        /// <returns>True when |a – b| ≤ tolerance; otherwise false.</returns>
        private bool RoughlyEquals(double b, double tolerance = DefaultTolerance) =>
            Abs(a - b) <= tolerance;

        /// <summary>
        /// Two numbers are considered to be roughly equal if the absolute difference between them is less than or equal to
        /// 10 raised to the negative precision.
        /// </summary>
        /// <param name="b">Second value.</param>
        /// <param name="precision">Decimal precision used to set the tolerance. Defaults to <see cref="DefaultPrecision"/>.</param>
        /// <returns>True when |a – b| ≤ 10^−precision; otherwise false.</returns>
        public bool RoughlyEquals(double b, uint precision = DefaultPrecision) =>
            a.RoughlyEquals(b, Pow(10, -precision));

        /// <summary>
        /// A number is roughly zero if its absolute value is less than or equal to the specified tolerance.
        /// </summary>
        /// <param name="tolerance">Maximum allowed absolute value. Defaults to <see cref="DefaultTolerance" /> when none is specified.</param>
        /// <returns>True when |a| ≤ tolerance; otherwise false.</returns>
        private bool IsRoughlyZero(double tolerance = DefaultTolerance) =>
            Abs(a) <= tolerance;

        /// <summary>
        /// A number is roughly zero if its absolute value is less than or equal to 10 raised to the negative precision.
        /// </summary>
        /// <param name="precision">Decimal precision used to set the tolerance. Defaults to <see cref="DefaultPrecision"/>.</param>
        /// <returns>True when |a| ≤ 10^−precision; otherwise false.</returns>
        public bool IsRoughlyZero(uint precision = DefaultPrecision) =>
            a.IsRoughlyZero(Pow(10, -precision));

        /// <summary>
        /// Compares the absolute integer part of the difference from one with the specified tolerance.
        /// </summary>
        /// <param name="tolerance">Maximum allowed difference. Defaults to <see cref="DefaultTolerance"/>.</param>
        /// <returns>True when |(int)(a – 1)| ≤ tolerance; otherwise false.</returns>
        /// <exception cref="OverflowException">The difference converts to <see cref="int.MinValue"/>.</exception>
        private bool IsRoughlyOne(double tolerance = DefaultTolerance) =>
            Abs(a - 1) <= tolerance;

        /// <summary>
        /// Compares the absolute integer part of the difference from one with the tolerance set by the specified precision.
        /// </summary>
        /// <param name="precision">Decimal precision used to set the tolerance. Defaults to <see cref="DefaultPrecision"/>.</param>
        /// <returns>True if the truncated difference is within tolerance; otherwise false.</returns>
        /// <exception cref="OverflowException">The difference converts to <see cref="int.MinValue"/>.</exception>
        public bool IsRoughlyOne(uint precision = DefaultPrecision) =>
            a.IsRoughlyOne(Pow(10, -precision));

        /// <summary>
        /// A finite negative number is roughly <see cref="double.MinValue"/> if its absolute difference from that value
        /// is less than or equal to the specified tolerance.
        /// </summary>
        /// <param name="tolerance">Maximum allowed difference. Defaults to <see cref="DefaultTolerance"/>.</param>
        /// <returns>True if the value is roughly <see cref="double.MinValue"/>; otherwise false.</returns>
        private bool IsRoughlyMinValue(double tolerance = DefaultTolerance) =>
            a < 0 && !double.IsNegativeInfinity(a) &&
            a.RoughlyEquals(double.MinValue, tolerance);

        /// <summary>
        /// A finite negative number is roughly <see cref="double.MinValue"/> if its absolute difference from that value
        /// is less than or equal to 10 raised to the negative precision.
        /// </summary>
        /// <param name="precision">Decimal precision used to set the tolerance. Defaults to <see cref="DefaultPrecision"/>.</param>
        /// <returns>True if the value is roughly <see cref="double.MinValue"/>; otherwise false.</returns>
        public bool IsRoughlyMinValue(uint precision = DefaultPrecision) =>
            a.IsRoughlyMinValue(Pow(10, -precision));

        /// <summary>
        /// A finite positive number is roughly <see cref="double.MaxValue"/> if its absolute difference from that value
        /// is less than or equal to the specified tolerance.
        /// </summary>
        /// <param name="tolerance">Maximum allowed difference. Defaults to <see cref="DefaultTolerance"/>.</param>
        /// <returns>True if the value is roughly <see cref="double.MaxValue"/>; otherwise false.</returns>
        private bool IsRoughlyMaxValue(double tolerance = DefaultTolerance) =>
            a > 0 && !double.IsPositiveInfinity(a) &&
            a.RoughlyEquals(double.MaxValue, tolerance);

        /// <summary>
        /// A finite positive number is roughly <see cref="double.MaxValue"/> if its absolute difference from that value
        /// is less than or equal to 10 raised to the negative precision.
        /// </summary>
        /// <param name="precision">Decimal precision used to set the tolerance. Defaults to <see cref="DefaultPrecision"/>.</param>
        /// <returns>True if the value is roughly <see cref="double.MaxValue"/>; otherwise false.</returns>
        public bool IsRoughlyMaxValue(uint precision = DefaultPrecision) =>
            a.IsRoughlyMaxValue(Pow(10, -precision));

        /// <summary>
        /// A number is roughly less than another if it is below the second value minus the specified tolerance.
        /// </summary>
        /// <param name="b">Second value.</param>
        /// <param name="tolerance">The margin below the second value. Defaults to <see cref="DefaultTolerance"/>.</param>
        /// <returns>True when a &lt; b – tolerance; otherwise false.</returns>
        private bool IsRoughlyLesserThan(double b, double tolerance = DefaultTolerance) =>
            a < b - tolerance;

        /// <summary>
        /// A number is roughly less than another if it is below the second value minus the tolerance set by precision.
        /// </summary>
        /// <param name="b">Second value.</param>
        /// <param name="precision">Decimal precision used to set the tolerance. Defaults to <see cref="DefaultPrecision"/>.</param>
        /// <returns>True when a &lt; b – 10^−precision; otherwise false.</returns>
        public bool IsRoughlyLesserThan(double b, uint precision = DefaultPrecision) =>
            a.IsRoughlyLesserThan(b, Pow(10, -precision));

        /// <summary>
        /// A number is roughly greater than another if it is above the second value plus the specified tolerance.
        /// </summary>
        /// <param name="b">Second value.</param>
        /// <param name="tolerance">The margin above the second value. Defaults to <see cref="DefaultTolerance"/>.</param>
        /// <returns>True when a &gt; b + tolerance; otherwise false.</returns>
        private bool IsRoughlyGreaterThan(double b, double tolerance = DefaultTolerance) =>
            a > b + tolerance;

        /// <summary>
        /// A number is roughly greater than another if it is above the second value plus the tolerance set by precision.
        /// </summary>
        /// <param name="b">Second value.</param>
        /// <param name="precision">Decimal precision used to set the tolerance. Defaults to <see cref="DefaultPrecision"/>.</param>
        /// <returns>True when a &gt; b + 10^−precision; otherwise false.</returns>
        public bool IsRoughlyGreaterThan(double b, uint precision = DefaultPrecision) =>
            a.IsRoughlyGreaterThan(b, Pow(10, -precision));

        /// <summary>
        /// A number is roughly less than or equal to another if the two are roughly equal,
        /// or the first is roughly less than the second, using the specified tolerance.
        /// </summary>
        /// <param name="b">Second value.</param>
        /// <param name="tolerance">Tolerance for the comparisons. Defaults to <see cref="DefaultTolerance"/>.</param>
        /// <returns>True if <paramref name="a"/> is roughly less than or equal to <paramref name="b"/>; otherwise false.</returns>
        private bool IsRoughlyLesserOrEqualTo(double b, double tolerance = DefaultTolerance) =>
            a.RoughlyEquals(b, tolerance) || a.IsRoughlyLesserThan(b, tolerance);

        /// <summary>
        /// A number is roughly less than or equal to another if the two are roughly equal,
        /// or the first is roughly less than the second, using the specified precision.
        /// </summary>
        /// <param name="b">Second value.</param>
        /// <param name="precision">Decimal precision used to set the tolerance. Defaults to <see cref="DefaultPrecision"/>.</param>
        /// <returns>True if <paramref name="a"/> is roughly less than or equal to <paramref name="b"/>; otherwise false.</returns>
        public bool IsRoughlyLesserOrEqualTo(double b, uint precision = DefaultPrecision) =>
            a.IsRoughlyLesserOrEqualTo(b, Pow(10, -precision));

        /// <summary>
        /// A number is roughly greater than or equal to another if the two are roughly equal,
        /// or the first is roughly greater than the second, using the specified tolerance.
        /// </summary>
        /// <param name="b">Second value.</param>
        /// <param name="tolerance">Tolerance for the comparisons. Defaults to <see cref="DefaultTolerance"/>.</param>
        /// <returns>True if <paramref name="a"/> is roughly greater than or equal to <paramref name="b"/>; otherwise false.</returns>
        private bool IsRoughlyGreaterOrEqualTo(double b, double tolerance = DefaultTolerance) =>
            a.RoughlyEquals(b, tolerance) || a.IsRoughlyGreaterThan(b, tolerance);

        /// <summary>
        /// A number is roughly greater than or equal to another if the two are roughly equal,
        /// or the first is roughly greater than the second, using the specified precision.
        /// </summary>
        /// <param name="b">Second value.</param>
        /// <param name="precision">Decimal precision used to set the tolerance. Defaults to <see cref="DefaultPrecision"/>.</param>
        /// <returns>True if <paramref name="a"/> is roughly greater than or equal to <paramref name="b"/>; otherwise false.</returns>
        public bool IsRoughlyGreaterOrEqualTo(double b, uint precision = DefaultPrecision) =>
            a.IsRoughlyGreaterOrEqualTo(b, Pow(10, -precision));

        /// <summary>Returns whether the value is roughly less than or equal to both supplied bounds.</summary>
        /// <param name="lower">The lower bound.</param>
        /// <param name="upper">The upper bound.</param>
        /// <param name="precision">Decimal precision used to set the tolerance. Defaults to <see cref="DefaultPrecision"/>.</param>
        /// <returns>True if both comparisons succeed; otherwise false.</returns>
        /// <remarks>Currently treats both bounds as upper limits, rather than checking containment.</remarks>
        public bool IsRoughlyBetween(double lower, double upper, uint precision = DefaultPrecision) =>
            a.IsRoughlyLesserOrEqualTo(lower, precision) && a.IsRoughlyGreaterOrEqualTo(upper, precision);

        /// <summary>Applies <c>IsRoughlyBetween</c> to the interval's bounds.</summary>
        /// <param name="interval">The interval whose bounds are compared.</param>
        /// <param name="precision">Decimal precision used to set the tolerance. Defaults to <see cref="DefaultPrecision"/>.</param>
        /// <returns>True if the value is roughly less than or equal to both bounds; otherwise false.</returns>
        /// <remarks>Currently treats both interval bounds as upper limits, rather than checking containment.</remarks>
        public bool IsRoughlyInRange(Interval interval, uint precision = DefaultPrecision) =>
            a.IsRoughlyBetween(interval.LowerBound, interval.UpperBound, precision);

        /// <summary>Returns the target if the two values are roughly equal; otherwise, returns the original value.</summary>
        /// <param name="b">The target value.</param>
        /// <param name="tolerance">Maximum allowed difference. Defaults to <see cref="DefaultTolerance"/>.</param>
        /// <returns>The target when within tolerance, or the original value.</returns>
        private double SnapTo(double b, double tolerance = DefaultTolerance) =>
            a.RoughlyEquals(b, tolerance) ? b : a;

        /// <summary>
        /// Returns the target if the two values are roughly equal at the specified precision;
        /// otherwise, returns the original value.
        /// </summary>
        /// <param name="b">The target value.</param>
        /// <param name="precision">Decimal precision used to set the tolerance. Defaults to <see cref="DefaultPrecision"/>.</param>
        /// <returns>The target when within tolerance, or the original value.</returns>
        public double SnapTo(double b, uint precision = DefaultPrecision) =>
            a.SnapTo(b, Pow(10, -precision));

        /// <summary>
        /// Returns the lower bound if the value is roughly equal to it, then checks the upper bound.
        /// Returns the original value if neither bound is within tolerance.
        /// </summary>
        /// <param name="min">The lower bound.</param>
        /// <param name="max">The upper bound.</param>
        /// <param name="tolerance">Maximum allowed difference. Defaults to <see cref="DefaultTolerance"/>.</param>
        /// <returns>The first matching bound, or the original value.</returns>
        /// <remarks>The lower bound takes precedence when both bounds are within tolerance.</remarks>
        private double SnapToBounds(double min, double max, double tolerance = DefaultTolerance)
        {
            if (a.RoughlyEquals(min, tolerance))
                return min;
            if (a.RoughlyEquals(max, tolerance))
                return max;
            return a;
        }

        /// <summary>
        /// Returns the lower bound if the value is roughly equal to it, then checks the upper bound,
        /// using the specified precision. Returns the original value if neither bound is within tolerance.
        /// </summary>
        /// <param name="min">The lower bound.</param>
        /// <param name="max">The upper bound.</param>
        /// <param name="precision">Decimal precision used to set the tolerance. Defaults to <see cref="DefaultPrecision"/>.</param>
        /// <returns>The first matching bound, or the original value.</returns>
        /// <remarks>The lower bound takes precedence when both bounds are within tolerance.</remarks>
        public double SnapToBounds(double min, double max, uint precision = DefaultPrecision) =>
            a.SnapToBounds(min, max, Pow(10, -precision));
    }
}
