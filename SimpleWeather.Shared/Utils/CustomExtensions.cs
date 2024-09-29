using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

#nullable enable
        public static TInput? TakeIf<TInput>(this TInput obj, Func<TInput, bool> predicate) where TInput : class
        {
            return predicate.Invoke(obj) ? obj : null;
        }
#nullable restore

        public static int? TakeIf(this int obj, Func<int, bool> predicate)
        {
            return predicate.Invoke(obj) ? obj : null;
        }

        public static long? TakeIf(this long obj, Func<long, bool> predicate)
        {
            return predicate.Invoke(obj) ? obj : null;
        }

        public static float? TakeIf(this float obj, Func<float, bool> predicate)
        {
            return predicate.Invoke(obj) ? obj : null;
        }

        public static double? TakeIf(this double obj, Func<double, bool> predicate)
        {
            return predicate.Invoke(obj) ? obj : null;
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

        public static async Task<Result<TInput>> RunCatching<TInput>(this object _, Func<Task<TInput>> action)
        {
            try
            {
                var result = await action();
                return Result<TInput>.success(result);
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

        public static Result<object> RunCatching(this object _, Action action)
        {
            try
            {
                action();
                return Result<object>.success(_);
            }
            catch (Exception e)
            {
                return Result<object>.failure(e);
            }
        }

        public static async Task<Result<object>> RunCatching(this object _, Func<Task> action)
        {
            try
            {
                await action();
                return Result<object>.success(_);
            }
            catch (Exception e)
            {
                return Result<object>.failure(e);
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
