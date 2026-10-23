using Kernel.Operator.Conorm.Abstractions;
using Kernel.Operator.Conorm.Implementations.Canonical;
using Kernel.Operator.Family.Abstractions;
using Kernel.Operator.Negation.Abstractions;
using Kernel.Operator.Negation.Implementations.Canonical;
using Kernel.Operator.Norm.Abstractions;
using Kernel.Operator.Norm.Implementations.Canonical;
using Knowledge.Memory.Abstractions;
using Reasoning.Base.Extensions;
using Reasoning.Rule.Functional.Abstractions;
using Shared.Approx;
using Shared.Options.Factory;
using Shared.Options.Implementations;

namespace Reasoning.Base.Functional.Extensions;

public static class FunctionalRulesExt
{
    extension(ICollection<IFunctionalRule> rules)
    {
        public Option<double> EvaluateTarget(string target, IWorkingMemory memory, INegation negation, INorm norm, IConorm conorm)
        {
            if (memory.GetNumericFact(target).IsSome(out var numericFact))
                return numericFact;

            List<IFunctionalRule> applicableRules = [..rules.FindByConclusion(target)];
            List<IFunctionalRule> evaluableRules = [..applicableRules.GetEvaluable(memory)];

            if (evaluableRules.Count == 0)
                return Option<double>.None();

            var numerator = 0D;
            var denominator = 0D;

            foreach (var rule in evaluableRules)
            {
                var premiseWeight = rule.EvaluatePremiseWeight(memory, negation, norm, conorm);
                if (!premiseWeight.IsSome(out var weight) || weight == 0)
                    continue;

                var consequentValue = rule.EvaluateConsequentValue(memory);
                if (!consequentValue.IsSome(out var value))
                    continue;

                numerator += weight * value;
                denominator += weight;
            }

            return denominator.IsRoughlyZero() ? Option<double>.None() : Option<double>.Some(numerator / denominator);
        }

        public Option<double> EvaluateTarget(string target, IWorkingMemory memory, IOperatorFamily operatorFamily) =>
            rules.EvaluateTarget(target, memory, operatorFamily.Negation, operatorFamily.Norm, operatorFamily.Conorm);

        public Option<double> EvaluateTarget(string target, IWorkingMemory memory) =>
            rules.EvaluateTarget(target, memory, Negation.Standard, Norm.Minimum, Conorm.Maximum);

    }
}