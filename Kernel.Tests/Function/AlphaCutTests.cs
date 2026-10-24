using Kernel.Function.Abstractions;
using Kernel.Function.Implementations;

namespace Kernel.Tests.Function;

public class AlphaCutTests
{
    [Theory]
    [InlineData("Gaussian", 1, 0.25)]
    [InlineData("Gaussian", 0.5, 0.25)]
    [InlineData("Gaussian", 0.4, 0.75)]
    [InlineData("Bell", 1, 0.25)]
    [InlineData("Bell", 0.5, 0.5)]
    [InlineData("Bell", 0.4, 0.75)]
    [InlineData("Left", 1, 0.25)]
    [InlineData("Left", 0.5, 0.5)]
    [InlineData("Left", 0.4, 0.75)]
    [InlineData("Right", 1, 0.25)]
    [InlineData("Right", 0.5, 0.5)]
    [InlineData("Right", 0.4, 0.75)]
    public void AlphaCut_FiniteEndpoints_HaveRequestedMembership(string shape, double height, double fraction)
    {
        var function = Create(shape, height);
        var alpha = height * fraction;
        var endpoints = new[] { function.AlphaCutLeft(alpha).Get, function.AlphaCutRight(alpha).Get };

        Assert.True(endpoints[0] <= endpoints[1]);
        Assert.Contains(endpoints, double.IsFinite);
        foreach (var endpoint in endpoints.Where(double.IsFinite))
            Assert.Equal(alpha, function.PureFunction(endpoint), 10);
    }

    [Theory]
    [InlineData("Gaussian", 3)]
    [InlineData("Bell", -4)]
    [InlineData("Left", 2)]
    [InlineData("Right", 10)]
    public void AlphaCut_AtMaximum_ReturnsPeakEndpoints(string shape, double expected)
    {
        var function = Create(shape, 0.4);
        var left = function.AlphaCutLeft(0.4).Get;
        var right = function.AlphaCutRight(0.4).Get;

        if (shape == "Left")
            Assert.True(double.IsNegativeInfinity(left));
        else
            Assert.Equal(expected, left);

        if (shape == "Right")
            Assert.True(double.IsPositiveInfinity(right));
        else
            Assert.Equal(expected, right);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(-1, 1)]
    [InlineData(2, 0.4)]
    [InlineData(-2, 0.4)]
    public void LogisticAlphaCut_HalfHeight_PassesThroughCenter(double steepness, double height)
    {
        var function = (LogisticFunction)LogisticFunction.Create("Logistic", steepness, 3, height);
        var left = function.AlphaCutLeft(height / 2).Get;
        var right = function.AlphaCutRight(height / 2).Get;

        Assert.Equal(steepness < 0, function.SaturatesLeft);
        Assert.Equal(steepness > 0, function.SaturatesRight);
        if (steepness > 0)
        {
            Assert.Equal(3, left, 10);
            Assert.True(double.IsPositiveInfinity(right));
        }
        else
        {
            Assert.True(double.IsNegativeInfinity(left));
            Assert.Equal(3, right, 10);
        }
    }

    [Theory]
    [InlineData(1, 0.25)]
    [InlineData(1, 0.75)]
    [InlineData(-1, 0.25)]
    [InlineData(-1, 0.75)]
    public void LogisticAlphaCut_AwayFromCenter_HasRequestedMembership(double steepness, double fraction)
    {
        var function = LogisticFunction.Create("Logistic", steepness, 3, 0.4);
        var alpha = 0.4 * fraction;
        var cut = steepness > 0 ? function.AlphaCutLeft(alpha).Get : function.AlphaCutRight(alpha).Get;

        Assert.Equal(alpha, function.PureFunction(cut), 10);
    }

    private static IMembershipFunction Create(string shape, double height) => shape switch
    {
        "Gaussian" => GaussianFunction.Create("Gaussian", 3, 2, height),
        "Bell" => GeneralizedBellFunction.Create("Bell", 2, 0.75, -4, height),
        "Left" => LeftTrapezoidFunction.Create("Left", 2, 10, height),
        "Right" => RightTrapezoidFunction.Create("Right", 2, 10, height),
        _ => throw new ArgumentOutOfRangeException(nameof(shape))
    };
}
