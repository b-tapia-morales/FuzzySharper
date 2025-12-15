using Kernel.Function.Abstractions;
using Kernel.Number;
using Kernel.Operator.Negation.Abstractions;
using Kernel.Operator.Negation.Implementations.Canonical;
using Knowledge.Memory.Abstractions;
using Reasoning.Proposition.Abstractions;
using Reasoning.Proposition.Components;
using Shared.Options.Factory;
using Shared.Options.Implementations;
using Shared.Primitives.Implementation;

namespace Reasoning.Proposition.Implementations;

public class FuzzyProposition(string variable, Connective connective, Literal literal, LinguisticHedge linguisticHedge, IMembershipFunction function)
    : IEquatableProposition<FuzzyProposition>
{
    public StringOrType Identifier { get; } = variable;
    public string Variable { get; } = variable;
    public Connective Connective { get; } = connective;
    public Literal Literal { get; } = literal;
    public string Term { get; } = function.Name;
    public LinguisticHedge LinguisticHedge { get; } = linguisticHedge;
    public IMembershipFunction Function { get; } = function;

    public override bool Equals(object? obj) =>
        ReferenceEquals(this, obj) || obj is FuzzyProposition other && MemberwiseEquals(other);

    public override int GetHashCode() =>
        MemberwiseHashCode();

    public override string ToString()
    {
        var hedge = LinguisticHedge != LinguisticHedge.None ? $"{LinguisticHedge.ReadableName} " : string.Empty;
        return $"{Connective} {Variable} {Literal.ReadableName} {hedge}{Function.Name}";
    }

    public bool MemberwiseEquals(FuzzyProposition? other) =>
        other != null &&
        string.Equals(Variable, other.Variable, StringComparison.OrdinalIgnoreCase) &&
        Connective == other.Connective &&
        Literal == other.Literal &&
        LinguisticHedge == other.LinguisticHedge &&
        string.Equals(Term, other.Term, StringComparison.OrdinalIgnoreCase);

    public int MemberwiseHashCode() =>
        HashCode.Combine(Variable, Connective.Name, Literal.Name, LinguisticHedge.Name, Term);

    public bool IsEvaluable(IWorkingMemory memory) =>
        memory.ContainsNumericFact(Variable);

    public Option<FuzzyNumber> Evaluate(IWorkingMemory memory, INegation negation) =>
        !memory.GetNumericFact(Variable).IsSomeVal(out var crispValue) ? OptionFactory.None<FuzzyNumber>() : Evaluate(crispValue, negation);

    public FuzzyNumber Evaluate(DoubleOrEnum value, INegation negation) =>
        value.IsDouble ? Evaluate(value.AsDouble, negation) : throw new ArgumentException("");

    public FuzzyNumber Evaluate(double crispValue, INegation negation)
    {
        var membershipFunction = Function.PureFunctionClipped;
        var hedgeFunction = LinguisticHedge.Function;
        var fuzzyNumber = hedgeFunction(membershipFunction(crispValue));
        return Literal == Literal.IsNot ? negation.Complement(fuzzyNumber) : fuzzyNumber;
    }

    public FuzzyNumber Evaluate(double crispValue) =>
        Evaluate(crispValue, Negation.Standard);
}