﻿using FuzzyLogic.Function.Base;
using FuzzyLogic.Function.Interface;

namespace FuzzyLogic.Function.Real;

public class SigmoidFunction : BaseSigmoidFunction<double>, IRealFunction
{
    public SigmoidFunction(string name, double a, double c) : base(name, a, c)
    {
    }

    public override double LowerBoundary() => double.NegativeInfinity;

    public override double UpperBoundary() => double.PositiveInfinity;
}