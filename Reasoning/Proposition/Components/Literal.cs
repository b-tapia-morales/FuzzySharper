using Reasoning.Proposition.Abstractions;
using Shared.Enums;

namespace Reasoning.Proposition.Components;

/// <summary>
/// Represents the affirmation or negation operator of a <see cref="IProposition">proposition</see>, defined by the
/// instances <see cref="Is"/> and <see cref="IsNot"/>, respectively.
/// </summary>
public class Literal : AbstractEnum<Literal, LiteralType>
{
    /// <summary>
    /// Represents the affirmation operator of a proposition.
    /// </summary>
    public static readonly Literal Is =
        new(nameof(Is), "IS", (int) LiteralType.Affirmation);

    /// <summary>
    /// Represents the negation operator of a proposition.
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