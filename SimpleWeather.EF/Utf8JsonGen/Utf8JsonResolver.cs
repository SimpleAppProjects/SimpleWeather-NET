using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Utf8Json;

namespace SimpleWeather.EF.Utf8JsonGen
{
    public class Utf8JsonResolver : IJsonFormatterResolver
    {
        // configure your resolver and formatters.
        private static readonly IJsonFormatterResolver[] Resolvers = new IJsonFormatterResolver[]
        {
                SimpleWeather.EF.Utf8JsonGen.Resolvers.GeneratedResolver.Instance,
#if !EF_PROJECT
                SimpleWeather.Utf8JsonGen.Resolvers.GeneratedResolver.Instance,
#endif
#if WINDOWS_UWP && !UNIT_TEST
                SimpleWeather.UWP.Utf8JsonGen.Resolvers.GeneratedResolver.Instance,
#endif
                // set StandardResolver or your use resolver chain
                Utf8Json.Resolvers.AttributeFormatterResolver.Instance,
                Utf8Json.Resolvers.BuiltinResolver.Instance,
                Utf8Json.Resolvers.EnumResolver.UnderlyingValue,
                SimpleWeather.EF.Utf8JsonGen.EnumerableCollectionResolver.Instance
        };

        public IJsonFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.Formatter;
        }

        static class FormatterCache<T>
        {
            public static readonly IJsonFormatter<T> Formatter;

            static FormatterCache()
            {
                foreach (var item in Resolvers)
                {
                    var f = item.GetFormatter<T>();
                    if (f != null)
                    {
                        Formatter = f;
                        return;
                    }
                }
            }
        }
    }

    public class AttrFirstUtf8JsonResolver : IJsonFormatterResolver
    {
        // configure your resolver and formatters.
        private static readonly IJsonFormatterResolver[] Resolvers = new IJsonFormatterResolver[]
        {
                Utf8Json.Resolvers.AttributeFormatterResolver.Instance,
                SimpleWeather.EF.Utf8JsonGen.Resolvers.GeneratedResolver.Instance,
#if !EF_PROJECT
                SimpleWeather.Utf8JsonGen.Resolvers.GeneratedResolver.Instance,
#endif
#if WINDOWS_UWP && !UNIT_TEST
                SimpleWeather.UWP.Utf8JsonGen.Resolvers.GeneratedResolver.Instance,
#endif
                // set StandardResolver or your use resolver chain
                Utf8Json.Resolvers.BuiltinResolver.Instance,
                Utf8Json.Resolvers.EnumResolver.UnderlyingValue,
                SimpleWeather.EF.Utf8JsonGen.EnumerableCollectionResolver.Instance
        };

        public IJsonFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.Formatter;
        }

        static class FormatterCache<T>
        {
            public static readonly IJsonFormatter<T> Formatter;

            static FormatterCache()
            {
                foreach (var item in Resolvers)
                {
                    var f = item.GetFormatter<T>();
                    if (f != null)
                    {
                        Formatter = f;
                        return;
                    }
                }
            }
        }
    }

    public class EnumerableCollectionResolver : IJsonFormatterResolver
    {
        public static readonly IJsonFormatterResolver Instance = new EnumerableCollectionResolver();

        EnumerableCollectionResolver()
        {
        }

        public IJsonFormatter<T> GetFormatter<T>()
        {
            if (typeof(T).GetGenericArguments()?.FirstOrDefault() is Type elementType &&
                typeof(CustomJsonObject).IsAssignableFrom(elementType))
                return new EnumerableFormatter<T>();
            return null;
        }
    }
}
