using System.Numerics;

namespace Utils.Numbers;

public static class NumberUtils
{
    public static T Min<T>(T a, T b, T c) where T : INumber<T> => T.Min(T.Min(a, b), c);

    public static T Min<T>(params IEnumerable<T> numbers) where T : INumber<T> => numbers.Min() ?? T.Zero;

    public static T Max<T>(T a, T b, T c) where T : INumber<T> => T.Max(T.Max(a, b), c);

    public static T Max<T>(params IEnumerable<T> numbers) where T : INumber<T> => numbers.Max() ?? T.Zero;

    public static T Median<T>(T a, T b, T c) where T : INumber<T> => T.Max(T.Min(a, b), T.Min(T.Max(a, b), c));
}