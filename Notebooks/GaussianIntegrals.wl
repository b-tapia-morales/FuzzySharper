(* Gaussian endpoints and integrals, derived by Wolfram Solve and Integrate.
   mu = center Mu; sigma = Sigma > 0; h = UMax.
   q is the endpoint membership as a fraction of h.
   Each moment derives F(x), defines its parts, then evaluates F(x1)-F(x0).
   The centered substitution z = (x-mu)/sigma gives dx = sigma*dz.
   No endpoint solution, antiderivative or final moment result is supplied.
*)

(* In: Setup *)
ClearAll[mu, sigma, h, x, z, x0, x1, gaussian, assumptions, zAssumptions,
  toShorthand, partDefinitions, inputX, transformed, primitiveZ,
  raw, short, parts, formula, g, e, e2, e3];
gaussian[t_] := h*Exp[-(t - mu)^2/(2*sigma^2)];
assumptions = Element[{mu, sigma, h, x, x0, x1}, Reals] &&
  sigma > 0 && 0 < h <= 1 && x0 < x1;
zAssumptions = Element[{mu, sigma, h, z}, Reals] &&
  sigma > 0 && 0 < h <= 1;

toShorthand[expression_] := FullSimplify[
  (expression /. {
    Exp[-z^2/2] -> g[x],
    Exp[-z^2] -> g[x]^2,
    Erf[z/Sqrt[2]] -> e[x],
    Erf[z] -> e2[x],
    Erf[Sqrt[3/2]*z] -> e3[x]
  }) /. z -> (x - mu)/sigma,
  assumptions
];

partDefinitions = <|
  g[x] -> Exp[-(x - mu)^2/(2*sigma^2)],
  e[x] -> Erf[(x - mu)/(Sqrt[2]*sigma)],
  e2[x] -> Erf[(x - mu)/sigma],
  e3[x] -> Erf[Sqrt[3/2]*(x - mu)/sigma]
|>;
<|"Gaussian" -> gaussian[x], "Assumptions" -> assumptions|>

(* In: Gaussian endpoints / 1. Solve for the distance from the center *)
ClearAll[d, q, endpointAssumptions, distanceSolution, distance, endpoints,
  libraryTailSolution];
endpointAssumptions = Element[{h, sigma, q, d}, Reals] &&
  0 < h <= 1 && sigma > 0 && 0 < q < 1;
distanceSolution = FullSimplify[
  Solve[h*Exp[-d^2/(2*sigma^2)] == h*q && d > 0,
    d, Reals, Assumptions -> endpointAssumptions],
  endpointAssumptions
];
<|"Equation" -> HoldForm[h*Exp[-d^2/(2*sigma^2)] == h*q],
  "Distance solutions" -> distanceSolution|>

(* In: Gaussian endpoints / 2. Apply the distance on both sides of the center *)
distance = d /. First[distanceSolution];
endpoints = FullSimplify[
  {x0 -> mu - distance, x1 -> mu + distance},
  assumptions && Element[q, Reals] && 0 < q < 1
];
<|"Distance from center" -> distance, "Endpoints" -> endpoints|>

