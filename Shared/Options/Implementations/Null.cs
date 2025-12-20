namespace Shared.Options.Implementations;

public sealed class Null : IEquatable<Null>, IEqualityComparer<Null>
{
    private Null()
    {
    }

    public static Null GetInstance { get; } = new();

    public override bool Equals(object? obj) =>
        ReferenceEquals(this, obj);

    public bool Equals(Null? other) =>
        ReferenceEquals(this, other);

    public bool Equals(Null? x, Null? y) =>
        ReferenceEquals(x, y);

    public override int GetHashCode() => 0;

    public int GetHashCode(Null obj) => 0;

    public override string ToString() => "null";
}