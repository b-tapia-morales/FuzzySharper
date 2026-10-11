using Reasoning.Base.Abstractions;
using Reasoning.Rule.Abstractions;

namespace Inference.Engine.Abstractions;

public interface IInferenceEngine<T> : IInferenceEngine 
    where T : class, IRule
{
    IRuleBase<T> RuleBase { get; }

    void ResetLearning();
    
    void UpdateLearning();
}