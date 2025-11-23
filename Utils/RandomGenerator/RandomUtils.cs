namespace Utils.RandomGenerator;

public static class RandomUtils
{
    private static readonly Random Random = new();
    
    public static double NextDouble(double minValue, double maxValue) => Random.NextDouble() * (maxValue - minValue) + minValue;
}