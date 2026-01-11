using Kernel.Number;
using Kernel.Operator.Conorm.Abstractions;
using Kernel.Operator.Conorm.Implementations.Canonical;
using Kernel.Operator.Family.Abstractions;
using Kernel.Operator.Negation.Abstractions;
using Kernel.Operator.Negation.Implementations.Canonical;
using Kernel.Operator.Norm.Abstractions;
using Kernel.Operator.Norm.Implementations.Canonical;
using Knowledge.Memory.Abstractions;
using Reasoning.Adaptation.Aggregator.Abstractions;
using Reasoning.Adaptation.Components;
using Reasoning.Proposition.Abstractions;
using Reasoning.Proposition.Components;
using Reasoning.Proposition.Implementations;
using Reasoning.Rule.Components;
using Reasoning.Rule.Functional.Abstractions;
using Reasoning.Rule.FuzzySet.Abstractions;
using Reasoning.Rule.FuzzySet.Comparer.Implementations.Deterministic;
using Shared.Options.Implementations;
using Shared.Primitives.Implementation;

namespace Reasoning.Rule.Abstractions;

/// <summary>
/// <para>
/// Defines a propositional rule in a fuzzy expert system, composed of a premise and a consequent, and expressed
/// in the general form <c>IF &lt;premise&gt; THEN &lt;consequent&gt;</c>. The premise may consist of one or more
/// <see cref="FuzzyProposition">fuzzy</see> or <see cref="BooleanProposition{T}">boolean</see> propositions combined
/// through logical connectives.
/// </para>
/// The consequent is represented by an <see cref="IRuleOutput"/> and determines the inference model of the rule.
/// A fuzzy set consequent yields an <see cref="IFuzzySetRule"/> (Mamdani-style), whereas a functional consequent
/// yields an <see cref="IFunctionalRule"/> (Sugeno-style).
/// <para>
/// A rule is only executable once it is finalized by appending a consequent. Once finalized, the rule
/// is evaluated by computing the activation weight of its premise with respect to a
/// <see cref="IWorkingMemory">working memory</see> using configurable fuzzy operators. This activation weight is then
/// applied during inference when processing the consequent.
/// </para>
/// Rules support adaptive behavior through an internal <see cref="AdaptationState"/> and may encode expert knowledge
/// through an optional <see cref="Priority">priority level</see> or <see cref="CertaintyFactor">certainty factor</see>.
/// <remarks>
/// <para>
/// A rule is considered structurally valid if it represents a single <c>IF</c>–<c>THEN</c> statement consisting of
/// a premise and a consequent.
/// </para>
/// The premise is formed from exactly one initial proposition (the <c>IF</c> part of the rule) and zero or more
/// additional propositions combined through logical connectives (e.g., <c>AND</c>, <c>OR</c>).
/// <para>
/// The rule must contain exactly one consequent (the <c>THEN</c> part of the rule). Once the consequent is appended,
/// the rule is considered finalized and no further propositions can be added.
/// </para>
/// A rule without a premise or without a consequent is incomplete and cannot be executed. Only finalized rules
/// participate in inference.
/// </remarks>
/// </summary>
public interface IRule
{
    /// <summary>
    /// Gets or sets the initial proposition of the rule premise (the <c>IF</c> part of the rule).
    /// </summary>
    IProposition? Conditional { get; internal set; }

    /// <summary>
    /// Gets the collection of propositions that extend the rule premise through logical connectives (e.g., AND, OR).
    /// </summary>
    ICollection<IProposition> Connectives { get; }

    /// <summary>
    /// Gets or sets the consequent of the rule (the <c>THEN</c> part of the rule), represented by an
    /// <see cref="IRuleOutput"/>.
    /// </summary>
    IRuleOutput? Consequent { get; internal set; }

    /// <summary>
    /// Gets or sets a value indicating whether the rule has been finalized by successfully appending a consequent,
    /// and is therefore a valid and structurally complete rule.
    /// </summary>
    bool IsFinalized { get; internal set; }

    /// <summary>
    /// Gets the optional priority level assigned to the rule, representing its relative importance among competing
    /// rules during inference.
    /// </summary>
    Option<RulePriority> Priority { get; }

    /// <summary>
    /// Gets the optional certainty factor associated with the rule, representing the level of confidence an expert has
    /// in its correctness and suitability for use among competing rules during inference.
    /// </summary>
    Option<double> CertaintyFactor { get; }

    /// <summary>
    /// Gets the timestamp indicating when the rule instance was created.
    /// </summary>
    DateTimeOffset CreationTime { get; }

    /// <summary>
    /// Gets the adaptation state associated with the rule, which maintains historical
    /// activation data and learned weights used for adaptive and reinforcement-based tuning.
    /// </summary>
    AdaptationState AdaptationState { get; }

    /// <summary>
    /// Provides a view of the propositions that compose the rule premise in the order in which they were defined.
    /// </summary>
    IReadOnlyList<IProposition> Premise { get; }

