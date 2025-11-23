using Kernel.Number;
using Reasoning.Adaptation.Aggregator.Abstractions;
using Shared.Options.Factory;
using Shared.Options.Implementations;

namespace Reasoning.Adaptation.Components;

public class AdaptationState
{
    public Option<FuzzyNumber> LatestWeight { get; private set; } = OptionFactory.None<FuzzyNumber>();
    public Option<FuzzyNumber> AggregatedWeight { get; private set; } = OptionFactory.None<FuzzyNumber>();
    public LinkedList<AdaptationRecord> LearningHistory { get; } = [];
    public uint FireCount { get; private set; }

    public void Reset()
    {
        LatestWeight = OptionFactory.None<FuzzyNumber>();
        AggregatedWeight = OptionFactory.None<FuzzyNumber>();
        LearningHistory.Clear();
        FireCount = 0;
    }

    public void Append(FuzzyNumber weight, uint iteration)
    {
        LatestWeight = weight;
        LearningHistory.AddLast(new AdaptationRecord(weight, iteration, DateTimeOffset.Now));
        FireCount++;
    }

    public void Update(uint maxHistorySize, IWeightAggregator aggregator)
    {
        ResizeHistory(maxHistorySize);
        RecomputeAggregatedWeight(aggregator);
    }

    private void ResizeHistory(uint maxHistorySize)
    {
        while (LearningHistory.Count > maxHistorySize)
            LearningHistory.RemoveFirst();
    }

    private void RecomputeAggregatedWeight(IWeightAggregator aggregator) =>
        AggregatedWeight = aggregator.Aggregate(LearningHistory);
}