﻿using FuzzyLogic.Rule;

namespace FuzzyLogic.Engine.Defuzzify.Methods;

public class MeanOfMaxima : IDefuzzifier
{
    public double? Defuzzify(ICollection<IRule> rules, IDictionary<string, double> facts)
    {
        var tuple = rules
            .Select(e => (e.Consequent!.Function, Weight: e.EvaluatePremiseWeight(facts).GetValueOrDefault()))
            .MaxBy(e => e.Weight);
        var (function, weight) = tuple;
        var (x1, x2) = function.LambdaCutInterval(weight);
        return (x1 + x2) / 2;
    }
}