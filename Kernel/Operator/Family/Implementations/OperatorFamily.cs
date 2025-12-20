using Kernel.Operator.Conorm.Abstractions;
using Kernel.Operator.Family.Abstractions;
using Kernel.Operator.Negation.Abstractions;
using Kernel.Operator.Norm.Abstractions;
using Kernel.Operator.Residuum.Abstractions;

namespace Kernel.Operator.Family.Implementations;

public class OperatorFamily(INegation negation, INorm norm, IConorm conorm, IResiduum residuum) : IOperatorFamily
{
    public INegation Negation { get; set; } = negation;
    public INorm Norm { get; set; } = norm;
    public IConorm Conorm { get; set; } = conorm;
    public IResiduum Residuum { get; set; } = residuum;

    public IOperatorFamily DeepCopy() =>
        new OperatorFamily(Negation, Norm, Conorm, Residuum);
}