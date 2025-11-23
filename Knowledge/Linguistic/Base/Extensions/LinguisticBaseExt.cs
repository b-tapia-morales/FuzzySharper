using Knowledge.Linguistic.Base.Implementations;
using Knowledge.Linguistic.Variable.Abstractions;

namespace Knowledge.Linguistic.Base.Extensions;

public static class LinguisticBaseExt
{
    extension(LinguisticBase @base)
    {
        public void AddMultiple(params IEnumerable<IVariable> variables) =>
            AddRange(@base, variables.ToList());

        public void AddRange(ICollection<IVariable> variables)
        {
            if (variables.Any(e => @base.ContainsVariable(e.Name)))
                throw new InvalidOperationException();

            foreach (var variable in variables)
                @base.LinguisticVariables.Add(variable.Name, variable);
        }
    }
}