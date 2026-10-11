# AGENTS.md

## Project Overview

FuzzySharper is a C# fuzzy logic and fuzzy expert-system library with a fluent API.

The library supports two principal rule families:

- **Mamdani-style rules**, whose consequents are fuzzy propositions or fuzzy sets.
- **Sugeno-style rules**, whose consequents are functional expressions.

Rules are structured as a single `IF ... THEN ...` statement composed of a premise and exactly one consequent.

Premises may contain fuzzy propositions, boolean propositions, or both.

Treat the current source code as authoritative. Existing documentation, design notes, examples, or historical discussions may describe older implementations.

---

## Development Philosophy

Prefer simple, low-coupling designs over speculative abstractions.

Do not introduce additional layers, types, indirection, or state unless they solve a concrete problem in the current design.

Prefer designs whose contracts are evident from their APIs and data structures. Avoid requiring callers to know incidental implementation details or undocumented conventions in order to use an API correctly.

Keep shared abstractions generic only over the types they actually require. Avoid propagating generic parameters or constraints into unrelated layers solely because a lower-level implementation needs them.

Prefer the smallest coherent change that solves the requested problem. Do not opportunistically refactor unrelated code.

If a local implementation issue reveals a meaningful architectural or API problem, address the underlying design rather than preserving it solely to minimize the diff.

---

## Architecture

Keep root-level architectural guidance focused on responsibilities and important boundaries. More detailed subsystem behavior may be moved to dedicated architecture documents when necessary.

### Rule Construction

Rule-building APIs construct rules and propositions.

When constructing fuzzy propositions, rule-building methods use a linguistic base as the resolution context for linguistic variables and terms.

A context-bound rule has an associated linguistic base that acts as its default resolution context.

A context-free rule receives the linguistic base explicitly through its rule-building methods.

Linguistic-base resolution occurs during rule construction. The resulting propositions evaluate already-constructed statements and do not retain responsibility for resolving linguistic bases.

### Propositions

A proposition represents an atomic statement used in a rule premise.

Its relevant components include:

- an identifier;
- a label;
- a literal;
- a connective.

Fuzzy propositions use string identifiers representing linguistic variables.

Boolean propositions use enum `Type` identifiers and categorical enum values.

A proposition evaluates its own truth value with respect to supplied data. It does not perform premise aggregation or inference.

### Rules

A structurally valid rule represents one `IF ... THEN ...` statement:

- exactly one initial `IF` proposition begins the premise;
- zero or more `AND` or `OR` propositions may follow;
- exactly one consequent completes the rule;
- no premise proposition may be appended after the consequent.

`IF` and `THEN` are structural delimiters.

`AND` and `OR` participate in premise aggregation through the configured norm and conorm, respectively.

Consequents cannot be negated.

### Rule Evaluation

Keep proposition evaluation separate from premise aggregation.

For fuzzy propositions, unary evaluation follows the conceptual order:

```text
membership degree → linguistic hedge → literal
```

Premise evaluation then aggregates proposition truth values from left to right.

During aggregation:

- `And` uses the configured norm;
- `Or` uses the configured conorm;
- `If` does not participate mathematically in aggregation.

A premise is evaluable if and only if all propositions required by it are evaluable with respect to the supplied working memory.

### Rule Families

The consequent determines the principal rule family.

**Fuzzy-set rules** use a fuzzy consequent expressing a linguistic term for an output variable. Their evaluation combines the premise activation weight with the evaluated consequent through fuzzy implication.

**Functional rules** use a functional consequent whose crisp output is computed from numeric inputs associated with fuzzy premise variables. Their rule output combines that consequent value with the premise activation weight.

Both rule families share the common premise structure and premise-evaluation behavior defined by `IRule`.

Keep family-specific behavior in the corresponding rule abstractions unless it is genuinely common to both rule families.

### Inference

Inference coordinates rule evaluation and the derivation of outputs.

Keep rule construction, proposition evaluation, inference coordination, and learning/adaptation responsibilities separate unless a design change explicitly calls for moving one of those responsibilities.

### Adaptation and Learning

`AdaptationState` stores learned activation data associated with a rule.

It may contain information such as:

