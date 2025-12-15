using Kernel.Function.Extensions;
using Reasoning.Proposition.Abstractions;
using Reasoning.Rule.Functional.Policy.Abstractions;
using static Reasoning.Rule.Functional.Policy.Abstractions.ICoefficientInitPolicy;

namespace Reasoning.Rule.Functional.Policy.Implementations;

public class AreaInit : ICoefficientInitPolicy
{
    protected bool Normalized { get; init; }

    public (IList<double> Coefficients, double Bias) Initialize(IList<IProposition> propositions)
    {
        var areas = propositions.Select(p => EvaluateCoefficient(p, prop => prop.Function.CalculateArea())).ToList();
        if (!Normalized)
            return (areas, 0);
        var maxArea = areas.Max();
        return (areas.Select(a => a / maxArea).ToList(), 0);
    }
}