using Kernel.Function.Extensions;
using Reasoning.Proposition.Abstractions;
using Reasoning.Proposition.Implementations;
using Reasoning.Rule.Functional.Policy.Abstractions;
using Shared.Intervals.Implementations;
using Shared.Options.Factory;
using Shared.Options.Implementations;
using Utils.Shape;

namespace Reasoning.Rule.Functional.Policy.Implementations;

public class CentroidInit : BaseCoefficientInitPolicy
{
    protected bool Normalize { get; init; }

    public override IEnumerable<double> Initialize(IReadOnlyList<IProposition> premise)
    {
        var centroids = premise.Select(p => EvaluateCoefficient(p, prop => prop.Function.CalculateCentroid(Axis.X))).ToList();
        if (!Normalize)
            return centroids;
        var options = premise.Select(p => p is FuzzyProposition {Function.UniverseOfDiscourse.IsFullyBounded: true} prop
            ? prop.Function.UniverseOfDiscourse
            : Option<Interval>.None()
        );
        return centroids.Zip(options, (centroid, option) => option.IsSome(out var uoD) ? (centroid - uoD.LowerBound) / uoD.Width.Get : 0);
    }
}