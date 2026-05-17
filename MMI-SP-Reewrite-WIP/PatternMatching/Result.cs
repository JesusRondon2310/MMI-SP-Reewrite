using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace MMI_SP.PatternMatching
{
    public abstract class Result<T>
    {
        public bool IsOk => this is Ok<T>;
        public bool IsErr => this is Err<T>;

        public abstract TResult Match<TResult>(Func<T, TResult> onOk, Func<string, TResult> onErr);

        public Result<TResult> AndThen<TResult>(Func<T, Result<TResult>> func)
        {
            if (this is Ok<T> ok) return func(ok.Value);
            if (this is Err<T> err) return new Err<TResult>(err.Message, err.Source);
            return new Err<TResult>("Estado desconocido.");
        }
    }

    public sealed class Ok<T> : Result<T>
    {
        public T Value { get; }
        public Ok(T value) => Value = value;

        public override TResult Match<TResult>(Func<T, TResult> onOk, Func<string, TResult> onErr) => onOk(Value);
    }

    public sealed class Err<T> : Result<T>
    {
        public string Message { get; }
        // "Archivo.Método:línea" — capturado en tiempo de compilación sin coste de reflexión
        public string Source { get; }

        public Err(string message,
            [CallerMemberName] string member = "",
            [CallerFilePath]   string file   = "",
            [CallerLineNumber] int    line   = 0)
        {
            Message = message;
            Source  = $"{Path.GetFileNameWithoutExtension(file)}.{member}:{line}";
        }

        // Constructor interno para AndThen: propaga el Source original sin sobreescribirlo.
        internal Err(string message, string source)
        {
            Message = message;
            Source  = source;
        }

        public string FullMessage => $"[{Source}] {Message}";

        public override TResult Match<TResult>(Func<T, TResult> onOk, Func<string, TResult> onErr) => onErr(Message);
    }
}
