using Shared.Options.Exceptions;
using Shared.Options.Abstractions;
using Shared.Options.Extensions;
using Shared.Options.Factory;
using Shared.Options.Implementations;

#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.

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
        var opt = Option<double>.None();
        var expected = new InvalidOperationException("Value required");
        var actual = Assert.Throws<InvalidOperationException>(() => opt.OrElseThrow(expected));
        Assert.Same(expected, actual);
    }

    [Fact]
    public void OrElsePreservesValueIfSome()
    {
        double? x = 1.0;
        const double expected = 1.0;
        var calls = 0;
        var actual1 = Option<double>.MaybeNullable(x).OrElse(2.0);
        var actual2 = Option<double>.MaybeNullable(x).OrElse(() =>
        {
            calls++;
            return 2.0;
        });
        Assert.Equal(expected, actual1);
        Assert.Equal(expected, actual2);
        Assert.Equal(0, calls);
    }

    [Fact]
    public void OrElseReplacesValueIfNone()
    {
        double? x = null;
        const double expected = 1.0;
        var calls = 0;
        var actual1 = Option<double>.MaybeNullable(x).OrElse(1.0);
        var actual2 = Option<double>.MaybeNullable(x).OrElse(() =>
        {
            calls++;
            return 1.0;
        });
        Assert.Equal(expected, actual1);
        Assert.Equal(expected, actual2);
        Assert.Equal(1, calls);
    }

    [Fact]
    public void SelectPerformsIfSome()
    {
        const string firstName = "John";
        const string lastName = "Doe";
        var calls = 0;
        var actual = Option<string>.Maybe(firstName).Select(name =>
        {
            calls++;
            Assert.Equal(firstName, name);
            return $"{name} {lastName}";
        });
        var expected = Option<string>.Some($"{firstName} {lastName}");
        Assert.Equal(expected, actual);
        Assert.Equal(1, calls);
    }

    [Fact]
    public void SelectDoesNothingIfNone()
    {
        string? firstName = null;
        const string lastName = "Doe";
        var calls = 0;
        var actual = Option<string>.Maybe(firstName).Select(name =>
        {
            calls++;
            return $"{name} {lastName}";
        });
        var expected = Option<string>.None();
        Assert.Equal(expected, actual);
        Assert.Equal(0, calls);
    }

    [Fact]
    public void WhereIsSomeIfPredicateIsTrue()
    {
        const string fullName = "John Doe";
        var opt = Option<string>.Maybe(fullName);
        var calls = 0;
        var actual = opt.Where(s =>
        {
            calls++;
            Assert.Equal(fullName, s);
            return s.Contains("Doe");
        });
        Assert.Equal(opt, actual);
        Assert.Equal(1, calls);
    }

    [Fact]
    public void WhereIsNoneIfPredicateIsFalse()
    {
        const string fullName = "John Doe";
        var opt = Option<string>.Some(fullName);
        var calls = 0;
        var actual = opt.Where(s =>
        {
            calls++;
            Assert.Equal(fullName, s);
            return s.Contains("Jones");
        });
        var expected = Option<string>.None();
        Assert.Equal(expected, actual);
        Assert.Equal(1, calls);
    }

    [Fact]
    public void ThrowsOnNullableTypes()
    {
        Assert.Throws<NullableTypeException>(() => Option<int?>.Some(1));
        Assert.Throws<NullableTypeException>(() => Option<double?>.Some(1.0));
        Assert.Throws<NullableTypeException>(Option<double?>.None);
        Assert.Throws<NullableTypeException>(Option<int?>.None);
    }

    [Fact]
    public void SomePreservesDefaultValues()
    {
        Assert.Equal(0, Option<int>.Some(0).Get);
        Assert.False(Option<bool>.Some(false).Get);
        Assert.Equal(string.Empty, Option<string>.Some(string.Empty).Get);
    }

    [Fact]
    public void SomePreservesTheSuppliedReference()
    {
        var value = new object();
        var option = Option<object>.Some(value);

        Assert.True(option.IsSome);
        Assert.False(option.IsNone);
        Assert.Same(value, option.Get);
    }

    [Fact]
    public void SomeRejectsNullReferences()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => Option<string>.Some(null!));

        Assert.Equal("value", exception.ParamName);
    }

    [Fact]
    public void SomeNullableRejectsMissingValues()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => Option<int>.SomeNullable(null));

        Assert.Equal("value", exception.ParamName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(-1)]
    public void SomeNullablePreservesPresentValues(int value)
    {
        var option = Option<int>.SomeNullable(value);

        Assert.True(option.IsSome);
        Assert.Equal(value, option.Get);
    }

    [Theory]
    [InlineData("")]
    [InlineData("value")]
    public void MaybePreservesNonNullValues(string value)
    {
        var option = Option<string>.Maybe(value);

        Assert.True(option.IsSome);
        Assert.Equal(value, option.Get);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(-1)]
    public void MaybeNullablePreservesPresentValues(int value)
    {
        var option = Option<int>.MaybeNullable(value);

        Assert.True(option.IsSome);
        Assert.Equal(value, option.Get);
    }

    [Fact]
    public void NoneHasNoValueAndGetThrows()
    {
        var option = Option<int>.None();

        Assert.False(option.IsSome);
        Assert.True(option.IsNone);
        Assert.Throws<MissingValueException>(() => option.Get);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void IsSomeReportsWhetherItSuppliedAValue(bool hasValue)
    {
        const int missingValue = 0;
        const int defaultValue = 10;
        var option = hasValue ? Option<int>.Some(defaultValue) : Option<int>.None();

        var success = option.IsSome(out var value);

        Assert.Equal(hasValue, success);
        Assert.Equal(hasValue ? defaultValue : missingValue, value);
    }

    [Theory]
    [InlineData(true, 1, true, 1, true)]
    [InlineData(true, 1, true, 2, false)]
    [InlineData(true, 0, false, 0, false)]
    [InlineData(false, 0, true, 0, false)]
    [InlineData(false, 0, false, 0, true)]
    public void EqualityComparesBothStateAndValue(
        bool firstHasValue, int firstValue, bool secondHasValue, int secondValue, bool equals)
    {
        var first = firstHasValue ? Option<int>.Some(firstValue) : Option<int>.None();
        var second = secondHasValue ? Option<int>.Some(secondValue) : Option<int>.None();

        Assert.Equal(equals, first.Equals(second));
        Assert.Equal(equals, first.Equals((object) second));
        Assert.Equal(equals, second.Equals(first));
    }

    [Fact]
    public void EqualityRejectsNullAndUnrelatedObjects()
    {
        var option = Option<int>.Some(1);

        Assert.False(option.Equals((object?) null));
        // ReSharper disable once SuspiciousTypeConversion.Global
        Assert.False(option.Equals((object) 1));
    }

    [Theory]
    [InlineData(false, false, true)]
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    [InlineData(true, true, true)]
    public void EqualityComparerHandlesNullReferences(bool firstExists, bool secondExists, bool equals)
    {
        var comparer = EqualityComparer<Option<int>>.Default;
        var first = firstExists ? Option<int>.Some(1) : null;
        var second = secondExists ? Option<int>.Some(1) : null;

        Assert.Equal(equals, comparer.Equals(first, second));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void EqualOptionsHaveEqualHashes(bool hasValue)
    {
        const int defaultValue = 10;
        var first = hasValue ? Option<int>.Some(defaultValue) : Option<int>.None();
        var second = hasValue ? Option<int>.Some(defaultValue) : Option<int>.None();
        var comparer = EqualityComparer<Option<int>>.Default;

        Assert.True(first.Equals(second));
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
        Assert.Equal(comparer.GetHashCode(first), comparer.GetHashCode(second));
        Assert.Equal(first.GetHashCode(), comparer.GetHashCode(second));
        Assert.Equal(second.GetHashCode(), comparer.GetHashCode(first));
    }

    [Fact]
    public void HashSetRecognizesEqualOptions()
    {
        const int defaultValue = 10;
        var options = new HashSet<Option<int>>
        {
            Option<int>.Some(defaultValue),
            Option<int>.Some(defaultValue),
            Option<int>.None(),
            Option<int>.None()
        };

        Assert.Equal(2, options.Count);
        Assert.Contains(Option<int>.Some(defaultValue), options);
        Assert.Contains(Option<int>.None(), options);
    }

    [Fact]
    public void OrElseThrowCallsTheExceptionSupplierOnceWhenNone()
    {
        var calls = 0;
        var expected = new InvalidOperationException("Value required");

        var actual = Assert.Throws<InvalidOperationException>(() => Option<int>.None().OrElseThrow(() =>
        {
            calls++;
            return expected;
        }));

        Assert.Same(expected, actual);
        Assert.Equal(1, calls);
    }

    [Fact]
    public void OrElseThrowPreservesSomeAndSkipsTheExceptionSupplier()
    {
        const int defaultValue = 10;
        var option = Option<int>.Some(defaultValue);
        var calls = 0;

        var direct = option.OrElseThrow(new InvalidOperationException());
        var supplied = option.OrElseThrow(() =>
        {
            calls++;
            return new InvalidOperationException();
        });

        Assert.Equal(defaultValue, direct);
        Assert.Equal(defaultValue, supplied);
        Assert.Equal(0, calls);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void IfPresentCallsTheActionOnlyWhenSome(bool hasValue)
    {
        const int defaultValue = 10;
        var option = hasValue ? Option<int>.Some(defaultValue) : Option<int>.None();
        var calls = 0;
        int? received = null;

        option.IfPresent(value =>
        {
            calls++;
            received = value;
        });

        Assert.Equal(hasValue ? 1 : 0, calls);
        Assert.Equal(hasValue ? defaultValue : null, received);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void IfEmptyCallsTheActionOnlyWhenNone(bool hasValue)
    {
        const int defaultValue = 10;
        var option = hasValue ? Option<int>.Some(defaultValue) : Option<int>.None();
        var calls = 0;

        option.IfEmpty(() => calls++);

        Assert.Equal(hasValue ? 0 : 1, calls);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void IfPresentOrElseCallsExactlyTheMatchingAction(bool hasValue)
    {
        const int defaultValue = 10;
        var option = hasValue ? Option<int>.Some(defaultValue) : Option<int>.None();
        var presentCalls = 0;
        var emptyCalls = 0;
        int? received = null;

        option.IfPresentOrElse(value =>
        {
            presentCalls++;
            received = value;
        }, () => emptyCalls++);

        Assert.Equal(hasValue ? 1 : 0, presentCalls);
        Assert.Equal(hasValue ? 0 : 1, emptyCalls);
        Assert.Equal(hasValue ? defaultValue : null, received);
    }

    [Fact]
    public void SelectCanChangeTheValueType()
    {
        var actual = Option<string>.Some("John").Select(name => name.Length);

        Assert.True(actual.IsSome);
        Assert.Equal(4, actual.Get);
    }

    [Fact]
    public void WhereSkipsThePredicateWhenNone()
    {
        var calls = 0;

        var actual = Option<int>.None().Where(value =>
        {
            calls++;
            return value > 0;
        });

        Assert.True(actual.IsNone);
        Assert.Equal(0, calls);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(-3)]
    public void SelectManyUnwrapsTheInnerValueAndInfersTheResultType(int defaultValue)
    {
        var wrapped = Option<Option<int>>.Some(Option<int>.Some(defaultValue));

        var actual = wrapped.SelectMany(e => e * defaultValue);

        Assert.Equal(Option<int>.Some(defaultValue * defaultValue), actual);
    }

    [Fact]
    public void SelectManyCallsTheSelectorOnceWithTheInnerValue()
    {
        const int defaultValue = 10;
        const int multiplier = 2;
        var wrapped = Option<Option<int>>.Some(Option<int>.Some(defaultValue));
        var calls = 0;

        var actual = wrapped.SelectMany(inner =>
        {
            calls++;
            Assert.Equal(defaultValue, inner);
            return inner * multiplier;
        });

        Assert.Equal(Option<int>.Some(defaultValue * multiplier), actual);
        Assert.Equal(1, calls);
    }

    [Fact]
    public void SelectManyCanChangeTheValueType()
    {
        var wrapped = Option<Option<string>>.Some(Option<string>.Some("John"));

        var actual = wrapped.SelectMany(name => name.Length);

        Assert.Equal(Option<int>.Some(4), actual);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void SelectManySkipsTheSelectorWhenEitherOptionIsNone(bool outerHasValue)
    {
        var wrapped = outerHasValue
            ? Option<Option<int>>.Some(Option<int>.None())
            : Option<Option<int>>.None();
        var calls = 0;

        var actual = wrapped.SelectMany(value =>
        {
            calls++;
            return value.ToString();
        });

        Assert.Equal(Option<string>.None(), actual);
        Assert.True(actual.IsNone);
        Assert.Equal(0, calls);
    }

    [Fact]
    public void SelectManyRemovesOnlyOneSourceLayer()
    {
        var value = Option<int>.Some(10);
        var wrapped = Option<Option<Option<int>>>.Some(Option<Option<int>>.Some(value));

        var actual = wrapped.SelectMany(inner => inner.Get * 2);

        Assert.Equal(Option<int>.Some(20), actual);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void FlattenPreservesInstanceOfPlainOption(bool hasValue)
    {
        var option = hasValue ? Option<int>.Some(42) : Option<int>.None();

        var actual = option.Flatten();

        Assert.Same(option, actual);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void FlattenReturnsNestedOption(bool innerHasValue)
    {
        var inner = innerHasValue ? Option<int>.Some(42) : Option<int>.None();
        var wrapped = Option<Option<int>>.Some(inner);

        var actual = wrapped.Flatten();

        Assert.Same(inner, actual);
    }

    [Fact]
    public void FlattenReturnsNoneWhenOuterOptionIsMissing()
    {
        var actual = Option<Option<int>>.None().Flatten();

        Assert.Equal(Option<int>.None(), actual);
    }

    [Fact]
    public void FlattenRemovesOnlyOneLayer()
    {
        var inner = Option<Option<int>>.Some(Option<int>.Some(42));
        var wrapped = Option<Option<Option<int>>>.Some(inner);

        var actual = wrapped.Flatten();

        Assert.Same(inner, actual);
        Assert.Equal(42, actual.Get.Get);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 42)]
    [InlineData(4, -3)]
    public void FlattenAllAlwaysReturnsDeepestType(int additionalLayers, int value)
    {
        var firstLayer = Option<int>.Some(value);
        Option<IOption> wrapped = firstLayer;
        for (var layer = 0; layer < additionalLayers; layer++)
            wrapped = Option<IOption>.Some(wrapped);

        var actual = wrapped.FlattenAll<int>();

        Assert.Equal(Option<int>.Some(value), actual);
    }

    [Fact]
    public void FlattenAllUnwrapsStaticallyNestedOptions()
    {
        var wrapped = Option<Option<Option<int>>>.Some(
            Option<Option<int>>.Some(Option<int>.Some(42)));

        var actual = wrapped.FlattenAll();

        Assert.Equal(Option<int>.Some(42), actual);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    public void FlattenAllInfersLeafTypeEightLayersDeep(int missingLayer)
    {
        const int value = 42;
        var one = missingLayer == 1 ? Option<int>.None() : Option<int>.Some(value);
        var two = Wrap(one, missingLayer == 2);
        var three = Wrap(two, missingLayer == 3);
        var four = Wrap(three, missingLayer == 4);
        var five = Wrap(four, missingLayer == 5);
        var six = Wrap(five, missingLayer == 6);
        var seven = Wrap(six, missingLayer == 7);
        var eight = Wrap(seven, missingLayer == 8);

        AssertResult(one.FlattenAll(), 1);
        AssertResult(two.FlattenAll(), 2);
        AssertResult(three.FlattenAll(), 3);
        AssertResult(four.FlattenAll(), 4);
        AssertResult(five.FlattenAll(), 5);
        AssertResult(six.FlattenAll(), 6);
        AssertResult(seven.FlattenAll(), 7);
        AssertResult(eight.FlattenAll(), 8);
        return;

        void AssertResult(Option<int> actual, int layers)
        {
            var expected = missingLayer > 0 && missingLayer <= layers
                ? Option<int>.None()
                : Option<int>.Some(value);
            Assert.Equal(expected, actual);
        }
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void FlattenAllRequiresAnExplicitLeafTypeBeyondEightLayers(bool innerIsMissing, bool outerIsMissing)
    {
        const int value = 42;
        var one = innerIsMissing ? Option<int>.None() : Option<int>.Some(value);
        var two = Wrap(one);
        var three = Wrap(two);
        var four = Wrap(three);
        var five = Wrap(four);
        var six = Wrap(five);
        var seven = Wrap(six);
        var eight = Wrap(seven);
        var nine = Wrap(eight, outerIsMissing);

        var exception = Assert.Throws<InvalidOperationException>(nine.FlattenAll);

        Assert.Equal(
            "FlattenAll() supports automatic type inference for up to 8 option layers. " +
            "Specify the innermost type explicitly using FlattenAll<TResult>().",
            exception.Message);
        var expected = innerIsMissing || outerIsMissing ? Option<int>.None() : Option<int>.Some(value);
        Assert.Equal(expected, nine.FlattenAll<int>());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(4)]
    public void FlattenAllReturnsNoneWhenAnyLayerIsNone(int additionalLayers)
    {
        IOption wrapped = Option<int>.None();
        for (var layer = 0; layer < additionalLayers; layer++)
            wrapped = Option<IOption>.Some(wrapped);

        var actual = wrapped.FlattenAll<int>();

        Assert.Equal(Option<int>.None(), actual);
    }

    [Fact]
    public void FlattenAllPreservesTheLeafReference()
    {
        var value = new object();
        var wrapped = Option<Option<object>>.Some(Option<object>.Some(value));

        var actual = wrapped.FlattenAll();

        Assert.Same(value, actual.Get);
    }

    [Fact]
    public void FlattenAllRejectsAnIncompatibleLeafType()
    {
        var wrapped = Option<Option<int>>.Some(Option<int>.Some(42));

        var exception = Assert.Throws<InvalidCastException>(wrapped.FlattenAll<string>);

        Assert.Equal(
            "Cannot flatten an option containing a value of type 'System.Int32' to Option<System.String>.",
            exception.Message);
    }

    [Theory]
    [InlineData("value", "System.String")]
    [InlineData(true, "System.Boolean")]
    [InlineData(42L, "System.Int64")]
    public void FlattenAllReportsTheActualRuntimeLeafType(object value, string typeName)
    {
        var wrapped = Option<Option<object>>.Some(Option<object>.Some(value));

        var exception = Assert.Throws<InvalidCastException>(wrapped.FlattenAll<int>);

        Assert.Equal(
            $"Cannot flatten an option containing a value of type '{typeName}' to Option<System.Int32>.",
            exception.Message);
    }

    [Fact]
    public void FlattenAllAcceptsAnAssignableLeafType()
    {
        const string value = "value";
        var wrapped = Option<Option<string>>.Some(Option<string>.Some(value));

        var actual = wrapped.FlattenAll<IComparable>();

        Assert.Same(value, actual.Get);
    }

    [Fact]
    public void FlattenAllReturnsNoneWithoutValidatingAMissingLeaf()
    {
        var wrapped = Option<Option<int>>.Some(Option<int>.None());

        var actual = wrapped.FlattenAll<string>();

        Assert.Equal(Option<string>.None(), actual);
    }

    private static Option<Option<T>> Wrap<T>(Option<T> option, bool isEmpty = false) where T : notnull =>
        isEmpty ? Option<Option<T>>.None() : Option<Option<T>>.Some(option);
}