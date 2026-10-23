using Shared.Primitives.Implementation;

namespace Inference.Tree.Abstractions;

public interface IGoalTree<T> where T: class, IGoalTree<T>
{
    StringOrType Identifier { get; }
    ICollection<T> Children { get; }
    bool IsProven { get; internal set; }

    bool IsLeaf();

    void AddChild(T child);

    void AddChildren(params IEnumerable<T> children);
}