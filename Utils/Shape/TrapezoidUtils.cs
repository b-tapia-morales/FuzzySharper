using Shared.Approx;

namespace Utils.Shape;

public static class TrapezoidUtils
{
    public static double TrapezoidArea(double lowerBase, double upperBase, double height) =>
        (1 / 2.0) * height * (lowerBase + upperBase);

    public static double CalculateXCentroid(double lowerBase, double upperBase, double height) =>
        (1 / 3.0) * height * (2 * lowerBase + upperBase) / (lowerBase + upperBase);

    public static double CalculateYCentroid(double height) =>
        (1 / 2.0) * height;

    public static double CalculateSideCutArea(double a, double b, double c, double d, double h, double x0, double x1)
    {
        var vertices = ToVertices(a, b, c, d, h, x0, x1);
        PolygonUtils.OrderCounterClockwise(vertices);
        return PolygonUtils.CalculateArea(vertices);
    }

    public static List<(double X, double Y)> ToVertices(double a, double b, double c, double d, double h) =>
        ToVertices(a, b, c, d, h, a, d);

    public static List<(double X, double Y)> ToVertices(double a, double b, double c, double d, double h, double x0, double x1)
    {
        var vertices = new List<(double X, double Y)>();
        AddLeftVertices(vertices, a, b, c, h, x0);
        AddRightVertices(vertices, b, c, d, h, x1);
        return vertices;
    }

    private static void AddLeftVertices(List<(double X, double Y)> vertices, double a, double b, double c, double h, double x0)
    {
        // No cut is performed inside the trapezoid => vertices kept as is
        if (x0.IsRoughlyLesserOrEqualTo(a))
        {
            vertices.Add((a, 0));
            vertices.Add((b, h));
            return;
        }

        // Cut is performed exactly in the top left corner of the trapezoid => left triangle is cut out entirely
        if (x0.IsRoughlyGreaterOrEqualTo(b))
        {
            vertices.Add((b, 0));
            vertices.Add((b, h));
            return;
        }

        // Cut is performed after the top left corner, but before the top left corner => trapezoid is clipped from the left
        if (x0.IsRoughlyGreaterThan(b) && x0.IsRoughlyLesserThan(c))
        {
            vertices.Add((x0, 0));
            vertices.Add((x0, h));
            return;
        }

        // Cut is performed inside the left triangle => left vertices are (x0, 0), (x0, y) and (b, h), with y = h * (x0 - a) / (b - a)
        var y = h * (x0 - a) / (b - a);
        vertices.Add((x0, 0));
        vertices.Add((x0, y));
        vertices.Add((b, h));
    }

    private static void AddRightVertices(List<(double X, double Y)> vertices, double b, double c, double d, double h, double x1)
    {
        // No cut is performed inside the trapezoid => vertices kept as is
        if (x1.IsRoughlyGreaterOrEqualTo(d))
        {
            vertices.Add((c, h));
            vertices.Add((d, 0));
            return;
        }

        // Cut is performed exactly in the top right corner of the trapezoid => right triangle is cut out entirely
        if (x1.RoughlyEquals(c))
        {
            vertices.Add((c, h));
            vertices.Add((c, 0));
            return;
        }

        // Cut is performed before the top right corner => trapezoid is clipped from the right
        if (x1.IsRoughlyGreaterThan(b) && x1.IsRoughlyLesserThan(c))
        {
            vertices.Add((x1, h));
            vertices.Add((x1, 0));
            return;
        }

        // Cut is performed inside the right triangle => right vertices are (c, h), (x1, y) and (x1, 0), with y = h * (c - x1) / (c - b)
        var y = h * (c - x1) / (c - b);
        vertices.Add((c, h));
        vertices.Add((x1, y));
        vertices.Add((x1, 0));
    }
}