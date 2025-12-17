using Knowledge.Linguistic.Base.Abstractions;
using Reasoning.Proposition.Components;

namespace Reasoning.Rule.Abstractions;

public interface IContextFreeRule<out T> : IRule where T : class, IContextFreeRule<T>
{
    T If(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None);

    T IfNot(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None);

    T And(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None);

    T AndNot(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None);

    T Or(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None);

    T OrNot(ILinguisticBase linguisticBase, string variableName, string termName, HedgeType hedgeType = HedgeType.None);
}