    /// <summary>
    /// Gets the total number of propositions that compose the rule premise.
    /// </summary>
    int PremiseLength { get; }

    /// <summary>
    /// Provides a view of all uniquely identifiable variables referenced by the propositions of the rule premise.
    /// <see cref="string"/> identifiers correspond to fuzzy propositions, while <see cref="Type"/> identifiers
    /// correspond to boolean propositions.
    /// </summary>
    IReadOnlyList<StringOrType> PremiseVariables { get; }

    /// <summary>
    /// Provides a view of the mapping between each uniquely identifiable variable used in the rule premise and
    /// the list of propositions that use that variable.
    /// </summary>
    IReadOnlyDictionary<StringOrType, IReadOnlyList<IProposition>> PremiseDict { get; }

    /// <summary>
    /// Determines whether the rule premise can be evaluated with respect to the working. A rule is considered evaluable
    /// if and only if every proposition in its premise is <see cref="IProposition.IsEvaluable">evaluable</see>.
    /// </summary>
    /// <param name="memory">The working memory used to resolve whether each proposition is evaluable.</param>
    /// <returns>
    /// <see langword="true"/> if all propositions in the premise are evaluable; otherwise, <see langword="false"/>.
    /// </returns>
    bool IsPremiseEvaluable(IWorkingMemory memory);

    /// <summary>
    /// Determines whether the rule premise contains a proposition that uses the specified
    /// identifier.
    /// </summary>
    /// <param name="identifier">
    /// The identifier to probe for usage within the rule premise.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the premise contains at least one proposition that uses the specified identifier;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// A <see cref="string"/> identifier corresponds to <see cref="FuzzyProposition">fuzzy propositions</see>, while
    /// a <see cref="Type"/> identifier corresponds to <see cref="BooleanProposition{T}">boolean propositions</see>.
    /// </remarks>
    bool PremiseContains(StringOrType identifier);

    /// <summary>
    /// Determines whether the <see cref="Consequent">rule consequent</see> uses the specified identifier.
    /// </summary>
    /// <param name="identifier">
    /// The identifier to probe for usage within the rule consequent.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the consequent uses the specified identifier; otherwise, <see langword="false"/>.
    /// </returns>
    bool ConsequentContains(string identifier);

    /// <summary>
    /// <para>
    /// Applies the unary operators associated with each <see cref="IProposition">proposition</see> in the rule premise
    /// and evaluates them with respect to the working memory, producing a sequence of fuzzy numbers representing
    /// the individual truth values of each proposition.
    /// </para>
    /// Each proposition is evaluated in the following order:
    /// <list type="number">
    /// <item>
    /// The proposition is evaluated with respect to the working memory to get its truth value as a fuzzy number.
    /// </item>
    /// <item>
    /// If the proposition is <see cref="FuzzyProposition">fuzzy</see> and defines a
    /// <see cref="LinguisticHedge">linguistic hedge</see>, the hedge is applied.
    /// </item>
    /// <item>
    /// The literal operator defined by the proposition (e.g., <see cref="Literal.Is">affirmation</see> or
    /// <see cref="Literal.IsNot">negation</see>) is applied.
    /// </item>
    /// </list>
    /// </summary>
    /// <param name="memory">The working memory to evaluate the truth value of the propositions.</param>
    /// <param name="negation">The negation operator to apply to negated propositions.</param>
    /// <returns>
    /// A sequence of fuzzy numbers corresponding to the evaluated propositions of the premise.
    /// </returns>
    /// <remarks>
    /// If the premise is not <see cref="IsPremiseEvaluable">evaluable</see>, then the rule is considered unapplicable
    /// and an empty sequence is returned.
    /// </remarks>
    IEnumerable<FuzzyNumber> ApplyUnaryOperators(IWorkingMemory memory, INegation negation);

    /// <summary>
    /// Applies the unary operators associated with each <see cref="IProposition">proposition</see> in the rule premise
    /// and evaluates them with respect to the working memory, producing a sequence of fuzzy numbers representing
    /// the individual truth values of each proposition.
    /// </summary>
    /// <param name="memory">The working memory to evaluate the truth value of the propositions.</param>
    /// <returns>
    /// A sequence of fuzzy numbers corresponding to the evaluated propositions of the premise.
    /// </returns>
    /// <remarks>
    /// This overload is functionally equivalent to <see cref="ApplyUnaryOperators(IWorkingMemory, INegation)"/> with
    /// the negation set to the <see cref="Negation.Standard">standard operator</see> by default.
    /// </remarks>
    IEnumerable<FuzzyNumber> ApplyUnaryOperators(IWorkingMemory memory);

