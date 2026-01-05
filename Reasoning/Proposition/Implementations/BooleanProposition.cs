using Kernel.Number;
using Kernel.Operator.Negation.Abstractions;
using Knowledge.Memory.Abstractions;
using Reasoning.Proposition.Abstractions;
using Reasoning.Proposition.Components;
using Shared.Options.Factory;
using Shared.Options.Implementations;
using Shared.Primitives.Implementation;

// ReSharper disable MemberCanBePrivate.Global

namespace Reasoning.Proposition.Implementations;

public sealed class BooleanProposition<T>(T value, Connective connective, Literal literal) : IEquatable<BooleanProposition<T>>, IProposition where T : struct, Enum, IConvertible
{
    public StringOrType Identifier { get; } = typeof(T);
    public Connective Connective { get; } = connective;
    public Literal Literal { get; } = literal;
    public string Label { get; } = value.ToString();
    private T Value { get; } = value;

    public bool Equals(BooleanProposition<T>? other) =>
        other != null &&
        EqualityComparer<T>.Equals(this, other) &&
        Connective == other.Connective &&
        Literal == other.Literal &&
        string.Equals(Label, other.Label, StringComparison.OrdinalIgnoreCase);

    public override bool Equals(object? obj) =>
        ReferenceEquals(this, obj) || obj is BooleanProposition<T> other && Equals(other);

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Value, EqualityComparer<T>.Default);
        hash.Add(Connective);
        hash.Add(Literal);
        hash.Add(Label, StringComparer.OrdinalIgnoreCase);
        return hash.ToHashCode();
    }

    public override string ToString() =>
        $"{Connective} {Identifier.AsType.Name} {Literal.ReadableName} {Label}";

    public bool IsEvaluable(IWorkingMemory memory) =>
        memory.ContainsCategoricalFact<T>();

    Option<FuzzyNumber> IProposition.Evaluate(IWorkingMemory memory, INegation negation) =>
        !memory.GetCategoricalFact<T>().IsSome(out var enumValue) ? Option<FuzzyNumber>.None() : Evaluate(enumValue);

    FuzzyNumber IProposition.Evaluate(DoubleOrEnum value, INegation negation) =>
        value.IsEnum ? Evaluate((T) value.AsEnum) : throw new ArgumentException("");

    public FuzzyNumber Evaluate(T value)
    {
        var booleanValue = EqualityComparer<T>.Default.Equals(Value, value);
        var literalValue = Literal == Literal.IsNot ? !booleanValue : booleanValue;
        return Convert.ToDouble(literalValue);
    }

    public IProposition DeepCopy() =>
        new BooleanProposition<T>(Value, Connective, Literal);
}