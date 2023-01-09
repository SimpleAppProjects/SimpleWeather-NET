#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168

namespace SimpleWeather.Uno.Utf8JsonGen.Resolvers
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
            lookup = new global::System.Collections.Generic.Dictionary<Type, int>(7)
            {
                {typeof(global::System.Collections.Generic.List<global::SimpleWeather.Uno.Radar.RainViewer.RadarItem>), 0 },
                {typeof(global::System.Collections.Generic.List<global::SimpleWeather.Uno.Radar.RainViewer.Infrared>), 1 },
                {typeof(global::SimpleWeather.Uno.Radar.RainViewer.RadarItem), 2 },
                {typeof(global::SimpleWeather.Uno.Radar.RainViewer.Radar), 3 },
                {typeof(global::SimpleWeather.Uno.Radar.RainViewer.Infrared), 4 },
                {typeof(global::SimpleWeather.Uno.Radar.RainViewer.Satellite), 5 },
                {typeof(global::SimpleWeather.Uno.Radar.RainViewer.Rootobject), 6 },
            };
        }

        internal static object GetFormatter(Type t)
        {
            int key;
            if (!lookup.TryGetValue(t, out key)) return null;

            switch (key)
            {
                case 0: return new global::Utf8Json.Formatters.ListFormatter<global::SimpleWeather.Uno.Radar.RainViewer.RadarItem>();
                case 1: return new global::Utf8Json.Formatters.ListFormatter<global::SimpleWeather.Uno.Radar.RainViewer.Infrared>();
                case 2: return new SimpleWeather.Uno.Utf8JsonGen.Formatters.SimpleWeather.Uno.Radar.RainViewer.RadarItemFormatter();
                case 3: return new SimpleWeather.Uno.Utf8JsonGen.Formatters.SimpleWeather.Uno.Radar.RainViewer.RadarFormatter();
                case 4: return new SimpleWeather.Uno.Utf8JsonGen.Formatters.SimpleWeather.Uno.Radar.RainViewer.InfraredFormatter();
                case 5: return new SimpleWeather.Uno.Utf8JsonGen.Formatters.SimpleWeather.Uno.Radar.RainViewer.SatelliteFormatter();
                case 6: return new SimpleWeather.Uno.Utf8JsonGen.Formatters.SimpleWeather.Uno.Radar.RainViewer.RootobjectFormatter();
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

namespace SimpleWeather.Uno.Utf8JsonGen.Formatters.SimpleWeather.Uno.Radar.RainViewer
{
    using System;
    using Utf8Json;


    public sealed class RadarItemFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.Uno.Radar.RainViewer.RadarItem>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public RadarItemFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("time"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("path"), 1},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("time"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("path"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.Uno.Radar.RainViewer.RadarItem value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteInt32(value.time);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.path);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.Uno.Radar.RainViewer.RadarItem Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __time__ = default(int);
            var __time__b__ = false;
            var __path__ = default(string);
            var __path__b__ = false;

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
                        __time__ = reader.ReadInt32();
                        __time__b__ = true;
                        break;
                    case 1:
                        __path__ = reader.ReadString();
                        __path__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.Uno.Radar.RainViewer.RadarItem();
            if(__time__b__) ____result.time = __time__;
            if(__path__b__) ____result.path = __path__;

            return ____result;
        }
    }


    public sealed class RadarFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.Uno.Radar.RainViewer.Radar>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public RadarFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("past"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("nowcast"), 1},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("past"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("nowcast"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.Uno.Radar.RainViewer.Radar value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<global::SimpleWeather.Uno.Radar.RainViewer.RadarItem>>().Serialize(ref writer, value.past, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<global::SimpleWeather.Uno.Radar.RainViewer.RadarItem>>().Serialize(ref writer, value.nowcast, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.Uno.Radar.RainViewer.Radar Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __past__ = default(global::System.Collections.Generic.List<global::SimpleWeather.Uno.Radar.RainViewer.RadarItem>);
            var __past__b__ = false;
            var __nowcast__ = default(global::System.Collections.Generic.List<global::SimpleWeather.Uno.Radar.RainViewer.RadarItem>);
            var __nowcast__b__ = false;

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
                        __past__ = formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<global::SimpleWeather.Uno.Radar.RainViewer.RadarItem>>().Deserialize(ref reader, formatterResolver);
                        __past__b__ = true;
                        break;
                    case 1:
                        __nowcast__ = formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<global::SimpleWeather.Uno.Radar.RainViewer.RadarItem>>().Deserialize(ref reader, formatterResolver);
                        __nowcast__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.Uno.Radar.RainViewer.Radar();
            if(__past__b__) ____result.past = __past__;
            if(__nowcast__b__) ____result.nowcast = __nowcast__;

            return ____result;
        }
    }


    public sealed class InfraredFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.Uno.Radar.RainViewer.Infrared>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public InfraredFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("time"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("path"), 1},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("time"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("path"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.Uno.Radar.RainViewer.Infrared value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteInt32(value.time);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.path);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.Uno.Radar.RainViewer.Infrared Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __time__ = default(int);
            var __time__b__ = false;
            var __path__ = default(string);
            var __path__b__ = false;

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
                        __time__ = reader.ReadInt32();
                        __time__b__ = true;
                        break;
                    case 1:
                        __path__ = reader.ReadString();
                        __path__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.Uno.Radar.RainViewer.Infrared();
            if(__time__b__) ____result.time = __time__;
            if(__path__b__) ____result.path = __path__;

            return ____result;
        }
    }


    public sealed class SatelliteFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.Uno.Radar.RainViewer.Satellite>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public SatelliteFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("infrared"), 0},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("infrared"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.Uno.Radar.RainViewer.Satellite value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<global::SimpleWeather.Uno.Radar.RainViewer.Infrared>>().Serialize(ref writer, value.infrared, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.Uno.Radar.RainViewer.Satellite Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __infrared__ = default(global::System.Collections.Generic.List<global::SimpleWeather.Uno.Radar.RainViewer.Infrared>);
            var __infrared__b__ = false;

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
                        __infrared__ = formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<global::SimpleWeather.Uno.Radar.RainViewer.Infrared>>().Deserialize(ref reader, formatterResolver);
                        __infrared__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.Uno.Radar.RainViewer.Satellite();
            if(__infrared__b__) ____result.infrared = __infrared__;

            return ____result;
        }
    }


    public sealed class RootobjectFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.Uno.Radar.RainViewer.Rootobject>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public RootobjectFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("version"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("generated"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("host"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("radar"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("satellite"), 4},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("version"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("generated"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("host"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("radar"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("satellite"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.Uno.Radar.RainViewer.Rootobject value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.version);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteInt32(value.generated);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.host);
            writer.WriteRaw(this.____stringByteKeys[3]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Uno.Radar.RainViewer.Radar>().Serialize(ref writer, value.radar, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[4]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Uno.Radar.RainViewer.Satellite>().Serialize(ref writer, value.satellite, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.Uno.Radar.RainViewer.Rootobject Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __version__ = default(string);
            var __version__b__ = false;
            var __generated__ = default(int);
            var __generated__b__ = false;
            var __host__ = default(string);
            var __host__b__ = false;
            var __radar__ = default(global::SimpleWeather.Uno.Radar.RainViewer.Radar);
            var __radar__b__ = false;
            var __satellite__ = default(global::SimpleWeather.Uno.Radar.RainViewer.Satellite);
            var __satellite__b__ = false;

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
                        __version__ = reader.ReadString();
                        __version__b__ = true;
                        break;
                    case 1:
                        __generated__ = reader.ReadInt32();
                        __generated__b__ = true;
                        break;
                    case 2:
                        __host__ = reader.ReadString();
                        __host__b__ = true;
                        break;
                    case 3:
                        __radar__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Uno.Radar.RainViewer.Radar>().Deserialize(ref reader, formatterResolver);
                        __radar__b__ = true;
                        break;
                    case 4:
                        __satellite__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Uno.Radar.RainViewer.Satellite>().Deserialize(ref reader, formatterResolver);
                        __satellite__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.Uno.Radar.RainViewer.Rootobject();
            if(__version__b__) ____result.version = __version__;
            if(__generated__b__) ____result.generated = __generated__;
            if(__host__b__) ____result.host = __host__;
            if(__radar__b__) ____result.radar = __radar__;
            if(__satellite__b__) ____result.satellite = __satellite__;

            return ____result;
        }
    }

}

#pragma warning disable 168
#pragma warning restore 219
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612
