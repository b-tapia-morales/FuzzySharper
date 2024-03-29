﻿using System.Globalization;
using static FuzzyLogic.Number.IFuzzyNumber<FuzzyLogic.Number.LukasiewiczNumber>;

namespace FuzzyLogic.Number;

public readonly record struct LukasiewiczNumber : IFuzzyNumber<LukasiewiczNumber>, IComparable<LukasiewiczNumber>
{
    private static readonly LukasiewiczNumber Min = Of(0);
    private static readonly LukasiewiczNumber Max = Of(1);

    private LukasiewiczNumber(double value)
    {
        if (Math.Abs(0 - value) < Tolerance) Value = 0;
        if (Math.Abs(1 - value) < Tolerance) Value = 1;
        RangeCheck(value);
        Value = value;
    }

    public double Value { get; }

    public static LukasiewiczNumber Of(double value) => new(value);

    public static bool TryCreate(double number, out LukasiewiczNumber fuzzyNumber)
    {
        try
        {
            fuzzyNumber = Of(number);
            return true;
        }
        catch (ArgumentException)
        {
            fuzzyNumber = number < 0 ? Math.Max(0, number) : Math.Min(1, number);
            return false;
        }
    }

    public static LukasiewiczNumber MinValue() => Min;

    public static LukasiewiczNumber MaxValue() => Max;

    public static LukasiewiczNumber operator !(LukasiewiczNumber x) => (1 - x.Value);

    public static LukasiewiczNumber operator &(LukasiewiczNumber a, LukasiewiczNumber b) =>
        Math.Min(1, a.Value + b.Value);

    public static LukasiewiczNumber operator |(LukasiewiczNumber a, LukasiewiczNumber b) =>
        Math.Max(0, a.Value + b.Value - 1);

    public static LukasiewiczNumber Implication(LukasiewiczNumber a, LukasiewiczNumber b) =>
        Math.Min(1, 1 - a.Value + b.Value);

    /// <summary>
    ///     Defines a implicit conversion from a <see cref="double" /> value to a <see cref="LukasiewiczNumber" />.
    ///     Note that this value must be in the range μ(x) ∈ [0, 1].
    /// </summary>
    /// <param name="x">The <see cref="double" /> value.</param>
    /// <returns>The <see cref="LukasiewiczNumber" /> value.</returns>
    public static implicit operator LukasiewiczNumber(double x) => new(x);

    /// <summary>
    ///     Defines a implicit conversion from a <see cref="LukasiewiczNumber" /> to a <see cref="double" /> value.
    /// </summary>
    /// <param name="x">The <see cref="double" /> value.</param>
    /// <returns>The <see cref="LukasiewiczNumber" /> value.</returns>
    public static implicit operator double(LukasiewiczNumber x) => x.Value;
    
    public int CompareTo(LukasiewiczNumber other) => Value.CompareTo(other.Value);

    /// <inheritdoc />
    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);

    private static void RangeCheck(double value)
    {
        if (value is < 0.0 or > 1.0)
            throw new ArgumentException(
                $"Value can't be lesser than 0 or greater than 1 (Value provided was: {value})");
    }
}