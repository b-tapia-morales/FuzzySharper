namespace Reasoning.Rule.Functional.Components;

/// <summary>
/// Defines the learning state of a functional rule's consequent coefficients and bias,
/// indicating whether they have been initialized, are undergoing learning, or are frozen
/// after learning.
/// </summary>
public enum FunctionalRuleState
{
    /// <summary>
    /// The consequent's coefficients and bias have been initialized with their initial values.
    /// </summary>
    Initialized,
    /// <summary>
    /// The consequent's coefficients and bias are currently undergoing learning.
    /// </summary>
    Learnable,
    /// <summary>
    /// The consequent's coefficients and bias are frozen and are intended to represent their final learned values.
    /// </summary>
    Frozen
}