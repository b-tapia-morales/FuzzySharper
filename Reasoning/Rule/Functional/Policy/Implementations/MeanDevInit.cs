using Kernel.Function.Extensions;
using Reasoning.Proposition.Abstractions;
using Reasoning.Rule.Functional.Policy.Abstractions;
using Utils.Shape;

namespace Reasoning.Rule.Functional.Policy.Implementations;

public class MeanDevInit : BaseCoefficientInitPolicy
{
    protected bool Normalize { get; init; }

    public override IEnumerable<double> Initialize(IReadOnlyList<IProposition> premise)
    {
        var coefficients = premise.Select(p => EvaluateCoefficient(p, prop => prop.Function.CalculateCentroid(Axis.X) - prop.Function.EffectiveSupport.Midpoint.Get)).ToList();
        if (!Normalize)
            return coefficients;
        var widths = premise.Select(p => EvaluateCoefficient(p, prop => prop.Function.EffectiveSupport.Width.Get));
        return coefficients.Zip(widths, (coefficient, width) => coefficient / width);
    }
}