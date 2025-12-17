using Reasoning.Rule.FuzzySet.Abstractions;

namespace Reasoning.Rule.Abstractions;

public interface IEquatableRule<TSelf> : IFuzzySetRule, IEquatable<TSelf>, IEqualityComparer<TSelf>
    where TSelf : class, IEquatableRule<TSelf>
{
    bool IEquatable<TSelf>.Equals(TSelf? other) =>
        ReferenceEquals(this, other) || MemberwiseEquals(other);

    bool IEqualityComparer<TSelf>.Equals(TSelf? x, TSelf? y) =>
        ReferenceEquals(x, y) || x != null && x.MemberwiseEquals(y);

    int IEqualityComparer<TSelf>.GetHashCode(TSelf obj) =>
        obj.MemberwiseHashCode();
    
    bool MemberwiseEquals(TSelf? other);
    
    int MemberwiseHashCode();
}