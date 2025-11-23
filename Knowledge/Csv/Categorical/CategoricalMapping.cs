// ReSharper disable ClassNeverInstantiated.Global

using CsvHelper.Configuration;

namespace Knowledge.Csv.Categorical;

public sealed class CategoricalMapping : ClassMap<CategoricalFact>
{
    public CategoricalMapping()
    {
        Map(p => p.TypeName).Index(0);
        Map(p => p.ConstValue).Index(1);
    }
}