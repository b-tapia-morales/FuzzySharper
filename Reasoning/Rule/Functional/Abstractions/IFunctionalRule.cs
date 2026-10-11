using Kernel.Operator.Conorm.Abstractions;
using Kernel.Operator.Conorm.Implementations.Canonical;
using Kernel.Operator.Family.Abstractions;
using Kernel.Operator.Negation.Abstractions;
using Kernel.Operator.Negation.Implementations.Canonical;
using Kernel.Operator.Norm.Abstractions;
using Kernel.Operator.Norm.Implementations.Canonical;
using Knowledge.Memory.Abstractions;
using Reasoning.Rule.Abstractions;
using Reasoning.Rule.Exceptions;
using Reasoning.Rule.Functional.Components;
using Shared.Options.Implementations;

namespace Reasoning.Rule.Functional.Abstractions;

/// <summary>
/// <para>
/// Represents a Sugeno-style rule whose <see cref="IRule.Consequent">consequent</see> is
/// <see cref="IFunctionalConsequent">functional</see>, producing an output by evaluating
/// an affine function of the crisp values associated with the distinct linguistic variables
/// used by fuzzy propositions in the premise.
/// </para>
/// <para>
/// The consequent function has one input and one coefficient for each distinct linguistic
/// variable used by the fuzzy propositions in the premise, even when multiple propositions
/// use the same variable. Boolean propositions contribute to premise evaluation but do not
/// supply inputs to the consequent function.
/// </para>
/// </summary>
/// <example>
/// <code>
/// IF Room Temperature IS Cool OR Room Temperature IS Comfortable THEN Heating power: z₁ = a₁ ⋅ x + c₁
/// </code>
/// <code>
/// IF Room Temperature IS Cold OR Outside Temperature IS Freezing THEN Heating power: z₂ = a₂ ⋅ x + b₂ ⋅ y + c₂
/// </code>
/// Here, <c>x</c> and <c>y</c> are the crisp values of <c>Room Temperature</c> and
/// <c>Outside Temperature</c>, respectively, while <c>a</c> and <c>b</c> are their corresponding
/// coefficients and <c>c</c> is the constant bias.
/// In the first example, both propositions participate in premise evaluation, but the consequent
/// function has only one input because both propositions use the same linguistic variable,
/// <c>Room Temperature</c>.
/// </example>
public interface IFunctionalRule : IRule
{
    /// <summary>
    /// Gets or sets the learning state of the rule's consequent coefficients, indicating whether they are
    /// initialized, learnable, or frozen.
    /// </summary>
    FunctionalRuleState State { get; set; }

    /// <summary>
    /// Computes the unweighted consequent value <c>z</c> by evaluating the consequent function
    /// with the crisp input values obtained from the working memory.
    /// </summary>
    /// <param name="memory">The working memory used to evaluate the consequent value.</param>
    /// <returns>
    /// An <see cref="Option{T}"/> containing the unweighted consequent value <c>z</c> if the
    /// consequent is <see cref="IFunctionalConsequent.IsEvaluable">evaluable</see>;
    /// otherwise, an empty option.
    /// </returns>
    /// <exception cref="InvalidRuleException">The rule is missing its premise or consequent.</exception>
    Option<double> EvaluateConsequentValue(IWorkingMemory memory);

    /// <summary>
    /// Computes the rule output <c>wᵢzᵢ = w ⋅ z</c> by evaluating the premise to obtain its
    /// activation weight <c>w</c> using the specified fuzzy operators, then multiplying it by
    /// the consequent value <c>z</c> obtained from <see cref="EvaluateConsequentValue"/>.
    /// </summary>
    /// <param name="memory">The working memory supplying facts for premise and consequent evaluation.</param>
    /// <param name="negation">The negation operator used to evaluate negated propositions in the premise.</param>
    /// <param name="norm">The t-norm operator used to aggregate conjunctions in the premise.</param>
    /// <param name="conorm">The t-conorm operator used to aggregate disjunctions in the premise.</param>
    /// <returns>
    /// An <see cref="Option{T}"/> containing the rule output if both the premise and consequent are
    /// evaluable; otherwise, an empty option.
    /// </returns>
    /// <exception cref="InvalidRuleException">The rule is missing its premise or consequent.</exception>
    Option<double> EvaluateRuleOutput(IWorkingMemory memory, INegation negation, INorm norm, IConorm conorm);

    /// <summary>
    /// Computes the rule output <c>wᵢzᵢ = w ⋅ z</c> by calculating the premise activation weight <c>w</c> using the
    /// specified operator family and multiplying it by the consequent value <c>z</c>.
    /// </summary>
    /// <param name="memory">The working memory supplying facts for premise and consequent evaluation.</param>
    /// <param name="operatorFamily">The operator family supplying the negation, norm, and conorm operators.</param>
    /// <returns>
    /// An <see cref="Option{T}"/> containing the rule output if both the premise and consequent are evaluable;
    /// otherwise, an empty option.
    /// </returns>
    /// <remarks>
    /// This overload is functionally equivalent to calling
    /// <see cref="EvaluateRuleOutput(IWorkingMemory, INegation, INorm, IConorm)"/>
    /// with the negation, norm, and conorm operators supplied by <paramref name="operatorFamily"/>.
    /// </remarks>
    /// <exception cref="InvalidRuleException">The rule is missing its premise or consequent.</exception>
    Option<double> EvaluateRuleOutput(IWorkingMemory memory, IOperatorFamily operatorFamily);

    /// <summary>
    /// Computes the rule output <c>wᵢzᵢ = w ⋅ z</c> by calculating the premise activation weight <c>w</c> using the
    /// default fuzzy operators and multiplying it by the consequent value <c>z</c>.
    /// </summary>
    /// <param name="memory">The working memory supplying facts for premise and consequent evaluation.</param>
    /// <returns>
    /// An <see cref="Option{T}"/> containing the rule output if both the premise and consequent are evaluable;
    /// otherwise, an empty option.
    /// </returns>
    /// <remarks>
    /// This overload is functionally equivalent to calling
    /// <see cref="EvaluateRuleOutput(IWorkingMemory, INegation, INorm, IConorm)"/>
    /// with the <see cref="Negation.Standard">standard operator</see> for negation, the
    /// <see cref="Norm.Minimum">minimum operator</see> for t-norm and the
    /// <see cref="Conorm.Maximum">maximum operator</see> for t-conorm.
    /// </remarks>
    /// <exception cref="InvalidRuleException">The rule is missing its premise or consequent.</exception>
    Option<double> EvaluateRuleOutput(IWorkingMemory memory);
}
