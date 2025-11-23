using Kernel.Number;
using Kernel.Operator.Negation.Abstractions;
using Knowledge.Memory.Abstractions;
using Reasoning.Proposition.Abstractions;
using Reasoning.Proposition.Components;
using Shared.Options.Factory;
using Shared.Options.Implementations;
using Shared.Primitives.Implementation;

namespace Reasoning.Proposition.Implementations;

public class BooleanProposition<T>(T value, Connective connective, Literal literal) :
    IEquatableProposition<BooleanProposition<T>> where T : struct, Enum, IConvertible
{
    public StringOrType Identifier { get; } = typeof(T);
    public string Variable { get; } = value.GetType().Name;
    public Connective Connective { get; } = connective;
    public Literal Literal { get; } = literal;
    public string Term { get; } = value.ToString();
    private T Value { get; } = value;

    public static bool EqualsMethod(BooleanProposition<T>? x, BooleanProposition<T>? y) =>
        ReferenceEquals(x, y) || x != null && x.MemberwiseEquals(y);

    public override bool Equals(object? obj) =>
        ReferenceEquals(this, obj) || obj is BooleanProposition<T> other && MemberwiseEquals(other);

    public override int GetHashCode() =>
        MemberwiseHashCode();

    public override string ToString() =>
        $"{Connective} {Variable} {Literal.ReadableName} {Term}";

    public bool IsEvaluable(IWorkingMemory memory) =>
        memory.ContainsCategoricalFact<T>();

    public Option<FuzzyNumber> Evaluate(IWorkingMemory memory, INegation negation) =>
        !memory.GetCategoricalFact<T>().IsSomeVal(out var crispValue) ? OptionFactory.None<FuzzyNumber>() : Evaluate(crispValue, negation);

    public FuzzyNumber Evaluate(DoubleOrEnum value, INegation negation) =>
        value.IsEnum ? Evaluate((T) value.AsEnum) : throw new ArgumentException("");

    public bool MemberwiseEquals(BooleanProposition<T>? other) =>
        other != null &&
        Value.Equals(other.Value) &&
        Connective == other.Connective &&
        Literal == other.Literal &&
        string.Equals(Term, other.Term, StringComparison.OrdinalIgnoreCase);

    public int MemberwiseHashCode() =>
        HashCode.Combine(Variable, Connective.Name, Literal.Name, Term);

    private FuzzyNumber Evaluate(T value)
    {
        var booleanValue = Value.Equals(value);
        var literalValue = Literal == Literal.IsNot ? !booleanValue : booleanValue;
        return Convert.ToDouble(literalValue);
    }
}