using SimpleWeather.Utils;
using SQLiteNetExtensions.Extensions.TextBlob;
using System;
using System.Text.Json;

namespace SimpleWeather.Database
{
    internal class DBTextBlobSerializer : ITextBlobSerializer
    {
        public object Deserialize(string text, Type type)
        {
            return JsonSerializer.Deserialize(text, type, JSONParser.DefaultSettings);
        }

        public string Serialize(object element)
        {
            return JsonSerializer.Serialize(element, JSONParser.DefaultSettings);
        }
    }
}