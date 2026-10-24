using Utils.Shape;

namespace Kernel.Tests.Function;

public class TriangleGeometryTests
{
    [Theory]
    [InlineData(0, 5, 10, 1, 0, 10, 5, 3)]
    [InlineData(0, 5, 10, 1, 2, 8, 4.2, 5)]
    [InlineData(0, 5, 10, 1, 6, 10, 1.6, 3)]
    [InlineData(0, 5, 10, 1, 0, 4, 1.6, 3)]
    [InlineData(0, 5, 10, 1, 6, 8, 1.2, 4)]
    [InlineData(0, 5, 10, 1, 5, 10, 2.5, 3)]
    [InlineData(0, 5, 10, 1, 0, 5, 2.5, 3)]
    [InlineData(0, 5, 10, 0.4, 2, 8, 1.68, 5)]
    [InlineData(0, 0, 10, 1, 0, 10, 5, 3)]
    [InlineData(0, 10, 10, 1, 0, 10, 5, 3)]
    [InlineData(0, 0, 10, 0.4, 2, 8, 1.2, 4)]
    [InlineData(0, 10, 10, 0.4, 2, 8, 1.2, 4)]
    public void ClippedTriangle_HasExpectedAreaAndCounterClockwiseVertices(
        double a, double b, double c, double height, double left, double right, double area, int vertexCount)
    {
        var vertices = TriangleUtils.ToVertices(a, b, c, height, left, right);

        Assert.Equal(vertexCount, vertices.Count);
        Assert.Equal(vertices.Count, vertices.Distinct().Count());
        Assert.All(vertices, vertex => Assert.InRange(vertex.X, left, right));
        Assert.Equal(area, PolygonUtils.CalculateArea(vertices), 10);
        Assert.Equal(area, TriangleUtils.CalculateSideCutArea(a, b, c, height, left, right), 10);

        var signedDoubleArea = 0d;
        for (var i = 0; i < vertices.Count; i++)
        {
            var next = vertices[(i + 1) % vertices.Count];
            signedDoubleArea += vertices[i].X * next.Y - next.X * vertices[i].Y;
        }
        Assert.Equal(2 * area, signedDoubleArea, 10);
    }

    [Theory]
    [InlineData(10, 15)]
    [InlineData(-5, 0)]
    [InlineData(5, 5)]
    public void Triangle_EmptyIntersection_HasNoVerticesAndZeroArea(double left, double right)
    {
        Assert.Empty(TriangleUtils.ToVertices(0, 5, 10, 1, left, right));
        Assert.Equal(0, TriangleUtils.CalculateSideCutArea(0, 5, 10, 1, left, right));
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(2, 8)]
    [InlineData(6, 8)]
    public void Triangle_ZeroHeight_AddsNoDuplicateVertices(double left, double right)
    {
        var vertices = TriangleUtils.ToVertices(0, 5, 10, 0, left, right);

        Assert.Equal(vertices.Count, vertices.Distinct().Count());
        Assert.All(vertices, vertex => Assert.Equal(0, vertex.Y));
        Assert.Equal(0, TriangleUtils.CalculateSideCutArea(0, 5, 10, 0, left, right));
    }
}
