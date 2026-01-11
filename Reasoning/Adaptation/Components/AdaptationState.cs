using Kernel.Number;
using Reasoning.Adaptation.Aggregator.Abstractions;
using Shared.Options.Factory;
using Shared.Options.Implementations;

namespace Reasoning.Adaptation.Components;

/// <summary>
/// Represents the adaptive learning state of a rule by recording activation weights and computing an aggregated
/// activation weight using a configurable aggregation strategy.
/// </summary>
/// <remarks>
/// The adaptation state serves exclusively as a holder of learned activation data, and does not participate in
/// rule evaluation or inference.
/// </remarks>
public class AdaptationState
{
    /// <summary>
    /// Gets the most recent activation weight produced from the rule's last activation.
    /// </summary>
    public Option<FuzzyNumber> LatestWeight { get; private set; } = Option<FuzzyNumber>.None();
    /// <summary>
    /// Gets the aggregated activation weight derived from the learning history.
    /// </summary>
    /// <seealso cref="IWeightAggregator"/>
    public Option<FuzzyNumber> AggregatedWeight { get; private set; } = Option<FuzzyNumber>.None();
    /// <summary>
    /// Gets the history of activation weights, preserving insertion order.
    /// </summary>
    public LinkedList<AdaptationRecord> LearningHistory { get; private init; } = [];
    /// <summary>
    /// Gets the total number of times the rule has been activated.
    /// </summary>
    public uint FireCount { get; private set; }
    
    /// <summary>
    /// Recomputes the aggregated activation weight from the learning history using the specified aggregation strategy.
    /// </summary>
    /// <param name="maxHistorySize">
    /// The maximum number of historical activation weights to retain.
    /// </param>
    /// <param name="aggregator">
    /// The aggregation strategy used to compute the adapted activation weight.
    /// </param>
    /// <remarks>
    /// If the learning history's size exceeds <paramref name="maxHistorySize"/>, the oldest entries are discarded.
    /// </remarks>
    public void Recompute(uint maxHistorySize, IWeightAggregator aggregator)
    {
        ResizeHistory(maxHistorySize);
        RecomputeAggregatedWeight(aggregator);
    }

    /// <summary>
    /// Resets the adaptation state by clearing the learning history and all derived weights.
    /// </summary>
    public void Reset()
    {
        LatestWeight = Option<FuzzyNumber>.None();
        AggregatedWeight = Option<FuzzyNumber>.None();
        LearningHistory.Clear();
        FireCount = 0;
    }

    /// <summary>
    /// Creates a deep copy of the adaptation state, including its learning history and derived activation weights.
    /// </summary>
    /// <returns>
    /// A new <see cref="AdaptationState"/> instance with the same learning data.
    /// </returns>
    public AdaptationState DeepCopy() =>
        new()
        {
            LatestWeight = LatestWeight,
            AggregatedWeight = AggregatedWeight,
            LearningHistory = new LinkedList<AdaptationRecord>(LearningHistory),
            FireCount = FireCount
        };
    
    /// <summary>
    /// Records a new activation weight into the learning history.
    /// </summary>
    /// <param name="weight">The activation weight produced by the rule.</param>
    /// <param name="iteration">The iteration in which the activation occurred.</param>
    /// <remarks>
    /// This method is intended to be called exclusively by the inference pipeline.
    /// Learning policy is handled externally.
    /// </remarks>
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