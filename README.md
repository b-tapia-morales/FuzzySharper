# Fuzzy Sharper

## Overview

Fuzzy Sharper is a library that allows you to create your own Fuzzy Expert System.

## About the project

This project aims to make the life of a knowledge engineer easier by providing the tools necessary to develop an expert
system regardless of the knowledge domain that is currently being developed in. It doesn't aim to perform the tasks that
are normally associated with a knowledge engineer, such as the acquisition and representation of the knowledge, but
rather to provide the tools to implement such knowledge in code.

## Prerequisites

The library uses .NET SDK 7.

The following open source libraries are used in this project:

- [SmartEnum](https://github.com/ardalis/SmartEnum)
- [Math.Net Numerics](https://github.com/mathnet/mathnet-numerics)

## Folder structure

```
TODO
```

## Usage

### Linguistic Variables

A Linguistic Variable is a variable whose values are expressed in natural language, and it can be declared as follows:

```csharp
var water = LinguisticVariable.Create("Water")
    .AddTrapezoidalFunction("Cold", 0, 0, 20, 40)
    .AddTriangularFunction("Warm", 30, 50, 70)
    .AddTrapezoidalFunction("Hot", 50, 80, 100, 100)
```

The first line declares the linguistic variable with the name "Water", and the two following lines declare the set of
linguistic terms that belong to it.
These terms must be unique in name, and they are intrinsically associated with membership functions (in this case,
triangular and trapezoidal ones).

The provided numerical values as parameters have a certain meaning depending on the shape that the Membership Function
describes.
For example, in the case of the triangular function, the values 30, 50, and 70 describe the coordinates of the
triangle: (0, 30), (50, 1), (70, 0).
The middle value is always situated at height 1 because all Membership Functions are normal, that is, there's at least
one *x* value such that μ(*x*) = 1.

### Linguistic Base

A Linguistic Base can be instantiated as follows:

```csharp
public static ILinguisticBase Create()
```

This will create an instance of a Linguistic Base with no linguistic variables.
It is preferable to create a Linguistic Base preloading data.
The preferred method for doing so is as follows:

A class must be declared that inherits from `LinguisticBase`. For example, let's declare the following class:

```csharp
public class LinguisticBaseImpl : LinguisticBase
```

The class `LinguisticBase` declares a method that is meant to be hidden by an implementing class (using the `new`
keyword), which is the following:

```csharp
public new static ILinguisticBase Initialize()
{
    var water = LinguisticVariable.Create("Water")
        .AddTrapezoidalFunction("Cold", 0, 0, 20, 40)
        .AddTriangularFunction("Warm", 30, 50, 70)
        .AddTrapezoidalFunction("Hot", 50, 80, 100, 100)
    return Create().AddAll(water);
 }
```

This method will now instantiate and preload all linguistic variables belonging to the Linguistic Base inside the method
above.

### Rule Creation

A fuzzy rule is created from fuzzy propositions and logical connectors. The following is an example:

```csharp
FuzzyRule
    .Create(RulePriotity.High)
    .If(FuzzyProposition.Is(linguisticBase, "Water", "Cold"))
    .Or(FuzzyProposition.IsNot(linguisticBase, "Water", "Hot"))
    .Then(FuzzyProposition.Is(linguisticBase, "Power", "High"));
```

The `If`, `Or`, `Then` method names reference the logical connectors that are currently supported by the library.
Not shown in the example above, but also currently supported, is the `And` method.
All these methods take an instance of a `FuzzyProposition` as an argument.

A `FuzzyProposition` can be created by providing an instance of a `LinguisticBase`, the `LinguisticVariable` that is
contained in the linguistic base, and the linguistic term, which is associated by an implicit semantic rule to the
linguistic variable.
The `Is`, `IsNot` method names reference the literals in propositional logic, in which a proposition can be either an
affirmation or a negation.

For example, at line 4, the fuzzy proposition can be read in natural language as follows:

```Water IS NOT Hot```

The entire rule can be read in natural language as follows:

```IF Water IS Cold OR Water IS NOT Hot THEN Power IS High```

Both the fuzzy propositions and the fuzzy rules will be shown exactly as above by calling the `ToString()` method on
them.

The `Create()` method creates an instance of a `FuzzyRule`, and it can be given an enumeration member of the type
`RulePriority`, which is an `enum`.
There are three possible enumeration members:

- **Low**.
- **Normal**.
- **High**.

This defines a strategy for resolving conflicts between rules, since it may be the case that two rules with the same
consequent can be applicable from the given facts.
Providing this `enum` value is optional, and it will default to `Normal` if omitted.

### Rule Base

A Rule Base can be instantiated as follows:

```csharp
public static IRuleBase Create(ComparingMethod method = Priority)
```

`ComparingMethod` is an `enum` that represents the resolution method for conflicts between rules, since it may be the
case that two rules with the same consequent are stored in the rule base.

There are three possible enumeration members:

- **Priority**: Indicates that the rule with the highest priority will prevail.
- **LargestPremise**: Indicates that the rule with the greatest number of connectives in the premise will prevail.
- **ShortestPremise**: Indicates that the rule with the lowest number of connectives in the premise will prevail.

This will create an instance of a Rule Base with no rules.

This class declares a method that is meant to be hidden by an implementing class (using the `new` keyword), which is the
following:

```csharp
public static IRuleBase Initialize(ILinguisticBase linguisticBase, ComparingMethod method = Priority)
```

Now, we must hide the base method by re-declaring the base method and preload all data inside it.

```csharp
public new static IRuleBase Initialize(ILinguisticBase linguisticBase, ComparingMethod method = Priority)
{
    var r1 = FuzzyRule.Create(RulePriotity.High)
        .If(FuzzyProposition.Is(linguisticBase, "Water", "Cold"))
        .Or(FuzzyProposition.IsNot(linguisticBase, "Water", "Hot"))
        .Then(FuzzyProposition.Is(linguisticBase, "Power", "High"));
    var r2 = FuzzyRule.Create(RulePriority.Normal)
        .If(FuzzyProposition.Is(linguisticBase, "Water", "Hot"))
        .Then(FuzzyProposition.Is(linguisticBase, "Power", "Low"));
    return Create(method).AddAll(r1, r2);
}
```

### Knowledge Base

The Knowledge Base is simply an aggregation of both a Linguistic Base and a Rule Base.
The preferred method of creating an instance of a Knowledge Base is as follows:

```csharp
public static IKnowledgeBase Create(ILinguisticBase linguisticBase, IRuleBase ruleBase)
```

### Working Memory

A Working Memory represents all the knowledge about the domain as facts, whether they are provided by the user or
inferred by the system.

A Working Memory can be instantiated as follows:

```csharp
public static IWorkingMemory Create(EntryResolutionMethod method = Replace)
```

This will create an instance of a Working Memory with no given facts. All facts are stored in
a `IDictionary<string, double>` collection, and they are uniquely identifiable by their name. There cannot be two
facts with the same name.

`EntryResolutionMethod` is an `enum` that represents the resolution method for new entries whose keys collide with
previous entries' keys in the working memory, because, as it was previously established, there cannot be two facts with
the same name.
There are two possible enumeration members:

- **Preserve**: Indicates that the previous entries will be preserved.
- **Replace**: Indicates that the new entries will replace the previous ones.

if none is specified, the default enumeration member is `Replace`.

If the user desires to preload a Working Memory with data, two methods can be used:

#### Load from a CSV file

The following method loads all data via a CSV file:

```csharp
public static IWorkingMemory InitializeFromFile(string folderPath, EntryResolutionMethod method = Replace)
```

`folderPath` is a `string` indicating the route of the file. A file preloaded with data would look as follows:

| <!-- --> | <!-- --> |
|----------|----------|
| Age      | 18       |
| Height   | 175      |
| Weight   | 70       |

#### Load from code

The following method is marked as meant to be hidden by an implementing class:

```csharp
static abstract IWorkingMemory Initialize(EntryResolutionMethod method = Replace)
```

A class can be declared that inherits from `WorkingMemory` by initializing all data at instantiation.
For example, let's declare the following class:

```csharp
public class WorkingMemoryImpl : WorkingMemory
```

Now, we must hide the base method by re-declaring the base method and preload all data inside it.

```csharp
public new static IWorkingMemory Initialize(EntryResolutionMethod method = Replace)
{
    var workingMemory = Create();
    workingMemory.AddFact("Age", 18);
    workingMemory.AddFact("Height", 175);
    workingMemory.AddFact("Weight", 70);
    return workingMemory;
}
```

### Inference Engine

The Inference Engine is the core component of the library.
Conceptually, an Inference Engine defines control strategies or search techniques, which search through the knowledge
base to arrive at decisions.
The method of inference applied is **Backward-Chaining**, which a goal-driven process which starts with a list of
goals (or a hypothesis) and works backwards from the consequent to the antecedent to see if any data support any of
these consequents.
It follows the process described below:

```
while (no untried hypothesis) and (unresolved)
    for each hypothesis
        for each rule with hypothesis as consequent
            try to support rule’s conditions from known facts or via recursion (trying all possible bindings)
            if all supported then assert consequent
```

The result of this process is a Derivation Tree, which is an N-ary tree which has an *n* number of child nodes, which
represent the sub-goals that are necessary to prove a given goal.
The leaf nodes represent the facts, while the internal nodes represent a collection of rules.
Note that the rules in this collection must have the same consequent.

Backward Chaining is performed by using Depth-First Search, and it generates a Derivation Tree as a result.
The Proof Search is performed by using a Reverse Level Order Traversal over the Derivation Tree.
For more information on the implementation details, see the class [ITreeNode<T>](FuzzyLogic/Tree/ITreeNode.cs) and the
methods: `CreateDerivationTree` and `InferFact`.

#### Instantiation process

At its core, the Inference Engine is simply an aggregation of both a Knowledge Base and a Working Memory.
The preferred method of creating an instance of a Knowledge Base is as follows:

```csharp
public static IEngine Create(IKnowledgeBase knowledgeBase, IWorkingMemory workingMemory, DefuzzificationMethod method = MeanOfMaxima)
```

`DefuzzificationMethod` is an `enum` that represents the defuzzification method for aggregating the collection of rules
with the same consequent, and defuzzifying them into a crisp value.

There are five possible enumeration members:

- **FirstOfMaxima**, **LastOfMaxima**, **MeanOfMaxima**:
  See [here](https://codecrucks.com/maxima-methods-for-defuzzification-fom-lom-and-mom/) for technical details.
- **CenterOfSums**: See [here](https://codecrucks.com/center-of-sums-cos-method-for-defuzzification/) for technical
  details.
- **CenterOfLargestArea**: See [here](https://codecrucks.com/center-of-largest-area-method-for-defuzzification/) for
  technical details.

if none is specified, the default enumeration member is `MeanOfMaxima`.

Finally, the method for inferring new facts is as follows:

```csharp
public double? Defuzzify(string variableName, bool provideExplanation = true)
```

This will return a non-null value which represents the defuzzified, crisp number associated to the variable name
represented as a `string`, if such variable is currently present in the Working Memory as a fact, or if it was
successfully inferred as a fact resulting from the inference process.
The method provides an explanation facility by default, which shows in the console the resulting Derivation Tree from
the Backward Chaining, and the Proof Search from the Reverse Level Order Traversal of such Tree.

## Test run

The library provides an example by default, found in [this](FuzzyLogic/Test/Two) directory.
This rule base was extracted from [the following paper](http://www.progmat.uaem.mx:8080/Vol11num2/vol11num2art8.pdf).