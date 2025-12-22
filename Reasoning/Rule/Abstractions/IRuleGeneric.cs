using Reasoning.Rule.Components;

namespace Reasoning.Rule.Abstractions;

public interface IRule<out T> : IRule where T : class, IRule<T>
{
    T DeepCopy(LifecycleMode lifecycleMode = LifecycleMode.New);
}