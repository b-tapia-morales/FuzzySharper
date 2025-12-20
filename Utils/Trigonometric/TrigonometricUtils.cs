using static System.Math;

namespace Utils.Trigonometric;

public static class TrigonometricUtils
{
    public static double Distance((double X1, double Y1) v1, (double X2, double Y2) v2) =>
        Sqrt(Pow(v2.X2 - v1.X1, 2) + Pow(v2.Y2 - v1.Y1, 2));

    public static double RightTriangleHypotenuse(double @base, double height) =>
        Sqrt(Pow(@base, 2) + Pow(height, 2));
}