using System.Collections;
using Inference.Aggregator.Factory;
using Inference.Defuzzifier.Factory;
using Kernel.Function.Implication.Factory;
using Kernel.Operator.Family.Factory.Canonical;

namespace Inference.Tests.Inference;

public sealed class EngineConfigurationData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator() => Union.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private static readonly IEnumerable<object[]> Union =
        from x in Enum.GetValues<CanonicalType>()
        from y in Enum.GetValues<ImplicationMethod>()
        from z in Enum.GetValues<DefuzzificationMethod>()
        from w in Enum.GetValues<ValueAggregatorMethod>()
        select new object[]
        {
            x, y, z, w
        };
}

public sealed class TippingGridData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator() => Union.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private static readonly IEnumerable<object[]> Union =
        from x in new EngineConfigurationData()
        from y in Enumerable.Range(0, 11)
        from z in Enumerable.Range(0, 11)
        select (object[]) [..x, y, z];
}

public sealed class TippingCasesData : TheoryData<ImplicationMethod, double, double, double>
{
    public TippingCasesData()
    {
        Add(ImplicationMethod.Mamdani, 0d, 0d, 13d / 3);
        Add(ImplicationMethod.Mamdani, 5d, 5d, 38d / 3);
        Add(ImplicationMethod.Mamdani, 10d, 10d, 73d / 3);
        Add(ImplicationMethod.Mamdani, 5d, 2.5d, 381d / 38);
        Add(ImplicationMethod.Mamdani, 2.5d, 2.5d, 381d / 38);
        Add(ImplicationMethod.Mamdani, 7.5d, 7.5d, 5089d / 282);
        Add(ImplicationMethod.Mamdani, 2.5d, 7.5d, 1645d / 108);
        Add(ImplicationMethod.Mamdani, 0d, 10d, 355d / 21);

        Add(ImplicationMethod.Larsen, 0d, 0d, 13d / 3);
        Add(ImplicationMethod.Larsen, 5d, 5d, 38d / 3);
        Add(ImplicationMethod.Larsen, 10d, 10d, 73d / 3);
        Add(ImplicationMethod.Larsen, 2.5d, 2.5d, 373d / 38);
        Add(ImplicationMethod.Larsen, 7.5d, 7.5d, 852d / 47);
        Add(ImplicationMethod.Larsen, 2.5d, 7.5d, 545d / 36);
        Add(ImplicationMethod.Larsen, 0d, 10d, 355d / 21);
    }
}