using Reasoning.Proposition.Implementations;
using Shared.Enums;

namespace Reasoning.Proposition.Components;

/// <summary>
/// Represents the <i>Affirmation</i> or <i>Negation</i> part of a <see cref="FuzzyProposition">Fuzzy Proposition</see>,
/// via its two defined instances <see cref="Is"/> and <see cref="IsNot"/>, respectively.
/// <seealso cref="FuzzyProposition"/>
/// </summary>
public class Literal : AbstractEnum<Literal, LiteralType>
{
    /// <summary>
    /// Represents the <i>Affirmation</i> part of a Fuzzy Proposition.
    /// </summary>
    public static readonly Literal Is =
        new(nameof(Is), "IS", (int) LiteralType.Affirmation);

    /// <summary>
    /// Represents the <i>Negation</i> part of a Fuzzy Proposition.
    /// </summary>
    public static readonly Literal IsNot =
        new(nameof(IsNot), "IS NOT", (int) LiteralType.Negation);

    private Literal(string name, string readableName, int value) : base(name, value) =>
        ReadableName = readableName;

    public override string ReadableName { get; }
}

public enum LiteralType
{
    Affirmation,
    Negation
}