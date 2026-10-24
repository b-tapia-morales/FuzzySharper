using Kernel.Function.Implementations;

namespace Kernel.Tests.Function;

public class FunctionParameterTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(0.0001)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void Gaussian_InvalidSigma_IsRejected(double sigma)
    {
        Assert.Throws<ArgumentException>(() => GaussianFunction.Create("Gaussian", 0, sigma));
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void Gaussian_NonfiniteCenter_IsRejected(double center)
    {
        Assert.Throws<ArgumentException>(() => GaussianFunction.Create("Gaussian", center, 1));
    }

    [Theory]
    [InlineData(0, 2, 0)]
    [InlineData(-1, 2, 0)]
    [InlineData(0.0001, 2, 0)]
    [InlineData(double.NaN, 2, 0)]
    [InlineData(double.PositiveInfinity, 2, 0)]
    [InlineData(double.NegativeInfinity, 2, 0)]
    [InlineData(1, 0, 0)]
    [InlineData(1, -1, 0)]
    [InlineData(1, 0.0001, 0)]
    [InlineData(1, double.NaN, 0)]
    [InlineData(1, double.PositiveInfinity, 0)]
    [InlineData(1, double.NegativeInfinity, 0)]
    [InlineData(1, 2, double.NaN)]
    [InlineData(1, 2, double.PositiveInfinity)]
    [InlineData(1, 2, double.NegativeInfinity)]
    public void GeneralizedBell_InvalidParameter_IsRejected(double width, double shape, double center)
    {
        Assert.Throws<ArgumentException>(() => GeneralizedBellFunction.Create("Bell", width, shape, center));
    }

    [Theory]
    [InlineData(2, 0.75, 0)]
    [InlineData(2, 0.75, -8)]
    [InlineData(1, 1, 1)]
    public void GeneralizedBell_IndependentPositiveWidthAndShape_AreAccepted(double width, double shape, double center)
    {
        var function = GeneralizedBellFunction.Create("Bell", width, shape, center);

        Assert.Equal(1, function.PureFunction(center), 10);
        Assert.Equal(0.5, function.PureFunction(center - width), 10);
        Assert.Equal(0.5, function.PureFunction(center + width), 10);
    }
}
