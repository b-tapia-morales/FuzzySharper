using Reasoning.Adaptation.Aggregator.Factory;
using Reasoning.Adaptation.Aggregator.Policy.Abstractions;
using Reasoning.Adaptation.Aggregator.Policy.Factory;
using Reasoning.Adaptation.RateScheduler.Factory;
using Reasoning.Adaptation.RateScheduler.Policy.Abstractions;
using Reasoning.Adaptation.RateScheduler.Policy.Factory;
using Shared.Options.Implementations;

namespace Reasoning.Adaptation.Components;

public readonly record struct AdaptationPolicy(
    uint MaxHistorySize,
    IWeightAggregatorPolicy AggregatorPolicy,
    IRateSchedulerPolicy RateSchedulerPolicy,
    Option<uint> MinSamplesBeforeLearning)
{
    public AdaptationPolicy(
        uint MaxHistorySize,
        WeightAggregationMethod aggregationMethod,
        RateSchedulerMethod schedulerMethod,
        Option<uint> MinSamplesBeforeLearning) :
        this(
            MaxHistorySize,
            WeightAggregationPolicyFactory.GetInstance(aggregationMethod),
            RateSchedulerPolicyFactory.GetInstance(schedulerMethod),
            MinSamplesBeforeLearning)
    {
    }
}