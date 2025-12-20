using Shared.Options.Exceptions;
using Shared.Options.Extensions;
using Shared.Options.Factory;
using Shared.Options.Implementations;

namespace Shared.Tests.Options;

public class OptionTests
{
    [Fact]
    public void SameValuesYieldEquality()
    {
        var opt1 = Option<int>.Some(1);
        var opt2 = Option<int>.Some(1);
        Assert.Equal(opt1, opt2);
    }

    [Fact]
    public void NullEqualsNone()
    {
        string? s = null;
        var actual = Option<string>.Maybe(s);
        var expected = Option<string>.None();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void NullableEqualsNone()
    {
        double? x = null;
        var actual = Option<double>.MaybeNullable(x);
        var expected = Option<double>.None();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ExceptionThrownIfNoneFromOrElseThrow()
    {
        double? x = null;
        var opt = Option<double>.MaybeNullable(x);
        Assert.ThrowsAny<Exception>(() => opt.OrElseThrow(new InvalidOperationException()));
        Assert.ThrowsAny<Exception>(() => opt.OrElseThrow(() => new InvalidOperationException()));
    }

    [Fact]
    public void OrElsePreservesValueIfSome()
    {
        double? x = 1.0;
        var expected = Option<double>.Maybe(1.0);
        var actual1 = Option<double>.MaybeNullable(x).OrElse(2.0);
        var actual2 = Option<double>.MaybeNullable(x).OrElse(() => 2.0);
        Assert.Equal(expected, actual1);
        Assert.Equal(expected, actual2);
    }

    [Fact]
    public void OrElseReplacesValueIfNone()
    {
        double? x = null;
        var expected = Option<double>.Maybe(1.0);
        var actual1 = Option<double>.MaybeNullable(x).OrElse(1.0);
        var actual2 = Option<double>.MaybeNullable(x).OrElse(() => 1.0);
        Assert.Equal(expected, actual1);
        Assert.Equal(expected, actual2);
    }

    [Fact]
    public void SelectPerformsIfSome()
    {
        const string firstName = "John";
        const string lastName = "Doe";
        var actual = Option<string>.Maybe(firstName).Select(name => $"{name} {lastName}");
        var expected = Option<string>.Some($"{firstName} {lastName}");
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void SelectDoesNothingIfNone()
    {
        string? firstName = null;
        const string lastName = "Doe";
        var actual = Option<string>.Maybe(firstName).Select(name => $"{name} {lastName}");
        var expected = Option<string>.None();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void WhereIsSomeIfPredicateIsTrue()
    {
        const string fullName = "John Doe";
        var opt = Option<string>.Maybe(fullName);
        var actual = opt.Where(s => s.Contains("Doe"));
        Assert.Equal(opt, actual);
    }

    [Fact]
    public void WhereIsNoneIfPredicateIsFalse()
    {
        const string fullName = "John Doe";
        var opt = Option<string>.Some(fullName);
        var actual = opt.Where(s => s.Contains("Jones"));
        var expected = Option<string>.None();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ThrowsOnNullableTypes()
    {
        Assert.Throws<NullableTypeException>(() => Option<int?>.Some(1));
        Assert.Throws<NullableTypeException>(() => Option<double?>.Some(1.0));
        Assert.Throws<NullableTypeException>(() => Option<int?>.Maybe(1));
        Assert.Throws<NullableTypeException>(() => Option<double?>.Maybe(1.0));
        Assert.Throws<NullableTypeException>(Option<double?>.None);
        Assert.Throws<NullableTypeException>(Option<int?>.None);
    }
}