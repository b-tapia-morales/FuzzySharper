using Shared.Enums;

namespace Utils.Csv;

public sealed class Delimiter : AbstractEnum<Delimiter, DelimiterType>
{
    public static readonly Delimiter Comma =
        new(nameof(Comma), ",", (int) DelimiterType.Comma);

    public static readonly Delimiter Semicolon =
        new(nameof(Semicolon), ";", (int) DelimiterType.Semicolon);

    public static readonly Delimiter Pipe =
        new(nameof(Pipe), "|", (int) DelimiterType.Pipe);

    public static readonly Delimiter Tab =
        new(nameof(Tab), "\t", (int) DelimiterType.Tab);

    private Delimiter(string name, string character, int value) : base(name, value) =>
        Character = character;

    public string Character { get; }

    public override string ReadableName => Name;
}

public enum DelimiterType
{
    Comma,
    Semicolon,
    Pipe,
    Tab
}