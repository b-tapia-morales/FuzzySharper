using System.Collections.Immutable;
using Ardalis.SmartEnum;

namespace Shared.Enums;

public abstract class AbstractEnum<TValue, TEnum>(string name, int value) : SmartEnum<TValue>(name, value),
    IEnum<TValue, TEnum>
    where TValue : SmartEnum<TValue>, IEnum<TValue, TEnum>
    where TEnum : struct, Enum, IConvertible
{
    public abstract string ReadableName { get; }

    public static ImmutableList<TEnum> GetEnums() => Enums;

    public static ImmutableList<TValue> GetValues() => Values;

    public static TValue ToValue(TEnum @enum) =>
        EnumDict.TryGetValue(@enum, out var value) ? value : throw new KeyNotFoundException();

    public static bool TryGetValue(TEnum @enum, out TValue? value) =>
        EnumDict.TryGetValue(@enum, out value);

    public static TEnum ToEnum(TValue value) =>
        ValueDict.TryGetValue(value, out var token) ? token : throw new KeyNotFoundException();

    public static bool TryGetEnum(TValue value, out TEnum? @enum)
    {
        @enum = ValueDict.GetValueOrDefault(value);
        return @enum != null;
    }

    private static readonly ImmutableList<TEnum> Enums =
        Enum.GetValues<TEnum>().OrderBy(e => e.ToInt32(null)).ToImmutableList();

    private static ImmutableList<TValue> Values =>
        List.OrderBy(e => e.Value).ToImmutableList();

    private static Dictionary<TEnum, TValue> EnumDict { get; } =
        Enums.Zip(GetValues(), (key, value) => (Key: key, Value: value)).ToDictionary(e => e.Key, e => e.Value);

    private static Dictionary<TValue, TEnum> ValueDict { get; } =
        Enums.Zip(GetValues(), (key, value) => (Key: key, Value: value)).ToDictionary(e => e.Value, e => e.Key);
}