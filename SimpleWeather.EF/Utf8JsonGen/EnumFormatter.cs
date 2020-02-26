using SimpleWeather.EF.Utf8JsonGen.Formatters;
using System;
using Utf8Json;

// EnumFormatter.cs (ab0e49cc1bab887dd5b5d9d193abf9d59e49ddfb)
namespace SimpleWeather.EF.Utf8JsonGen.Formatters
{
    public static class EnumFormatterHelper
    {
        public static object GetSerializeDelegate(Type type)
        {
            var underlyingType = Enum.GetUnderlyingType(type);

            // Boxed
            JsonSerializeAction<object> f;
            if (underlyingType == typeof(byte))
            {
                f = (ref JsonWriter writer, object value, IJsonFormatterResolver _) => writer.WriteByte((byte)value);
            }
            else if (underlyingType == typeof(sbyte))
            {
                f = (ref JsonWriter writer, object value, IJsonFormatterResolver _) => writer.WriteSByte((sbyte)value);
            }
            else if (underlyingType == typeof(short))
            {
                f = (ref JsonWriter writer, object value, IJsonFormatterResolver _) => writer.WriteInt16((short)value);
            }
            else if (underlyingType == typeof(ushort))
            {
                f = (ref JsonWriter writer, object value, IJsonFormatterResolver _) => writer.WriteUInt16((ushort)value);
            }
            else if (underlyingType == typeof(int))
            {
                f = (ref JsonWriter writer, object value, IJsonFormatterResolver _) => writer.WriteInt32((int)value);
            }
            else if (underlyingType == typeof(uint))
            {
                f = (ref JsonWriter writer, object value, IJsonFormatterResolver _) => writer.WriteUInt32((uint)value);
            }
            else if (underlyingType == typeof(long))
            {
                f = (ref JsonWriter writer, object value, IJsonFormatterResolver _) => writer.WriteInt64((long)value);
            }
            else if (underlyingType == typeof(ulong))
            {
                f = (ref JsonWriter writer, object value, IJsonFormatterResolver _) => writer.WriteUInt64((ulong)value);
            }
            else
            {
                throw new InvalidOperationException("Type is not Enum. Type:" + type);
            }
            return f;
        }

        public static object GetDeserializeDelegate(Type type)
        {
            var underlyingType = Enum.GetUnderlyingType(type);

            // Boxed
            JsonDeserializeFunc<object> f;
            if (underlyingType == typeof(byte))
            {
                f = (ref JsonReader reader, IJsonFormatterResolver _) => (object)reader.ReadByte();
            }
            else if (underlyingType == typeof(sbyte))
            {
                f = (ref JsonReader reader, IJsonFormatterResolver _) => (object)reader.ReadSByte();
            }
            else if (underlyingType == typeof(short))
            {
                f = (ref JsonReader reader, IJsonFormatterResolver _) => (object)reader.ReadInt16();
            }
            else if (underlyingType == typeof(ushort))
            {
                f = (ref JsonReader reader, IJsonFormatterResolver _) => (object)reader.ReadUInt16();
            }
            else if (underlyingType == typeof(int))
            {
                f = (ref JsonReader reader, IJsonFormatterResolver _) => (object)reader.ReadInt32();
            }
            else if (underlyingType == typeof(uint))
            {
                f = (ref JsonReader reader, IJsonFormatterResolver _) => (object)reader.ReadUInt32();
            }
            else if (underlyingType == typeof(long))
            {
                f = (ref JsonReader reader, IJsonFormatterResolver _) => (object)reader.ReadInt64();
            }
            else if (underlyingType == typeof(ulong))
            {
                f = (ref JsonReader reader, IJsonFormatterResolver _) => (object)reader.ReadUInt64();
            }
            else
            {
                throw new InvalidOperationException("Type is not Enum. Type:" + type);
            }
            return f;
        }
    }
}

namespace SimpleWeather.EF.Utf8JsonGen.Formatters
{
    // can inehrit for set optimize manual serialize/deserialize func.
    public class EnumFormatter<T> : IJsonFormatter<T>, IObjectPropertyNameFormatter<T>
    {
        readonly static JsonSerializeAction<T> defaultSerializeByUnderlyingValue;
        readonly static JsonDeserializeFunc<T> defaultDeserializeByUnderlyingValue;

        static EnumFormatter()
        {
            // boxed... or generate...
            {
                var serialize = EnumFormatterHelper.GetSerializeDelegate(typeof(T));
                var boxSerialize = (JsonSerializeAction<object>)serialize;
                defaultSerializeByUnderlyingValue = (ref JsonWriter writer, T value, IJsonFormatterResolver _) => boxSerialize.Invoke(ref writer, (object)value, _);
            }

            {
                var deserialize = EnumFormatterHelper.GetDeserializeDelegate(typeof(T));
                var boxDeserialize = (JsonDeserializeFunc<object>)deserialize;
                defaultDeserializeByUnderlyingValue = (ref JsonReader reader, IJsonFormatterResolver _) => (T)boxDeserialize.Invoke(ref reader, _);
            }
        }

        readonly JsonSerializeAction<T> serializeByUnderlyingValue;
        readonly JsonDeserializeFunc<T> deserializeByUnderlyingValue;

        public EnumFormatter()
        {
            this.serializeByUnderlyingValue = defaultSerializeByUnderlyingValue;
            this.deserializeByUnderlyingValue = defaultDeserializeByUnderlyingValue;
        }

        public void Serialize(ref JsonWriter writer, T value, IJsonFormatterResolver formatterResolver)
        {
            serializeByUnderlyingValue(ref writer, value, formatterResolver);
        }

        public T Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            return deserializeByUnderlyingValue(ref reader, formatterResolver);
        }

        public void SerializeToPropertyName(ref JsonWriter writer, T value, IJsonFormatterResolver formatterResolver)
        {
            writer.WriteQuotation();
            Serialize(ref writer, value, formatterResolver);
            writer.WriteQuotation();
        }

        public T DeserializeFromPropertyName(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            var token = reader.GetCurrentJsonToken();
            if (token != JsonToken.String) throw new InvalidOperationException("Can't parse JSON to Enum format.");
            reader.AdvanceOffset(1); // skip \""
            var t = Deserialize(ref reader, formatterResolver); // token is Number
            reader.SkipWhiteSpace();
            reader.AdvanceOffset(1); // skip \""
            return t;
        }
    }
}