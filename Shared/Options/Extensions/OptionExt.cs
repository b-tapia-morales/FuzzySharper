using Shared.Options.Factory;
using Shared.Options.Implementations;

namespace Shared.Options.Extensions;

public static class OptionExt
{
    extension<T>(Option<T> option)
    {
        public void IfPresent(Action<T> action)
        {
            if (option.IsSome)
                action(option.Get);
        }

        public void IfEmpty(Action action)
        {
            if (option.IsNone)
                action();
        }

        public void IfPresentOrElse(Action<T> ifPresent, Action elseAction)
        {
            if (option.IsSome)
            {
                ifPresent(option.Get);
                return;
            }

            elseAction();
        }

        public T OrElse(T other) =>
            option.IsSome ? option.Get : other;

        public T OrElse(Func<T> supplier) =>
            option.IsSome ? option.Get : supplier();

        public T OrElseThrow<TException>(TException exception) where TException : Exception =>
            option.IsSome ? option.Get : throw exception;

        public T OrElseThrow<TException>(Func<TException> exceptionSupplier) where TException : Exception =>
            option.IsSome ? option.Get : throw exceptionSupplier();

        public Option<T> Where(Predicate<T> predicate) =>
            option.IsSome && predicate(option.Get) ? option : OptionFactory.None<T>();

        public Option<TResult> Select<TResult>(Func<T, TResult> selector) =>
            option.IsSome ? new Option<TResult>(selector(option.Get)) : OptionFactory.None<TResult>();

        public Option<TResult> SelectMany<TResult>(Func<T, Option<TResult>> mapper) =>
            option.IsSome ? mapper(option.Get) : OptionFactory.None<TResult>();
    }
}