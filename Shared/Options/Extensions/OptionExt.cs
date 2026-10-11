using Shared.Options.Abstractions;
using Shared.Options.Factory;
using Shared.Options.Implementations;

namespace Shared.Options.Extensions;

/// <summary>
/// Provides conditional actions, fallback values, filtering, selection, and flattening for options.
/// </summary>
public static class OptionExt
{
    extension<T>(Option<T> option) where T : notnull
    {
        /// <summary>
        /// If a value is present, performs the action with the value; otherwise, does nothing.
        /// </summary>
        /// <param name="action">The action to perform with the value.</param>
        /// <exception cref="ArgumentNullException">The action is null.</exception>
        public void IfPresent(Action<T> action)
        {
            ArgumentNullException.ThrowIfNull(action);
            if (option.IsSome)
                action(option.Get);
        }

        /// <summary>
        /// If the option is empty, performs the action; otherwise, does nothing.
        /// </summary>
        /// <param name="action">The action to perform when empty.</param>
        /// <exception cref="ArgumentNullException">The action is null.</exception>
        public void IfEmpty(Action action)
        {
            ArgumentNullException.ThrowIfNull(action);
            if (option.IsNone)
                action();
        }

        /// <summary>
        /// Performs <paramref name="ifPresent"/> with the value when present,
        /// or <paramref name="elseAction"/> when the option is empty.
        /// </summary>
        /// <param name="ifPresent">The action to perform with the value.</param>
        /// <param name="elseAction">The action to perform when empty.</param>
        /// <exception cref="ArgumentNullException">Any action is null.</exception>
        public void IfPresentOrElse(Action<T> ifPresent, Action elseAction)
        {
            ArgumentNullException.ThrowIfNull(ifPresent);
            ArgumentNullException.ThrowIfNull(elseAction);
            if (option.IsSome)
            {
                ifPresent(option.Get);
                return;
            }

            elseAction();
        }

        /// <summary>
        /// Returns the value when present, or <paramref name="other"/> when the option is empty.
        /// </summary>
        /// <param name="other">The fallback value.</param>
        /// <returns>The value, or <paramref name="other"/>.</returns>
        public T OrElse(T other) =>
            option.IsSome ? option.Get : other;

        /// <summary>
        /// Returns the value when present or calls the supplier when the option is empty.
        /// </summary>
        /// <param name="supplier">The function that supplies a fallback value.</param>
        /// <returns>The value, or the supplier's result.</returns>
        /// <exception cref="ArgumentNullException">The supplier is null.</exception>
        public T OrElse(Func<T> supplier)
        {
            ArgumentNullException.ThrowIfNull(supplier);
            return option.IsSome ? option.Get : supplier();
        }

        /// <summary>
        /// Returns the value when present or throws the supplied exception when the option is empty.
        /// </summary>
        /// <typeparam name="TException">The type of exception to throw.</typeparam>
        /// <param name="exception">The exception to throw.</param>
        /// <returns>The value when present.</returns>
        /// <exception cref="Exception">The option is empty; the supplied <typeparamref name="TException"/> is thrown.</exception>
        /// <exception cref="ArgumentNullException">The exception is null.</exception>
        public T OrElseThrow<TException>(TException exception) where TException : Exception
        {
            ArgumentNullException.ThrowIfNull(exception);
            return option.IsSome ? option.Get : throw exception;
        }

        /// <summary>
        /// Returns the value when present or calls the exception supplier and throws its result
        /// when the option is empty.
        /// </summary>
        /// <typeparam name="TException">The type of exception to throw.</typeparam>
        /// <param name="exceptionSupplier">The function that supplies an exception.</param>
        /// <returns>The value when present.</returns>
        /// <exception cref="Exception">The option is empty; the supplied <typeparamref name="TException"/> is thrown.</exception>
        /// <exception cref="ArgumentNullException">The supplier is null.</exception>
        /// <exception cref="InvalidOperationException">The supplier's result is null.</exception>
        public T OrElseThrow<TException>(Func<TException> exceptionSupplier) where TException : Exception
        {
            ArgumentNullException.ThrowIfNull(exceptionSupplier);
            if (option.IsSome)
                return option.Get;

            var exception = exceptionSupplier();
            if (exception is null)
                throw new InvalidOperationException("The exception supplier returned null.");

            throw exception;
        }

        /// <summary>
        /// Returns this option if a value is present and satisfies the predicate;
        /// otherwise, returns an empty option.
        /// </summary>
        /// <param name="predicate">The condition to test against the value.</param>
        /// <returns>This option if the predicate is satisfied, or an empty option.</returns>
        /// <exception cref="ArgumentNullException">The predicate is null.</exception>
        public Option<T> Where(Predicate<T> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);
            return option.IsSome && predicate(option.Get) ? option : Option<T>.None();
        }

