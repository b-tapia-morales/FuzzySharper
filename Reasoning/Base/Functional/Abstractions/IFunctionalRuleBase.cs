using Kernel.Operator.Conorm.Abstractions;
using Kernel.Operator.Negation.Abstractions;
using Kernel.Operator.Norm.Abstractions;
using Knowledge.Memory.Abstractions;
using Reasoning.Base.Abstractions;
using Reasoning.Rule.Functional.Abstractions;
using Shared.Options.Implementations;

namespace Reasoning.Base.Functional.Abstractions;

public interface IFunctionalRuleBase : IRuleBase<IFunctionalRule>
{
    Option<double> AggregateOutput(string target, IWorkingMemory workingMemory, INegation negation, INorm norm, IConorm conorm);
}