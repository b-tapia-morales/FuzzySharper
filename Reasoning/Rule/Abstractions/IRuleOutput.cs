namespace Reasoning.Rule.Abstractions;

public interface IRuleOutput
{
    string Target { get; }
    
    bool Contains(string target) => 
        string.Equals(Target, target, StringComparison.OrdinalIgnoreCase);

    IRuleOutput DeepCopy();
}