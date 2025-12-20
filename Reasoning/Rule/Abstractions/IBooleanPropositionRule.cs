namespace Reasoning.Rule.Abstractions;

public interface IBooleanPropositionRule<out T> : IRule where T: class, IBooleanPropositionRule<T>
{
    T If<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible;
    
    T IfNot<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible;
    
    T And<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible;
    
    T AndNot<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible;
    
    T Or<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible;
    
    T OrNot<TEnum>(TEnum value) where TEnum : struct, Enum, IConvertible;
}