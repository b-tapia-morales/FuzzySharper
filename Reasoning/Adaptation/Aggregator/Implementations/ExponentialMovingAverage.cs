using Kernel.Number;
using Reasoning.Adaptation.Aggregator.Abstractions;
using Reasoning.Adaptation.Components;
using Shared.Approx;
using Shared.Options.Factory;
using Shared.Options.Implementations;

namespace Reasoning.Adaptation.Aggregator.Implementations;

public class ExponentialMovingAverage : IWeightAggregator
{
    public const double DefaultAlpha = 0.25;
    
    private readonly double _alpha;

    public ExponentialMovingAverage(double alpha)
    {
        if (alpha.IsRoughlyLesserThan(0) || alpha.IsRoughlyGreaterThan(1))
            throw new ArgumentOutOfRangeException(nameof(alpha),
                "Alpha must be in the range (0, 1].");

        _alpha = double.Clamp(alpha, 0, 1);
    }

    public ExponentialMovingAverage() : this(DefaultAlpha)
    {
    }

    public Option<FuzzyNumber> Aggregate(IReadOnlyCollection<AdaptationRecord> records)
    {
        if (records.Count == 0)
            return OptionFactory.None<FuzzyNumber>();

        var ema = records.First().Weight.Value;

        return FuzzyNumber.Of(
            records
                .Skip(1)
                .Select(record => record.Weight.Value)
                .Aggregate(ema, (current, next) => _alpha * next + (1.0 - _alpha) * current)
        );
    }
}