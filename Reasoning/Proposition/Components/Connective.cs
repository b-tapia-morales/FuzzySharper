using Reasoning.Proposition.Implementations;
using Shared.Enums;

namespace Reasoning.Proposition.Components;

/// <summary>
/// Represents a <i>Logical Connective</i> between <see cref="FuzzyProposition">Fuzzy Propositions</see>,
/// defined by the instances <see cref="If"/>, <see cref="And"/>, <see cref="Or"/>, and <see cref="Then"/>.
/// Logical connectives enable the creation of compound propositions by conjoining two or more
/// <see cref="FuzzyProposition">Fuzzy Propositions</see> with their respective connectives.
/// <seealso cref="FuzzyProposition"/>
/// </summary>
public class Connective : AbstractEnum<Connective, ConnectiveType>
{
    /// <summary>
    /// Marks the part of a compound proposition <i>before</i> the <see cref="Then"/> connective
    /// as the premise.
    /// </summary>
    public static readonly Connective If =
        new(nameof(If), "IF", (int) ConnectiveType.Antecedent);

    /// <summary>
    /// Represents the <b>Conjunction</b> between two Fuzzy Propositions.
    /// </summary>
    public static readonly Connective And =
        new(nameof(And), "AND", (int) ConnectiveType.Conjunction);

    /// <summary>
    /// Represents the <b>Disjunction</b> between two Fuzzy Propositions.
    /// </summary>
    public static readonly Connective Or =
        new(nameof(Or), "OR", (int) ConnectiveType.Disjunction);

    /// <summary>
    /// Marks the part of a compound proposition <i>after</i> the <see cref="Then"/> connective
    /// as the conclusion.
    /// </summary>
    public static readonly Connective Then =
        new(nameof(Then), "THEN", (int) ConnectiveType.Consequent);

    private Connective(string name, string readableName, int value) : base(name, value) =>
        ReadableName = readableName;

    public override string ReadableName { get; }
}

public enum ConnectiveType
{
    Antecedent,
    Conjunction,
    Disjunction,
    Consequent
}