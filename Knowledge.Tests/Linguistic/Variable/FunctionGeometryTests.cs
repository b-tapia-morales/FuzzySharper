using Kernel.Function.Extensions;
using Kernel.Function.Implementations;
using Kernel.Function.Implication.Factory;
using Knowledge.Linguistic.Variable.Implementations;
using Shared.Intervals.Implementations;
using Utils.Shape;

namespace Knowledge.Tests.Linguistic.Variable;

public class FunctionGeometryTests
{
    [Theory]
    [InlineData(false, ImplicationMethod.Mamdani, 2, 8, true, 2.2775)]
    [InlineData(true, ImplicationMethod.Mamdani, 2, 8, true, 2.2775)]
    [InlineData(false, ImplicationMethod.Mamdani, 2, 8, false, 3.4375)]
    [InlineData(true, ImplicationMethod.Mamdani, 2, 8, false, 3.4375)]
    [InlineData(false, ImplicationMethod.Larsen, 2, 8, true, 1.2)]
    [InlineData(true, ImplicationMethod.Larsen, 2, 8, true, 1.2)]
    [InlineData(false, ImplicationMethod.Larsen, 2, 8, false, 2)]
    [InlineData(true, ImplicationMethod.Larsen, 2, 8, false, 2)]
    [InlineData(false, ImplicationMethod.Mamdani, 0, 5, true, 1)]
    [InlineData(true, ImplicationMethod.Mamdani, 5, 10, true, 1)]
    [InlineData(false, ImplicationMethod.Mamdani, 7, 10, true, 1.5)]
    [InlineData(true, ImplicationMethod.Mamdani, 0, 3, true, 1.5)]
    [InlineData(false, ImplicationMethod.Mamdani, 0, 6.25, true, 1.5625)]
    [InlineData(true, ImplicationMethod.Mamdani, 3.75, 10, true, 1.5625)]
    [InlineData(false, ImplicationMethod.Mamdani, 6.25, 10, true, 1.875)]
    [InlineData(true, ImplicationMethod.Mamdani, 0, 3.75, true, 1.875)]
    public void ShoulderImplication_RespectsUniverseAndAlphaCutIntersection(
        bool isLeft, ImplicationMethod method, double lower, double upper, bool useUoD, double expectedArea)
    {
        var variable = LinguisticVariable.Create("Variable", new Interval(lower, upper));
        if (isLeft)
            variable.AddLeftTrapezoidFunction("Shoulder", 0, 10, 0.8);
        else
            variable.AddRightTrapezoidFunction("Shoulder", 0, 10, 0.8);
        var function = variable.GetMapping("Shoulder").Get;

        Assert.Equal(expectedArea, function.CalculateArea(method, 0.5, useUoD), 8);
    }

    [Theory]
    [InlineData(true, 1, 10.0 / 3, 12.5, 2.0 / 15)]
    [InlineData(false, 4, 80.0 / 3, 200, 4.0 / 15)]
    public void ShoulderWithoutImplication_ForwardsUniverseChoiceToEachMetricFamily(
        bool useUoD, double expectedArea, double expectedMomentX,
        double expectedMomentXx, double expectedCentroidY)
    {
        var variable = LinguisticVariable.Create("Variable", new Interval(0, 5))
            .AddRightTrapezoidFunction("Right", 0, 10, 0.8);
        var function = variable.GetMapping("Right").Get;

        Assert.Equal(expectedArea, function.CalculateArea(useUoD), 8);
        Assert.Equal(expectedMomentX, function.CalculateFirstMoment(Axis.X, useUoD), 8);
        Assert.Equal(expectedMomentXx, function.CalculateSecondMoment(Axis.X, Axis.X, useUoD), 8);
        Assert.Equal(expectedCentroidY, function.CalculateCentroid(Axis.Y, useUoD), 8);
    }

    [Theory]
    [InlineData(0, 10, true, 3.75, 5)]
    [InlineData(1, 10, true, 3.65, 1121.0 / 219)]
    [InlineData(0, 9, true, 3.65, 1069.0 / 219)]
    [InlineData(1, 9, true, 3.55, 5)]
    [InlineData(1, 10, false, 3.75, 5)]
    [InlineData(0, 9, false, 3.75, 5)]
    [InlineData(1, 9, false, 3.75, 5)]
    public void MamdaniTriangle_ClipsOnlyRequestedSides(
        double lower, double upper, bool useUoD, double expectedArea, double expectedCentroid)
    {
        var variable = LinguisticVariable.Create("Variable", new Interval(lower, upper))
            .AddTriangularFunction("Triangle", 0, 5, 10);
        var function = variable.GetMapping("Triangle").Get;

        var area = function.CalculateArea(ImplicationMethod.Mamdani, 0.5, useUoD);
        var centroid = function.CalculateCentroid(Axis.X, ImplicationMethod.Mamdani, 0.5, useUoD);

        Assert.Equal(expectedArea, area, 8);
        Assert.Equal(expectedCentroid, centroid, 8);
    }

    [Fact]
    public void AddRightTrapezoidFunction_CreatesIncreasingShoulder()
    {
        var variable = LinguisticVariable.Create("Variable", new Interval(0, 10))
            .AddRightTrapezoidFunction("Right", 2, 8, 0.5);

        var function = Assert.IsType<RightTrapezoidFunction>(variable.GetMapping("Right").Get);

        Assert.Equal(0, function.PureFunction(2));
        Assert.Equal(0.5, function.PureFunction(8));
        Assert.Equal(variable.UniverseOfDiscourse.ToTuple(), function.UniverseOfDiscourse.ToTuple());
    }

    [Theory]
    [InlineData(0, 10, 5, 5)]
    [InlineData(6, 10, 6, 6)]
    [InlineData(0, 4, 4, 4)]
    [InlineData(6, double.PositiveInfinity, 6, 6)]
    [InlineData(double.NegativeInfinity, 4, 4, 4)]
    public void TrianglePeak_ClampsBothEndpointsToUniverse(double lower, double upper, double expectedLeft, double expectedRight)
    {
        var variable = LinguisticVariable.Create("Variable", new Interval(lower, upper))
            .AddTriangularFunction("Triangle", 0, 5, 10);

        var peak = variable.GetMapping("Triangle").Get.PeakClipped.Get;

        Assert.Equal(expectedLeft, peak.LowerBound);
        Assert.Equal(expectedRight, peak.UpperBound);
    }

    [Theory]
    [InlineData(0, 10, 3, 7)]
    [InlineData(4, 6, 4, 6)]
    [InlineData(8, 10, 8, 8)]
    [InlineData(0, 2, 2, 2)]
    public void TrapezoidPeak_ClampsPlateauToUniverse(double lower, double upper, double expectedLeft, double expectedRight)
    {
        var variable = LinguisticVariable.Create("Variable", new Interval(lower, upper))
            .AddTrapezoidFunction("Trapezoid", 0, 3, 7, 10);

        var peak = variable.GetMapping("Trapezoid").Get.PeakClipped.Get;

        Assert.Equal(expectedLeft, peak.LowerBound);
        Assert.Equal(expectedRight, peak.UpperBound);
    }

    [Fact]
    public void Logistic_WithoutGlobalPeak_HasNoClippedPeak()
    {
        var variable = LinguisticVariable.Create("Variable", new Interval(0, 1))
            .AddLogisticFunction("Logistic", 1, 0);

        var function = variable.GetMapping("Logistic").Get;

        Assert.True(function.PeakLeftClipped.IsNone);
        Assert.True(function.PeakRightClipped.IsNone);
        Assert.True(function.PeakClipped.IsNone);
    }
}
