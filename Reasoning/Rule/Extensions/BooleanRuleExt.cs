using Reasoning.Proposition.Components;
using Reasoning.Rule.Abstractions;

namespace Reasoning.Rule.Extensions;

internal static class BooleanRuleExt
{
    extension<T>(T rule) where T : class, IBooleanPropositionRule<T>
    {
        public T If<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
            rule.AddAntecedent(value, Literal.Is);

        public T IfNot<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
            rule.AddAntecedent(value, Literal.IsNot);

        public T And<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
            rule.AddConnective(value, Connective.And, Literal.Is);

        public T AndNot<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
            rule.AddConnective(value, Connective.And, Literal.IsNot);

        public T Or<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
            rule.AddConnective(value, Connective.Or, Literal.Is);

        public T OrNot<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible =>
            rule.AddConnective(value, Connective.Or, Literal.IsNot);
    }
}