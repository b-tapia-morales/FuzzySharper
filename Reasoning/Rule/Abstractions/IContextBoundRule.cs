using Knowledge.Linguistic.Base.Abstractions;
using Reasoning.Proposition.Components;

namespace Reasoning.Rule.Abstractions;

public interface IContextBoundRule<out T> : IRule where T : class, IContextBoundRule<T>
{
    ILinguisticBase LinguisticBase { get; }

    T If(string variableName, string termName, HedgeType hedgeType = HedgeType.None);

    T IfNot(string variableName, string termName, HedgeType hedgeType = HedgeType.None);

    T And(string variableName, string termName, HedgeType hedgeType = HedgeType.None);

    T AndNot(string variableName, string termName, HedgeType hedgeType = HedgeType.None);

    T Or(string variableName, string termName, HedgeType hedgeType = HedgeType.None);

    T OrNot(string variableName, string termName, HedgeType hedgeType = HedgeType.None);
}