        /// <summary>Returns this option unchanged.</summary>
        /// <returns>This option.</returns>
        public Option<T> Flatten() => option;

        /// <summary>
        /// Removes all option layers, returning an option containing the innermost value,
        /// or an empty option if any layer is empty.
        /// </summary>
        /// <returns>An option containing the innermost value, or an empty option.</returns>
        /// <remarks>
        /// Type inference supports up to eight option layers. For deeper nesting, specify the
        /// innermost type explicitly, for example, <c>option.FlattenAll&lt;int&gt;()</c>.
        /// </remarks>
        /// <exception cref="InvalidOperationException">The inferred value type is an option type.</exception>
        /// <exception cref="InvalidCastException">The innermost value is incompatible with the inferred type.</exception>
        public Option<T> FlattenAll() => FlattenAllInferred<T>(option);

        /// <summary>
        /// Applies the selector to a present value and returns an option containing the result,
        /// or an empty option if no value is present.
        /// </summary>
        /// <typeparam name="TResult">The type of the selected value.</typeparam>
        /// <param name="selector">The function to apply to the value.</param>
        /// <returns>An option containing the selected value, or an empty option.</returns>
        /// <remarks>If the selected value is an option, it remains nested.</remarks>
        /// <exception cref="ArgumentNullException">The selector is null.</exception>
        public Option<TResult> Select<TResult>(Func<T, TResult> selector) where TResult : notnull
        {
            ArgumentNullException.ThrowIfNull(selector);
            return option.IsSome ? new Option<TResult>(selector(option.Get)) : Option<TResult>.None();
        }
    }

    extension<T>(Option<Option<T>> option) where T : notnull
    {
        /// <summary>
        /// Removes one option layer, returning the inner option when the outer option is present,
        /// or an empty option otherwise.
        /// </summary>
        /// <returns>The inner option, or an empty option.</returns>
        public Option<T> Flatten() =>
            option.IsSome ? option.Get : Option<T>.None();

        /// <summary>
        /// Removes all option layers, returning an option containing the innermost value,
        /// or an empty option if any layer is empty.
        /// </summary>
        /// <returns>An option containing the innermost value, or an empty option.</returns>
        /// <remarks>
        /// Type inference supports up to eight option layers. For deeper nesting, specify the
        /// innermost type explicitly, for example, <c>option.FlattenAll&lt;int&gt;()</c>.
        /// </remarks>
        /// <exception cref="InvalidOperationException">The inferred value type is an option type.</exception>
        /// <exception cref="InvalidCastException">The innermost value is incompatible with the inferred type.</exception>
        public Option<T> FlattenAll() => FlattenAllInferred<T>(option);

        /// <summary>
        /// Removes one option layer and applies the selector to the inner value when both options
        /// are present, returning an option containing the result. Returns an empty option otherwise.
        /// </summary>
        /// <typeparam name="TResult">The type of the selected value.</typeparam>
        /// <param name="selector">The function to apply to the inner value.</param>
        /// <returns>An option containing the selected value, or an empty option.</returns>
        /// <remarks>Further nesting in the source or selected result is preserved.</remarks>
        /// <exception cref="ArgumentNullException">The selector is null.</exception>
        public Option<TResult> SelectMany<TResult>(Func<T, TResult> selector) where TResult : notnull
        {
            ArgumentNullException.ThrowIfNull(selector);
            return option.Flatten().Select(selector);
        }
    }

    extension<T>(Option<Option<Option<T>>> option) where T : notnull
    {
        /// <summary>
        /// Removes all option layers, returning an option containing the innermost value,
        /// or an empty option if any layer is empty.
        /// </summary>
        /// <returns>An option containing the innermost value, or an empty option.</returns>
        /// <remarks>
        /// Type inference supports up to eight option layers. For deeper nesting, specify the
        /// innermost type explicitly, for example, <c>option.FlattenAll&lt;int&gt;()</c>.
        /// </remarks>
        /// <exception cref="InvalidOperationException">The inferred value type is an option type.</exception>
        /// <exception cref="InvalidCastException">The innermost value is incompatible with the inferred type.</exception>
        public Option<T> FlattenAll() => FlattenAllInferred<T>(option);
    }

    extension<T>(Option<Option<Option<Option<T>>>> option) where T : notnull
    {
        /// <summary>
        /// Removes all option layers, returning an option containing the innermost value,
        /// or an empty option if any layer is empty.
        /// </summary>
        /// <returns>An option containing the innermost value, or an empty option.</returns>
        /// <remarks>
        /// Type inference supports up to eight option layers. For deeper nesting, specify the
        /// innermost type explicitly, for example, <c>option.FlattenAll&lt;int&gt;()</c>.
        /// </remarks>
        /// <exception cref="InvalidOperationException">The inferred value type is an option type.</exception>
        /// <exception cref="InvalidCastException">The innermost value is incompatible with the inferred type.</exception>
        public Option<T> FlattenAll() => FlattenAllInferred<T>(option);
    }

    extension<T>(Option<Option<Option<Option<Option<T>>>>> option) where T : notnull
    {
        /// <summary>
        /// Removes all option layers, returning an option containing the innermost value,
        /// or an empty option if any layer is empty.
        /// </summary>
        /// <returns>An option containing the innermost value, or an empty option.</returns>
        /// <remarks>
        /// Type inference supports up to eight option layers. For deeper nesting, specify the
        /// innermost type explicitly, for example, <c>option.FlattenAll&lt;int&gt;()</c>.
        /// </remarks>
        /// <exception cref="InvalidOperationException">The inferred value type is an option type.</exception>
        /// <exception cref="InvalidCastException">The innermost value is incompatible with the inferred type.</exception>
        public Option<T> FlattenAll() => FlattenAllInferred<T>(option);
    }

    extension<T>(Option<Option<Option<Option<Option<Option<T>>>>>> option) where T : notnull
    {
        /// <summary>
        /// Removes all option layers, returning an option containing the innermost value,
        /// or an empty option if any layer is empty.
        /// </summary>
        /// <returns>An option containing the innermost value, or an empty option.</returns>
        /// <remarks>
        /// Type inference supports up to eight option layers. For deeper nesting, specify the
        /// innermost type explicitly, for example, <c>option.FlattenAll&lt;int&gt;()</c>.
        /// </remarks>
        /// <exception cref="InvalidOperationException">The inferred value type is an option type.</exception>
        public Option<T> FlattenAll() => FlattenAllInferred<T>(option);
    }

    extension<T>(Option<Option<Option<Option<Option<Option<Option<T>>>>>>> option) where T : notnull
    {
        /// <summary>
        /// Removes all option layers, returning an option containing the innermost value,
        /// or an empty option if any layer is empty.
        /// </summary>
        /// <returns>An option containing the innermost value, or an empty option.</returns>
        /// <remarks>
        /// Type inference supports up to eight option layers. For deeper nesting, specify the
        /// innermost type explicitly, for example, <c>option.FlattenAll&lt;int&gt;()</c>.
        /// </remarks>
        /// <exception cref="InvalidOperationException">The inferred value type is an option type.</exception>
        /// <exception cref="InvalidCastException">The innermost value is incompatible with the inferred type.</exception>
        public Option<T> FlattenAll() => FlattenAllInferred<T>(option);
    }

    extension<T>(Option<Option<Option<Option<Option<Option<Option<Option<T>>>>>>>> option) where T : notnull
    {
        /// <summary>
        /// Removes all option layers, returning an option containing the innermost value,
        /// or an empty option if any layer is empty.
        /// </summary>
        /// <returns>An option containing the innermost value, or an empty option.</returns>
        /// <remarks>
        /// Type inference supports up to eight option layers. For deeper nesting, specify the
        /// innermost type explicitly, for example, <c>option.FlattenAll&lt;int&gt;()</c>.
        /// </remarks>
        /// <exception cref="InvalidOperationException">The inferred value type is an option type.</exception>
        /// <exception cref="InvalidCastException">The innermost value is incompatible with the inferred type.</exception>
        public Option<T> FlattenAll() => FlattenAllInferred<T>(option);
    }

    extension(IOption option)
    {
        /// <summary>
        /// Removes all option layers, returning an option containing the innermost value as
        /// <typeparamref name="TResult"/>, or an empty option if any layer is empty.
        /// </summary>
        /// <typeparam name="TResult">The type to which the innermost value must be assignable.</typeparam>
        /// <returns>An option containing the innermost value, or an empty option.</returns>
        /// <remarks>
        /// Supports any nesting depth. If a layer is empty, no value type is checked.
        /// </remarks>
        /// <exception cref="InvalidCastException">The innermost value is not assignable to <typeparamref name="TResult"/>.</exception>
        public Option<TResult> FlattenAll<TResult>() where TResult : notnull
        {
            object current = option;

            while (current is IOption nested)
            {
                if (nested.IsNone)
                    return Option<TResult>.None();

                current = nested.Get;
            }

            if (current is TResult value)
                return Option<TResult>.Some(value);

            throw new InvalidCastException(
                $"Cannot flatten an option containing a value of type '{current.GetType()}' to Option<{typeof(TResult)}>.");
        }
    }

    private static Option<T> FlattenAllInferred<T>(IOption option) where T : notnull
    {
        if (typeof(T).IsAssignableTo(typeof(IOption)))
            throw new InvalidOperationException(
                "FlattenAll() supports automatic type inference for up to 8 option layers. Specify the innermost type explicitly using FlattenAll<TResult>().");

        return option.FlattenAll<T>();
    }
}