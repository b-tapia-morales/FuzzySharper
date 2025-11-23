using System.Collections;
using System.Globalization;
using Inference.Defuzzifier.Factory;
using Inference.Engine.Builder;
using Kernel.Function.Extensions;
using Kernel.Operator.Family.Factory.Canonical;
using Knowledge.Memory.Implementations;
using Microsoft.Extensions.DependencyInjection;
using Reasoning.Base.Abstractions;
using Shared.Options.Factory;
using Xunit.Abstractions;

namespace Inference.Tests.Inference;

public class EngineTests(ITestOutputHelper outputHelper)
{
    private static readonly ServiceProvider ServiceProvider = EngineProvider.ConfigureProvider();
    private static readonly IRuleBase Rules = ServiceProvider.GetService<IRuleBase>()!;

    [Theory]
    [ClassData(typeof(MassiveUnionData))]
    public void DefuzzifiedValueIsInRange(CanonicalType canonicalType, ImplicationMethod implicationMethod, DefuzzificationMethod defuzzificationMethod, double foodRating, double serviceRating)
    {
        var canonicalFamily = CanonicalFactory.UseFamily(canonicalType);
        var workingMemory = WorkingMemory.Create(("food quality", foodRating), ("service quality", serviceRating));
        var ruleBase = Rules.DeepCopy();
        var engine = EngineBuilder
            .Create()
            .WithRuleBase(ruleBase)
            .WithWorkingMemory(workingMemory)
            .WithCanonicalFamily(canonicalType)
            .WithImplication(implicationMethod)
            .WithDefuzzification(defuzzificationMethod)
            .Build();
        var tuples = engine.RuleBase.ProductionRules.Select(r => (
            Rule: r,
            Weight: r.EvaluatePremiseWeight(workingMemory, canonicalFamily)));
        outputHelper.WriteLine(string.Join(Environment.NewLine, tuples.Select(t => $"{t.Rule} : {t.Weight}")));
        var success = engine.Defuzzify("Tip").IsSomeVal(out var value);
        outputHelper.WriteLine(value.ToString(CultureInfo.InvariantCulture));
        Assert.True(success);
        Assert.InRange(value, 0, 35);
    }
}

file class MassiveUnionData : IEnumerable<object[]>
{
    private static readonly IList<int> Ratings = Enumerable.Range(1, 10).ToList();
    private static readonly IReadOnlyList<CanonicalType> OperatorFamilies = Enum.GetValues<CanonicalType>();
    private static readonly IReadOnlyList<ImplicationMethod> ImplicationMethods = Enum.GetValues<ImplicationMethod>();
    private static readonly IReadOnlyList<DefuzzificationMethod> DefuzzificationMethods = Enum.GetValues<DefuzzificationMethod>();

    private static readonly IEnumerable<object[]> Union =
        from x in OperatorFamilies
        from y in ImplicationMethods
        from z in DefuzzificationMethods
        from a in Ratings
        from b in Ratings
        select new object[] {x, y, z, (double) a, (double) b};

    public IEnumerator<object[]> GetEnumerator() => Union.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}