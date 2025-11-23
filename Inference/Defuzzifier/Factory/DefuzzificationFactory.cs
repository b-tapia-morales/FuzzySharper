using Inference.Defuzzifier.Abstractions;
using Inference.Defuzzifier.Implementations;

namespace Inference.Defuzzifier.Factory;

public static class DefuzzificationFactory
{
    private static readonly IDefuzzifier FirstOfMaxima = new FirstOfMaxima();
    private static readonly IDefuzzifier LastOfMaxima = new LastOfMaxima();
    private static readonly IDefuzzifier MeanOfMaxima = new MeanOfMaxima();
    private static readonly IDefuzzifier CenterOfSums = new CenterOfSums();
    private static readonly IDefuzzifier CenterOfLargestArea = new CenterOfLargestArea();

    public static IDefuzzifier GetInstance(DefuzzificationMethod method) =>
        method switch
        {
            DefuzzificationMethod.FirstOfMaxima => FirstOfMaxima,
            DefuzzificationMethod.LastOfMaxima => LastOfMaxima,
            DefuzzificationMethod.MeanOfMaxima => MeanOfMaxima,
            DefuzzificationMethod.CenterOfSums => CenterOfSums,
            DefuzzificationMethod.CenterOfLargestArea => CenterOfLargestArea,
            _ => throw new ArgumentOutOfRangeException(nameof(method), method, null)
        };
}