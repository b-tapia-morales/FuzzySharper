namespace Shared.Options.Implementations;

/// <summary>Represents the missing-value alternative stored by an empty option.</summary>
/// <remarks>
/// <see cref="GetInstance"/> provides the shared sentinel. This object is distinct from a null reference,
/// and its equality methods compare references rather than contained data.
/// </remarks>
public sealed class Null : IEquatable<Null>, IEqualityComparer<Null>
{
    private Null()
    {
    }

    /// <summary>Gets the shared sentinel used to represent an option's missing-value alternative.</summary>
    public static Null GetInstance { get; } = new();

    /// <summary>Determines whether the supplied object is this sentinel instance.</summary>
    /// <param name="obj">The object reference to compare with this instance.</param>
    /// <returns>True for the same reference; false for null or any other reference.</returns>
    public override bool Equals(object? obj) =>
        ReferenceEquals(this, obj);

    /// <summary>Determines whether the supplied sentinel is this instance.</summary>
    /// <param name="other">The sentinel reference to compare with this instance, or null.</param>
    /// <returns>True for the same reference; false for null or any other reference.</returns>
    public bool Equals(Null? other) =>
        ReferenceEquals(this, other);

    /// <summary>Compares two sentinel references by reference identity.</summary>
    /// <param name="x">The first sentinel reference, which may be null.</param>
    /// <param name="y">The second sentinel reference, which may be null.</param>
    /// <returns>True for the same reference, including two null references; otherwise false.</returns>
    public bool Equals(Null? x, Null? y) =>
        ReferenceEquals(x, y);

    /// <summary>Returns the sentinel's constant hash code.</summary>
    /// <returns>Zero.</returns>
    public override int GetHashCode() => 0;

    /// <summary>Returns the constant hash code used for a sentinel.</summary>
    /// <param name="obj">The sentinel whose hash code is requested; its value does not affect the result.</param>
    /// <returns>Zero.</returns>
    public int GetHashCode(Null obj) => 0;

    /// <summary>Returns the sentinel's textual representation.</summary>
    /// <returns>The literal string <c>null</c>.</returns>
    public override string ToString() => "null";
}
