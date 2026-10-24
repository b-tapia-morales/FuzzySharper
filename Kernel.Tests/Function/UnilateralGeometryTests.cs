using Kernel.Function.Abstractions;
using Kernel.Function.Extensions;
using Kernel.Function.Implementations;
using Kernel.Function.Implication.Factory;
using Utils.Shape;

namespace Kernel.Tests.Function;

public class UnilateralGeometryTests
{
    [Theory]
    [InlineData(false, ImplicationMethod.Mamdani, 0.5, 3.4375, 835.0 / 132, 7.0 / 33)]
    [InlineData(true, ImplicationMethod.Mamdani, 0.5, 3.4375, 485.0 / 132, 7.0 / 33)]
    [InlineData(false, ImplicationMethod.Larsen, 0.5, 2, 20.0 / 3, 2.0 / 15)]
    [InlineData(true, ImplicationMethod.Larsen, 0.5, 2, 10.0 / 3, 2.0 / 15)]
    [InlineData(false, ImplicationMethod.Mamdani, 1, 4, 20.0 / 3, 4.0 / 15)]
    [InlineData(true, ImplicationMethod.Mamdani, 1, 4, 10.0 / 3, 4.0 / 15)]
    [InlineData(false, ImplicationMethod.Larsen, 1, 4, 20.0 / 3, 4.0 / 15)]
    [InlineData(true, ImplicationMethod.Larsen, 1, 4, 10.0 / 3, 4.0 / 15)]
    [InlineData(false, ImplicationMethod.Mamdani, 0, 0, 0, 0)]
    [InlineData(true, ImplicationMethod.Mamdani, 0, 0, 0, 0)]
    [InlineData(false, ImplicationMethod.Larsen, 0, 0, 0, 0)]
    [InlineData(true, ImplicationMethod.Larsen, 0, 0, 0, 0)]
    public void NonUnitShoulder_ReturnsExpectedAreaMomentsAndCentroids(
        bool isLeft, ImplicationMethod method, double parameter,
        double expectedArea, double expectedCentroidX, double expectedCentroidY)
    {
        IMembershipFunction function = isLeft
            ? LeftTrapezoidFunction.Create("Left", 0, 10, 0.8)
            : RightTrapezoidFunction.Create("Right", 0, 10, 0.8);

        Assert.Equal(expectedArea, function.CalculateArea(method, parameter), 8);
        Assert.Equal(expectedCentroidX, function.CalculateCentroid(Axis.X, method, parameter), 8);
        Assert.Equal(expectedCentroidY, function.CalculateCentroid(Axis.Y, method, parameter), 8);
        Assert.Equal(expectedArea * expectedCentroidX,
            function.CalculateFirstMoment(Axis.X, method, parameter), 8);
        Assert.Equal(expectedArea * expectedCentroidY,
            function.CalculateFirstMoment(Axis.Y, method, parameter), 8);
    }
}
