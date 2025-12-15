using Kernel.Function.Abstractions;
using Kernel.Function.Implication.Abstractions;
using Kernel.Function.Implication.Larsen;
using Kernel.Function.Implication.Mamdani;
using Kernel.Number;

namespace Kernel.Function.Implication.Factory;

public class ImplicationFactory
{
    public static IImplicationEvaluator GetInstance(ImplicationMethod method, MeasurableFunction function, FuzzyNumber implicationParam, bool useUoD) =>
        method switch
        {
            ImplicationMethod.Mamdani => MamdaniEvaluator.BuildPlan(function, implicationParam, useUoD),
            ImplicationMethod.Larsen => LarsenEvaluator.BuildPlan(function, implicationParam, useUoD),
            _ => throw new ArgumentOutOfRangeException(nameof(method), method, null)
        };
}