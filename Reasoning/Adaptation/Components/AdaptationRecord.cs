using Kernel.Number;

namespace Reasoning.Adaptation.Components;

public record struct AdaptationRecord(FuzzyNumber Weight, uint Iteration)
{
    private DateTimeOffset TimeStamp { get; } = DateTimeOffset.UtcNow;
}