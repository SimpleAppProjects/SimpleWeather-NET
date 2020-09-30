#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168

namespace SimpleWeather.UWP.Utf8JsonGen.Resolvers
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
            lookup = new global::System.Collections.Generic.Dictionary<Type, int>(2)
            {
                {typeof(global::SimpleWeather.UWP.WNS.FirebaseUWPUser), 0 },
                {typeof(global::SimpleWeather.UWP.WNS.WNSChannel), 1 },
            };
        }

        internal static object GetFormatter(Type t)
        {
            int key;
            if (!lookup.TryGetValue(t, out key)) return null;

            switch (key)
            {
                case 0: return new SimpleWeather.UWP.Utf8JsonGen.Formatters.SimpleWeather.UWP.WNS.FirebaseUWPUserFormatter();
                case 1: return new SimpleWeather.UWP.Utf8JsonGen.Formatters.SimpleWeather.UWP.WNS.WNSChannelFormatter();
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

namespace SimpleWeather.UWP.Utf8JsonGen.Formatters.SimpleWeather.UWP.WNS
{
    using System;
    using Utf8Json;


    internal sealed class FirebaseUWPUserFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.UWP.WNS.FirebaseUWPUser>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public FirebaseUWPUserFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("channel_uri"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("expiration_time"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("package_name"), 2},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("channel_uri"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("expiration_time"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("package_name"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.UWP.WNS.FirebaseUWPUser value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.channel_uri);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteInt64(value.expiration_time);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.package_name);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.UWP.WNS.FirebaseUWPUser Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __channel_uri__ = default(string);
            var __channel_uri__b__ = false;
            var __expiration_time__ = default(long);
            var __expiration_time__b__ = false;
            var __package_name__ = default(string);
            var __package_name__b__ = false;

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
                        __channel_uri__ = reader.ReadString();
                        __channel_uri__b__ = true;
                        break;
                    case 1:
                        __expiration_time__ = reader.ReadInt64();
                        __expiration_time__b__ = true;
                        break;
                    case 2:
                        __package_name__ = reader.ReadString();
                        __package_name__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.UWP.WNS.FirebaseUWPUser();
            if(__channel_uri__b__) ____result.channel_uri = __channel_uri__;
            if(__expiration_time__b__) ____result.expiration_time = __expiration_time__;
            if(__package_name__b__) ____result.package_name = __package_name__;

            return ____result;
        }
    }


    internal sealed class WNSChannelFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.UWP.WNS.WNSChannel>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public WNSChannelFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("ChannelUri"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("ExpirationTime"), 1},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("ChannelUri"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("ExpirationTime"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.UWP.WNS.WNSChannel value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.ChannelUri);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset>().Serialize(ref writer, value.ExpirationTime, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.UWP.WNS.WNSChannel Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __ChannelUri__ = default(string);
            var __ChannelUri__b__ = false;
            var __ExpirationTime__ = default(global::System.DateTimeOffset);
            var __ExpirationTime__b__ = false;

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
                        __ChannelUri__ = reader.ReadString();
                        __ChannelUri__b__ = true;
                        break;
                    case 1:
                        __ExpirationTime__ = formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset>().Deserialize(ref reader, formatterResolver);
                        __ExpirationTime__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.UWP.WNS.WNSChannel();
            if(__ChannelUri__b__) ____result.ChannelUri = __ChannelUri__;
            if(__ExpirationTime__b__) ____result.ExpirationTime = __ExpirationTime__;

            return ____result;
        }
    }

}

#pragma warning disable 168
#pragma warning restore 219
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612
