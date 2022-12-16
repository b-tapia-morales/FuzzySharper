﻿using FuzzyLogic.Function.Base;
using FuzzyLogic.Function.Interface;
using FuzzyLogic.Number;

namespace FuzzyLogic.Function.Real;

public class TrapezoidalFunction : BaseTrapezoidalFunction<double>, IRealFunction, IClosedSurface
{
    public TrapezoidalFunction(string name, double a, double b, double c, double d) : base(name, a, b, c, d)
    {
    }

    public double CalculateArea(double errorMargin = IClosedSurface.DefaultErrorMargin)
    {
        var a = Math.Abs(B - C);
        var b = Math.Abs(A - D);
        return 0.5 * Math.Sqrt(a * b);
    }

    public double CalculateArea(FuzzyNumber y, double errorMargin = IClosedSurface.DefaultErrorMargin)
    {
        var (x1, x2) = LambdaCutInterval(y);
        var a = Math.Abs(x1 - x2);
        var b = Math.Abs(A - D);
        return 0.5 * (a + b) * y.Value;
    }
}