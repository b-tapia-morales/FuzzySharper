using Kernel.Number;
using Reasoning.Adaptation.Aggregator.Abstractions;
using Shared.Options.Factory;
using Shared.Options.Implementations;

namespace Reasoning.Adaptation.Components;

public class AdaptationState
{
    public Option<FuzzyNumber> LatestWeight { get; private set; } = Option<FuzzyNumber>.None();
    public Option<FuzzyNumber> AggregatedWeight { get; private set; } = Option<FuzzyNumber>.None();
    public LinkedList<AdaptationRecord> LearningHistory { get; private init; } = [];
    public uint FireCount { get; private set; }
    
    public void Recompute(uint maxHistorySize, IWeightAggregator aggregator)
    {
        ResizeHistory(maxHistorySize);
        RecomputeAggregatedWeight(aggregator);
    }

    public void Reset()
    {
        LatestWeight = Option<FuzzyNumber>.None();
        AggregatedWeight = Option<FuzzyNumber>.None();
        LearningHistory.Clear();
        FireCount = 0;
    }

    public AdaptationState DeepCopy() =>
        new()
        {
            LatestWeight = LatestWeight,
            AggregatedWeight = AggregatedWeight,
            LearningHistory = new LinkedList<AdaptationRecord>(LearningHistory),
            FireCount = FireCount
        };
    
    internal void Append(FuzzyNumber weight, uint iteration)
    {
        LatestWeight = weight;
        LearningHistory.AddLast(new AdaptationRecord(weight, iteration, DateTimeOffset.Now));
        FireCount++;
    }

    private void ResizeHistory(uint maxHistorySize)
    {
        while (LearningHistory.Count > maxHistorySize)
            LearningHistory.RemoveFirst();
    }

    private void RecomputeAggregatedWeight(IWeightAggregator aggregator) =>
        AggregatedWeight = aggregator.Aggregate(LearningHistory);
}