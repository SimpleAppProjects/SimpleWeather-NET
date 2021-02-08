#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168

namespace SimpleWeather.RemoteConfig.Utf8JsonGen.Resolvers
{
    using System;
    using Utf8Json;

    public class GeneratedResolver : global::Utf8Json.IJsonFormatterResolver
    {
        public static readonly global::Utf8Json.IJsonFormatterResolver Instance = new GeneratedResolver();

        GeneratedResolver()
        {

        }

        public global::Utf8Json.IJsonFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.formatter;
        }

        static class FormatterCache<T>
        {
            public static readonly global::Utf8Json.IJsonFormatter<T> formatter;

            static FormatterCache()
            {
                var f = GeneratedResolverGetFormatterHelper.GetFormatter(typeof(T));
                if (f != null)
                {
                    formatter = (global::Utf8Json.IJsonFormatter<T>)f;
                }
            }
        }
    }

    internal static class GeneratedResolverGetFormatterHelper
    {
        static readonly global::System.Collections.Generic.Dictionary<Type, int> lookup;

        static GeneratedResolverGetFormatterHelper()
        {
            lookup = new global::System.Collections.Generic.Dictionary<Type, int>(1)
            {
                {typeof(global::SimpleWeather.RemoteConfig.WeatherProviderConfig), 0 },
            };
        }

        internal static object GetFormatter(Type t)
        {
            int key;
            if (!lookup.TryGetValue(t, out key)) return null;

            switch (key)
            {
                case 0: return new SimpleWeather.RemoteConfig.Utf8JsonGen.Formatters.SimpleWeather.RemoteConfig.WeatherProviderConfigFormatter();
                default: return null;
            }
        }
    }
}

#pragma warning disable 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612

#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 219
#pragma warning disable 168

namespace SimpleWeather.RemoteConfig.Utf8JsonGen.Formatters.SimpleWeather.RemoteConfig
{
    using System;
    using Utf8Json;


    public sealed class WeatherProviderConfigFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.RemoteConfig.WeatherProviderConfig>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public WeatherProviderConfigFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("enabled"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("locSource"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("newWeatherSource"), 2},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("enabled"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("locSource"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("newWeatherSource"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.RemoteConfig.WeatherProviderConfig value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteBoolean(value.enabled);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.locSource);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.newWeatherSource);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.RemoteConfig.WeatherProviderConfig Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __enabled__ = default(bool);
            var __enabled__b__ = false;
            var __locSource__ = default(string);
            var __locSource__b__ = false;
            var __newWeatherSource__ = default(string);
            var __newWeatherSource__b__ = false;

            var ____count = 0;
            reader.ReadIsBeginObjectWithVerify();
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref ____count))
            {
                var stringKey = reader.ReadPropertyNameSegmentRaw();
                int key;
                if (!____keyMapping.TryGetValueSafe(stringKey, out key))
                {
                    reader.ReadNextBlock();
                    goto NEXT_LOOP;
                }

                switch (key)
                {
                    case 0:
                        __enabled__ = reader.ReadBoolean();
                        __enabled__b__ = true;
                        break;
                    case 1:
                        __locSource__ = reader.ReadString();
                        __locSource__b__ = true;
                        break;
                    case 2:
                        __newWeatherSource__ = reader.ReadString();
                        __newWeatherSource__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.RemoteConfig.WeatherProviderConfig();
            if(__enabled__b__) ____result.enabled = __enabled__;
            if(__locSource__b__) ____result.locSource = __locSource__;
            if(__newWeatherSource__b__) ____result.newWeatherSource = __newWeatherSource__;

            return ____result;
        }
    }

}

#pragma warning disable 168
#pragma warning restore 219
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612
