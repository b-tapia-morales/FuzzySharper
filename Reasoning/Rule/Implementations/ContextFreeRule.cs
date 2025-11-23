using Knowledge.Linguistic.Base.Abstractions;
using Reasoning.Adaptation.Components;
using Reasoning.Comparer.Implementations.Deterministic;
using Reasoning.Proposition.Abstractions;
using Reasoning.Proposition.Components;
using Reasoning.Proposition.Implementations;
using Reasoning.Rule.Abstractions;
using Reasoning.Rule.Exceptions;
using Reasoning.Rule.Extensions;
using Shared.Options.Factory;
using Shared.Options.Implementations;

namespace Reasoning.Rule.Implementations;

public class ContextFreeRule : IEquatableRule<ContextFreeRule>
{
    public IProposition? Conditional { get; set; }
    public ICollection<IProposition> Connectives { get; } = new List<IProposition>();
    public FuzzyProposition? Consequent { get; set; }
    public bool IsFinalized { get; set; }
    public Option<RulePriority> Priority { get; private init; } = OptionFactory.None<RulePriority>();
    public Option<double> CertaintyFactor { get; private init; } = OptionFactory.None<double>();
    public DateTimeOffset CreationTime { get; } = DateTimeOffset.Now;
    public AdaptationState AdaptationState { get; set; } = new();

    public static IRule Create() =>
        new ContextFreeRule();

    public static IRule Create(RulePriority priority) =>
        new ContextFreeRule
        {
            Priority = priority
        };

    public static IRule Create(double certaintyFactor) =>
        new ContextFreeRule
        {
            CertaintyFactor = IRule.ValidateFactor(certaintyFactor)
        };

    public override bool Equals(object? obj)
    {
        this.Validate();
        return ReferenceEquals(this, obj) || obj is ContextFreeRule other && MemberwiseEquals(other);
    }

    public override int GetHashCode()
    {
        this.Validate();
        return MemberwiseHashCode();
    }

    public override string ToString()
    {
        this.Validate();
        return $"{Conditional} {(Connectives.Count != 0 ? $"{string.Join(' ', Connectives)} " : string.Empty)}{Consequent}";
    }

    public bool MemberwiseEquals(ContextFreeRule? other)
    {
        this.Validate();
        return other != null &&
               Equals(Conditional, other.Conditional) &&
               Connectives.SequenceEqual(other.Connectives) &&
               Equals(Consequent, other.Consequent);
    }

    public int MemberwiseHashCode()
    {
        this.Validate();
        return HashCode.Combine(Conditional, Connectives, Consequent);
    }

    public IRule If(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        this.AddAntecedent(linguisticBase, variableName, Literal.Is, LinguisticHedge.ToValue(hedgeType), termName);

    public IRule If(string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        throw new ContextMismatchException();

    public IRule If<T>(T value) where T : struct, Enum, IEquatable<T> =>
        this.AddAntecedent(value, Literal.Is);

    public IRule IfNot(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        this.AddAntecedent(linguisticBase, variableName, Literal.IsNot, LinguisticHedge.ToValue(hedgeType), termName);

    public IRule IfNot(string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        throw new ContextMismatchException();

    public IRule IfNot<T>(T value) where T : struct, Enum, IConvertible =>
        this.AddAntecedent(value, Literal.IsNot);

    public IRule And(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        this.AddConnective(linguisticBase, Connective.And, variableName, Literal.Is, LinguisticHedge.ToValue(hedgeType), termName);

    public IRule And(string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        throw new ContextMismatchException();

    public IRule And<T>(T value) where T : struct, Enum, IConvertible =>
        this.AddConnective(value, Connective.And, Literal.Is);

    public IRule AndNot(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        this.AddConnective(linguisticBase, Connective.And, variableName, Literal.IsNot, LinguisticHedge.ToValue(hedgeType), termName);

    public IRule AndNot(string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        throw new ContextMismatchException();

    public IRule AndNot<T>(T value) where T : struct, Enum, IConvertible =>
        this.AddConnective(value, Connective.And, Literal.IsNot);

    public IRule Or(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        this.AddConnective(linguisticBase, Connective.Or, variableName, Literal.Is, LinguisticHedge.ToValue(hedgeType), termName);

    public IRule Or(string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        throw new ContextMismatchException();

    public IRule Or<T>(T value) where T : struct, Enum, IConvertible =>
        this.AddConnective(value, Connective.Or, Literal.Is);

    public IRule OrNot(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        this.AddConnective(linguisticBase, Connective.Or, variableName, Literal.IsNot, LinguisticHedge.ToValue(hedgeType), termName);

    public IRule OrNot(string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        throw new ContextMismatchException();

    public IRule OrNot<T>(T value) where T : struct, Enum, IConvertible =>
        this.AddConnective(value, Connective.Or, Literal.IsNot);

    public IRule Then(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        this.AddConsequent(linguisticBase, variableName, Literal.Is, LinguisticHedge.ToValue(hedgeType), termName);

    public IRule Then(string variableName, string termName, HedgeType hedgeType = HedgeType.None) =>
        throw new ContextMismatchException();
}