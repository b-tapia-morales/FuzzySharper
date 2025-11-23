using Shared.Approx;

namespace Utils.Shape;

public static class PolygonUtils
{
    public static void OrderCounterClockwise(List<(double X, double Y)> vertices)
    {
        // Using a centroid-ish point as a reference point for convenience, it's not the true centroid.
        // The only condition for a point to be a valid reference point is that it lies inside the shape.
        var refX = vertices.Average(v => v.X);
        var refY = vertices.Average(v => v.Y);

        // Sort by atan2 of (y - cy, x - cx).
        vertices.Sort((a, b) =>
        {
            var angleA = Math.Atan2(a.Y - refY, a.X - refX);
            var angleB = Math.Atan2(b.Y - refY, b.X - refX);

            // In ascending order.
            return angleA.CompareTo(angleB);
        });
    }

    public static double CalculateArea(IList<(double X, double Y)> vertices)
    {
        ArgumentNullException.ThrowIfNull(vertices);

        if (vertices.Count < 3)
            throw new ArgumentException("A polygon must have at least three vertices", nameof(vertices));

        var sum = 0.0;
        var n = vertices.Count;
        for (var i = 0; i < n; i++)
        {
            var (xi, yi) = vertices[i];
            var (xj, yj) = vertices[(i + 1) % n];
            sum += xi * yj - xj * yi;
        }

        return Math.Abs((1 / 2.0) * sum);
    }

    public static double CalculateCentroidX(List<(double X, double Y)> vertices) =>
        CalculateCentroid(vertices, Axis.X);

    public static double CalculateCentroidY(List<(double X, double Y)> vertices) =>
        CalculateCentroid(vertices, Axis.Y);

    public static double CalculateCentroid(List<(double X, double Y)> vertices, Axis axis)
    {
        var area = CalculateArea(vertices);
        if (area.IsRoughlyZero())
            throw new ArgumentException("The area of the polygon is roughly zero", nameof(vertices));

        var moment = CalculateFirstMoment(vertices, axis);
        return moment / area;
    }

    public static double CalculateFirstMoment(List<(double X, double Y)> vertices, Axis axis)
    {
        if (!Enum.IsDefined(axis))
            throw new ArgumentException("Invalid constant value", nameof(axis));

        ArgumentNullException.ThrowIfNull(vertices);

        if (vertices.Count < 3)
            throw new ArgumentException("A polygon must have at least three vertices", nameof(vertices));

        OrderCounterClockwise(vertices);

        var crossWeightedSum = 0.0;

        var n = vertices.Count;
        for (var i = 0; i < n; i++)
        {
            var v0 = vertices[i];
            var v1 = vertices[(i + 1) % n];

            var sum = axis == Axis.X ? v0.X + v1.X : v0.Y + v1.Y;
            var cross = v0.X * v1.Y - v1.X * v0.Y;
            crossWeightedSum += sum * cross;
        }

        return (1 / 6.0) * crossWeightedSum;
    }

    public static double CalculateSecondMoment(List<(double X, double Y)> vertices, Axis firstAxis, Axis secondAxis)
    {
        if (!Enum.IsDefined(firstAxis) || !Enum.IsDefined(secondAxis))
            throw new ArgumentException("Invalid constant value");

        ArgumentNullException.ThrowIfNull(vertices);

        if (vertices.Count < 3)
            throw new ArgumentException("A polygon must have at least three vertices", nameof(vertices));

        OrderCounterClockwise(vertices);

        var crossWeightedSum = 0.0;

        var n = vertices.Count;
        for (var i = 0; i < n; i++)
        {
            var v0 = vertices[i];
            var v1 = vertices[(i + 1) % n];

            var sum = (firstAxis, secondAxis) switch
            {
                (Axis.X, Axis.X) => Math.Pow(v0.Y, 2) + v0.Y * v1.Y + Math.Pow(v1.Y, 2),
                (Axis.Y, Axis.Y) => Math.Pow(v0.X, 2) + v0.X * v1.X + Math.Pow(v1.X, 2),
                _ => v0.X * v1.Y + 2 * (v0.X * v0.Y + v1.X * v1.Y) + v1.X * v0.Y
            };
            var cross = v0.X * v1.Y - v1.X * v0.Y;
            crossWeightedSum += sum * cross;
        }

        return (1 / 24.0) * crossWeightedSum;
    }
}