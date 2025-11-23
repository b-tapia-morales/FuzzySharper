using System.Collections.Immutable;

namespace Utils.Enumerable;

public static class EnumerableUtils
{
    private static ScrambledListComparer<T> ScrambledComparer<T>() where T : IComparable<T> => new();

    public static bool ScrambledEquals<T>(this IEnumerable<T> list1, IEnumerable<T> list2) where T : IComparable<T>
    {
        var dict1 = ToFrequencyMap(list1);
        var dict2 = ToFrequencyMap(list2);
        return dict1.All(pair => dict2.TryGetValue(pair.Key, out var value) && value.Equals(pair.Value));
    }

    public static IEnumerable<IList<T>> FilterUnique<T>(this IEnumerable<IList<T>> listOfLists) where T : IComparable<T> =>
        listOfLists.Distinct(ScrambledComparer<T>());

    public static IReadOnlyDictionary<T, int> ToFrequencyMap<T>(this IEnumerable<T> list) where T : IComparable<T> =>
        list.GroupBy(item => item).ToImmutableDictionary(group => group.Key, group => group.Count());
}

public class ScrambledListComparer<T> : IEqualityComparer<IList<T>> where T : IComparable<T>
{
    public bool Equals(IList<T>? x, IList<T>? y) =>
        x != null && y != null && x.ScrambledEquals(y);

    public int GetHashCode(IList<T> obj) =>
        obj.Order().GetHashCode();
}