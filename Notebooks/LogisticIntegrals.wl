(* Logistic sigmoid integrals, derived by Wolfram Integrate.
   a = steepness A; c = center C; h = UMax.
   Each section derives F(x), defines its parts, then evaluates F(x1)-F(x0).
   The substitution t = Exp[a*(x-c)] makes t positive and dx = dt/(a*t).
   Pulling out factors constant in t avoids unnecessary Log[a] terms.
   No antiderivative or definite-moment result is supplied in this file.
*)

(* In: Setup *)
ClearAll[a, c, h, x, t, x0, x1, logistic, assumptions, tAssumptions,
  splitConstant, toShorthand, partDefinitions, inputX, transformed,
  coefficient, unitInput, primitiveT, raw, short, parts, formula, s, l, p, r];
logistic[z_] := h/(1 + Exp[-a*(z - c)]);
assumptions = Element[{a, c, h, x, x0, x1}, Reals] &&
  a != 0 && 0 < h <= 1 && x0 < x1;
tAssumptions = Element[{a, c, h, t}, Reals] && a != 0 && 0 < h <= 1 && t > 0;

splitConstant[expression_, variable_] := Module[{factored, factors},
  factored = Factor[expression];
  factors = If[Head[factored] === Times, List @@ factored, {factored}];
  {
    Times @@ Select[factors, FreeQ[#, variable] &],
    Times @@ Select[factors, !FreeQ[#, variable] &]
  }
];

toShorthand[expression_] := FullSimplify[
  (expression /. {
    Log[t] -> a*(x - c),
    Log[1 + t] -> l[x],
    PolyLog[2, -t] -> p[x],
    PolyLog[3, -t] -> r[x]
  }) /. t -> 1/s[x] - 1,
  assumptions
];

partDefinitions = <|
  s[x] -> 1/(1 + Exp[a*(x - c)]),
  l[x] -> Log[1 + Exp[a*(x - c)]],
  p[x] -> PolyLog[2, -Exp[a*(x - c)]],
  r[x] -> PolyLog[3, -Exp[a*(x - c)]]
|>;
<|"Sigmoid" -> logistic[x], "Assumptions" -> assumptions|>

(* In: Sigmoid endpoints / 1. Solve for the distance using positive steepness k *)
ClearAll[k, d, q, endpointAssumptions, distanceSolution, distance, endpoints];
endpointAssumptions = Element[{h, k, q, d}, Reals] &&
  0 < h <= 1 && k > 0 && 0 < q < 1/2;
distanceSolution = FullSimplify[
  Solve[h/(1 + Exp[-k*d]) == h*(1 - q) && d > 0,
    d, Reals, Assumptions -> endpointAssumptions],
  endpointAssumptions
];
<|"Equation" -> HoldForm[h/(1 + Exp[-k*d]) == h*(1 - q)],
  "Distance solutions" -> distanceSolution|>

(* In: Sigmoid endpoints / 2. Apply the distance on both sides of the center *)
distance = d /. First[distanceSolution];
endpoints = FullSimplify[
  {x0 -> c - (distance /. k -> Abs[a]),
   x1 -> c + (distance /. k -> Abs[a])},
  assumptions && Element[q, Reals] && 0 < q < 1/2
];
<|"Distance from center" -> distance, "Endpoints" -> endpoints|>

(* In: Sigmoid endpoints / 3. Evaluate the membership at each endpoint *)
<|
  "Increasing: {left, right}" -> FullSimplify[
    {logistic[x0], logistic[x1]} /. endpoints,
    assumptions && Element[q, Reals] && 0 < q < 1/2 && a > 0],
  "Decreasing: {left, right}" -> FullSimplify[
    {logistic[x0], logistic[x1]} /. endpoints,
    assumptions && Element[q, Reals] && 0 < q < 1/2 && a < 0]
|>

(* In: Area / 1. Derive the antiderivative *)
ClearAll[inputX, transformed, coefficient, unitInput, primitiveT,
  raw, short, parts, formula, s, l, p, r];
inputX = logistic[x];
transformed = FullSimplify[
  (inputX /. x -> c + Log[t]/a)/(a*t), tAssumptions];
{coefficient, unitInput} = splitConstant[transformed, t];
primitiveT = coefficient*Integrate[unitInput, t, Assumptions -> tAssumptions];
raw = <|"Area" -> Collect[
  Expand[FullSimplify[primitiveT /. t -> Exp[a*(x - c)], assumptions]],
  {_Log, _PolyLog}, Factor
]|>;
raw

(* In: Area / 2. Define the parts used by the derived formula *)
short = toShorthand[primitiveT];
parts = KeySelect[partDefinitions, !FreeQ[short, #] &];
parts

(* In: Area / 3. Compute the compact definite moment *)
formula = <|"Area" -> Factor[
  (short /. x -> x1) - (short /. x -> x0)
]|>;
formula

(* In: MomentX / 1. Derive the antiderivative *)
ClearAll[inputX, transformed, coefficient, unitInput, primitiveT,
  raw, short, parts, formula, s, l, p, r];
inputX = x*logistic[x];
transformed = FullSimplify[
  (inputX /. x -> c + Log[t]/a)/(a*t), tAssumptions];
{coefficient, unitInput} = splitConstant[transformed, t];
primitiveT = coefficient*Integrate[unitInput, t, Assumptions -> tAssumptions];
raw = <|"MomentX" -> Collect[
  Expand[FullSimplify[primitiveT /. t -> Exp[a*(x - c)], assumptions]],
  {_Log, _PolyLog}, Factor
]|>;
raw

(* In: MomentX / 2. Define the parts used by the derived formula *)
short = toShorthand[primitiveT];
parts = KeySelect[partDefinitions, !FreeQ[short, #] &];
parts

(* In: MomentX / 3. Compute the compact definite moment *)
formula = <|"MomentX" -> Factor[
  (short /. x -> x1) - (short /. x -> x0)
]|>;
formula

(* In: MomentY / 1. Derive the antiderivative *)
ClearAll[inputX, transformed, coefficient, unitInput, primitiveT,
  raw, short, parts, formula, s, l, p, r];
inputX = logistic[x]^2/2;
transformed = FullSimplify[
  (inputX /. x -> c + Log[t]/a)/(a*t), tAssumptions];
{coefficient, unitInput} = splitConstant[transformed, t];
primitiveT = coefficient*Integrate[unitInput, t, Assumptions -> tAssumptions];
raw = <|"MomentY" -> Collect[
  Expand[FullSimplify[primitiveT /. t -> Exp[a*(x - c)], assumptions]],
  {_Log, _PolyLog}, Factor
]|>;
raw

(* In: MomentY / 2. Define the parts used by the derived formula *)
short = toShorthand[primitiveT];
parts = KeySelect[partDefinitions, !FreeQ[short, #] &];
parts

(* In: MomentY / 3. Compute the compact definite moment *)
formula = <|"MomentY" -> Factor[
  (short /. x -> x1) - (short /. x -> x0)
]|>;
formula

(* In: MomentXx / 1. Derive the antiderivative *)
ClearAll[inputX, transformed, coefficient, unitInput, primitiveT,
  raw, short, parts, formula, s, l, p, r];
inputX = x^2*logistic[x];
transformed = FullSimplify[
  (inputX /. x -> c + Log[t]/a)/(a*t), tAssumptions];
{coefficient, unitInput} = splitConstant[transformed, t];
primitiveT = coefficient*Integrate[unitInput, t, Assumptions -> tAssumptions];
raw = <|"MomentXx" -> Collect[
  Expand[FullSimplify[primitiveT /. t -> Exp[a*(x - c)], assumptions]],
  {_Log, _PolyLog}, Factor
]|>;
raw

(* In: MomentXx / 2. Define the parts used by the derived formula *)
short = toShorthand[primitiveT];
parts = KeySelect[partDefinitions, !FreeQ[short, #] &];
parts

(* In: MomentXx / 3. Compute the compact definite moment *)
formula = <|"MomentXx" -> Factor[
  (short /. x -> x1) - (short /. x -> x0)
]|>;
formula

(* In: MomentXy / 1. Derive the antiderivative *)
ClearAll[inputX, transformed, coefficient, unitInput, primitiveT,
  raw, short, parts, formula, s, l, p, r];
inputX = x*logistic[x]^2/2;
transformed = FullSimplify[
  (inputX /. x -> c + Log[t]/a)/(a*t), tAssumptions];
{coefficient, unitInput} = splitConstant[transformed, t];
primitiveT = coefficient*Integrate[unitInput, t, Assumptions -> tAssumptions];
raw = <|"MomentXy" -> Collect[
  Expand[FullSimplify[primitiveT /. t -> Exp[a*(x - c)], assumptions]],
  {_Log, _PolyLog}, Factor
]|>;
raw

(* In: MomentXy / 2. Define the parts used by the derived formula *)
short = toShorthand[primitiveT];
parts = KeySelect[partDefinitions, !FreeQ[short, #] &];
parts

(* In: MomentXy / 3. Compute the compact definite moment *)
formula = <|"MomentXy" -> Factor[
  (short /. x -> x1) - (short /. x -> x0)
]|>;
formula

(* In: MomentYy / 1. Derive the antiderivative *)
ClearAll[inputX, transformed, coefficient, unitInput, primitiveT,
  raw, short, parts, formula, s, l, p, r];
inputX = logistic[x]^3/3;
transformed = FullSimplify[
  (inputX /. x -> c + Log[t]/a)/(a*t), tAssumptions];
{coefficient, unitInput} = splitConstant[transformed, t];
primitiveT = coefficient*Integrate[unitInput, t, Assumptions -> tAssumptions];
raw = <|"MomentYy" -> Collect[
  Expand[FullSimplify[primitiveT /. t -> Exp[a*(x - c)], assumptions]],
  {_Log, _PolyLog}, Factor
]|>;
raw

(* In: MomentYy / 2. Define the parts used by the derived formula *)
short = toShorthand[primitiveT];
parts = KeySelect[partDefinitions, !FreeQ[short, #] &];
parts

(* In: MomentYy / 3. Compute the compact definite moment *)
formula = <|"MomentYy" -> Factor[
  (short /. x -> x1) - (short /. x -> x0)
]|>;
formula
