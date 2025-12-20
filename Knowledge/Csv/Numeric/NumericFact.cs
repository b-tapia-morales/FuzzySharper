// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Knowledge.Csv.Numeric;

public sealed class NumericFact
{
    public string Key { get; set; } = null!;
    public double Value { get; set; } = 0;

    public override string ToString() => $"{Key} - {Value}";
}