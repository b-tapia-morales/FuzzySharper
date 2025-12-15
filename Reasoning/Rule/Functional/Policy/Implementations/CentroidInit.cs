using Kernel.Function.Extensions;
using Reasoning.Proposition.Abstractions;
using Reasoning.Proposition.Implementations;
using Reasoning.Rule.Functional.Policy.Abstractions;
using Shared.Intervals.Implementations;
using Shared.Options.Factory;
using Utils.Shape;
using static Reasoning.Rule.Functional.Policy.Abstractions.ICoefficientInitPolicy;

namespace Reasoning.Rule.Functional.Policy.Implementations;

public class CentroidInit : ICoefficientInitPolicy
{
    protected bool Normalize { get; init; }

    public (IList<double> Coefficients, double Bias) Initialize(IList<IProposition> propositions)
    {
        var centroids = propositions.Select(p => EvaluateCoefficient(p, prop => prop.Function.CalculateCentroid(Axis.X))).ToList();
        if (!Normalize)
            return (centroids, 0);
        var options = propositions.Select(p => p is FuzzyProposition {Function.UniverseOfDiscourse.IsFullyBounded: true} prop
            ? prop.Function.UniverseOfDiscourse
            : OptionFactory.None<Interval>()
        );
        return ([..centroids.Zip(options, (centroid, option) => option.IsSomeVal(out var uoD) ? (centroid - uoD.LowerBound) / uoD.Width.Get : 0)], 0);
    }
}