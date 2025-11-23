using Kernel.Number;

namespace Reasoning.Adaptation.Components;

public record struct AdaptationRecord(FuzzyNumber Weight, uint Iteration, DateTimeOffset TimeStamp);