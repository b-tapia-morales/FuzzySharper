using Kernel.Function.Abstractions;
using Kernel.Number;
using Reasoning.Rule.FuzzySet.Abstractions;

namespace Inference.Aggregator.Components;

public record FiringStrength(IFuzzySetRule Rule, IMembershipFunction Function, FuzzyNumber Weight);