- the latest activation weight;
- an aggregated activation weight;
- learning history;
- fire count.

`AdaptationState` does not:

- evaluate rules;
- perform inference;
- decide learning policy.

Learning and adaptation policy belong to the inference system or other components responsible for coordinating learning behavior.

Components participating in inference may record learning observations when appropriate, but policy decisions such as aggregation strategy or retained history size should remain outside the state container.

---

## Architectural Changes

Substantial architectural changes should normally be discussed before implementation.

Examples include:

- moving responsibility between major abstractions;
- introducing a new architectural layer;
- changing ownership of state or policy;
- significantly changing inheritance or generic relationships;
- replacing an established public API pattern;
- introducing a new cross-cutting abstraction.

When the user asks for a preview, stub, sketch, proposed API, or architectural discussion, treat the work as exploratory.

Do not apply the proposed change to the repository until the user explicitly approves it.

Discussion, refinement, or acceptance of individual ideas does not by itself constitute approval to implement the full change.

Minor implementation decisions that do not materially affect architecture do not require this process.

---

## Public API Design

Public APIs should reflect the semantics of the domain rather than internal implementation convenience.

Prefer contracts whose meaning can be understood from the signature and associated documentation without requiring knowledge of internal storage or incidental implementation behavior.

Do not add overloads, interfaces, or public members solely to simplify an internal implementation.

When changing a public contract:

1. inspect related interfaces and implementations;
2. identify affected overloads and derived types;
3. update existing documentation where it would otherwise become incorrect;
4. update or add focused tests when appropriate and practical for the changed behavior.

Avoid expanding the scope into unrelated API cleanup unless the change is necessary for consistency or correctness.

---

## Documentation Philosophy

Documentation should be concrete, technically precise, and difficult to misinterpret.

Prefer wording that clearly identifies:

- what abstraction is being described;
- what it does;
- what data or state it operates on;
- which conditions affect its behavior;
- what result or state transition occurs.

Conciseness is desirable, but not when it creates ambiguity.

Additional detail is justified when it prevents a plausible misunderstanding of the contract or domain semantics.

Avoid unnecessary detail that merely restates the implementation without improving understanding.

Do not use promotional or self-congratulatory language.

Avoid rhetorical constructions that add emphasis without adding technical meaning.

Be especially careful not to assign responsibility or ownership to the wrong abstraction.

The exact wording does not need to follow a fixed vocabulary as long as it is concrete, accurate, and consistent with the architecture.

---

## Documentation Timing

Major documentation work should generally happen after the relevant building blocks and public APIs are sufficiently stable.

Do not perform a large documentation pass for an abstraction that is still being actively redesigned unless explicitly requested.

This does not mean existing documentation may remain knowingly incorrect.

When implementation changes invalidate existing documentation, update or flag the affected documentation as part of the change.

Distinguish between:

- **maintaining documentation accuracy**, which should happen alongside behavioral changes;
- **performing a full documentation pass**, which should generally wait until the design is stable.

---

## Documentation Workflow

When asked to create substantial documentation from scratch or propose meaningful documentation changes, use a proposal-first workflow.

1. Inspect the relevant code and existing documentation.
2. Produce a draft.
3. Explain any significant structural or semantic changes being proposed.
4. Allow the user to review and provide feedback.
5. Revise the draft as needed.
6. Modify repository files only after explicit approval, such as a clear "good to go".

Do not treat discussion of the draft as permission to edit the repository.

For small documentation corrections, limit changes to the affected passage. When asked to review, identify the exact location and provide a drop-in replacement. When explicitly asked to implement the correction, apply it directly.

If a documentation problem requires a broader structural rewrite, explain why before presenting the larger replacement.

---

## XML Documentation

Public APIs should normally have XML documentation once their contracts are stable enough to document reliably.

### Style

- Describe observable behavior in plain language. Avoid explaining internal mechanisms unless callers need that information to use the API correctly.
- Use documentation from established APIs as a stylistic guideline when relevant, while describing the actual behavior of this implementation.
- Include parameter and type-parameter tags, a returns tag for members that return values, and exception tags for documented failure conditions, even when the summary already covers that information.
- Keep these tags brief when the summary provides the full explanation.
- Use remarks only for additional information that affects how callers use or understand the API.

