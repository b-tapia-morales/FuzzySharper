// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Knowledge.Csv.Categorical;

public sealed class CategoricalFact
{
    public string TypeName { get; set; } = null!;
    public string ConstValue { get; set; } = null!;

    public override string ToString() => $"{TypeName} - {ConstValue}";
}