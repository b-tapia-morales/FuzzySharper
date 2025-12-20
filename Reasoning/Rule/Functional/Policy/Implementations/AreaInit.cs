using Kernel.Function.Extensions;
using Reasoning.Proposition.Abstractions;
using Reasoning.Rule.Functional.Policy.Abstractions;

namespace Reasoning.Rule.Functional.Policy.Implementations;

public class AreaInit : BaseCoefficientInitPolicy
{
    protected bool Normalized { get; init; }

    public override IEnumerable<double> Initialize(IReadOnlyList<IProposition> premise)
    {
        var areas = premise.Select(p => EvaluateCoefficient(p, prop => prop.Function.CalculateArea())).ToList();
        if (!Normalized)
            return areas;
        var maxArea = areas.Max();
        return areas.Select(a => a / maxArea);
    }
}