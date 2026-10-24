(* Generalized bell endpoints and integrals, derived by Wolfram Solve and Integrate.
   a = Abs[A] > 0; b = B > 0; c = C; h = UMax.
   q is the endpoint membership as a fraction of h.
   First integrate on the right using t = (x-c)/a > 0 and dx = a*dt.
   Each term has an integer power of t times an even radial factor.
   To extend the primitive to the left, retain the signed polynomial powers
   and replace t^(2*b) inside the radial factor by Abs[(x-c)/a]^(2*b).
   No antiderivative, endpoint solution or final moment result is supplied.
*)

(* In: Setup *)
ClearAll[a, b, c, h, x, t, x0, x1, bell, assumptions, tAssumptions,
  splitConstant, toX, inputX, transformed, terms, coefficients, integrands,
  primitiveT, raw, kernels, names, partDefinitions, short, parts, formula,
  u, p, r, s];
bell[z_] := h/(1 + Abs[(z - c)/a]^(2*b));
assumptions = Element[{a, b, c, h, x, x0, x1}, Reals] &&
  a > 0 && b > 0 && 0 < h <= 1 && x0 < x1;
tAssumptions = Element[{a, b, c, h, t}, Reals] &&
  a > 0 && b > 0 && 0 < h <= 1 && t > 0;

