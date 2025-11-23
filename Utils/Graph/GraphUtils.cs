namespace Utils.Graph;

public static class GraphUtils
{
    public static ISet<T> FindReachableNodes<T>(IDictionary<T, List<T>> adjacencyList, T root) where T : notnull
    {
        var visited = new HashSet<T>();
        var stack = new Stack<T>();
        stack.Push(root);

        while (stack.Count > 0)
        {
            var current = stack.Pop();
            if (!visited.Add(current))
                continue;

            foreach (var neighbor in adjacencyList[current].Where(neighbor => !visited.Contains(neighbor)))
                stack.Push(neighbor);
        }

        return visited;
    }

    public static IDictionary<T, List<T>> BuildReachableSubgraph<T>(IDictionary<T, List<T>> adjacencyList, T root) where T : notnull
    {
        var reachable = FindReachableNodes(adjacencyList, root);
        return adjacencyList.Where(pair => reachable.Contains(pair.Key)).ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    public static IDictionary<T, int> BuildDepthMap<T>(IDictionary<T, List<T>> adjacencyList, T root) where T : notnull
    {
        var levelMap = new Dictionary<T, int>();
        var queue = new Queue<T>();

        levelMap[root] = 0;
        queue.Enqueue(root);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            foreach (var neighbor in adjacencyList[current].Where(neighbor => !levelMap.ContainsKey(neighbor)))
            {
                levelMap[neighbor] = levelMap[current] + 1;
                queue.Enqueue(neighbor);
            }
        }

        return levelMap;
    }

    public static List<(T From, T To)> FindBackEdges<T>(IDictionary<T, List<T>> adjacencyList, T root) where T : notnull
    {
        var subgraph = BuildReachableSubgraph(adjacencyList, root);
        var levelMap = BuildDepthMap(subgraph, root);

        var backEdges = new List<(T From, T To)>();
        foreach (var (parent, children) in subgraph)
        {
            foreach (var child in children.Where(c => levelMap[c] <= levelMap[parent]))
                backEdges.Add((parent, child));
        }

        return backEdges;
    }
}