### Summaries

`<summary>` should contain the essential conceptual meaning and behavior needed to understand the member.

Do not use a vague one-line summary and move the real definition into `<remarks>`.

If understanding a method requires knowing its high-level procedure, include that procedure in the summary when practical.

### Remarks

Use `<remarks>` for supplemental information such as:

- non-obvious caveats;
- edge cases;
- capability boundaries;
- equivalence between overloads;
- behavior not apparent from the signature.

Do not use remarks as the primary location for essential semantics.

### Parameters

Describe the role of the parameter in the operation rather than merely restating its type or name.

When a parameter participates in a non-obvious mapping or contract, document that relationship explicitly.

### Returns

Describe both the returned value and any meaningful condition under which no value is returned.

For fluent rule-building methods, describe the resulting state of the rule rather than merely saying that the current instance is returned.

### Enum Members

Provide concise member-level documentation when the meaning of an enum constant is useful in IDE hover information.

Keep member documentation brief unless the state has behavior that is not obvious from its name.

---

## Documentation Review Style

When reviewing existing documentation:

- preserve correct wording when possible;
- identify the exact sentence, paragraph, or section that needs modification;
- explain the ambiguity or semantic issue;
- provide an exact replacement;
- focus grammar feedback on wording that materially affects readability or meaning;
- avoid rewriting the entire block unless the conceptual structure itself is flawed.

When several small corrections are sufficient, prefer targeted corrections over a full rewrite.

When a larger rewrite is necessary, explain the structural reason first.

---

## Review Workflow

Keep iterative reviews focused on the material currently under discussion.

When feedback affects only part of a document, proposal, or design:

- show the affected sections or passages;
- preserve previously accepted material unless the new change also affects it;
- do not reproduce the complete document after every minor revision.

Provide a consolidated full revision when explicitly requested.

For short artifacts, this restriction may be relaxed when reproducing the complete text is clearer and does not materially clutter the discussion.

Do not silently revise previously accepted sections while presenting an unrelated localized change.

---

## Proposal vs. Implementation

Treat requests for drafts, previews, stubs, sketches, reviews, or design discussions as proposal work unless the user explicitly asks for immediate implementation.

Examples include:

- "show me how this could look";
- "give me a rough draft";
- "create a stub";
- "what would you change?";
- "review this design";
- "propose an API";
- "let's discuss the architecture";
- documentation drafts intended for review.

For proposal work:

- inspect enough of the surrounding code to make the proposal realistic;
- present the proposed design or text;
- identify important tradeoffs;
- do not modify repository files.

Once explicit approval is given, apply the agreed changes without reopening settled design questions unless implementation reveals a new material issue.

---

## Testing and Verification

Automated test coverage is currently incomplete and should not be assumed to provide comprehensive behavioral validation.

Existing tests include some broad integration and configuration coverage, but the strength and specificity of their assertions varies. A passing test suite therefore does not by itself establish that all affected behavior is correct.

When making implementation changes:

- inspect the relevant interfaces, implementations, and nearby callers before changing behavior;
- run relevant existing tests when they are available;
- add focused tests when they are reasonably scoped to the change, particularly for bug fixes, edge cases, and newly introduced behavior;
- prefer assertions that verify the specific behavioral contract under test rather than only checking that execution succeeds or produces a broadly valid result;
- treat existing integration or smoke tests as useful regression coverage, but do not assume that passing them proves behavior they do not explicitly assert;
- do not expand a localized task into a large test-suite retrofit unless explicitly requested;
- identify important behavior that remains untested when the lack of coverage materially affects confidence in the change.

If testing a localized change would first require building substantial new test infrastructure, raise that issue rather than silently expanding the scope.

Do not claim that tests passed unless they were actually executed.

Do not claim that a behavior has been verified solely because the existing suite passes.

---

## Scope Discipline

Keep each change focused on the requested problem.

Do not use a small task as an opportunity for unrelated cleanup.

If a related issue materially affects correctness, API semantics, or the requested implementation, call it out explicitly.

When uncertain whether a larger refactor is warranted, prefer presenting the architectural issue and a proposed direction before modifying the broader design.
