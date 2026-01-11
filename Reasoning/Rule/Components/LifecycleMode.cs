using Reasoning.Adaptation.Components;
using Reasoning.Rule.Abstractions;

namespace Reasoning.Rule.Components;

/// <summary>
/// Defines the lifecycle strategy used when creating a deep copy of a rule by determining how learning state and
/// creation metadata are transferred from the source rule to its copy.
/// </summary>
public enum LifecycleMode
{
    /// <summary>
    /// Creates a copy by treating it as an exact replica of the source rule.
    /// The <see cref="IRule.AdaptationState">adaptation state</see> and
    /// <see cref="IRule.CreationTime">creation time</see> are both preserved.
    /// </summary>
    PreserveState,
    /// <summary>
    /// Creates a copy by treating it as a newly instantiated rule.
    /// The source's structure and <see cref="IRule.AdaptationState">adaptation state</see> are preserved,
    /// but a new <see cref="IRule.CreationTime">creation time</see> is assigned.
    /// </summary>
    NewInstance,
    /// <summary>
    /// Creates a copy by treating it as an untrained rule.
    /// The source's structure is preserved, but its <see cref="IRule.AdaptationState">adaptation state</see> is
    /// <see cref="AdaptationState.Reset()">reset</see>.
    /// </summary>
    ResetLearning
}