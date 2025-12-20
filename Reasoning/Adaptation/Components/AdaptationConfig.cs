using Reasoning.Adaptation.Aggregator.Abstractions;
using Reasoning.Adaptation.RateScheduler.Abstractions;
using Shared.Options.Extensions;
using Shared.Options.Implementations;

namespace Reasoning.Adaptation.Components;

public class AdaptationConfig
{
    public const uint DefaultSize = 32;
    public const uint DefaultMinSize = 5;
    public const uint DefaultMaxSize = 200;

    public const uint DefaultMinSamples = DefaultMinSize;

    public uint MaxHistorySize { get; }
    public IWeightAggregator WeightAggregator { get; }
    public IRateScheduler RateScheduler { get; }
    public Option<uint> MinSamplesBeforeLearning { get; }

    public AdaptationConfig(AdaptationPolicy policy)
    {
        MaxHistorySize = policy.MaxHistorySize;
        WeightAggregator = policy.AggregatorPolicy.Create();
        RateScheduler = policy.RateSchedulerPolicy.Create();
        MinSamplesBeforeLearning = policy.MinSamplesBeforeLearning;
    }

    public AdaptationConfig(
        uint maxHistorySize,
        IWeightAggregator weightAggregator,
        IRateScheduler rateScheduler,
        Option<uint> minSamplesBeforeLearning)
    {
        ArgumentNullException.ThrowIfNull(weightAggregator);
        ArgumentNullException.ThrowIfNull(rateScheduler);

        CheckValue(nameof(MaxHistorySize), maxHistorySize, DefaultMinSize, DefaultMaxSize);
        minSamplesBeforeLearning.IfPresent(value => CheckValue(nameof(MinSamplesBeforeLearning), value, DefaultMinSamples, MaxHistorySize));

        MaxHistorySize = maxHistorySize;
        WeightAggregator = weightAggregator;
        RateScheduler = rateScheduler;
        MinSamplesBeforeLearning = minSamplesBeforeLearning;
    }

    private static void CheckValue(string name, uint value, uint lowerBound, uint upperBound)
    {
        if (value < lowerBound || value > upperBound)
            throw new ArgumentException($"Invalid {name} value. It must be in the range [{lowerBound}, {upperBound}] (Value provided was: {value})");
    }
}