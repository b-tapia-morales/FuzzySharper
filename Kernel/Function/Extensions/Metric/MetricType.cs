namespace Kernel.Function.Extensions.Metric;

/// <summary>
/// Identifies geometric metrics of a planar region. Moments are raw coordinate moments about the origin;
/// their suffixes identify the coordinate factors integrated over the region.
/// </summary>
public enum MetricType
{
    /// <summary>The area of the region.</summary>
    Area,
    /// <summary>The integral of x over the region.</summary>
    MomentX,
    /// <summary>The integral of y over the region.</summary>
    MomentY,
    /// <summary>The integral of x squared over the region.</summary>
    MomentXx,
    /// <summary>The integral of x times y over the region.</summary>
    MomentXy,
    /// <summary>The integral of y squared over the region.</summary>
    MomentYy,
    /// <summary>The X coordinate of the region's centroid.</summary>
    CentroidX,
    /// <summary>The Y coordinate of the region's centroid.</summary>
    CentroidY
}