splitConstant[expression_, variable_] := Module[{factored, factors},
  factored = Factor[expression];
  factors = If[Head[factored] === Times, List @@ factored, {factored}];
  {
    Times @@ Select[factors, FreeQ[#, variable] &],
    Times @@ Select[factors, !FreeQ[#, variable] &]
  }
];

toX[expression_] :=
  (expression /. t^(2*b) -> Abs[(x - c)/a]^(2*b)) /. t -> (x - c)/a;
<|"Generalized bell" -> bell[x], "Assumptions" -> assumptions|>

(* In: Generalized bell endpoints / 1. Solve for the distance from the center *)
ClearAll[d, q, alpha, endpointAssumptions, distanceSolution, distance,
  endpoints, absoluteTailSolution, absoluteDistance, absoluteEndpoints, cutoff];
endpointAssumptions = Element[{a, b, h, d, q}, Reals] &&
  a > 0 && b > 0 && 0 < h <= 1 && 0 < q < 1;
distanceSolution = FullSimplify[
  Solve[h/(1 + (d/a)^(2*b)) == h*q && d > 0,
    d, Reals, Assumptions -> endpointAssumptions],
  endpointAssumptions
];
<|"Equation" -> HoldForm[h/(1 + (d/a)^(2*b)) == h*q],
  "Distance solutions" -> distanceSolution|>

(* In: Generalized bell endpoints / 2. Apply the distance on both sides of the center *)
distance = d /. First[distanceSolution];
endpoints = FullSimplify[
  {x0 -> c - distance, x1 -> c + distance},
  assumptions && Element[q, Reals] && 0 < q < 1
];
<|"Distance from center" -> distance, "Endpoints" -> endpoints|>

(* In: Generalized bell endpoints / 3. Evaluate the membership at each endpoint *)
<|"Memberships {left, right}" -> FullSimplify[
  {bell[x0], bell[x1]} /. endpoints,
  assumptions && Element[q, Reals] && 0 < q < 1
]|>

(* In: Generalized bell endpoints / 4. Convert an absolute membership cutoff to a fraction *)
absoluteTailSolution = FullSimplify[
  Solve[h*q == alpha, q, Reals,
    Assumptions -> Element[{h, alpha}, Reals] && 0 < alpha < h <= 1],
  Element[{h, alpha}, Reals] && 0 < alpha < h <= 1
];
absoluteDistance = FullSimplify[
  distance /. First[absoluteTailSolution],
  assumptions && Element[alpha, Reals] && 0 < alpha < h
];
absoluteEndpoints = {x0 -> c - absoluteDistance, x1 -> c + absoluteDistance};
cutoff = 2/10000;
<|
  "Fraction for membership alpha" -> absoluteTailSolution,
  "Distance for membership alpha" -> absoluteDistance,
  "Endpoints for membership alpha" -> absoluteEndpoints,
  "Library cutoff: 2 * DefaultTolerance" -> cutoff,
  "Distance at that cutoff, for h > cutoff" ->
    FullSimplify[absoluteDistance /. alpha -> cutoff,
      assumptions && h > cutoff]
|>

(* In: Area / 1. Derive the antiderivative *)
ClearAll[inputX, transformed, terms, coefficients, integrands, primitiveT,
  raw, kernels, names, partDefinitions, short, parts, formula, u, p, r, s];
inputX = bell[x];
transformed = Expand[
  (a*(inputX /. x -> c + a*t)) /. Abs[t] -> t];
terms = If[Head[transformed] === Plus, List @@ transformed, {transformed}];
{coefficients, integrands} = Transpose[splitConstant[#, t] & /@ terms];
primitiveT = coefficients . (Integrate[#, t, Assumptions -> tAssumptions] & /@ integrands);
raw = <|"Area" -> Collect[
  Expand[toX[primitiveT]], _Hypergeometric2F1, Factor
]|>;
raw

(* In: Area / 2. Define the parts used by the derived formula *)
kernels = DeleteDuplicates[Cases[primitiveT, _Hypergeometric2F1, Infinity]];
names = Take[{p[x], r[x], s[x]}, Length[kernels]];
partDefinitions = Association[Join[
  {u[x] -> x - c}, Thread[names -> (toX /@ kernels)]
]];
short = Factor[
  (primitiveT /. Thread[kernels -> names]) /. t -> u[x]/a];
parts = KeySelect[partDefinitions, !FreeQ[short, #] &];
parts

(* In: Area / 3. Compute the compact definite moment *)
formula = <|"Area" -> Factor[
  (short /. x -> x1) - (short /. x -> x0)
]|>;
formula

(* In: MomentX / 1. Derive the antiderivative *)
ClearAll[inputX, transformed, terms, coefficients, integrands, primitiveT,
  raw, kernels, names, partDefinitions, short, parts, formula, u, p, r, s];
inputX = x*bell[x];
transformed = Expand[
  (a*(inputX /. x -> c + a*t)) /. Abs[t] -> t];
terms = If[Head[transformed] === Plus, List @@ transformed, {transformed}];
{coefficients, integrands} = Transpose[splitConstant[#, t] & /@ terms];
primitiveT = coefficients . (Integrate[#, t, Assumptions -> tAssumptions] & /@ integrands);
raw = <|"MomentX" -> Collect[
  Expand[toX[primitiveT]], _Hypergeometric2F1, Factor
]|>;
raw

(* In: MomentX / 2. Define the parts used by the derived formula *)
kernels = DeleteDuplicates[Cases[primitiveT, _Hypergeometric2F1, Infinity]];
names = Take[{p[x], r[x], s[x]}, Length[kernels]];
partDefinitions = Association[Join[
  {u[x] -> x - c}, Thread[names -> (toX /@ kernels)]
]];
short = Factor[
  (primitiveT /. Thread[kernels -> names]) /. t -> u[x]/a];
parts = KeySelect[partDefinitions, !FreeQ[short, #] &];
parts

(* In: MomentX / 3. Compute the compact definite moment *)
formula = <|"MomentX" -> Factor[
  (short /. x -> x1) - (short /. x -> x0)
]|>;
formula

(* In: MomentY / 1. Derive the antiderivative *)
ClearAll[inputX, transformed, terms, coefficients, integrands, primitiveT,
  raw, kernels, names, partDefinitions, short, parts, formula, u, p, r, s];
inputX = bell[x]^2/2;
transformed = Expand[
  (a*(inputX /. x -> c + a*t)) /. Abs[t] -> t];
terms = If[Head[transformed] === Plus, List @@ transformed, {transformed}];
{coefficients, integrands} = Transpose[splitConstant[#, t] & /@ terms];
primitiveT = coefficients . (Integrate[#, t, Assumptions -> tAssumptions] & /@ integrands);
raw = <|"MomentY" -> Collect[
  Expand[toX[primitiveT]], _Hypergeometric2F1, Factor
]|>;
raw

(* In: MomentY / 2. Define the parts used by the derived formula *)
kernels = DeleteDuplicates[Cases[primitiveT, _Hypergeometric2F1, Infinity]];
names = Take[{p[x], r[x], s[x]}, Length[kernels]];
partDefinitions = Association[Join[
  {u[x] -> x - c}, Thread[names -> (toX /@ kernels)]
]];
short = Factor[
  (primitiveT /. Thread[kernels -> names]) /. t -> u[x]/a];
parts = KeySelect[partDefinitions, !FreeQ[short, #] &];
parts

(* In: MomentY / 3. Compute the compact definite moment *)
formula = <|"MomentY" -> Factor[
  (short /. x -> x1) - (short /. x -> x0)
]|>;
formula

(* In: MomentXx / 1. Derive the antiderivative *)
ClearAll[inputX, transformed, terms, coefficients, integrands, primitiveT,
  raw, kernels, names, partDefinitions, short, parts, formula, u, p, r, s];
inputX = x^2*bell[x];
transformed = Expand[
  (a*(inputX /. x -> c + a*t)) /. Abs[t] -> t];
terms = If[Head[transformed] === Plus, List @@ transformed, {transformed}];
{coefficients, integrands} = Transpose[splitConstant[#, t] & /@ terms];
primitiveT = coefficients . (Integrate[#, t, Assumptions -> tAssumptions] & /@ integrands);
raw = <|"MomentXx" -> Collect[
  Expand[toX[primitiveT]], _Hypergeometric2F1, Factor
]|>;
raw

(* In: MomentXx / 2. Define the parts used by the derived formula *)
kernels = DeleteDuplicates[Cases[primitiveT, _Hypergeometric2F1, Infinity]];
names = Take[{p[x], r[x], s[x]}, Length[kernels]];
partDefinitions = Association[Join[
  {u[x] -> x - c}, Thread[names -> (toX /@ kernels)]
]];
short = Factor[
  (primitiveT /. Thread[kernels -> names]) /. t -> u[x]/a];
parts = KeySelect[partDefinitions, !FreeQ[short, #] &];
parts

(* In: MomentXx / 3. Compute the compact definite moment *)
formula = <|"MomentXx" -> Factor[
  (short /. x -> x1) - (short /. x -> x0)
]|>;
formula

(* In: MomentXy / 1. Derive the antiderivative *)
ClearAll[inputX, transformed, terms, coefficients, integrands, primitiveT,
  raw, kernels, names, partDefinitions, short, parts, formula, u, p, r, s];
inputX = x*bell[x]^2/2;
transformed = Expand[
  (a*(inputX /. x -> c + a*t)) /. Abs[t] -> t];
terms = If[Head[transformed] === Plus, List @@ transformed, {transformed}];
{coefficients, integrands} = Transpose[splitConstant[#, t] & /@ terms];
primitiveT = coefficients . (Integrate[#, t, Assumptions -> tAssumptions] & /@ integrands);
raw = <|"MomentXy" -> Collect[
  Expand[toX[primitiveT]], _Hypergeometric2F1, Factor
]|>;
raw

(* In: MomentXy / 2. Define the parts used by the derived formula *)
kernels = DeleteDuplicates[Cases[primitiveT, _Hypergeometric2F1, Infinity]];
names = Take[{p[x], r[x], s[x]}, Length[kernels]];
partDefinitions = Association[Join[
  {u[x] -> x - c}, Thread[names -> (toX /@ kernels)]
]];
short = Factor[
  (primitiveT /. Thread[kernels -> names]) /. t -> u[x]/a];
parts = KeySelect[partDefinitions, !FreeQ[short, #] &];
parts

(* In: MomentXy / 3. Compute the compact definite moment *)
formula = <|"MomentXy" -> Factor[
  (short /. x -> x1) - (short /. x -> x0)
]|>;
formula

(* In: MomentYy / 1. Derive the antiderivative *)
ClearAll[inputX, transformed, terms, coefficients, integrands, primitiveT,
  raw, kernels, names, partDefinitions, short, parts, formula, u, p, r, s];
inputX = bell[x]^3/3;
transformed = Expand[
  (a*(inputX /. x -> c + a*t)) /. Abs[t] -> t];
terms = If[Head[transformed] === Plus, List @@ transformed, {transformed}];
{coefficients, integrands} = Transpose[splitConstant[#, t] & /@ terms];
primitiveT = coefficients . (Integrate[#, t, Assumptions -> tAssumptions] & /@ integrands);
raw = <|"MomentYy" -> Collect[
  Expand[toX[primitiveT]], _Hypergeometric2F1, Factor
]|>;
raw

(* In: MomentYy / 2. Define the parts used by the derived formula *)
kernels = DeleteDuplicates[Cases[primitiveT, _Hypergeometric2F1, Infinity]];
names = Take[{p[x], r[x], s[x]}, Length[kernels]];
partDefinitions = Association[Join[
  {u[x] -> x - c}, Thread[names -> (toX /@ kernels)]
]];
short = Factor[
  (primitiveT /. Thread[kernels -> names]) /. t -> u[x]/a];
parts = KeySelect[partDefinitions, !FreeQ[short, #] &];
parts

(* In: MomentYy / 3. Compute the compact definite moment *)
formula = <|"MomentYy" -> Factor[
  (short /. x -> x1) - (short /. x -> x0)
]|>;
formula
