﻿namespace FuzzyLogic.MembershipFunctions.Real;

public class RealGaussianFunction : BaseGaussianFunction<double>
{
    public RealGaussianFunction(string name, double m, double o) : base(name, m, o)
    {
        M = m;
        O = o;
    }

    protected override double M { get; }
    protected override double O { get; }
}