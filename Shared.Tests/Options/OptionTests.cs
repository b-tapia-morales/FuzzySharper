using Shared.Options.Exceptions;
using Shared.Options.Extensions;
using Shared.Options.Factory;

namespace Shared.Tests.Options;

public class OptionTests
{
    [Fact]
    public void SameValuesYieldEquality()
    {
        var opt1 = OptionFactory.SomeVal(1);
        var opt2 = OptionFactory.SomeVal(1);
        Assert.Equal(opt1, opt2);
    }

    [Fact]
    public void NullEqualsNone()
    {
        string? s = null;
        var actual = OptionFactory.MaybeRef(s);
        var expected = OptionFactory.None<string>();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void NullableEqualsNone()
    {
        double? x = null;
        var actual = OptionFactory.MaybeVal(x);
        var expected = OptionFactory.None<double>();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ExceptionThrownIfNoneFromOrElseThrow()
    {
        double? x = null;
        var opt = OptionFactory.MaybeVal(x);
        Assert.ThrowsAny<Exception>(() => opt.OrElseThrow(new InvalidOperationException()));
        Assert.ThrowsAny<Exception>(() => opt.OrElseThrow(() => new InvalidOperationException()));
    }
    
    [Fact]
    public void OrElsePreservesValueIfSome()
    {
        double? x = 1.0;
        var expected = OptionFactory.MaybeVal(1.0);
        var actual1 = OptionFactory.MaybeVal(x).OrElse(2.0);
        var actual2 = OptionFactory.MaybeVal(x).OrElse(() => 2.0);
        Assert.Equal(expected, actual1);
        Assert.Equal(expected, actual2);       
    }
    
    [Fact]
    public void OrElseReplacesValueIfNone()
    {
        double? x = null;
        var expected = OptionFactory.MaybeVal(1.0);
        var actual1 = OptionFactory.MaybeVal(x).OrElse(1.0);
        var actual2 = OptionFactory.MaybeVal(x).OrElse(() => 1.0);
        Assert.Equal(expected, actual1);
        Assert.Equal(expected, actual2);
    }

    [Fact]
    public void SelectPerformsIfSome()
    {
        const string firstName = "John";
        const string lastName = "Doe";
        var actual = OptionFactory.MaybeRef(firstName).Select(name => $"{name} {lastName}");
        var expected = OptionFactory.SomeRef($"{firstName} {lastName}");
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void SelectDoesNothingIfNone()
    {
        string? firstName = null;
        const string lastName = "Doe";
        var actual = OptionFactory.MaybeRef(firstName).Select(name => $"{name} {lastName}");
        var expected = OptionFactory.None<string>();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void WhereIsSomeIfPredicateIsTrue()
    {
        const string fullName = "John Doe";
        var opt = OptionFactory.MaybeRef(fullName);
        var actual = opt.Where(s => s.Contains("Doe"));
        var expected = opt;
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void WhereIsNoneIfPredicateIsFalse()
    {
        const string fullName = "John Doe";
        var opt = OptionFactory.SomeRef(fullName);
        var actual = opt.Where(s => s.Contains("Jones"));
        var expected = OptionFactory.None<string>();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void NoneThrowsOnNullableTypes()
    {
        Assert.Throws<NullableTypeException>(OptionFactory.None<double?>);
        Assert.Throws<NullableTypeException>(OptionFactory.None<int?>);
    }
}