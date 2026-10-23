using Shared.Primitives.Implementation;

namespace Inference.Tree.Abstractions;

public abstract class AbstractGoalTree<T>(StringOrType identifier) : IGoalTree<T> where T : AbstractGoalTree<T>
{
    public StringOrType Identifier { get; } = identifier;
    public ICollection<T> Children { get; } = new List<T>();
    public bool IsProven { get; set; }

    public bool IsLeaf() =>
        Children.Count == 0;

    public void AddChild(T child) =>
        Children.Add(child);

    public void AddChildren(IEnumerable<T> children)
    {
        foreach (var child in children)
            Children.Add(child);
    }
}