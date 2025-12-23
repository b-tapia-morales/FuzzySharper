# Fuzzy Sharper

## Overview

Fuzzy Sharper is a library that allows you to create your own Fuzzy Expert System.

## About the project

**FuzzySharper** is a fuzzy logic inference library built around explicit composition of inference components.

It exposes rules, propositions, operators, implication methods, among others, as independent parts that can be combined
to form an inference engine. The library avoids enforcing a single inference model and instead relies on common
abstractions to support different rule styles and evaluation strategies under a shared structure.

Configuration is primarily done through composition, allowing components to be exchanged or extended without modifying
the surrounding system. The goal is to keep inference behavior explicit and predictable, while remaining flexible in how
inference systems are assembled.

## Prerequisites

The library uses .NET SDK 10.

The following open-source libraries are used in this project:

- [SmartEnum](https://github.com/ardalis/SmartEnum)
- [Math.Net Numerics](https://github.com/mathnet/mathnet-numerics)
- [CsvHelper](https://github.com/JoshClose/CsvHelper)
- [OneOf](https://github.com/mcintyre321/OneOf/)

## Folder structure

The project is organized with each major subsystem isolated into its own top-level directory.
Test projects mirror the production structure to preserve locality and intent.

```
+---Inference
¦   +---Aggregator
¦   +---Defuzzifier
¦   +---Engine
¦   +---Tree
+---Inference.Tests
+---Kernel
¦   +---Function
¦   +---Number
¦   +---Operator
+---Kernel.Tests
+---Knowledge
¦   +---Csv
¦   +---FactStorage
¦   +---Linguistic
¦   +---Memory
+---Knowledge.Tests
+---Reasoning
¦   +---Adaptation
¦   +---Base
¦   +---Proposition
¦   +---Rule
+---Reasoning.Tests
+---Shared
+---Shared.Tests
+---Utils
```

### High-level overview

- **Inference**: Contains the execution layer of the fuzzy inference process, including aggregation, defuzzification,
  and inference engine orchestration.
- **Kernel**: Defines the mathematical and logical primitives used throughout the system, such as definitions of a fuzzy
  number, membership functions, and logical operators.
- **Knowledge**: Responsible for fact modeling, working memory management, the linguistic base, linguistic variables,
  and CSV-based data ingestion.
- **Reasoning**: Encapsulates rule modeling, propositions, and the structures used to record and represent learned
  information produced during inference.
- **Shared**: Cross-cutting abstractions and utilities shared across multiple subsystems.
- **Utils**: General-purpose helpers that do not belong to a specific domain module.
- ***.Tests**: Each test project mirrors its corresponding module and contains unit and behavioral tests.

## Usage

### Membership Functions

A **Membership function** defines how a numeric (crisp) value *x* maps to a *Membership degree* *μ(x)* in a *Fuzzy set*.

Within a **Linguistic variable**, membership functions are used to express the meaning of *Linguistic terms* over a
given *Universe of discourse*.
Each linguistic term is associated with exactly one membership function, which determines how input values are
interpreted during inference.

In *FuzzySharper*, Membership functions are treated as first-class objects.
A function encapsulates both its shape and the operations required to evaluate membership degrees, compute
characteristic metrics (such as centroid or support), and participate in *Inference* and *Defuzzification*. Once
created, membership functions are immutable, ensuring consistent behavior throughout an inference run.

The library currently provides built-in support for the following membership function types:

- **Triangular**
- **Trapezoidal**
- **Left-Open Trapezoidal**
- **Right-Open Trapezoidal**
- **Gaussian**
- **Generalized Bell**
- **Logistic**
- **Singleton**

All membership functions conform to a common abstraction, allowing custom function types to be introduced without
affecting the surrounding inference logic.

### Linguistic Variables

A **Linguistic variable** is a variable whose values are words or sentences in a natural language.
Each of these values — referred to as *Linguistic terms* — is defined by a *Membership function* over a shared *Universe
of discourse*.

A linguistic variable acts as a container that groups a set of named linguistic terms and associates each term with
exactly one membership function.
During inference, crisp input values are evaluated against these membership functions to obtain *Membership degrees*.

A linguistic variable can be declared as follows:

```csharp
var water = LinguisticVariable.Create("Water")
    .AddTrapezoidalFunction("Cold", -10, -10, 5, 15)
    .AddTriangularFunction("Mild", 10, 20, 30)
    .AddTrapezoidalFunction("Hot", 25, 35, 50, 50);

var humidity = LinguisticVariable.Create("Humidity")
    .AddTrapezoidalFunction("Dry", 0, 0, 20, 40)
    .AddTriangularFunction("Normal", 30, 50, 70)
    .AddTrapezoidalFunction("Humid", 60, 80, 100, 100);
```

Lines 1 and 6 declare linguistic variables named `Temperature` and `Humidity`, respectively.
The next lines define the set of linguistic terms that belong to this variable.
Each term is identified by a unique name and is intrinsically associated with a membership function — this association
is commonly referred to as a *Semantic mapping*.

The numerical parameters supplied when defining a term describe the shape of its membership function and are interpreted
according to the selected function type.
For example, for `Water`, `Cold` represents a *Trapezoid* with vertices at `(-10, -10)`, `(-10, 1)`, `(5, 1)`, and
`(15, 0)`,
while `Mild` represents a *Triangle* with vertices at `(10, 0)`, `(20, 1)`, and `(30, 0)`.

While the exact values produced by a membership function depend on its definition,
all membership functions follow the same basic idea: they are evaluated over a shared domain, and the degree of
membership is determined by the function’s shape.
This way, the value being evaluated and the strength of its membership are treated as related but distinct aspects.

### Linguistic Base

A **Linguistic base** is a collection of linguistic variables.

The linguistic base stores linguistic variables by name and provides a single point of access to them.
It contains no inference logic and does not evaluate rules or propositions.

Linguistic variables are typically added to the linguistic base during system setup. Other components access the
linguistic base to retrieve linguistic variables and their associated membership functions when needed.

A linguistic base can be created and populated explicitly:

```csharp
var linguisticBase = LinguisticBase
    .Create(water, humidity);
```

### Working Memory

A Working Memory represents the current knowledge about a *Domain* in the form of *Facts*.
These facts describe the current state of the domain that may change over time.

Facts are stored using identifiers that correspond to domain concepts—such as linguistic variables or enumerated
states—whose current values characterize the state of the domain.
By updating the working memory, the user provides the information that rules can later reason about.

#### Creating a Working Memory

A working memory can be created empty and populated incrementally:

```csharp
var memory = WorkingMemory.Create();
```

#### Adding Facts

Numeric facts describe measured values and are identified by a string key that matches the name of the linguistic
variable they describe.

```csharp
memory.AddNumericFact("Room Temperature", 21.5);
memory.AddNumericFact("Outside Temperature", -2.0);
```

Alternatively, numeric facts can be added in batches using key-value tuples:

```csharp
memory.AddNumericFacts(("Room Temperature", 21.5), ("Outside Temperature", -2.0));
```

Categorical facts describe discrete states and are expressed using enum values:

```csharp
memory.AddCategoricalFact(Occupancy.Occupied);
memory.AddCategoricalFact(Window.Closed);
```

Categorical facts can also be added in batches:

```csharp
memory.AddCategoricalFacts(Occupancy.Ocupied, Window.Closed);
```

#### Retrieving Facts

Facts can be retrieved directly from the working memory:

```csharp
var temperature = memory.GetNumericFact("Room Temperature");
var occupancy   = memory.GetCategoricalFact<Occupancy>();
```

If a fact is not present, the returned value reflects its absence.

#### Load from a CSV file

Facts can also be loaded from CSV files to initialize a working memory with a predefined domain state:

```csharp
memory.ReadNumericFactsFromFile("data/inputs.csv");
memory.ReadCategoricalFactsFromFile("data/states.csv");
```

### Rules

A rule expresses a conditional relationship between a set of propositions (the *Premise*) and a *Consequent*.
Rules are evaluated during inference using the linguistic variables, membership functions, and facts available to the
system.

#### Bounded and Unbounded Rules

Rules can be created either unbounded or bounded to a linguistic base.

An **Unbounded rule** does not carry any implicit reference to a linguistic base.
When appending fuzzy propositions to such a rule, the linguistic base must be provided explicitly so that linguistic
variables and linguistic terms can be resolved.

```csharp
UnboundedFuzzySetRule.Create()
    .If(linguisticBase, "Temperature", "Mild");
```

A **Bounded rule** is created with an associated linguistic base.
Once bound, fuzzy propositions can be appended without explicitly specifying where linguistic variables are retrieved
from.

```csharp
BoundedFuzzySetRule.Create(linguisticBase)
    .If("Temperature", "Mild");
```

#### Propositions

A proposition represents a condition evaluated as part of a rule’s premise.
Propositions can be either fuzzy or boolean.

##### Fuzzy Proposition

A *Fuzzy proposition* references a *Linguistic variable* and one of its *Linguistic terms*.
During inference, it is evaluated by computing the *Membership degree* of the current input value in its corresponding
membership function.

```csharp
.If("Humidity", "Humid")
```

##### Boolean Propositions

Rules may also include *Boolean propositions*, which represent categorical facts.
Boolean propositions are defined using enums, where the *Type* and the *Constant value* of an <code>enum</code> play a
role similar to a linguistic variable and term, respectively.

```csharp
.And(Ventilation.On)
```

Boolean propositions are independent of linguistic bases and can be freely combined with fuzzy propositions within the
same rule.

#### Fuzzy Set Rules (Mamdani-style)

A **Fuzzy set rule** uses fuzzy sets in both its *Premise* and its *Consequent*.
The consequent specifies a linguistic term that will be activated when the rule fires.

```csharp
BoundedFuzzySetRule.Create(linguisticBase)
    .If("Temperature", "Cold")
    .And("Humidity", "Humid")
    .And(Ventilation.Off)
    .Then("Heating Power", "High");
```

The previous rule can be read as:

    If the Temperature is Cold and the Humidity is Humid, and the Ventilation is Off, then Heating power is/should be set to High.

The previous rule can be read entirely as a natural language statement.
When evaluated, the *Membership degrees* of its propositions are aggregated into a single truth value for the premise,
which in turn determines the *Firing strength* of the rule.
This firing strength is later used in later stages of inference.

#### Functional Rules (Sugeno-style)

A **Functional rule** defines its consequent as a function of the input variables rather than as a fuzzy set.
The consequent consists of a coefficients list and a bias.

Coefficients can be provided explicitly:

```csharp
BoundedFunctionalRule.Create(linguisticBase)
    .If("Temperature", "Cool")
    .And("Outside Temperature", "Freezing")
    .Then([1.5, 2.0], 1.0);
```

The previous rule can be read as:

    If the Temperature is Cool and the Outside Temperature is Freezing, then the Heating power = 1.5 * Temperature + 2.0 * Outside Temperature + 1.0.

While the premise of a functional rule can be expressed in natural language, its consequent cannot.
The output is produced by evaluating the function defined by the rule rather than by activating a linguistic term.

Alternatively, coefficients can be initialized using an initialization policy:

```csharp
BoundedFunctionalRule.Create(linguisticBase)
    .If("Temperature", "Cool")
    .And("Outside Temperature", "Freezing")
    .Then(CoefficientInitMethod.Centroid);
```

### Rule Base

A Rule Base is a container for rules that have already been instantiated.
Its responsibility is to store, organize, and expose rule collections so they can be queried and consumed by other parts
of the system.

At its core, a rule base operates over a single rule type and provides a common set of operations for managing rules and
inspecting their relationships.

#### Rule Base Variants

As with rules themselves, rule bases are specialized according to the kind of rules they contain.

The base interface for a rule base is:

```csharp
public interface IRuleBase<T> where T : class, IRule;
```

Two primary variants are provided, both extending the same base interface:

```csharp
public interface IFuzzySetRuleBase : IRuleBase<IFuzzySetRule>;
public interface IFunctionalRuleBase : IRuleBase<IFunctionalRule>;
```

A Fuzzy Set Rule Base stores Mamdani-style rules with linguistic consequents, while a Functional Rule Base stores
Sugeno-style rules whose consequents are mathematical functions.
Each variant operates on a homogeneous set of rules and can expose behavior specific to that rule type when needed.

#### Rule Storage and Lifecycle

The rule base acts as a container for instantiated rules.
It allows rules to be added and removed and exposes the full collection when direct access is required.

```csharp
ICollection<T> ProductionRules { get; }
void Add(T rule);
void AddAll(ICollection<T> rules);
void AddAll(params IEnumerable<T> rules);
bool Remove(T rule);
void RemoveAll(params IEnumerable<T> rules);
```

#### Rule Discovery and Filtering

Rules can be queried based on the variables they reference in their premises or conclusions.
This makes it possible to retrieve subsets of rules relevant to a specific variable without inspecting individual rule
definitions.

```csharp
IEnumerable<T> FindByPremise(StringOrType identifier);
IEnumerable<T> FindByConclusion(string target);
```

*Note*: `StringOrType` represents an identifier that can be either a `string` or a `Type`.
This reflects the two kinds of propositions supported by the system, as described in the
[Propositions](#propositions) section:

- For **Fuzzy propositions**, the identifier is a `string` corresponding to the name of a linguistic variable.
- For **Boolean propositions**, the identifier is a `Type` corresponding to the enum that defines the proposition.

#### Variable Introspection

The rule base can report which variables appear across the rule set, distinguishing between variables that serve as
inputs and those that are inferred by conclusions.
Boolean and fuzzy variables are also exposed separately.

```csharp
ISet<StringOrType> GetBaseVariables();
ISet<string> GetInferredVariables();
ISet<StringOrType> GetAllVariables();
ISet<Type> GetBooleanVariables();
ISet<string> GetFuzzyVariables();
```

#### Dependency Analysis

Rules often form dependency chains through shared variables.
The rule base can expose these relationships explicitly, allowing variable-level and rule-level dependencies to be
inspected.

```csharp
ISet<StringOrType> FindDependentVariables(string target);
IDictionary<StringOrType, List<StringOrType>> BuildDependencyGraph();
IDictionary<string, List<T>> BuildRuleDependencyMap();
```

#### Evaluation Readiness

Given a **Working memory**, the rule base can determine which rules are eligible for evaluation based on the
availability of the required facts.

```csharp
IEnumerable<IRule> GetEvaluable(IWorkingMemory memory);
```

#### Rule Activation Tracking

The rule base can track rule activation across inference iterations.
This makes it possible to distinguish between rules that have fired, have not fired, or are currently inactive.

```csharp
IEnumerable<T> GetActivated(uint iteration);
IEnumerable<T> GetUnactivated(uint iteration);
IEnumerable<T> GetDormant(IWorkingMemory memory, uint iteration);
IEnumerable<T> GetNeverActivated();
IEnumerable<T> GetEverActivated();
```

#### Learning and Adaptation Support

The rule base can record information about rule evaluations across inference iterations. This information is supplied by
the inference engine during execution and is stored for later inspection.

```csharp
void RecomputeAdaptation(AdaptationConfig config);
void ResetAdaptation();
```

Adaptation relies on evaluation data recorded during inference; the rule base itself does not compute or assign
evaluation values.

### Inference Engine

An **Inference Engine** is responsible for executing reasoning over a set of rules using the current domain state.
It coordinates the evaluation of rule premises, the application of consequents, and—when working with fuzzy-set
rules—the production of crisp output values.

At present, the library provides an inference engine for fuzzy-set (**Mamdani-style**) consequents.
Support for functional (**Sugeno-style**) inference engines is planned and currently underdevelopment.

#### Inference Model

The inference process is **Goal-driven** and follows a **Backward-chaining** strategy.
Reasoning starts from one or more target variables and proceeds by identifying rules capable of supporting those goals,
evaluating their premises against the available facts, and combining their effects.

For fuzzy-set rules, each evaluated rule produces a *fuzzy rule output*.
These outputs are aggregated into a single fuzzy result for the target variable, which is then defuzzified to obtain a
crisp value.
**Defuzzification** is the primary externally visible outcome of the inference engine.

#### Building an Inference Engine

Inference engines are constructed using a dedicated builder, allowing configuration to be expressed declaratively
through chained method calls.

A minimal example of constructing a **Fuzzy-consequent inference engine** is shown below:

```csharp
var engine =
    FuzzyConsequentEngineBuilder
        .Create()
        .WithRuleBase(ruleBase)
        .WithWorkingMemory(memory)
        .WithOperatorFamily(CanonicalType.Godel)
        .WithImplicationMethod(ImplicationMethod.Mamdani)
        .WithAggregationMethod(ValueAggregatorMethod.Mean)
        .WithDefuzzificationMethod(DefuzzificationMethod.CenterOfLargestArea)
        .Build();
```

The builder requires a *Rule Base* and a *Working Memory* to be supplied.
All other aspects of the engine configuration are optional and fall back to sensible defaults when not explicitly
specified.

##### Configuration Options

Each configuration step controls a specific aspect of the inference process:

##### Operator Family

This determines how the logical connectives `NOT` ($\neg$), `AND` ($\otimes$), `OR` ($\oplus$), and `THEN` ($\to$) are
evaluated when combining premise conditions.
FuzzySharper provides support for the following canonical families of fuzzy operators:

- Gödel:
  $$x \otimes_G y = \min(x, y)$$
  $$x \oplus_G y = \max(x, y)$$
  $$x \to_G y =
  \begin{cases}
  1 & \text{if } x \le y \\
  y & \text{if } x > y
  \end{cases}$$
- Łukasiewicz:
  $$x \otimes_L y = \max(0, x + y - 1)$$
  $$x \oplus_L y = \min(1, x + y)$$
  $$x \to_L y = \min(1, 1 - x + y)$$
- Nilpotent:
  $$x \otimes_N y =
  \begin{cases}
  \min(x, y)    & \text{if } x + y > 1 \\
  0 & \text{if } x + y \le 1
  \end{cases}$$
  $$x \oplus_N y =
  \begin{cases}
  \max(x, y) & \text{if } x + y < 1 \\
  1 & \text{if } x + y \ge 1
  \end{cases}$$
  $$x \to_N y = \max(1 - x, y)$$
- Product:
  $$x \otimes_P y = x \cdot y$$
  $$x \oplus_P y = x + y - x \cdot y$$
  $$x \to_P y =
  \begin{cases}
  1 & \text{if } x \le y \\
  \frac{y}{x} & \text{if } x > y
  \end{cases}$$

All families use the standard negation:
$$\neg x = 1 - x$$

##### Custom Operator Families

In addition to the built-in canonical families, FuzzySharper allows users to define custom conjunction, disjunction,
implication, and negation operators.

```csharp
public FuzzyConsequentEngineBuilder WithDisjunction(INorm norm);
public FuzzyConsequentEngineBuilder WithConjunction(IConorm conorm);
public FuzzyConsequentEngineBuilder WithResiduum(IResiduum residuum);
public FuzzyConsequentEngineBuilder WithOperators(INegation negation, INorm norm, IConorm conorm, IResiduum residuum);
```

This provides a high degree of flexibility, at the cost of stepping outside the well-established behavior of the
predefined operator families.

##### Implication Method

The implication method determines how the truth value of a rule’s premise is applied to its consequent.
In fuzzy-set rules, this step transforms the membership function associated with the rule’s conclusion based on how
strongly the premise is satisfied.
The resulting fuzzy sets are later combined and defuzzified to produce a final crisp value.

Only the following implication methods are currently supported:

- **Mamdani**
- **Larsen**

##### Defuzzification Method

The defuzzification method determines how the fuzzy outputs produced by all fired rules with the same consequent are
aggregated into a single crisp value.
This step represents one of the final stages of fuzzy inference and is the primary externally visible result of the
inference
engine.

Defuzzification is configured by selecting a method that defines how the aggregated fuzzy output is converted into a
crisp value.
The following defuzzification strategies are supported:

- **First of Maxima**, **Last of Maxima**, **Mean of Maxima**:
  Methods that operate on the consequent membership function associated with the highest premise truth value.
  See [here](https://codecrucks.com/maxima-methods-for-defuzzification-fom-lom-and-mom/) for technical details.
- **Center of Sums**:
  Computes the *Centroid* of the combined area contributed by each rule.
  See [here](https://codecrucks.com/center-of-sums-cos-method-for-defuzzification/) for technical
  details.
- **Center of Largest Area**:
  Computes the *Centroid* of the consequent membership function with the *Largest area* after implication is applied.
  See [here](https://codecrucks.com/center-of-largest-area-method-for-defuzzification/) for
  technical details.

#### Value Aggregation Method

The value aggregator defines how a final crisp value is selected when a defuzzification method yields multiple equally
valid candidates.

This situation can occur when more than one consequent membership function attains the same maximum value, the nature of
which depends on the selected defuzzification method—
for example, First/Last/Mean of Maxima rely on the highest premise truth value, while Center of Largest Area selects the
function with the largest area.
In such cases, the value aggregator acts as a tie-breaking strategy.

The following aggregation methods are supported:

- **Leftmost**: Selects the value associated with the leftmost candidate.
- **Mean**: Computes the arithmetic mean of all candidate values.
- **Rightmost**: Selects the value associated with the rightmost candidate.

#### Defuzzification (Execution)

Once the engine has been fully constructed and configured, inference is performed by invoking the Defuzzify method on
the fuzzy consequent engine:

```csharp
Option<double> Defuzzify(string target, bool provideExplanation = true);
```

This operation represents the final execution step of the fuzzy inference process.
Given the name of an output variable, the engine: evaluates all applicable rules; applies implication to their
consequent membership functions; resolves any ambiguities according to the configured value aggregation strategy;
produces a single crisp value using the selected defuzzification method.

If no rules contribute to the specified variable, or if inference cannot be completed, the method returns and empty
`Option`.

The optional `provideExplanation` flag controls whether explanatory metadata is shown to the user after the inference
process has been completed.
This has no effect on the numerical result itself, but it enables downstream inspection of how the final value was
obtained when explanation support is enabled.
For fuzzy-set–based inference, this method constitutes the primary externally observable result of the engine.

## Roadmap

The following items describe planned areas of evolution for FuzzySharper.

1. **Functional (Sugeno-Style) Inference Engine**. *(In progress)*
    - Completion of a first-class inference engine supporting functional consequents.
    - **Deterministic evaluation** with **no training phase**.
2. **Mamdani → Sugeno Knowledge Derivation**.
    - Derivation of functional consequents from an existing Mamdani rule base.
    - **One-shot conversion** based on an **already defined** model.
3. **Sugeno Engine Training from Data**.
    - Construction of Sugeno engines directly from datasets.
    - **Incremental or batch training**, constrained to interpretable functional rules.
4. **Tsukamoto-Style Implication Support**.
    - Support for Tsukamoto-style monotonic consequents within fuzzy inference.
    - **Deterministic evaluation** with **no training phase**.