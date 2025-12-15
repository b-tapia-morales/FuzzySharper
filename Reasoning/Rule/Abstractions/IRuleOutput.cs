namespace Reasoning.Rule.Abstractions;

public interface IRuleOutput
{
    string Target { get; }
    
    bool ConsequentContains(string target) => 
        string.Equals(Target, target, StringComparison.OrdinalIgnoreCase);
}