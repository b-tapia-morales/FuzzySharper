using Kernel.Number;
using Shared.Approx;
using Shared.Enums;

namespace Kernel.Operator.Norm.Implementations.Parameterized;

public class PUnitor : AbstractEnum<PUnitor, PUnitorType>
{
    public static readonly PUnitor Hamacher =
        new(nameof(Hamacher), "Hamacher", (x, y, alpha) => (x * y) / (alpha + (1 - alpha) * (x + y + x * y)),
            (int) PUnitorType.Hamacher);

    public static readonly PUnitor SugenoWeber =
        new(nameof(SugenoWeber), "Sugeno-Weber", (x, y, alpha) => Math.Max((x + y - 1 + alpha * x * y) / (1 + alpha), 0),
            (int) PUnitorType.SugenoWeber);

    public static readonly PUnitor SchweizerSklar =
        new(nameof(SchweizerSklar), "Schweizer-Sklar", (x, y, p) =>
        {
            // p = -∞
            if (double.IsNegativeInfinity(p) || p.IsRoughlyMinValue())
                return Canonical.Norm.Minimum.Intersection(x, y);
            // p = 0
            if (p.IsRoughlyZero())
                return Canonical.Norm.Product.Intersection(x, y);
            // p = +∞
            if (double.IsPositiveInfinity(p) || p.IsRoughlyMaxValue())
                return Canonical.Norm.Drastic.Intersection(x, y);
            var a = x.Value;
            var b = y.Value;
            // -∞ < p < +0
            // +0 < p < +∞
            return p < 0 ? Math.Pow(Math.Pow(a, p) + Math.Pow(b, p) - 1, 1 / p) : Math.Pow(Math.Max(0, Math.Pow(a, p) + Math.Pow(b, p) - 1), 1 / p);
        }, (int) PUnitorType.SchweizerSklar);

    public static readonly PUnitor Frank =
        new(nameof(Frank), "Frank", (x, y, p) =>
        {
            // p = +∞
            if (double.IsPositiveInfinity(p) || p.IsRoughlyMaxValue())
                return Canonical.Norm.Lukasiewicz.Intersection(x, y);
            // p = 0
            if (p.IsRoughlyZero())
                return Canonical.Norm.Minimum.Intersection(x, y);
            // p = 1
            if (p.IsRoughlyOne())
                return Canonical.Norm.Product.Intersection(x, y);
            var a = x.Value;
            var b = y.Value;
            return Math.Log(1 + ((Math.Pow(p, a) - 1) * (Math.Pow(p, b) - 1)) / (p - 1), p);
        }, (int) PUnitorType.Frank);

    public static readonly PUnitor AczelAlsina =
        new(nameof(AczelAlsina), "Aczél-Alsina", (x, y, p) =>
        {
            // p = +∞
            if (double.IsPositiveInfinity(p) || p.IsRoughlyMaxValue())
                return Canonical.Norm.Minimum.Intersection(x, y);
            // p = 0
            if (p.IsRoughlyZero())
                return Canonical.Norm.Drastic.Intersection(x, y);
            var a = x.Value;
            var b = y.Value;
            // 0 <= p < +∞
            return Math.Exp(-Math.Pow(Math.Pow(Math.Abs(-Math.Log(a)), p) + Math.Pow(Math.Abs(-Math.Log(b)), p), 1 / p));

        }, (int) PUnitorType.AczelAlsina);

    public static readonly PUnitor Dombi =
        new(nameof(Dombi), "Dombi", (x, y, p) =>
        {
            // p = +∞
            if (double.IsPositiveInfinity(p) || p.IsRoughlyMaxValue())
                return Canonical.Norm.Minimum.Intersection(x, y);
            // p = 0
            if (p.IsRoughlyZero())
                return Canonical.Norm.Drastic.Intersection(x, y);
            var a = x.Value;
            var b = y.Value;
            // x = 0 ∨ y = 0
            if (a.IsRoughlyZero() || b.IsRoughlyZero())
                return 0;
            return 1 / (1 + Math.Pow(Math.Pow((1 - a) / a, p) + Math.Pow((1 - b) / b, p), 1 / p));
        }, (int) PUnitorType.Dombi);

    private PUnitor(string name, string readableName, Func<FuzzyNumber, FuzzyNumber, double, FuzzyNumber> function, int value) : base(name, value)
    {
        ReadableName = readableName;
        Function = function;
    }

    public override string ReadableName { get; }
    public Func<FuzzyNumber, FuzzyNumber, double, FuzzyNumber> Function { get; }
}