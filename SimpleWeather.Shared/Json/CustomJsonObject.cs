using System;
using System.Text;

namespace SimpleWeather.Utils
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public abstract class CustomJsonObject
    {
        public abstract String ToJson();

        /// <summary>
        /// FromJson
        /// </summary>
        /// <param name="reader"></param>
        /// <exception cref="JsonException"></exception>
        public abstract void FromJson(ref Utf8JsonReader reader);
    }

    internal class CustomJsonConverter<T> : JsonConverter<T> where T : CustomJsonObject
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return JSONParser.CustomDeserializer<T>(ref reader);
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            JSONParser.CustomSerializer(writer, value);
        }
    }

    internal class CustomJsonListConverter<T> : JsonConverter<IEnumerable<T>> where T : CustomJsonObject
    {
        public override IEnumerable<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var list = new List<T>();
            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
            {
                if (reader.TokenType == JsonTokenType.String || reader.TokenType == JsonTokenType.StartObject)
                    list.Add(JSONParser.CustomDeserializer<T>(ref reader));
            }
            return list;
        }

        public override void Write(Utf8JsonWriter writer, IEnumerable<T> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            if (value != null)
            {
                foreach (var item in value)
                {
                    JSONParser.CustomSerializer(writer, item);
                }
            }
            writer.WriteEndArray();
        }
    }

    public static partial class JSONParser
    {
        public static string CustomSerializer<T>(T obj) where T : CustomJsonObject
        {
            using var stream = new MemoryStream();
            var writer = new Utf8JsonWriter(stream);
            CustomSerializer(writer, obj);
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        public static T CustomDeserializer<T>(string json) where T : CustomJsonObject
        {
            var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json));
            return CustomDeserializer<T>(ref reader);
        }

        internal static object CustomDeserializer(ref Utf8JsonReader reader, Type type)
        {
            CustomJsonObject obj = null;
            try
            {
                obj = Activator.CreateInstance(type, true) as CustomJsonObject;
                obj.FromJson(ref reader);
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Error, ex, "SimpleWeather: CustomJsonConverter: error invoking FromJson method");
            }

            return obj;
        }

        internal static T CustomDeserializer<T>(ref Utf8JsonReader reader) where T : CustomJsonObject
        {
            T obj = null;
            try
            {
                obj = Activator.CreateInstance(typeof(T), true) as T;
                obj.FromJson(ref reader);
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Error, ex, "SimpleWeather: CustomJsonConverter: error invoking FromJson method");
            }

            return obj;
        }

        internal static void CustomSerializer<T>(Utf8JsonWriter writer, T value) where T : CustomJsonObject
        {
            writer.WriteRawValue(value.ToJson());
        }
    }
}
