using System.Collections.Immutable;
using Ardalis.SmartEnum;

namespace Shared.Enums;

public interface IEnum<TValue, TEnum>
    where TValue : SmartEnum<TValue>, IEnum<TValue, TEnum>
    where TEnum : struct, Enum, IConvertible
{
    string ReadableName { get; }

    static abstract ImmutableList<TEnum> GetEnums();

    static abstract ImmutableList<TValue> GetValues();

    static abstract TValue ToValue(TEnum @enum);

    static abstract bool TryGetValue(TEnum @enum, out TValue? value);

    static abstract TEnum ToEnum(TValue value);

    static abstract bool TryGetEnum(TValue value, out TEnum? @enum);
}