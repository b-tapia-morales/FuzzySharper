using Kernel.Number;
using Kernel.Operator.Negation.Abstractions;
using Shared.Enums;
using static System.Math;

namespace Kernel.Operator.Negation.Implementations.Canonical;

public class Negation : AbstractEnum<Negation, NegationType>, INegation
{
    public static readonly Negation Standard =
        new(nameof(Standard), "Standard", a => 1 - a.Value, (int) NegationType.Standard);

    public static readonly Negation RaisedCosine =
        new(nameof(RaisedCosine), "Raised Cosine", a => (1 / 2.0) * (1 + Cos(PI * a.Value)),
            (int) NegationType.RaisedCosine);

    private Negation(string name, string readableName, Func<FuzzyNumber, FuzzyNumber> function, int value) : base(name, value)
    {
        ReadableName = readableName;
        Function = function;
    }

    public override string ReadableName { get; }
    private Func<FuzzyNumber, FuzzyNumber> Function { get; }

    public FuzzyNumber Complement(FuzzyNumber x) => Function(x);
}