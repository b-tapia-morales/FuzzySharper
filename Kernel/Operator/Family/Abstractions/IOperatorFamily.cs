using Kernel.Number;
using Kernel.Operator.Conorm.Abstractions;
using Kernel.Operator.Negation.Abstractions;
using Kernel.Operator.Norm.Abstractions;
using Kernel.Operator.Residuum.Abstractions;

namespace Kernel.Operator.Family.Abstractions;

public interface IOperatorFamily : INegation, INorm, IConorm, IResiduum
{
    INegation Negation { get; set; }
    INorm Norm { get; set; }
    IConorm Conorm { get; set; }
    IResiduum Residuum { get; set; }

    FuzzyNumber INegation.Complement(FuzzyNumber x) =>
        Negation.Complement(x);

    FuzzyNumber INorm.Intersection(FuzzyNumber x, FuzzyNumber y) =>
        Norm.Intersection(x, y);

    FuzzyNumber IConorm.Union(FuzzyNumber x, FuzzyNumber y) =>
        Conorm.Union(x, y);

    FuzzyNumber IResiduum.Implication(FuzzyNumber x, FuzzyNumber y) =>
        Residuum.Implication(x, y);

    IOperatorFamily DeepCopy();
}