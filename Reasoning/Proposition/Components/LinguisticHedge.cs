using Kernel.Number;
using Shared.Enums;
using static System.Math;

namespace Reasoning.Proposition.Components;

public class LinguisticHedge : AbstractEnum<LinguisticHedge, HedgeType>
{
    public static readonly LinguisticHedge None =
        new(nameof(None), string.Empty, y => y, (int) HedgeType.None);

    public static readonly LinguisticHedge Very =
        new(nameof(Very), "Very", y => Pow(y.Value, 2), (int) HedgeType.Very);

    public static readonly LinguisticHedge VeryVery =
        new(nameof(VeryVery), "Very, very", y => Pow(y.Value, 4), (int) HedgeType.VeryVery);

    public static readonly LinguisticHedge Plus =
        new(nameof(Plus), "Plus", y => Pow(y.Value, 5 / 4.0), (int) HedgeType.Plus);

    public static readonly LinguisticHedge Slightly =
        new(nameof(Slightly), "Slightly", y => Sqrt(y.Value), (int) HedgeType.Slightly);

    public static readonly LinguisticHedge Minus =
        new(nameof(Minus), "Minus", y => Pow(y.Value, 3 / 4.0), (int) HedgeType.Minus);

    public static readonly LinguisticHedge Indeed = new(nameof(Indeed), "Indeed",
        y =>
        {
            if (y == 0.5) return y;
            return y < 0.5 ? 2 * Pow(y.Value, 2) : 1 - 2 * Pow(1 - y.Value, 2);
        }, (int) HedgeType.Indeed);

    private LinguisticHedge(string name, string readableName, Func<FuzzyNumber, FuzzyNumber> function, int value) :
        base(name, value)
    {
        ReadableName = readableName;
        Function = function;
    }

    public override string ReadableName { get; }
    public Func<FuzzyNumber, FuzzyNumber> Function { get; }
}

public enum HedgeType
{
    None,
    Very,
    VeryVery,
    Plus,
    Slightly,
    Minus,
    Indeed
}