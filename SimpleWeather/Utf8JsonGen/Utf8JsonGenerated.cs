#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168

namespace SimpleWeather.Utf8JsonGen.Resolvers
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
            lookup = new global::System.Collections.Generic.Dictionary<Type, int>(108)
            {
                {typeof(global::SimpleWeather.AQICN.Attribution[]), 0 },
                {typeof(global::SimpleWeather.Bing.Value[]), 1 },
                {typeof(global::SimpleWeather.Bing.Resource[]), 2 },
                {typeof(global::SimpleWeather.Bing.Resourceset[]), 3 },
                {typeof(global::SimpleWeather.HERE.Timesegment[]), 4 },
                {typeof(global::SimpleWeather.HERE.Alert[]), 5 },
                {typeof(global::SimpleWeather.HERE.Additionaldata[]), 6 },
                {typeof(global::SimpleWeather.HERE.Suggestion[]), 7 },
                {typeof(global::SimpleWeather.HERE.Navigationposition[]), 8 },
                {typeof(global::SimpleWeather.HERE.Result[]), 9 },
                {typeof(global::SimpleWeather.HERE.View[]), 10 },
                {typeof(global::SimpleWeather.HERE.Observation[]), 11 },
                {typeof(global::SimpleWeather.HERE.Location[]), 12 },
                {typeof(global::SimpleWeather.HERE.Forecast[]), 13 },
                {typeof(global::SimpleWeather.HERE.Forecast1[]), 14 },
                {typeof(global::SimpleWeather.HERE.Astronomy1[]), 15 },
                {typeof(global::SimpleWeather.NWS.AlertGraph[]), 16 },
                {typeof(global::SimpleWeather.NWS.Period[]), 17 },
                {typeof(global::SimpleWeather.OpenWeather.Weather[]), 18 },
                {typeof(global::SimpleWeather.OpenWeather.Hourly[]), 19 },
                {typeof(global::SimpleWeather.OpenWeather.Daily[]), 20 },
                {typeof(global::SimpleWeather.WeatherYahoo.Forecast[]), 21 },
                {typeof(global::SimpleWeather.AQICN.Attribution), 22 },
                {typeof(global::SimpleWeather.AQICN.City), 23 },
                {typeof(global::SimpleWeather.AQICN.Co), 24 },
                {typeof(global::SimpleWeather.AQICN.H), 25 },
                {typeof(global::SimpleWeather.AQICN.No2), 26 },
                {typeof(global::SimpleWeather.AQICN.O3), 27 },
                {typeof(global::SimpleWeather.AQICN.P), 28 },
                {typeof(global::SimpleWeather.AQICN.Pm10), 29 },
                {typeof(global::SimpleWeather.AQICN.Pm25), 30 },
                {typeof(global::SimpleWeather.AQICN.So2), 31 },
                {typeof(global::SimpleWeather.AQICN.T), 32 },
                {typeof(global::SimpleWeather.AQICN.W), 33 },
                {typeof(global::SimpleWeather.AQICN.Iaqi), 34 },
                {typeof(global::SimpleWeather.AQICN.Time), 35 },
                {typeof(global::SimpleWeather.AQICN.Debug), 36 },
                {typeof(global::SimpleWeather.AQICN.Data), 37 },
                {typeof(global::SimpleWeather.AQICN.Rootobject), 38 },
                {typeof(global::SimpleWeather.Bing.Address), 39 },
                {typeof(global::SimpleWeather.Bing.Value), 40 },
                {typeof(global::SimpleWeather.Bing.Resource), 41 },
                {typeof(global::SimpleWeather.Bing.Resourceset), 42 },
                {typeof(global::SimpleWeather.Bing.AC_Rootobject), 43 },
                {typeof(global::SimpleWeather.HERE.Timesegment), 44 },
                {typeof(global::SimpleWeather.HERE.Alert), 45 },
                {typeof(global::SimpleWeather.HERE.Alerts), 46 },
                {typeof(global::SimpleWeather.HERE.Additionaldata), 47 },
                {typeof(global::SimpleWeather.HERE.Address), 48 },
                {typeof(global::SimpleWeather.HERE.Suggestion), 49 },
                {typeof(global::SimpleWeather.HERE.AC_Rootobject), 50 },
                {typeof(global::SimpleWeather.HERE.Metainfo), 51 },
                {typeof(global::SimpleWeather.HERE.Matchquality), 52 },
                {typeof(global::SimpleWeather.HERE.Displayposition), 53 },
                {typeof(global::SimpleWeather.HERE.Navigationposition), 54 },
                {typeof(global::SimpleWeather.HERE.Timezone), 55 },
                {typeof(global::SimpleWeather.HERE.Admininfo), 56 },
                {typeof(global::SimpleWeather.HERE.GeoLocation), 57 },
                {typeof(global::SimpleWeather.HERE.Result), 58 },
                {typeof(global::SimpleWeather.HERE.View), 59 },
                {typeof(global::SimpleWeather.HERE.Response), 60 },
                {typeof(global::SimpleWeather.HERE.Geo_Rootobject), 61 },
                {typeof(global::SimpleWeather.HERE.TokenRootobject), 62 },
                {typeof(global::SimpleWeather.HERE.Token), 63 },
                {typeof(global::SimpleWeather.HERE.Observation), 64 },
                {typeof(global::SimpleWeather.HERE.Location), 65 },
                {typeof(global::SimpleWeather.HERE.Observations), 66 },
                {typeof(global::SimpleWeather.HERE.Forecast), 67 },
                {typeof(global::SimpleWeather.HERE.Forecastlocation), 68 },
                {typeof(global::SimpleWeather.HERE.Dailyforecasts), 69 },
                {typeof(global::SimpleWeather.HERE.Forecast1), 70 },
                {typeof(global::SimpleWeather.HERE.Forecastlocation1), 71 },
                {typeof(global::SimpleWeather.HERE.Hourlyforecasts), 72 },
                {typeof(global::SimpleWeather.HERE.Astronomy1), 73 },
                {typeof(global::SimpleWeather.HERE.Astronomy), 74 },
                {typeof(global::SimpleWeather.HERE.Rootobject), 75 },
                {typeof(global::SimpleWeather.NWS.AlertGraph), 76 },
                {typeof(global::SimpleWeather.NWS.AlertRootobject), 77 },
                {typeof(global::SimpleWeather.NWS.PointsRootobject), 78 },
                {typeof(global::SimpleWeather.NWS.Period), 79 },
                {typeof(global::SimpleWeather.NWS.ForecastRootobject), 80 },
                {typeof(global::SimpleWeather.NWS.ObservationsStationsRootobject), 81 },
                {typeof(global::SimpleWeather.NWS.Temperature), 82 },
                {typeof(global::SimpleWeather.NWS.Dewpoint), 83 },
                {typeof(global::SimpleWeather.NWS.Winddirection), 84 },
                {typeof(global::SimpleWeather.NWS.Windspeed), 85 },
                {typeof(global::SimpleWeather.NWS.Barometricpressure), 86 },
                {typeof(global::SimpleWeather.NWS.Visibility), 87 },
                {typeof(global::SimpleWeather.NWS.Relativehumidity), 88 },
                {typeof(global::SimpleWeather.NWS.ObservationsCurrentRootobject), 89 },
                {typeof(global::SimpleWeather.OpenWeather.Rain), 90 },
                {typeof(global::SimpleWeather.OpenWeather.Snow), 91 },
                {typeof(global::SimpleWeather.OpenWeather.Weather), 92 },
                {typeof(global::SimpleWeather.OpenWeather.Current), 93 },
                {typeof(global::SimpleWeather.OpenWeather.Hourly), 94 },
                {typeof(global::SimpleWeather.OpenWeather.Temp), 95 },
                {typeof(global::SimpleWeather.OpenWeather.Feels_Like), 96 },
                {typeof(global::SimpleWeather.OpenWeather.Daily), 97 },
                {typeof(global::SimpleWeather.OpenWeather.Rootobject), 98 },
                {typeof(global::SimpleWeather.WeatherYahoo.Location), 99 },
                {typeof(global::SimpleWeather.WeatherYahoo.Wind), 100 },
                {typeof(global::SimpleWeather.WeatherYahoo.Atmosphere), 101 },
                {typeof(global::SimpleWeather.WeatherYahoo.Astronomy), 102 },
                {typeof(global::SimpleWeather.WeatherYahoo.Condition), 103 },
                {typeof(global::SimpleWeather.WeatherYahoo.Current_Observation), 104 },
                {typeof(global::SimpleWeather.WeatherYahoo.Forecast), 105 },
                {typeof(global::SimpleWeather.WeatherYahoo.Rootobject), 106 },
                {typeof(global::SimpleWeather.WeatherData.Images.Model.ImageData), 107 },
            };
        }

        internal static object GetFormatter(Type t)
        {
            int key;
            if (!lookup.TryGetValue(t, out key)) return null;

            switch (key)
            {
                case 0: return new global::Utf8Json.Formatters.ArrayFormatter<global::SimpleWeather.AQICN.Attribution>();
                case 1: return new global::Utf8Json.Formatters.ArrayFormatter<global::SimpleWeather.Bing.Value>();
                case 2: return new global::Utf8Json.Formatters.ArrayFormatter<global::SimpleWeather.Bing.Resource>();
                case 3: return new global::Utf8Json.Formatters.ArrayFormatter<global::SimpleWeather.Bing.Resourceset>();
                case 4: return new global::Utf8Json.Formatters.ArrayFormatter<global::SimpleWeather.HERE.Timesegment>();
                case 5: return new global::Utf8Json.Formatters.ArrayFormatter<global::SimpleWeather.HERE.Alert>();
                case 6: return new global::Utf8Json.Formatters.ArrayFormatter<global::SimpleWeather.HERE.Additionaldata>();
                case 7: return new global::Utf8Json.Formatters.ArrayFormatter<global::SimpleWeather.HERE.Suggestion>();
                case 8: return new global::Utf8Json.Formatters.ArrayFormatter<global::SimpleWeather.HERE.Navigationposition>();
                case 9: return new global::Utf8Json.Formatters.ArrayFormatter<global::SimpleWeather.HERE.Result>();
                case 10: return new global::Utf8Json.Formatters.ArrayFormatter<global::SimpleWeather.HERE.View>();
                case 11: return new global::Utf8Json.Formatters.ArrayFormatter<global::SimpleWeather.HERE.Observation>();
                case 12: return new global::Utf8Json.Formatters.ArrayFormatter<global::SimpleWeather.HERE.Location>();
                case 13: return new global::Utf8Json.Formatters.ArrayFormatter<global::SimpleWeather.HERE.Forecast>();
                case 14: return new global::Utf8Json.Formatters.ArrayFormatter<global::SimpleWeather.HERE.Forecast1>();
                case 15: return new global::Utf8Json.Formatters.ArrayFormatter<global::SimpleWeather.HERE.Astronomy1>();
                case 16: return new global::Utf8Json.Formatters.ArrayFormatter<global::SimpleWeather.NWS.AlertGraph>();
                case 17: return new global::Utf8Json.Formatters.ArrayFormatter<global::SimpleWeather.NWS.Period>();
                case 18: return new global::Utf8Json.Formatters.ArrayFormatter<global::SimpleWeather.OpenWeather.Weather>();
                case 19: return new global::Utf8Json.Formatters.ArrayFormatter<global::SimpleWeather.OpenWeather.Hourly>();
                case 20: return new global::Utf8Json.Formatters.ArrayFormatter<global::SimpleWeather.OpenWeather.Daily>();
                case 21: return new global::Utf8Json.Formatters.ArrayFormatter<global::SimpleWeather.WeatherYahoo.Forecast>();
                case 22: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.AQICN.AttributionFormatter();
                case 23: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.AQICN.CityFormatter();
                case 24: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.AQICN.CoFormatter();
                case 25: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.AQICN.HFormatter();
                case 26: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.AQICN.No2Formatter();
                case 27: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.AQICN.O3Formatter();
                case 28: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.AQICN.PFormatter();
                case 29: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.AQICN.Pm10Formatter();
                case 30: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.AQICN.Pm25Formatter();
                case 31: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.AQICN.So2Formatter();
                case 32: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.AQICN.TFormatter();
                case 33: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.AQICN.WFormatter();
                case 34: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.AQICN.IaqiFormatter();
                case 35: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.AQICN.TimeFormatter();
                case 36: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.AQICN.DebugFormatter();
                case 37: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.AQICN.DataFormatter();
                case 38: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.AQICN.RootobjectFormatter();
                case 39: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.Bing.AddressFormatter();
                case 40: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.Bing.ValueFormatter();
                case 41: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.Bing.ResourceFormatter();
                case 42: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.Bing.ResourcesetFormatter();
                case 43: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.Bing.AC_RootobjectFormatter();
                case 44: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.TimesegmentFormatter();
                case 45: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.AlertFormatter();
                case 46: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.AlertsFormatter();
                case 47: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.AdditionaldataFormatter();
                case 48: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.AddressFormatter();
                case 49: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.SuggestionFormatter();
                case 50: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.AC_RootobjectFormatter();
                case 51: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.MetainfoFormatter();
                case 52: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.MatchqualityFormatter();
                case 53: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.DisplaypositionFormatter();
                case 54: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.NavigationpositionFormatter();
                case 55: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.TimezoneFormatter();
                case 56: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.AdmininfoFormatter();
                case 57: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.GeoLocationFormatter();
                case 58: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.ResultFormatter();
                case 59: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.ViewFormatter();
                case 60: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.ResponseFormatter();
                case 61: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.Geo_RootobjectFormatter();
                case 62: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.TokenRootobjectFormatter();
                case 63: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.TokenFormatter();
                case 64: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.ObservationFormatter();
                case 65: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.LocationFormatter();
                case 66: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.ObservationsFormatter();
                case 67: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.ForecastFormatter();
                case 68: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.ForecastlocationFormatter();
                case 69: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.DailyforecastsFormatter();
                case 70: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.Forecast1Formatter();
                case 71: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.Forecastlocation1Formatter();
                case 72: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.HourlyforecastsFormatter();
                case 73: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.Astronomy1Formatter();
                case 74: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.AstronomyFormatter();
                case 75: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.RootobjectFormatter();
                case 76: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.NWS.AlertGraphFormatter();
                case 77: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.NWS.AlertRootobjectFormatter();
                case 78: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.NWS.PointsRootobjectFormatter();
                case 79: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.NWS.PeriodFormatter();
                case 80: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.NWS.ForecastRootobjectFormatter();
                case 81: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.NWS.ObservationsStationsRootobjectFormatter();
                case 82: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.NWS.TemperatureFormatter();
                case 83: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.NWS.DewpointFormatter();
                case 84: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.NWS.WinddirectionFormatter();
                case 85: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.NWS.WindspeedFormatter();
                case 86: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.NWS.BarometricpressureFormatter();
                case 87: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.NWS.VisibilityFormatter();
                case 88: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.NWS.RelativehumidityFormatter();
                case 89: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.NWS.ObservationsCurrentRootobjectFormatter();
                case 90: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.OpenWeather.RainFormatter();
                case 91: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.OpenWeather.SnowFormatter();
                case 92: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.OpenWeather.WeatherFormatter();
                case 93: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.OpenWeather.CurrentFormatter();
                case 94: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.OpenWeather.HourlyFormatter();
                case 95: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.OpenWeather.TempFormatter();
                case 96: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.OpenWeather.Feels_LikeFormatter();
                case 97: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.OpenWeather.DailyFormatter();
                case 98: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.OpenWeather.RootobjectFormatter();
                case 99: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.WeatherYahoo.LocationFormatter();
                case 100: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.WeatherYahoo.WindFormatter();
                case 101: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.WeatherYahoo.AtmosphereFormatter();
                case 102: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.WeatherYahoo.AstronomyFormatter();
                case 103: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.WeatherYahoo.ConditionFormatter();
                case 104: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.WeatherYahoo.Current_ObservationFormatter();
                case 105: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.WeatherYahoo.ForecastFormatter();
                case 106: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.WeatherYahoo.RootobjectFormatter();
                case 107: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.WeatherData.Images.Model.ImageDataFormatter();
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

namespace SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.AQICN
{
    using System;
    using Utf8Json;


    public sealed class AttributionFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.AQICN.Attribution>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public AttributionFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("url"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("name"), 1},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("url"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("name"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.AQICN.Attribution value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.url);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.name);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.AQICN.Attribution Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __url__ = default(string);
            var __url__b__ = false;
            var __name__ = default(string);
            var __name__b__ = false;

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
                        __url__ = reader.ReadString();
                        __url__b__ = true;
                        break;
                    case 1:
                        __name__ = reader.ReadString();
                        __name__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.AQICN.Attribution();
            if(__url__b__) ____result.url = __url__;
            if(__name__b__) ____result.name = __name__;

            return ____result;
        }
    }


    public sealed class CityFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.AQICN.City>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public CityFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("geo"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("name"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("url"), 2},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("geo"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("name"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("url"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.AQICN.City value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<float[]>().Serialize(ref writer, value.geo, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.name);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.url);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.AQICN.City Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __geo__ = default(float[]);
            var __geo__b__ = false;
            var __name__ = default(string);
            var __name__b__ = false;
            var __url__ = default(string);
            var __url__b__ = false;

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
                        __geo__ = formatterResolver.GetFormatterWithVerify<float[]>().Deserialize(ref reader, formatterResolver);
                        __geo__b__ = true;
                        break;
                    case 1:
                        __name__ = reader.ReadString();
                        __name__b__ = true;
                        break;
                    case 2:
                        __url__ = reader.ReadString();
                        __url__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.AQICN.City();
            if(__geo__b__) ____result.geo = __geo__;
            if(__name__b__) ____result.name = __name__;
            if(__url__b__) ____result.url = __url__;

            return ____result;
        }
    }


    public sealed class CoFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.AQICN.Co>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public CoFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("v"), 0},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("v"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.AQICN.Co value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteSingle(value.v);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.AQICN.Co Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __v__ = default(float);
            var __v__b__ = false;

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
                        __v__ = reader.ReadSingle();
                        __v__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.AQICN.Co();
            if(__v__b__) ____result.v = __v__;

            return ____result;
        }
    }


    public sealed class HFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.AQICN.H>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public HFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("v"), 0},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("v"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.AQICN.H value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteSingle(value.v);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.AQICN.H Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __v__ = default(float);
            var __v__b__ = false;

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
                        __v__ = reader.ReadSingle();
                        __v__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.AQICN.H();
            if(__v__b__) ____result.v = __v__;

            return ____result;
        }
    }


    public sealed class No2Formatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.AQICN.No2>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public No2Formatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("v"), 0},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("v"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.AQICN.No2 value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteSingle(value.v);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.AQICN.No2 Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __v__ = default(float);
            var __v__b__ = false;

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
                        __v__ = reader.ReadSingle();
                        __v__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.AQICN.No2();
            if(__v__b__) ____result.v = __v__;

            return ____result;
        }
    }


    public sealed class O3Formatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.AQICN.O3>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public O3Formatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("v"), 0},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("v"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.AQICN.O3 value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteSingle(value.v);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.AQICN.O3 Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __v__ = default(float);
            var __v__b__ = false;

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
                        __v__ = reader.ReadSingle();
                        __v__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.AQICN.O3();
            if(__v__b__) ____result.v = __v__;

            return ____result;
        }
    }


    public sealed class PFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.AQICN.P>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public PFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("v"), 0},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("v"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.AQICN.P value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteSingle(value.v);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.AQICN.P Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __v__ = default(float);
            var __v__b__ = false;

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
                        __v__ = reader.ReadSingle();
                        __v__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.AQICN.P();
            if(__v__b__) ____result.v = __v__;

            return ____result;
        }
    }


    public sealed class Pm10Formatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.AQICN.Pm10>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public Pm10Formatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("v"), 0},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("v"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.AQICN.Pm10 value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteInt32(value.v);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.AQICN.Pm10 Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __v__ = default(int);
            var __v__b__ = false;

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
                        __v__ = reader.ReadInt32();
                        __v__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.AQICN.Pm10();
            if(__v__b__) ____result.v = __v__;

            return ____result;
        }
    }


    public sealed class Pm25Formatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.AQICN.Pm25>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public Pm25Formatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("v"), 0},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("v"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.AQICN.Pm25 value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteInt32(value.v);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.AQICN.Pm25 Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __v__ = default(int);
            var __v__b__ = false;

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
                        __v__ = reader.ReadInt32();
                        __v__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.AQICN.Pm25();
            if(__v__b__) ____result.v = __v__;

            return ____result;
        }
    }


    public sealed class So2Formatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.AQICN.So2>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public So2Formatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("v"), 0},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("v"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.AQICN.So2 value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteSingle(value.v);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.AQICN.So2 Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __v__ = default(float);
            var __v__b__ = false;

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
                        __v__ = reader.ReadSingle();
                        __v__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.AQICN.So2();
            if(__v__b__) ____result.v = __v__;

            return ____result;
        }
    }


    public sealed class TFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.AQICN.T>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public TFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("v"), 0},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("v"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.AQICN.T value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteSingle(value.v);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.AQICN.T Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __v__ = default(float);
            var __v__b__ = false;

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
                        __v__ = reader.ReadSingle();
                        __v__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.AQICN.T();
            if(__v__b__) ____result.v = __v__;

            return ____result;
        }
    }


    public sealed class WFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.AQICN.W>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public WFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("v"), 0},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("v"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.AQICN.W value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteSingle(value.v);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.AQICN.W Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __v__ = default(float);
            var __v__b__ = false;

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
                        __v__ = reader.ReadSingle();
                        __v__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.AQICN.W();
            if(__v__b__) ____result.v = __v__;

            return ____result;
        }
    }


    public sealed class IaqiFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.AQICN.Iaqi>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public IaqiFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("co"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("h"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("no2"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("o3"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("p"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("pm10"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("pm25"), 6},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("so2"), 7},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("t"), 8},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("w"), 9},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("co"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("h"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("no2"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("o3"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("p"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("pm10"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("pm25"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("so2"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("t"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("w"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.AQICN.Iaqi value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.AQICN.Co>().Serialize(ref writer, value.co, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.AQICN.H>().Serialize(ref writer, value.h, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[2]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.AQICN.No2>().Serialize(ref writer, value.no2, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[3]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.AQICN.O3>().Serialize(ref writer, value.o3, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[4]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.AQICN.P>().Serialize(ref writer, value.p, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[5]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.AQICN.Pm10>().Serialize(ref writer, value.pm10, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[6]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.AQICN.Pm25>().Serialize(ref writer, value.pm25, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[7]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.AQICN.So2>().Serialize(ref writer, value.so2, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[8]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.AQICN.T>().Serialize(ref writer, value.t, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[9]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.AQICN.W>().Serialize(ref writer, value.w, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.AQICN.Iaqi Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __co__ = default(global::SimpleWeather.AQICN.Co);
            var __co__b__ = false;
            var __h__ = default(global::SimpleWeather.AQICN.H);
            var __h__b__ = false;
            var __no2__ = default(global::SimpleWeather.AQICN.No2);
            var __no2__b__ = false;
            var __o3__ = default(global::SimpleWeather.AQICN.O3);
            var __o3__b__ = false;
            var __p__ = default(global::SimpleWeather.AQICN.P);
            var __p__b__ = false;
            var __pm10__ = default(global::SimpleWeather.AQICN.Pm10);
            var __pm10__b__ = false;
            var __pm25__ = default(global::SimpleWeather.AQICN.Pm25);
            var __pm25__b__ = false;
            var __so2__ = default(global::SimpleWeather.AQICN.So2);
            var __so2__b__ = false;
            var __t__ = default(global::SimpleWeather.AQICN.T);
            var __t__b__ = false;
            var __w__ = default(global::SimpleWeather.AQICN.W);
            var __w__b__ = false;

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
                        __co__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.AQICN.Co>().Deserialize(ref reader, formatterResolver);
                        __co__b__ = true;
                        break;
                    case 1:
                        __h__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.AQICN.H>().Deserialize(ref reader, formatterResolver);
                        __h__b__ = true;
                        break;
                    case 2:
                        __no2__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.AQICN.No2>().Deserialize(ref reader, formatterResolver);
                        __no2__b__ = true;
                        break;
                    case 3:
                        __o3__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.AQICN.O3>().Deserialize(ref reader, formatterResolver);
                        __o3__b__ = true;
                        break;
                    case 4:
                        __p__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.AQICN.P>().Deserialize(ref reader, formatterResolver);
                        __p__b__ = true;
                        break;
                    case 5:
                        __pm10__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.AQICN.Pm10>().Deserialize(ref reader, formatterResolver);
                        __pm10__b__ = true;
                        break;
                    case 6:
                        __pm25__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.AQICN.Pm25>().Deserialize(ref reader, formatterResolver);
                        __pm25__b__ = true;
                        break;
                    case 7:
                        __so2__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.AQICN.So2>().Deserialize(ref reader, formatterResolver);
                        __so2__b__ = true;
                        break;
                    case 8:
                        __t__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.AQICN.T>().Deserialize(ref reader, formatterResolver);
                        __t__b__ = true;
                        break;
                    case 9:
                        __w__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.AQICN.W>().Deserialize(ref reader, formatterResolver);
                        __w__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.AQICN.Iaqi();
            if(__co__b__) ____result.co = __co__;
            if(__h__b__) ____result.h = __h__;
            if(__no2__b__) ____result.no2 = __no2__;
            if(__o3__b__) ____result.o3 = __o3__;
            if(__p__b__) ____result.p = __p__;
            if(__pm10__b__) ____result.pm10 = __pm10__;
            if(__pm25__b__) ____result.pm25 = __pm25__;
            if(__so2__b__) ____result.so2 = __so2__;
            if(__t__b__) ____result.t = __t__;
            if(__w__b__) ____result.w = __w__;

            return ____result;
        }
    }


    public sealed class TimeFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.AQICN.Time>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public TimeFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("s"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("tz"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("v"), 2},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("s"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("tz"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("v"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.AQICN.Time value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.s);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.tz);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteInt32(value.v);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.AQICN.Time Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __s__ = default(string);
            var __s__b__ = false;
            var __tz__ = default(string);
            var __tz__b__ = false;
            var __v__ = default(int);
            var __v__b__ = false;

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
                        __s__ = reader.ReadString();
                        __s__b__ = true;
                        break;
                    case 1:
                        __tz__ = reader.ReadString();
                        __tz__b__ = true;
                        break;
                    case 2:
                        __v__ = reader.ReadInt32();
                        __v__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.AQICN.Time();
            if(__s__b__) ____result.s = __s__;
            if(__tz__b__) ____result.tz = __tz__;
            if(__v__b__) ____result.v = __v__;

            return ____result;
        }
    }


    public sealed class DebugFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.AQICN.Debug>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public DebugFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("sync"), 0},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("sync"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.AQICN.Debug value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Serialize(ref writer, value.sync, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.AQICN.Debug Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __sync__ = default(global::System.DateTime);
            var __sync__b__ = false;

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
                        __sync__ = formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Deserialize(ref reader, formatterResolver);
                        __sync__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.AQICN.Debug();
            if(__sync__b__) ____result.sync = __sync__;

            return ____result;
        }
    }


    public sealed class DataFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.AQICN.Data>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public DataFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("aqi"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("idx"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("attributions"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("city"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("dominentpol"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("iaqi"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("time"), 6},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("debug"), 7},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("aqi"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("idx"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("attributions"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("city"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("dominentpol"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("iaqi"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("time"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("debug"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.AQICN.Data value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteInt32(value.aqi);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteInt32(value.idx);
            writer.WriteRaw(this.____stringByteKeys[2]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.AQICN.Attribution[]>().Serialize(ref writer, value.attributions, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[3]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.AQICN.City>().Serialize(ref writer, value.city, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteString(value.dominentpol);
            writer.WriteRaw(this.____stringByteKeys[5]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.AQICN.Iaqi>().Serialize(ref writer, value.iaqi, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[6]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.AQICN.Time>().Serialize(ref writer, value.time, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[7]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.AQICN.Debug>().Serialize(ref writer, value.debug, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.AQICN.Data Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __aqi__ = default(int);
            var __aqi__b__ = false;
            var __idx__ = default(int);
            var __idx__b__ = false;
            var __attributions__ = default(global::SimpleWeather.AQICN.Attribution[]);
            var __attributions__b__ = false;
            var __city__ = default(global::SimpleWeather.AQICN.City);
            var __city__b__ = false;
            var __dominentpol__ = default(string);
            var __dominentpol__b__ = false;
            var __iaqi__ = default(global::SimpleWeather.AQICN.Iaqi);
            var __iaqi__b__ = false;
            var __time__ = default(global::SimpleWeather.AQICN.Time);
            var __time__b__ = false;
            var __debug__ = default(global::SimpleWeather.AQICN.Debug);
            var __debug__b__ = false;

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
                        __aqi__ = reader.ReadInt32();
                        __aqi__b__ = true;
                        break;
                    case 1:
                        __idx__ = reader.ReadInt32();
                        __idx__b__ = true;
                        break;
                    case 2:
                        __attributions__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.AQICN.Attribution[]>().Deserialize(ref reader, formatterResolver);
                        __attributions__b__ = true;
                        break;
                    case 3:
                        __city__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.AQICN.City>().Deserialize(ref reader, formatterResolver);
                        __city__b__ = true;
                        break;
                    case 4:
                        __dominentpol__ = reader.ReadString();
                        __dominentpol__b__ = true;
                        break;
                    case 5:
                        __iaqi__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.AQICN.Iaqi>().Deserialize(ref reader, formatterResolver);
                        __iaqi__b__ = true;
                        break;
                    case 6:
                        __time__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.AQICN.Time>().Deserialize(ref reader, formatterResolver);
                        __time__b__ = true;
                        break;
                    case 7:
                        __debug__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.AQICN.Debug>().Deserialize(ref reader, formatterResolver);
                        __debug__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.AQICN.Data();
            if(__aqi__b__) ____result.aqi = __aqi__;
            if(__idx__b__) ____result.idx = __idx__;
            if(__attributions__b__) ____result.attributions = __attributions__;
            if(__city__b__) ____result.city = __city__;
            if(__dominentpol__b__) ____result.dominentpol = __dominentpol__;
            if(__iaqi__b__) ____result.iaqi = __iaqi__;
            if(__time__b__) ____result.time = __time__;
            if(__debug__b__) ____result.debug = __debug__;

            return ____result;
        }
    }


    public sealed class RootobjectFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.AQICN.Rootobject>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public RootobjectFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("status"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("data"), 1},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("status"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("data"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.AQICN.Rootobject value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.status);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.AQICN.Data>().Serialize(ref writer, value.data, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.AQICN.Rootobject Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __status__ = default(string);
            var __status__b__ = false;
            var __data__ = default(global::SimpleWeather.AQICN.Data);
            var __data__b__ = false;

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
                        __status__ = reader.ReadString();
                        __status__b__ = true;
                        break;
                    case 1:
                        __data__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.AQICN.Data>().Deserialize(ref reader, formatterResolver);
                        __data__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.AQICN.Rootobject();
            if(__status__b__) ____result.status = __status__;
            if(__data__b__) ____result.data = __data__;

            return ____result;
        }
    }

}

#pragma warning disable 168
#pragma warning restore 219
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612
#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 219
#pragma warning disable 168

namespace SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.Bing
{
    using System;
    using Utf8Json;


    public sealed class AddressFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.Bing.Address>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public AddressFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("countryRegion"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("locality"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("adminDistrict"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("countryRegionIso2"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("formattedAddress"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("adminDistrict2"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("houseNumber"), 6},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("streetName"), 7},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("addressLine"), 8},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("neighborhood"), 9},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("postalCode"), 10},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("entityType"), 11},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("name"), 12},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("countryRegion"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("locality"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("adminDistrict"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("countryRegionIso2"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("formattedAddress"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("adminDistrict2"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("houseNumber"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("streetName"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("addressLine"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("neighborhood"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("postalCode"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("entityType"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("name"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.Bing.Address value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.countryRegion);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.locality);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.adminDistrict);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteString(value.countryRegionIso2);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteString(value.formattedAddress);
            writer.WriteRaw(this.____stringByteKeys[5]);
            writer.WriteString(value.adminDistrict2);
            writer.WriteRaw(this.____stringByteKeys[6]);
            writer.WriteString(value.houseNumber);
            writer.WriteRaw(this.____stringByteKeys[7]);
            writer.WriteString(value.streetName);
            writer.WriteRaw(this.____stringByteKeys[8]);
            writer.WriteString(value.addressLine);
            writer.WriteRaw(this.____stringByteKeys[9]);
            writer.WriteString(value.neighborhood);
            writer.WriteRaw(this.____stringByteKeys[10]);
            writer.WriteString(value.postalCode);
            writer.WriteRaw(this.____stringByteKeys[11]);
            writer.WriteString(value.entityType);
            writer.WriteRaw(this.____stringByteKeys[12]);
            writer.WriteString(value.name);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.Bing.Address Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __countryRegion__ = default(string);
            var __countryRegion__b__ = false;
            var __locality__ = default(string);
            var __locality__b__ = false;
            var __adminDistrict__ = default(string);
            var __adminDistrict__b__ = false;
            var __countryRegionIso2__ = default(string);
            var __countryRegionIso2__b__ = false;
            var __formattedAddress__ = default(string);
            var __formattedAddress__b__ = false;
            var __adminDistrict2__ = default(string);
            var __adminDistrict2__b__ = false;
            var __houseNumber__ = default(string);
            var __houseNumber__b__ = false;
            var __streetName__ = default(string);
            var __streetName__b__ = false;
            var __addressLine__ = default(string);
            var __addressLine__b__ = false;
            var __neighborhood__ = default(string);
            var __neighborhood__b__ = false;
            var __postalCode__ = default(string);
            var __postalCode__b__ = false;
            var __entityType__ = default(string);
            var __entityType__b__ = false;
            var __name__ = default(string);
            var __name__b__ = false;

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
                        __countryRegion__ = reader.ReadString();
                        __countryRegion__b__ = true;
                        break;
                    case 1:
                        __locality__ = reader.ReadString();
                        __locality__b__ = true;
                        break;
                    case 2:
                        __adminDistrict__ = reader.ReadString();
                        __adminDistrict__b__ = true;
                        break;
                    case 3:
                        __countryRegionIso2__ = reader.ReadString();
                        __countryRegionIso2__b__ = true;
                        break;
                    case 4:
                        __formattedAddress__ = reader.ReadString();
                        __formattedAddress__b__ = true;
                        break;
                    case 5:
                        __adminDistrict2__ = reader.ReadString();
                        __adminDistrict2__b__ = true;
                        break;
                    case 6:
                        __houseNumber__ = reader.ReadString();
                        __houseNumber__b__ = true;
                        break;
                    case 7:
                        __streetName__ = reader.ReadString();
                        __streetName__b__ = true;
                        break;
                    case 8:
                        __addressLine__ = reader.ReadString();
                        __addressLine__b__ = true;
                        break;
                    case 9:
                        __neighborhood__ = reader.ReadString();
                        __neighborhood__b__ = true;
                        break;
                    case 10:
                        __postalCode__ = reader.ReadString();
                        __postalCode__b__ = true;
                        break;
                    case 11:
                        __entityType__ = reader.ReadString();
                        __entityType__b__ = true;
                        break;
                    case 12:
                        __name__ = reader.ReadString();
                        __name__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.Bing.Address();
            if(__countryRegion__b__) ____result.countryRegion = __countryRegion__;
            if(__locality__b__) ____result.locality = __locality__;
            if(__adminDistrict__b__) ____result.adminDistrict = __adminDistrict__;
            if(__countryRegionIso2__b__) ____result.countryRegionIso2 = __countryRegionIso2__;
            if(__formattedAddress__b__) ____result.formattedAddress = __formattedAddress__;
            if(__adminDistrict2__b__) ____result.adminDistrict2 = __adminDistrict2__;
            if(__houseNumber__b__) ____result.houseNumber = __houseNumber__;
            if(__streetName__b__) ____result.streetName = __streetName__;
            if(__addressLine__b__) ____result.addressLine = __addressLine__;
            if(__neighborhood__b__) ____result.neighborhood = __neighborhood__;
            if(__postalCode__b__) ____result.postalCode = __postalCode__;
            if(__entityType__b__) ____result.entityType = __entityType__;
            if(__name__b__) ____result.name = __name__;

            return ____result;
        }
    }


    public sealed class ValueFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.Bing.Value>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public ValueFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("__type"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("address"), 1},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("__type"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("address"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.Bing.Value value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.__type);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Bing.Address>().Serialize(ref writer, value.address, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.Bing.Value Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var ____type__ = default(string);
            var ____type__b__ = false;
            var __address__ = default(global::SimpleWeather.Bing.Address);
            var __address__b__ = false;

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
                        ____type__ = reader.ReadString();
                        ____type__b__ = true;
                        break;
                    case 1:
                        __address__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Bing.Address>().Deserialize(ref reader, formatterResolver);
                        __address__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.Bing.Value();
            if(____type__b__) ____result.__type = ____type__;
            if(__address__b__) ____result.address = __address__;

            return ____result;
        }
    }


    public sealed class ResourceFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.Bing.Resource>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public ResourceFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("__type"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("value"), 1},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("__type"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("value"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.Bing.Resource value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.__type);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Bing.Value[]>().Serialize(ref writer, value.value, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.Bing.Resource Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var ____type__ = default(string);
            var ____type__b__ = false;
            var __value__ = default(global::SimpleWeather.Bing.Value[]);
            var __value__b__ = false;

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
                        ____type__ = reader.ReadString();
                        ____type__b__ = true;
                        break;
                    case 1:
                        __value__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Bing.Value[]>().Deserialize(ref reader, formatterResolver);
                        __value__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.Bing.Resource();
            if(____type__b__) ____result.__type = ____type__;
            if(__value__b__) ____result.value = __value__;

            return ____result;
        }
    }


    public sealed class ResourcesetFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.Bing.Resourceset>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public ResourcesetFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("estimatedTotal"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("resources"), 1},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("estimatedTotal"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("resources"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.Bing.Resourceset value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteInt32(value.estimatedTotal);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Bing.Resource[]>().Serialize(ref writer, value.resources, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.Bing.Resourceset Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __estimatedTotal__ = default(int);
            var __estimatedTotal__b__ = false;
            var __resources__ = default(global::SimpleWeather.Bing.Resource[]);
            var __resources__b__ = false;

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
                        __estimatedTotal__ = reader.ReadInt32();
                        __estimatedTotal__b__ = true;
                        break;
                    case 1:
                        __resources__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Bing.Resource[]>().Deserialize(ref reader, formatterResolver);
                        __resources__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.Bing.Resourceset();
            if(__estimatedTotal__b__) ____result.estimatedTotal = __estimatedTotal__;
            if(__resources__b__) ____result.resources = __resources__;

            return ____result;
        }
    }


    public sealed class AC_RootobjectFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.Bing.AC_Rootobject>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public AC_RootobjectFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("authenticationResultCode"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("brandLogoUri"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("copyright"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("resourceSets"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("statusCode"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("statusDescription"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("traceId"), 6},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("authenticationResultCode"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("brandLogoUri"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("copyright"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("resourceSets"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("statusCode"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("statusDescription"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("traceId"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.Bing.AC_Rootobject value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.authenticationResultCode);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.brandLogoUri);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.copyright);
            writer.WriteRaw(this.____stringByteKeys[3]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Bing.Resourceset[]>().Serialize(ref writer, value.resourceSets, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteInt32(value.statusCode);
            writer.WriteRaw(this.____stringByteKeys[5]);
            writer.WriteString(value.statusDescription);
            writer.WriteRaw(this.____stringByteKeys[6]);
            writer.WriteString(value.traceId);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.Bing.AC_Rootobject Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __authenticationResultCode__ = default(string);
            var __authenticationResultCode__b__ = false;
            var __brandLogoUri__ = default(string);
            var __brandLogoUri__b__ = false;
            var __copyright__ = default(string);
            var __copyright__b__ = false;
            var __resourceSets__ = default(global::SimpleWeather.Bing.Resourceset[]);
            var __resourceSets__b__ = false;
            var __statusCode__ = default(int);
            var __statusCode__b__ = false;
            var __statusDescription__ = default(string);
            var __statusDescription__b__ = false;
            var __traceId__ = default(string);
            var __traceId__b__ = false;

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
                        __authenticationResultCode__ = reader.ReadString();
                        __authenticationResultCode__b__ = true;
                        break;
                    case 1:
                        __brandLogoUri__ = reader.ReadString();
                        __brandLogoUri__b__ = true;
                        break;
                    case 2:
                        __copyright__ = reader.ReadString();
                        __copyright__b__ = true;
                        break;
                    case 3:
                        __resourceSets__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Bing.Resourceset[]>().Deserialize(ref reader, formatterResolver);
                        __resourceSets__b__ = true;
                        break;
                    case 4:
                        __statusCode__ = reader.ReadInt32();
                        __statusCode__b__ = true;
                        break;
                    case 5:
                        __statusDescription__ = reader.ReadString();
                        __statusDescription__b__ = true;
                        break;
                    case 6:
                        __traceId__ = reader.ReadString();
                        __traceId__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.Bing.AC_Rootobject();
            if(__authenticationResultCode__b__) ____result.authenticationResultCode = __authenticationResultCode__;
            if(__brandLogoUri__b__) ____result.brandLogoUri = __brandLogoUri__;
            if(__copyright__b__) ____result.copyright = __copyright__;
            if(__resourceSets__b__) ____result.resourceSets = __resourceSets__;
            if(__statusCode__b__) ____result.statusCode = __statusCode__;
            if(__statusDescription__b__) ____result.statusDescription = __statusDescription__;
            if(__traceId__b__) ____result.traceId = __traceId__;

            return ____result;
        }
    }

}

#pragma warning disable 168
#pragma warning restore 219
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612
#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 219
#pragma warning disable 168

namespace SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE
{
    using System;
    using Utf8Json;


    public sealed class TimesegmentFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.HERE.Timesegment>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public TimesegmentFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("value"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("segment"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("day_of_week"), 2},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("value"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("segment"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("day_of_week"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.HERE.Timesegment value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.value);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.segment);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.day_of_week);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.HERE.Timesegment Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __value__ = default(string);
            var __value__b__ = false;
            var __segment__ = default(string);
            var __segment__b__ = false;
            var __day_of_week__ = default(string);
            var __day_of_week__b__ = false;

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
                        __value__ = reader.ReadString();
                        __value__b__ = true;
                        break;
                    case 1:
                        __segment__ = reader.ReadString();
                        __segment__b__ = true;
                        break;
                    case 2:
                        __day_of_week__ = reader.ReadString();
                        __day_of_week__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.HERE.Timesegment();
            if(__value__b__) ____result.value = __value__;
            if(__segment__b__) ____result.segment = __segment__;
            if(__day_of_week__b__) ____result.day_of_week = __day_of_week__;

            return ____result;
        }
    }


    public sealed class AlertFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.HERE.Alert>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public AlertFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("timeSegment"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("type"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("description"), 2},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("timeSegment"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("type"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("description"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.HERE.Alert value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Timesegment[]>().Serialize(ref writer, value.timeSegment, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.type);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.description);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.HERE.Alert Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __timeSegment__ = default(global::SimpleWeather.HERE.Timesegment[]);
            var __timeSegment__b__ = false;
            var __type__ = default(string);
            var __type__b__ = false;
            var __description__ = default(string);
            var __description__b__ = false;

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
                        __timeSegment__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Timesegment[]>().Deserialize(ref reader, formatterResolver);
                        __timeSegment__b__ = true;
                        break;
                    case 1:
                        __type__ = reader.ReadString();
                        __type__b__ = true;
                        break;
                    case 2:
                        __description__ = reader.ReadString();
                        __description__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.HERE.Alert();
            if(__timeSegment__b__) ____result.timeSegment = __timeSegment__;
            if(__type__b__) ____result.type = __type__;
            if(__description__b__) ____result.description = __description__;

            return ____result;
        }
    }


    public sealed class AlertsFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.HERE.Alerts>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public AlertsFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("alerts"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("country"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("state"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("city"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("latitude"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("longitude"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("timezone"), 6},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("alerts"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("country"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("state"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("city"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("latitude"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("longitude"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("timezone"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.HERE.Alerts value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Alert[]>().Serialize(ref writer, value.alerts, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.country);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.state);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteString(value.city);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteSingle(value.latitude);
            writer.WriteRaw(this.____stringByteKeys[5]);
            writer.WriteSingle(value.longitude);
            writer.WriteRaw(this.____stringByteKeys[6]);
            writer.WriteInt32(value.timezone);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.HERE.Alerts Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __alerts__ = default(global::SimpleWeather.HERE.Alert[]);
            var __alerts__b__ = false;
            var __country__ = default(string);
            var __country__b__ = false;
            var __state__ = default(string);
            var __state__b__ = false;
            var __city__ = default(string);
            var __city__b__ = false;
            var __latitude__ = default(float);
            var __latitude__b__ = false;
            var __longitude__ = default(float);
            var __longitude__b__ = false;
            var __timezone__ = default(int);
            var __timezone__b__ = false;

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
                        __alerts__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Alert[]>().Deserialize(ref reader, formatterResolver);
                        __alerts__b__ = true;
                        break;
                    case 1:
                        __country__ = reader.ReadString();
                        __country__b__ = true;
                        break;
                    case 2:
                        __state__ = reader.ReadString();
                        __state__b__ = true;
                        break;
                    case 3:
                        __city__ = reader.ReadString();
                        __city__b__ = true;
                        break;
                    case 4:
                        __latitude__ = reader.ReadSingle();
                        __latitude__b__ = true;
                        break;
                    case 5:
                        __longitude__ = reader.ReadSingle();
                        __longitude__b__ = true;
                        break;
                    case 6:
                        __timezone__ = reader.ReadInt32();
                        __timezone__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.HERE.Alerts();
            if(__alerts__b__) ____result.alerts = __alerts__;
            if(__country__b__) ____result.country = __country__;
            if(__state__b__) ____result.state = __state__;
            if(__city__b__) ____result.city = __city__;
            if(__latitude__b__) ____result.latitude = __latitude__;
            if(__longitude__b__) ____result.longitude = __longitude__;
            if(__timezone__b__) ____result.timezone = __timezone__;

            return ____result;
        }
    }


    public sealed class AdditionaldataFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.HERE.Additionaldata>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public AdditionaldataFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("value"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("key"), 1},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("value"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("key"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.HERE.Additionaldata value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.value);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.key);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.HERE.Additionaldata Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __value__ = default(string);
            var __value__b__ = false;
            var __key__ = default(string);
            var __key__b__ = false;

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
                        __value__ = reader.ReadString();
                        __value__b__ = true;
                        break;
                    case 1:
                        __key__ = reader.ReadString();
                        __key__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.HERE.Additionaldata();
            if(__value__b__) ____result.value = __value__;
            if(__key__b__) ____result.key = __key__;

            return ____result;
        }
    }


    public sealed class AddressFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.HERE.Address>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public AddressFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("country"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("state"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("county"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("city"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("postalCode"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("district"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("street"), 6},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("label"), 7},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("houseNumber"), 8},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("additionalData"), 9},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("country"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("state"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("county"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("city"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("postalCode"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("district"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("street"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("label"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("houseNumber"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("additionalData"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.HERE.Address value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.country);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.state);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.county);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteString(value.city);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteString(value.postalCode);
            writer.WriteRaw(this.____stringByteKeys[5]);
            writer.WriteString(value.district);
            writer.WriteRaw(this.____stringByteKeys[6]);
            writer.WriteString(value.street);
            writer.WriteRaw(this.____stringByteKeys[7]);
            writer.WriteString(value.label);
            writer.WriteRaw(this.____stringByteKeys[8]);
            writer.WriteString(value.houseNumber);
            writer.WriteRaw(this.____stringByteKeys[9]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Additionaldata[]>().Serialize(ref writer, value.additionalData, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.HERE.Address Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __country__ = default(string);
            var __country__b__ = false;
            var __state__ = default(string);
            var __state__b__ = false;
            var __county__ = default(string);
            var __county__b__ = false;
            var __city__ = default(string);
            var __city__b__ = false;
            var __postalCode__ = default(string);
            var __postalCode__b__ = false;
            var __district__ = default(string);
            var __district__b__ = false;
            var __street__ = default(string);
            var __street__b__ = false;
            var __label__ = default(string);
            var __label__b__ = false;
            var __houseNumber__ = default(string);
            var __houseNumber__b__ = false;
            var __additionalData__ = default(global::SimpleWeather.HERE.Additionaldata[]);
            var __additionalData__b__ = false;

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
                        __country__ = reader.ReadString();
                        __country__b__ = true;
                        break;
                    case 1:
                        __state__ = reader.ReadString();
                        __state__b__ = true;
                        break;
                    case 2:
                        __county__ = reader.ReadString();
                        __county__b__ = true;
                        break;
                    case 3:
                        __city__ = reader.ReadString();
                        __city__b__ = true;
                        break;
                    case 4:
                        __postalCode__ = reader.ReadString();
                        __postalCode__b__ = true;
                        break;
                    case 5:
                        __district__ = reader.ReadString();
                        __district__b__ = true;
                        break;
                    case 6:
                        __street__ = reader.ReadString();
                        __street__b__ = true;
                        break;
                    case 7:
                        __label__ = reader.ReadString();
                        __label__b__ = true;
                        break;
                    case 8:
                        __houseNumber__ = reader.ReadString();
                        __houseNumber__b__ = true;
                        break;
                    case 9:
                        __additionalData__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Additionaldata[]>().Deserialize(ref reader, formatterResolver);
                        __additionalData__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.HERE.Address();
            if(__country__b__) ____result.country = __country__;
            if(__state__b__) ____result.state = __state__;
            if(__county__b__) ____result.county = __county__;
            if(__city__b__) ____result.city = __city__;
            if(__postalCode__b__) ____result.postalCode = __postalCode__;
            if(__district__b__) ____result.district = __district__;
            if(__street__b__) ____result.street = __street__;
            if(__label__b__) ____result.label = __label__;
            if(__houseNumber__b__) ____result.houseNumber = __houseNumber__;
            if(__additionalData__b__) ____result.additionalData = __additionalData__;

            return ____result;
        }
    }


    public sealed class SuggestionFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.HERE.Suggestion>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public SuggestionFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("label"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("language"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("countryCode"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("locationId"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("address"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("matchLevel"), 5},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("label"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("language"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("countryCode"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("locationId"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("address"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("matchLevel"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.HERE.Suggestion value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.label);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.language);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.countryCode);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteString(value.locationId);
            writer.WriteRaw(this.____stringByteKeys[4]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Address>().Serialize(ref writer, value.address, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[5]);
            writer.WriteString(value.matchLevel);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.HERE.Suggestion Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __label__ = default(string);
            var __label__b__ = false;
            var __language__ = default(string);
            var __language__b__ = false;
            var __countryCode__ = default(string);
            var __countryCode__b__ = false;
            var __locationId__ = default(string);
            var __locationId__b__ = false;
            var __address__ = default(global::SimpleWeather.HERE.Address);
            var __address__b__ = false;
            var __matchLevel__ = default(string);
            var __matchLevel__b__ = false;

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
                        __label__ = reader.ReadString();
                        __label__b__ = true;
                        break;
                    case 1:
                        __language__ = reader.ReadString();
                        __language__b__ = true;
                        break;
                    case 2:
                        __countryCode__ = reader.ReadString();
                        __countryCode__b__ = true;
                        break;
                    case 3:
                        __locationId__ = reader.ReadString();
                        __locationId__b__ = true;
                        break;
                    case 4:
                        __address__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Address>().Deserialize(ref reader, formatterResolver);
                        __address__b__ = true;
                        break;
                    case 5:
                        __matchLevel__ = reader.ReadString();
                        __matchLevel__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.HERE.Suggestion();
            if(__label__b__) ____result.label = __label__;
            if(__language__b__) ____result.language = __language__;
            if(__countryCode__b__) ____result.countryCode = __countryCode__;
            if(__locationId__b__) ____result.locationId = __locationId__;
            if(__address__b__) ____result.address = __address__;
            if(__matchLevel__b__) ____result.matchLevel = __matchLevel__;

            return ____result;
        }
    }


    public sealed class AC_RootobjectFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.HERE.AC_Rootobject>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public AC_RootobjectFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("suggestions"), 0},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("suggestions"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.HERE.AC_Rootobject value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Suggestion[]>().Serialize(ref writer, value.suggestions, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.HERE.AC_Rootobject Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __suggestions__ = default(global::SimpleWeather.HERE.Suggestion[]);
            var __suggestions__b__ = false;

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
                        __suggestions__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Suggestion[]>().Deserialize(ref reader, formatterResolver);
                        __suggestions__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.HERE.AC_Rootobject();
            if(__suggestions__b__) ____result.suggestions = __suggestions__;

            return ____result;
        }
    }


    public sealed class MetainfoFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.HERE.Metainfo>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public MetainfoFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("timestamp"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("nextPageInformation"), 1},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("timestamp"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("nextPageInformation"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.HERE.Metainfo value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.timestamp);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.nextPageInformation);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.HERE.Metainfo Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __timestamp__ = default(string);
            var __timestamp__b__ = false;
            var __nextPageInformation__ = default(string);
            var __nextPageInformation__b__ = false;

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
                        __timestamp__ = reader.ReadString();
                        __timestamp__b__ = true;
                        break;
                    case 1:
                        __nextPageInformation__ = reader.ReadString();
                        __nextPageInformation__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.HERE.Metainfo();
            if(__timestamp__b__) ____result.timestamp = __timestamp__;
            if(__nextPageInformation__b__) ____result.nextPageInformation = __nextPageInformation__;

            return ____result;
        }
    }


    public sealed class MatchqualityFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.HERE.Matchquality>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public MatchqualityFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("country"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("state"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("county"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("city"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("district"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("street"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("houseNumber"), 6},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("postalCode"), 7},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("country"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("state"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("county"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("city"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("district"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("street"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("houseNumber"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("postalCode"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.HERE.Matchquality value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteSingle(value.country);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteSingle(value.state);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteSingle(value.county);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteSingle(value.city);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteSingle(value.district);
            writer.WriteRaw(this.____stringByteKeys[5]);
            formatterResolver.GetFormatterWithVerify<float[]>().Serialize(ref writer, value.street, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[6]);
            writer.WriteSingle(value.houseNumber);
            writer.WriteRaw(this.____stringByteKeys[7]);
            writer.WriteSingle(value.postalCode);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.HERE.Matchquality Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __country__ = default(float);
            var __country__b__ = false;
            var __state__ = default(float);
            var __state__b__ = false;
            var __county__ = default(float);
            var __county__b__ = false;
            var __city__ = default(float);
            var __city__b__ = false;
            var __district__ = default(float);
            var __district__b__ = false;
            var __street__ = default(float[]);
            var __street__b__ = false;
            var __houseNumber__ = default(float);
            var __houseNumber__b__ = false;
            var __postalCode__ = default(float);
            var __postalCode__b__ = false;

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
                        __country__ = reader.ReadSingle();
                        __country__b__ = true;
                        break;
                    case 1:
                        __state__ = reader.ReadSingle();
                        __state__b__ = true;
                        break;
                    case 2:
                        __county__ = reader.ReadSingle();
                        __county__b__ = true;
                        break;
                    case 3:
                        __city__ = reader.ReadSingle();
                        __city__b__ = true;
                        break;
                    case 4:
                        __district__ = reader.ReadSingle();
                        __district__b__ = true;
                        break;
                    case 5:
                        __street__ = formatterResolver.GetFormatterWithVerify<float[]>().Deserialize(ref reader, formatterResolver);
                        __street__b__ = true;
                        break;
                    case 6:
                        __houseNumber__ = reader.ReadSingle();
                        __houseNumber__b__ = true;
                        break;
                    case 7:
                        __postalCode__ = reader.ReadSingle();
                        __postalCode__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.HERE.Matchquality();
            if(__country__b__) ____result.country = __country__;
            if(__state__b__) ____result.state = __state__;
            if(__county__b__) ____result.county = __county__;
            if(__city__b__) ____result.city = __city__;
            if(__district__b__) ____result.district = __district__;
            if(__street__b__) ____result.street = __street__;
            if(__houseNumber__b__) ____result.houseNumber = __houseNumber__;
            if(__postalCode__b__) ____result.postalCode = __postalCode__;

            return ____result;
        }
    }


    public sealed class DisplaypositionFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.HERE.Displayposition>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public DisplaypositionFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("latitude"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("longitude"), 1},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("latitude"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("longitude"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.HERE.Displayposition value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteSingle(value.latitude);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteSingle(value.longitude);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.HERE.Displayposition Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __latitude__ = default(float);
            var __latitude__b__ = false;
            var __longitude__ = default(float);
            var __longitude__b__ = false;

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
                        __latitude__ = reader.ReadSingle();
                        __latitude__b__ = true;
                        break;
                    case 1:
                        __longitude__ = reader.ReadSingle();
                        __longitude__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.HERE.Displayposition();
            if(__latitude__b__) ____result.latitude = __latitude__;
            if(__longitude__b__) ____result.longitude = __longitude__;

            return ____result;
        }
    }


    public sealed class NavigationpositionFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.HERE.Navigationposition>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public NavigationpositionFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("latitude"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("longitude"), 1},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("latitude"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("longitude"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.HERE.Navigationposition value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteSingle(value.latitude);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteSingle(value.longitude);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.HERE.Navigationposition Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __latitude__ = default(float);
            var __latitude__b__ = false;
            var __longitude__ = default(float);
            var __longitude__b__ = false;

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
                        __latitude__ = reader.ReadSingle();
                        __latitude__b__ = true;
                        break;
                    case 1:
                        __longitude__ = reader.ReadSingle();
                        __longitude__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.HERE.Navigationposition();
            if(__latitude__b__) ____result.latitude = __latitude__;
            if(__longitude__b__) ____result.longitude = __longitude__;

            return ____result;
        }
    }


    public sealed class TimezoneFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.HERE.Timezone>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public TimezoneFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("offset"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("rawOffset"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("nameShort"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("nameLong"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("nameDstShort"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("nameDstLong"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("inDaylightTime"), 6},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("dstSavings"), 7},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("id"), 8},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("offset"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("rawOffset"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("nameShort"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("nameLong"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("nameDstShort"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("nameDstLong"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("inDaylightTime"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("dstSavings"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("id"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.HERE.Timezone value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteInt32(value.offset);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteInt32(value.rawOffset);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.nameShort);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteString(value.nameLong);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteString(value.nameDstShort);
            writer.WriteRaw(this.____stringByteKeys[5]);
            writer.WriteString(value.nameDstLong);
            writer.WriteRaw(this.____stringByteKeys[6]);
            writer.WriteBoolean(value.inDaylightTime);
            writer.WriteRaw(this.____stringByteKeys[7]);
            writer.WriteInt32(value.dstSavings);
            writer.WriteRaw(this.____stringByteKeys[8]);
            writer.WriteString(value.id);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.HERE.Timezone Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __offset__ = default(int);
            var __offset__b__ = false;
            var __rawOffset__ = default(int);
            var __rawOffset__b__ = false;
            var __nameShort__ = default(string);
            var __nameShort__b__ = false;
            var __nameLong__ = default(string);
            var __nameLong__b__ = false;
            var __nameDstShort__ = default(string);
            var __nameDstShort__b__ = false;
            var __nameDstLong__ = default(string);
            var __nameDstLong__b__ = false;
            var __inDaylightTime__ = default(bool);
            var __inDaylightTime__b__ = false;
            var __dstSavings__ = default(int);
            var __dstSavings__b__ = false;
            var __id__ = default(string);
            var __id__b__ = false;

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
                        __offset__ = reader.ReadInt32();
                        __offset__b__ = true;
                        break;
                    case 1:
                        __rawOffset__ = reader.ReadInt32();
                        __rawOffset__b__ = true;
                        break;
                    case 2:
                        __nameShort__ = reader.ReadString();
                        __nameShort__b__ = true;
                        break;
                    case 3:
                        __nameLong__ = reader.ReadString();
                        __nameLong__b__ = true;
                        break;
                    case 4:
                        __nameDstShort__ = reader.ReadString();
                        __nameDstShort__b__ = true;
                        break;
                    case 5:
                        __nameDstLong__ = reader.ReadString();
                        __nameDstLong__b__ = true;
                        break;
                    case 6:
                        __inDaylightTime__ = reader.ReadBoolean();
                        __inDaylightTime__b__ = true;
                        break;
                    case 7:
                        __dstSavings__ = reader.ReadInt32();
                        __dstSavings__b__ = true;
                        break;
                    case 8:
                        __id__ = reader.ReadString();
                        __id__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.HERE.Timezone();
            if(__offset__b__) ____result.offset = __offset__;
            if(__rawOffset__b__) ____result.rawOffset = __rawOffset__;
            if(__nameShort__b__) ____result.nameShort = __nameShort__;
            if(__nameLong__b__) ____result.nameLong = __nameLong__;
            if(__nameDstShort__b__) ____result.nameDstShort = __nameDstShort__;
            if(__nameDstLong__b__) ____result.nameDstLong = __nameDstLong__;
            if(__inDaylightTime__b__) ____result.inDaylightTime = __inDaylightTime__;
            if(__dstSavings__b__) ____result.dstSavings = __dstSavings__;
            if(__id__b__) ____result.id = __id__;

            return ____result;
        }
    }


    public sealed class AdmininfoFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.HERE.Admininfo>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public AdmininfoFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("localTime"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("currency"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("drivingSide"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("systemOfMeasure"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("timeZone"), 4},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("localTime"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("currency"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("drivingSide"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("systemOfMeasure"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("timeZone"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.HERE.Admininfo value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.localTime);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.currency);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.drivingSide);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteString(value.systemOfMeasure);
            writer.WriteRaw(this.____stringByteKeys[4]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Timezone>().Serialize(ref writer, value.timeZone, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.HERE.Admininfo Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __localTime__ = default(string);
            var __localTime__b__ = false;
            var __currency__ = default(string);
            var __currency__b__ = false;
            var __drivingSide__ = default(string);
            var __drivingSide__b__ = false;
            var __systemOfMeasure__ = default(string);
            var __systemOfMeasure__b__ = false;
            var __timeZone__ = default(global::SimpleWeather.HERE.Timezone);
            var __timeZone__b__ = false;

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
                        __localTime__ = reader.ReadString();
                        __localTime__b__ = true;
                        break;
                    case 1:
                        __currency__ = reader.ReadString();
                        __currency__b__ = true;
                        break;
                    case 2:
                        __drivingSide__ = reader.ReadString();
                        __drivingSide__b__ = true;
                        break;
                    case 3:
                        __systemOfMeasure__ = reader.ReadString();
                        __systemOfMeasure__b__ = true;
                        break;
                    case 4:
                        __timeZone__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Timezone>().Deserialize(ref reader, formatterResolver);
                        __timeZone__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.HERE.Admininfo();
            if(__localTime__b__) ____result.localTime = __localTime__;
            if(__currency__b__) ____result.currency = __currency__;
            if(__drivingSide__b__) ____result.drivingSide = __drivingSide__;
            if(__systemOfMeasure__b__) ____result.systemOfMeasure = __systemOfMeasure__;
            if(__timeZone__b__) ____result.timeZone = __timeZone__;

            return ____result;
        }
    }


    public sealed class GeoLocationFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.HERE.GeoLocation>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public GeoLocationFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("locationId"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("locationType"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("displayPosition"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("navigationPosition"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("address"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("adminInfo"), 5},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("locationId"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("locationType"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("displayPosition"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("navigationPosition"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("address"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("adminInfo"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.HERE.GeoLocation value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.locationId);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.locationType);
            writer.WriteRaw(this.____stringByteKeys[2]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Displayposition>().Serialize(ref writer, value.displayPosition, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[3]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Navigationposition[]>().Serialize(ref writer, value.navigationPosition, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[4]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Address>().Serialize(ref writer, value.address, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[5]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Admininfo>().Serialize(ref writer, value.adminInfo, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.HERE.GeoLocation Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __locationId__ = default(string);
            var __locationId__b__ = false;
            var __locationType__ = default(string);
            var __locationType__b__ = false;
            var __displayPosition__ = default(global::SimpleWeather.HERE.Displayposition);
            var __displayPosition__b__ = false;
            var __navigationPosition__ = default(global::SimpleWeather.HERE.Navigationposition[]);
            var __navigationPosition__b__ = false;
            var __address__ = default(global::SimpleWeather.HERE.Address);
            var __address__b__ = false;
            var __adminInfo__ = default(global::SimpleWeather.HERE.Admininfo);
            var __adminInfo__b__ = false;

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
                        __locationId__ = reader.ReadString();
                        __locationId__b__ = true;
                        break;
                    case 1:
                        __locationType__ = reader.ReadString();
                        __locationType__b__ = true;
                        break;
                    case 2:
                        __displayPosition__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Displayposition>().Deserialize(ref reader, formatterResolver);
                        __displayPosition__b__ = true;
                        break;
                    case 3:
                        __navigationPosition__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Navigationposition[]>().Deserialize(ref reader, formatterResolver);
                        __navigationPosition__b__ = true;
                        break;
                    case 4:
                        __address__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Address>().Deserialize(ref reader, formatterResolver);
                        __address__b__ = true;
                        break;
                    case 5:
                        __adminInfo__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Admininfo>().Deserialize(ref reader, formatterResolver);
                        __adminInfo__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.HERE.GeoLocation();
            if(__locationId__b__) ____result.locationId = __locationId__;
            if(__locationType__b__) ____result.locationType = __locationType__;
            if(__displayPosition__b__) ____result.displayPosition = __displayPosition__;
            if(__navigationPosition__b__) ____result.navigationPosition = __navigationPosition__;
            if(__address__b__) ____result.address = __address__;
            if(__adminInfo__b__) ____result.adminInfo = __adminInfo__;

            return ____result;
        }
    }


    public sealed class ResultFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.HERE.Result>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public ResultFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("relevance"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("distance"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("matchLevel"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("matchQuality"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("matchType"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("location"), 5},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("relevance"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("distance"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("matchLevel"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("matchQuality"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("matchType"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("location"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.HERE.Result value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteSingle(value.relevance);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteSingle(value.distance);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.matchLevel);
            writer.WriteRaw(this.____stringByteKeys[3]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Matchquality>().Serialize(ref writer, value.matchQuality, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteString(value.matchType);
            writer.WriteRaw(this.____stringByteKeys[5]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.GeoLocation>().Serialize(ref writer, value.location, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.HERE.Result Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __relevance__ = default(float);
            var __relevance__b__ = false;
            var __distance__ = default(float);
            var __distance__b__ = false;
            var __matchLevel__ = default(string);
            var __matchLevel__b__ = false;
            var __matchQuality__ = default(global::SimpleWeather.HERE.Matchquality);
            var __matchQuality__b__ = false;
            var __matchType__ = default(string);
            var __matchType__b__ = false;
            var __location__ = default(global::SimpleWeather.HERE.GeoLocation);
            var __location__b__ = false;

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
                        __relevance__ = reader.ReadSingle();
                        __relevance__b__ = true;
                        break;
                    case 1:
                        __distance__ = reader.ReadSingle();
                        __distance__b__ = true;
                        break;
                    case 2:
                        __matchLevel__ = reader.ReadString();
                        __matchLevel__b__ = true;
                        break;
                    case 3:
                        __matchQuality__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Matchquality>().Deserialize(ref reader, formatterResolver);
                        __matchQuality__b__ = true;
                        break;
                    case 4:
                        __matchType__ = reader.ReadString();
                        __matchType__b__ = true;
                        break;
                    case 5:
                        __location__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.GeoLocation>().Deserialize(ref reader, formatterResolver);
                        __location__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.HERE.Result();
            if(__relevance__b__) ____result.relevance = __relevance__;
            if(__distance__b__) ____result.distance = __distance__;
            if(__matchLevel__b__) ____result.matchLevel = __matchLevel__;
            if(__matchQuality__b__) ____result.matchQuality = __matchQuality__;
            if(__matchType__b__) ____result.matchType = __matchType__;
            if(__location__b__) ____result.location = __location__;

            return ____result;
        }
    }


    public sealed class ViewFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.HERE.View>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public ViewFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("result"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("viewId"), 1},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("result"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("viewId"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.HERE.View value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Result[]>().Serialize(ref writer, value.result, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteInt32(value.viewId);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.HERE.View Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __result__ = default(global::SimpleWeather.HERE.Result[]);
            var __result__b__ = false;
            var __viewId__ = default(int);
            var __viewId__b__ = false;

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
                        __result__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Result[]>().Deserialize(ref reader, formatterResolver);
                        __result__b__ = true;
                        break;
                    case 1:
                        __viewId__ = reader.ReadInt32();
                        __viewId__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.HERE.View();
            if(__result__b__) ____result.result = __result__;
            if(__viewId__b__) ____result.viewId = __viewId__;

            return ____result;
        }
    }


    public sealed class ResponseFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.HERE.Response>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public ResponseFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("metaInfo"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("view"), 1},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("metaInfo"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("view"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.HERE.Response value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Metainfo>().Serialize(ref writer, value.metaInfo, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.View[]>().Serialize(ref writer, value.view, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.HERE.Response Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __metaInfo__ = default(global::SimpleWeather.HERE.Metainfo);
            var __metaInfo__b__ = false;
            var __view__ = default(global::SimpleWeather.HERE.View[]);
            var __view__b__ = false;

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
                        __metaInfo__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Metainfo>().Deserialize(ref reader, formatterResolver);
                        __metaInfo__b__ = true;
                        break;
                    case 1:
                        __view__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.View[]>().Deserialize(ref reader, formatterResolver);
                        __view__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.HERE.Response();
            if(__metaInfo__b__) ____result.metaInfo = __metaInfo__;
            if(__view__b__) ____result.view = __view__;

            return ____result;
        }
    }


    public sealed class Geo_RootobjectFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.HERE.Geo_Rootobject>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public Geo_RootobjectFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("response"), 0},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("response"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.HERE.Geo_Rootobject value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Response>().Serialize(ref writer, value.response, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.HERE.Geo_Rootobject Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __response__ = default(global::SimpleWeather.HERE.Response);
            var __response__b__ = false;

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
                        __response__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Response>().Deserialize(ref reader, formatterResolver);
                        __response__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.HERE.Geo_Rootobject();
            if(__response__b__) ____result.response = __response__;

            return ____result;
        }
    }


    public sealed class TokenRootobjectFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.HERE.TokenRootobject>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public TokenRootobjectFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("access_token"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("token_type"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("expires_in"), 2},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("access_token"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("token_type"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("expires_in"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.HERE.TokenRootobject value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.access_token);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.token_type);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteInt32(value.expires_in);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.HERE.TokenRootobject Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __access_token__ = default(string);
            var __access_token__b__ = false;
            var __token_type__ = default(string);
            var __token_type__b__ = false;
            var __expires_in__ = default(int);
            var __expires_in__b__ = false;

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
                        __access_token__ = reader.ReadString();
                        __access_token__b__ = true;
                        break;
                    case 1:
                        __token_type__ = reader.ReadString();
                        __token_type__b__ = true;
                        break;
                    case 2:
                        __expires_in__ = reader.ReadInt32();
                        __expires_in__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.HERE.TokenRootobject();
            if(__access_token__b__) ____result.access_token = __access_token__;
            if(__token_type__b__) ____result.token_type = __token_type__;
            if(__expires_in__b__) ____result.expires_in = __expires_in__;

            return ____result;
        }
    }


    internal sealed class TokenFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.HERE.Token>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public TokenFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("access_token"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("expiration_date"), 1},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("access_token"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("expiration_date"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.HERE.Token value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.access_token);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Serialize(ref writer, value.expiration_date, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.HERE.Token Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __access_token__ = default(string);
            var __access_token__b__ = false;
            var __expiration_date__ = default(global::System.DateTime);
            var __expiration_date__b__ = false;

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
                        __access_token__ = reader.ReadString();
                        __access_token__b__ = true;
                        break;
                    case 1:
                        __expiration_date__ = formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Deserialize(ref reader, formatterResolver);
                        __expiration_date__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.HERE.Token();
            if(__access_token__b__) ____result.access_token = __access_token__;
            if(__expiration_date__b__) ____result.expiration_date = __expiration_date__;

            return ____result;
        }
    }


    public sealed class ObservationFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.HERE.Observation>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public ObservationFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("daylight"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("description"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("skyInfo"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("skyDescription"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("temperature"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("temperatureDesc"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("comfort"), 6},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("highTemperature"), 7},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("lowTemperature"), 8},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("humidity"), 9},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("dewPoint"), 10},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("precipitation1H"), 11},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("precipitation3H"), 12},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("precipitation6H"), 13},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("precipitation12H"), 14},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("precipitation24H"), 15},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("precipitationDesc"), 16},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("airInfo"), 17},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("airDescription"), 18},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("windSpeed"), 19},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("windDirection"), 20},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("windDesc"), 21},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("windDescShort"), 22},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("barometerPressure"), 23},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("barometerTrend"), 24},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("visibility"), 25},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("snowCover"), 26},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("icon"), 27},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("iconName"), 28},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("iconLink"), 29},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("ageMinutes"), 30},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("activeAlerts"), 31},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("country"), 32},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("state"), 33},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("city"), 34},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("latitude"), 35},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("longitude"), 36},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("distance"), 37},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("elevation"), 38},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("utcTime"), 39},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("daylight"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("description"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("skyInfo"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("skyDescription"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("temperature"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("temperatureDesc"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("comfort"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("highTemperature"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("lowTemperature"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("humidity"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("dewPoint"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("precipitation1H"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("precipitation3H"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("precipitation6H"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("precipitation12H"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("precipitation24H"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("precipitationDesc"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("airInfo"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("airDescription"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("windSpeed"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("windDirection"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("windDesc"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("windDescShort"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("barometerPressure"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("barometerTrend"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("visibility"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("snowCover"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("icon"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("iconName"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("iconLink"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("ageMinutes"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("activeAlerts"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("country"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("state"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("city"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("latitude"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("longitude"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("distance"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("elevation"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("utcTime"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.HERE.Observation value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.daylight);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.description);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.skyInfo);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteString(value.skyDescription);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteString(value.temperature);
            writer.WriteRaw(this.____stringByteKeys[5]);
            writer.WriteString(value.temperatureDesc);
            writer.WriteRaw(this.____stringByteKeys[6]);
            writer.WriteString(value.comfort);
            writer.WriteRaw(this.____stringByteKeys[7]);
            writer.WriteString(value.highTemperature);
            writer.WriteRaw(this.____stringByteKeys[8]);
            writer.WriteString(value.lowTemperature);
            writer.WriteRaw(this.____stringByteKeys[9]);
            writer.WriteString(value.humidity);
            writer.WriteRaw(this.____stringByteKeys[10]);
            writer.WriteString(value.dewPoint);
            writer.WriteRaw(this.____stringByteKeys[11]);
            writer.WriteString(value.precipitation1H);
            writer.WriteRaw(this.____stringByteKeys[12]);
            writer.WriteString(value.precipitation3H);
            writer.WriteRaw(this.____stringByteKeys[13]);
            writer.WriteString(value.precipitation6H);
            writer.WriteRaw(this.____stringByteKeys[14]);
            writer.WriteString(value.precipitation12H);
            writer.WriteRaw(this.____stringByteKeys[15]);
            writer.WriteString(value.precipitation24H);
            writer.WriteRaw(this.____stringByteKeys[16]);
            writer.WriteString(value.precipitationDesc);
            writer.WriteRaw(this.____stringByteKeys[17]);
            writer.WriteString(value.airInfo);
            writer.WriteRaw(this.____stringByteKeys[18]);
            writer.WriteString(value.airDescription);
            writer.WriteRaw(this.____stringByteKeys[19]);
            writer.WriteString(value.windSpeed);
            writer.WriteRaw(this.____stringByteKeys[20]);
            writer.WriteString(value.windDirection);
            writer.WriteRaw(this.____stringByteKeys[21]);
            writer.WriteString(value.windDesc);
            writer.WriteRaw(this.____stringByteKeys[22]);
            writer.WriteString(value.windDescShort);
            writer.WriteRaw(this.____stringByteKeys[23]);
            writer.WriteString(value.barometerPressure);
            writer.WriteRaw(this.____stringByteKeys[24]);
            writer.WriteString(value.barometerTrend);
            writer.WriteRaw(this.____stringByteKeys[25]);
            writer.WriteString(value.visibility);
            writer.WriteRaw(this.____stringByteKeys[26]);
            writer.WriteString(value.snowCover);
            writer.WriteRaw(this.____stringByteKeys[27]);
            writer.WriteString(value.icon);
            writer.WriteRaw(this.____stringByteKeys[28]);
            writer.WriteString(value.iconName);
            writer.WriteRaw(this.____stringByteKeys[29]);
            writer.WriteString(value.iconLink);
            writer.WriteRaw(this.____stringByteKeys[30]);
            writer.WriteString(value.ageMinutes);
            writer.WriteRaw(this.____stringByteKeys[31]);
            writer.WriteString(value.activeAlerts);
            writer.WriteRaw(this.____stringByteKeys[32]);
            writer.WriteString(value.country);
            writer.WriteRaw(this.____stringByteKeys[33]);
            writer.WriteString(value.state);
            writer.WriteRaw(this.____stringByteKeys[34]);
            writer.WriteString(value.city);
            writer.WriteRaw(this.____stringByteKeys[35]);
            writer.WriteSingle(value.latitude);
            writer.WriteRaw(this.____stringByteKeys[36]);
            writer.WriteSingle(value.longitude);
            writer.WriteRaw(this.____stringByteKeys[37]);
            writer.WriteSingle(value.distance);
            writer.WriteRaw(this.____stringByteKeys[38]);
            writer.WriteSingle(value.elevation);
            writer.WriteRaw(this.____stringByteKeys[39]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset>().Serialize(ref writer, value.utcTime, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.HERE.Observation Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __daylight__ = default(string);
            var __daylight__b__ = false;
            var __description__ = default(string);
            var __description__b__ = false;
            var __skyInfo__ = default(string);
            var __skyInfo__b__ = false;
            var __skyDescription__ = default(string);
            var __skyDescription__b__ = false;
            var __temperature__ = default(string);
            var __temperature__b__ = false;
            var __temperatureDesc__ = default(string);
            var __temperatureDesc__b__ = false;
            var __comfort__ = default(string);
            var __comfort__b__ = false;
            var __highTemperature__ = default(string);
            var __highTemperature__b__ = false;
            var __lowTemperature__ = default(string);
            var __lowTemperature__b__ = false;
            var __humidity__ = default(string);
            var __humidity__b__ = false;
            var __dewPoint__ = default(string);
            var __dewPoint__b__ = false;
            var __precipitation1H__ = default(string);
            var __precipitation1H__b__ = false;
            var __precipitation3H__ = default(string);
            var __precipitation3H__b__ = false;
            var __precipitation6H__ = default(string);
            var __precipitation6H__b__ = false;
            var __precipitation12H__ = default(string);
            var __precipitation12H__b__ = false;
            var __precipitation24H__ = default(string);
            var __precipitation24H__b__ = false;
            var __precipitationDesc__ = default(string);
            var __precipitationDesc__b__ = false;
            var __airInfo__ = default(string);
            var __airInfo__b__ = false;
            var __airDescription__ = default(string);
            var __airDescription__b__ = false;
            var __windSpeed__ = default(string);
            var __windSpeed__b__ = false;
            var __windDirection__ = default(string);
            var __windDirection__b__ = false;
            var __windDesc__ = default(string);
            var __windDesc__b__ = false;
            var __windDescShort__ = default(string);
            var __windDescShort__b__ = false;
            var __barometerPressure__ = default(string);
            var __barometerPressure__b__ = false;
            var __barometerTrend__ = default(string);
            var __barometerTrend__b__ = false;
            var __visibility__ = default(string);
            var __visibility__b__ = false;
            var __snowCover__ = default(string);
            var __snowCover__b__ = false;
            var __icon__ = default(string);
            var __icon__b__ = false;
            var __iconName__ = default(string);
            var __iconName__b__ = false;
            var __iconLink__ = default(string);
            var __iconLink__b__ = false;
            var __ageMinutes__ = default(string);
            var __ageMinutes__b__ = false;
            var __activeAlerts__ = default(string);
            var __activeAlerts__b__ = false;
            var __country__ = default(string);
            var __country__b__ = false;
            var __state__ = default(string);
            var __state__b__ = false;
            var __city__ = default(string);
            var __city__b__ = false;
            var __latitude__ = default(float);
            var __latitude__b__ = false;
            var __longitude__ = default(float);
            var __longitude__b__ = false;
            var __distance__ = default(float);
            var __distance__b__ = false;
            var __elevation__ = default(float);
            var __elevation__b__ = false;
            var __utcTime__ = default(global::System.DateTimeOffset);
            var __utcTime__b__ = false;

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
                        __daylight__ = reader.ReadString();
                        __daylight__b__ = true;
                        break;
                    case 1:
                        __description__ = reader.ReadString();
                        __description__b__ = true;
                        break;
                    case 2:
                        __skyInfo__ = reader.ReadString();
                        __skyInfo__b__ = true;
                        break;
                    case 3:
                        __skyDescription__ = reader.ReadString();
                        __skyDescription__b__ = true;
                        break;
                    case 4:
                        __temperature__ = reader.ReadString();
                        __temperature__b__ = true;
                        break;
                    case 5:
                        __temperatureDesc__ = reader.ReadString();
                        __temperatureDesc__b__ = true;
                        break;
                    case 6:
                        __comfort__ = reader.ReadString();
                        __comfort__b__ = true;
                        break;
                    case 7:
                        __highTemperature__ = reader.ReadString();
                        __highTemperature__b__ = true;
                        break;
                    case 8:
                        __lowTemperature__ = reader.ReadString();
                        __lowTemperature__b__ = true;
                        break;
                    case 9:
                        __humidity__ = reader.ReadString();
                        __humidity__b__ = true;
                        break;
                    case 10:
                        __dewPoint__ = reader.ReadString();
                        __dewPoint__b__ = true;
                        break;
                    case 11:
                        __precipitation1H__ = reader.ReadString();
                        __precipitation1H__b__ = true;
                        break;
                    case 12:
                        __precipitation3H__ = reader.ReadString();
                        __precipitation3H__b__ = true;
                        break;
                    case 13:
                        __precipitation6H__ = reader.ReadString();
                        __precipitation6H__b__ = true;
                        break;
                    case 14:
                        __precipitation12H__ = reader.ReadString();
                        __precipitation12H__b__ = true;
                        break;
                    case 15:
                        __precipitation24H__ = reader.ReadString();
                        __precipitation24H__b__ = true;
                        break;
                    case 16:
                        __precipitationDesc__ = reader.ReadString();
                        __precipitationDesc__b__ = true;
                        break;
                    case 17:
                        __airInfo__ = reader.ReadString();
                        __airInfo__b__ = true;
                        break;
                    case 18:
                        __airDescription__ = reader.ReadString();
                        __airDescription__b__ = true;
                        break;
                    case 19:
                        __windSpeed__ = reader.ReadString();
                        __windSpeed__b__ = true;
                        break;
                    case 20:
                        __windDirection__ = reader.ReadString();
                        __windDirection__b__ = true;
                        break;
                    case 21:
                        __windDesc__ = reader.ReadString();
                        __windDesc__b__ = true;
                        break;
                    case 22:
                        __windDescShort__ = reader.ReadString();
                        __windDescShort__b__ = true;
                        break;
                    case 23:
                        __barometerPressure__ = reader.ReadString();
                        __barometerPressure__b__ = true;
                        break;
                    case 24:
                        __barometerTrend__ = reader.ReadString();
                        __barometerTrend__b__ = true;
                        break;
                    case 25:
                        __visibility__ = reader.ReadString();
                        __visibility__b__ = true;
                        break;
                    case 26:
                        __snowCover__ = reader.ReadString();
                        __snowCover__b__ = true;
                        break;
                    case 27:
                        __icon__ = reader.ReadString();
                        __icon__b__ = true;
                        break;
                    case 28:
                        __iconName__ = reader.ReadString();
                        __iconName__b__ = true;
                        break;
                    case 29:
                        __iconLink__ = reader.ReadString();
                        __iconLink__b__ = true;
                        break;
                    case 30:
                        __ageMinutes__ = reader.ReadString();
                        __ageMinutes__b__ = true;
                        break;
                    case 31:
                        __activeAlerts__ = reader.ReadString();
                        __activeAlerts__b__ = true;
                        break;
                    case 32:
                        __country__ = reader.ReadString();
                        __country__b__ = true;
                        break;
                    case 33:
                        __state__ = reader.ReadString();
                        __state__b__ = true;
                        break;
                    case 34:
                        __city__ = reader.ReadString();
                        __city__b__ = true;
                        break;
                    case 35:
                        __latitude__ = reader.ReadSingle();
                        __latitude__b__ = true;
                        break;
                    case 36:
                        __longitude__ = reader.ReadSingle();
                        __longitude__b__ = true;
                        break;
                    case 37:
                        __distance__ = reader.ReadSingle();
                        __distance__b__ = true;
                        break;
                    case 38:
                        __elevation__ = reader.ReadSingle();
                        __elevation__b__ = true;
                        break;
                    case 39:
                        __utcTime__ = formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset>().Deserialize(ref reader, formatterResolver);
                        __utcTime__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.HERE.Observation();
            if(__daylight__b__) ____result.daylight = __daylight__;
            if(__description__b__) ____result.description = __description__;
            if(__skyInfo__b__) ____result.skyInfo = __skyInfo__;
            if(__skyDescription__b__) ____result.skyDescription = __skyDescription__;
            if(__temperature__b__) ____result.temperature = __temperature__;
            if(__temperatureDesc__b__) ____result.temperatureDesc = __temperatureDesc__;
            if(__comfort__b__) ____result.comfort = __comfort__;
            if(__highTemperature__b__) ____result.highTemperature = __highTemperature__;
            if(__lowTemperature__b__) ____result.lowTemperature = __lowTemperature__;
            if(__humidity__b__) ____result.humidity = __humidity__;
            if(__dewPoint__b__) ____result.dewPoint = __dewPoint__;
            if(__precipitation1H__b__) ____result.precipitation1H = __precipitation1H__;
            if(__precipitation3H__b__) ____result.precipitation3H = __precipitation3H__;
            if(__precipitation6H__b__) ____result.precipitation6H = __precipitation6H__;
            if(__precipitation12H__b__) ____result.precipitation12H = __precipitation12H__;
            if(__precipitation24H__b__) ____result.precipitation24H = __precipitation24H__;
            if(__precipitationDesc__b__) ____result.precipitationDesc = __precipitationDesc__;
            if(__airInfo__b__) ____result.airInfo = __airInfo__;
            if(__airDescription__b__) ____result.airDescription = __airDescription__;
            if(__windSpeed__b__) ____result.windSpeed = __windSpeed__;
            if(__windDirection__b__) ____result.windDirection = __windDirection__;
            if(__windDesc__b__) ____result.windDesc = __windDesc__;
            if(__windDescShort__b__) ____result.windDescShort = __windDescShort__;
            if(__barometerPressure__b__) ____result.barometerPressure = __barometerPressure__;
            if(__barometerTrend__b__) ____result.barometerTrend = __barometerTrend__;
            if(__visibility__b__) ____result.visibility = __visibility__;
            if(__snowCover__b__) ____result.snowCover = __snowCover__;
            if(__icon__b__) ____result.icon = __icon__;
            if(__iconName__b__) ____result.iconName = __iconName__;
            if(__iconLink__b__) ____result.iconLink = __iconLink__;
            if(__ageMinutes__b__) ____result.ageMinutes = __ageMinutes__;
            if(__activeAlerts__b__) ____result.activeAlerts = __activeAlerts__;
            if(__country__b__) ____result.country = __country__;
            if(__state__b__) ____result.state = __state__;
            if(__city__b__) ____result.city = __city__;
            if(__latitude__b__) ____result.latitude = __latitude__;
            if(__longitude__b__) ____result.longitude = __longitude__;
            if(__distance__b__) ____result.distance = __distance__;
            if(__elevation__b__) ____result.elevation = __elevation__;
            if(__utcTime__b__) ____result.utcTime = __utcTime__;

            return ____result;
        }
    }


    public sealed class LocationFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.HERE.Location>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public LocationFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("observation"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("country"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("state"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("city"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("latitude"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("longitude"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("distance"), 6},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("timezone"), 7},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("observation"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("country"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("state"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("city"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("latitude"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("longitude"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("distance"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("timezone"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.HERE.Location value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Observation[]>().Serialize(ref writer, value.observation, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.country);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.state);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteString(value.city);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteSingle(value.latitude);
            writer.WriteRaw(this.____stringByteKeys[5]);
            writer.WriteSingle(value.longitude);
            writer.WriteRaw(this.____stringByteKeys[6]);
            writer.WriteSingle(value.distance);
            writer.WriteRaw(this.____stringByteKeys[7]);
            writer.WriteInt32(value.timezone);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.HERE.Location Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __observation__ = default(global::SimpleWeather.HERE.Observation[]);
            var __observation__b__ = false;
            var __country__ = default(string);
            var __country__b__ = false;
            var __state__ = default(string);
            var __state__b__ = false;
            var __city__ = default(string);
            var __city__b__ = false;
            var __latitude__ = default(float);
            var __latitude__b__ = false;
            var __longitude__ = default(float);
            var __longitude__b__ = false;
            var __distance__ = default(float);
            var __distance__b__ = false;
            var __timezone__ = default(int);
            var __timezone__b__ = false;

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
                        __observation__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Observation[]>().Deserialize(ref reader, formatterResolver);
                        __observation__b__ = true;
                        break;
                    case 1:
                        __country__ = reader.ReadString();
                        __country__b__ = true;
                        break;
                    case 2:
                        __state__ = reader.ReadString();
                        __state__b__ = true;
                        break;
                    case 3:
                        __city__ = reader.ReadString();
                        __city__b__ = true;
                        break;
                    case 4:
                        __latitude__ = reader.ReadSingle();
                        __latitude__b__ = true;
                        break;
                    case 5:
                        __longitude__ = reader.ReadSingle();
                        __longitude__b__ = true;
                        break;
                    case 6:
                        __distance__ = reader.ReadSingle();
                        __distance__b__ = true;
                        break;
                    case 7:
                        __timezone__ = reader.ReadInt32();
                        __timezone__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.HERE.Location();
            if(__observation__b__) ____result.observation = __observation__;
            if(__country__b__) ____result.country = __country__;
            if(__state__b__) ____result.state = __state__;
            if(__city__b__) ____result.city = __city__;
            if(__latitude__b__) ____result.latitude = __latitude__;
            if(__longitude__b__) ____result.longitude = __longitude__;
            if(__distance__b__) ____result.distance = __distance__;
            if(__timezone__b__) ____result.timezone = __timezone__;

            return ____result;
        }
    }


    public sealed class ObservationsFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.HERE.Observations>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public ObservationsFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("location"), 0},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("location"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.HERE.Observations value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Location[]>().Serialize(ref writer, value.location, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.HERE.Observations Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __location__ = default(global::SimpleWeather.HERE.Location[]);
            var __location__b__ = false;

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
                        __location__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Location[]>().Deserialize(ref reader, formatterResolver);
                        __location__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.HERE.Observations();
            if(__location__b__) ____result.location = __location__;

            return ____result;
        }
    }


    public sealed class ForecastFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.HERE.Forecast>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public ForecastFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("daylight"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("description"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("skyInfo"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("skyDescription"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("temperatureDesc"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("comfort"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("highTemperature"), 6},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("lowTemperature"), 7},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("humidity"), 8},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("dewPoint"), 9},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("precipitationProbability"), 10},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("precipitationDesc"), 11},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("rainFall"), 12},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("snowFall"), 13},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("airInfo"), 14},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("airDescription"), 15},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("windSpeed"), 16},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("windDirection"), 17},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("windDesc"), 18},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("windDescShort"), 19},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("beaufortScale"), 20},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("beaufortDescription"), 21},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("uvIndex"), 22},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("uvDesc"), 23},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("barometerPressure"), 24},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("icon"), 25},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("iconName"), 26},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("iconLink"), 27},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("dayOfWeek"), 28},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("weekday"), 29},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("utcTime"), 30},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("daylight"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("description"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("skyInfo"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("skyDescription"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("temperatureDesc"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("comfort"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("highTemperature"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("lowTemperature"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("humidity"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("dewPoint"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("precipitationProbability"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("precipitationDesc"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("rainFall"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("snowFall"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("airInfo"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("airDescription"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("windSpeed"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("windDirection"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("windDesc"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("windDescShort"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("beaufortScale"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("beaufortDescription"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("uvIndex"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("uvDesc"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("barometerPressure"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("icon"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("iconName"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("iconLink"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("dayOfWeek"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("weekday"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("utcTime"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.HERE.Forecast value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.daylight);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.description);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.skyInfo);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteString(value.skyDescription);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteString(value.temperatureDesc);
            writer.WriteRaw(this.____stringByteKeys[5]);
            writer.WriteString(value.comfort);
            writer.WriteRaw(this.____stringByteKeys[6]);
            writer.WriteString(value.highTemperature);
            writer.WriteRaw(this.____stringByteKeys[7]);
            writer.WriteString(value.lowTemperature);
            writer.WriteRaw(this.____stringByteKeys[8]);
            writer.WriteString(value.humidity);
            writer.WriteRaw(this.____stringByteKeys[9]);
            writer.WriteString(value.dewPoint);
            writer.WriteRaw(this.____stringByteKeys[10]);
            writer.WriteString(value.precipitationProbability);
            writer.WriteRaw(this.____stringByteKeys[11]);
            writer.WriteString(value.precipitationDesc);
            writer.WriteRaw(this.____stringByteKeys[12]);
            writer.WriteString(value.rainFall);
            writer.WriteRaw(this.____stringByteKeys[13]);
            writer.WriteString(value.snowFall);
            writer.WriteRaw(this.____stringByteKeys[14]);
            writer.WriteString(value.airInfo);
            writer.WriteRaw(this.____stringByteKeys[15]);
            writer.WriteString(value.airDescription);
            writer.WriteRaw(this.____stringByteKeys[16]);
            writer.WriteString(value.windSpeed);
            writer.WriteRaw(this.____stringByteKeys[17]);
            writer.WriteString(value.windDirection);
            writer.WriteRaw(this.____stringByteKeys[18]);
            writer.WriteString(value.windDesc);
            writer.WriteRaw(this.____stringByteKeys[19]);
            writer.WriteString(value.windDescShort);
            writer.WriteRaw(this.____stringByteKeys[20]);
            writer.WriteString(value.beaufortScale);
            writer.WriteRaw(this.____stringByteKeys[21]);
            writer.WriteString(value.beaufortDescription);
            writer.WriteRaw(this.____stringByteKeys[22]);
            writer.WriteString(value.uvIndex);
            writer.WriteRaw(this.____stringByteKeys[23]);
            writer.WriteString(value.uvDesc);
            writer.WriteRaw(this.____stringByteKeys[24]);
            writer.WriteString(value.barometerPressure);
            writer.WriteRaw(this.____stringByteKeys[25]);
            writer.WriteString(value.icon);
            writer.WriteRaw(this.____stringByteKeys[26]);
            writer.WriteString(value.iconName);
            writer.WriteRaw(this.____stringByteKeys[27]);
            writer.WriteString(value.iconLink);
            writer.WriteRaw(this.____stringByteKeys[28]);
            writer.WriteString(value.dayOfWeek);
            writer.WriteRaw(this.____stringByteKeys[29]);
            writer.WriteString(value.weekday);
            writer.WriteRaw(this.____stringByteKeys[30]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset>().Serialize(ref writer, value.utcTime, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.HERE.Forecast Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __daylight__ = default(string);
            var __daylight__b__ = false;
            var __description__ = default(string);
            var __description__b__ = false;
            var __skyInfo__ = default(string);
            var __skyInfo__b__ = false;
            var __skyDescription__ = default(string);
            var __skyDescription__b__ = false;
            var __temperatureDesc__ = default(string);
            var __temperatureDesc__b__ = false;
            var __comfort__ = default(string);
            var __comfort__b__ = false;
            var __highTemperature__ = default(string);
            var __highTemperature__b__ = false;
            var __lowTemperature__ = default(string);
            var __lowTemperature__b__ = false;
            var __humidity__ = default(string);
            var __humidity__b__ = false;
            var __dewPoint__ = default(string);
            var __dewPoint__b__ = false;
            var __precipitationProbability__ = default(string);
            var __precipitationProbability__b__ = false;
            var __precipitationDesc__ = default(string);
            var __precipitationDesc__b__ = false;
            var __rainFall__ = default(string);
            var __rainFall__b__ = false;
            var __snowFall__ = default(string);
            var __snowFall__b__ = false;
            var __airInfo__ = default(string);
            var __airInfo__b__ = false;
            var __airDescription__ = default(string);
            var __airDescription__b__ = false;
            var __windSpeed__ = default(string);
            var __windSpeed__b__ = false;
            var __windDirection__ = default(string);
            var __windDirection__b__ = false;
            var __windDesc__ = default(string);
            var __windDesc__b__ = false;
            var __windDescShort__ = default(string);
            var __windDescShort__b__ = false;
            var __beaufortScale__ = default(string);
            var __beaufortScale__b__ = false;
            var __beaufortDescription__ = default(string);
            var __beaufortDescription__b__ = false;
            var __uvIndex__ = default(string);
            var __uvIndex__b__ = false;
            var __uvDesc__ = default(string);
            var __uvDesc__b__ = false;
            var __barometerPressure__ = default(string);
            var __barometerPressure__b__ = false;
            var __icon__ = default(string);
            var __icon__b__ = false;
            var __iconName__ = default(string);
            var __iconName__b__ = false;
            var __iconLink__ = default(string);
            var __iconLink__b__ = false;
            var __dayOfWeek__ = default(string);
            var __dayOfWeek__b__ = false;
            var __weekday__ = default(string);
            var __weekday__b__ = false;
            var __utcTime__ = default(global::System.DateTimeOffset);
            var __utcTime__b__ = false;

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
                        __daylight__ = reader.ReadString();
                        __daylight__b__ = true;
                        break;
                    case 1:
                        __description__ = reader.ReadString();
                        __description__b__ = true;
                        break;
                    case 2:
                        __skyInfo__ = reader.ReadString();
                        __skyInfo__b__ = true;
                        break;
                    case 3:
                        __skyDescription__ = reader.ReadString();
                        __skyDescription__b__ = true;
                        break;
                    case 4:
                        __temperatureDesc__ = reader.ReadString();
                        __temperatureDesc__b__ = true;
                        break;
                    case 5:
                        __comfort__ = reader.ReadString();
                        __comfort__b__ = true;
                        break;
                    case 6:
                        __highTemperature__ = reader.ReadString();
                        __highTemperature__b__ = true;
                        break;
                    case 7:
                        __lowTemperature__ = reader.ReadString();
                        __lowTemperature__b__ = true;
                        break;
                    case 8:
                        __humidity__ = reader.ReadString();
                        __humidity__b__ = true;
                        break;
                    case 9:
                        __dewPoint__ = reader.ReadString();
                        __dewPoint__b__ = true;
                        break;
                    case 10:
                        __precipitationProbability__ = reader.ReadString();
                        __precipitationProbability__b__ = true;
                        break;
                    case 11:
                        __precipitationDesc__ = reader.ReadString();
                        __precipitationDesc__b__ = true;
                        break;
                    case 12:
                        __rainFall__ = reader.ReadString();
                        __rainFall__b__ = true;
                        break;
                    case 13:
                        __snowFall__ = reader.ReadString();
                        __snowFall__b__ = true;
                        break;
                    case 14:
                        __airInfo__ = reader.ReadString();
                        __airInfo__b__ = true;
                        break;
                    case 15:
                        __airDescription__ = reader.ReadString();
                        __airDescription__b__ = true;
                        break;
                    case 16:
                        __windSpeed__ = reader.ReadString();
                        __windSpeed__b__ = true;
                        break;
                    case 17:
                        __windDirection__ = reader.ReadString();
                        __windDirection__b__ = true;
                        break;
                    case 18:
                        __windDesc__ = reader.ReadString();
                        __windDesc__b__ = true;
                        break;
                    case 19:
                        __windDescShort__ = reader.ReadString();
                        __windDescShort__b__ = true;
                        break;
                    case 20:
                        __beaufortScale__ = reader.ReadString();
                        __beaufortScale__b__ = true;
                        break;
                    case 21:
                        __beaufortDescription__ = reader.ReadString();
                        __beaufortDescription__b__ = true;
                        break;
                    case 22:
                        __uvIndex__ = reader.ReadString();
                        __uvIndex__b__ = true;
                        break;
                    case 23:
                        __uvDesc__ = reader.ReadString();
                        __uvDesc__b__ = true;
                        break;
                    case 24:
                        __barometerPressure__ = reader.ReadString();
                        __barometerPressure__b__ = true;
                        break;
                    case 25:
                        __icon__ = reader.ReadString();
                        __icon__b__ = true;
                        break;
                    case 26:
                        __iconName__ = reader.ReadString();
                        __iconName__b__ = true;
                        break;
                    case 27:
                        __iconLink__ = reader.ReadString();
                        __iconLink__b__ = true;
                        break;
                    case 28:
                        __dayOfWeek__ = reader.ReadString();
                        __dayOfWeek__b__ = true;
                        break;
                    case 29:
                        __weekday__ = reader.ReadString();
                        __weekday__b__ = true;
                        break;
                    case 30:
                        __utcTime__ = formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset>().Deserialize(ref reader, formatterResolver);
                        __utcTime__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.HERE.Forecast();
            if(__daylight__b__) ____result.daylight = __daylight__;
            if(__description__b__) ____result.description = __description__;
            if(__skyInfo__b__) ____result.skyInfo = __skyInfo__;
            if(__skyDescription__b__) ____result.skyDescription = __skyDescription__;
            if(__temperatureDesc__b__) ____result.temperatureDesc = __temperatureDesc__;
            if(__comfort__b__) ____result.comfort = __comfort__;
            if(__highTemperature__b__) ____result.highTemperature = __highTemperature__;
            if(__lowTemperature__b__) ____result.lowTemperature = __lowTemperature__;
            if(__humidity__b__) ____result.humidity = __humidity__;
            if(__dewPoint__b__) ____result.dewPoint = __dewPoint__;
            if(__precipitationProbability__b__) ____result.precipitationProbability = __precipitationProbability__;
            if(__precipitationDesc__b__) ____result.precipitationDesc = __precipitationDesc__;
            if(__rainFall__b__) ____result.rainFall = __rainFall__;
            if(__snowFall__b__) ____result.snowFall = __snowFall__;
            if(__airInfo__b__) ____result.airInfo = __airInfo__;
            if(__airDescription__b__) ____result.airDescription = __airDescription__;
            if(__windSpeed__b__) ____result.windSpeed = __windSpeed__;
            if(__windDirection__b__) ____result.windDirection = __windDirection__;
            if(__windDesc__b__) ____result.windDesc = __windDesc__;
            if(__windDescShort__b__) ____result.windDescShort = __windDescShort__;
            if(__beaufortScale__b__) ____result.beaufortScale = __beaufortScale__;
            if(__beaufortDescription__b__) ____result.beaufortDescription = __beaufortDescription__;
            if(__uvIndex__b__) ____result.uvIndex = __uvIndex__;
            if(__uvDesc__b__) ____result.uvDesc = __uvDesc__;
            if(__barometerPressure__b__) ____result.barometerPressure = __barometerPressure__;
            if(__icon__b__) ____result.icon = __icon__;
            if(__iconName__b__) ____result.iconName = __iconName__;
            if(__iconLink__b__) ____result.iconLink = __iconLink__;
            if(__dayOfWeek__b__) ____result.dayOfWeek = __dayOfWeek__;
            if(__weekday__b__) ____result.weekday = __weekday__;
            if(__utcTime__b__) ____result.utcTime = __utcTime__;

            return ____result;
        }
    }


    public sealed class ForecastlocationFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.HERE.Forecastlocation>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public ForecastlocationFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("forecast"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("country"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("state"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("city"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("latitude"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("longitude"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("distance"), 6},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("timezone"), 7},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("forecast"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("country"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("state"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("city"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("latitude"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("longitude"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("distance"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("timezone"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.HERE.Forecastlocation value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Forecast[]>().Serialize(ref writer, value.forecast, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.country);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.state);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteString(value.city);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteSingle(value.latitude);
            writer.WriteRaw(this.____stringByteKeys[5]);
            writer.WriteSingle(value.longitude);
            writer.WriteRaw(this.____stringByteKeys[6]);
            writer.WriteSingle(value.distance);
            writer.WriteRaw(this.____stringByteKeys[7]);
            writer.WriteInt32(value.timezone);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.HERE.Forecastlocation Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __forecast__ = default(global::SimpleWeather.HERE.Forecast[]);
            var __forecast__b__ = false;
            var __country__ = default(string);
            var __country__b__ = false;
            var __state__ = default(string);
            var __state__b__ = false;
            var __city__ = default(string);
            var __city__b__ = false;
            var __latitude__ = default(float);
            var __latitude__b__ = false;
            var __longitude__ = default(float);
            var __longitude__b__ = false;
            var __distance__ = default(float);
            var __distance__b__ = false;
            var __timezone__ = default(int);
            var __timezone__b__ = false;

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
                        __forecast__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Forecast[]>().Deserialize(ref reader, formatterResolver);
                        __forecast__b__ = true;
                        break;
                    case 1:
                        __country__ = reader.ReadString();
                        __country__b__ = true;
                        break;
                    case 2:
                        __state__ = reader.ReadString();
                        __state__b__ = true;
                        break;
                    case 3:
                        __city__ = reader.ReadString();
                        __city__b__ = true;
                        break;
                    case 4:
                        __latitude__ = reader.ReadSingle();
                        __latitude__b__ = true;
                        break;
                    case 5:
                        __longitude__ = reader.ReadSingle();
                        __longitude__b__ = true;
                        break;
                    case 6:
                        __distance__ = reader.ReadSingle();
                        __distance__b__ = true;
                        break;
                    case 7:
                        __timezone__ = reader.ReadInt32();
                        __timezone__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.HERE.Forecastlocation();
            if(__forecast__b__) ____result.forecast = __forecast__;
            if(__country__b__) ____result.country = __country__;
            if(__state__b__) ____result.state = __state__;
            if(__city__b__) ____result.city = __city__;
            if(__latitude__b__) ____result.latitude = __latitude__;
            if(__longitude__b__) ____result.longitude = __longitude__;
            if(__distance__b__) ____result.distance = __distance__;
            if(__timezone__b__) ____result.timezone = __timezone__;

            return ____result;
        }
    }


    public sealed class DailyforecastsFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.HERE.Dailyforecasts>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public DailyforecastsFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("forecastLocation"), 0},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("forecastLocation"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.HERE.Dailyforecasts value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Forecastlocation>().Serialize(ref writer, value.forecastLocation, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.HERE.Dailyforecasts Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __forecastLocation__ = default(global::SimpleWeather.HERE.Forecastlocation);
            var __forecastLocation__b__ = false;

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
                        __forecastLocation__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Forecastlocation>().Deserialize(ref reader, formatterResolver);
                        __forecastLocation__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.HERE.Dailyforecasts();
            if(__forecastLocation__b__) ____result.forecastLocation = __forecastLocation__;

            return ____result;
        }
    }


    public sealed class Forecast1Formatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.HERE.Forecast1>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public Forecast1Formatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("daylight"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("description"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("skyInfo"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("skyDescription"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("temperature"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("temperatureDesc"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("comfort"), 6},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("humidity"), 7},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("dewPoint"), 8},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("precipitationProbability"), 9},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("precipitationDesc"), 10},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("rainFall"), 11},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("snowFall"), 12},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("airInfo"), 13},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("airDescription"), 14},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("windSpeed"), 15},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("windDirection"), 16},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("windDesc"), 17},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("windDescShort"), 18},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("beaufortScale"), 19},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("beaufortDescription"), 20},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("uvIndex"), 21},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("uvDesc"), 22},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("visibility"), 23},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("barometerPressure"), 24},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("icon"), 25},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("iconName"), 26},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("iconLink"), 27},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("dayOfWeek"), 28},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("weekday"), 29},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("utcTime"), 30},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("localTime"), 31},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("localTimeFormat"), 32},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("daylight"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("description"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("skyInfo"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("skyDescription"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("temperature"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("temperatureDesc"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("comfort"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("humidity"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("dewPoint"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("precipitationProbability"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("precipitationDesc"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("rainFall"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("snowFall"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("airInfo"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("airDescription"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("windSpeed"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("windDirection"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("windDesc"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("windDescShort"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("beaufortScale"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("beaufortDescription"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("uvIndex"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("uvDesc"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("visibility"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("barometerPressure"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("icon"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("iconName"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("iconLink"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("dayOfWeek"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("weekday"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("utcTime"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("localTime"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("localTimeFormat"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.HERE.Forecast1 value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.daylight);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.description);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.skyInfo);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteString(value.skyDescription);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteString(value.temperature);
            writer.WriteRaw(this.____stringByteKeys[5]);
            writer.WriteString(value.temperatureDesc);
            writer.WriteRaw(this.____stringByteKeys[6]);
            writer.WriteString(value.comfort);
            writer.WriteRaw(this.____stringByteKeys[7]);
            writer.WriteString(value.humidity);
            writer.WriteRaw(this.____stringByteKeys[8]);
            writer.WriteString(value.dewPoint);
            writer.WriteRaw(this.____stringByteKeys[9]);
            writer.WriteString(value.precipitationProbability);
            writer.WriteRaw(this.____stringByteKeys[10]);
            writer.WriteString(value.precipitationDesc);
            writer.WriteRaw(this.____stringByteKeys[11]);
            writer.WriteString(value.rainFall);
            writer.WriteRaw(this.____stringByteKeys[12]);
            writer.WriteString(value.snowFall);
            writer.WriteRaw(this.____stringByteKeys[13]);
            writer.WriteString(value.airInfo);
            writer.WriteRaw(this.____stringByteKeys[14]);
            writer.WriteString(value.airDescription);
            writer.WriteRaw(this.____stringByteKeys[15]);
            writer.WriteString(value.windSpeed);
            writer.WriteRaw(this.____stringByteKeys[16]);
            writer.WriteString(value.windDirection);
            writer.WriteRaw(this.____stringByteKeys[17]);
            writer.WriteString(value.windDesc);
            writer.WriteRaw(this.____stringByteKeys[18]);
            writer.WriteString(value.windDescShort);
            writer.WriteRaw(this.____stringByteKeys[19]);
            writer.WriteString(value.beaufortScale);
            writer.WriteRaw(this.____stringByteKeys[20]);
            writer.WriteString(value.beaufortDescription);
            writer.WriteRaw(this.____stringByteKeys[21]);
            writer.WriteString(value.uvIndex);
            writer.WriteRaw(this.____stringByteKeys[22]);
            writer.WriteString(value.uvDesc);
            writer.WriteRaw(this.____stringByteKeys[23]);
            writer.WriteString(value.visibility);
            writer.WriteRaw(this.____stringByteKeys[24]);
            writer.WriteString(value.barometerPressure);
            writer.WriteRaw(this.____stringByteKeys[25]);
            writer.WriteString(value.icon);
            writer.WriteRaw(this.____stringByteKeys[26]);
            writer.WriteString(value.iconName);
            writer.WriteRaw(this.____stringByteKeys[27]);
            writer.WriteString(value.iconLink);
            writer.WriteRaw(this.____stringByteKeys[28]);
            writer.WriteString(value.dayOfWeek);
            writer.WriteRaw(this.____stringByteKeys[29]);
            writer.WriteString(value.weekday);
            writer.WriteRaw(this.____stringByteKeys[30]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset>().Serialize(ref writer, value.utcTime, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[31]);
            writer.WriteString(value.localTime);
            writer.WriteRaw(this.____stringByteKeys[32]);
            writer.WriteString(value.localTimeFormat);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.HERE.Forecast1 Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __daylight__ = default(string);
            var __daylight__b__ = false;
            var __description__ = default(string);
            var __description__b__ = false;
            var __skyInfo__ = default(string);
            var __skyInfo__b__ = false;
            var __skyDescription__ = default(string);
            var __skyDescription__b__ = false;
            var __temperature__ = default(string);
            var __temperature__b__ = false;
            var __temperatureDesc__ = default(string);
            var __temperatureDesc__b__ = false;
            var __comfort__ = default(string);
            var __comfort__b__ = false;
            var __humidity__ = default(string);
            var __humidity__b__ = false;
            var __dewPoint__ = default(string);
            var __dewPoint__b__ = false;
            var __precipitationProbability__ = default(string);
            var __precipitationProbability__b__ = false;
            var __precipitationDesc__ = default(string);
            var __precipitationDesc__b__ = false;
            var __rainFall__ = default(string);
            var __rainFall__b__ = false;
            var __snowFall__ = default(string);
            var __snowFall__b__ = false;
            var __airInfo__ = default(string);
            var __airInfo__b__ = false;
            var __airDescription__ = default(string);
            var __airDescription__b__ = false;
            var __windSpeed__ = default(string);
            var __windSpeed__b__ = false;
            var __windDirection__ = default(string);
            var __windDirection__b__ = false;
            var __windDesc__ = default(string);
            var __windDesc__b__ = false;
            var __windDescShort__ = default(string);
            var __windDescShort__b__ = false;
            var __beaufortScale__ = default(string);
            var __beaufortScale__b__ = false;
            var __beaufortDescription__ = default(string);
            var __beaufortDescription__b__ = false;
            var __uvIndex__ = default(string);
            var __uvIndex__b__ = false;
            var __uvDesc__ = default(string);
            var __uvDesc__b__ = false;
            var __visibility__ = default(string);
            var __visibility__b__ = false;
            var __barometerPressure__ = default(string);
            var __barometerPressure__b__ = false;
            var __icon__ = default(string);
            var __icon__b__ = false;
            var __iconName__ = default(string);
            var __iconName__b__ = false;
            var __iconLink__ = default(string);
            var __iconLink__b__ = false;
            var __dayOfWeek__ = default(string);
            var __dayOfWeek__b__ = false;
            var __weekday__ = default(string);
            var __weekday__b__ = false;
            var __utcTime__ = default(global::System.DateTimeOffset);
            var __utcTime__b__ = false;
            var __localTime__ = default(string);
            var __localTime__b__ = false;
            var __localTimeFormat__ = default(string);
            var __localTimeFormat__b__ = false;

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
                        __daylight__ = reader.ReadString();
                        __daylight__b__ = true;
                        break;
                    case 1:
                        __description__ = reader.ReadString();
                        __description__b__ = true;
                        break;
                    case 2:
                        __skyInfo__ = reader.ReadString();
                        __skyInfo__b__ = true;
                        break;
                    case 3:
                        __skyDescription__ = reader.ReadString();
                        __skyDescription__b__ = true;
                        break;
                    case 4:
                        __temperature__ = reader.ReadString();
                        __temperature__b__ = true;
                        break;
                    case 5:
                        __temperatureDesc__ = reader.ReadString();
                        __temperatureDesc__b__ = true;
                        break;
                    case 6:
                        __comfort__ = reader.ReadString();
                        __comfort__b__ = true;
                        break;
                    case 7:
                        __humidity__ = reader.ReadString();
                        __humidity__b__ = true;
                        break;
                    case 8:
                        __dewPoint__ = reader.ReadString();
                        __dewPoint__b__ = true;
                        break;
                    case 9:
                        __precipitationProbability__ = reader.ReadString();
                        __precipitationProbability__b__ = true;
                        break;
                    case 10:
                        __precipitationDesc__ = reader.ReadString();
                        __precipitationDesc__b__ = true;
                        break;
                    case 11:
                        __rainFall__ = reader.ReadString();
                        __rainFall__b__ = true;
                        break;
                    case 12:
                        __snowFall__ = reader.ReadString();
                        __snowFall__b__ = true;
                        break;
                    case 13:
                        __airInfo__ = reader.ReadString();
                        __airInfo__b__ = true;
                        break;
                    case 14:
                        __airDescription__ = reader.ReadString();
                        __airDescription__b__ = true;
                        break;
                    case 15:
                        __windSpeed__ = reader.ReadString();
                        __windSpeed__b__ = true;
                        break;
                    case 16:
                        __windDirection__ = reader.ReadString();
                        __windDirection__b__ = true;
                        break;
                    case 17:
                        __windDesc__ = reader.ReadString();
                        __windDesc__b__ = true;
                        break;
                    case 18:
                        __windDescShort__ = reader.ReadString();
                        __windDescShort__b__ = true;
                        break;
                    case 19:
                        __beaufortScale__ = reader.ReadString();
                        __beaufortScale__b__ = true;
                        break;
                    case 20:
                        __beaufortDescription__ = reader.ReadString();
                        __beaufortDescription__b__ = true;
                        break;
                    case 21:
                        __uvIndex__ = reader.ReadString();
                        __uvIndex__b__ = true;
                        break;
                    case 22:
                        __uvDesc__ = reader.ReadString();
                        __uvDesc__b__ = true;
                        break;
                    case 23:
                        __visibility__ = reader.ReadString();
                        __visibility__b__ = true;
                        break;
                    case 24:
                        __barometerPressure__ = reader.ReadString();
                        __barometerPressure__b__ = true;
                        break;
                    case 25:
                        __icon__ = reader.ReadString();
                        __icon__b__ = true;
                        break;
                    case 26:
                        __iconName__ = reader.ReadString();
                        __iconName__b__ = true;
                        break;
                    case 27:
                        __iconLink__ = reader.ReadString();
                        __iconLink__b__ = true;
                        break;
                    case 28:
                        __dayOfWeek__ = reader.ReadString();
                        __dayOfWeek__b__ = true;
                        break;
                    case 29:
                        __weekday__ = reader.ReadString();
                        __weekday__b__ = true;
                        break;
                    case 30:
                        __utcTime__ = formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset>().Deserialize(ref reader, formatterResolver);
                        __utcTime__b__ = true;
                        break;
                    case 31:
                        __localTime__ = reader.ReadString();
                        __localTime__b__ = true;
                        break;
                    case 32:
                        __localTimeFormat__ = reader.ReadString();
                        __localTimeFormat__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.HERE.Forecast1();
            if(__daylight__b__) ____result.daylight = __daylight__;
            if(__description__b__) ____result.description = __description__;
            if(__skyInfo__b__) ____result.skyInfo = __skyInfo__;
            if(__skyDescription__b__) ____result.skyDescription = __skyDescription__;
            if(__temperature__b__) ____result.temperature = __temperature__;
            if(__temperatureDesc__b__) ____result.temperatureDesc = __temperatureDesc__;
            if(__comfort__b__) ____result.comfort = __comfort__;
            if(__humidity__b__) ____result.humidity = __humidity__;
            if(__dewPoint__b__) ____result.dewPoint = __dewPoint__;
            if(__precipitationProbability__b__) ____result.precipitationProbability = __precipitationProbability__;
            if(__precipitationDesc__b__) ____result.precipitationDesc = __precipitationDesc__;
            if(__rainFall__b__) ____result.rainFall = __rainFall__;
            if(__snowFall__b__) ____result.snowFall = __snowFall__;
            if(__airInfo__b__) ____result.airInfo = __airInfo__;
            if(__airDescription__b__) ____result.airDescription = __airDescription__;
            if(__windSpeed__b__) ____result.windSpeed = __windSpeed__;
            if(__windDirection__b__) ____result.windDirection = __windDirection__;
            if(__windDesc__b__) ____result.windDesc = __windDesc__;
            if(__windDescShort__b__) ____result.windDescShort = __windDescShort__;
            if(__beaufortScale__b__) ____result.beaufortScale = __beaufortScale__;
            if(__beaufortDescription__b__) ____result.beaufortDescription = __beaufortDescription__;
            if(__uvIndex__b__) ____result.uvIndex = __uvIndex__;
            if(__uvDesc__b__) ____result.uvDesc = __uvDesc__;
            if(__visibility__b__) ____result.visibility = __visibility__;
            if(__barometerPressure__b__) ____result.barometerPressure = __barometerPressure__;
            if(__icon__b__) ____result.icon = __icon__;
            if(__iconName__b__) ____result.iconName = __iconName__;
            if(__iconLink__b__) ____result.iconLink = __iconLink__;
            if(__dayOfWeek__b__) ____result.dayOfWeek = __dayOfWeek__;
            if(__weekday__b__) ____result.weekday = __weekday__;
            if(__utcTime__b__) ____result.utcTime = __utcTime__;
            if(__localTime__b__) ____result.localTime = __localTime__;
            if(__localTimeFormat__b__) ____result.localTimeFormat = __localTimeFormat__;

            return ____result;
        }
    }


    public sealed class Forecastlocation1Formatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.HERE.Forecastlocation1>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public Forecastlocation1Formatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("forecast"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("country"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("state"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("city"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("latitude"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("longitude"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("distance"), 6},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("timezone"), 7},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("forecast"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("country"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("state"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("city"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("latitude"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("longitude"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("distance"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("timezone"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.HERE.Forecastlocation1 value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Forecast1[]>().Serialize(ref writer, value.forecast, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.country);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.state);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteString(value.city);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteSingle(value.latitude);
            writer.WriteRaw(this.____stringByteKeys[5]);
            writer.WriteSingle(value.longitude);
            writer.WriteRaw(this.____stringByteKeys[6]);
            writer.WriteSingle(value.distance);
            writer.WriteRaw(this.____stringByteKeys[7]);
            writer.WriteInt32(value.timezone);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.HERE.Forecastlocation1 Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __forecast__ = default(global::SimpleWeather.HERE.Forecast1[]);
            var __forecast__b__ = false;
            var __country__ = default(string);
            var __country__b__ = false;
            var __state__ = default(string);
            var __state__b__ = false;
            var __city__ = default(string);
            var __city__b__ = false;
            var __latitude__ = default(float);
            var __latitude__b__ = false;
            var __longitude__ = default(float);
            var __longitude__b__ = false;
            var __distance__ = default(float);
            var __distance__b__ = false;
            var __timezone__ = default(int);
            var __timezone__b__ = false;

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
                        __forecast__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Forecast1[]>().Deserialize(ref reader, formatterResolver);
                        __forecast__b__ = true;
                        break;
                    case 1:
                        __country__ = reader.ReadString();
                        __country__b__ = true;
                        break;
                    case 2:
                        __state__ = reader.ReadString();
                        __state__b__ = true;
                        break;
                    case 3:
                        __city__ = reader.ReadString();
                        __city__b__ = true;
                        break;
                    case 4:
                        __latitude__ = reader.ReadSingle();
                        __latitude__b__ = true;
                        break;
                    case 5:
                        __longitude__ = reader.ReadSingle();
                        __longitude__b__ = true;
                        break;
                    case 6:
                        __distance__ = reader.ReadSingle();
                        __distance__b__ = true;
                        break;
                    case 7:
                        __timezone__ = reader.ReadInt32();
                        __timezone__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.HERE.Forecastlocation1();
            if(__forecast__b__) ____result.forecast = __forecast__;
            if(__country__b__) ____result.country = __country__;
            if(__state__b__) ____result.state = __state__;
            if(__city__b__) ____result.city = __city__;
            if(__latitude__b__) ____result.latitude = __latitude__;
            if(__longitude__b__) ____result.longitude = __longitude__;
            if(__distance__b__) ____result.distance = __distance__;
            if(__timezone__b__) ____result.timezone = __timezone__;

            return ____result;
        }
    }


    public sealed class HourlyforecastsFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.HERE.Hourlyforecasts>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public HourlyforecastsFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("forecastLocation"), 0},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("forecastLocation"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.HERE.Hourlyforecasts value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Forecastlocation1>().Serialize(ref writer, value.forecastLocation, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.HERE.Hourlyforecasts Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __forecastLocation__ = default(global::SimpleWeather.HERE.Forecastlocation1);
            var __forecastLocation__b__ = false;

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
                        __forecastLocation__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Forecastlocation1>().Deserialize(ref reader, formatterResolver);
                        __forecastLocation__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.HERE.Hourlyforecasts();
            if(__forecastLocation__b__) ____result.forecastLocation = __forecastLocation__;

            return ____result;
        }
    }


    public sealed class Astronomy1Formatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.HERE.Astronomy1>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public Astronomy1Formatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("sunrise"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("sunset"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("moonrise"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("moonset"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("moonPhase"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("moonPhaseDesc"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("iconName"), 6},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("city"), 7},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("latitude"), 8},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("longitude"), 9},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("utcTime"), 10},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("sunrise"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("sunset"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("moonrise"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("moonset"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("moonPhase"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("moonPhaseDesc"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("iconName"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("city"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("latitude"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("longitude"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("utcTime"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.HERE.Astronomy1 value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.sunrise);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.sunset);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.moonrise);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteString(value.moonset);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteSingle(value.moonPhase);
            writer.WriteRaw(this.____stringByteKeys[5]);
            writer.WriteString(value.moonPhaseDesc);
            writer.WriteRaw(this.____stringByteKeys[6]);
            writer.WriteString(value.iconName);
            writer.WriteRaw(this.____stringByteKeys[7]);
            writer.WriteString(value.city);
            writer.WriteRaw(this.____stringByteKeys[8]);
            writer.WriteSingle(value.latitude);
            writer.WriteRaw(this.____stringByteKeys[9]);
            writer.WriteSingle(value.longitude);
            writer.WriteRaw(this.____stringByteKeys[10]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset>().Serialize(ref writer, value.utcTime, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.HERE.Astronomy1 Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __sunrise__ = default(string);
            var __sunrise__b__ = false;
            var __sunset__ = default(string);
            var __sunset__b__ = false;
            var __moonrise__ = default(string);
            var __moonrise__b__ = false;
            var __moonset__ = default(string);
            var __moonset__b__ = false;
            var __moonPhase__ = default(float);
            var __moonPhase__b__ = false;
            var __moonPhaseDesc__ = default(string);
            var __moonPhaseDesc__b__ = false;
            var __iconName__ = default(string);
            var __iconName__b__ = false;
            var __city__ = default(string);
            var __city__b__ = false;
            var __latitude__ = default(float);
            var __latitude__b__ = false;
            var __longitude__ = default(float);
            var __longitude__b__ = false;
            var __utcTime__ = default(global::System.DateTimeOffset);
            var __utcTime__b__ = false;

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
                        __sunrise__ = reader.ReadString();
                        __sunrise__b__ = true;
                        break;
                    case 1:
                        __sunset__ = reader.ReadString();
                        __sunset__b__ = true;
                        break;
                    case 2:
                        __moonrise__ = reader.ReadString();
                        __moonrise__b__ = true;
                        break;
                    case 3:
                        __moonset__ = reader.ReadString();
                        __moonset__b__ = true;
                        break;
                    case 4:
                        __moonPhase__ = reader.ReadSingle();
                        __moonPhase__b__ = true;
                        break;
                    case 5:
                        __moonPhaseDesc__ = reader.ReadString();
                        __moonPhaseDesc__b__ = true;
                        break;
                    case 6:
                        __iconName__ = reader.ReadString();
                        __iconName__b__ = true;
                        break;
                    case 7:
                        __city__ = reader.ReadString();
                        __city__b__ = true;
                        break;
                    case 8:
                        __latitude__ = reader.ReadSingle();
                        __latitude__b__ = true;
                        break;
                    case 9:
                        __longitude__ = reader.ReadSingle();
                        __longitude__b__ = true;
                        break;
                    case 10:
                        __utcTime__ = formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset>().Deserialize(ref reader, formatterResolver);
                        __utcTime__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.HERE.Astronomy1();
            if(__sunrise__b__) ____result.sunrise = __sunrise__;
            if(__sunset__b__) ____result.sunset = __sunset__;
            if(__moonrise__b__) ____result.moonrise = __moonrise__;
            if(__moonset__b__) ____result.moonset = __moonset__;
            if(__moonPhase__b__) ____result.moonPhase = __moonPhase__;
            if(__moonPhaseDesc__b__) ____result.moonPhaseDesc = __moonPhaseDesc__;
            if(__iconName__b__) ____result.iconName = __iconName__;
            if(__city__b__) ____result.city = __city__;
            if(__latitude__b__) ____result.latitude = __latitude__;
            if(__longitude__b__) ____result.longitude = __longitude__;
            if(__utcTime__b__) ____result.utcTime = __utcTime__;

            return ____result;
        }
    }


    public sealed class AstronomyFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.HERE.Astronomy>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public AstronomyFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("astronomy"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("country"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("state"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("city"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("latitude"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("longitude"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("timezone"), 6},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("astronomy"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("country"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("state"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("city"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("latitude"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("longitude"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("timezone"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.HERE.Astronomy value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Astronomy1[]>().Serialize(ref writer, value.astronomy, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.country);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.state);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteString(value.city);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteSingle(value.latitude);
            writer.WriteRaw(this.____stringByteKeys[5]);
            writer.WriteSingle(value.longitude);
            writer.WriteRaw(this.____stringByteKeys[6]);
            writer.WriteInt32(value.timezone);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.HERE.Astronomy Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __astronomy__ = default(global::SimpleWeather.HERE.Astronomy1[]);
            var __astronomy__b__ = false;
            var __country__ = default(string);
            var __country__b__ = false;
            var __state__ = default(string);
            var __state__b__ = false;
            var __city__ = default(string);
            var __city__b__ = false;
            var __latitude__ = default(float);
            var __latitude__b__ = false;
            var __longitude__ = default(float);
            var __longitude__b__ = false;
            var __timezone__ = default(int);
            var __timezone__b__ = false;

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
                        __astronomy__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Astronomy1[]>().Deserialize(ref reader, formatterResolver);
                        __astronomy__b__ = true;
                        break;
                    case 1:
                        __country__ = reader.ReadString();
                        __country__b__ = true;
                        break;
                    case 2:
                        __state__ = reader.ReadString();
                        __state__b__ = true;
                        break;
                    case 3:
                        __city__ = reader.ReadString();
                        __city__b__ = true;
                        break;
                    case 4:
                        __latitude__ = reader.ReadSingle();
                        __latitude__b__ = true;
                        break;
                    case 5:
                        __longitude__ = reader.ReadSingle();
                        __longitude__b__ = true;
                        break;
                    case 6:
                        __timezone__ = reader.ReadInt32();
                        __timezone__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.HERE.Astronomy();
            if(__astronomy__b__) ____result.astronomy = __astronomy__;
            if(__country__b__) ____result.country = __country__;
            if(__state__b__) ____result.state = __state__;
            if(__city__b__) ____result.city = __city__;
            if(__latitude__b__) ____result.latitude = __latitude__;
            if(__longitude__b__) ____result.longitude = __longitude__;
            if(__timezone__b__) ____result.timezone = __timezone__;

            return ____result;
        }
    }


    public sealed class RootobjectFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.HERE.Rootobject>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public RootobjectFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("observations"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("dailyForecasts"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("hourlyForecasts"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("alerts"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("astronomy"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("feedCreation"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("metric"), 6},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("Type"), 7},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("Message"), 8},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("observations"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("dailyForecasts"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("hourlyForecasts"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("alerts"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("astronomy"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("feedCreation"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("metric"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("Type"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("Message"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.HERE.Rootobject value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Observations>().Serialize(ref writer, value.observations, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Dailyforecasts>().Serialize(ref writer, value.dailyForecasts, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[2]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Hourlyforecasts>().Serialize(ref writer, value.hourlyForecasts, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[3]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Alerts>().Serialize(ref writer, value.alerts, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[4]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Astronomy>().Serialize(ref writer, value.astronomy, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[5]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset>().Serialize(ref writer, value.feedCreation, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[6]);
            writer.WriteBoolean(value.metric);
            writer.WriteRaw(this.____stringByteKeys[7]);
            writer.WriteString(value.Type);
            writer.WriteRaw(this.____stringByteKeys[8]);
            formatterResolver.GetFormatterWithVerify<string[]>().Serialize(ref writer, value.Message, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.HERE.Rootobject Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __observations__ = default(global::SimpleWeather.HERE.Observations);
            var __observations__b__ = false;
            var __dailyForecasts__ = default(global::SimpleWeather.HERE.Dailyforecasts);
            var __dailyForecasts__b__ = false;
            var __hourlyForecasts__ = default(global::SimpleWeather.HERE.Hourlyforecasts);
            var __hourlyForecasts__b__ = false;
            var __alerts__ = default(global::SimpleWeather.HERE.Alerts);
            var __alerts__b__ = false;
            var __astronomy__ = default(global::SimpleWeather.HERE.Astronomy);
            var __astronomy__b__ = false;
            var __feedCreation__ = default(global::System.DateTimeOffset);
            var __feedCreation__b__ = false;
            var __metric__ = default(bool);
            var __metric__b__ = false;
            var __Type__ = default(string);
            var __Type__b__ = false;
            var __Message__ = default(string[]);
            var __Message__b__ = false;

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
                        __observations__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Observations>().Deserialize(ref reader, formatterResolver);
                        __observations__b__ = true;
                        break;
                    case 1:
                        __dailyForecasts__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Dailyforecasts>().Deserialize(ref reader, formatterResolver);
                        __dailyForecasts__b__ = true;
                        break;
                    case 2:
                        __hourlyForecasts__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Hourlyforecasts>().Deserialize(ref reader, formatterResolver);
                        __hourlyForecasts__b__ = true;
                        break;
                    case 3:
                        __alerts__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Alerts>().Deserialize(ref reader, formatterResolver);
                        __alerts__b__ = true;
                        break;
                    case 4:
                        __astronomy__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Astronomy>().Deserialize(ref reader, formatterResolver);
                        __astronomy__b__ = true;
                        break;
                    case 5:
                        __feedCreation__ = formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset>().Deserialize(ref reader, formatterResolver);
                        __feedCreation__b__ = true;
                        break;
                    case 6:
                        __metric__ = reader.ReadBoolean();
                        __metric__b__ = true;
                        break;
                    case 7:
                        __Type__ = reader.ReadString();
                        __Type__b__ = true;
                        break;
                    case 8:
                        __Message__ = formatterResolver.GetFormatterWithVerify<string[]>().Deserialize(ref reader, formatterResolver);
                        __Message__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.HERE.Rootobject();
            if(__observations__b__) ____result.observations = __observations__;
            if(__dailyForecasts__b__) ____result.dailyForecasts = __dailyForecasts__;
            if(__hourlyForecasts__b__) ____result.hourlyForecasts = __hourlyForecasts__;
            if(__alerts__b__) ____result.alerts = __alerts__;
            if(__astronomy__b__) ____result.astronomy = __astronomy__;
            if(__feedCreation__b__) ____result.feedCreation = __feedCreation__;
            if(__metric__b__) ____result.metric = __metric__;
            if(__Type__b__) ____result.Type = __Type__;
            if(__Message__b__) ____result.Message = __Message__;

            return ____result;
        }
    }

}

#pragma warning disable 168
#pragma warning restore 219
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612
#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 219
#pragma warning disable 168

namespace SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.NWS
{
    using System;
    using Utf8Json;


    public sealed class AlertGraphFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.NWS.AlertGraph>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public AlertGraphFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("sent"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("effective"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("onset"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("expires"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("severity"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("event"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("description"), 6},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("instruction"), 7},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("sent"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("effective"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("onset"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("expires"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("severity"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("event"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("description"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("instruction"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.NWS.AlertGraph value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset>().Serialize(ref writer, value.sent, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset>().Serialize(ref writer, value.effective, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[2]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset>().Serialize(ref writer, value.onset, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[3]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset>().Serialize(ref writer, value.expires, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteString(value.severity);
            writer.WriteRaw(this.____stringByteKeys[5]);
            writer.WriteString(value._event);
            writer.WriteRaw(this.____stringByteKeys[6]);
            writer.WriteString(value.description);
            writer.WriteRaw(this.____stringByteKeys[7]);
            writer.WriteString(value.instruction);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.NWS.AlertGraph Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __sent__ = default(global::System.DateTimeOffset);
            var __sent__b__ = false;
            var __effective__ = default(global::System.DateTimeOffset);
            var __effective__b__ = false;
            var __onset__ = default(global::System.DateTimeOffset);
            var __onset__b__ = false;
            var __expires__ = default(global::System.DateTimeOffset);
            var __expires__b__ = false;
            var __severity__ = default(string);
            var __severity__b__ = false;
            var ___event__ = default(string);
            var ___event__b__ = false;
            var __description__ = default(string);
            var __description__b__ = false;
            var __instruction__ = default(string);
            var __instruction__b__ = false;

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
                        __sent__ = formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset>().Deserialize(ref reader, formatterResolver);
                        __sent__b__ = true;
                        break;
                    case 1:
                        __effective__ = formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset>().Deserialize(ref reader, formatterResolver);
                        __effective__b__ = true;
                        break;
                    case 2:
                        __onset__ = formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset>().Deserialize(ref reader, formatterResolver);
                        __onset__b__ = true;
                        break;
                    case 3:
                        __expires__ = formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset>().Deserialize(ref reader, formatterResolver);
                        __expires__b__ = true;
                        break;
                    case 4:
                        __severity__ = reader.ReadString();
                        __severity__b__ = true;
                        break;
                    case 5:
                        ___event__ = reader.ReadString();
                        ___event__b__ = true;
                        break;
                    case 6:
                        __description__ = reader.ReadString();
                        __description__b__ = true;
                        break;
                    case 7:
                        __instruction__ = reader.ReadString();
                        __instruction__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.NWS.AlertGraph();
            if(__sent__b__) ____result.sent = __sent__;
            if(__effective__b__) ____result.effective = __effective__;
            if(__onset__b__) ____result.onset = __onset__;
            if(__expires__b__) ____result.expires = __expires__;
            if(__severity__b__) ____result.severity = __severity__;
            if(___event__b__) ____result._event = ___event__;
            if(__description__b__) ____result.description = __description__;
            if(__instruction__b__) ____result.instruction = __instruction__;

            return ____result;
        }
    }


    public sealed class AlertRootobjectFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.NWS.AlertRootobject>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public AlertRootobjectFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("@graph"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("title"), 1},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("@graph"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("title"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.NWS.AlertRootobject value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.NWS.AlertGraph[]>().Serialize(ref writer, value.graph, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.title);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.NWS.AlertRootobject Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __graph__ = default(global::SimpleWeather.NWS.AlertGraph[]);
            var __graph__b__ = false;
            var __title__ = default(string);
            var __title__b__ = false;

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
                        __graph__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.NWS.AlertGraph[]>().Deserialize(ref reader, formatterResolver);
                        __graph__b__ = true;
                        break;
                    case 1:
                        __title__ = reader.ReadString();
                        __title__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.NWS.AlertRootobject();
            if(__graph__b__) ____result.graph = __graph__;
            if(__title__b__) ____result.title = __title__;

            return ____result;
        }
    }


    public sealed class PointsRootobjectFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.NWS.PointsRootobject>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public PointsRootobjectFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("forecast"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("forecastHourly"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("forecastGridData"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("observationStations"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("timeZone"), 4},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("forecast"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("forecastHourly"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("forecastGridData"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("observationStations"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("timeZone"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.NWS.PointsRootobject value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.forecast);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.forecastHourly);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.forecastGridData);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteString(value.observationStations);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteString(value.timeZone);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.NWS.PointsRootobject Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __forecast__ = default(string);
            var __forecast__b__ = false;
            var __forecastHourly__ = default(string);
            var __forecastHourly__b__ = false;
            var __forecastGridData__ = default(string);
            var __forecastGridData__b__ = false;
            var __observationStations__ = default(string);
            var __observationStations__b__ = false;
            var __timeZone__ = default(string);
            var __timeZone__b__ = false;

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
                        __forecast__ = reader.ReadString();
                        __forecast__b__ = true;
                        break;
                    case 1:
                        __forecastHourly__ = reader.ReadString();
                        __forecastHourly__b__ = true;
                        break;
                    case 2:
                        __forecastGridData__ = reader.ReadString();
                        __forecastGridData__b__ = true;
                        break;
                    case 3:
                        __observationStations__ = reader.ReadString();
                        __observationStations__b__ = true;
                        break;
                    case 4:
                        __timeZone__ = reader.ReadString();
                        __timeZone__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.NWS.PointsRootobject();
            if(__forecast__b__) ____result.forecast = __forecast__;
            if(__forecastHourly__b__) ____result.forecastHourly = __forecastHourly__;
            if(__forecastGridData__b__) ____result.forecastGridData = __forecastGridData__;
            if(__observationStations__b__) ____result.observationStations = __observationStations__;
            if(__timeZone__b__) ____result.timeZone = __timeZone__;

            return ____result;
        }
    }


    public sealed class PeriodFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.NWS.Period>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public PeriodFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("number"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("name"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("startTime"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("endTime"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("isDaytime"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("temperature"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("temperatureUnit"), 6},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("temperatureTrend"), 7},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("windSpeed"), 8},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("windDirection"), 9},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("icon"), 10},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("shortForecast"), 11},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("detailedForecast"), 12},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("number"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("name"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("startTime"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("endTime"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("isDaytime"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("temperature"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("temperatureUnit"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("temperatureTrend"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("windSpeed"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("windDirection"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("icon"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("shortForecast"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("detailedForecast"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.NWS.Period value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteInt32(value.number);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.name);
            writer.WriteRaw(this.____stringByteKeys[2]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset>().Serialize(ref writer, value.startTime, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[3]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset>().Serialize(ref writer, value.endTime, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteBoolean(value.isDaytime);
            writer.WriteRaw(this.____stringByteKeys[5]);
            writer.WriteInt32(value.temperature);
            writer.WriteRaw(this.____stringByteKeys[6]);
            writer.WriteString(value.temperatureUnit);
            writer.WriteRaw(this.____stringByteKeys[7]);
            writer.WriteString(value.temperatureTrend);
            writer.WriteRaw(this.____stringByteKeys[8]);
            writer.WriteString(value.windSpeed);
            writer.WriteRaw(this.____stringByteKeys[9]);
            writer.WriteString(value.windDirection);
            writer.WriteRaw(this.____stringByteKeys[10]);
            writer.WriteString(value.icon);
            writer.WriteRaw(this.____stringByteKeys[11]);
            writer.WriteString(value.shortForecast);
            writer.WriteRaw(this.____stringByteKeys[12]);
            writer.WriteString(value.detailedForecast);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.NWS.Period Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __number__ = default(int);
            var __number__b__ = false;
            var __name__ = default(string);
            var __name__b__ = false;
            var __startTime__ = default(global::System.DateTimeOffset);
            var __startTime__b__ = false;
            var __endTime__ = default(global::System.DateTimeOffset);
            var __endTime__b__ = false;
            var __isDaytime__ = default(bool);
            var __isDaytime__b__ = false;
            var __temperature__ = default(int);
            var __temperature__b__ = false;
            var __temperatureUnit__ = default(string);
            var __temperatureUnit__b__ = false;
            var __temperatureTrend__ = default(string);
            var __temperatureTrend__b__ = false;
            var __windSpeed__ = default(string);
            var __windSpeed__b__ = false;
            var __windDirection__ = default(string);
            var __windDirection__b__ = false;
            var __icon__ = default(string);
            var __icon__b__ = false;
            var __shortForecast__ = default(string);
            var __shortForecast__b__ = false;
            var __detailedForecast__ = default(string);
            var __detailedForecast__b__ = false;

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
                        __number__ = reader.ReadInt32();
                        __number__b__ = true;
                        break;
                    case 1:
                        __name__ = reader.ReadString();
                        __name__b__ = true;
                        break;
                    case 2:
                        __startTime__ = formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset>().Deserialize(ref reader, formatterResolver);
                        __startTime__b__ = true;
                        break;
                    case 3:
                        __endTime__ = formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset>().Deserialize(ref reader, formatterResolver);
                        __endTime__b__ = true;
                        break;
                    case 4:
                        __isDaytime__ = reader.ReadBoolean();
                        __isDaytime__b__ = true;
                        break;
                    case 5:
                        __temperature__ = reader.ReadInt32();
                        __temperature__b__ = true;
                        break;
                    case 6:
                        __temperatureUnit__ = reader.ReadString();
                        __temperatureUnit__b__ = true;
                        break;
                    case 7:
                        __temperatureTrend__ = reader.ReadString();
                        __temperatureTrend__b__ = true;
                        break;
                    case 8:
                        __windSpeed__ = reader.ReadString();
                        __windSpeed__b__ = true;
                        break;
                    case 9:
                        __windDirection__ = reader.ReadString();
                        __windDirection__b__ = true;
                        break;
                    case 10:
                        __icon__ = reader.ReadString();
                        __icon__b__ = true;
                        break;
                    case 11:
                        __shortForecast__ = reader.ReadString();
                        __shortForecast__b__ = true;
                        break;
                    case 12:
                        __detailedForecast__ = reader.ReadString();
                        __detailedForecast__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.NWS.Period();
            if(__number__b__) ____result.number = __number__;
            if(__name__b__) ____result.name = __name__;
            if(__startTime__b__) ____result.startTime = __startTime__;
            if(__endTime__b__) ____result.endTime = __endTime__;
            if(__isDaytime__b__) ____result.isDaytime = __isDaytime__;
            if(__temperature__b__) ____result.temperature = __temperature__;
            if(__temperatureUnit__b__) ____result.temperatureUnit = __temperatureUnit__;
            if(__temperatureTrend__b__) ____result.temperatureTrend = __temperatureTrend__;
            if(__windSpeed__b__) ____result.windSpeed = __windSpeed__;
            if(__windDirection__b__) ____result.windDirection = __windDirection__;
            if(__icon__b__) ____result.icon = __icon__;
            if(__shortForecast__b__) ____result.shortForecast = __shortForecast__;
            if(__detailedForecast__b__) ____result.detailedForecast = __detailedForecast__;

            return ____result;
        }
    }


    public sealed class ForecastRootobjectFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.NWS.ForecastRootobject>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public ForecastRootobjectFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("updated"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("units"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("generatedAt"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("updateTime"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("validTimes"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("periods"), 5},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("updated"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("units"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("generatedAt"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("updateTime"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("validTimes"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("periods"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.NWS.ForecastRootobject value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset>().Serialize(ref writer, value.updated, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.units);
            writer.WriteRaw(this.____stringByteKeys[2]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset>().Serialize(ref writer, value.generatedAt, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[3]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset>().Serialize(ref writer, value.updateTime, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteString(value.validTimes);
            writer.WriteRaw(this.____stringByteKeys[5]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.NWS.Period[]>().Serialize(ref writer, value.periods, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.NWS.ForecastRootobject Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __updated__ = default(global::System.DateTimeOffset);
            var __updated__b__ = false;
            var __units__ = default(string);
            var __units__b__ = false;
            var __generatedAt__ = default(global::System.DateTimeOffset);
            var __generatedAt__b__ = false;
            var __updateTime__ = default(global::System.DateTimeOffset);
            var __updateTime__b__ = false;
            var __validTimes__ = default(string);
            var __validTimes__b__ = false;
            var __periods__ = default(global::SimpleWeather.NWS.Period[]);
            var __periods__b__ = false;

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
                        __updated__ = formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset>().Deserialize(ref reader, formatterResolver);
                        __updated__b__ = true;
                        break;
                    case 1:
                        __units__ = reader.ReadString();
                        __units__b__ = true;
                        break;
                    case 2:
                        __generatedAt__ = formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset>().Deserialize(ref reader, formatterResolver);
                        __generatedAt__b__ = true;
                        break;
                    case 3:
                        __updateTime__ = formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset>().Deserialize(ref reader, formatterResolver);
                        __updateTime__b__ = true;
                        break;
                    case 4:
                        __validTimes__ = reader.ReadString();
                        __validTimes__b__ = true;
                        break;
                    case 5:
                        __periods__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.NWS.Period[]>().Deserialize(ref reader, formatterResolver);
                        __periods__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.NWS.ForecastRootobject();
            if(__updated__b__) ____result.updated = __updated__;
            if(__units__b__) ____result.units = __units__;
            if(__generatedAt__b__) ____result.generatedAt = __generatedAt__;
            if(__updateTime__b__) ____result.updateTime = __updateTime__;
            if(__validTimes__b__) ____result.validTimes = __validTimes__;
            if(__periods__b__) ____result.periods = __periods__;

            return ____result;
        }
    }


    public sealed class ObservationsStationsRootobjectFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.NWS.ObservationsStationsRootobject>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public ObservationsStationsRootobjectFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("observationStations"), 0},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("observationStations"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.NWS.ObservationsStationsRootobject value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<string[]>().Serialize(ref writer, value.observationStations, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.NWS.ObservationsStationsRootobject Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __observationStations__ = default(string[]);
            var __observationStations__b__ = false;

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
                        __observationStations__ = formatterResolver.GetFormatterWithVerify<string[]>().Deserialize(ref reader, formatterResolver);
                        __observationStations__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.NWS.ObservationsStationsRootobject();
            if(__observationStations__b__) ____result.observationStations = __observationStations__;

            return ____result;
        }
    }


    public sealed class TemperatureFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.NWS.Temperature>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public TemperatureFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("value"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("unitCode"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("qualityControl"), 2},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("value"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("unitCode"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("qualityControl"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.NWS.Temperature value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.value, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.unitCode);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.qualityControl);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.NWS.Temperature Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __value__ = default(float?);
            var __value__b__ = false;
            var __unitCode__ = default(string);
            var __unitCode__b__ = false;
            var __qualityControl__ = default(string);
            var __qualityControl__b__ = false;

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
                        __value__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __value__b__ = true;
                        break;
                    case 1:
                        __unitCode__ = reader.ReadString();
                        __unitCode__b__ = true;
                        break;
                    case 2:
                        __qualityControl__ = reader.ReadString();
                        __qualityControl__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.NWS.Temperature();
            if(__value__b__) ____result.value = __value__;
            if(__unitCode__b__) ____result.unitCode = __unitCode__;
            if(__qualityControl__b__) ____result.qualityControl = __qualityControl__;

            return ____result;
        }
    }


    public sealed class DewpointFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.NWS.Dewpoint>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public DewpointFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("value"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("unitCode"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("qualityControl"), 2},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("value"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("unitCode"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("qualityControl"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.NWS.Dewpoint value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.value, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.unitCode);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.qualityControl);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.NWS.Dewpoint Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __value__ = default(float?);
            var __value__b__ = false;
            var __unitCode__ = default(string);
            var __unitCode__b__ = false;
            var __qualityControl__ = default(string);
            var __qualityControl__b__ = false;

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
                        __value__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __value__b__ = true;
                        break;
                    case 1:
                        __unitCode__ = reader.ReadString();
                        __unitCode__b__ = true;
                        break;
                    case 2:
                        __qualityControl__ = reader.ReadString();
                        __qualityControl__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.NWS.Dewpoint();
            if(__value__b__) ____result.value = __value__;
            if(__unitCode__b__) ____result.unitCode = __unitCode__;
            if(__qualityControl__b__) ____result.qualityControl = __qualityControl__;

            return ____result;
        }
    }


    public sealed class WinddirectionFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.NWS.Winddirection>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public WinddirectionFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("value"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("unitCode"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("qualityControl"), 2},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("value"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("unitCode"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("qualityControl"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.NWS.Winddirection value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.value, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.unitCode);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.qualityControl);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.NWS.Winddirection Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __value__ = default(float?);
            var __value__b__ = false;
            var __unitCode__ = default(string);
            var __unitCode__b__ = false;
            var __qualityControl__ = default(string);
            var __qualityControl__b__ = false;

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
                        __value__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __value__b__ = true;
                        break;
                    case 1:
                        __unitCode__ = reader.ReadString();
                        __unitCode__b__ = true;
                        break;
                    case 2:
                        __qualityControl__ = reader.ReadString();
                        __qualityControl__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.NWS.Winddirection();
            if(__value__b__) ____result.value = __value__;
            if(__unitCode__b__) ____result.unitCode = __unitCode__;
            if(__qualityControl__b__) ____result.qualityControl = __qualityControl__;

            return ____result;
        }
    }


    public sealed class WindspeedFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.NWS.Windspeed>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public WindspeedFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("value"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("unitCode"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("qualityControl"), 2},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("value"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("unitCode"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("qualityControl"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.NWS.Windspeed value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.value, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.unitCode);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.qualityControl);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.NWS.Windspeed Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __value__ = default(float?);
            var __value__b__ = false;
            var __unitCode__ = default(string);
            var __unitCode__b__ = false;
            var __qualityControl__ = default(string);
            var __qualityControl__b__ = false;

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
                        __value__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __value__b__ = true;
                        break;
                    case 1:
                        __unitCode__ = reader.ReadString();
                        __unitCode__b__ = true;
                        break;
                    case 2:
                        __qualityControl__ = reader.ReadString();
                        __qualityControl__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.NWS.Windspeed();
            if(__value__b__) ____result.value = __value__;
            if(__unitCode__b__) ____result.unitCode = __unitCode__;
            if(__qualityControl__b__) ____result.qualityControl = __qualityControl__;

            return ____result;
        }
    }


    public sealed class BarometricpressureFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.NWS.Barometricpressure>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public BarometricpressureFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("value"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("unitCode"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("qualityControl"), 2},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("value"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("unitCode"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("qualityControl"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.NWS.Barometricpressure value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.value, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.unitCode);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.qualityControl);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.NWS.Barometricpressure Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __value__ = default(float?);
            var __value__b__ = false;
            var __unitCode__ = default(string);
            var __unitCode__b__ = false;
            var __qualityControl__ = default(string);
            var __qualityControl__b__ = false;

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
                        __value__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __value__b__ = true;
                        break;
                    case 1:
                        __unitCode__ = reader.ReadString();
                        __unitCode__b__ = true;
                        break;
                    case 2:
                        __qualityControl__ = reader.ReadString();
                        __qualityControl__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.NWS.Barometricpressure();
            if(__value__b__) ____result.value = __value__;
            if(__unitCode__b__) ____result.unitCode = __unitCode__;
            if(__qualityControl__b__) ____result.qualityControl = __qualityControl__;

            return ____result;
        }
    }


    public sealed class VisibilityFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.NWS.Visibility>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public VisibilityFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("value"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("unitCode"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("qualityControl"), 2},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("value"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("unitCode"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("qualityControl"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.NWS.Visibility value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.value, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.unitCode);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.qualityControl);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.NWS.Visibility Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __value__ = default(float?);
            var __value__b__ = false;
            var __unitCode__ = default(string);
            var __unitCode__b__ = false;
            var __qualityControl__ = default(string);
            var __qualityControl__b__ = false;

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
                        __value__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __value__b__ = true;
                        break;
                    case 1:
                        __unitCode__ = reader.ReadString();
                        __unitCode__b__ = true;
                        break;
                    case 2:
                        __qualityControl__ = reader.ReadString();
                        __qualityControl__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.NWS.Visibility();
            if(__value__b__) ____result.value = __value__;
            if(__unitCode__b__) ____result.unitCode = __unitCode__;
            if(__qualityControl__b__) ____result.qualityControl = __qualityControl__;

            return ____result;
        }
    }


    public sealed class RelativehumidityFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.NWS.Relativehumidity>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public RelativehumidityFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("value"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("unitCode"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("qualityControl"), 2},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("value"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("unitCode"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("qualityControl"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.NWS.Relativehumidity value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.value, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.unitCode);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.qualityControl);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.NWS.Relativehumidity Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __value__ = default(float?);
            var __value__b__ = false;
            var __unitCode__ = default(string);
            var __unitCode__b__ = false;
            var __qualityControl__ = default(string);
            var __qualityControl__b__ = false;

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
                        __value__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __value__b__ = true;
                        break;
                    case 1:
                        __unitCode__ = reader.ReadString();
                        __unitCode__b__ = true;
                        break;
                    case 2:
                        __qualityControl__ = reader.ReadString();
                        __qualityControl__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.NWS.Relativehumidity();
            if(__value__b__) ____result.value = __value__;
            if(__unitCode__b__) ____result.unitCode = __unitCode__;
            if(__qualityControl__b__) ____result.qualityControl = __qualityControl__;

            return ____result;
        }
    }


    public sealed class ObservationsCurrentRootobjectFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.NWS.ObservationsCurrentRootobject>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public ObservationsCurrentRootobjectFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("timestamp"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("textDescription"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("icon"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("temperature"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("dewpoint"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("windDirection"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("windSpeed"), 6},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("barometricPressure"), 7},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("visibility"), 8},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("relativeHumidity"), 9},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("timestamp"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("textDescription"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("icon"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("temperature"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("dewpoint"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("windDirection"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("windSpeed"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("barometricPressure"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("visibility"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("relativeHumidity"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.NWS.ObservationsCurrentRootobject value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset>().Serialize(ref writer, value.timestamp, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.textDescription);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.icon);
            writer.WriteRaw(this.____stringByteKeys[3]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.NWS.Temperature>().Serialize(ref writer, value.temperature, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[4]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.NWS.Dewpoint>().Serialize(ref writer, value.dewpoint, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[5]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.NWS.Winddirection>().Serialize(ref writer, value.windDirection, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[6]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.NWS.Windspeed>().Serialize(ref writer, value.windSpeed, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[7]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.NWS.Barometricpressure>().Serialize(ref writer, value.barometricPressure, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[8]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.NWS.Visibility>().Serialize(ref writer, value.visibility, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[9]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.NWS.Relativehumidity>().Serialize(ref writer, value.relativeHumidity, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.NWS.ObservationsCurrentRootobject Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __timestamp__ = default(global::System.DateTimeOffset);
            var __timestamp__b__ = false;
            var __textDescription__ = default(string);
            var __textDescription__b__ = false;
            var __icon__ = default(string);
            var __icon__b__ = false;
            var __temperature__ = default(global::SimpleWeather.NWS.Temperature);
            var __temperature__b__ = false;
            var __dewpoint__ = default(global::SimpleWeather.NWS.Dewpoint);
            var __dewpoint__b__ = false;
            var __windDirection__ = default(global::SimpleWeather.NWS.Winddirection);
            var __windDirection__b__ = false;
            var __windSpeed__ = default(global::SimpleWeather.NWS.Windspeed);
            var __windSpeed__b__ = false;
            var __barometricPressure__ = default(global::SimpleWeather.NWS.Barometricpressure);
            var __barometricPressure__b__ = false;
            var __visibility__ = default(global::SimpleWeather.NWS.Visibility);
            var __visibility__b__ = false;
            var __relativeHumidity__ = default(global::SimpleWeather.NWS.Relativehumidity);
            var __relativeHumidity__b__ = false;

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
                        __timestamp__ = formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset>().Deserialize(ref reader, formatterResolver);
                        __timestamp__b__ = true;
                        break;
                    case 1:
                        __textDescription__ = reader.ReadString();
                        __textDescription__b__ = true;
                        break;
                    case 2:
                        __icon__ = reader.ReadString();
                        __icon__b__ = true;
                        break;
                    case 3:
                        __temperature__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.NWS.Temperature>().Deserialize(ref reader, formatterResolver);
                        __temperature__b__ = true;
                        break;
                    case 4:
                        __dewpoint__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.NWS.Dewpoint>().Deserialize(ref reader, formatterResolver);
                        __dewpoint__b__ = true;
                        break;
                    case 5:
                        __windDirection__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.NWS.Winddirection>().Deserialize(ref reader, formatterResolver);
                        __windDirection__b__ = true;
                        break;
                    case 6:
                        __windSpeed__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.NWS.Windspeed>().Deserialize(ref reader, formatterResolver);
                        __windSpeed__b__ = true;
                        break;
                    case 7:
                        __barometricPressure__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.NWS.Barometricpressure>().Deserialize(ref reader, formatterResolver);
                        __barometricPressure__b__ = true;
                        break;
                    case 8:
                        __visibility__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.NWS.Visibility>().Deserialize(ref reader, formatterResolver);
                        __visibility__b__ = true;
                        break;
                    case 9:
                        __relativeHumidity__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.NWS.Relativehumidity>().Deserialize(ref reader, formatterResolver);
                        __relativeHumidity__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.NWS.ObservationsCurrentRootobject();
            if(__timestamp__b__) ____result.timestamp = __timestamp__;
            if(__textDescription__b__) ____result.textDescription = __textDescription__;
            if(__icon__b__) ____result.icon = __icon__;
            if(__temperature__b__) ____result.temperature = __temperature__;
            if(__dewpoint__b__) ____result.dewpoint = __dewpoint__;
            if(__windDirection__b__) ____result.windDirection = __windDirection__;
            if(__windSpeed__b__) ____result.windSpeed = __windSpeed__;
            if(__barometricPressure__b__) ____result.barometricPressure = __barometricPressure__;
            if(__visibility__b__) ____result.visibility = __visibility__;
            if(__relativeHumidity__b__) ____result.relativeHumidity = __relativeHumidity__;

            return ____result;
        }
    }

}

#pragma warning disable 168
#pragma warning restore 219
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612
#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 219
#pragma warning disable 168

namespace SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.OpenWeather
{
    using System;
    using Utf8Json;


    public sealed class RainFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.OpenWeather.Rain>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public RainFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("_1h"), 0},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("_1h"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.OpenWeather.Rain value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteSingle(value._1h);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.OpenWeather.Rain Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var ___1h__ = default(float);
            var ___1h__b__ = false;

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
                        ___1h__ = reader.ReadSingle();
                        ___1h__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.OpenWeather.Rain();
            if(___1h__b__) ____result._1h = ___1h__;

            return ____result;
        }
    }


    public sealed class SnowFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.OpenWeather.Snow>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public SnowFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("_1h"), 0},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("_1h"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.OpenWeather.Snow value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteSingle(value._1h);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.OpenWeather.Snow Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var ___1h__ = default(float);
            var ___1h__b__ = false;

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
                        ___1h__ = reader.ReadSingle();
                        ___1h__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.OpenWeather.Snow();
            if(___1h__b__) ____result._1h = ___1h__;

            return ____result;
        }
    }


    public sealed class WeatherFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.OpenWeather.Weather>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public WeatherFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("id"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("main"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("description"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("icon"), 3},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("id"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("main"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("description"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("icon"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.OpenWeather.Weather value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteInt32(value.id);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.main);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.description);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteString(value.icon);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.OpenWeather.Weather Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __id__ = default(int);
            var __id__b__ = false;
            var __main__ = default(string);
            var __main__b__ = false;
            var __description__ = default(string);
            var __description__b__ = false;
            var __icon__ = default(string);
            var __icon__b__ = false;

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
                        __id__ = reader.ReadInt32();
                        __id__b__ = true;
                        break;
                    case 1:
                        __main__ = reader.ReadString();
                        __main__b__ = true;
                        break;
                    case 2:
                        __description__ = reader.ReadString();
                        __description__b__ = true;
                        break;
                    case 3:
                        __icon__ = reader.ReadString();
                        __icon__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.OpenWeather.Weather();
            if(__id__b__) ____result.id = __id__;
            if(__main__b__) ____result.main = __main__;
            if(__description__b__) ____result.description = __description__;
            if(__icon__b__) ____result.icon = __icon__;

            return ____result;
        }
    }


    public sealed class CurrentFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.OpenWeather.Current>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public CurrentFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("dt"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("sunrise"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("sunset"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("temp"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("feels_like"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("pressure"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("humidity"), 6},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("dew_point"), 7},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("clouds"), 8},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("uvi"), 9},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("visibility"), 10},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("wind_speed"), 11},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("wind_deg"), 12},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("rain"), 13},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("snow"), 14},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("weather"), 15},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("dt"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("sunrise"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("sunset"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("temp"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("feels_like"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("pressure"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("humidity"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("dew_point"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("clouds"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("uvi"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("visibility"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("wind_speed"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("wind_deg"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("rain"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("snow"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("weather"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.OpenWeather.Current value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteInt64(value.dt);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteInt64(value.sunrise);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteInt64(value.sunset);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteSingle(value.temp);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteSingle(value.feels_like);
            writer.WriteRaw(this.____stringByteKeys[5]);
            writer.WriteSingle(value.pressure);
            writer.WriteRaw(this.____stringByteKeys[6]);
            writer.WriteInt32(value.humidity);
            writer.WriteRaw(this.____stringByteKeys[7]);
            writer.WriteSingle(value.dew_point);
            writer.WriteRaw(this.____stringByteKeys[8]);
            writer.WriteInt32(value.clouds);
            writer.WriteRaw(this.____stringByteKeys[9]);
            writer.WriteSingle(value.uvi);
            writer.WriteRaw(this.____stringByteKeys[10]);
            writer.WriteInt32(value.visibility);
            writer.WriteRaw(this.____stringByteKeys[11]);
            writer.WriteSingle(value.wind_speed);
            writer.WriteRaw(this.____stringByteKeys[12]);
            writer.WriteInt32(value.wind_deg);
            writer.WriteRaw(this.____stringByteKeys[13]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Rain>().Serialize(ref writer, value.rain, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[14]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Snow>().Serialize(ref writer, value.snow, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[15]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Weather[]>().Serialize(ref writer, value.weather, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.OpenWeather.Current Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __dt__ = default(long);
            var __dt__b__ = false;
            var __sunrise__ = default(long);
            var __sunrise__b__ = false;
            var __sunset__ = default(long);
            var __sunset__b__ = false;
            var __temp__ = default(float);
            var __temp__b__ = false;
            var __feels_like__ = default(float);
            var __feels_like__b__ = false;
            var __pressure__ = default(float);
            var __pressure__b__ = false;
            var __humidity__ = default(int);
            var __humidity__b__ = false;
            var __dew_point__ = default(float);
            var __dew_point__b__ = false;
            var __clouds__ = default(int);
            var __clouds__b__ = false;
            var __uvi__ = default(float);
            var __uvi__b__ = false;
            var __visibility__ = default(int);
            var __visibility__b__ = false;
            var __wind_speed__ = default(float);
            var __wind_speed__b__ = false;
            var __wind_deg__ = default(int);
            var __wind_deg__b__ = false;
            var __rain__ = default(global::SimpleWeather.OpenWeather.Rain);
            var __rain__b__ = false;
            var __snow__ = default(global::SimpleWeather.OpenWeather.Snow);
            var __snow__b__ = false;
            var __weather__ = default(global::SimpleWeather.OpenWeather.Weather[]);
            var __weather__b__ = false;

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
                        __dt__ = reader.ReadInt64();
                        __dt__b__ = true;
                        break;
                    case 1:
                        __sunrise__ = reader.ReadInt64();
                        __sunrise__b__ = true;
                        break;
                    case 2:
                        __sunset__ = reader.ReadInt64();
                        __sunset__b__ = true;
                        break;
                    case 3:
                        __temp__ = reader.ReadSingle();
                        __temp__b__ = true;
                        break;
                    case 4:
                        __feels_like__ = reader.ReadSingle();
                        __feels_like__b__ = true;
                        break;
                    case 5:
                        __pressure__ = reader.ReadSingle();
                        __pressure__b__ = true;
                        break;
                    case 6:
                        __humidity__ = reader.ReadInt32();
                        __humidity__b__ = true;
                        break;
                    case 7:
                        __dew_point__ = reader.ReadSingle();
                        __dew_point__b__ = true;
                        break;
                    case 8:
                        __clouds__ = reader.ReadInt32();
                        __clouds__b__ = true;
                        break;
                    case 9:
                        __uvi__ = reader.ReadSingle();
                        __uvi__b__ = true;
                        break;
                    case 10:
                        __visibility__ = reader.ReadInt32();
                        __visibility__b__ = true;
                        break;
                    case 11:
                        __wind_speed__ = reader.ReadSingle();
                        __wind_speed__b__ = true;
                        break;
                    case 12:
                        __wind_deg__ = reader.ReadInt32();
                        __wind_deg__b__ = true;
                        break;
                    case 13:
                        __rain__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Rain>().Deserialize(ref reader, formatterResolver);
                        __rain__b__ = true;
                        break;
                    case 14:
                        __snow__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Snow>().Deserialize(ref reader, formatterResolver);
                        __snow__b__ = true;
                        break;
                    case 15:
                        __weather__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Weather[]>().Deserialize(ref reader, formatterResolver);
                        __weather__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.OpenWeather.Current();
            if(__dt__b__) ____result.dt = __dt__;
            if(__sunrise__b__) ____result.sunrise = __sunrise__;
            if(__sunset__b__) ____result.sunset = __sunset__;
            if(__temp__b__) ____result.temp = __temp__;
            if(__feels_like__b__) ____result.feels_like = __feels_like__;
            if(__pressure__b__) ____result.pressure = __pressure__;
            if(__humidity__b__) ____result.humidity = __humidity__;
            if(__dew_point__b__) ____result.dew_point = __dew_point__;
            if(__clouds__b__) ____result.clouds = __clouds__;
            if(__uvi__b__) ____result.uvi = __uvi__;
            if(__visibility__b__) ____result.visibility = __visibility__;
            if(__wind_speed__b__) ____result.wind_speed = __wind_speed__;
            if(__wind_deg__b__) ____result.wind_deg = __wind_deg__;
            if(__rain__b__) ____result.rain = __rain__;
            if(__snow__b__) ____result.snow = __snow__;
            if(__weather__b__) ____result.weather = __weather__;

            return ____result;
        }
    }


    public sealed class HourlyFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.OpenWeather.Hourly>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public HourlyFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("dt"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("temp"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("feels_like"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("pressure"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("humidity"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("dew_point"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("clouds"), 6},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("visibility"), 7},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("wind_speed"), 8},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("wind_deg"), 9},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("rain"), 10},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("snow"), 11},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("weather"), 12},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("dt"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("temp"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("feels_like"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("pressure"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("humidity"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("dew_point"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("clouds"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("visibility"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("wind_speed"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("wind_deg"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("rain"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("snow"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("weather"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.OpenWeather.Hourly value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteInt64(value.dt);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteSingle(value.temp);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteSingle(value.feels_like);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteSingle(value.pressure);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteInt32(value.humidity);
            writer.WriteRaw(this.____stringByteKeys[5]);
            writer.WriteSingle(value.dew_point);
            writer.WriteRaw(this.____stringByteKeys[6]);
            writer.WriteInt32(value.clouds);
            writer.WriteRaw(this.____stringByteKeys[7]);
            formatterResolver.GetFormatterWithVerify<int?>().Serialize(ref writer, value.visibility, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[8]);
            writer.WriteSingle(value.wind_speed);
            writer.WriteRaw(this.____stringByteKeys[9]);
            writer.WriteInt32(value.wind_deg);
            writer.WriteRaw(this.____stringByteKeys[10]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Rain>().Serialize(ref writer, value.rain, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[11]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Snow>().Serialize(ref writer, value.snow, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[12]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Weather[]>().Serialize(ref writer, value.weather, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.OpenWeather.Hourly Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __dt__ = default(long);
            var __dt__b__ = false;
            var __temp__ = default(float);
            var __temp__b__ = false;
            var __feels_like__ = default(float);
            var __feels_like__b__ = false;
            var __pressure__ = default(float);
            var __pressure__b__ = false;
            var __humidity__ = default(int);
            var __humidity__b__ = false;
            var __dew_point__ = default(float);
            var __dew_point__b__ = false;
            var __clouds__ = default(int);
            var __clouds__b__ = false;
            var __visibility__ = default(int?);
            var __visibility__b__ = false;
            var __wind_speed__ = default(float);
            var __wind_speed__b__ = false;
            var __wind_deg__ = default(int);
            var __wind_deg__b__ = false;
            var __rain__ = default(global::SimpleWeather.OpenWeather.Rain);
            var __rain__b__ = false;
            var __snow__ = default(global::SimpleWeather.OpenWeather.Snow);
            var __snow__b__ = false;
            var __weather__ = default(global::SimpleWeather.OpenWeather.Weather[]);
            var __weather__b__ = false;

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
                        __dt__ = reader.ReadInt64();
                        __dt__b__ = true;
                        break;
                    case 1:
                        __temp__ = reader.ReadSingle();
                        __temp__b__ = true;
                        break;
                    case 2:
                        __feels_like__ = reader.ReadSingle();
                        __feels_like__b__ = true;
                        break;
                    case 3:
                        __pressure__ = reader.ReadSingle();
                        __pressure__b__ = true;
                        break;
                    case 4:
                        __humidity__ = reader.ReadInt32();
                        __humidity__b__ = true;
                        break;
                    case 5:
                        __dew_point__ = reader.ReadSingle();
                        __dew_point__b__ = true;
                        break;
                    case 6:
                        __clouds__ = reader.ReadInt32();
                        __clouds__b__ = true;
                        break;
                    case 7:
                        __visibility__ = formatterResolver.GetFormatterWithVerify<int?>().Deserialize(ref reader, formatterResolver);
                        __visibility__b__ = true;
                        break;
                    case 8:
                        __wind_speed__ = reader.ReadSingle();
                        __wind_speed__b__ = true;
                        break;
                    case 9:
                        __wind_deg__ = reader.ReadInt32();
                        __wind_deg__b__ = true;
                        break;
                    case 10:
                        __rain__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Rain>().Deserialize(ref reader, formatterResolver);
                        __rain__b__ = true;
                        break;
                    case 11:
                        __snow__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Snow>().Deserialize(ref reader, formatterResolver);
                        __snow__b__ = true;
                        break;
                    case 12:
                        __weather__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Weather[]>().Deserialize(ref reader, formatterResolver);
                        __weather__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.OpenWeather.Hourly();
            if(__dt__b__) ____result.dt = __dt__;
            if(__temp__b__) ____result.temp = __temp__;
            if(__feels_like__b__) ____result.feels_like = __feels_like__;
            if(__pressure__b__) ____result.pressure = __pressure__;
            if(__humidity__b__) ____result.humidity = __humidity__;
            if(__dew_point__b__) ____result.dew_point = __dew_point__;
            if(__clouds__b__) ____result.clouds = __clouds__;
            if(__visibility__b__) ____result.visibility = __visibility__;
            if(__wind_speed__b__) ____result.wind_speed = __wind_speed__;
            if(__wind_deg__b__) ____result.wind_deg = __wind_deg__;
            if(__rain__b__) ____result.rain = __rain__;
            if(__snow__b__) ____result.snow = __snow__;
            if(__weather__b__) ____result.weather = __weather__;

            return ____result;
        }
    }


    public sealed class TempFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.OpenWeather.Temp>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public TempFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("morn"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("day"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("eve"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("night"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("min"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("max"), 5},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("morn"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("day"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("eve"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("night"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("min"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("max"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.OpenWeather.Temp value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteSingle(value.morn);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteSingle(value.day);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteSingle(value.eve);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteSingle(value.night);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteSingle(value.min);
            writer.WriteRaw(this.____stringByteKeys[5]);
            writer.WriteSingle(value.max);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.OpenWeather.Temp Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __morn__ = default(float);
            var __morn__b__ = false;
            var __day__ = default(float);
            var __day__b__ = false;
            var __eve__ = default(float);
            var __eve__b__ = false;
            var __night__ = default(float);
            var __night__b__ = false;
            var __min__ = default(float);
            var __min__b__ = false;
            var __max__ = default(float);
            var __max__b__ = false;

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
                        __morn__ = reader.ReadSingle();
                        __morn__b__ = true;
                        break;
                    case 1:
                        __day__ = reader.ReadSingle();
                        __day__b__ = true;
                        break;
                    case 2:
                        __eve__ = reader.ReadSingle();
                        __eve__b__ = true;
                        break;
                    case 3:
                        __night__ = reader.ReadSingle();
                        __night__b__ = true;
                        break;
                    case 4:
                        __min__ = reader.ReadSingle();
                        __min__b__ = true;
                        break;
                    case 5:
                        __max__ = reader.ReadSingle();
                        __max__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.OpenWeather.Temp();
            if(__morn__b__) ____result.morn = __morn__;
            if(__day__b__) ____result.day = __day__;
            if(__eve__b__) ____result.eve = __eve__;
            if(__night__b__) ____result.night = __night__;
            if(__min__b__) ____result.min = __min__;
            if(__max__b__) ____result.max = __max__;

            return ____result;
        }
    }


    public sealed class Feels_LikeFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.OpenWeather.Feels_Like>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public Feels_LikeFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("morn"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("day"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("eve"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("night"), 3},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("morn"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("day"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("eve"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("night"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.OpenWeather.Feels_Like value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteSingle(value.morn);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteSingle(value.day);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteSingle(value.eve);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteSingle(value.night);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.OpenWeather.Feels_Like Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __morn__ = default(float);
            var __morn__b__ = false;
            var __day__ = default(float);
            var __day__b__ = false;
            var __eve__ = default(float);
            var __eve__b__ = false;
            var __night__ = default(float);
            var __night__b__ = false;

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
                        __morn__ = reader.ReadSingle();
                        __morn__b__ = true;
                        break;
                    case 1:
                        __day__ = reader.ReadSingle();
                        __day__b__ = true;
                        break;
                    case 2:
                        __eve__ = reader.ReadSingle();
                        __eve__b__ = true;
                        break;
                    case 3:
                        __night__ = reader.ReadSingle();
                        __night__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.OpenWeather.Feels_Like();
            if(__morn__b__) ____result.morn = __morn__;
            if(__day__b__) ____result.day = __day__;
            if(__eve__b__) ____result.eve = __eve__;
            if(__night__b__) ____result.night = __night__;

            return ____result;
        }
    }


    public sealed class DailyFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.OpenWeather.Daily>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public DailyFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("dt"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("sunrise"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("sunset"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("temp"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("feels_like"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("pressure"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("humidity"), 6},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("dew_point"), 7},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("wind_speed"), 8},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("wind_deg"), 9},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("clouds"), 10},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("uvi"), 11},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("visibility"), 12},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("rain"), 13},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("snow"), 14},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("weather"), 15},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("dt"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("sunrise"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("sunset"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("temp"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("feels_like"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("pressure"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("humidity"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("dew_point"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("wind_speed"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("wind_deg"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("clouds"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("uvi"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("visibility"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("rain"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("snow"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("weather"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.OpenWeather.Daily value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteInt64(value.dt);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteInt32(value.sunrise);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteInt32(value.sunset);
            writer.WriteRaw(this.____stringByteKeys[3]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Temp>().Serialize(ref writer, value.temp, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[4]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Feels_Like>().Serialize(ref writer, value.feels_like, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[5]);
            writer.WriteSingle(value.pressure);
            writer.WriteRaw(this.____stringByteKeys[6]);
            writer.WriteInt32(value.humidity);
            writer.WriteRaw(this.____stringByteKeys[7]);
            writer.WriteSingle(value.dew_point);
            writer.WriteRaw(this.____stringByteKeys[8]);
            writer.WriteSingle(value.wind_speed);
            writer.WriteRaw(this.____stringByteKeys[9]);
            writer.WriteInt32(value.wind_deg);
            writer.WriteRaw(this.____stringByteKeys[10]);
            writer.WriteInt32(value.clouds);
            writer.WriteRaw(this.____stringByteKeys[11]);
            writer.WriteSingle(value.uvi);
            writer.WriteRaw(this.____stringByteKeys[12]);
            formatterResolver.GetFormatterWithVerify<int?>().Serialize(ref writer, value.visibility, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[13]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.rain, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[14]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.snow, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[15]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Weather[]>().Serialize(ref writer, value.weather, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.OpenWeather.Daily Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __dt__ = default(long);
            var __dt__b__ = false;
            var __sunrise__ = default(int);
            var __sunrise__b__ = false;
            var __sunset__ = default(int);
            var __sunset__b__ = false;
            var __temp__ = default(global::SimpleWeather.OpenWeather.Temp);
            var __temp__b__ = false;
            var __feels_like__ = default(global::SimpleWeather.OpenWeather.Feels_Like);
            var __feels_like__b__ = false;
            var __pressure__ = default(float);
            var __pressure__b__ = false;
            var __humidity__ = default(int);
            var __humidity__b__ = false;
            var __dew_point__ = default(float);
            var __dew_point__b__ = false;
            var __wind_speed__ = default(float);
            var __wind_speed__b__ = false;
            var __wind_deg__ = default(int);
            var __wind_deg__b__ = false;
            var __clouds__ = default(int);
            var __clouds__b__ = false;
            var __uvi__ = default(float);
            var __uvi__b__ = false;
            var __visibility__ = default(int?);
            var __visibility__b__ = false;
            var __rain__ = default(float?);
            var __rain__b__ = false;
            var __snow__ = default(float?);
            var __snow__b__ = false;
            var __weather__ = default(global::SimpleWeather.OpenWeather.Weather[]);
            var __weather__b__ = false;

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
                        __dt__ = reader.ReadInt64();
                        __dt__b__ = true;
                        break;
                    case 1:
                        __sunrise__ = reader.ReadInt32();
                        __sunrise__b__ = true;
                        break;
                    case 2:
                        __sunset__ = reader.ReadInt32();
                        __sunset__b__ = true;
                        break;
                    case 3:
                        __temp__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Temp>().Deserialize(ref reader, formatterResolver);
                        __temp__b__ = true;
                        break;
                    case 4:
                        __feels_like__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Feels_Like>().Deserialize(ref reader, formatterResolver);
                        __feels_like__b__ = true;
                        break;
                    case 5:
                        __pressure__ = reader.ReadSingle();
                        __pressure__b__ = true;
                        break;
                    case 6:
                        __humidity__ = reader.ReadInt32();
                        __humidity__b__ = true;
                        break;
                    case 7:
                        __dew_point__ = reader.ReadSingle();
                        __dew_point__b__ = true;
                        break;
                    case 8:
                        __wind_speed__ = reader.ReadSingle();
                        __wind_speed__b__ = true;
                        break;
                    case 9:
                        __wind_deg__ = reader.ReadInt32();
                        __wind_deg__b__ = true;
                        break;
                    case 10:
                        __clouds__ = reader.ReadInt32();
                        __clouds__b__ = true;
                        break;
                    case 11:
                        __uvi__ = reader.ReadSingle();
                        __uvi__b__ = true;
                        break;
                    case 12:
                        __visibility__ = formatterResolver.GetFormatterWithVerify<int?>().Deserialize(ref reader, formatterResolver);
                        __visibility__b__ = true;
                        break;
                    case 13:
                        __rain__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __rain__b__ = true;
                        break;
                    case 14:
                        __snow__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __snow__b__ = true;
                        break;
                    case 15:
                        __weather__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Weather[]>().Deserialize(ref reader, formatterResolver);
                        __weather__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.OpenWeather.Daily();
            if(__dt__b__) ____result.dt = __dt__;
            if(__sunrise__b__) ____result.sunrise = __sunrise__;
            if(__sunset__b__) ____result.sunset = __sunset__;
            if(__temp__b__) ____result.temp = __temp__;
            if(__feels_like__b__) ____result.feels_like = __feels_like__;
            if(__pressure__b__) ____result.pressure = __pressure__;
            if(__humidity__b__) ____result.humidity = __humidity__;
            if(__dew_point__b__) ____result.dew_point = __dew_point__;
            if(__wind_speed__b__) ____result.wind_speed = __wind_speed__;
            if(__wind_deg__b__) ____result.wind_deg = __wind_deg__;
            if(__clouds__b__) ____result.clouds = __clouds__;
            if(__uvi__b__) ____result.uvi = __uvi__;
            if(__visibility__b__) ____result.visibility = __visibility__;
            if(__rain__b__) ____result.rain = __rain__;
            if(__snow__b__) ____result.snow = __snow__;
            if(__weather__b__) ____result.weather = __weather__;

            return ____result;
        }
    }


    public sealed class RootobjectFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.OpenWeather.Rootobject>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public RootobjectFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("lat"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("lon"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("timezone"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("timezone_offset"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("current"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("hourly"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("daily"), 6},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("lat"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("lon"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("timezone"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("timezone_offset"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("current"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("hourly"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("daily"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.OpenWeather.Rootobject value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteSingle(value.lat);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteSingle(value.lon);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.timezone);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteInt32(value.timezone_offset);
            writer.WriteRaw(this.____stringByteKeys[4]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Current>().Serialize(ref writer, value.current, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[5]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Hourly[]>().Serialize(ref writer, value.hourly, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[6]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Daily[]>().Serialize(ref writer, value.daily, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.OpenWeather.Rootobject Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __lat__ = default(float);
            var __lat__b__ = false;
            var __lon__ = default(float);
            var __lon__b__ = false;
            var __timezone__ = default(string);
            var __timezone__b__ = false;
            var __timezone_offset__ = default(int);
            var __timezone_offset__b__ = false;
            var __current__ = default(global::SimpleWeather.OpenWeather.Current);
            var __current__b__ = false;
            var __hourly__ = default(global::SimpleWeather.OpenWeather.Hourly[]);
            var __hourly__b__ = false;
            var __daily__ = default(global::SimpleWeather.OpenWeather.Daily[]);
            var __daily__b__ = false;

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
                        __lat__ = reader.ReadSingle();
                        __lat__b__ = true;
                        break;
                    case 1:
                        __lon__ = reader.ReadSingle();
                        __lon__b__ = true;
                        break;
                    case 2:
                        __timezone__ = reader.ReadString();
                        __timezone__b__ = true;
                        break;
                    case 3:
                        __timezone_offset__ = reader.ReadInt32();
                        __timezone_offset__b__ = true;
                        break;
                    case 4:
                        __current__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Current>().Deserialize(ref reader, formatterResolver);
                        __current__b__ = true;
                        break;
                    case 5:
                        __hourly__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Hourly[]>().Deserialize(ref reader, formatterResolver);
                        __hourly__b__ = true;
                        break;
                    case 6:
                        __daily__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Daily[]>().Deserialize(ref reader, formatterResolver);
                        __daily__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.OpenWeather.Rootobject();
            if(__lat__b__) ____result.lat = __lat__;
            if(__lon__b__) ____result.lon = __lon__;
            if(__timezone__b__) ____result.timezone = __timezone__;
            if(__timezone_offset__b__) ____result.timezone_offset = __timezone_offset__;
            if(__current__b__) ____result.current = __current__;
            if(__hourly__b__) ____result.hourly = __hourly__;
            if(__daily__b__) ____result.daily = __daily__;

            return ____result;
        }
    }

}

#pragma warning disable 168
#pragma warning restore 219
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612
#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 219
#pragma warning disable 168

namespace SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.WeatherYahoo
{
    using System;
    using Utf8Json;


    public sealed class LocationFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.WeatherYahoo.Location>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public LocationFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("woeid"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("city"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("region"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("country"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("lat"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("long"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("timezone_id"), 6},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("woeid"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("city"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("region"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("country"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("lat"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("long"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("timezone_id"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.WeatherYahoo.Location value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteInt32(value.woeid);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.city);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.region);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteString(value.country);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteDouble(value.lat);
            writer.WriteRaw(this.____stringByteKeys[5]);
            writer.WriteDouble(value._long);
            writer.WriteRaw(this.____stringByteKeys[6]);
            writer.WriteString(value.timezone_id);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.WeatherYahoo.Location Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __woeid__ = default(int);
            var __woeid__b__ = false;
            var __city__ = default(string);
            var __city__b__ = false;
            var __region__ = default(string);
            var __region__b__ = false;
            var __country__ = default(string);
            var __country__b__ = false;
            var __lat__ = default(double);
            var __lat__b__ = false;
            var ___long__ = default(double);
            var ___long__b__ = false;
            var __timezone_id__ = default(string);
            var __timezone_id__b__ = false;

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
                        __woeid__ = reader.ReadInt32();
                        __woeid__b__ = true;
                        break;
                    case 1:
                        __city__ = reader.ReadString();
                        __city__b__ = true;
                        break;
                    case 2:
                        __region__ = reader.ReadString();
                        __region__b__ = true;
                        break;
                    case 3:
                        __country__ = reader.ReadString();
                        __country__b__ = true;
                        break;
                    case 4:
                        __lat__ = reader.ReadDouble();
                        __lat__b__ = true;
                        break;
                    case 5:
                        ___long__ = reader.ReadDouble();
                        ___long__b__ = true;
                        break;
                    case 6:
                        __timezone_id__ = reader.ReadString();
                        __timezone_id__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.WeatherYahoo.Location();
            if(__woeid__b__) ____result.woeid = __woeid__;
            if(__city__b__) ____result.city = __city__;
            if(__region__b__) ____result.region = __region__;
            if(__country__b__) ____result.country = __country__;
            if(__lat__b__) ____result.lat = __lat__;
            if(___long__b__) ____result._long = ___long__;
            if(__timezone_id__b__) ____result.timezone_id = __timezone_id__;

            return ____result;
        }
    }


    public sealed class WindFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.WeatherYahoo.Wind>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public WindFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("chill"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("direction"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("speed"), 2},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("chill"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("direction"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("speed"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.WeatherYahoo.Wind value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteInt32(value.chill);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteInt32(value.direction);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteSingle(value.speed);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.WeatherYahoo.Wind Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __chill__ = default(int);
            var __chill__b__ = false;
            var __direction__ = default(int);
            var __direction__b__ = false;
            var __speed__ = default(float);
            var __speed__b__ = false;

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
                        __chill__ = reader.ReadInt32();
                        __chill__b__ = true;
                        break;
                    case 1:
                        __direction__ = reader.ReadInt32();
                        __direction__b__ = true;
                        break;
                    case 2:
                        __speed__ = reader.ReadSingle();
                        __speed__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.WeatherYahoo.Wind();
            if(__chill__b__) ____result.chill = __chill__;
            if(__direction__b__) ____result.direction = __direction__;
            if(__speed__b__) ____result.speed = __speed__;

            return ____result;
        }
    }


    public sealed class AtmosphereFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.WeatherYahoo.Atmosphere>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public AtmosphereFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("pressure"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("rising"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("visibility"), 2},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("pressure"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("rising"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("visibility"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.WeatherYahoo.Atmosphere value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteSingle(value.pressure);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteInt32(value.rising);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteSingle(value.visibility);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.WeatherYahoo.Atmosphere Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __pressure__ = default(float);
            var __pressure__b__ = false;
            var __rising__ = default(int);
            var __rising__b__ = false;
            var __visibility__ = default(float);
            var __visibility__b__ = false;

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
                        __pressure__ = reader.ReadSingle();
                        __pressure__b__ = true;
                        break;
                    case 1:
                        __rising__ = reader.ReadInt32();
                        __rising__b__ = true;
                        break;
                    case 2:
                        __visibility__ = reader.ReadSingle();
                        __visibility__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.WeatherYahoo.Atmosphere();
            if(__pressure__b__) ____result.pressure = __pressure__;
            if(__rising__b__) ____result.rising = __rising__;
            if(__visibility__b__) ____result.visibility = __visibility__;

            return ____result;
        }
    }


    public sealed class AstronomyFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.WeatherYahoo.Astronomy>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public AstronomyFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("sunrise"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("sunset"), 1},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("sunrise"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("sunset"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.WeatherYahoo.Astronomy value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.sunrise);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.sunset);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.WeatherYahoo.Astronomy Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __sunrise__ = default(string);
            var __sunrise__b__ = false;
            var __sunset__ = default(string);
            var __sunset__b__ = false;

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
                        __sunrise__ = reader.ReadString();
                        __sunrise__b__ = true;
                        break;
                    case 1:
                        __sunset__ = reader.ReadString();
                        __sunset__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.WeatherYahoo.Astronomy();
            if(__sunrise__b__) ____result.sunrise = __sunrise__;
            if(__sunset__b__) ____result.sunset = __sunset__;

            return ____result;
        }
    }


    public sealed class ConditionFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.WeatherYahoo.Condition>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public ConditionFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("code"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("temperature"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("text"), 2},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("code"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("temperature"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("text"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.WeatherYahoo.Condition value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteInt32(value.code);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteInt32(value.temperature);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.text);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.WeatherYahoo.Condition Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __code__ = default(int);
            var __code__b__ = false;
            var __temperature__ = default(int);
            var __temperature__b__ = false;
            var __text__ = default(string);
            var __text__b__ = false;

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
                        __code__ = reader.ReadInt32();
                        __code__b__ = true;
                        break;
                    case 1:
                        __temperature__ = reader.ReadInt32();
                        __temperature__b__ = true;
                        break;
                    case 2:
                        __text__ = reader.ReadString();
                        __text__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.WeatherYahoo.Condition();
            if(__code__b__) ____result.code = __code__;
            if(__temperature__b__) ____result.temperature = __temperature__;
            if(__text__b__) ____result.text = __text__;

            return ____result;
        }
    }


    public sealed class Current_ObservationFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.WeatherYahoo.Current_Observation>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public Current_ObservationFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("wind"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("atmosphere"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("astronomy"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("condition"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("pubDate"), 4},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("wind"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("atmosphere"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("astronomy"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("condition"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("pubDate"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.WeatherYahoo.Current_Observation value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherYahoo.Wind>().Serialize(ref writer, value.wind, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherYahoo.Atmosphere>().Serialize(ref writer, value.atmosphere, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[2]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherYahoo.Astronomy>().Serialize(ref writer, value.astronomy, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[3]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherYahoo.Condition>().Serialize(ref writer, value.condition, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteInt64(value.pubDate);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.WeatherYahoo.Current_Observation Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __wind__ = default(global::SimpleWeather.WeatherYahoo.Wind);
            var __wind__b__ = false;
            var __atmosphere__ = default(global::SimpleWeather.WeatherYahoo.Atmosphere);
            var __atmosphere__b__ = false;
            var __astronomy__ = default(global::SimpleWeather.WeatherYahoo.Astronomy);
            var __astronomy__b__ = false;
            var __condition__ = default(global::SimpleWeather.WeatherYahoo.Condition);
            var __condition__b__ = false;
            var __pubDate__ = default(long);
            var __pubDate__b__ = false;

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
                        __wind__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherYahoo.Wind>().Deserialize(ref reader, formatterResolver);
                        __wind__b__ = true;
                        break;
                    case 1:
                        __atmosphere__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherYahoo.Atmosphere>().Deserialize(ref reader, formatterResolver);
                        __atmosphere__b__ = true;
                        break;
                    case 2:
                        __astronomy__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherYahoo.Astronomy>().Deserialize(ref reader, formatterResolver);
                        __astronomy__b__ = true;
                        break;
                    case 3:
                        __condition__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherYahoo.Condition>().Deserialize(ref reader, formatterResolver);
                        __condition__b__ = true;
                        break;
                    case 4:
                        __pubDate__ = reader.ReadInt64();
                        __pubDate__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.WeatherYahoo.Current_Observation();
            if(__wind__b__) ____result.wind = __wind__;
            if(__atmosphere__b__) ____result.atmosphere = __atmosphere__;
            if(__astronomy__b__) ____result.astronomy = __astronomy__;
            if(__condition__b__) ____result.condition = __condition__;
            if(__pubDate__b__) ____result.pubDate = __pubDate__;

            return ____result;
        }
    }


    public sealed class ForecastFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.WeatherYahoo.Forecast>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public ForecastFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("code"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("date"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("day"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("high"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("low"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("text"), 5},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("code"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("date"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("day"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("high"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("low"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("text"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.WeatherYahoo.Forecast value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteInt32(value.code);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteInt64(value.date);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.day);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteInt32(value.high);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteInt32(value.low);
            writer.WriteRaw(this.____stringByteKeys[5]);
            writer.WriteString(value.text);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.WeatherYahoo.Forecast Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __code__ = default(int);
            var __code__b__ = false;
            var __date__ = default(long);
            var __date__b__ = false;
            var __day__ = default(string);
            var __day__b__ = false;
            var __high__ = default(int);
            var __high__b__ = false;
            var __low__ = default(int);
            var __low__b__ = false;
            var __text__ = default(string);
            var __text__b__ = false;

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
                        __code__ = reader.ReadInt32();
                        __code__b__ = true;
                        break;
                    case 1:
                        __date__ = reader.ReadInt64();
                        __date__b__ = true;
                        break;
                    case 2:
                        __day__ = reader.ReadString();
                        __day__b__ = true;
                        break;
                    case 3:
                        __high__ = reader.ReadInt32();
                        __high__b__ = true;
                        break;
                    case 4:
                        __low__ = reader.ReadInt32();
                        __low__b__ = true;
                        break;
                    case 5:
                        __text__ = reader.ReadString();
                        __text__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.WeatherYahoo.Forecast();
            if(__code__b__) ____result.code = __code__;
            if(__date__b__) ____result.date = __date__;
            if(__day__b__) ____result.day = __day__;
            if(__high__b__) ____result.high = __high__;
            if(__low__b__) ____result.low = __low__;
            if(__text__b__) ____result.text = __text__;

            return ____result;
        }
    }


    public sealed class RootobjectFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.WeatherYahoo.Rootobject>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public RootobjectFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("location"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("current_observation"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("forecasts"), 2},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("location"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("current_observation"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("forecasts"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.WeatherYahoo.Rootobject value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherYahoo.Location>().Serialize(ref writer, value.location, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherYahoo.Current_Observation>().Serialize(ref writer, value.current_observation, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[2]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherYahoo.Forecast[]>().Serialize(ref writer, value.forecasts, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.WeatherYahoo.Rootobject Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __location__ = default(global::SimpleWeather.WeatherYahoo.Location);
            var __location__b__ = false;
            var __current_observation__ = default(global::SimpleWeather.WeatherYahoo.Current_Observation);
            var __current_observation__b__ = false;
            var __forecasts__ = default(global::SimpleWeather.WeatherYahoo.Forecast[]);
            var __forecasts__b__ = false;

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
                        __location__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherYahoo.Location>().Deserialize(ref reader, formatterResolver);
                        __location__b__ = true;
                        break;
                    case 1:
                        __current_observation__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherYahoo.Current_Observation>().Deserialize(ref reader, formatterResolver);
                        __current_observation__b__ = true;
                        break;
                    case 2:
                        __forecasts__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherYahoo.Forecast[]>().Deserialize(ref reader, formatterResolver);
                        __forecasts__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.WeatherYahoo.Rootobject();
            if(__location__b__) ____result.location = __location__;
            if(__current_observation__b__) ____result.current_observation = __current_observation__;
            if(__forecasts__b__) ____result.forecasts = __forecasts__;

            return ____result;
        }
    }

}

#pragma warning disable 168
#pragma warning restore 219
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612
#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 219
#pragma warning disable 168

namespace SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.WeatherData.Images.Model
{
    using System;
    using Utf8Json;


    public sealed class ImageDataFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.WeatherData.Images.Model.ImageData>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public ImageDataFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("ArtistName"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("HexColor"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("Condition"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("ImageUrl"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("Location"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("OriginalLink"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("SiteName"), 6},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("ArtistName"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("HexColor"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("Condition"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("ImageUrl"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("Location"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("OriginalLink"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("SiteName"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.WeatherData.Images.Model.ImageData value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.ArtistName);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.HexColor);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.Condition);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteString(value.ImageUrl);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteString(value.Location);
            writer.WriteRaw(this.____stringByteKeys[5]);
            writer.WriteString(value.OriginalLink);
            writer.WriteRaw(this.____stringByteKeys[6]);
            writer.WriteString(value.SiteName);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.WeatherData.Images.Model.ImageData Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __ArtistName__ = default(string);
            var __ArtistName__b__ = false;
            var __HexColor__ = default(string);
            var __HexColor__b__ = false;
            var __Condition__ = default(string);
            var __Condition__b__ = false;
            var __ImageUrl__ = default(string);
            var __ImageUrl__b__ = false;
            var __Location__ = default(string);
            var __Location__b__ = false;
            var __OriginalLink__ = default(string);
            var __OriginalLink__b__ = false;
            var __SiteName__ = default(string);
            var __SiteName__b__ = false;

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
                        __ArtistName__ = reader.ReadString();
                        __ArtistName__b__ = true;
                        break;
                    case 1:
                        __HexColor__ = reader.ReadString();
                        __HexColor__b__ = true;
                        break;
                    case 2:
                        __Condition__ = reader.ReadString();
                        __Condition__b__ = true;
                        break;
                    case 3:
                        __ImageUrl__ = reader.ReadString();
                        __ImageUrl__b__ = true;
                        break;
                    case 4:
                        __Location__ = reader.ReadString();
                        __Location__b__ = true;
                        break;
                    case 5:
                        __OriginalLink__ = reader.ReadString();
                        __OriginalLink__b__ = true;
                        break;
                    case 6:
                        __SiteName__ = reader.ReadString();
                        __SiteName__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.WeatherData.Images.Model.ImageData();
            if(__ArtistName__b__) ____result.ArtistName = __ArtistName__;
            if(__HexColor__b__) ____result.HexColor = __HexColor__;
            if(__Condition__b__) ____result.Condition = __Condition__;
            if(__ImageUrl__b__) ____result.ImageUrl = __ImageUrl__;
            if(__Location__b__) ____result.Location = __Location__;
            if(__OriginalLink__b__) ____result.OriginalLink = __OriginalLink__;
            if(__SiteName__b__) ____result.SiteName = __SiteName__;

            return ____result;
        }
    }

}

#pragma warning disable 168
#pragma warning restore 219
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612