(* In: Gaussian endpoints / 3. Endpoint memberships and the library's four-sigma bounds *)
libraryTailSolution = FullSimplify[
  Solve[h*Exp[-(4*sigma)^2/(2*sigma^2)] == h*q,
    q, Reals, Assumptions -> endpointAssumptions],
  endpointAssumptions
];
<|
  "Memberships {left, right}" -> FullSimplify[
    {gaussian[x0], gaussian[x1]} /. endpoints,
    assumptions && Element[q, Reals] && 0 < q < 1],
  "Tail fraction at four sigma" -> libraryTailSolution,
  "Current library endpoints" -> FullSimplify[
    endpoints /. First[libraryTailSolution], assumptions]
|>

(* In: Area / 1. Derive the antiderivative *)
ClearAll[inputX, transformed, primitiveZ, raw, short, parts, formula, g, e, e2, e3];
inputX = gaussian[x];
transformed = FullSimplify[
  sigma*(inputX /. x -> mu + sigma*z), zAssumptions];
primitiveZ = Integrate[transformed, z, Assumptions -> zAssumptions];
raw = <|"Area" -> Collect[
  Expand[FullSimplify[primitiveZ /. z -> (x - mu)/sigma, assumptions]],
  _Erf, Factor
]|>;
raw

(* In: Area / 2. Define the parts used by the derived formula *)
short = toShorthand[primitiveZ];
parts = KeySelect[partDefinitions, !FreeQ[short, #] &];
parts

(* In: Area / 3. Compute the compact definite moment *)
formula = <|"Area" -> Factor[
  (short /. x -> x1) - (short /. x -> x0)
]|>;
formula

(* In: MomentX / 1. Derive the antiderivative *)
ClearAll[inputX, transformed, primitiveZ, raw, short, parts, formula, g, e, e2, e3];
inputX = x*gaussian[x];
transformed = FullSimplify[
  sigma*(inputX /. x -> mu + sigma*z), zAssumptions];
primitiveZ = Integrate[transformed, z, Assumptions -> zAssumptions];
raw = <|"MomentX" -> Collect[
  Expand[FullSimplify[primitiveZ /. z -> (x - mu)/sigma, assumptions]],
  _Erf, Factor
]|>;
raw

(* In: MomentX / 2. Define the parts used by the derived formula *)
short = toShorthand[primitiveZ];
parts = KeySelect[partDefinitions, !FreeQ[short, #] &];
parts

(* In: MomentX / 3. Compute the compact definite moment *)
formula = <|"MomentX" -> Factor[
  (short /. x -> x1) - (short /. x -> x0)
]|>;
formula

(* In: MomentY / 1. Derive the antiderivative *)
ClearAll[inputX, transformed, primitiveZ, raw, short, parts, formula, g, e, e2, e3];
inputX = gaussian[x]^2/2;
transformed = FullSimplify[
  sigma*(inputX /. x -> mu + sigma*z), zAssumptions];
primitiveZ = Integrate[transformed, z, Assumptions -> zAssumptions];
raw = <|"MomentY" -> Collect[
  Expand[FullSimplify[primitiveZ /. z -> (x - mu)/sigma, assumptions]],
  _Erf, Factor
]|>;
raw

(* In: MomentY / 2. Define the parts used by the derived formula *)
short = toShorthand[primitiveZ];
parts = KeySelect[partDefinitions, !FreeQ[short, #] &];
parts

(* In: MomentY / 3. Compute the compact definite moment *)
formula = <|"MomentY" -> Factor[
  (short /. x -> x1) - (short /. x -> x0)
]|>;
formula

(* In: MomentXx / 1. Derive the antiderivative *)
ClearAll[inputX, transformed, primitiveZ, raw, short, parts, formula, g, e, e2, e3];
inputX = x^2*gaussian[x];
transformed = FullSimplify[
  sigma*(inputX /. x -> mu + sigma*z), zAssumptions];
primitiveZ = Integrate[transformed, z, Assumptions -> zAssumptions];
raw = <|"MomentXx" -> Collect[
  Expand[FullSimplify[primitiveZ /. z -> (x - mu)/sigma, assumptions]],
  _Erf, Factor
]|>;
raw

(* In: MomentXx / 2. Define the parts used by the derived formula *)
short = toShorthand[primitiveZ];
parts = KeySelect[partDefinitions, !FreeQ[short, #] &];
parts

(* In: MomentXx / 3. Compute the compact definite moment *)
formula = <|"MomentXx" -> Factor[
  (short /. x -> x1) - (short /. x -> x0)
]|>;
formula

(* In: MomentXy / 1. Derive the antiderivative *)
ClearAll[inputX, transformed, primitiveZ, raw, short, parts, formula, g, e, e2, e3];
inputX = x*gaussian[x]^2/2;
transformed = FullSimplify[
  sigma*(inputX /. x -> mu + sigma*z), zAssumptions];
primitiveZ = Integrate[transformed, z, Assumptions -> zAssumptions];
raw = <|"MomentXy" -> Collect[
  Expand[FullSimplify[primitiveZ /. z -> (x - mu)/sigma, assumptions]],
  _Erf, Factor
]|>;
raw

(* In: MomentXy / 2. Define the parts used by the derived formula *)
short = toShorthand[primitiveZ];
parts = KeySelect[partDefinitions, !FreeQ[short, #] &];
parts

(* In: MomentXy / 3. Compute the compact definite moment *)
formula = <|"MomentXy" -> Factor[
  (short /. x -> x1) - (short /. x -> x0)
]|>;
formula

(* In: MomentYy / 1. Derive the antiderivative *)
ClearAll[inputX, transformed, primitiveZ, raw, short, parts, formula, g, e, e2, e3];
inputX = gaussian[x]^3/3;
transformed = FullSimplify[
  sigma*(inputX /. x -> mu + sigma*z), zAssumptions];
primitiveZ = Integrate[transformed, z, Assumptions -> zAssumptions];
raw = <|"MomentYy" -> Collect[
  Expand[FullSimplify[primitiveZ /. z -> (x - mu)/sigma, assumptions]],
  _Erf, Factor
]|>;
raw

(* In: MomentYy / 2. Define the parts used by the derived formula *)
short = toShorthand[primitiveZ];
parts = KeySelect[partDefinitions, !FreeQ[short, #] &];
parts

(* In: MomentYy / 3. Compute the compact definite moment *)
formula = <|"MomentYy" -> Factor[
  (short /. x -> x1) - (short /. x -> x0)
]|>;
formula
