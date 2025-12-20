using Shared.Approx;

namespace Utils.Shape;

public static class TriangleUtils
{
    public static double CalculateArea(double @base, double height) =>
        @base * (height / 2);

    public static double CalculateXCentroid(double x0, double x1, double x2) =>
        (x0 + x1 + x2) / 3;

    public static double CalculateYCentroid(double y0, double y1, double y2) =>
        (y0 + y1 + y2) / 3;

    public static double CalculateSideCutArea(double a, double b, double c, double h, double x0, double x1) =>
        CalculateLeftCutArea(a, b, h, x0) + CalculateRightCutArea(b, c, h, x1);

    public static List<(double X, double Y)> ToVertices(double a, double b, double c, double h) =>
        ToVertices(a, b, c, h, a, c);

    public static List<(double X, double Y)> ToVertices(double a, double b, double c, double h, double x0, double x1)
    {
        var vertices = new List<(double X, double Y)>();
        AddLeftVertices(vertices, a, b, h, x0);
        vertices.Add((b, h));
        AddRightVertices(vertices, b, c, h, x1);
        return vertices;
    }

    private static double CalculateLeftCutArea(double a, double b, double h, double x0)
    {
        // No cut is performed inside => left area kept as is
        if (x0.IsRoughlyLesserOrEqualTo(a))
            return CalculateArea(b - a, h);
        // Cut is performed exactly at the triangle's peak => left area is entirely cut out
        if (x0.IsRoughlyGreaterOrEqualTo(b))
            return 0;
        // Cut is performed inside => the intersection point y0 has to be calculated; the remaining area will be the sum of
        // rectangle sitting at the base's area, and the area of the triangle sitting above it.
        var y1 = (x0 - a) / (b - a);
        var rectangle = (b - x0) * y1;
        var triangle = CalculateArea(b - x0, h - y1);
        return rectangle + triangle;
    }

    private static double CalculateRightCutArea(double b, double c, double h, double x1)
    {
        // No cut is performed inside => right area kept as is
        if (x1.IsRoughlyLesserOrEqualTo(b))
            return CalculateArea(c - b, h);
        // Cut is performed exactly at the triangle's peak => right area is entirely cut out
        if (x1.IsRoughlyGreaterOrEqualTo(c))
            return 0;
        // Cut is performed inside => the intersection point y1 has to be calculated; the remaining area will be the sum of
        // rectangle sitting at the base's area, and the area of the triangle sitting above it.
        var y2 = (c - x1) / (c - b);
        var rectangleArea = (x1 - b) * y2;
        var triangleArea = CalculateArea(x1 - b, h - y2);
        return rectangleArea + triangleArea;
    }

    private static void AddLeftVertices(List<(double X, double Y)> vertices, double a, double b, double h, double x0)
    {
        // Cut is performed exactly at the triangle's peak => no vertices added
        if (x0.RoughlyEquals(b))
            return;
        // No cut is performed inside the triangle => left vertex kept as is
        if (x0.IsRoughlyLesserOrEqualTo(a))
        {
            vertices.Add((b, 0));
            return;
        }

        // Cut is performed inside the triangle => left vertices are (x0, 0) and (x0, y), with y = (x0 - a) / (b - a)
        var y = h * (x0 - a) / (b - a);
        vertices.Add((x0, 0));
        vertices.Add((x0, y));
    }

    private static void AddRightVertices(List<(double X, double Y)> vertices, double b, double c, double h, double x1)
    {
        // Cut is performed exactly at the triangle's peak => no vertices added
        if (x1.RoughlyEquals(b))
            return;
        // No cut is performed inside the triangle => right vertex kept as is
        if (x1.IsRoughlyGreaterOrEqualTo(c))
        {
            vertices.Add((b, 0));
            return;
        }

        // Cut is performed inside the triangle => right vertices are (x1, 0) and (x1, y), with y = (c - x1) / (c - b)
        var y = h * (c - x1) / (c - b);
        vertices.Add((x1, 0));
        vertices.Add((x1, y));
    }
}