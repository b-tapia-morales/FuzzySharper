using Kernel.Function.Abstractions;
using Kernel.Function.Implementations;

namespace Kernel.Tests.Function;

public class LarsenProductTests
{
    [Theory]
    [InlineData("Triangle", 0, 0)]
    [InlineData("Triangle", 2.5, 0.5)]
    [InlineData("Triangle", 5, 1)]
    [InlineData("Triangle", 7.5, 0.5)]
    [InlineData("Triangle", 10, 0)]
    [InlineData("LeftShoulderTriangle", 0, 1)]
    [InlineData("RightShoulderTriangle", 10, 1)]
    [InlineData("Trapezoid", 0, 0)]
    [InlineData("Trapezoid", 1, 0.5)]
    [InlineData("Trapezoid", 2, 1)]
    [InlineData("Trapezoid", 3, 1)]
    [InlineData("Trapezoid", 4, 1)]
    [InlineData("Trapezoid", 5, 0.5)]
    [InlineData("Trapezoid", 6, 0)]
    [InlineData("Gaussian", 0, 1)]
    [InlineData("Gaussian", -1, 0.6065306597126334)]
    [InlineData("Gaussian", 1, 0.6065306597126334)]
    [InlineData("GeneralizedBell", 2, 0.5)]
    [InlineData("GeneralizedBell", 3, 1)]
    [InlineData("GeneralizedBell", 4, 0.5)]
    [InlineData("Logistic", -1.0986122886681098, 0.25)]
    [InlineData("Logistic", 0, 0.5)]
    [InlineData("Logistic", 1.0986122886681098, 0.75)]
    [InlineData("LeftTrapezoid", -5, 1)]
    [InlineData("LeftTrapezoid", 0, 1)]
    [InlineData("LeftTrapezoid", 5, 0.5)]
    [InlineData("LeftTrapezoid", 10, 0)]
    [InlineData("RightTrapezoid", 0, 0)]
    [InlineData("RightTrapezoid", 5, 0.5)]
    [InlineData("RightTrapezoid", 10, 1)]
    [InlineData("RightTrapezoid", 15, 1)]
    [InlineData("Singleton", 0, 1)]
    [InlineData("Singleton", 0.01, 0)]
    public void MembershipAndProduct_UseOriginalHeightAndScalingFactor(string shape, double x, double normalizedMembership)
    {
        foreach (var height in new[] { 0.4, 1 })
        {
            var function = CreateFunction(shape, height);
            var expected = height * normalizedMembership;
            var expectedClipped = function.RestrictedSupport.Contains(x) ? expected : 0;

            Assert.Equal(expected, function.PureFunction(x), 12);
            Assert.Equal(expectedClipped, function.PureFunctionClipped(x), 12);

            foreach (var factor in new[] { 0, 0.25, 0.4, 0.75, 1 })
            {
                Assert.Equal(factor * expected, function.LarsenProduct(factor)(x), 12);
                Assert.Equal(factor * expectedClipped, function.LarsenProductClipped(factor)(x), 12);
            }
        }
    }

    private static IMembershipFunction CreateFunction(string shape, double height) => shape switch
    {
        "Triangle" => TriangleFunction.Create("Triangle", 0, 5, 10, height),
        "LeftShoulderTriangle" => TriangleFunction.Create("Triangle", 0, 0, 10, height),
        "RightShoulderTriangle" => TriangleFunction.Create("Triangle", 0, 10, 10, height),
        "Trapezoid" => TrapezoidFunction.Create("Trapezoid", 0, 2, 4, 6, height),
        "Gaussian" => GaussianFunction.Create("Gaussian", 0, 1, height),
        "GeneralizedBell" => GeneralizedBellFunction.Create("Bell", 1, 2, 3, height),
        "Logistic" => LogisticFunction.Create("Logistic", 1, 0, height),
        "LeftTrapezoid" => LeftTrapezoidFunction.Create("Left", 0, 10, height),
        "RightTrapezoid" => RightTrapezoidFunction.Create("Right", 0, 10, height),
        "Singleton" => SingletonFunction.Create("Singleton", 0, decimalPlaces: 2, uMax: height),
        _ => throw new ArgumentOutOfRangeException(nameof(shape), shape, null)
    };
}
