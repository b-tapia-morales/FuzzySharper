using Shared.Approx;
using Shared.Enumerable;
using Shared.Intervals.Implementations;

namespace Shared.Intervals.Extensions;

public static class IntervalExt
{
    extension(IEnumerable<Interval> intervals)
    {
        public IEnumerable<Interval> FindIntersections()
        {
            return intervals
                .OrderBy(i => i.LowerBound)
                .Aggregate(new List<Interval>(), (accumulator, current) =>
                {
                    // No overlap – create a new interval
                    if (accumulator.Count == 0 || current.LowerBound.IsRoughlyGreaterThan(accumulator[^1].UpperBound))
                    {
                        accumulator.Add(current);
                        return accumulator;
                    }

                    // Overlap – Extend the last one
                    accumulator[^1] =
                        new Interval(accumulator[^1].LowerBound,
                            Math.Max(accumulator[^1].UpperBound, current.UpperBound));
                    return accumulator;
                });
        }

        public IEnumerable<Interval> FindGaps(bool includeInfinity = false)
        {
            var list = intervals.ToList();
            var (first, last) = (list[0].LowerBound, list[^1].UpperBound);
            var gaps = list
                .FindIntersections()
                .ToAdjacentPairs()
                .Select(p => new Interval(p.Current.UpperBound, p.Next.LowerBound));
            return includeInfinity
                ? gaps.Prepend(new Interval(double.NegativeInfinity, first)).Append(new Interval(last, double.PositiveInfinity))
                : gaps;
        }

        public IEnumerable<Interval> ClipTo(Interval bounds)
        {
            foreach (var interval in intervals)
            {
                // Discard if the whole gap lies outside the universe
                if (interval.UpperBound.IsRoughlyLesserThan(bounds.LowerBound) || interval.LowerBound.IsRoughlyGreaterThan(bounds.UpperBound))
                    continue;

                var lower = Math.Max(interval.LowerBound, bounds.LowerBound);
                var upper = Math.Min(interval.UpperBound, bounds.UpperBound);

                if (lower.IsRoughlyLesserOrEqualTo(upper))
                    yield return new Interval(lower, upper);
            }
        }
    }
}