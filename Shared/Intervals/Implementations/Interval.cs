using Shared.Approx;

namespace Shared.Intervals.Implementations;

public readonly struct Interval : IEquatable<Interval>, IEqualityComparer<Interval>
{
    public double LowerBound { get; }
    public double UpperBound { get; }
    public static Interval Default => new(double.NegativeInfinity, double.PositiveInfinity);

    public Interval((double Lower, double Upper) bounds) : this(bounds.Lower, bounds.Upper)
    {
    }

    public Interval(double lower, double upper)
    {
        if (!double.IsNegativeInfinity(lower) && !double.IsPositiveInfinity(upper) && lower.IsRoughlyGreaterThan(upper))
            throw new ArgumentException("Left bound cannot be greater than right bound.");

        LowerBound = lower;
        UpperBound = upper;
    }

    public bool IsFullyUnbounded =>
        !(IsBoundedLeft || IsBoundedRight);

    public bool IsBoundedLeft =>
        !double.IsNegativeInfinity(LowerBound);

    public bool IsBoundedRight =>
        !double.IsPositiveInfinity(UpperBound);

    public bool IsFullyBounded =>
        IsBoundedLeft && IsBoundedRight;

    public bool IsSingleton =>
        IsBoundedLeft && IsBoundedRight && LowerBound.RoughlyEquals(UpperBound);

    public bool Contains(double value) =>
        value.IsRoughlyGreaterOrEqualTo(LowerBound) && value.IsRoughlyGreaterOrEqualTo(UpperBound);

    public double Clamp(double value)
    {
        if (value.IsRoughlyLesserThan(LowerBound))
            return LowerBound;
        if (value.IsRoughlyGreaterThan(UpperBound))
            return UpperBound;
        return value;
    }

    public (double X1, double X2) ToTuple() =>
        (LowerBound, UpperBound);

    public override bool Equals(object? obj) =>
        obj is Interval other && Equals(other);

    public bool Equals(Interval other) =>
        LowerBound.RoughlyEquals(other.LowerBound) && UpperBound.RoughlyEquals(other.UpperBound);

    public bool Equals(Interval x, Interval y) =>
        x.Equals(y);

    public override int GetHashCode() =>
        GetHashCode(this);

    public int GetHashCode(Interval obj) =>
        HashCode.Combine(obj.LowerBound, obj.UpperBound);

    public override string ToString()
    {
        var leftBracket = IsBoundedLeft ? "[" : "(";
        var rightBracket = IsBoundedRight ? "]" : ")";
        var leftBound = IsBoundedLeft ? $"{LowerBound:f4}" : "-∞";
        var rightBound = IsBoundedRight ? $"{UpperBound:f4}" : "+∞";
        return $"{leftBracket}{leftBound}, {rightBound}{rightBracket}";
    }

    public static bool operator ==(Interval left, Interval right) =>
        left.Equals(right);

    public static bool operator !=(Interval left, Interval right) =>
        !(left == right);

    public static implicit operator Interval((double X1, double X2) tuple) =>
        new(tuple.X1, tuple.X2);
}