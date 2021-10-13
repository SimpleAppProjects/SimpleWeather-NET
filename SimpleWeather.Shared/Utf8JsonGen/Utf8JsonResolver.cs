using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Utf8Json;

namespace SimpleWeather.Utf8JsonGen
{
    public class Utf8JsonResolver : IJsonFormatterResolver
    {
        // configure your resolver and formatters.
        private static readonly IJsonFormatterResolver[] Resolvers = new IJsonFormatterResolver[]
        {
                SimpleWeather.Utf8JsonGen.Resolvers.GeneratedResolver.Instance,
                SimpleWeather.RemoteConfig.Utf8JsonGen.Resolvers.GeneratedResolver.Instance,
                // set StandardResolver or your use resolver chain
                Utf8Json.Resolvers.AttributeFormatterResolver.Instance,
                Utf8Json.Resolvers.BuiltinResolver.Instance,
                SimpleWeather.Utf8JsonGen.Resolvers.EnumResolver.ByValue,
                SimpleWeather.Utf8JsonGen.EnumerableCollectionResolver.Instance
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
                SimpleWeather.Utf8JsonGen.Resolvers.GeneratedResolver.Instance,
                SimpleWeather.RemoteConfig.Utf8JsonGen.Resolvers.GeneratedResolver.Instance,
                // set StandardResolver or your use resolver chain
                Utf8Json.Resolvers.BuiltinResolver.Instance,
                SimpleWeather.Utf8JsonGen.Resolvers.EnumResolver.ByValue,
                SimpleWeather.Utf8JsonGen.EnumerableCollectionResolver.Instance
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
