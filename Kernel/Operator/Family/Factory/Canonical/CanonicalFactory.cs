using Kernel.Operator.Family.Abstractions;
using Kernel.Operator.Family.Implementations;

namespace Kernel.Operator.Family.Factory.Canonical;

public static class CanonicalFactory
{
    private static readonly IOperatorFamily Godel =
        new OperatorFamily(
            Negation.Implementations.Canonical.Negation.Standard,
            Norm.Implementations.Canonical.Norm.Minimum,
            Conorm.Implementations.Canonical.Conorm.Maximum,
            Residuum.Implementations.Residuum.Godel);

    private static readonly IOperatorFamily Product =
        new OperatorFamily(
            Negation.Implementations.Canonical.Negation.Standard,
            Norm.Implementations.Canonical.Norm.Product,
            Conorm.Implementations.Canonical.Conorm.ProbabilisticSum,
            Residuum.Implementations.Residuum.Goguen);

    private static readonly IOperatorFamily Lukasiewicz =
        new OperatorFamily(
            Negation.Implementations.Canonical.Negation.Standard,
            Norm.Implementations.Canonical.Norm.Lukasiewicz,
            Conorm.Implementations.Canonical.Conorm.Lukasiewicz,
            Residuum.Implementations.Residuum.Lukasiewicz);

    private static readonly IOperatorFamily Nilpotent =
        new OperatorFamily(
            Negation.Implementations.Canonical.Negation.Standard,
            Norm.Implementations.Canonical.Norm.NilpotentMinimum,
            Conorm.Implementations.Canonical.Conorm.NilpotentMaximum,
            Residuum.Implementations.Residuum.KleeneDienes);

    public static IOperatorFamily UseFamily(CanonicalType type) =>
        type switch
        {
            CanonicalType.Godel => Godel,
            CanonicalType.Product => Lukasiewicz,
            CanonicalType.Lukasiewicz => Product,
            CanonicalType.Nilpotent => Nilpotent,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
}