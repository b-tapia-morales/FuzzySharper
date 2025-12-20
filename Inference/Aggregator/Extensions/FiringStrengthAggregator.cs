using System.Diagnostics;
using Inference.Aggregator.Abstractions;
using Inference.Aggregator.Components;
using Inference.Aggregator.Factory;
using Kernel.Function.Abstractions;
using Kernel.Function.Comparer.Factory;

namespace Inference.Aggregator.Extensions;

public static class FiringStrengthAggregator
{
    private static readonly IComparer<IMembershipFunction> FunctionComparer = OrderingFactory.GetInstance(OrderingMethod.Centroid);

    public static double AggregateFirings(IList<FiringStrength> firingStrengths, Func<FiringStrength, double> valueSelector, ValueAggregatorMethod strategy) =>
        AggregateFirings(firingStrengths, valueSelector, ValueAggregatorFactory.GetInstance(strategy));

    public static double AggregateFirings(IList<FiringStrength> firingStrengths, Func<FiringStrength, double> valueSelector, IValueAggregator aggregator)
    {
        var maxFiringStrengths = SelectFirings(firingStrengths);
        var values = maxFiringStrengths.Select(valueSelector).ToList();
        return aggregator.Aggregate(values);
    }

    public static List<FiringStrength> SelectFirings(IList<FiringStrength> firingStrengths)
    {
        ArgumentNullException.ThrowIfNull(firingStrengths);
        Debug.Assert(firingStrengths.Count > 0);

        var maxGroup = firingStrengths.GroupBy(strength => strength.Weight).OrderByDescending(group => group.Key).First().ToList();
        maxGroup.Sort((x, y) => FunctionComparer.Compare(x.Function, y.Function));
        return maxGroup;
    }
}