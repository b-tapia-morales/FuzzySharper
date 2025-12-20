using Kernel.Operator.Conorm.Implementations.Parameterized;
using Kernel.Operator.Family.Abstractions;
using Kernel.Operator.Family.Implementations;
using Kernel.Operator.Negation.Implementations.Parameterized;
using Kernel.Operator.Norm.Implementations.Parameterized;
using Kernel.Operator.Residuum.Implementations.Parameterized;

namespace Kernel.Operator.Family.Factory.Parameterized;

public static class ParameterizedFactory
{
    public static IOperatorFamily UseFamily(ParameterizedType type, double gamma, double alpha, double beta, double omega) =>
        type switch
        {
            ParameterizedType.Hamacher => new OperatorFamily(
                new PNegation(PNegator.Yager, gamma),
                new PNorm(PUnitor.Hamacher, alpha),
                new PConorm(PIntersector.Hamacher, beta),
                new PResiduum(PResiduumOperator.PseudoLukasiewicz1, omega)),
            ParameterizedType.Sugeno => new OperatorFamily(
                new PNegation(PNegator.Sugeno, gamma),
                new PNorm(PUnitor.SugenoWeber, alpha),
                new PConorm(PIntersector.SugenoWeber, beta),
                new PResiduum(PResiduumOperator.PseudoLukasiewicz2, omega)),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
}