using Reasoning.Rule.Functional.Policy.Abstractions;
using Reasoning.Rule.Functional.Policy.Implementations;

namespace Reasoning.Rule.Functional.Policy.Factory;

public static class CoefficientInitFactory
{
    private static readonly ICoefficientInitPolicy ZeroInit = new ZeroInit();
    private static readonly ICoefficientInitPolicy AreaInit = new AreaInit();
    private static readonly ICoefficientInitPolicy NormalizedAreaInit = new NormalizedAreaInit();
    private static readonly ICoefficientInitPolicy CentroidInit = new CentroidInit();
    private static readonly ICoefficientInitPolicy NormalizedCentroidInit = new NormalizedCentroidInit();
    private static readonly ICoefficientInitPolicy MeanDevInit = new MeanDevInit();
    private static readonly ICoefficientInitPolicy NormalizedMeanDeviationInit = new NormalizedMeanDevInit();
    private static readonly ICoefficientInitPolicy SupportWidthInit = new SupportWidthInit();
    private static readonly ICoefficientInitPolicy SupportMidpointInit = new SupportMidpointInit();

    public static ICoefficientInitPolicy GetInstance(CoefficientInitMethod method) => method switch
    {
        CoefficientInitMethod.Zero => ZeroInit,
        CoefficientInitMethod.Random => RandomInit.Default,
        CoefficientInitMethod.Area => AreaInit,
        CoefficientInitMethod.NormalizedArea => NormalizedAreaInit,
        CoefficientInitMethod.Centroid => CentroidInit,
        CoefficientInitMethod.NormalizedCentroid => NormalizedCentroidInit,
        CoefficientInitMethod.MeanDeviation => MeanDevInit,
        CoefficientInitMethod.NormalizedMeanDeviation => NormalizedMeanDeviationInit,
        CoefficientInitMethod.SupportWidth => SupportWidthInit,
        CoefficientInitMethod.SupportMidpoint => SupportMidpointInit,
        CoefficientInitMethod.PolarityAware => PolarityAwareInit.Default,
        _ => throw new ArgumentOutOfRangeException(nameof(method), method, null)
    };
}