    /// <summary>
    /// Evaluates the rule premise against the working memory by computing its activation weight using the provided
    /// fuzzy logic operators.
    /// </summary>
    /// <param name="memory">The working memory used to resolve proposition values.</param>
    /// <param name="negation">The negation operator used to evaluate negated propositions.</param>
    /// <param name="norm">The norm operator used to aggregate conjunctive connectives.</param>
    /// <param name="conorm">The conorm operator used to aggregate disjunctive connectives.</param>
    /// <returns>
    /// An <see cref="Option{T}"/> containing the activation weight if
    /// <see cref="IsPremiseEvaluable">the premise is evaluable</see>; otherwise, an empty option.
    /// </returns>
    /// <remarks>
    /// The activation weight is computed by:
    /// <list type="number">
    /// <item>
    /// Applying the unary operators of each proposition to obtain their individual truth values (see:
    /// <see cref="ApplyUnaryOperators(IWorkingMemory, INegation)">ApplyUnaryOperators</see>).
    /// </item>
    /// <item>
    /// Aggregating those truth values from left to right according to the
    /// <see cref="IProposition.Connective">logical connectives defined by each proposition</see>.
    /// The first pair of adjacent values is aggregated into a single value using the fuzzy operator corresponding to
    /// the proposition’s connective (norm for <see cref="Connective.And">And</see>, conorm for
    /// <see cref="Connective.Or">Or</see>). This resulting truth value is then aggregated with the next one until
    /// the whole sequence has been processed.
    /// The <see cref="Connective.If">If</see> connective serves only as a premise delimiter and does not participate
    /// in aggregation.
    /// </item>
    /// </list>
    /// </remarks>
    Option<FuzzyNumber> EvaluatePremiseWeight(IWorkingMemory memory,
        INegation negation, INorm norm, IConorm conorm);

    /// <summary>
    /// Evaluates the rule premise against the working memory by computing its activation weight using the provided
    /// fuzzy logic operators.
    /// </summary>
    /// <param name="memory">The working memory used to resolve proposition values.</param>
    /// <param name="operatorFamily">The operator family supplying the fuzzy logic operators.</param>
    /// <returns>
    /// An <see cref="Option{T}"/> containing the activation weight if
    /// <see cref="IsPremiseEvaluable">the premise is evaluable</see>; otherwise, an empty option.
    /// </returns>
    /// <remarks>
    /// This overload is functionally equivalent to calling
    /// <see cref="EvaluatePremiseWeight(IWorkingMemory, INegation, INorm, IConorm)"/>
    /// with the negation, norm, and conorm operators supplied by the operator family.
    /// </remarks>
    Option<FuzzyNumber> EvaluatePremiseWeight(IWorkingMemory memory, IOperatorFamily operatorFamily);

    /// <summary>
    /// Evaluates the rule premise against the working memory by computing its activation weight using the provided
    /// fuzzy logic operators.
    /// </summary>
    /// <param name="memory">The working memory used to resolve proposition values.</param>
    /// <returns>
    /// An <see cref="Option{T}"/> containing the activation weight if
    /// <see cref="IsPremiseEvaluable">the premise is evaluable</see>; otherwise, an empty option.
    /// </returns>
    /// <remarks>
    /// This overload is functionally equivalent to calling
    /// <see cref="EvaluatePremiseWeight(IWorkingMemory, INegation, INorm, IConorm)"/>
    /// with the <see cref="Negation.Standard">standard operator</see> for negation, the
    /// <see cref="Norm.Minimum">minimum operator</see> for norm and the
    /// <see cref="Conorm.Maximum">maximum operator</see> for conorm.
    /// </remarks>
    Option<FuzzyNumber> EvaluatePremiseWeight(IWorkingMemory memory);

    /// <summary>
    /// Recomputes the rule adaptation state using the specified aggregation strategy.
    /// </summary>
    /// <param name="maxHistorySize">
    /// The maximum number of historical activation weights to retain after recomputation.
    /// </param>
    /// <param name="aggregator">
    /// The aggregation strategy used to recompute the adapted activation weights.
    /// </param>
    /// <remarks>
    /// This method is a convenience wrapper over <see cref="AdaptationState.Recompute"/> and delegates the operation
    /// to the rule's <see cref="AdaptationState"/>.
    /// </remarks>
    void RecomputeAdaptation(uint maxHistorySize, IWeightAggregator aggregator);

    /// <summary>
    /// Resets the rule adaptation state.
    /// </summary>
    /// <remarks>
    /// This method is a convenience wrapper over <see cref="AdaptationState.Reset"/> and delegates the operation
    /// to the rule's <see cref="AdaptationState"/>.
    /// </remarks>
    void ResetAdaptation();

    /// <summary>
    /// Creates a deep copy of the rule using the specified lifecycle mode.
    /// </summary>
    /// <param name="mode">
    /// The lifecycle mode applied to the copied rule instance.
    /// </param>
    /// <returns>
    /// A new <see cref="IRule"/> instance that is structurally identical to the current one.
    /// </returns>
    IRule DeepCopy(LifecycleMode mode = LifecycleMode.NewInstance);
}