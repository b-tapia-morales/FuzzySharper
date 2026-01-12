using Reasoning.Proposition.Abstractions;
using Reasoning.Rule.Abstractions;
using Shared.Enums;

namespace Reasoning.Proposition.Components;

/// <summary>
/// Represents a logical connective used to compose a <see cref="IRule">rule</see> by chaining multiple
/// <see cref="IProposition">propositions</see> into a single structured expression. A connective defines how a
/// proposition participates in the rule and how additional propositions may be joined to it.
/// The supported connectives are <see cref="If"/>, <see cref="And"/>, <see cref="Or"/>, and <see cref="Then"/>.
/// </summary>
public class Connective : AbstractEnum<Connective, ConnectiveType>
{
    /// <summary>
    /// Marks the beginning of a rule premise and identifies the proposition as part of the antecedent.
    /// </summary>
    public static readonly Connective If =
        new(nameof(If), "IF", (int) ConnectiveType.Antecedent);
    
    /// <summary>
    /// Represents a logical conjunction between two propositions in the rule premise.
    /// </summary>
    public static readonly Connective And =
        new(nameof(And), "AND", (int) ConnectiveType.Conjunction);

    /// <summary>
    /// Represents a logical disjunction between two propositions in the rule premise.
    /// </summary>
    public static readonly Connective Or =
        new(nameof(Or), "OR", (int) ConnectiveType.Disjunction);

    /// <summary>
    /// Marks the beginning of the rule consequent and identifies the proposition as part of the conclusion.
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