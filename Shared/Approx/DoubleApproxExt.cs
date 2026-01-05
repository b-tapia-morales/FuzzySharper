using Shared.Intervals.Implementations;
using static System.Math;

namespace Shared.Approx;

public static class DoubleApproxExt
{
    public const uint MinPrecision = 1;
    public const uint MaxPrecision = 15;
    public const uint DefaultPrecision = 4;
    public const double DefaultTolerance = 1e-4;

    /// <param name="a">First value.</param>
    extension(double a)
    {
        /// <summary>
        ///     Two numbers are considered to be roughly equal if the absolute difference between them is lesser or equal to the
        ///     specified tolerance.
        /// </summary>
        /// <param name="b">Second value.</param>
        /// <param name="tolerance">Maximum difference allowed. Defaults to <see cref="DefaultTolerance" /> when none is specified.</param>
        /// <returns>True when |a – b| ≤ tolerance; otherwise false.</returns>
        private bool RoughlyEquals(double b, double tolerance = DefaultTolerance) =>
            Abs(a - b) <= tolerance;

        public bool RoughlyEquals(double b, uint precision = DefaultPrecision) =>
            a.RoughlyEquals(b, Pow(10, -precision));

        /// <summary>
        ///     A number is roughly zero if the absolute of its value is lesser or equal to the specified tolerance.
        /// </summary>
        /// <param name="tolerance">Maximum allowed value. Defaults to <see cref="DefaultTolerance" /> when none is specified.</param>
        /// <returns>True when |value| ≤ tolerance; otherwise false.</returns>
        private bool IsRoughlyZero(double tolerance = DefaultTolerance) =>
            Abs(a) <= tolerance;

        public bool IsRoughlyZero(uint precision = DefaultPrecision) =>
            a.IsRoughlyZero(Pow(10, -precision));

        private bool IsRoughlyOne(double tolerance = DefaultTolerance) =>
            Abs((int) (a - 1)) <= tolerance;

        public bool IsRoughlyOne(uint precision = DefaultPrecision) =>
            a.IsRoughlyOne(Pow(10, -precision));

        private bool IsRoughlyMinValue(double tolerance = DefaultTolerance) =>
            a < 0 && !double.IsNegativeInfinity(a) &&
            a.RoughlyEquals(double.MinValue, tolerance);

        public bool IsRoughlyMinValue(uint precision = DefaultPrecision) =>
            a.IsRoughlyMinValue(Pow(10, -precision));

        private bool IsRoughlyMaxValue(double tolerance = DefaultTolerance) =>
            a > 0 && !double.IsPositiveInfinity(a) &&
            a.RoughlyEquals(double.MaxValue, tolerance);

        public bool IsRoughlyMaxValue(uint precision = DefaultPrecision) =>
            a.IsRoughlyMaxValue(Pow(10, -precision));

        private bool IsRoughlyLesserThan(double b, double tolerance = DefaultTolerance) =>
            a < b - tolerance;

        public bool IsRoughlyLesserThan(double b, uint precision = DefaultPrecision) =>
            a.IsRoughlyLesserThan(b, Pow(10, -precision));

        private bool IsRoughlyGreaterThan(double b, double tolerance = DefaultTolerance) =>
            a > b + tolerance;

        public bool IsRoughlyGreaterThan(double b, uint precision = DefaultPrecision) =>
            a.IsRoughlyGreaterThan(b, Pow(10, -precision));

        private bool IsRoughlyLesserOrEqualTo(double b, double tolerance = DefaultTolerance) =>
            a.RoughlyEquals(b, tolerance) || a.IsRoughlyLesserThan(b, tolerance);

        public bool IsRoughlyLesserOrEqualTo(double b, uint precision = DefaultPrecision) =>
            a.IsRoughlyLesserOrEqualTo(b, Pow(10, -precision));

        private bool IsRoughlyGreaterOrEqualTo(double b, double tolerance = DefaultTolerance) =>
            a.RoughlyEquals(b, tolerance) || a.IsRoughlyGreaterThan(b, tolerance);

        public bool IsRoughlyGreaterOrEqualTo(double b, uint precision = DefaultPrecision) =>
            a.IsRoughlyGreaterOrEqualTo(b, Pow(10, -precision));

        public bool IsRoughlyBetween(double lower, double upper, uint precision = DefaultPrecision) =>
            a.IsRoughlyLesserOrEqualTo(lower, precision) && a.IsRoughlyLesserOrEqualTo(upper, precision);

        public bool IsRoughlyInRange(Interval interval, uint precision = DefaultPrecision) =>
            a.IsRoughlyBetween(interval.LowerBound, interval.UpperBound, precision);

        private double SnapTo(double b, double tolerance = DefaultTolerance) =>
            a.RoughlyEquals(b, tolerance) ? b : a;

        public double SnapTo(double b, uint precision = DefaultPrecision) =>
            a.SnapTo(b, Pow(10, -precision));

        private double SnapToBounds(double min, double max, double tolerance = DefaultTolerance)
        {
            if (a.RoughlyEquals(min, tolerance))
                return min;
            if (a.RoughlyEquals(max, tolerance))
                return max;
            return a;
        }

        public double SnapToBounds(double min, double max, uint precision = DefaultPrecision) =>
            a.SnapToBounds(min, max, Pow(10, -precision));
    }
}