namespace Shared.Enumerable;

public static class EnumerableExt
{
    extension<T>(IEnumerable<T> source) where T : notnull
    {
        public IEnumerable<(T Current, T Next)> ToAdjacentPairs()
        {
            ArgumentNullException.ThrowIfNull(source);

            using var enumerator = source.GetEnumerator();

            // Advance to the first element
            if (!enumerator.MoveNext())
                yield break; // empty collection → nothing to pair

            var previous = enumerator.Current;

            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                yield return (previous, current); // emit the pair (prev, curr)
                previous = current; // shift window
            }
        }

        public bool UnsortedEquals(IEnumerable<T> target) =>
            UnsortedEquals(source, target, EqualityComparer<T>.Default);

        public bool UnsortedEquals(IEnumerable<T> target, IEqualityComparer<T> comparer)
        {
            var counterDict = new Dictionary<T, int>(comparer);
            foreach (var item in source)
            {
                if (!counterDict.TryAdd(item, +1))
                {
                    counterDict[item]++;
                }
            }

            foreach (var item in target)
            {
                if (!counterDict.TryAdd(item, -1))
                {
                    return false;
                }
            }

            return counterDict.Values.All(c => c == 0);
        }
    }
}