using Kernel.Function.Extensions;
using Reasoning.Proposition.Abstractions;
using Reasoning.Rule.Functional.Policy.Abstractions;
using Utils.Shape;
using static Reasoning.Rule.Functional.Policy.Abstractions.ICoefficientInitPolicy;

namespace Reasoning.Rule.Functional.Policy.Implementations;

public class MeanDevInit : ICoefficientInitPolicy
{
    protected bool Normalize { get; init; }

    public (IList<double> Coefficients, double Bias) Initialize(IList<IProposition> propositions)
    {
        var coefficients = propositions.Select(p => EvaluateCoefficient(p, prop => prop.Function.CalculateCentroid(Axis.X) - prop.Function.EffectiveSupport.Midpoint.Get)).ToList();
        if (!Normalize)
            return (coefficients, 0);
        var widths = propositions.Select(p => EvaluateCoefficient(p, prop => prop.Function.EffectiveSupport.Width.Get));
        return ([..coefficients.Zip(widths, (coefficient, width) => coefficient / width)], 0);
    }
}