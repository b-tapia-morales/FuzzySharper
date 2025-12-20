using Shared.Approx;

namespace Reasoning.Adaptation.RateScheduler.Abstractions;

public interface IRateScheduler
{
    double GetAlpha(uint iteration);
    
    internal static void CheckValue(string name, double value, double minValue)
    {
        var message = $"Value provided was: {value}";
        if (double.IsNaN(value) || double.IsInfinity(value))
            throw new ArgumentException($"{name} must not be NaN or Infinity. {message}");
        if (value.IsRoughlyLesserThan(minValue))
            throw new ArgumentException($"{name} must be a value greater than {minValue}. {message}");
    }
}