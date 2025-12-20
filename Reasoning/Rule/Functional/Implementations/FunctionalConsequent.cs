using Knowledge.Memory.Abstractions;
using Reasoning.Rule.Functional.Abstractions;
using Shared.Approx;
using Shared.Options.Factory;
using Shared.Options.Implementations;

// ReSharper disable NonReadonlyMemberInGetHashCode

namespace Reasoning.Rule.Functional.Implementations;

public class FunctionalConsequent : IFunctionalConsequent, IEquatable<FunctionalConsequent>
{
    private const int DecimalPlaces = (int) DoubleApproxExt.DefaultPrecision;

    public required string Target { get; init; }
    public required IReadOnlyDictionary<string, double> CoefficientDict { get; init; }
    public double Bias { get; set; } = 0D;
    public uint Arity { get; init; }

    public bool IsEvaluable(IWorkingMemory memory) =>
        CoefficientDict.All(pair => memory.Contains(pair.Key));

    public Option<double> Evaluate(IWorkingMemory memory)
    {
        if (!IsEvaluable(memory))
            return Option<double>.None();
        return CoefficientDict.Select(pair => pair.Value * memory.GetNumericFact(pair.Key).Get).Sum() + Bias;
    }

    public bool Equals(FunctionalConsequent? other)
    {
        if (ReferenceEquals(this, other))
            return true;
        if (other is null)
            return false;
        if (!(string.Equals(Target, other.Target, StringComparison.OrdinalIgnoreCase) && Arity == other.Arity && Normalize(Bias) == Normalize(other.Bias)))
            return false;
        foreach (var (key, value) in CoefficientDict)
        {
            if (!other.CoefficientDict.TryGetValue(key, out var otherValue))
                return false;
            if (Normalize(value) != Normalize(otherValue))
                return false;
        }

        return true;
    }

    public override bool Equals(object? obj) =>
        obj is FunctionalConsequent other && Equals(other);

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Target, StringComparer.OrdinalIgnoreCase);
        hash.Add(Normalize(Bias));
        hash.Add(Arity);
        foreach (var (key, value) in CoefficientDict.OrderBy(kvp => kvp.Key, StringComparer.OrdinalIgnoreCase))
        {
            hash.Add(key, StringComparer.OrdinalIgnoreCase);
            hash.Add(Normalize(value));
        }

        return hash.ToHashCode();
    }

    public override string ToString()
    {
        var coefficients = CoefficientDict.Count == 0
            ? string.Empty
            : $"{CoefficientDict.Select(pair => $"{pair.Value:4D} * {pair.Key} + ")}";
        return $"THEN {Target} = {coefficients}{Bias:4D}";
    }

    private static decimal Normalize(double value) =>
        decimal.Round(new decimal(value), DecimalPlaces, MidpointRounding.ToZero);
}