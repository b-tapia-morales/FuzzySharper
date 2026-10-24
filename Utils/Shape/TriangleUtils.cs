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

    public static double CalculateSideCutArea(double a, double b, double c, double h, double x0, double x1)
    {
        var vertices = ToVertices(a, b, c, h, x0, x1);
        return vertices.Count < 3 ? 0 : PolygonUtils.CalculateArea(vertices);
    }

    public static List<(double X, double Y)> ToVertices(double a, double b, double c, double h) =>
        ToVertices(a, b, c, h, a, c);

    public static List<(double X, double Y)> ToVertices(double a, double b, double c, double h, double x0, double x1)
    {
        var left = Math.Max(a, x0);
        var right = Math.Min(c, x1);

        if (left >= right)
            return [];

        var vertices = new List<(double X, double Y)>();
        
        AddBoundaryVertices(vertices, a, b, c, h, left);
        if (left < b && b < right)
            vertices.Add((b, h));
        AddBoundaryVertices(vertices, a, b, c, h, right);

        PolygonUtils.OrderCounterClockwise(vertices);
        return vertices;
    }

    private static void AddBoundaryVertices(List<(double X, double Y)> vertices, double a, double b, double c, double h, double x)
    {
        vertices.Add((x, 0));
        var y = HeightAt(a, b, c, h, x);
        if (y != 0)
            vertices.Add((x, y));
    }

    private static double HeightAt(double a, double b, double c, double h, double x)
    {
        if (x < b)
            return h * (x - a) / (b - a);
        if (x > b)
            return h * (c - x) / (c - b);
        return h;
    }
}
