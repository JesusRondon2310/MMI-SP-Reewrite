using System;

namespace MMI_SP.PatternMatching
{
    public abstract class Option<T>
    {
        public bool IsSome => this is Some<T>;
        public bool IsNone => this is None<T>;

        public abstract TResult Match<TResult>(Func<T, TResult> onSome, Func<TResult> onNone);

        public Option<TResult> AndThen<TResult>(Func<T, Option<TResult>> func)
        {
            if (this is Some<T> some) return func(some.Value);
            return new None<TResult>();
        }
    }

    public sealed class Some<T> : Option<T>
    {
        public T Value { get; }
        public Some(T value) => Value = value;

        public override TResult Match<TResult>(Func<T, TResult> onSome, Func<TResult> onNone) => onSome(Value);
    }

    public sealed class None<T> : Option<T>
    {
        public override TResult Match<TResult>(Func<T, TResult> onSome, Func<TResult> onNone) => onNone();
    }

    public static class Option
    {
        public static Option<T> FromNullable<T>(T value) where T : class
            => value != null ? new Some<T>(value) : (Option<T>)new None<T>();

        public static None<T> NewNone<T>() => new None<T>();
    }
}
