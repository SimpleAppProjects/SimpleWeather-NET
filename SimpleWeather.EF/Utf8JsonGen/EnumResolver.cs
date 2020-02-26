using System;
using Utf8Json.Internal;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using SimpleWeather.EF.Utf8JsonGen.Resolvers.Internal;

// EnumResolver.cs (f724c83986d7c919a336c63e55f5a5886cca3575)
namespace SimpleWeather.EF.Utf8JsonGen.Resolvers
{
    public static class EnumResolver
    {
        /// <summary>Serialize as Value.</summary>
        public static Utf8Json.IJsonFormatterResolver ByValue { get { return EnumByValueResolver.Instance; } }
    }
}

namespace SimpleWeather.EF.Utf8JsonGen.Resolvers.Internal
{
    internal sealed class EnumByValueResolver : Utf8Json.IJsonFormatterResolver
    {
        public static Utf8Json.IJsonFormatterResolver Instance = new EnumByValueResolver();

        EnumByValueResolver()
        {
        }

        public Utf8Json.IJsonFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.formatter;
        }

        static object GetFormatterDynamic(Utf8Json.IJsonFormatterResolver resolver, Type type)
        {
            var methodInfo = typeof(Utf8Json.IJsonFormatterResolver).GetRuntimeMethod("GetFormatter", Type.EmptyTypes);

            var formatter = methodInfo.MakeGenericMethod(type).Invoke(resolver, null);
            return formatter;
        }

        static class FormatterCache<T>
        {
            public static readonly Utf8Json.IJsonFormatter<T> formatter;

            static FormatterCache()
            {
                var ti = typeof(T).GetTypeInfo();

                if (ti.IsGenericType && ti.GetGenericTypeDefinition() == typeof(System.Nullable<>))
                {
                    // build underlying type and use wrapped formatter.
                    ti = ti.GenericTypeArguments[0].GetTypeInfo();
                    if (!ti.IsEnum)
                    {
                        return;
                    }

                    var innerFormatter = GetFormatterDynamic(Instance, ti.AsType());
                    if (innerFormatter == null)
                    {
                        return;
                    }
                    formatter = (Utf8Json.IJsonFormatter<T>)Activator.CreateInstance(typeof(Utf8Json.Formatters.StaticNullableFormatter<>).MakeGenericType(ti.AsType()), new object[] { innerFormatter });
                    return;
                }
                else if (typeof(T).IsEnum)
                {
                    formatter = (Utf8Json.IJsonFormatter<T>)(object)new SimpleWeather.EF.Utf8JsonGen.Formatters.EnumFormatter<T>();
                }
            }
        }
    }
}