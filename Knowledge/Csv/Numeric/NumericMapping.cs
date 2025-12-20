// ReSharper disable ClassNeverInstantiated.Global

using CsvHelper.Configuration;

namespace Knowledge.Csv.Numeric;

public sealed class NumericMapping : ClassMap<NumericFact>
{
    public NumericMapping()
    {
        Map(p => p.Key).Index(0);
        Map(p => p.Value).Index(1);
    }
}