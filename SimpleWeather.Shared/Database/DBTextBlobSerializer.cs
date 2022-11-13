using SimpleWeather.Utils;
using SQLiteNetExtensions.Extensions.TextBlob;
using System;
using System.Linq;

namespace SimpleWeather.Database
{
    internal class DBTextBlobSerializer : ITextBlobSerializer
    {
        private readonly Utf8Json.IJsonFormatterResolver Resolver;

        public DBTextBlobSerializer()
        {
            Resolver = new Utf8JsonGen.AttrFirstUtf8JsonResolver();
        }

        public object Deserialize(string text, Type type)
        {
            bool useAttrResolver;
            string str;

            // Use our own resolver (custom deserializer) if json string is escaped
            // since the Utf8Json deserializer is alot more strict
            if (text.Contains("\"{\\\""))
            {
                str = text;
                useAttrResolver = true;
            }
            else
            {
                var unescape = new System.Text.StringBuilder(System.Text.RegularExpressions.Regex.Unescape(text));
                if (unescape.Length > 1 && unescape[0] == '"' && unescape[unescape.Length - 1] == '"')
                {
                    unescape.Remove(0, 1);
                    unescape.Remove(unescape.Length - 1, 1);
                }
                str = unescape.ToString();
                useAttrResolver = str.Contains("\\") || str.Contains("[\"{\"") || str.Contains("\"{\"");
            }

            var method = typeof(Utf8Json.JsonSerializer).GetMethod("Deserialize", new Type[] { typeof(string), typeof(Utf8Json.IJsonFormatterResolver) });
            var genMethod = method.MakeGenericMethod(type);
            return genMethod.Invoke(null, new object[] { str, useAttrResolver ? Resolver : JSONParser.Resolver });
        }

        public string Serialize(object element)
        {
            var method = typeof(Utf8Json.JsonSerializer).GetMethods().Single(m =>
                m.Name == "ToJsonString" &&
                m.GetParameters().Length == 2 &&
                m.IsGenericMethod &&
                m.ContainsGenericParameters && m.GetParameters()[1].ParameterType == typeof(Utf8Json.IJsonFormatterResolver));
            var genMethod = method.MakeGenericMethod(new Type[] { element.GetType() });
            return genMethod.Invoke(null, new object[] { element, JSONParser.Resolver }) as string;
        }
    }
}