using System;
using System.Collections.Generic;

namespace SimpleWeather.Utils
{
    public static class CustomExtensions
    {
        /* Kotlin-like extensions */
        public static TOutput Let<TInput, TOutput>(this TInput obj, Func<TInput, TOutput> func)
        {
            return func.Invoke(obj);
        }

        public static void Let<TInput>(this TInput obj, Action<TInput> action)
        {
            action.Invoke(obj);
        }

        public static TInput Apply<TInput>(this TInput obj, Action<TInput> action)
        {
            action.Invoke(obj);
            return obj;
        }

        public static Result<TInput> RunCatching<TInput>(this object _, Func<TInput> action)
        {
            try
            {
                return Result<TInput>.success(action());
            }
            catch (Exception e)
            {
                return Result<TInput>.failure(e);
            }
        }

        public static Result<TOutput> RunCatching<TInput, TOutput>(this TInput obj, Func<TInput, TOutput> func)
        {
            try
            {
                return Result<TOutput>.success(func(obj));
            }
            catch (Exception e)
            {
                return Result<TOutput>.failure(e);
            }
        }
    }

    public struct Result<T>
    {
        internal object Value { get; set; }

        internal Result(object value)
        {
            this.Value = value;
        }

        internal Result(Failure value)
        {
            this.Value = value;
        }

        public bool IsSuccess => Value is not Failure;
        public bool IsFailure => Value is Failure;

        public T? GetOrNull()
        {
            if (IsFailure)
            {
                return default;
            }
            else
            {
                return (T)Value;
            }
        }

        public Exception ExceptionOrNull()
        {
            if (Value is Failure failure)
            {
                return failure.Exception;
            }
            else
            {
                return null;
            }
        }

        public override string ToString()
        {
            if (Value is Failure failure)
            {
                return failure.ToString();
            }
            else
            {
                return $"Success(${Value})";
            }
        }

        public static Result<T> success(T value) => new Result<T>(value);
        public static Result<T> failure(Exception exception)
        {
            return new Result<T>(new Failure(exception));
        }

        internal class Failure : object
        {
            public Exception Exception { get; internal set; }

            internal Failure(Exception exception)
            {
                this.Exception = exception;
            }

            public override bool Equals(object obj)
            {
                return obj is Failure failure &&
                       EqualityComparer<Exception>.Default.Equals(Exception, failure.Exception);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Exception);
            }

            public override string ToString()
            {
                return $"Failure(${Exception})";
            }
        }
    }

    public static class ResultExtensions
    {
        public static void ThrowOnFailure<T>(this Result<T> result)
        {
            if (result.Value is Result<T>.Failure failure)
            {
                throw failure.Exception;
            }
        }

        public static T GetOrThrow<T>(this Result<T> result)
        {
            result.ThrowOnFailure();
            return (T)result.Value;
        }

        public static R GetOrElse<T, R>(this Result<T> result, Func<Exception, R> onFailure) where T : R
        {
            var exception = result.ExceptionOrNull();
            if (exception == null)
            {
                return (T)result.Value;
            }
            else
            {
                return onFailure(exception);
            }
        }

        public static R GetOrDefault<T, R>(this Result<T> result, R defaultValue) where T : R
        {
            if (result.IsFailure)
            {
                return defaultValue;
            }
            return (T)result.Value;
        }
    }
}
