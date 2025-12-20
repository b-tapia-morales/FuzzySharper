using Kernel.Number;
using Reasoning.Adaptation.Aggregator.Abstractions;
using Reasoning.Adaptation.Components;
using Shared.Options.Factory;
using Shared.Options.Implementations;

namespace Reasoning.Adaptation.Aggregator.Implementations;

public class FrequencyWeightedAverage : IWeightAggregator
{
    public const uint DefaultDecimalPlaces = 1U;
    
    private readonly uint _decimalPlaces;

    public FrequencyWeightedAverage(uint decimalPlaces)
    {
        if (decimalPlaces < 1)
            throw new ArgumentException("Minimum amount of decimal places is 1.");

        _decimalPlaces = decimalPlaces;
    }

    public FrequencyWeightedAverage() : this(1)
    {
    }

    public Option<FuzzyNumber> Aggregate(IReadOnlyCollection<AdaptationRecord> records)
    {
        if (records.Count == 0)
            return Option<FuzzyNumber>.None();

        var dict = records
            .Select(e => Math.Round(e.Weight.Value, (int) _decimalPlaces))
            .GroupBy(e => e)
            .ToDictionary(e => e.Key, e => e.Count());
        return FuzzyNumber.Of(dict.Sum(p => p.Key * p.Value) / dict.Values.Sum());
    }
}