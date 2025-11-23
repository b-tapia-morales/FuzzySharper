using Shared.Intervals.Extensions;
using Shared.Intervals.Implementations;
using Xunit.Abstractions;

namespace Shared.Tests.Intervals;

public class IntervalTests(ITestOutputHelper output)
{
    private static readonly List<Interval> Intervals =
    [
        new(0, 1),
        new(2, 3),
        new(3, 6),
        new(4, 8),
        new(9, 12),
        new(11, 15),
        new(16, 20),
        new(18, 22),
        new(23, 27),
        new(28, 30),
        new(29, 35),
        new(36, 40)
    ];
    
    private static readonly List<Interval> Intersections =
    [
        new(0, 1),
        new(2, 8),
        new(9, 15),
        new(16, 22),
        new(23, 27),
        new(28, 35),
        new(36, 40)
    ];
    
    private static readonly List<Interval> Gaps =
    [
        new(1, 2),
        new(8, 9),
        new(15, 16),
        new(22, 23),
        new(27, 28),
        new(35, 36)
    ];

    [Fact]
    public void FindsIntersectionsCorrectly()
    {
        var expected = Intersections;
        var actual = Intervals.FindIntersections().ToList();
        output.WriteLine(string.Join(", ", expected));
        output.WriteLine(string.Join(", ", actual));
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void ClipsToUniverseCorrectly()
    {
        var firstExpected = new Interval(2, 8);
        var secondExpected = new Interval(36, 37);
        var universe = new Interval(2, 37);
        var intervals = Intervals.FindIntersections().ClipTo(universe).ToList();
        output.WriteLine(string.Join(", ", intervals));
        Assert.Equal(firstExpected, intervals[0]);
        Assert.Equal(secondExpected, intervals[^1]);
    }
    
    [Fact]
    public void FindsGapsCorrectly()
    {
        var expected = Gaps;
        var actual = Intervals.FindGaps().ToList();
        output.WriteLine(string.Join(", ", expected));
        output.WriteLine(string.Join(", ", actual));
        Assert.Equal(expected, actual);
    }
}