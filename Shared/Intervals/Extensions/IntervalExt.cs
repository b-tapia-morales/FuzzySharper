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

        /// <summary>
        /// Finds gaps between the merged input intervals, optionally including exterior gaps that extend to infinity.
        /// Empty input yields no internal gaps, or the fully unbounded interval when exterior gaps are included.
        /// </summary>
        /// <param name="includeInfinity">Whether to include exterior gaps on sides where coverage is bounded.</param>
        /// <returns>The gaps in ascending order of their lower bounds.</returns>
        public IEnumerable<Interval> FindGaps(bool includeInfinity = false)
        {
            var merged = intervals.FindIntersections().ToList();
            if (merged.Count == 0)
                return includeInfinity ? [Interval.Default] : [];

            var gaps = merged
                .ToAdjacentPairs()
                .Select(p => new Interval(p.Current.UpperBound, p.Next.LowerBound));

            if (!includeInfinity)
                return gaps;

            if (merged[0].IsBoundedLeft)
                gaps = gaps.Prepend(new Interval(double.NegativeInfinity, merged[0].LowerBound));
            if (merged[^1].IsBoundedRight)
                gaps = gaps.Append(new Interval(merged[^1].UpperBound, double.PositiveInfinity));

            return gaps;
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
