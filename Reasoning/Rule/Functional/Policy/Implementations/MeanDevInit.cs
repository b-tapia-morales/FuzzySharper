using Kernel.Function.Extensions;
using Reasoning.Proposition.Implementations;
using Reasoning.Rule.Functional.Policy.Abstractions;
using Utils.Shape;

namespace Reasoning.Rule.Functional.Policy.Implementations;

public class MeanDevInit : BaseCoefficientInitPolicy
{
    protected bool Normalize { get; init; }

    public override IEnumerable<double> Initialize(IReadOnlyList<FuzzyProposition> premise)
    {
        var coefficients = premise.Select(prop => prop.Function.CalculateCentroid(Axis.X) - prop.Function.EffectiveSupport.Midpoint.Get).ToList();
        if (!Normalize)
            return coefficients;
        var widths = premise.Select(prop => prop.Function.EffectiveSupport.Width.Get);
        return coefficients.Zip(widths, (coefficient, width) => coefficient / width);
    }
}