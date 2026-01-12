using Kernel.Number;
using Reasoning.Proposition.Implementations;
using Shared.Enums;
using static System.Math;

namespace Reasoning.Proposition.Components;

/// <summary>
/// <para>
/// Represents a <i>linguistic hedge</i>, a unary modifier applied to a
/// <see cref="FuzzyProposition">fuzzy proposition</see>’s truth value in order to concentrate, dilate, or intensify
/// its membership degree.
/// A linguistic hedge transforms a <see cref="FuzzyNumber"/> by applying a mathematical function that models common
/// linguistic intensifiers and attenuators such as <i>Very</i>, <i>Slightly</i>, or <i>Indeed</i>.
/// </para>
/// Linguistic hedges participate in the unary evaluation pipeline of a proposition, where the truth value is computed
/// as: <c>Literal(Hedge(MembershipDegree))</c>.
/// </summary>
public class LinguisticHedge : AbstractEnum<LinguisticHedge, HedgeType>
{
    /// <summary>
    /// Represents the neutral hedge, which leaves the truth value unchanged: <c>y' = y</c>
    /// </summary>
    public static readonly LinguisticHedge None =
        new(nameof(None), string.Empty, y => y, (int) HedgeType.None);

    /// <summary>
    /// Represents the hedge <i>Very</i>, which intensifies a truth value: <c>y' = y²</c>
    /// </summary>
    public static readonly LinguisticHedge Very =
        new(nameof(Very), "Very", y => Pow(y.Value, 2), (int) HedgeType.Very);

    /// <summary>
    /// Represents the hedge <i>Very Very</i>, which strongly intensifies a truth value: <c>y' = y⁴</c>
    /// </summary>
    public static readonly LinguisticHedge VeryVery =
        new(nameof(VeryVery), "Very, very", y => Pow(y.Value, 4), (int) HedgeType.VeryVery);

    /// <summary>
    /// Represents the hedge <i>Plus</i>, which mildly intensifies a truth value: <c>y' = y^(5/4)</c>
    /// </summary>
    public static readonly LinguisticHedge Plus =
        new(nameof(Plus), "Plus", y => Pow(y.Value, 5 / 4.0), (int) HedgeType.Plus);

    /// <summary>
    /// Represents the hedge <i>Slightly</i>, which attenuates a truth value: <c>y' = √y</c>
    /// </summary>
    public static readonly LinguisticHedge Slightly =
        new(nameof(Slightly), "Slightly", y => Sqrt(y.Value), (int) HedgeType.Slightly);

    /// <summary>
    /// Represents the hedge <i>Minus</i>, which mildly attenuates a truth value: <c>y' = y^(3/4)</c>
    /// </summary>
    public static readonly LinguisticHedge Minus =
        new(nameof(Minus), "Minus", y => Pow(y.Value, 3 / 4.0), (int) HedgeType.Minus);

    /// <summary>
    /// Represents the hedge <i>Indeed</i>, which emphasizes values close to full membership or non-membership:
    /// <list type="bullet">
    /// <item>if y &lt; 0.5 ⇒ <c>y' = 2y²</c></item>
    /// <item>if y = 0.5 ⇒ <c>y' = y</c></item>
    /// <item>if y &gt; 0.5 ⇒ <c>y' = 1 − 2(1 − y)²</c></item>
    /// </list>
    /// </summary>
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