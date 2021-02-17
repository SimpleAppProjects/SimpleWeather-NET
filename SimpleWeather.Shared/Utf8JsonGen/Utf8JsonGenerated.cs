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
            lookup = new global::System.Collections.Generic.Dictionary<Type, int>(166)
            {
                {typeof(global::System.Collections.Generic.IList<global::SimpleWeather.WeatherData.Forecast>), 0 },
                {typeof(global::System.Collections.Generic.IList<global::SimpleWeather.WeatherData.HourlyForecast>), 1 },
                {typeof(global::System.Collections.Generic.IList<global::SimpleWeather.WeatherData.TextForecast>), 2 },
                {typeof(global::System.Collections.Generic.ICollection<global::SimpleWeather.WeatherData.WeatherAlert>), 3 },
                {typeof(global::System.Collections.Generic.List<global::SimpleWeather.AQICN.Uvi>), 4 },
                {typeof(global::SimpleWeather.AQICN.Uvi[]), 5 },
                {typeof(global::SimpleWeather.Bing.Value[]), 6 },
                {typeof(global::SimpleWeather.Bing.Resource[]), 7 },
                {typeof(global::SimpleWeather.Bing.Resourceset[]), 8 },
                {typeof(global::SimpleWeather.HERE.Timesegment[]), 9 },
                {typeof(global::SimpleWeather.HERE.Alert[]), 10 },
                {typeof(global::SimpleWeather.HERE.Additionaldata[]), 11 },
                {typeof(global::SimpleWeather.HERE.Suggestion[]), 12 },
                {typeof(global::SimpleWeather.HERE.Navigationposition[]), 13 },
                {typeof(global::SimpleWeather.HERE.Result[]), 14 },
                {typeof(global::SimpleWeather.HERE.View[]), 15 },
                {typeof(global::SimpleWeather.HERE.Warning[]), 16 },
                {typeof(global::SimpleWeather.HERE.Watch[]), 17 },
                {typeof(global::SimpleWeather.HERE.Observation[]), 18 },
                {typeof(global::SimpleWeather.HERE.Location[]), 19 },
                {typeof(global::SimpleWeather.HERE.Forecast[]), 20 },
                {typeof(global::SimpleWeather.HERE.Forecast1[]), 21 },
                {typeof(global::SimpleWeather.HERE.Astronomy1[]), 22 },
                {typeof(global::SimpleWeather.Metno.Time[]), 23 },
                {typeof(global::SimpleWeather.Metno.Timesery[]), 24 },
                {typeof(global::SimpleWeather.NWS.AlertGraph[]), 25 },
                {typeof(global::System.DateTimeOffset[]), 26 },
                {typeof(global::System.Collections.Generic.List<string>), 27 },
                {typeof(global::System.Collections.Generic.List<global::SimpleWeather.NWS.Hourly.PeriodsItem>), 28 },
                {typeof(global::SimpleWeather.OpenWeather.Weather[]), 29 },
                {typeof(global::SimpleWeather.OpenWeather.List[]), 30 },
                {typeof(global::SimpleWeather.WeatherApi.LocationItem[]), 31 },
                {typeof(global::SimpleWeather.WeatherUnlocked.Timeframe[]), 32 },
                {typeof(global::SimpleWeather.WeatherUnlocked.Day[]), 33 },
                {typeof(global::SimpleWeather.Location.LocationData), 34 },
                {typeof(global::SimpleWeather.Location.Favorites), 35 },
                {typeof(global::SimpleWeather.WeatherData.Location), 36 },
                {typeof(global::SimpleWeather.WeatherData.ForecastExtras), 37 },
                {typeof(global::SimpleWeather.WeatherData.Forecast), 38 },
                {typeof(global::SimpleWeather.WeatherData.HourlyForecast), 39 },
                {typeof(global::SimpleWeather.WeatherData.TextForecast), 40 },
                {typeof(global::SimpleWeather.WeatherData.Beaufort), 41 },
                {typeof(global::SimpleWeather.WeatherData.UV), 42 },
                {typeof(global::SimpleWeather.WeatherData.AirQuality), 43 },
                {typeof(global::SimpleWeather.WeatherData.Condition), 44 },
                {typeof(global::SimpleWeather.WeatherData.Atmosphere), 45 },
                {typeof(global::SimpleWeather.WeatherData.MoonPhase), 46 },
                {typeof(global::SimpleWeather.WeatherData.Astronomy), 47 },
                {typeof(global::SimpleWeather.WeatherData.Precipitation), 48 },
                {typeof(global::SimpleWeather.WeatherData.WeatherAlert), 49 },
                {typeof(global::SimpleWeather.WeatherData.Weather), 50 },
                {typeof(global::SimpleWeather.WeatherData.BaseForecast), 51 },
                {typeof(global::SimpleWeather.WeatherData.Forecasts), 52 },
                {typeof(global::SimpleWeather.WeatherData.HourlyForecasts), 53 },
                {typeof(global::SimpleWeather.WeatherData.WeatherAlerts), 54 },
                {typeof(global::SimpleWeather.AQICN.Uvi), 55 },
                {typeof(global::SimpleWeather.AQICN.Daily), 56 },
                {typeof(global::SimpleWeather.AQICN.Forecast), 57 },
                {typeof(global::SimpleWeather.AQICN.Data), 58 },
                {typeof(global::SimpleWeather.AQICN.Rootobject), 59 },
                {typeof(global::SimpleWeather.Bing.Address), 60 },
                {typeof(global::SimpleWeather.Bing.Value), 61 },
                {typeof(global::SimpleWeather.Bing.Resource), 62 },
                {typeof(global::SimpleWeather.Bing.Resourceset), 63 },
                {typeof(global::SimpleWeather.Bing.AC_Rootobject), 64 },
                {typeof(global::SimpleWeather.HERE.Timesegment), 65 },
                {typeof(global::SimpleWeather.HERE.Alert), 66 },
                {typeof(global::SimpleWeather.HERE.Alerts), 67 },
                {typeof(global::SimpleWeather.HERE.Additionaldata), 68 },
                {typeof(global::SimpleWeather.HERE.Address), 69 },
                {typeof(global::SimpleWeather.HERE.Suggestion), 70 },
                {typeof(global::SimpleWeather.HERE.AC_Rootobject), 71 },
                {typeof(global::SimpleWeather.HERE.Metainfo), 72 },
                {typeof(global::SimpleWeather.HERE.Matchquality), 73 },
                {typeof(global::SimpleWeather.HERE.Displayposition), 74 },
                {typeof(global::SimpleWeather.HERE.Navigationposition), 75 },
                {typeof(global::SimpleWeather.HERE.Timezone), 76 },
                {typeof(global::SimpleWeather.HERE.Admininfo), 77 },
                {typeof(global::SimpleWeather.HERE.GeoLocation), 78 },
                {typeof(global::SimpleWeather.HERE.Result), 79 },
                {typeof(global::SimpleWeather.HERE.View), 80 },
                {typeof(global::SimpleWeather.HERE.Response), 81 },
                {typeof(global::SimpleWeather.HERE.Geo_Rootobject), 82 },
                {typeof(global::SimpleWeather.HERE.TokenRootobject), 83 },
                {typeof(global::SimpleWeather.HERE.Token), 84 },
                {typeof(global::SimpleWeather.HERE.Warning), 85 },
                {typeof(global::SimpleWeather.HERE.Watch), 86 },
                {typeof(global::SimpleWeather.HERE.Nwsalerts), 87 },
                {typeof(global::SimpleWeather.HERE.Observation), 88 },
                {typeof(global::SimpleWeather.HERE.Location), 89 },
                {typeof(global::SimpleWeather.HERE.Observations), 90 },
                {typeof(global::SimpleWeather.HERE.Forecast), 91 },
                {typeof(global::SimpleWeather.HERE.Forecastlocation), 92 },
                {typeof(global::SimpleWeather.HERE.Dailyforecasts), 93 },
                {typeof(global::SimpleWeather.HERE.Forecast1), 94 },
                {typeof(global::SimpleWeather.HERE.Forecastlocation1), 95 },
                {typeof(global::SimpleWeather.HERE.Hourlyforecasts), 96 },
                {typeof(global::SimpleWeather.HERE.Astronomy1), 97 },
                {typeof(global::SimpleWeather.HERE.Astronomy), 98 },
                {typeof(global::SimpleWeather.HERE.Rootobject), 99 },
                {typeof(global::SimpleWeather.Metno.AstroMeta), 100 },
                {typeof(global::SimpleWeather.Metno.Low_Moon), 101 },
                {typeof(global::SimpleWeather.Metno.High_Moon), 102 },
                {typeof(global::SimpleWeather.Metno.Solarnoon), 103 },
                {typeof(global::SimpleWeather.Metno.Moonphase), 104 },
                {typeof(global::SimpleWeather.Metno.Moonposition), 105 },
                {typeof(global::SimpleWeather.Metno.Sunrise), 106 },
                {typeof(global::SimpleWeather.Metno.Moonshadow), 107 },
                {typeof(global::SimpleWeather.Metno.Moonrise), 108 },
                {typeof(global::SimpleWeather.Metno.Solarmidnight), 109 },
                {typeof(global::SimpleWeather.Metno.Sunset), 110 },
                {typeof(global::SimpleWeather.Metno.Moonset), 111 },
                {typeof(global::SimpleWeather.Metno.Time), 112 },
                {typeof(global::SimpleWeather.Metno.Location), 113 },
                {typeof(global::SimpleWeather.Metno.AstroRootobject), 114 },
                {typeof(global::SimpleWeather.Metno.Geometry), 115 },
                {typeof(global::SimpleWeather.Metno.Units), 116 },
                {typeof(global::SimpleWeather.Metno.Meta), 117 },
                {typeof(global::SimpleWeather.Metno.Details), 118 },
                {typeof(global::SimpleWeather.Metno.Instant), 119 },
                {typeof(global::SimpleWeather.Metno.Summary), 120 },
                {typeof(global::SimpleWeather.Metno.Details1), 121 },
                {typeof(global::SimpleWeather.Metno.Next_12_Hours), 122 },
                {typeof(global::SimpleWeather.Metno.Summary1), 123 },
                {typeof(global::SimpleWeather.Metno.Details2), 124 },
                {typeof(global::SimpleWeather.Metno.Next_1_Hours), 125 },
                {typeof(global::SimpleWeather.Metno.Summary2), 126 },
                {typeof(global::SimpleWeather.Metno.Details3), 127 },
                {typeof(global::SimpleWeather.Metno.Next_6_Hours), 128 },
                {typeof(global::SimpleWeather.Metno.Data), 129 },
                {typeof(global::SimpleWeather.Metno.Timesery), 130 },
                {typeof(global::SimpleWeather.Metno.Properties), 131 },
                {typeof(global::SimpleWeather.Metno.Rootobject), 132 },
                {typeof(global::SimpleWeather.NWS.AlertGraph), 133 },
                {typeof(global::SimpleWeather.NWS.AlertRootobject), 134 },
                {typeof(global::SimpleWeather.NWS.Observation.Location), 135 },
                {typeof(global::SimpleWeather.NWS.Observation.Time), 136 },
                {typeof(global::SimpleWeather.NWS.Observation.Data), 137 },
                {typeof(global::SimpleWeather.NWS.Observation.Currentobservation), 138 },
                {typeof(global::SimpleWeather.NWS.Observation.ForecastRootobject), 139 },
                {typeof(global::SimpleWeather.NWS.Hourly.Location), 140 },
                {typeof(global::SimpleWeather.NWS.Hourly.PeriodsItem), 141 },
                {typeof(global::SimpleWeather.NWS.Hourly.HourlyForecastResponse), 142 },
                {typeof(global::SimpleWeather.NWS.Hourly.PeriodItem), 143 },
                {typeof(global::SimpleWeather.NWS.SolCalcAstroProvider.AstroData), 144 },
                {typeof(global::SimpleWeather.OpenWeather.Coord), 145 },
                {typeof(global::SimpleWeather.OpenWeather.Weather), 146 },
                {typeof(global::SimpleWeather.OpenWeather.Main), 147 },
                {typeof(global::SimpleWeather.OpenWeather.Wind), 148 },
                {typeof(global::SimpleWeather.OpenWeather.Clouds), 149 },
                {typeof(global::SimpleWeather.OpenWeather.Rain), 150 },
                {typeof(global::SimpleWeather.OpenWeather.Snow), 151 },
                {typeof(global::SimpleWeather.OpenWeather.CurrentSys), 152 },
                {typeof(global::SimpleWeather.OpenWeather.CurrentRootobject), 153 },
                {typeof(global::SimpleWeather.OpenWeather.ForecastSys), 154 },
                {typeof(global::SimpleWeather.OpenWeather.List), 155 },
                {typeof(global::SimpleWeather.OpenWeather.City), 156 },
                {typeof(global::SimpleWeather.OpenWeather.ForecastRootobject), 157 },
                {typeof(global::SimpleWeather.OpenWeather.Rootobject), 158 },
                {typeof(global::SimpleWeather.WeatherApi.LocationItem), 159 },
                {typeof(global::SimpleWeather.WeatherApi.Rootobject), 160 },
                {typeof(global::SimpleWeather.WeatherUnlocked.CurrentRootobject), 161 },
                {typeof(global::SimpleWeather.WeatherUnlocked.Timeframe), 162 },
                {typeof(global::SimpleWeather.WeatherUnlocked.Day), 163 },
                {typeof(global::SimpleWeather.WeatherUnlocked.ForecastRootobject), 164 },
                {typeof(global::SimpleWeather.WeatherData.Images.Model.ImageData), 165 },
            };
        }

        internal static object GetFormatter(Type t)
        {
            int key;
            if (!lookup.TryGetValue(t, out key)) return null;

            switch (key)
            {
                case 0: return new global::Utf8Json.Formatters.InterfaceListFormatter<global::SimpleWeather.WeatherData.Forecast>();
                case 1: return new global::Utf8Json.Formatters.InterfaceListFormatter<global::SimpleWeather.WeatherData.HourlyForecast>();
                case 2: return new global::Utf8Json.Formatters.InterfaceListFormatter<global::SimpleWeather.WeatherData.TextForecast>();
                case 3: return new global::Utf8Json.Formatters.InterfaceCollectionFormatter<global::SimpleWeather.WeatherData.WeatherAlert>();
                case 4: return new global::Utf8Json.Formatters.ListFormatter<global::SimpleWeather.AQICN.Uvi>();
                case 5: return new global::Utf8Json.Formatters.ArrayFormatter<global::SimpleWeather.AQICN.Uvi>();
                case 6: return new global::Utf8Json.Formatters.ArrayFormatter<global::SimpleWeather.Bing.Value>();
                case 7: return new global::Utf8Json.Formatters.ArrayFormatter<global::SimpleWeather.Bing.Resource>();
                case 8: return new global::Utf8Json.Formatters.ArrayFormatter<global::SimpleWeather.Bing.Resourceset>();
                case 9: return new global::Utf8Json.Formatters.ArrayFormatter<global::SimpleWeather.HERE.Timesegment>();
                case 10: return new global::Utf8Json.Formatters.ArrayFormatter<global::SimpleWeather.HERE.Alert>();
                case 11: return new global::Utf8Json.Formatters.ArrayFormatter<global::SimpleWeather.HERE.Additionaldata>();
                case 12: return new global::Utf8Json.Formatters.ArrayFormatter<global::SimpleWeather.HERE.Suggestion>();
                case 13: return new global::Utf8Json.Formatters.ArrayFormatter<global::SimpleWeather.HERE.Navigationposition>();
                case 14: return new global::Utf8Json.Formatters.ArrayFormatter<global::SimpleWeather.HERE.Result>();
                case 15: return new global::Utf8Json.Formatters.ArrayFormatter<global::SimpleWeather.HERE.View>();
                case 16: return new global::Utf8Json.Formatters.ArrayFormatter<global::SimpleWeather.HERE.Warning>();
                case 17: return new global::Utf8Json.Formatters.ArrayFormatter<global::SimpleWeather.HERE.Watch>();
                case 18: return new global::Utf8Json.Formatters.ArrayFormatter<global::SimpleWeather.HERE.Observation>();
                case 19: return new global::Utf8Json.Formatters.ArrayFormatter<global::SimpleWeather.HERE.Location>();
                case 20: return new global::Utf8Json.Formatters.ArrayFormatter<global::SimpleWeather.HERE.Forecast>();
                case 21: return new global::Utf8Json.Formatters.ArrayFormatter<global::SimpleWeather.HERE.Forecast1>();
                case 22: return new global::Utf8Json.Formatters.ArrayFormatter<global::SimpleWeather.HERE.Astronomy1>();
                case 23: return new global::Utf8Json.Formatters.ArrayFormatter<global::SimpleWeather.Metno.Time>();
                case 24: return new global::Utf8Json.Formatters.ArrayFormatter<global::SimpleWeather.Metno.Timesery>();
                case 25: return new global::Utf8Json.Formatters.ArrayFormatter<global::SimpleWeather.NWS.AlertGraph>();
                case 26: return new global::Utf8Json.Formatters.ArrayFormatter<global::System.DateTimeOffset>();
                case 27: return new global::Utf8Json.Formatters.ListFormatter<string>();
                case 28: return new global::Utf8Json.Formatters.ListFormatter<global::SimpleWeather.NWS.Hourly.PeriodsItem>();
                case 29: return new global::Utf8Json.Formatters.ArrayFormatter<global::SimpleWeather.OpenWeather.Weather>();
                case 30: return new global::Utf8Json.Formatters.ArrayFormatter<global::SimpleWeather.OpenWeather.List>();
                case 31: return new global::Utf8Json.Formatters.ArrayFormatter<global::SimpleWeather.WeatherApi.LocationItem>();
                case 32: return new global::Utf8Json.Formatters.ArrayFormatter<global::SimpleWeather.WeatherUnlocked.Timeframe>();
                case 33: return new global::Utf8Json.Formatters.ArrayFormatter<global::SimpleWeather.WeatherUnlocked.Day>();
                case 34: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.Location.LocationDataFormatter();
                case 35: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.Location.FavoritesFormatter();
                case 36: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.WeatherData.LocationFormatter();
                case 37: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.WeatherData.ForecastExtrasFormatter();
                case 38: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.WeatherData.ForecastFormatter();
                case 39: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.WeatherData.HourlyForecastFormatter();
                case 40: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.WeatherData.TextForecastFormatter();
                case 41: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.WeatherData.BeaufortFormatter();
                case 42: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.WeatherData.UVFormatter();
                case 43: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.WeatherData.AirQualityFormatter();
                case 44: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.WeatherData.ConditionFormatter();
                case 45: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.WeatherData.AtmosphereFormatter();
                case 46: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.WeatherData.MoonPhaseFormatter();
                case 47: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.WeatherData.AstronomyFormatter();
                case 48: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.WeatherData.PrecipitationFormatter();
                case 49: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.WeatherData.WeatherAlertFormatter();
                case 50: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.WeatherData.WeatherFormatter();
                case 51: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.WeatherData.BaseForecastFormatter();
                case 52: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.WeatherData.ForecastsFormatter();
                case 53: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.WeatherData.HourlyForecastsFormatter();
                case 54: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.WeatherData.WeatherAlertsFormatter();
                case 55: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.AQICN.UviFormatter();
                case 56: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.AQICN.DailyFormatter();
                case 57: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.AQICN.ForecastFormatter();
                case 58: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.AQICN.DataFormatter();
                case 59: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.AQICN.RootobjectFormatter();
                case 60: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.Bing.AddressFormatter();
                case 61: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.Bing.ValueFormatter();
                case 62: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.Bing.ResourceFormatter();
                case 63: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.Bing.ResourcesetFormatter();
                case 64: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.Bing.AC_RootobjectFormatter();
                case 65: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.TimesegmentFormatter();
                case 66: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.AlertFormatter();
                case 67: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.AlertsFormatter();
                case 68: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.AdditionaldataFormatter();
                case 69: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.AddressFormatter();
                case 70: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.SuggestionFormatter();
                case 71: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.AC_RootobjectFormatter();
                case 72: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.MetainfoFormatter();
                case 73: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.MatchqualityFormatter();
                case 74: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.DisplaypositionFormatter();
                case 75: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.NavigationpositionFormatter();
                case 76: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.TimezoneFormatter();
                case 77: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.AdmininfoFormatter();
                case 78: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.GeoLocationFormatter();
                case 79: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.ResultFormatter();
                case 80: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.ViewFormatter();
                case 81: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.ResponseFormatter();
                case 82: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.Geo_RootobjectFormatter();
                case 83: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.TokenRootobjectFormatter();
                case 84: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.TokenFormatter();
                case 85: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.WarningFormatter();
                case 86: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.WatchFormatter();
                case 87: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.NwsalertsFormatter();
                case 88: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.ObservationFormatter();
                case 89: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.LocationFormatter();
                case 90: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.ObservationsFormatter();
                case 91: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.ForecastFormatter();
                case 92: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.ForecastlocationFormatter();
                case 93: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.DailyforecastsFormatter();
                case 94: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.Forecast1Formatter();
                case 95: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.Forecastlocation1Formatter();
                case 96: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.HourlyforecastsFormatter();
                case 97: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.Astronomy1Formatter();
                case 98: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.AstronomyFormatter();
                case 99: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.HERE.RootobjectFormatter();
                case 100: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.Metno.AstroMetaFormatter();
                case 101: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.Metno.Low_MoonFormatter();
                case 102: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.Metno.High_MoonFormatter();
                case 103: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.Metno.SolarnoonFormatter();
                case 104: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.Metno.MoonphaseFormatter();
                case 105: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.Metno.MoonpositionFormatter();
                case 106: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.Metno.SunriseFormatter();
                case 107: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.Metno.MoonshadowFormatter();
                case 108: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.Metno.MoonriseFormatter();
                case 109: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.Metno.SolarmidnightFormatter();
                case 110: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.Metno.SunsetFormatter();
                case 111: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.Metno.MoonsetFormatter();
                case 112: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.Metno.TimeFormatter();
                case 113: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.Metno.LocationFormatter();
                case 114: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.Metno.AstroRootobjectFormatter();
                case 115: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.Metno.GeometryFormatter();
                case 116: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.Metno.UnitsFormatter();
                case 117: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.Metno.MetaFormatter();
                case 118: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.Metno.DetailsFormatter();
                case 119: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.Metno.InstantFormatter();
                case 120: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.Metno.SummaryFormatter();
                case 121: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.Metno.Details1Formatter();
                case 122: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.Metno.Next_12_HoursFormatter();
                case 123: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.Metno.Summary1Formatter();
                case 124: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.Metno.Details2Formatter();
                case 125: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.Metno.Next_1_HoursFormatter();
                case 126: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.Metno.Summary2Formatter();
                case 127: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.Metno.Details3Formatter();
                case 128: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.Metno.Next_6_HoursFormatter();
                case 129: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.Metno.DataFormatter();
                case 130: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.Metno.TimeseryFormatter();
                case 131: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.Metno.PropertiesFormatter();
                case 132: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.Metno.RootobjectFormatter();
                case 133: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.NWS.AlertGraphFormatter();
                case 134: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.NWS.AlertRootobjectFormatter();
                case 135: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.NWS.Observation.LocationFormatter();
                case 136: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.NWS.Observation.TimeFormatter();
                case 137: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.NWS.Observation.DataFormatter();
                case 138: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.NWS.Observation.CurrentobservationFormatter();
                case 139: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.NWS.Observation.ForecastRootobjectFormatter();
                case 140: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.NWS.Hourly.LocationFormatter();
                case 141: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.NWS.Hourly.PeriodsItemFormatter();
                case 142: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.NWS.Hourly.HourlyForecastResponseFormatter();
                case 143: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.NWS.Hourly.PeriodItemFormatter();
                case 144: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.NWS.SolCalcAstroProvider_AstroDataFormatter();
                case 145: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.OpenWeather.CoordFormatter();
                case 146: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.OpenWeather.WeatherFormatter();
                case 147: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.OpenWeather.MainFormatter();
                case 148: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.OpenWeather.WindFormatter();
                case 149: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.OpenWeather.CloudsFormatter();
                case 150: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.OpenWeather.RainFormatter();
                case 151: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.OpenWeather.SnowFormatter();
                case 152: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.OpenWeather.CurrentSysFormatter();
                case 153: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.OpenWeather.CurrentRootobjectFormatter();
                case 154: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.OpenWeather.ForecastSysFormatter();
                case 155: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.OpenWeather.ListFormatter();
                case 156: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.OpenWeather.CityFormatter();
                case 157: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.OpenWeather.ForecastRootobjectFormatter();
                case 158: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.OpenWeather.RootobjectFormatter();
                case 159: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.WeatherApi.LocationItemFormatter();
                case 160: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.WeatherApi.RootobjectFormatter();
                case 161: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.WeatherUnlocked.CurrentRootobjectFormatter();
                case 162: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.WeatherUnlocked.TimeframeFormatter();
                case 163: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.WeatherUnlocked.DayFormatter();
                case 164: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.WeatherUnlocked.ForecastRootobjectFormatter();
                case 165: return new SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.WeatherData.Images.Model.ImageDataFormatter();
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

namespace SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.Location
{
    using System;
    using Utf8Json;


    public sealed class LocationDataFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.Location.LocationData>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public LocationDataFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("query"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("name"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("latitude"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("longitude"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("tz_long"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("locationType"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("weatherSource"), 6},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("locationSource"), 7},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("query"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("name"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("latitude"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("longitude"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("tz_long"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("locationType"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("weatherSource"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("locationSource"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.Location.LocationData value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.query);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.name);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteDouble(value.latitude);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteDouble(value.longitude);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteString(value.tz_long);
            writer.WriteRaw(this.____stringByteKeys[5]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Location.LocationType>().Serialize(ref writer, value.locationType, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[6]);
            writer.WriteString(value.weatherSource);
            writer.WriteRaw(this.____stringByteKeys[7]);
            writer.WriteString(value.locationSource);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.Location.LocationData Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __query__ = default(string);
            var __query__b__ = false;
            var __name__ = default(string);
            var __name__b__ = false;
            var __latitude__ = default(double);
            var __latitude__b__ = false;
            var __longitude__ = default(double);
            var __longitude__b__ = false;
            var __tz_long__ = default(string);
            var __tz_long__b__ = false;
            var __locationType__ = default(global::SimpleWeather.Location.LocationType);
            var __locationType__b__ = false;
            var __weatherSource__ = default(string);
            var __weatherSource__b__ = false;
            var __locationSource__ = default(string);
            var __locationSource__b__ = false;

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
                        __query__ = reader.ReadString();
                        __query__b__ = true;
                        break;
                    case 1:
                        __name__ = reader.ReadString();
                        __name__b__ = true;
                        break;
                    case 2:
                        __latitude__ = reader.ReadDouble();
                        __latitude__b__ = true;
                        break;
                    case 3:
                        __longitude__ = reader.ReadDouble();
                        __longitude__b__ = true;
                        break;
                    case 4:
                        __tz_long__ = reader.ReadString();
                        __tz_long__b__ = true;
                        break;
                    case 5:
                        __locationType__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Location.LocationType>().Deserialize(ref reader, formatterResolver);
                        __locationType__b__ = true;
                        break;
                    case 6:
                        __weatherSource__ = reader.ReadString();
                        __weatherSource__b__ = true;
                        break;
                    case 7:
                        __locationSource__ = reader.ReadString();
                        __locationSource__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.Location.LocationData();
            if(__query__b__) ____result.query = __query__;
            if(__name__b__) ____result.name = __name__;
            if(__latitude__b__) ____result.latitude = __latitude__;
            if(__longitude__b__) ____result.longitude = __longitude__;
            if(__tz_long__b__) ____result.tz_long = __tz_long__;
            if(__locationType__b__) ____result.locationType = __locationType__;
            if(__weatherSource__b__) ____result.weatherSource = __weatherSource__;
            if(__locationSource__b__) ____result.locationSource = __locationSource__;

            return ____result;
        }
    }


    public sealed class FavoritesFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.Location.Favorites>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public FavoritesFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("query"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("position"), 1},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("query"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("position"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.Location.Favorites value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.query);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteInt32(value.position);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.Location.Favorites Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __query__ = default(string);
            var __query__b__ = false;
            var __position__ = default(int);
            var __position__b__ = false;

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
                        __query__ = reader.ReadString();
                        __query__b__ = true;
                        break;
                    case 1:
                        __position__ = reader.ReadInt32();
                        __position__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.Location.Favorites();
            if(__query__b__) ____result.query = __query__;
            if(__position__b__) ____result.position = __position__;

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

namespace SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.WeatherData
{
    using System;
    using Utf8Json;


    public sealed class LocationFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.WeatherData.Location>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public LocationFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("name"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("latitude"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("longitude"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("tz_long"), 3},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("name"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("latitude"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("longitude"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("tz_long"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.WeatherData.Location value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.name);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.latitude, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[2]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.longitude, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteString(value.tz_long);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.WeatherData.Location Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __name__ = default(string);
            var __name__b__ = false;
            var __latitude__ = default(float?);
            var __latitude__b__ = false;
            var __longitude__ = default(float?);
            var __longitude__b__ = false;
            var __tz_long__ = default(string);
            var __tz_long__b__ = false;

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
                        __name__ = reader.ReadString();
                        __name__b__ = true;
                        break;
                    case 1:
                        __latitude__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __latitude__b__ = true;
                        break;
                    case 2:
                        __longitude__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __longitude__b__ = true;
                        break;
                    case 3:
                        __tz_long__ = reader.ReadString();
                        __tz_long__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.WeatherData.Location();
            if(__name__b__) ____result.name = __name__;
            if(__latitude__b__) ____result.latitude = __latitude__;
            if(__longitude__b__) ____result.longitude = __longitude__;
            if(__tz_long__b__) ____result.tz_long = __tz_long__;

            return ____result;
        }
    }


    public sealed class ForecastExtrasFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.WeatherData.ForecastExtras>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public ForecastExtrasFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("feelslike_f"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("feelslike_c"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("humidity"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("dewpoint_f"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("dewpoint_c"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("uv_index"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("pop"), 6},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("cloudiness"), 7},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("qpf_rain_in"), 8},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("qpf_rain_mm"), 9},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("qpf_snow_in"), 10},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("qpf_snow_cm"), 11},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("pressure_mb"), 12},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("pressure_in"), 13},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("wind_degrees"), 14},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("wind_mph"), 15},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("wind_kph"), 16},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("visibility_mi"), 17},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("visibility_km"), 18},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("windgust_mph"), 19},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("windgust_kph"), 20},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("feelslike_f"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("feelslike_c"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("humidity"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("dewpoint_f"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("dewpoint_c"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("uv_index"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("pop"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("cloudiness"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("qpf_rain_in"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("qpf_rain_mm"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("qpf_snow_in"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("qpf_snow_cm"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("pressure_mb"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("pressure_in"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("wind_degrees"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("wind_mph"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("wind_kph"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("visibility_mi"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("visibility_km"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("windgust_mph"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("windgust_kph"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.WeatherData.ForecastExtras value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.feelslike_f, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.feelslike_c, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[2]);
            formatterResolver.GetFormatterWithVerify<int?>().Serialize(ref writer, value.humidity, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[3]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.dewpoint_f, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[4]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.dewpoint_c, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[5]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.uv_index, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[6]);
            formatterResolver.GetFormatterWithVerify<int?>().Serialize(ref writer, value.pop, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[7]);
            formatterResolver.GetFormatterWithVerify<int?>().Serialize(ref writer, value.cloudiness, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[8]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.qpf_rain_in, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[9]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.qpf_rain_mm, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[10]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.qpf_snow_in, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[11]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.qpf_snow_cm, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[12]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.pressure_mb, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[13]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.pressure_in, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[14]);
            formatterResolver.GetFormatterWithVerify<int?>().Serialize(ref writer, value.wind_degrees, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[15]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.wind_mph, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[16]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.wind_kph, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[17]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.visibility_mi, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[18]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.visibility_km, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[19]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.windgust_mph, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[20]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.windgust_kph, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.WeatherData.ForecastExtras Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __feelslike_f__ = default(float?);
            var __feelslike_f__b__ = false;
            var __feelslike_c__ = default(float?);
            var __feelslike_c__b__ = false;
            var __humidity__ = default(int?);
            var __humidity__b__ = false;
            var __dewpoint_f__ = default(float?);
            var __dewpoint_f__b__ = false;
            var __dewpoint_c__ = default(float?);
            var __dewpoint_c__b__ = false;
            var __uv_index__ = default(float?);
            var __uv_index__b__ = false;
            var __pop__ = default(int?);
            var __pop__b__ = false;
            var __cloudiness__ = default(int?);
            var __cloudiness__b__ = false;
            var __qpf_rain_in__ = default(float?);
            var __qpf_rain_in__b__ = false;
            var __qpf_rain_mm__ = default(float?);
            var __qpf_rain_mm__b__ = false;
            var __qpf_snow_in__ = default(float?);
            var __qpf_snow_in__b__ = false;
            var __qpf_snow_cm__ = default(float?);
            var __qpf_snow_cm__b__ = false;
            var __pressure_mb__ = default(float?);
            var __pressure_mb__b__ = false;
            var __pressure_in__ = default(float?);
            var __pressure_in__b__ = false;
            var __wind_degrees__ = default(int?);
            var __wind_degrees__b__ = false;
            var __wind_mph__ = default(float?);
            var __wind_mph__b__ = false;
            var __wind_kph__ = default(float?);
            var __wind_kph__b__ = false;
            var __visibility_mi__ = default(float?);
            var __visibility_mi__b__ = false;
            var __visibility_km__ = default(float?);
            var __visibility_km__b__ = false;
            var __windgust_mph__ = default(float?);
            var __windgust_mph__b__ = false;
            var __windgust_kph__ = default(float?);
            var __windgust_kph__b__ = false;

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
                        __feelslike_f__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __feelslike_f__b__ = true;
                        break;
                    case 1:
                        __feelslike_c__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __feelslike_c__b__ = true;
                        break;
                    case 2:
                        __humidity__ = formatterResolver.GetFormatterWithVerify<int?>().Deserialize(ref reader, formatterResolver);
                        __humidity__b__ = true;
                        break;
                    case 3:
                        __dewpoint_f__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __dewpoint_f__b__ = true;
                        break;
                    case 4:
                        __dewpoint_c__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __dewpoint_c__b__ = true;
                        break;
                    case 5:
                        __uv_index__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __uv_index__b__ = true;
                        break;
                    case 6:
                        __pop__ = formatterResolver.GetFormatterWithVerify<int?>().Deserialize(ref reader, formatterResolver);
                        __pop__b__ = true;
                        break;
                    case 7:
                        __cloudiness__ = formatterResolver.GetFormatterWithVerify<int?>().Deserialize(ref reader, formatterResolver);
                        __cloudiness__b__ = true;
                        break;
                    case 8:
                        __qpf_rain_in__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __qpf_rain_in__b__ = true;
                        break;
                    case 9:
                        __qpf_rain_mm__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __qpf_rain_mm__b__ = true;
                        break;
                    case 10:
                        __qpf_snow_in__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __qpf_snow_in__b__ = true;
                        break;
                    case 11:
                        __qpf_snow_cm__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __qpf_snow_cm__b__ = true;
                        break;
                    case 12:
                        __pressure_mb__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __pressure_mb__b__ = true;
                        break;
                    case 13:
                        __pressure_in__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __pressure_in__b__ = true;
                        break;
                    case 14:
                        __wind_degrees__ = formatterResolver.GetFormatterWithVerify<int?>().Deserialize(ref reader, formatterResolver);
                        __wind_degrees__b__ = true;
                        break;
                    case 15:
                        __wind_mph__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __wind_mph__b__ = true;
                        break;
                    case 16:
                        __wind_kph__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __wind_kph__b__ = true;
                        break;
                    case 17:
                        __visibility_mi__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __visibility_mi__b__ = true;
                        break;
                    case 18:
                        __visibility_km__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __visibility_km__b__ = true;
                        break;
                    case 19:
                        __windgust_mph__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __windgust_mph__b__ = true;
                        break;
                    case 20:
                        __windgust_kph__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __windgust_kph__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.WeatherData.ForecastExtras();
            if(__feelslike_f__b__) ____result.feelslike_f = __feelslike_f__;
            if(__feelslike_c__b__) ____result.feelslike_c = __feelslike_c__;
            if(__humidity__b__) ____result.humidity = __humidity__;
            if(__dewpoint_f__b__) ____result.dewpoint_f = __dewpoint_f__;
            if(__dewpoint_c__b__) ____result.dewpoint_c = __dewpoint_c__;
            if(__uv_index__b__) ____result.uv_index = __uv_index__;
            if(__pop__b__) ____result.pop = __pop__;
            if(__cloudiness__b__) ____result.cloudiness = __cloudiness__;
            if(__qpf_rain_in__b__) ____result.qpf_rain_in = __qpf_rain_in__;
            if(__qpf_rain_mm__b__) ____result.qpf_rain_mm = __qpf_rain_mm__;
            if(__qpf_snow_in__b__) ____result.qpf_snow_in = __qpf_snow_in__;
            if(__qpf_snow_cm__b__) ____result.qpf_snow_cm = __qpf_snow_cm__;
            if(__pressure_mb__b__) ____result.pressure_mb = __pressure_mb__;
            if(__pressure_in__b__) ____result.pressure_in = __pressure_in__;
            if(__wind_degrees__b__) ____result.wind_degrees = __wind_degrees__;
            if(__wind_mph__b__) ____result.wind_mph = __wind_mph__;
            if(__wind_kph__b__) ____result.wind_kph = __wind_kph__;
            if(__visibility_mi__b__) ____result.visibility_mi = __visibility_mi__;
            if(__visibility_km__b__) ____result.visibility_km = __visibility_km__;
            if(__windgust_mph__b__) ____result.windgust_mph = __windgust_mph__;
            if(__windgust_kph__b__) ____result.windgust_kph = __windgust_kph__;

            return ____result;
        }
    }


    public sealed class ForecastFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.WeatherData.Forecast>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public ForecastFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("date"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("low_f"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("low_c"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("high_f"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("high_c"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("condition"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("icon"), 6},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("extras"), 7},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("date"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("low_f"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("low_c"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("high_f"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("high_c"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("condition"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("icon"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("extras"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.WeatherData.Forecast value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Serialize(ref writer, value.date, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.low_f, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[2]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.low_c, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[3]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.high_f, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[4]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.high_c, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[5]);
            writer.WriteString(value.condition);
            writer.WriteRaw(this.____stringByteKeys[6]);
            writer.WriteString(value.icon);
            writer.WriteRaw(this.____stringByteKeys[7]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherData.ForecastExtras>().Serialize(ref writer, value.extras, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.WeatherData.Forecast Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __date__ = default(global::System.DateTime);
            var __date__b__ = false;
            var __low_f__ = default(float?);
            var __low_f__b__ = false;
            var __low_c__ = default(float?);
            var __low_c__b__ = false;
            var __high_f__ = default(float?);
            var __high_f__b__ = false;
            var __high_c__ = default(float?);
            var __high_c__b__ = false;
            var __condition__ = default(string);
            var __condition__b__ = false;
            var __icon__ = default(string);
            var __icon__b__ = false;
            var __extras__ = default(global::SimpleWeather.WeatherData.ForecastExtras);
            var __extras__b__ = false;

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
                        __date__ = formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Deserialize(ref reader, formatterResolver);
                        __date__b__ = true;
                        break;
                    case 1:
                        __low_f__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __low_f__b__ = true;
                        break;
                    case 2:
                        __low_c__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __low_c__b__ = true;
                        break;
                    case 3:
                        __high_f__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __high_f__b__ = true;
                        break;
                    case 4:
                        __high_c__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __high_c__b__ = true;
                        break;
                    case 5:
                        __condition__ = reader.ReadString();
                        __condition__b__ = true;
                        break;
                    case 6:
                        __icon__ = reader.ReadString();
                        __icon__b__ = true;
                        break;
                    case 7:
                        __extras__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherData.ForecastExtras>().Deserialize(ref reader, formatterResolver);
                        __extras__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.WeatherData.Forecast();
            if(__date__b__) ____result.date = __date__;
            if(__low_f__b__) ____result.low_f = __low_f__;
            if(__low_c__b__) ____result.low_c = __low_c__;
            if(__high_f__b__) ____result.high_f = __high_f__;
            if(__high_c__b__) ____result.high_c = __high_c__;
            if(__condition__b__) ____result.condition = __condition__;
            if(__icon__b__) ____result.icon = __icon__;
            if(__extras__b__) ____result.extras = __extras__;

            return ____result;
        }
    }


    public sealed class HourlyForecastFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.WeatherData.HourlyForecast>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public HourlyForecastFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("date"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("wind_degrees"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("wind_mph"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("wind_kph"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("high_f"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("high_c"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("condition"), 6},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("icon"), 7},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("extras"), 8},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("date"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("wind_degrees"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("wind_mph"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("wind_kph"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("high_f"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("high_c"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("condition"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("icon"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("extras"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.WeatherData.HourlyForecast value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset>().Serialize(ref writer, value.date, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<int?>().Serialize(ref writer, value.wind_degrees, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[2]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.wind_mph, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[3]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.wind_kph, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[4]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.high_f, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[5]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.high_c, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[6]);
            writer.WriteString(value.condition);
            writer.WriteRaw(this.____stringByteKeys[7]);
            writer.WriteString(value.icon);
            writer.WriteRaw(this.____stringByteKeys[8]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherData.ForecastExtras>().Serialize(ref writer, value.extras, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.WeatherData.HourlyForecast Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __date__ = default(global::System.DateTimeOffset);
            var __date__b__ = false;
            var __wind_degrees__ = default(int?);
            var __wind_degrees__b__ = false;
            var __wind_mph__ = default(float?);
            var __wind_mph__b__ = false;
            var __wind_kph__ = default(float?);
            var __wind_kph__b__ = false;
            var __high_f__ = default(float?);
            var __high_f__b__ = false;
            var __high_c__ = default(float?);
            var __high_c__b__ = false;
            var __condition__ = default(string);
            var __condition__b__ = false;
            var __icon__ = default(string);
            var __icon__b__ = false;
            var __extras__ = default(global::SimpleWeather.WeatherData.ForecastExtras);
            var __extras__b__ = false;

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
                        __date__ = formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset>().Deserialize(ref reader, formatterResolver);
                        __date__b__ = true;
                        break;
                    case 1:
                        __wind_degrees__ = formatterResolver.GetFormatterWithVerify<int?>().Deserialize(ref reader, formatterResolver);
                        __wind_degrees__b__ = true;
                        break;
                    case 2:
                        __wind_mph__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __wind_mph__b__ = true;
                        break;
                    case 3:
                        __wind_kph__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __wind_kph__b__ = true;
                        break;
                    case 4:
                        __high_f__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __high_f__b__ = true;
                        break;
                    case 5:
                        __high_c__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __high_c__b__ = true;
                        break;
                    case 6:
                        __condition__ = reader.ReadString();
                        __condition__b__ = true;
                        break;
                    case 7:
                        __icon__ = reader.ReadString();
                        __icon__b__ = true;
                        break;
                    case 8:
                        __extras__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherData.ForecastExtras>().Deserialize(ref reader, formatterResolver);
                        __extras__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.WeatherData.HourlyForecast();
            if(__date__b__) ____result.date = __date__;
            if(__wind_degrees__b__) ____result.wind_degrees = __wind_degrees__;
            if(__wind_mph__b__) ____result.wind_mph = __wind_mph__;
            if(__wind_kph__b__) ____result.wind_kph = __wind_kph__;
            if(__high_f__b__) ____result.high_f = __high_f__;
            if(__high_c__b__) ____result.high_c = __high_c__;
            if(__condition__b__) ____result.condition = __condition__;
            if(__icon__b__) ____result.icon = __icon__;
            if(__extras__b__) ____result.extras = __extras__;

            return ____result;
        }
    }


    public sealed class TextForecastFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.WeatherData.TextForecast>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public TextForecastFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("date"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("fcttext"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("fcttext_metric"), 2},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("date"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("fcttext"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("fcttext_metric"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.WeatherData.TextForecast value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset>().Serialize(ref writer, value.date, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.fcttext);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.fcttext_metric);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.WeatherData.TextForecast Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __date__ = default(global::System.DateTimeOffset);
            var __date__b__ = false;
            var __fcttext__ = default(string);
            var __fcttext__b__ = false;
            var __fcttext_metric__ = default(string);
            var __fcttext_metric__b__ = false;

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
                        __date__ = formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset>().Deserialize(ref reader, formatterResolver);
                        __date__b__ = true;
                        break;
                    case 1:
                        __fcttext__ = reader.ReadString();
                        __fcttext__b__ = true;
                        break;
                    case 2:
                        __fcttext_metric__ = reader.ReadString();
                        __fcttext_metric__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.WeatherData.TextForecast();
            if(__date__b__) ____result.date = __date__;
            if(__fcttext__b__) ____result.fcttext = __fcttext__;
            if(__fcttext_metric__b__) ____result.fcttext_metric = __fcttext_metric__;

            return ____result;
        }
    }


    public sealed class BeaufortFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.WeatherData.Beaufort>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public BeaufortFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("scale"), 0},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("scale"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.WeatherData.Beaufort value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherData.Beaufort.BeaufortScale>().Serialize(ref writer, value.scale, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.WeatherData.Beaufort Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __scale__ = default(global::SimpleWeather.WeatherData.Beaufort.BeaufortScale);
            var __scale__b__ = false;

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
                        __scale__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherData.Beaufort.BeaufortScale>().Deserialize(ref reader, formatterResolver);
                        __scale__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.WeatherData.Beaufort();
            if(__scale__b__) ____result.scale = __scale__;

            return ____result;
        }
    }


    public sealed class UVFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.WeatherData.UV>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public UVFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("index"), 0},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("index"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.WeatherData.UV value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.index, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.WeatherData.UV Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __index__ = default(float?);
            var __index__b__ = false;

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
                        __index__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __index__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.WeatherData.UV();
            if(__index__b__) ____result.index = __index__;

            return ____result;
        }
    }


    public sealed class AirQualityFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.WeatherData.AirQuality>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public AirQualityFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("index"), 0},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("index"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.WeatherData.AirQuality value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<int?>().Serialize(ref writer, value.index, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.WeatherData.AirQuality Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __index__ = default(int?);
            var __index__b__ = false;

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
                        __index__ = formatterResolver.GetFormatterWithVerify<int?>().Deserialize(ref reader, formatterResolver);
                        __index__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.WeatherData.AirQuality();
            if(__index__b__) ____result.index = __index__;

            return ____result;
        }
    }


    public sealed class ConditionFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.WeatherData.Condition>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public ConditionFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("weather"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("temp_f"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("temp_c"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("wind_degrees"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("wind_mph"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("wind_kph"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("windgust_mph"), 6},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("windgust_kph"), 7},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("feelslike_f"), 8},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("feelslike_c"), 9},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("icon"), 10},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("beaufort"), 11},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("uv"), 12},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("high_f"), 13},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("high_c"), 14},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("low_f"), 15},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("low_c"), 16},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("airQuality"), 17},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("observation_time"), 18},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("weather"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("temp_f"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("temp_c"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("wind_degrees"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("wind_mph"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("wind_kph"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("windgust_mph"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("windgust_kph"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("feelslike_f"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("feelslike_c"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("icon"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("beaufort"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("uv"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("high_f"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("high_c"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("low_f"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("low_c"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("airQuality"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("observation_time"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.WeatherData.Condition value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.weather);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.temp_f, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[2]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.temp_c, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[3]);
            formatterResolver.GetFormatterWithVerify<int?>().Serialize(ref writer, value.wind_degrees, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[4]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.wind_mph, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[5]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.wind_kph, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[6]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.windgust_mph, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[7]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.windgust_kph, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[8]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.feelslike_f, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[9]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.feelslike_c, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[10]);
            writer.WriteString(value.icon);
            writer.WriteRaw(this.____stringByteKeys[11]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherData.Beaufort>().Serialize(ref writer, value.beaufort, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[12]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherData.UV>().Serialize(ref writer, value.uv, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[13]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.high_f, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[14]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.high_c, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[15]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.low_f, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[16]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.low_c, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[17]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherData.AirQuality>().Serialize(ref writer, value.airQuality, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[18]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset>().Serialize(ref writer, value.observation_time, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.WeatherData.Condition Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __weather__ = default(string);
            var __weather__b__ = false;
            var __temp_f__ = default(float?);
            var __temp_f__b__ = false;
            var __temp_c__ = default(float?);
            var __temp_c__b__ = false;
            var __wind_degrees__ = default(int?);
            var __wind_degrees__b__ = false;
            var __wind_mph__ = default(float?);
            var __wind_mph__b__ = false;
            var __wind_kph__ = default(float?);
            var __wind_kph__b__ = false;
            var __windgust_mph__ = default(float?);
            var __windgust_mph__b__ = false;
            var __windgust_kph__ = default(float?);
            var __windgust_kph__b__ = false;
            var __feelslike_f__ = default(float?);
            var __feelslike_f__b__ = false;
            var __feelslike_c__ = default(float?);
            var __feelslike_c__b__ = false;
            var __icon__ = default(string);
            var __icon__b__ = false;
            var __beaufort__ = default(global::SimpleWeather.WeatherData.Beaufort);
            var __beaufort__b__ = false;
            var __uv__ = default(global::SimpleWeather.WeatherData.UV);
            var __uv__b__ = false;
            var __high_f__ = default(float?);
            var __high_f__b__ = false;
            var __high_c__ = default(float?);
            var __high_c__b__ = false;
            var __low_f__ = default(float?);
            var __low_f__b__ = false;
            var __low_c__ = default(float?);
            var __low_c__b__ = false;
            var __airQuality__ = default(global::SimpleWeather.WeatherData.AirQuality);
            var __airQuality__b__ = false;
            var __observation_time__ = default(global::System.DateTimeOffset);
            var __observation_time__b__ = false;

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
                        __weather__ = reader.ReadString();
                        __weather__b__ = true;
                        break;
                    case 1:
                        __temp_f__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __temp_f__b__ = true;
                        break;
                    case 2:
                        __temp_c__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __temp_c__b__ = true;
                        break;
                    case 3:
                        __wind_degrees__ = formatterResolver.GetFormatterWithVerify<int?>().Deserialize(ref reader, formatterResolver);
                        __wind_degrees__b__ = true;
                        break;
                    case 4:
                        __wind_mph__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __wind_mph__b__ = true;
                        break;
                    case 5:
                        __wind_kph__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __wind_kph__b__ = true;
                        break;
                    case 6:
                        __windgust_mph__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __windgust_mph__b__ = true;
                        break;
                    case 7:
                        __windgust_kph__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __windgust_kph__b__ = true;
                        break;
                    case 8:
                        __feelslike_f__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __feelslike_f__b__ = true;
                        break;
                    case 9:
                        __feelslike_c__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __feelslike_c__b__ = true;
                        break;
                    case 10:
                        __icon__ = reader.ReadString();
                        __icon__b__ = true;
                        break;
                    case 11:
                        __beaufort__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherData.Beaufort>().Deserialize(ref reader, formatterResolver);
                        __beaufort__b__ = true;
                        break;
                    case 12:
                        __uv__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherData.UV>().Deserialize(ref reader, formatterResolver);
                        __uv__b__ = true;
                        break;
                    case 13:
                        __high_f__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __high_f__b__ = true;
                        break;
                    case 14:
                        __high_c__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __high_c__b__ = true;
                        break;
                    case 15:
                        __low_f__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __low_f__b__ = true;
                        break;
                    case 16:
                        __low_c__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __low_c__b__ = true;
                        break;
                    case 17:
                        __airQuality__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherData.AirQuality>().Deserialize(ref reader, formatterResolver);
                        __airQuality__b__ = true;
                        break;
                    case 18:
                        __observation_time__ = formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset>().Deserialize(ref reader, formatterResolver);
                        __observation_time__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.WeatherData.Condition();
            if(__weather__b__) ____result.weather = __weather__;
            if(__temp_f__b__) ____result.temp_f = __temp_f__;
            if(__temp_c__b__) ____result.temp_c = __temp_c__;
            if(__wind_degrees__b__) ____result.wind_degrees = __wind_degrees__;
            if(__wind_mph__b__) ____result.wind_mph = __wind_mph__;
            if(__wind_kph__b__) ____result.wind_kph = __wind_kph__;
            if(__windgust_mph__b__) ____result.windgust_mph = __windgust_mph__;
            if(__windgust_kph__b__) ____result.windgust_kph = __windgust_kph__;
            if(__feelslike_f__b__) ____result.feelslike_f = __feelslike_f__;
            if(__feelslike_c__b__) ____result.feelslike_c = __feelslike_c__;
            if(__icon__b__) ____result.icon = __icon__;
            if(__beaufort__b__) ____result.beaufort = __beaufort__;
            if(__uv__b__) ____result.uv = __uv__;
            if(__high_f__b__) ____result.high_f = __high_f__;
            if(__high_c__b__) ____result.high_c = __high_c__;
            if(__low_f__b__) ____result.low_f = __low_f__;
            if(__low_c__b__) ____result.low_c = __low_c__;
            if(__airQuality__b__) ____result.airQuality = __airQuality__;
            if(__observation_time__b__) ____result.observation_time = __observation_time__;

            return ____result;
        }
    }


    public sealed class AtmosphereFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.WeatherData.Atmosphere>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public AtmosphereFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("humidity"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("pressure_mb"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("pressure_in"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("pressure_trend"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("visibility_mi"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("visibility_km"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("dewpoint_f"), 6},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("dewpoint_c"), 7},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("humidity"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("pressure_mb"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("pressure_in"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("pressure_trend"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("visibility_mi"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("visibility_km"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("dewpoint_f"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("dewpoint_c"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.WeatherData.Atmosphere value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<int?>().Serialize(ref writer, value.humidity, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.pressure_mb, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[2]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.pressure_in, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteString(value.pressure_trend);
            writer.WriteRaw(this.____stringByteKeys[4]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.visibility_mi, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[5]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.visibility_km, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[6]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.dewpoint_f, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[7]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.dewpoint_c, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.WeatherData.Atmosphere Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __humidity__ = default(int?);
            var __humidity__b__ = false;
            var __pressure_mb__ = default(float?);
            var __pressure_mb__b__ = false;
            var __pressure_in__ = default(float?);
            var __pressure_in__b__ = false;
            var __pressure_trend__ = default(string);
            var __pressure_trend__b__ = false;
            var __visibility_mi__ = default(float?);
            var __visibility_mi__b__ = false;
            var __visibility_km__ = default(float?);
            var __visibility_km__b__ = false;
            var __dewpoint_f__ = default(float?);
            var __dewpoint_f__b__ = false;
            var __dewpoint_c__ = default(float?);
            var __dewpoint_c__b__ = false;

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
                        __humidity__ = formatterResolver.GetFormatterWithVerify<int?>().Deserialize(ref reader, formatterResolver);
                        __humidity__b__ = true;
                        break;
                    case 1:
                        __pressure_mb__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __pressure_mb__b__ = true;
                        break;
                    case 2:
                        __pressure_in__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __pressure_in__b__ = true;
                        break;
                    case 3:
                        __pressure_trend__ = reader.ReadString();
                        __pressure_trend__b__ = true;
                        break;
                    case 4:
                        __visibility_mi__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __visibility_mi__b__ = true;
                        break;
                    case 5:
                        __visibility_km__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __visibility_km__b__ = true;
                        break;
                    case 6:
                        __dewpoint_f__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __dewpoint_f__b__ = true;
                        break;
                    case 7:
                        __dewpoint_c__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __dewpoint_c__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.WeatherData.Atmosphere();
            if(__humidity__b__) ____result.humidity = __humidity__;
            if(__pressure_mb__b__) ____result.pressure_mb = __pressure_mb__;
            if(__pressure_in__b__) ____result.pressure_in = __pressure_in__;
            if(__pressure_trend__b__) ____result.pressure_trend = __pressure_trend__;
            if(__visibility_mi__b__) ____result.visibility_mi = __visibility_mi__;
            if(__visibility_km__b__) ____result.visibility_km = __visibility_km__;
            if(__dewpoint_f__b__) ____result.dewpoint_f = __dewpoint_f__;
            if(__dewpoint_c__b__) ____result.dewpoint_c = __dewpoint_c__;

            return ____result;
        }
    }


    public sealed class MoonPhaseFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.WeatherData.MoonPhase>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public MoonPhaseFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("phase"), 0},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("phase"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.WeatherData.MoonPhase value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherData.MoonPhase.MoonPhaseType>().Serialize(ref writer, value.phase, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.WeatherData.MoonPhase Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __phase__ = default(global::SimpleWeather.WeatherData.MoonPhase.MoonPhaseType);
            var __phase__b__ = false;

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
                        __phase__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherData.MoonPhase.MoonPhaseType>().Deserialize(ref reader, formatterResolver);
                        __phase__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.WeatherData.MoonPhase();
            if(__phase__b__) ____result.phase = __phase__;

            return ____result;
        }
    }


    public sealed class AstronomyFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.WeatherData.Astronomy>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public AstronomyFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("sunrise"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("sunset"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("moonrise"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("moonset"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("moonphase"), 4},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("sunrise"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("sunset"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("moonrise"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("moonset"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("moonphase"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.WeatherData.Astronomy value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Serialize(ref writer, value.sunrise, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Serialize(ref writer, value.sunset, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[2]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Serialize(ref writer, value.moonrise, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[3]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Serialize(ref writer, value.moonset, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[4]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherData.MoonPhase>().Serialize(ref writer, value.moonphase, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.WeatherData.Astronomy Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __sunrise__ = default(global::System.DateTime);
            var __sunrise__b__ = false;
            var __sunset__ = default(global::System.DateTime);
            var __sunset__b__ = false;
            var __moonrise__ = default(global::System.DateTime);
            var __moonrise__b__ = false;
            var __moonset__ = default(global::System.DateTime);
            var __moonset__b__ = false;
            var __moonphase__ = default(global::SimpleWeather.WeatherData.MoonPhase);
            var __moonphase__b__ = false;

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
                        __sunrise__ = formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Deserialize(ref reader, formatterResolver);
                        __sunrise__b__ = true;
                        break;
                    case 1:
                        __sunset__ = formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Deserialize(ref reader, formatterResolver);
                        __sunset__b__ = true;
                        break;
                    case 2:
                        __moonrise__ = formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Deserialize(ref reader, formatterResolver);
                        __moonrise__b__ = true;
                        break;
                    case 3:
                        __moonset__ = formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Deserialize(ref reader, formatterResolver);
                        __moonset__b__ = true;
                        break;
                    case 4:
                        __moonphase__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherData.MoonPhase>().Deserialize(ref reader, formatterResolver);
                        __moonphase__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.WeatherData.Astronomy();
            if(__sunrise__b__) ____result.sunrise = __sunrise__;
            if(__sunset__b__) ____result.sunset = __sunset__;
            if(__moonrise__b__) ____result.moonrise = __moonrise__;
            if(__moonset__b__) ____result.moonset = __moonset__;
            if(__moonphase__b__) ____result.moonphase = __moonphase__;

            return ____result;
        }
    }


    public sealed class PrecipitationFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.WeatherData.Precipitation>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public PrecipitationFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("pop"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("cloudiness"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("qpf_rain_in"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("qpf_rain_mm"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("qpf_snow_in"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("qpf_snow_cm"), 5},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("pop"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("cloudiness"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("qpf_rain_in"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("qpf_rain_mm"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("qpf_snow_in"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("qpf_snow_cm"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.WeatherData.Precipitation value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<int?>().Serialize(ref writer, value.pop, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<int?>().Serialize(ref writer, value.cloudiness, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[2]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.qpf_rain_in, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[3]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.qpf_rain_mm, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[4]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.qpf_snow_in, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[5]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.qpf_snow_cm, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.WeatherData.Precipitation Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __pop__ = default(int?);
            var __pop__b__ = false;
            var __cloudiness__ = default(int?);
            var __cloudiness__b__ = false;
            var __qpf_rain_in__ = default(float?);
            var __qpf_rain_in__b__ = false;
            var __qpf_rain_mm__ = default(float?);
            var __qpf_rain_mm__b__ = false;
            var __qpf_snow_in__ = default(float?);
            var __qpf_snow_in__b__ = false;
            var __qpf_snow_cm__ = default(float?);
            var __qpf_snow_cm__b__ = false;

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
                        __pop__ = formatterResolver.GetFormatterWithVerify<int?>().Deserialize(ref reader, formatterResolver);
                        __pop__b__ = true;
                        break;
                    case 1:
                        __cloudiness__ = formatterResolver.GetFormatterWithVerify<int?>().Deserialize(ref reader, formatterResolver);
                        __cloudiness__b__ = true;
                        break;
                    case 2:
                        __qpf_rain_in__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __qpf_rain_in__b__ = true;
                        break;
                    case 3:
                        __qpf_rain_mm__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __qpf_rain_mm__b__ = true;
                        break;
                    case 4:
                        __qpf_snow_in__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __qpf_snow_in__b__ = true;
                        break;
                    case 5:
                        __qpf_snow_cm__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __qpf_snow_cm__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.WeatherData.Precipitation();
            if(__pop__b__) ____result.pop = __pop__;
            if(__cloudiness__b__) ____result.cloudiness = __cloudiness__;
            if(__qpf_rain_in__b__) ____result.qpf_rain_in = __qpf_rain_in__;
            if(__qpf_rain_mm__b__) ____result.qpf_rain_mm = __qpf_rain_mm__;
            if(__qpf_snow_in__b__) ____result.qpf_snow_in = __qpf_snow_in__;
            if(__qpf_snow_cm__b__) ____result.qpf_snow_cm = __qpf_snow_cm__;

            return ____result;
        }
    }


    public sealed class WeatherAlertFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.WeatherData.WeatherAlert>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public WeatherAlertFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("Type"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("Severity"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("Title"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("Message"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("Attribution"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("Date"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("ExpiresDate"), 6},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("Notified"), 7},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("Type"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("Severity"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("Title"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("Message"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("Attribution"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("Date"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("ExpiresDate"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("Notified"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.WeatherData.WeatherAlert value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherData.WeatherAlertType>().Serialize(ref writer, value.Type, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherData.WeatherAlertSeverity>().Serialize(ref writer, value.Severity, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.Title);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteString(value.Message);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteString(value.Attribution);
            writer.WriteRaw(this.____stringByteKeys[5]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset>().Serialize(ref writer, value.Date, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[6]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset>().Serialize(ref writer, value.ExpiresDate, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[7]);
            writer.WriteBoolean(value.Notified);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.WeatherData.WeatherAlert Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __Type__ = default(global::SimpleWeather.WeatherData.WeatherAlertType);
            var __Type__b__ = false;
            var __Severity__ = default(global::SimpleWeather.WeatherData.WeatherAlertSeverity);
            var __Severity__b__ = false;
            var __Title__ = default(string);
            var __Title__b__ = false;
            var __Message__ = default(string);
            var __Message__b__ = false;
            var __Attribution__ = default(string);
            var __Attribution__b__ = false;
            var __Date__ = default(global::System.DateTimeOffset);
            var __Date__b__ = false;
            var __ExpiresDate__ = default(global::System.DateTimeOffset);
            var __ExpiresDate__b__ = false;
            var __Notified__ = default(bool);
            var __Notified__b__ = false;

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
                        __Type__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherData.WeatherAlertType>().Deserialize(ref reader, formatterResolver);
                        __Type__b__ = true;
                        break;
                    case 1:
                        __Severity__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherData.WeatherAlertSeverity>().Deserialize(ref reader, formatterResolver);
                        __Severity__b__ = true;
                        break;
                    case 2:
                        __Title__ = reader.ReadString();
                        __Title__b__ = true;
                        break;
                    case 3:
                        __Message__ = reader.ReadString();
                        __Message__b__ = true;
                        break;
                    case 4:
                        __Attribution__ = reader.ReadString();
                        __Attribution__b__ = true;
                        break;
                    case 5:
                        __Date__ = formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset>().Deserialize(ref reader, formatterResolver);
                        __Date__b__ = true;
                        break;
                    case 6:
                        __ExpiresDate__ = formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset>().Deserialize(ref reader, formatterResolver);
                        __ExpiresDate__b__ = true;
                        break;
                    case 7:
                        __Notified__ = reader.ReadBoolean();
                        __Notified__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.WeatherData.WeatherAlert();
            if(__Type__b__) ____result.Type = __Type__;
            if(__Severity__b__) ____result.Severity = __Severity__;
            if(__Title__b__) ____result.Title = __Title__;
            if(__Message__b__) ____result.Message = __Message__;
            if(__Attribution__b__) ____result.Attribution = __Attribution__;
            if(__Date__b__) ____result.Date = __Date__;
            if(__ExpiresDate__b__) ____result.ExpiresDate = __ExpiresDate__;
            if(__Notified__b__) ____result.Notified = __Notified__;

            return ____result;
        }
    }


    public sealed class WeatherFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.WeatherData.Weather>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public WeatherFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("location"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("forecast"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("hr_forecast"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("txt_forecast"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("condition"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("atmosphere"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("astronomy"), 6},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("precipitation"), 7},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("weather_alerts"), 8},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("ttl"), 9},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("source"), 10},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("query"), 11},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("locale"), 12},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("update_time"), 13},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("location"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("forecast"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("hr_forecast"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("txt_forecast"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("condition"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("atmosphere"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("astronomy"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("precipitation"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("weather_alerts"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("ttl"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("source"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("query"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("locale"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("update_time"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.WeatherData.Weather value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherData.Location>().Serialize(ref writer, value.location, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.IList<global::SimpleWeather.WeatherData.Forecast>>().Serialize(ref writer, value.forecast, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[2]);
            formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.IList<global::SimpleWeather.WeatherData.HourlyForecast>>().Serialize(ref writer, value.hr_forecast, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[3]);
            formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.IList<global::SimpleWeather.WeatherData.TextForecast>>().Serialize(ref writer, value.txt_forecast, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[4]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherData.Condition>().Serialize(ref writer, value.condition, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[5]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherData.Atmosphere>().Serialize(ref writer, value.atmosphere, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[6]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherData.Astronomy>().Serialize(ref writer, value.astronomy, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[7]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherData.Precipitation>().Serialize(ref writer, value.precipitation, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[8]);
            formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.ICollection<global::SimpleWeather.WeatherData.WeatherAlert>>().Serialize(ref writer, value.weather_alerts, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[9]);
            writer.WriteInt32(value.ttl);
            writer.WriteRaw(this.____stringByteKeys[10]);
            writer.WriteString(value.source);
            writer.WriteRaw(this.____stringByteKeys[11]);
            writer.WriteString(value.query);
            writer.WriteRaw(this.____stringByteKeys[12]);
            writer.WriteString(value.locale);
            writer.WriteRaw(this.____stringByteKeys[13]);
            writer.WriteString(value.updatetimeblob);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.WeatherData.Weather Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __location__ = default(global::SimpleWeather.WeatherData.Location);
            var __location__b__ = false;
            var __forecast__ = default(global::System.Collections.Generic.IList<global::SimpleWeather.WeatherData.Forecast>);
            var __forecast__b__ = false;
            var __hr_forecast__ = default(global::System.Collections.Generic.IList<global::SimpleWeather.WeatherData.HourlyForecast>);
            var __hr_forecast__b__ = false;
            var __txt_forecast__ = default(global::System.Collections.Generic.IList<global::SimpleWeather.WeatherData.TextForecast>);
            var __txt_forecast__b__ = false;
            var __condition__ = default(global::SimpleWeather.WeatherData.Condition);
            var __condition__b__ = false;
            var __atmosphere__ = default(global::SimpleWeather.WeatherData.Atmosphere);
            var __atmosphere__b__ = false;
            var __astronomy__ = default(global::SimpleWeather.WeatherData.Astronomy);
            var __astronomy__b__ = false;
            var __precipitation__ = default(global::SimpleWeather.WeatherData.Precipitation);
            var __precipitation__b__ = false;
            var __weather_alerts__ = default(global::System.Collections.Generic.ICollection<global::SimpleWeather.WeatherData.WeatherAlert>);
            var __weather_alerts__b__ = false;
            var __ttl__ = default(int);
            var __ttl__b__ = false;
            var __source__ = default(string);
            var __source__b__ = false;
            var __query__ = default(string);
            var __query__b__ = false;
            var __locale__ = default(string);
            var __locale__b__ = false;
            var __updatetimeblob__ = default(string);
            var __updatetimeblob__b__ = false;

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
                        __location__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherData.Location>().Deserialize(ref reader, formatterResolver);
                        __location__b__ = true;
                        break;
                    case 1:
                        __forecast__ = formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.IList<global::SimpleWeather.WeatherData.Forecast>>().Deserialize(ref reader, formatterResolver);
                        __forecast__b__ = true;
                        break;
                    case 2:
                        __hr_forecast__ = formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.IList<global::SimpleWeather.WeatherData.HourlyForecast>>().Deserialize(ref reader, formatterResolver);
                        __hr_forecast__b__ = true;
                        break;
                    case 3:
                        __txt_forecast__ = formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.IList<global::SimpleWeather.WeatherData.TextForecast>>().Deserialize(ref reader, formatterResolver);
                        __txt_forecast__b__ = true;
                        break;
                    case 4:
                        __condition__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherData.Condition>().Deserialize(ref reader, formatterResolver);
                        __condition__b__ = true;
                        break;
                    case 5:
                        __atmosphere__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherData.Atmosphere>().Deserialize(ref reader, formatterResolver);
                        __atmosphere__b__ = true;
                        break;
                    case 6:
                        __astronomy__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherData.Astronomy>().Deserialize(ref reader, formatterResolver);
                        __astronomy__b__ = true;
                        break;
                    case 7:
                        __precipitation__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherData.Precipitation>().Deserialize(ref reader, formatterResolver);
                        __precipitation__b__ = true;
                        break;
                    case 8:
                        __weather_alerts__ = formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.ICollection<global::SimpleWeather.WeatherData.WeatherAlert>>().Deserialize(ref reader, formatterResolver);
                        __weather_alerts__b__ = true;
                        break;
                    case 9:
                        __ttl__ = reader.ReadInt32();
                        __ttl__b__ = true;
                        break;
                    case 10:
                        __source__ = reader.ReadString();
                        __source__b__ = true;
                        break;
                    case 11:
                        __query__ = reader.ReadString();
                        __query__b__ = true;
                        break;
                    case 12:
                        __locale__ = reader.ReadString();
                        __locale__b__ = true;
                        break;
                    case 13:
                        __updatetimeblob__ = reader.ReadString();
                        __updatetimeblob__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.WeatherData.Weather();
            if(__location__b__) ____result.location = __location__;
            if(__forecast__b__) ____result.forecast = __forecast__;
            if(__hr_forecast__b__) ____result.hr_forecast = __hr_forecast__;
            if(__txt_forecast__b__) ____result.txt_forecast = __txt_forecast__;
            if(__condition__b__) ____result.condition = __condition__;
            if(__atmosphere__b__) ____result.atmosphere = __atmosphere__;
            if(__astronomy__b__) ____result.astronomy = __astronomy__;
            if(__precipitation__b__) ____result.precipitation = __precipitation__;
            if(__weather_alerts__b__) ____result.weather_alerts = __weather_alerts__;
            if(__ttl__b__) ____result.ttl = __ttl__;
            if(__source__b__) ____result.source = __source__;
            if(__query__b__) ____result.query = __query__;
            if(__locale__b__) ____result.locale = __locale__;
            if(__updatetimeblob__b__) ____result.updatetimeblob = __updatetimeblob__;

            return ____result;
        }
    }


    public sealed class BaseForecastFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.WeatherData.BaseForecast>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public BaseForecastFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("high_f"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("high_c"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("condition"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("icon"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("extras"), 4},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("high_f"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("high_c"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("condition"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("icon"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("extras"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.WeatherData.BaseForecast value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.high_f, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.high_c, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.condition);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteString(value.icon);
            writer.WriteRaw(this.____stringByteKeys[4]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherData.ForecastExtras>().Serialize(ref writer, value.extras, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.WeatherData.BaseForecast Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            
            
	        throw new InvalidOperationException("generated serializer for IInterface does not support deserialize.");
        }
    }


    public sealed class ForecastsFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.WeatherData.Forecasts>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public ForecastsFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("query"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("forecast"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("txt_forecast"), 2},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("query"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("forecast"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("txt_forecast"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.WeatherData.Forecasts value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.query);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.IList<global::SimpleWeather.WeatherData.Forecast>>().Serialize(ref writer, value.forecast, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[2]);
            formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.IList<global::SimpleWeather.WeatherData.TextForecast>>().Serialize(ref writer, value.txt_forecast, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.WeatherData.Forecasts Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __query__ = default(string);
            var __query__b__ = false;
            var __forecast__ = default(global::System.Collections.Generic.IList<global::SimpleWeather.WeatherData.Forecast>);
            var __forecast__b__ = false;
            var __txt_forecast__ = default(global::System.Collections.Generic.IList<global::SimpleWeather.WeatherData.TextForecast>);
            var __txt_forecast__b__ = false;

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
                        __query__ = reader.ReadString();
                        __query__b__ = true;
                        break;
                    case 1:
                        __forecast__ = formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.IList<global::SimpleWeather.WeatherData.Forecast>>().Deserialize(ref reader, formatterResolver);
                        __forecast__b__ = true;
                        break;
                    case 2:
                        __txt_forecast__ = formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.IList<global::SimpleWeather.WeatherData.TextForecast>>().Deserialize(ref reader, formatterResolver);
                        __txt_forecast__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.WeatherData.Forecasts();
            if(__query__b__) ____result.query = __query__;
            if(__forecast__b__) ____result.forecast = __forecast__;
            if(__txt_forecast__b__) ____result.txt_forecast = __txt_forecast__;

            return ____result;
        }
    }


    public sealed class HourlyForecastsFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.WeatherData.HourlyForecasts>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public HourlyForecastsFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("id"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("query"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("hr_forecast"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("date"), 3},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("id"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("query"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("hr_forecast"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("date"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.WeatherData.HourlyForecasts value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.id);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.query);
            writer.WriteRaw(this.____stringByteKeys[2]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherData.HourlyForecast>().Serialize(ref writer, value.hr_forecast, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteString(value.dateblob);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.WeatherData.HourlyForecasts Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __id__ = default(string);
            var __id__b__ = false;
            var __query__ = default(string);
            var __query__b__ = false;
            var __hr_forecast__ = default(global::SimpleWeather.WeatherData.HourlyForecast);
            var __hr_forecast__b__ = false;
            var __dateblob__ = default(string);
            var __dateblob__b__ = false;

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
                        __id__ = reader.ReadString();
                        __id__b__ = true;
                        break;
                    case 1:
                        __query__ = reader.ReadString();
                        __query__b__ = true;
                        break;
                    case 2:
                        __hr_forecast__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherData.HourlyForecast>().Deserialize(ref reader, formatterResolver);
                        __hr_forecast__b__ = true;
                        break;
                    case 3:
                        __dateblob__ = reader.ReadString();
                        __dateblob__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.WeatherData.HourlyForecasts();
            if(__id__b__) ____result.id = __id__;
            if(__query__b__) ____result.query = __query__;
            if(__hr_forecast__b__) ____result.hr_forecast = __hr_forecast__;
            if(__dateblob__b__) ____result.dateblob = __dateblob__;

            return ____result;
        }
    }


    public sealed class WeatherAlertsFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.WeatherData.WeatherAlerts>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public WeatherAlertsFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("query"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("alerts"), 1},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("query"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("alerts"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.WeatherData.WeatherAlerts value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.query);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.ICollection<global::SimpleWeather.WeatherData.WeatherAlert>>().Serialize(ref writer, value.alerts, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.WeatherData.WeatherAlerts Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __query__ = default(string);
            var __query__b__ = false;
            var __alerts__ = default(global::System.Collections.Generic.ICollection<global::SimpleWeather.WeatherData.WeatherAlert>);
            var __alerts__b__ = false;

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
                        __query__ = reader.ReadString();
                        __query__b__ = true;
                        break;
                    case 1:
                        __alerts__ = formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.ICollection<global::SimpleWeather.WeatherData.WeatherAlert>>().Deserialize(ref reader, formatterResolver);
                        __alerts__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.WeatherData.WeatherAlerts();
            if(__query__b__) ____result.query = __query__;
            if(__alerts__b__) ____result.alerts = __alerts__;

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

namespace SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.AQICN
{
    using System;
    using Utf8Json;


    public sealed class UviFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.AQICN.Uvi>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public UviFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("avg"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("day"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("max"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("min"), 3},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("avg"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("day"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("max"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("min"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.AQICN.Uvi value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteSingle(value.avg);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.day);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteSingle(value.max);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteSingle(value.min);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.AQICN.Uvi Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __avg__ = default(float);
            var __avg__b__ = false;
            var __day__ = default(string);
            var __day__b__ = false;
            var __max__ = default(float);
            var __max__b__ = false;
            var __min__ = default(float);
            var __min__b__ = false;

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
                        __avg__ = reader.ReadSingle();
                        __avg__b__ = true;
                        break;
                    case 1:
                        __day__ = reader.ReadString();
                        __day__b__ = true;
                        break;
                    case 2:
                        __max__ = reader.ReadSingle();
                        __max__b__ = true;
                        break;
                    case 3:
                        __min__ = reader.ReadSingle();
                        __min__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.AQICN.Uvi();
            if(__avg__b__) ____result.avg = __avg__;
            if(__day__b__) ____result.day = __day__;
            if(__max__b__) ____result.max = __max__;
            if(__min__b__) ____result.min = __min__;

            return ____result;
        }
    }


    public sealed class DailyFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.AQICN.Daily>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public DailyFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("uvi"), 0},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("uvi"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.AQICN.Daily value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.AQICN.Uvi[]>().Serialize(ref writer, value.uvi, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.AQICN.Daily Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __uvi__ = default(global::SimpleWeather.AQICN.Uvi[]);
            var __uvi__b__ = false;

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
                        __uvi__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.AQICN.Uvi[]>().Deserialize(ref reader, formatterResolver);
                        __uvi__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.AQICN.Daily();
            if(__uvi__b__) ____result.uvi = __uvi__;

            return ____result;
        }
    }


    public sealed class ForecastFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.AQICN.Forecast>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public ForecastFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("daily"), 0},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("daily"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.AQICN.Forecast value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.AQICN.Daily>().Serialize(ref writer, value.daily, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.AQICN.Forecast Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __daily__ = default(global::SimpleWeather.AQICN.Daily);
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
                        __daily__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.AQICN.Daily>().Deserialize(ref reader, formatterResolver);
                        __daily__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.AQICN.Forecast();
            if(__daily__b__) ____result.daily = __daily__;

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
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("forecast"), 1},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("aqi"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("forecast"),
                
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
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.AQICN.Forecast>().Serialize(ref writer, value.forecast, formatterResolver);
            
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
            var __forecast__ = default(global::SimpleWeather.AQICN.Forecast);
            var __forecast__b__ = false;

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
                        __forecast__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.AQICN.Forecast>().Deserialize(ref reader, formatterResolver);
                        __forecast__b__ = true;
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
            if(__forecast__b__) ____result.forecast = __forecast__;

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


    public sealed class WarningFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.HERE.Warning>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public WarningFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("type"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("description"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("severity"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("message"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("name"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("validFromTimeLocal"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("validUntilTimeLocal"), 6},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("country"), 7},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("state"), 8},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("latitude"), 9},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("longitude"), 10},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("type"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("description"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("severity"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("message"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("name"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("validFromTimeLocal"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("validUntilTimeLocal"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("country"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("state"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("latitude"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("longitude"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.HERE.Warning value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.type);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.description);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteInt32(value.severity);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteString(value.message);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteString(value.name);
            writer.WriteRaw(this.____stringByteKeys[5]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Serialize(ref writer, value.validFromTimeLocal, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[6]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Serialize(ref writer, value.validUntilTimeLocal, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[7]);
            writer.WriteString(value.country);
            writer.WriteRaw(this.____stringByteKeys[8]);
            writer.WriteString(value.state);
            writer.WriteRaw(this.____stringByteKeys[9]);
            writer.WriteString(value.latitude);
            writer.WriteRaw(this.____stringByteKeys[10]);
            writer.WriteString(value.longitude);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.HERE.Warning Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __type__ = default(string);
            var __type__b__ = false;
            var __description__ = default(string);
            var __description__b__ = false;
            var __severity__ = default(int);
            var __severity__b__ = false;
            var __message__ = default(string);
            var __message__b__ = false;
            var __name__ = default(string);
            var __name__b__ = false;
            var __validFromTimeLocal__ = default(global::System.DateTime);
            var __validFromTimeLocal__b__ = false;
            var __validUntilTimeLocal__ = default(global::System.DateTime);
            var __validUntilTimeLocal__b__ = false;
            var __country__ = default(string);
            var __country__b__ = false;
            var __state__ = default(string);
            var __state__b__ = false;
            var __latitude__ = default(string);
            var __latitude__b__ = false;
            var __longitude__ = default(string);
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
                        __type__ = reader.ReadString();
                        __type__b__ = true;
                        break;
                    case 1:
                        __description__ = reader.ReadString();
                        __description__b__ = true;
                        break;
                    case 2:
                        __severity__ = reader.ReadInt32();
                        __severity__b__ = true;
                        break;
                    case 3:
                        __message__ = reader.ReadString();
                        __message__b__ = true;
                        break;
                    case 4:
                        __name__ = reader.ReadString();
                        __name__b__ = true;
                        break;
                    case 5:
                        __validFromTimeLocal__ = formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Deserialize(ref reader, formatterResolver);
                        __validFromTimeLocal__b__ = true;
                        break;
                    case 6:
                        __validUntilTimeLocal__ = formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Deserialize(ref reader, formatterResolver);
                        __validUntilTimeLocal__b__ = true;
                        break;
                    case 7:
                        __country__ = reader.ReadString();
                        __country__b__ = true;
                        break;
                    case 8:
                        __state__ = reader.ReadString();
                        __state__b__ = true;
                        break;
                    case 9:
                        __latitude__ = reader.ReadString();
                        __latitude__b__ = true;
                        break;
                    case 10:
                        __longitude__ = reader.ReadString();
                        __longitude__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.HERE.Warning();
            if(__type__b__) ____result.type = __type__;
            if(__description__b__) ____result.description = __description__;
            if(__severity__b__) ____result.severity = __severity__;
            if(__message__b__) ____result.message = __message__;
            if(__name__b__) ____result.name = __name__;
            if(__validFromTimeLocal__b__) ____result.validFromTimeLocal = __validFromTimeLocal__;
            if(__validUntilTimeLocal__b__) ____result.validUntilTimeLocal = __validUntilTimeLocal__;
            if(__country__b__) ____result.country = __country__;
            if(__state__b__) ____result.state = __state__;
            if(__latitude__b__) ____result.latitude = __latitude__;
            if(__longitude__b__) ____result.longitude = __longitude__;

            return ____result;
        }
    }


    public sealed class WatchFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.HERE.Watch>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public WatchFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("type"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("description"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("severity"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("message"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("name"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("validFromTimeLocal"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("validUntilTimeLocal"), 6},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("country"), 7},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("state"), 8},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("latitude"), 9},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("longitude"), 10},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("type"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("description"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("severity"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("message"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("name"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("validFromTimeLocal"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("validUntilTimeLocal"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("country"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("state"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("latitude"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("longitude"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.HERE.Watch value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.type);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.description);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteInt32(value.severity);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteString(value.message);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteString(value.name);
            writer.WriteRaw(this.____stringByteKeys[5]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Serialize(ref writer, value.validFromTimeLocal, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[6]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Serialize(ref writer, value.validUntilTimeLocal, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[7]);
            writer.WriteString(value.country);
            writer.WriteRaw(this.____stringByteKeys[8]);
            writer.WriteString(value.state);
            writer.WriteRaw(this.____stringByteKeys[9]);
            writer.WriteString(value.latitude);
            writer.WriteRaw(this.____stringByteKeys[10]);
            writer.WriteString(value.longitude);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.HERE.Watch Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __type__ = default(string);
            var __type__b__ = false;
            var __description__ = default(string);
            var __description__b__ = false;
            var __severity__ = default(int);
            var __severity__b__ = false;
            var __message__ = default(string);
            var __message__b__ = false;
            var __name__ = default(string);
            var __name__b__ = false;
            var __validFromTimeLocal__ = default(global::System.DateTime);
            var __validFromTimeLocal__b__ = false;
            var __validUntilTimeLocal__ = default(global::System.DateTime);
            var __validUntilTimeLocal__b__ = false;
            var __country__ = default(string);
            var __country__b__ = false;
            var __state__ = default(string);
            var __state__b__ = false;
            var __latitude__ = default(string);
            var __latitude__b__ = false;
            var __longitude__ = default(string);
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
                        __type__ = reader.ReadString();
                        __type__b__ = true;
                        break;
                    case 1:
                        __description__ = reader.ReadString();
                        __description__b__ = true;
                        break;
                    case 2:
                        __severity__ = reader.ReadInt32();
                        __severity__b__ = true;
                        break;
                    case 3:
                        __message__ = reader.ReadString();
                        __message__b__ = true;
                        break;
                    case 4:
                        __name__ = reader.ReadString();
                        __name__b__ = true;
                        break;
                    case 5:
                        __validFromTimeLocal__ = formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Deserialize(ref reader, formatterResolver);
                        __validFromTimeLocal__b__ = true;
                        break;
                    case 6:
                        __validUntilTimeLocal__ = formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Deserialize(ref reader, formatterResolver);
                        __validUntilTimeLocal__b__ = true;
                        break;
                    case 7:
                        __country__ = reader.ReadString();
                        __country__b__ = true;
                        break;
                    case 8:
                        __state__ = reader.ReadString();
                        __state__b__ = true;
                        break;
                    case 9:
                        __latitude__ = reader.ReadString();
                        __latitude__b__ = true;
                        break;
                    case 10:
                        __longitude__ = reader.ReadString();
                        __longitude__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.HERE.Watch();
            if(__type__b__) ____result.type = __type__;
            if(__description__b__) ____result.description = __description__;
            if(__severity__b__) ____result.severity = __severity__;
            if(__message__b__) ____result.message = __message__;
            if(__name__b__) ____result.name = __name__;
            if(__validFromTimeLocal__b__) ____result.validFromTimeLocal = __validFromTimeLocal__;
            if(__validUntilTimeLocal__b__) ____result.validUntilTimeLocal = __validUntilTimeLocal__;
            if(__country__b__) ____result.country = __country__;
            if(__state__b__) ____result.state = __state__;
            if(__latitude__b__) ____result.latitude = __latitude__;
            if(__longitude__b__) ____result.longitude = __longitude__;

            return ____result;
        }
    }


    public sealed class NwsalertsFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.HERE.Nwsalerts>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public NwsalertsFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("warning"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("watch"), 1},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("warning"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("watch"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.HERE.Nwsalerts value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Warning[]>().Serialize(ref writer, value.warning, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Watch[]>().Serialize(ref writer, value.watch, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.HERE.Nwsalerts Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __warning__ = default(global::SimpleWeather.HERE.Warning[]);
            var __warning__b__ = false;
            var __watch__ = default(global::SimpleWeather.HERE.Watch[]);
            var __watch__b__ = false;

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
                        __warning__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Warning[]>().Deserialize(ref reader, formatterResolver);
                        __warning__b__ = true;
                        break;
                    case 1:
                        __watch__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Watch[]>().Deserialize(ref reader, formatterResolver);
                        __watch__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.HERE.Nwsalerts();
            if(__warning__b__) ____result.warning = __warning__;
            if(__watch__b__) ____result.watch = __watch__;

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
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("nwsAlerts"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("astronomy"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("feedCreation"), 6},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("metric"), 7},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("Type"), 8},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("Message"), 9},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("observations"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("dailyForecasts"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("hourlyForecasts"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("alerts"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("nwsAlerts"),
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
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Nwsalerts>().Serialize(ref writer, value.nwsAlerts, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[5]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Astronomy>().Serialize(ref writer, value.astronomy, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[6]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset>().Serialize(ref writer, value.feedCreation, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[7]);
            writer.WriteBoolean(value.metric);
            writer.WriteRaw(this.____stringByteKeys[8]);
            writer.WriteString(value.Type);
            writer.WriteRaw(this.____stringByteKeys[9]);
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
            var __nwsAlerts__ = default(global::SimpleWeather.HERE.Nwsalerts);
            var __nwsAlerts__b__ = false;
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
                        __nwsAlerts__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Nwsalerts>().Deserialize(ref reader, formatterResolver);
                        __nwsAlerts__b__ = true;
                        break;
                    case 5:
                        __astronomy__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.HERE.Astronomy>().Deserialize(ref reader, formatterResolver);
                        __astronomy__b__ = true;
                        break;
                    case 6:
                        __feedCreation__ = formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset>().Deserialize(ref reader, formatterResolver);
                        __feedCreation__b__ = true;
                        break;
                    case 7:
                        __metric__ = reader.ReadBoolean();
                        __metric__b__ = true;
                        break;
                    case 8:
                        __Type__ = reader.ReadString();
                        __Type__b__ = true;
                        break;
                    case 9:
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
            if(__nwsAlerts__b__) ____result.nwsAlerts = __nwsAlerts__;
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

namespace SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.Metno
{
    using System;
    using Utf8Json;


    public sealed class AstroMetaFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.Metno.AstroMeta>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public AstroMetaFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("licenseurl"), 0},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("licenseurl"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.Metno.AstroMeta value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.licenseurl);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.Metno.AstroMeta Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __licenseurl__ = default(string);
            var __licenseurl__b__ = false;

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
                        __licenseurl__ = reader.ReadString();
                        __licenseurl__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.Metno.AstroMeta();
            if(__licenseurl__b__) ____result.licenseurl = __licenseurl__;

            return ____result;
        }
    }


    public sealed class Low_MoonFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.Metno.Low_Moon>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public Low_MoonFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("elevation"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("time"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("desc"), 2},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("elevation"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("time"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("desc"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.Metno.Low_Moon value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.elevation);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Serialize(ref writer, value.time, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.desc);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.Metno.Low_Moon Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __elevation__ = default(string);
            var __elevation__b__ = false;
            var __time__ = default(global::System.DateTime);
            var __time__b__ = false;
            var __desc__ = default(string);
            var __desc__b__ = false;

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
                        __elevation__ = reader.ReadString();
                        __elevation__b__ = true;
                        break;
                    case 1:
                        __time__ = formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Deserialize(ref reader, formatterResolver);
                        __time__b__ = true;
                        break;
                    case 2:
                        __desc__ = reader.ReadString();
                        __desc__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.Metno.Low_Moon();
            if(__elevation__b__) ____result.elevation = __elevation__;
            if(__time__b__) ____result.time = __time__;
            if(__desc__b__) ____result.desc = __desc__;

            return ____result;
        }
    }


    public sealed class High_MoonFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.Metno.High_Moon>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public High_MoonFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("elevation"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("time"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("desc"), 2},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("elevation"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("time"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("desc"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.Metno.High_Moon value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.elevation);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Serialize(ref writer, value.time, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.desc);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.Metno.High_Moon Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __elevation__ = default(string);
            var __elevation__b__ = false;
            var __time__ = default(global::System.DateTime);
            var __time__b__ = false;
            var __desc__ = default(string);
            var __desc__b__ = false;

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
                        __elevation__ = reader.ReadString();
                        __elevation__b__ = true;
                        break;
                    case 1:
                        __time__ = formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Deserialize(ref reader, formatterResolver);
                        __time__b__ = true;
                        break;
                    case 2:
                        __desc__ = reader.ReadString();
                        __desc__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.Metno.High_Moon();
            if(__elevation__b__) ____result.elevation = __elevation__;
            if(__time__b__) ____result.time = __time__;
            if(__desc__b__) ____result.desc = __desc__;

            return ____result;
        }
    }


    public sealed class SolarnoonFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.Metno.Solarnoon>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public SolarnoonFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("desc"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("time"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("elevation"), 2},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("desc"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("time"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("elevation"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.Metno.Solarnoon value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.desc);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Serialize(ref writer, value.time, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.elevation);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.Metno.Solarnoon Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __desc__ = default(string);
            var __desc__b__ = false;
            var __time__ = default(global::System.DateTime);
            var __time__b__ = false;
            var __elevation__ = default(string);
            var __elevation__b__ = false;

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
                        __desc__ = reader.ReadString();
                        __desc__b__ = true;
                        break;
                    case 1:
                        __time__ = formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Deserialize(ref reader, formatterResolver);
                        __time__b__ = true;
                        break;
                    case 2:
                        __elevation__ = reader.ReadString();
                        __elevation__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.Metno.Solarnoon();
            if(__desc__b__) ____result.desc = __desc__;
            if(__time__b__) ____result.time = __time__;
            if(__elevation__b__) ____result.elevation = __elevation__;

            return ____result;
        }
    }


    public sealed class MoonphaseFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.Metno.Moonphase>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public MoonphaseFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("value"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("desc"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("time"), 2},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("value"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("desc"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("time"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.Metno.Moonphase value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.value);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.desc);
            writer.WriteRaw(this.____stringByteKeys[2]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Serialize(ref writer, value.time, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.Metno.Moonphase Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __value__ = default(string);
            var __value__b__ = false;
            var __desc__ = default(string);
            var __desc__b__ = false;
            var __time__ = default(global::System.DateTime);
            var __time__b__ = false;

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
                        __desc__ = reader.ReadString();
                        __desc__b__ = true;
                        break;
                    case 2:
                        __time__ = formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Deserialize(ref reader, formatterResolver);
                        __time__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.Metno.Moonphase();
            if(__value__b__) ____result.value = __value__;
            if(__desc__b__) ____result.desc = __desc__;
            if(__time__b__) ____result.time = __time__;

            return ____result;
        }
    }


    public sealed class MoonpositionFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.Metno.Moonposition>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public MoonpositionFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("desc"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("azimuth"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("phase"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("time"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("range"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("elevation"), 5},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("desc"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("azimuth"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("phase"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("time"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("range"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("elevation"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.Metno.Moonposition value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.desc);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.azimuth);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.phase);
            writer.WriteRaw(this.____stringByteKeys[3]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Serialize(ref writer, value.time, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteString(value.range);
            writer.WriteRaw(this.____stringByteKeys[5]);
            writer.WriteString(value.elevation);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.Metno.Moonposition Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __desc__ = default(string);
            var __desc__b__ = false;
            var __azimuth__ = default(string);
            var __azimuth__b__ = false;
            var __phase__ = default(string);
            var __phase__b__ = false;
            var __time__ = default(global::System.DateTime);
            var __time__b__ = false;
            var __range__ = default(string);
            var __range__b__ = false;
            var __elevation__ = default(string);
            var __elevation__b__ = false;

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
                        __desc__ = reader.ReadString();
                        __desc__b__ = true;
                        break;
                    case 1:
                        __azimuth__ = reader.ReadString();
                        __azimuth__b__ = true;
                        break;
                    case 2:
                        __phase__ = reader.ReadString();
                        __phase__b__ = true;
                        break;
                    case 3:
                        __time__ = formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Deserialize(ref reader, formatterResolver);
                        __time__b__ = true;
                        break;
                    case 4:
                        __range__ = reader.ReadString();
                        __range__b__ = true;
                        break;
                    case 5:
                        __elevation__ = reader.ReadString();
                        __elevation__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.Metno.Moonposition();
            if(__desc__b__) ____result.desc = __desc__;
            if(__azimuth__b__) ____result.azimuth = __azimuth__;
            if(__phase__b__) ____result.phase = __phase__;
            if(__time__b__) ____result.time = __time__;
            if(__range__b__) ____result.range = __range__;
            if(__elevation__b__) ____result.elevation = __elevation__;

            return ____result;
        }
    }


    public sealed class SunriseFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.Metno.Sunrise>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public SunriseFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("time"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("desc"), 1},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("time"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("desc"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.Metno.Sunrise value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Serialize(ref writer, value.time, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.desc);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.Metno.Sunrise Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __time__ = default(global::System.DateTime);
            var __time__b__ = false;
            var __desc__ = default(string);
            var __desc__b__ = false;

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
                        __time__ = formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Deserialize(ref reader, formatterResolver);
                        __time__b__ = true;
                        break;
                    case 1:
                        __desc__ = reader.ReadString();
                        __desc__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.Metno.Sunrise();
            if(__time__b__) ____result.time = __time__;
            if(__desc__b__) ____result.desc = __desc__;

            return ____result;
        }
    }


    public sealed class MoonshadowFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.Metno.Moonshadow>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public MoonshadowFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("elevation"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("time"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("azimuth"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("desc"), 3},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("elevation"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("time"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("azimuth"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("desc"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.Metno.Moonshadow value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.elevation);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Serialize(ref writer, value.time, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.azimuth);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteString(value.desc);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.Metno.Moonshadow Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __elevation__ = default(string);
            var __elevation__b__ = false;
            var __time__ = default(global::System.DateTime);
            var __time__b__ = false;
            var __azimuth__ = default(string);
            var __azimuth__b__ = false;
            var __desc__ = default(string);
            var __desc__b__ = false;

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
                        __elevation__ = reader.ReadString();
                        __elevation__b__ = true;
                        break;
                    case 1:
                        __time__ = formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Deserialize(ref reader, formatterResolver);
                        __time__b__ = true;
                        break;
                    case 2:
                        __azimuth__ = reader.ReadString();
                        __azimuth__b__ = true;
                        break;
                    case 3:
                        __desc__ = reader.ReadString();
                        __desc__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.Metno.Moonshadow();
            if(__elevation__b__) ____result.elevation = __elevation__;
            if(__time__b__) ____result.time = __time__;
            if(__azimuth__b__) ____result.azimuth = __azimuth__;
            if(__desc__b__) ____result.desc = __desc__;

            return ____result;
        }
    }


    public sealed class MoonriseFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.Metno.Moonrise>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public MoonriseFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("time"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("desc"), 1},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("time"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("desc"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.Metno.Moonrise value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Serialize(ref writer, value.time, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.desc);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.Metno.Moonrise Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __time__ = default(global::System.DateTime);
            var __time__b__ = false;
            var __desc__ = default(string);
            var __desc__b__ = false;

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
                        __time__ = formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Deserialize(ref reader, formatterResolver);
                        __time__b__ = true;
                        break;
                    case 1:
                        __desc__ = reader.ReadString();
                        __desc__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.Metno.Moonrise();
            if(__time__b__) ____result.time = __time__;
            if(__desc__b__) ____result.desc = __desc__;

            return ____result;
        }
    }


    public sealed class SolarmidnightFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.Metno.Solarmidnight>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public SolarmidnightFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("time"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("elevation"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("desc"), 2},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("time"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("elevation"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("desc"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.Metno.Solarmidnight value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Serialize(ref writer, value.time, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.elevation);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.desc);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.Metno.Solarmidnight Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __time__ = default(global::System.DateTime);
            var __time__b__ = false;
            var __elevation__ = default(string);
            var __elevation__b__ = false;
            var __desc__ = default(string);
            var __desc__b__ = false;

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
                        __time__ = formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Deserialize(ref reader, formatterResolver);
                        __time__b__ = true;
                        break;
                    case 1:
                        __elevation__ = reader.ReadString();
                        __elevation__b__ = true;
                        break;
                    case 2:
                        __desc__ = reader.ReadString();
                        __desc__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.Metno.Solarmidnight();
            if(__time__b__) ____result.time = __time__;
            if(__elevation__b__) ____result.elevation = __elevation__;
            if(__desc__b__) ____result.desc = __desc__;

            return ____result;
        }
    }


    public sealed class SunsetFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.Metno.Sunset>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public SunsetFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("time"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("desc"), 1},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("time"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("desc"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.Metno.Sunset value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Serialize(ref writer, value.time, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.desc);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.Metno.Sunset Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __time__ = default(global::System.DateTime);
            var __time__b__ = false;
            var __desc__ = default(string);
            var __desc__b__ = false;

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
                        __time__ = formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Deserialize(ref reader, formatterResolver);
                        __time__b__ = true;
                        break;
                    case 1:
                        __desc__ = reader.ReadString();
                        __desc__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.Metno.Sunset();
            if(__time__b__) ____result.time = __time__;
            if(__desc__b__) ____result.desc = __desc__;

            return ____result;
        }
    }


    public sealed class MoonsetFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.Metno.Moonset>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public MoonsetFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("time"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("desc"), 1},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("time"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("desc"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.Metno.Moonset value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Serialize(ref writer, value.time, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.desc);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.Metno.Moonset Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __time__ = default(global::System.DateTime);
            var __time__b__ = false;
            var __desc__ = default(string);
            var __desc__b__ = false;

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
                        __time__ = formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Deserialize(ref reader, formatterResolver);
                        __time__b__ = true;
                        break;
                    case 1:
                        __desc__ = reader.ReadString();
                        __desc__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.Metno.Moonset();
            if(__time__b__) ____result.time = __time__;
            if(__desc__b__) ____result.desc = __desc__;

            return ____result;
        }
    }


    public sealed class TimeFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.Metno.Time>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public TimeFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("low_moon"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("high_moon"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("solarnoon"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("moonphase"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("moonposition"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("sunrise"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("date"), 6},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("moonshadow"), 7},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("moonrise"), 8},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("solarmidnight"), 9},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("sunset"), 10},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("moonset"), 11},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("low_moon"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("high_moon"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("solarnoon"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("moonphase"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("moonposition"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("sunrise"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("date"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("moonshadow"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("moonrise"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("solarmidnight"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("sunset"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("moonset"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.Metno.Time value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Low_Moon>().Serialize(ref writer, value.low_moon, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.High_Moon>().Serialize(ref writer, value.high_moon, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[2]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Solarnoon>().Serialize(ref writer, value.solarnoon, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[3]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Moonphase>().Serialize(ref writer, value.moonphase, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[4]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Moonposition>().Serialize(ref writer, value.moonposition, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[5]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Sunrise>().Serialize(ref writer, value.sunrise, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[6]);
            writer.WriteString(value.date);
            writer.WriteRaw(this.____stringByteKeys[7]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Moonshadow>().Serialize(ref writer, value.moonshadow, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[8]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Moonrise>().Serialize(ref writer, value.moonrise, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[9]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Solarmidnight>().Serialize(ref writer, value.solarmidnight, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[10]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Sunset>().Serialize(ref writer, value.sunset, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[11]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Moonset>().Serialize(ref writer, value.moonset, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.Metno.Time Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __low_moon__ = default(global::SimpleWeather.Metno.Low_Moon);
            var __low_moon__b__ = false;
            var __high_moon__ = default(global::SimpleWeather.Metno.High_Moon);
            var __high_moon__b__ = false;
            var __solarnoon__ = default(global::SimpleWeather.Metno.Solarnoon);
            var __solarnoon__b__ = false;
            var __moonphase__ = default(global::SimpleWeather.Metno.Moonphase);
            var __moonphase__b__ = false;
            var __moonposition__ = default(global::SimpleWeather.Metno.Moonposition);
            var __moonposition__b__ = false;
            var __sunrise__ = default(global::SimpleWeather.Metno.Sunrise);
            var __sunrise__b__ = false;
            var __date__ = default(string);
            var __date__b__ = false;
            var __moonshadow__ = default(global::SimpleWeather.Metno.Moonshadow);
            var __moonshadow__b__ = false;
            var __moonrise__ = default(global::SimpleWeather.Metno.Moonrise);
            var __moonrise__b__ = false;
            var __solarmidnight__ = default(global::SimpleWeather.Metno.Solarmidnight);
            var __solarmidnight__b__ = false;
            var __sunset__ = default(global::SimpleWeather.Metno.Sunset);
            var __sunset__b__ = false;
            var __moonset__ = default(global::SimpleWeather.Metno.Moonset);
            var __moonset__b__ = false;

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
                        __low_moon__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Low_Moon>().Deserialize(ref reader, formatterResolver);
                        __low_moon__b__ = true;
                        break;
                    case 1:
                        __high_moon__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.High_Moon>().Deserialize(ref reader, formatterResolver);
                        __high_moon__b__ = true;
                        break;
                    case 2:
                        __solarnoon__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Solarnoon>().Deserialize(ref reader, formatterResolver);
                        __solarnoon__b__ = true;
                        break;
                    case 3:
                        __moonphase__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Moonphase>().Deserialize(ref reader, formatterResolver);
                        __moonphase__b__ = true;
                        break;
                    case 4:
                        __moonposition__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Moonposition>().Deserialize(ref reader, formatterResolver);
                        __moonposition__b__ = true;
                        break;
                    case 5:
                        __sunrise__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Sunrise>().Deserialize(ref reader, formatterResolver);
                        __sunrise__b__ = true;
                        break;
                    case 6:
                        __date__ = reader.ReadString();
                        __date__b__ = true;
                        break;
                    case 7:
                        __moonshadow__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Moonshadow>().Deserialize(ref reader, formatterResolver);
                        __moonshadow__b__ = true;
                        break;
                    case 8:
                        __moonrise__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Moonrise>().Deserialize(ref reader, formatterResolver);
                        __moonrise__b__ = true;
                        break;
                    case 9:
                        __solarmidnight__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Solarmidnight>().Deserialize(ref reader, formatterResolver);
                        __solarmidnight__b__ = true;
                        break;
                    case 10:
                        __sunset__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Sunset>().Deserialize(ref reader, formatterResolver);
                        __sunset__b__ = true;
                        break;
                    case 11:
                        __moonset__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Moonset>().Deserialize(ref reader, formatterResolver);
                        __moonset__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.Metno.Time();
            if(__low_moon__b__) ____result.low_moon = __low_moon__;
            if(__high_moon__b__) ____result.high_moon = __high_moon__;
            if(__solarnoon__b__) ____result.solarnoon = __solarnoon__;
            if(__moonphase__b__) ____result.moonphase = __moonphase__;
            if(__moonposition__b__) ____result.moonposition = __moonposition__;
            if(__sunrise__b__) ____result.sunrise = __sunrise__;
            if(__date__b__) ____result.date = __date__;
            if(__moonshadow__b__) ____result.moonshadow = __moonshadow__;
            if(__moonrise__b__) ____result.moonrise = __moonrise__;
            if(__solarmidnight__b__) ____result.solarmidnight = __solarmidnight__;
            if(__sunset__b__) ____result.sunset = __sunset__;
            if(__moonset__b__) ____result.moonset = __moonset__;

            return ____result;
        }
    }


    public sealed class LocationFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.Metno.Location>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public LocationFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("time"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("longitude"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("latitude"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("height"), 3},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("time"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("longitude"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("latitude"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("height"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.Metno.Location value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Time[]>().Serialize(ref writer, value.time, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.longitude);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.latitude);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteString(value.height);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.Metno.Location Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __time__ = default(global::SimpleWeather.Metno.Time[]);
            var __time__b__ = false;
            var __longitude__ = default(string);
            var __longitude__b__ = false;
            var __latitude__ = default(string);
            var __latitude__b__ = false;
            var __height__ = default(string);
            var __height__b__ = false;

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
                        __time__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Time[]>().Deserialize(ref reader, formatterResolver);
                        __time__b__ = true;
                        break;
                    case 1:
                        __longitude__ = reader.ReadString();
                        __longitude__b__ = true;
                        break;
                    case 2:
                        __latitude__ = reader.ReadString();
                        __latitude__b__ = true;
                        break;
                    case 3:
                        __height__ = reader.ReadString();
                        __height__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.Metno.Location();
            if(__time__b__) ____result.time = __time__;
            if(__longitude__b__) ____result.longitude = __longitude__;
            if(__latitude__b__) ____result.latitude = __latitude__;
            if(__height__b__) ____result.height = __height__;

            return ____result;
        }
    }


    public sealed class AstroRootobjectFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.Metno.AstroRootobject>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public AstroRootobjectFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("meta"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("location"), 1},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("meta"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("location"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.Metno.AstroRootobject value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.AstroMeta>().Serialize(ref writer, value.meta, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Location>().Serialize(ref writer, value.location, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.Metno.AstroRootobject Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __meta__ = default(global::SimpleWeather.Metno.AstroMeta);
            var __meta__b__ = false;
            var __location__ = default(global::SimpleWeather.Metno.Location);
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
                        __meta__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.AstroMeta>().Deserialize(ref reader, formatterResolver);
                        __meta__b__ = true;
                        break;
                    case 1:
                        __location__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Location>().Deserialize(ref reader, formatterResolver);
                        __location__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.Metno.AstroRootobject();
            if(__meta__b__) ____result.meta = __meta__;
            if(__location__b__) ____result.location = __location__;

            return ____result;
        }
    }


    public sealed class GeometryFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.Metno.Geometry>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public GeometryFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("type"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("coordinates"), 1},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("type"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("coordinates"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.Metno.Geometry value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.type);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<float[]>().Serialize(ref writer, value.coordinates, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.Metno.Geometry Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __type__ = default(string);
            var __type__b__ = false;
            var __coordinates__ = default(float[]);
            var __coordinates__b__ = false;

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
                        __type__ = reader.ReadString();
                        __type__b__ = true;
                        break;
                    case 1:
                        __coordinates__ = formatterResolver.GetFormatterWithVerify<float[]>().Deserialize(ref reader, formatterResolver);
                        __coordinates__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.Metno.Geometry();
            if(__type__b__) ____result.type = __type__;
            if(__coordinates__b__) ____result.coordinates = __coordinates__;

            return ____result;
        }
    }


    public sealed class UnitsFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.Metno.Units>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public UnitsFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("air_pressure_at_sea_level"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("air_temperature"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("air_temperature_max"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("air_temperature_min"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("cloud_area_fraction"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("cloud_area_fraction_high"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("cloud_area_fraction_low"), 6},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("cloud_area_fraction_medium"), 7},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("dew_point_temperature"), 8},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("fog_area_fraction"), 9},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("precipitation_amount"), 10},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("relative_humidity"), 11},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("ultraviolet_index_clear_sky"), 12},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("wind_from_direction"), 13},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("wind_speed"), 14},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("air_pressure_at_sea_level"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("air_temperature"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("air_temperature_max"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("air_temperature_min"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("cloud_area_fraction"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("cloud_area_fraction_high"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("cloud_area_fraction_low"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("cloud_area_fraction_medium"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("dew_point_temperature"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("fog_area_fraction"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("precipitation_amount"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("relative_humidity"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("ultraviolet_index_clear_sky"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("wind_from_direction"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("wind_speed"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.Metno.Units value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.air_pressure_at_sea_level);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.air_temperature);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.air_temperature_max);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteString(value.air_temperature_min);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteString(value.cloud_area_fraction);
            writer.WriteRaw(this.____stringByteKeys[5]);
            writer.WriteString(value.cloud_area_fraction_high);
            writer.WriteRaw(this.____stringByteKeys[6]);
            writer.WriteString(value.cloud_area_fraction_low);
            writer.WriteRaw(this.____stringByteKeys[7]);
            writer.WriteString(value.cloud_area_fraction_medium);
            writer.WriteRaw(this.____stringByteKeys[8]);
            writer.WriteString(value.dew_point_temperature);
            writer.WriteRaw(this.____stringByteKeys[9]);
            writer.WriteString(value.fog_area_fraction);
            writer.WriteRaw(this.____stringByteKeys[10]);
            writer.WriteString(value.precipitation_amount);
            writer.WriteRaw(this.____stringByteKeys[11]);
            writer.WriteString(value.relative_humidity);
            writer.WriteRaw(this.____stringByteKeys[12]);
            writer.WriteString(value.ultraviolet_index_clear_sky);
            writer.WriteRaw(this.____stringByteKeys[13]);
            writer.WriteString(value.wind_from_direction);
            writer.WriteRaw(this.____stringByteKeys[14]);
            writer.WriteString(value.wind_speed);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.Metno.Units Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __air_pressure_at_sea_level__ = default(string);
            var __air_pressure_at_sea_level__b__ = false;
            var __air_temperature__ = default(string);
            var __air_temperature__b__ = false;
            var __air_temperature_max__ = default(string);
            var __air_temperature_max__b__ = false;
            var __air_temperature_min__ = default(string);
            var __air_temperature_min__b__ = false;
            var __cloud_area_fraction__ = default(string);
            var __cloud_area_fraction__b__ = false;
            var __cloud_area_fraction_high__ = default(string);
            var __cloud_area_fraction_high__b__ = false;
            var __cloud_area_fraction_low__ = default(string);
            var __cloud_area_fraction_low__b__ = false;
            var __cloud_area_fraction_medium__ = default(string);
            var __cloud_area_fraction_medium__b__ = false;
            var __dew_point_temperature__ = default(string);
            var __dew_point_temperature__b__ = false;
            var __fog_area_fraction__ = default(string);
            var __fog_area_fraction__b__ = false;
            var __precipitation_amount__ = default(string);
            var __precipitation_amount__b__ = false;
            var __relative_humidity__ = default(string);
            var __relative_humidity__b__ = false;
            var __ultraviolet_index_clear_sky__ = default(string);
            var __ultraviolet_index_clear_sky__b__ = false;
            var __wind_from_direction__ = default(string);
            var __wind_from_direction__b__ = false;
            var __wind_speed__ = default(string);
            var __wind_speed__b__ = false;

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
                        __air_pressure_at_sea_level__ = reader.ReadString();
                        __air_pressure_at_sea_level__b__ = true;
                        break;
                    case 1:
                        __air_temperature__ = reader.ReadString();
                        __air_temperature__b__ = true;
                        break;
                    case 2:
                        __air_temperature_max__ = reader.ReadString();
                        __air_temperature_max__b__ = true;
                        break;
                    case 3:
                        __air_temperature_min__ = reader.ReadString();
                        __air_temperature_min__b__ = true;
                        break;
                    case 4:
                        __cloud_area_fraction__ = reader.ReadString();
                        __cloud_area_fraction__b__ = true;
                        break;
                    case 5:
                        __cloud_area_fraction_high__ = reader.ReadString();
                        __cloud_area_fraction_high__b__ = true;
                        break;
                    case 6:
                        __cloud_area_fraction_low__ = reader.ReadString();
                        __cloud_area_fraction_low__b__ = true;
                        break;
                    case 7:
                        __cloud_area_fraction_medium__ = reader.ReadString();
                        __cloud_area_fraction_medium__b__ = true;
                        break;
                    case 8:
                        __dew_point_temperature__ = reader.ReadString();
                        __dew_point_temperature__b__ = true;
                        break;
                    case 9:
                        __fog_area_fraction__ = reader.ReadString();
                        __fog_area_fraction__b__ = true;
                        break;
                    case 10:
                        __precipitation_amount__ = reader.ReadString();
                        __precipitation_amount__b__ = true;
                        break;
                    case 11:
                        __relative_humidity__ = reader.ReadString();
                        __relative_humidity__b__ = true;
                        break;
                    case 12:
                        __ultraviolet_index_clear_sky__ = reader.ReadString();
                        __ultraviolet_index_clear_sky__b__ = true;
                        break;
                    case 13:
                        __wind_from_direction__ = reader.ReadString();
                        __wind_from_direction__b__ = true;
                        break;
                    case 14:
                        __wind_speed__ = reader.ReadString();
                        __wind_speed__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.Metno.Units();
            if(__air_pressure_at_sea_level__b__) ____result.air_pressure_at_sea_level = __air_pressure_at_sea_level__;
            if(__air_temperature__b__) ____result.air_temperature = __air_temperature__;
            if(__air_temperature_max__b__) ____result.air_temperature_max = __air_temperature_max__;
            if(__air_temperature_min__b__) ____result.air_temperature_min = __air_temperature_min__;
            if(__cloud_area_fraction__b__) ____result.cloud_area_fraction = __cloud_area_fraction__;
            if(__cloud_area_fraction_high__b__) ____result.cloud_area_fraction_high = __cloud_area_fraction_high__;
            if(__cloud_area_fraction_low__b__) ____result.cloud_area_fraction_low = __cloud_area_fraction_low__;
            if(__cloud_area_fraction_medium__b__) ____result.cloud_area_fraction_medium = __cloud_area_fraction_medium__;
            if(__dew_point_temperature__b__) ____result.dew_point_temperature = __dew_point_temperature__;
            if(__fog_area_fraction__b__) ____result.fog_area_fraction = __fog_area_fraction__;
            if(__precipitation_amount__b__) ____result.precipitation_amount = __precipitation_amount__;
            if(__relative_humidity__b__) ____result.relative_humidity = __relative_humidity__;
            if(__ultraviolet_index_clear_sky__b__) ____result.ultraviolet_index_clear_sky = __ultraviolet_index_clear_sky__;
            if(__wind_from_direction__b__) ____result.wind_from_direction = __wind_from_direction__;
            if(__wind_speed__b__) ____result.wind_speed = __wind_speed__;

            return ____result;
        }
    }


    public sealed class MetaFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.Metno.Meta>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public MetaFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("updated_at"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("units"), 1},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("updated_at"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("units"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.Metno.Meta value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Serialize(ref writer, value.updated_at, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Units>().Serialize(ref writer, value.units, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.Metno.Meta Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __updated_at__ = default(global::System.DateTime);
            var __updated_at__b__ = false;
            var __units__ = default(global::SimpleWeather.Metno.Units);
            var __units__b__ = false;

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
                        __updated_at__ = formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Deserialize(ref reader, formatterResolver);
                        __updated_at__b__ = true;
                        break;
                    case 1:
                        __units__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Units>().Deserialize(ref reader, formatterResolver);
                        __units__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.Metno.Meta();
            if(__updated_at__b__) ____result.updated_at = __updated_at__;
            if(__units__b__) ____result.units = __units__;

            return ____result;
        }
    }


    public sealed class DetailsFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.Metno.Details>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public DetailsFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("air_pressure_at_sea_level"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("air_temperature"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("cloud_area_fraction"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("cloud_area_fraction_high"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("cloud_area_fraction_low"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("cloud_area_fraction_medium"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("dew_point_temperature"), 6},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("fog_area_fraction"), 7},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("probability_of_precipitation"), 8},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("relative_humidity"), 9},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("ultraviolet_index_clear_sky"), 10},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("wind_from_direction"), 11},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("wind_speed"), 12},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("wind_speed_of_gust"), 13},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("air_pressure_at_sea_level"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("air_temperature"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("cloud_area_fraction"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("cloud_area_fraction_high"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("cloud_area_fraction_low"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("cloud_area_fraction_medium"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("dew_point_temperature"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("fog_area_fraction"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("probability_of_precipitation"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("relative_humidity"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("ultraviolet_index_clear_sky"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("wind_from_direction"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("wind_speed"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("wind_speed_of_gust"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.Metno.Details value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.air_pressure_at_sea_level, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.air_temperature, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[2]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.cloud_area_fraction, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[3]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.cloud_area_fraction_high, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[4]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.cloud_area_fraction_low, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[5]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.cloud_area_fraction_medium, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[6]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.dew_point_temperature, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[7]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.fog_area_fraction, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[8]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.probability_of_precipitation, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[9]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.relative_humidity, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[10]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.ultraviolet_index_clear_sky, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[11]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.wind_from_direction, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[12]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.wind_speed, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[13]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.wind_speed_of_gust, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.Metno.Details Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __air_pressure_at_sea_level__ = default(float?);
            var __air_pressure_at_sea_level__b__ = false;
            var __air_temperature__ = default(float?);
            var __air_temperature__b__ = false;
            var __cloud_area_fraction__ = default(float?);
            var __cloud_area_fraction__b__ = false;
            var __cloud_area_fraction_high__ = default(float?);
            var __cloud_area_fraction_high__b__ = false;
            var __cloud_area_fraction_low__ = default(float?);
            var __cloud_area_fraction_low__b__ = false;
            var __cloud_area_fraction_medium__ = default(float?);
            var __cloud_area_fraction_medium__b__ = false;
            var __dew_point_temperature__ = default(float?);
            var __dew_point_temperature__b__ = false;
            var __fog_area_fraction__ = default(float?);
            var __fog_area_fraction__b__ = false;
            var __probability_of_precipitation__ = default(float?);
            var __probability_of_precipitation__b__ = false;
            var __relative_humidity__ = default(float?);
            var __relative_humidity__b__ = false;
            var __ultraviolet_index_clear_sky__ = default(float?);
            var __ultraviolet_index_clear_sky__b__ = false;
            var __wind_from_direction__ = default(float?);
            var __wind_from_direction__b__ = false;
            var __wind_speed__ = default(float?);
            var __wind_speed__b__ = false;
            var __wind_speed_of_gust__ = default(float?);
            var __wind_speed_of_gust__b__ = false;

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
                        __air_pressure_at_sea_level__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __air_pressure_at_sea_level__b__ = true;
                        break;
                    case 1:
                        __air_temperature__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __air_temperature__b__ = true;
                        break;
                    case 2:
                        __cloud_area_fraction__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __cloud_area_fraction__b__ = true;
                        break;
                    case 3:
                        __cloud_area_fraction_high__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __cloud_area_fraction_high__b__ = true;
                        break;
                    case 4:
                        __cloud_area_fraction_low__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __cloud_area_fraction_low__b__ = true;
                        break;
                    case 5:
                        __cloud_area_fraction_medium__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __cloud_area_fraction_medium__b__ = true;
                        break;
                    case 6:
                        __dew_point_temperature__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __dew_point_temperature__b__ = true;
                        break;
                    case 7:
                        __fog_area_fraction__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __fog_area_fraction__b__ = true;
                        break;
                    case 8:
                        __probability_of_precipitation__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __probability_of_precipitation__b__ = true;
                        break;
                    case 9:
                        __relative_humidity__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __relative_humidity__b__ = true;
                        break;
                    case 10:
                        __ultraviolet_index_clear_sky__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __ultraviolet_index_clear_sky__b__ = true;
                        break;
                    case 11:
                        __wind_from_direction__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __wind_from_direction__b__ = true;
                        break;
                    case 12:
                        __wind_speed__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __wind_speed__b__ = true;
                        break;
                    case 13:
                        __wind_speed_of_gust__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __wind_speed_of_gust__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.Metno.Details();
            if(__air_pressure_at_sea_level__b__) ____result.air_pressure_at_sea_level = __air_pressure_at_sea_level__;
            if(__air_temperature__b__) ____result.air_temperature = __air_temperature__;
            if(__cloud_area_fraction__b__) ____result.cloud_area_fraction = __cloud_area_fraction__;
            if(__cloud_area_fraction_high__b__) ____result.cloud_area_fraction_high = __cloud_area_fraction_high__;
            if(__cloud_area_fraction_low__b__) ____result.cloud_area_fraction_low = __cloud_area_fraction_low__;
            if(__cloud_area_fraction_medium__b__) ____result.cloud_area_fraction_medium = __cloud_area_fraction_medium__;
            if(__dew_point_temperature__b__) ____result.dew_point_temperature = __dew_point_temperature__;
            if(__fog_area_fraction__b__) ____result.fog_area_fraction = __fog_area_fraction__;
            if(__probability_of_precipitation__b__) ____result.probability_of_precipitation = __probability_of_precipitation__;
            if(__relative_humidity__b__) ____result.relative_humidity = __relative_humidity__;
            if(__ultraviolet_index_clear_sky__b__) ____result.ultraviolet_index_clear_sky = __ultraviolet_index_clear_sky__;
            if(__wind_from_direction__b__) ____result.wind_from_direction = __wind_from_direction__;
            if(__wind_speed__b__) ____result.wind_speed = __wind_speed__;
            if(__wind_speed_of_gust__b__) ____result.wind_speed_of_gust = __wind_speed_of_gust__;

            return ____result;
        }
    }


    public sealed class InstantFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.Metno.Instant>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public InstantFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("details"), 0},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("details"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.Metno.Instant value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Details>().Serialize(ref writer, value.details, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.Metno.Instant Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __details__ = default(global::SimpleWeather.Metno.Details);
            var __details__b__ = false;

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
                        __details__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Details>().Deserialize(ref reader, formatterResolver);
                        __details__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.Metno.Instant();
            if(__details__b__) ____result.details = __details__;

            return ____result;
        }
    }


    public sealed class SummaryFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.Metno.Summary>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public SummaryFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("symbol_code"), 0},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("symbol_code"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.Metno.Summary value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.symbol_code);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.Metno.Summary Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __symbol_code__ = default(string);
            var __symbol_code__b__ = false;

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
                        __symbol_code__ = reader.ReadString();
                        __symbol_code__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.Metno.Summary();
            if(__symbol_code__b__) ____result.symbol_code = __symbol_code__;

            return ____result;
        }
    }


    public sealed class Details1Formatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.Metno.Details1>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public Details1Formatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("probability_of_precipitation"), 0},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("probability_of_precipitation"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.Metno.Details1 value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.probability_of_precipitation, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.Metno.Details1 Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __probability_of_precipitation__ = default(float?);
            var __probability_of_precipitation__b__ = false;

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
                        __probability_of_precipitation__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __probability_of_precipitation__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.Metno.Details1();
            if(__probability_of_precipitation__b__) ____result.probability_of_precipitation = __probability_of_precipitation__;

            return ____result;
        }
    }


    public sealed class Next_12_HoursFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.Metno.Next_12_Hours>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public Next_12_HoursFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("summary"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("details"), 1},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("summary"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("details"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.Metno.Next_12_Hours value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Summary>().Serialize(ref writer, value.summary, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Details1>().Serialize(ref writer, value.details, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.Metno.Next_12_Hours Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __summary__ = default(global::SimpleWeather.Metno.Summary);
            var __summary__b__ = false;
            var __details__ = default(global::SimpleWeather.Metno.Details1);
            var __details__b__ = false;

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
                        __summary__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Summary>().Deserialize(ref reader, formatterResolver);
                        __summary__b__ = true;
                        break;
                    case 1:
                        __details__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Details1>().Deserialize(ref reader, formatterResolver);
                        __details__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.Metno.Next_12_Hours();
            if(__summary__b__) ____result.summary = __summary__;
            if(__details__b__) ____result.details = __details__;

            return ____result;
        }
    }


    public sealed class Summary1Formatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.Metno.Summary1>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public Summary1Formatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("symbol_code"), 0},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("symbol_code"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.Metno.Summary1 value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.symbol_code);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.Metno.Summary1 Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __symbol_code__ = default(string);
            var __symbol_code__b__ = false;

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
                        __symbol_code__ = reader.ReadString();
                        __symbol_code__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.Metno.Summary1();
            if(__symbol_code__b__) ____result.symbol_code = __symbol_code__;

            return ____result;
        }
    }


    public sealed class Details2Formatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.Metno.Details2>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public Details2Formatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("precipitation_amount"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("precipitation_amount_max"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("precipitation_amount_min"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("probability_of_precipitation"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("probability_of_thunder"), 4},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("precipitation_amount"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("precipitation_amount_max"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("precipitation_amount_min"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("probability_of_precipitation"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("probability_of_thunder"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.Metno.Details2 value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.precipitation_amount, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.precipitation_amount_max, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[2]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.precipitation_amount_min, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[3]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.probability_of_precipitation, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[4]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.probability_of_thunder, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.Metno.Details2 Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __precipitation_amount__ = default(float?);
            var __precipitation_amount__b__ = false;
            var __precipitation_amount_max__ = default(float?);
            var __precipitation_amount_max__b__ = false;
            var __precipitation_amount_min__ = default(float?);
            var __precipitation_amount_min__b__ = false;
            var __probability_of_precipitation__ = default(float?);
            var __probability_of_precipitation__b__ = false;
            var __probability_of_thunder__ = default(float?);
            var __probability_of_thunder__b__ = false;

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
                        __precipitation_amount__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __precipitation_amount__b__ = true;
                        break;
                    case 1:
                        __precipitation_amount_max__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __precipitation_amount_max__b__ = true;
                        break;
                    case 2:
                        __precipitation_amount_min__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __precipitation_amount_min__b__ = true;
                        break;
                    case 3:
                        __probability_of_precipitation__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __probability_of_precipitation__b__ = true;
                        break;
                    case 4:
                        __probability_of_thunder__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __probability_of_thunder__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.Metno.Details2();
            if(__precipitation_amount__b__) ____result.precipitation_amount = __precipitation_amount__;
            if(__precipitation_amount_max__b__) ____result.precipitation_amount_max = __precipitation_amount_max__;
            if(__precipitation_amount_min__b__) ____result.precipitation_amount_min = __precipitation_amount_min__;
            if(__probability_of_precipitation__b__) ____result.probability_of_precipitation = __probability_of_precipitation__;
            if(__probability_of_thunder__b__) ____result.probability_of_thunder = __probability_of_thunder__;

            return ____result;
        }
    }


    public sealed class Next_1_HoursFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.Metno.Next_1_Hours>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public Next_1_HoursFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("summary"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("details"), 1},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("summary"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("details"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.Metno.Next_1_Hours value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Summary1>().Serialize(ref writer, value.summary, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Details2>().Serialize(ref writer, value.details, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.Metno.Next_1_Hours Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __summary__ = default(global::SimpleWeather.Metno.Summary1);
            var __summary__b__ = false;
            var __details__ = default(global::SimpleWeather.Metno.Details2);
            var __details__b__ = false;

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
                        __summary__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Summary1>().Deserialize(ref reader, formatterResolver);
                        __summary__b__ = true;
                        break;
                    case 1:
                        __details__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Details2>().Deserialize(ref reader, formatterResolver);
                        __details__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.Metno.Next_1_Hours();
            if(__summary__b__) ____result.summary = __summary__;
            if(__details__b__) ____result.details = __details__;

            return ____result;
        }
    }


    public sealed class Summary2Formatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.Metno.Summary2>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public Summary2Formatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("symbol_code"), 0},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("symbol_code"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.Metno.Summary2 value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.symbol_code);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.Metno.Summary2 Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __symbol_code__ = default(string);
            var __symbol_code__b__ = false;

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
                        __symbol_code__ = reader.ReadString();
                        __symbol_code__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.Metno.Summary2();
            if(__symbol_code__b__) ____result.symbol_code = __symbol_code__;

            return ____result;
        }
    }


    public sealed class Details3Formatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.Metno.Details3>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public Details3Formatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("air_temperature_max"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("air_temperature_min"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("precipitation_amount"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("precipitation_amount_max"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("precipitation_amount_min"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("probability_of_precipitation"), 5},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("air_temperature_max"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("air_temperature_min"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("precipitation_amount"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("precipitation_amount_max"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("precipitation_amount_min"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("probability_of_precipitation"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.Metno.Details3 value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.air_temperature_max, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.air_temperature_min, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[2]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.precipitation_amount, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[3]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.precipitation_amount_max, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[4]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.precipitation_amount_min, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[5]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.probability_of_precipitation, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.Metno.Details3 Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __air_temperature_max__ = default(float?);
            var __air_temperature_max__b__ = false;
            var __air_temperature_min__ = default(float?);
            var __air_temperature_min__b__ = false;
            var __precipitation_amount__ = default(float?);
            var __precipitation_amount__b__ = false;
            var __precipitation_amount_max__ = default(float?);
            var __precipitation_amount_max__b__ = false;
            var __precipitation_amount_min__ = default(float?);
            var __precipitation_amount_min__b__ = false;
            var __probability_of_precipitation__ = default(float?);
            var __probability_of_precipitation__b__ = false;

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
                        __air_temperature_max__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __air_temperature_max__b__ = true;
                        break;
                    case 1:
                        __air_temperature_min__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __air_temperature_min__b__ = true;
                        break;
                    case 2:
                        __precipitation_amount__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __precipitation_amount__b__ = true;
                        break;
                    case 3:
                        __precipitation_amount_max__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __precipitation_amount_max__b__ = true;
                        break;
                    case 4:
                        __precipitation_amount_min__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __precipitation_amount_min__b__ = true;
                        break;
                    case 5:
                        __probability_of_precipitation__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __probability_of_precipitation__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.Metno.Details3();
            if(__air_temperature_max__b__) ____result.air_temperature_max = __air_temperature_max__;
            if(__air_temperature_min__b__) ____result.air_temperature_min = __air_temperature_min__;
            if(__precipitation_amount__b__) ____result.precipitation_amount = __precipitation_amount__;
            if(__precipitation_amount_max__b__) ____result.precipitation_amount_max = __precipitation_amount_max__;
            if(__precipitation_amount_min__b__) ____result.precipitation_amount_min = __precipitation_amount_min__;
            if(__probability_of_precipitation__b__) ____result.probability_of_precipitation = __probability_of_precipitation__;

            return ____result;
        }
    }


    public sealed class Next_6_HoursFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.Metno.Next_6_Hours>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public Next_6_HoursFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("summary"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("details"), 1},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("summary"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("details"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.Metno.Next_6_Hours value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Summary2>().Serialize(ref writer, value.summary, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Details3>().Serialize(ref writer, value.details, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.Metno.Next_6_Hours Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __summary__ = default(global::SimpleWeather.Metno.Summary2);
            var __summary__b__ = false;
            var __details__ = default(global::SimpleWeather.Metno.Details3);
            var __details__b__ = false;

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
                        __summary__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Summary2>().Deserialize(ref reader, formatterResolver);
                        __summary__b__ = true;
                        break;
                    case 1:
                        __details__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Details3>().Deserialize(ref reader, formatterResolver);
                        __details__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.Metno.Next_6_Hours();
            if(__summary__b__) ____result.summary = __summary__;
            if(__details__b__) ____result.details = __details__;

            return ____result;
        }
    }


    public sealed class DataFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.Metno.Data>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public DataFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("instant"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("next_12_hours"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("next_1_hours"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("next_6_hours"), 3},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("instant"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("next_12_hours"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("next_1_hours"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("next_6_hours"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.Metno.Data value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Instant>().Serialize(ref writer, value.instant, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Next_12_Hours>().Serialize(ref writer, value.next_12_hours, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[2]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Next_1_Hours>().Serialize(ref writer, value.next_1_hours, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[3]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Next_6_Hours>().Serialize(ref writer, value.next_6_hours, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.Metno.Data Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __instant__ = default(global::SimpleWeather.Metno.Instant);
            var __instant__b__ = false;
            var __next_12_hours__ = default(global::SimpleWeather.Metno.Next_12_Hours);
            var __next_12_hours__b__ = false;
            var __next_1_hours__ = default(global::SimpleWeather.Metno.Next_1_Hours);
            var __next_1_hours__b__ = false;
            var __next_6_hours__ = default(global::SimpleWeather.Metno.Next_6_Hours);
            var __next_6_hours__b__ = false;

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
                        __instant__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Instant>().Deserialize(ref reader, formatterResolver);
                        __instant__b__ = true;
                        break;
                    case 1:
                        __next_12_hours__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Next_12_Hours>().Deserialize(ref reader, formatterResolver);
                        __next_12_hours__b__ = true;
                        break;
                    case 2:
                        __next_1_hours__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Next_1_Hours>().Deserialize(ref reader, formatterResolver);
                        __next_1_hours__b__ = true;
                        break;
                    case 3:
                        __next_6_hours__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Next_6_Hours>().Deserialize(ref reader, formatterResolver);
                        __next_6_hours__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.Metno.Data();
            if(__instant__b__) ____result.instant = __instant__;
            if(__next_12_hours__b__) ____result.next_12_hours = __next_12_hours__;
            if(__next_1_hours__b__) ____result.next_1_hours = __next_1_hours__;
            if(__next_6_hours__b__) ____result.next_6_hours = __next_6_hours__;

            return ____result;
        }
    }


    public sealed class TimeseryFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.Metno.Timesery>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public TimeseryFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("time"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("data"), 1},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("time"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("data"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.Metno.Timesery value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Serialize(ref writer, value.time, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Data>().Serialize(ref writer, value.data, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.Metno.Timesery Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __time__ = default(global::System.DateTime);
            var __time__b__ = false;
            var __data__ = default(global::SimpleWeather.Metno.Data);
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
                        __time__ = formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Deserialize(ref reader, formatterResolver);
                        __time__b__ = true;
                        break;
                    case 1:
                        __data__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Data>().Deserialize(ref reader, formatterResolver);
                        __data__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.Metno.Timesery();
            if(__time__b__) ____result.time = __time__;
            if(__data__b__) ____result.data = __data__;

            return ____result;
        }
    }


    public sealed class PropertiesFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.Metno.Properties>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public PropertiesFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("meta"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("timeseries"), 1},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("meta"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("timeseries"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.Metno.Properties value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Meta>().Serialize(ref writer, value.meta, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Timesery[]>().Serialize(ref writer, value.timeseries, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.Metno.Properties Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __meta__ = default(global::SimpleWeather.Metno.Meta);
            var __meta__b__ = false;
            var __timeseries__ = default(global::SimpleWeather.Metno.Timesery[]);
            var __timeseries__b__ = false;

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
                        __meta__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Meta>().Deserialize(ref reader, formatterResolver);
                        __meta__b__ = true;
                        break;
                    case 1:
                        __timeseries__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Timesery[]>().Deserialize(ref reader, formatterResolver);
                        __timeseries__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.Metno.Properties();
            if(__meta__b__) ____result.meta = __meta__;
            if(__timeseries__b__) ____result.timeseries = __timeseries__;

            return ____result;
        }
    }


    public sealed class RootobjectFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.Metno.Rootobject>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public RootobjectFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("type"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("geometry"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("properties"), 2},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("type"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("geometry"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("properties"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.Metno.Rootobject value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.type);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Geometry>().Serialize(ref writer, value.geometry, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[2]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Properties>().Serialize(ref writer, value.properties, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.Metno.Rootobject Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __type__ = default(string);
            var __type__b__ = false;
            var __geometry__ = default(global::SimpleWeather.Metno.Geometry);
            var __geometry__b__ = false;
            var __properties__ = default(global::SimpleWeather.Metno.Properties);
            var __properties__b__ = false;

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
                        __type__ = reader.ReadString();
                        __type__b__ = true;
                        break;
                    case 1:
                        __geometry__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Geometry>().Deserialize(ref reader, formatterResolver);
                        __geometry__b__ = true;
                        break;
                    case 2:
                        __properties__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.Metno.Properties>().Deserialize(ref reader, formatterResolver);
                        __properties__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.Metno.Rootobject();
            if(__type__b__) ____result.type = __type__;
            if(__geometry__b__) ____result.geometry = __geometry__;
            if(__properties__b__) ____result.properties = __properties__;

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


    internal sealed class SolCalcAstroProvider_AstroDataFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.NWS.SolCalcAstroProvider.AstroData>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public SolCalcAstroProvider_AstroDataFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("sunriseUTC"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("sunsetUTC"), 1},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("sunriseUTC"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("sunsetUTC"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.NWS.SolCalcAstroProvider.AstroData value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Serialize(ref writer, value.sunriseUTC, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Serialize(ref writer, value.sunsetUTC, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.NWS.SolCalcAstroProvider.AstroData Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __sunriseUTC__ = default(global::System.DateTime);
            var __sunriseUTC__b__ = false;
            var __sunsetUTC__ = default(global::System.DateTime);
            var __sunsetUTC__b__ = false;

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
                        __sunriseUTC__ = formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Deserialize(ref reader, formatterResolver);
                        __sunriseUTC__b__ = true;
                        break;
                    case 1:
                        __sunsetUTC__ = formatterResolver.GetFormatterWithVerify<global::System.DateTime>().Deserialize(ref reader, formatterResolver);
                        __sunsetUTC__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.NWS.SolCalcAstroProvider.AstroData();
            if(__sunriseUTC__b__) ____result.sunriseUTC = __sunriseUTC__;
            if(__sunsetUTC__b__) ____result.sunsetUTC = __sunsetUTC__;

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

namespace SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.NWS.Observation
{
    using System;
    using Utf8Json;


    public sealed class LocationFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.NWS.Observation.Location>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public LocationFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("region"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("latitude"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("longitude"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("elevation"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("wfo"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("timezone"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("areaDescription"), 6},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("radar"), 7},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("zone"), 8},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("county"), 9},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("firezone"), 10},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("metar"), 11},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("region"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("latitude"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("longitude"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("elevation"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("wfo"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("timezone"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("areaDescription"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("radar"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("zone"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("county"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("firezone"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("metar"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.NWS.Observation.Location value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.region);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.latitude);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.longitude);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteString(value.elevation);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteString(value.wfo);
            writer.WriteRaw(this.____stringByteKeys[5]);
            writer.WriteString(value.timezone);
            writer.WriteRaw(this.____stringByteKeys[6]);
            writer.WriteString(value.areaDescription);
            writer.WriteRaw(this.____stringByteKeys[7]);
            writer.WriteString(value.radar);
            writer.WriteRaw(this.____stringByteKeys[8]);
            writer.WriteString(value.zone);
            writer.WriteRaw(this.____stringByteKeys[9]);
            writer.WriteString(value.county);
            writer.WriteRaw(this.____stringByteKeys[10]);
            writer.WriteString(value.firezone);
            writer.WriteRaw(this.____stringByteKeys[11]);
            writer.WriteString(value.metar);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.NWS.Observation.Location Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __region__ = default(string);
            var __region__b__ = false;
            var __latitude__ = default(string);
            var __latitude__b__ = false;
            var __longitude__ = default(string);
            var __longitude__b__ = false;
            var __elevation__ = default(string);
            var __elevation__b__ = false;
            var __wfo__ = default(string);
            var __wfo__b__ = false;
            var __timezone__ = default(string);
            var __timezone__b__ = false;
            var __areaDescription__ = default(string);
            var __areaDescription__b__ = false;
            var __radar__ = default(string);
            var __radar__b__ = false;
            var __zone__ = default(string);
            var __zone__b__ = false;
            var __county__ = default(string);
            var __county__b__ = false;
            var __firezone__ = default(string);
            var __firezone__b__ = false;
            var __metar__ = default(string);
            var __metar__b__ = false;

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
                        __region__ = reader.ReadString();
                        __region__b__ = true;
                        break;
                    case 1:
                        __latitude__ = reader.ReadString();
                        __latitude__b__ = true;
                        break;
                    case 2:
                        __longitude__ = reader.ReadString();
                        __longitude__b__ = true;
                        break;
                    case 3:
                        __elevation__ = reader.ReadString();
                        __elevation__b__ = true;
                        break;
                    case 4:
                        __wfo__ = reader.ReadString();
                        __wfo__b__ = true;
                        break;
                    case 5:
                        __timezone__ = reader.ReadString();
                        __timezone__b__ = true;
                        break;
                    case 6:
                        __areaDescription__ = reader.ReadString();
                        __areaDescription__b__ = true;
                        break;
                    case 7:
                        __radar__ = reader.ReadString();
                        __radar__b__ = true;
                        break;
                    case 8:
                        __zone__ = reader.ReadString();
                        __zone__b__ = true;
                        break;
                    case 9:
                        __county__ = reader.ReadString();
                        __county__b__ = true;
                        break;
                    case 10:
                        __firezone__ = reader.ReadString();
                        __firezone__b__ = true;
                        break;
                    case 11:
                        __metar__ = reader.ReadString();
                        __metar__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.NWS.Observation.Location();
            if(__region__b__) ____result.region = __region__;
            if(__latitude__b__) ____result.latitude = __latitude__;
            if(__longitude__b__) ____result.longitude = __longitude__;
            if(__elevation__b__) ____result.elevation = __elevation__;
            if(__wfo__b__) ____result.wfo = __wfo__;
            if(__timezone__b__) ____result.timezone = __timezone__;
            if(__areaDescription__b__) ____result.areaDescription = __areaDescription__;
            if(__radar__b__) ____result.radar = __radar__;
            if(__zone__b__) ____result.zone = __zone__;
            if(__county__b__) ____result.county = __county__;
            if(__firezone__b__) ____result.firezone = __firezone__;
            if(__metar__b__) ____result.metar = __metar__;

            return ____result;
        }
    }


    public sealed class TimeFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.NWS.Observation.Time>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public TimeFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("layoutKey"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("startPeriodName"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("startValidTime"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("tempLabel"), 3},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("layoutKey"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("startPeriodName"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("startValidTime"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("tempLabel"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.NWS.Observation.Time value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.layoutKey);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<string[]>().Serialize(ref writer, value.startPeriodName, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[2]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset[]>().Serialize(ref writer, value.startValidTime, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[3]);
            formatterResolver.GetFormatterWithVerify<string[]>().Serialize(ref writer, value.tempLabel, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.NWS.Observation.Time Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __layoutKey__ = default(string);
            var __layoutKey__b__ = false;
            var __startPeriodName__ = default(string[]);
            var __startPeriodName__b__ = false;
            var __startValidTime__ = default(global::System.DateTimeOffset[]);
            var __startValidTime__b__ = false;
            var __tempLabel__ = default(string[]);
            var __tempLabel__b__ = false;

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
                        __layoutKey__ = reader.ReadString();
                        __layoutKey__b__ = true;
                        break;
                    case 1:
                        __startPeriodName__ = formatterResolver.GetFormatterWithVerify<string[]>().Deserialize(ref reader, formatterResolver);
                        __startPeriodName__b__ = true;
                        break;
                    case 2:
                        __startValidTime__ = formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset[]>().Deserialize(ref reader, formatterResolver);
                        __startValidTime__b__ = true;
                        break;
                    case 3:
                        __tempLabel__ = formatterResolver.GetFormatterWithVerify<string[]>().Deserialize(ref reader, formatterResolver);
                        __tempLabel__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.NWS.Observation.Time();
            if(__layoutKey__b__) ____result.layoutKey = __layoutKey__;
            if(__startPeriodName__b__) ____result.startPeriodName = __startPeriodName__;
            if(__startValidTime__b__) ____result.startValidTime = __startValidTime__;
            if(__tempLabel__b__) ____result.tempLabel = __tempLabel__;

            return ____result;
        }
    }


    public sealed class DataFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.NWS.Observation.Data>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public DataFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("temperature"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("pop"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("weather"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("iconLink"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("hazard"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("hazardUrl"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("text"), 6},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("temperature"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("pop"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("weather"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("iconLink"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("hazard"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("hazardUrl"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("text"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.NWS.Observation.Data value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<string[]>().Serialize(ref writer, value.temperature, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<string[]>().Serialize(ref writer, value.pop, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[2]);
            formatterResolver.GetFormatterWithVerify<string[]>().Serialize(ref writer, value.weather, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[3]);
            formatterResolver.GetFormatterWithVerify<string[]>().Serialize(ref writer, value.iconLink, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[4]);
            formatterResolver.GetFormatterWithVerify<string[]>().Serialize(ref writer, value.hazard, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[5]);
            formatterResolver.GetFormatterWithVerify<string[]>().Serialize(ref writer, value.hazardUrl, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[6]);
            formatterResolver.GetFormatterWithVerify<string[]>().Serialize(ref writer, value.text, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.NWS.Observation.Data Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __temperature__ = default(string[]);
            var __temperature__b__ = false;
            var __pop__ = default(string[]);
            var __pop__b__ = false;
            var __weather__ = default(string[]);
            var __weather__b__ = false;
            var __iconLink__ = default(string[]);
            var __iconLink__b__ = false;
            var __hazard__ = default(string[]);
            var __hazard__b__ = false;
            var __hazardUrl__ = default(string[]);
            var __hazardUrl__b__ = false;
            var __text__ = default(string[]);
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
                        __temperature__ = formatterResolver.GetFormatterWithVerify<string[]>().Deserialize(ref reader, formatterResolver);
                        __temperature__b__ = true;
                        break;
                    case 1:
                        __pop__ = formatterResolver.GetFormatterWithVerify<string[]>().Deserialize(ref reader, formatterResolver);
                        __pop__b__ = true;
                        break;
                    case 2:
                        __weather__ = formatterResolver.GetFormatterWithVerify<string[]>().Deserialize(ref reader, formatterResolver);
                        __weather__b__ = true;
                        break;
                    case 3:
                        __iconLink__ = formatterResolver.GetFormatterWithVerify<string[]>().Deserialize(ref reader, formatterResolver);
                        __iconLink__b__ = true;
                        break;
                    case 4:
                        __hazard__ = formatterResolver.GetFormatterWithVerify<string[]>().Deserialize(ref reader, formatterResolver);
                        __hazard__b__ = true;
                        break;
                    case 5:
                        __hazardUrl__ = formatterResolver.GetFormatterWithVerify<string[]>().Deserialize(ref reader, formatterResolver);
                        __hazardUrl__b__ = true;
                        break;
                    case 6:
                        __text__ = formatterResolver.GetFormatterWithVerify<string[]>().Deserialize(ref reader, formatterResolver);
                        __text__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.NWS.Observation.Data();
            if(__temperature__b__) ____result.temperature = __temperature__;
            if(__pop__b__) ____result.pop = __pop__;
            if(__weather__b__) ____result.weather = __weather__;
            if(__iconLink__b__) ____result.iconLink = __iconLink__;
            if(__hazard__b__) ____result.hazard = __hazard__;
            if(__hazardUrl__b__) ____result.hazardUrl = __hazardUrl__;
            if(__text__b__) ____result.text = __text__;

            return ____result;
        }
    }


    public sealed class CurrentobservationFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.NWS.Observation.Currentobservation>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public CurrentobservationFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("id"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("name"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("elev"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("latitude"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("longitude"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("Date"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("Temp"), 6},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("Dewp"), 7},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("Relh"), 8},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("Winds"), 9},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("Windd"), 10},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("Gust"), 11},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("Weather"), 12},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("Weatherimage"), 13},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("Visibility"), 14},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("Altimeter"), 15},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("SLP"), 16},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("timezone"), 17},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("state"), 18},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("WindChill"), 19},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("id"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("name"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("elev"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("latitude"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("longitude"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("Date"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("Temp"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("Dewp"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("Relh"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("Winds"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("Windd"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("Gust"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("Weather"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("Weatherimage"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("Visibility"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("Altimeter"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("SLP"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("timezone"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("state"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("WindChill"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.NWS.Observation.Currentobservation value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.id);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.name);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.elev);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteString(value.latitude);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteString(value.longitude);
            writer.WriteRaw(this.____stringByteKeys[5]);
            writer.WriteString(value.Date);
            writer.WriteRaw(this.____stringByteKeys[6]);
            writer.WriteString(value.Temp);
            writer.WriteRaw(this.____stringByteKeys[7]);
            writer.WriteString(value.Dewp);
            writer.WriteRaw(this.____stringByteKeys[8]);
            writer.WriteString(value.Relh);
            writer.WriteRaw(this.____stringByteKeys[9]);
            writer.WriteString(value.Winds);
            writer.WriteRaw(this.____stringByteKeys[10]);
            writer.WriteString(value.Windd);
            writer.WriteRaw(this.____stringByteKeys[11]);
            writer.WriteString(value.Gust);
            writer.WriteRaw(this.____stringByteKeys[12]);
            writer.WriteString(value.Weather);
            writer.WriteRaw(this.____stringByteKeys[13]);
            writer.WriteString(value.Weatherimage);
            writer.WriteRaw(this.____stringByteKeys[14]);
            writer.WriteString(value.Visibility);
            writer.WriteRaw(this.____stringByteKeys[15]);
            writer.WriteString(value.Altimeter);
            writer.WriteRaw(this.____stringByteKeys[16]);
            writer.WriteString(value.SLP);
            writer.WriteRaw(this.____stringByteKeys[17]);
            writer.WriteString(value.timezone);
            writer.WriteRaw(this.____stringByteKeys[18]);
            writer.WriteString(value.state);
            writer.WriteRaw(this.____stringByteKeys[19]);
            writer.WriteString(value.WindChill);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.NWS.Observation.Currentobservation Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __id__ = default(string);
            var __id__b__ = false;
            var __name__ = default(string);
            var __name__b__ = false;
            var __elev__ = default(string);
            var __elev__b__ = false;
            var __latitude__ = default(string);
            var __latitude__b__ = false;
            var __longitude__ = default(string);
            var __longitude__b__ = false;
            var __Date__ = default(string);
            var __Date__b__ = false;
            var __Temp__ = default(string);
            var __Temp__b__ = false;
            var __Dewp__ = default(string);
            var __Dewp__b__ = false;
            var __Relh__ = default(string);
            var __Relh__b__ = false;
            var __Winds__ = default(string);
            var __Winds__b__ = false;
            var __Windd__ = default(string);
            var __Windd__b__ = false;
            var __Gust__ = default(string);
            var __Gust__b__ = false;
            var __Weather__ = default(string);
            var __Weather__b__ = false;
            var __Weatherimage__ = default(string);
            var __Weatherimage__b__ = false;
            var __Visibility__ = default(string);
            var __Visibility__b__ = false;
            var __Altimeter__ = default(string);
            var __Altimeter__b__ = false;
            var __SLP__ = default(string);
            var __SLP__b__ = false;
            var __timezone__ = default(string);
            var __timezone__b__ = false;
            var __state__ = default(string);
            var __state__b__ = false;
            var __WindChill__ = default(string);
            var __WindChill__b__ = false;

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
                        __id__ = reader.ReadString();
                        __id__b__ = true;
                        break;
                    case 1:
                        __name__ = reader.ReadString();
                        __name__b__ = true;
                        break;
                    case 2:
                        __elev__ = reader.ReadString();
                        __elev__b__ = true;
                        break;
                    case 3:
                        __latitude__ = reader.ReadString();
                        __latitude__b__ = true;
                        break;
                    case 4:
                        __longitude__ = reader.ReadString();
                        __longitude__b__ = true;
                        break;
                    case 5:
                        __Date__ = reader.ReadString();
                        __Date__b__ = true;
                        break;
                    case 6:
                        __Temp__ = reader.ReadString();
                        __Temp__b__ = true;
                        break;
                    case 7:
                        __Dewp__ = reader.ReadString();
                        __Dewp__b__ = true;
                        break;
                    case 8:
                        __Relh__ = reader.ReadString();
                        __Relh__b__ = true;
                        break;
                    case 9:
                        __Winds__ = reader.ReadString();
                        __Winds__b__ = true;
                        break;
                    case 10:
                        __Windd__ = reader.ReadString();
                        __Windd__b__ = true;
                        break;
                    case 11:
                        __Gust__ = reader.ReadString();
                        __Gust__b__ = true;
                        break;
                    case 12:
                        __Weather__ = reader.ReadString();
                        __Weather__b__ = true;
                        break;
                    case 13:
                        __Weatherimage__ = reader.ReadString();
                        __Weatherimage__b__ = true;
                        break;
                    case 14:
                        __Visibility__ = reader.ReadString();
                        __Visibility__b__ = true;
                        break;
                    case 15:
                        __Altimeter__ = reader.ReadString();
                        __Altimeter__b__ = true;
                        break;
                    case 16:
                        __SLP__ = reader.ReadString();
                        __SLP__b__ = true;
                        break;
                    case 17:
                        __timezone__ = reader.ReadString();
                        __timezone__b__ = true;
                        break;
                    case 18:
                        __state__ = reader.ReadString();
                        __state__b__ = true;
                        break;
                    case 19:
                        __WindChill__ = reader.ReadString();
                        __WindChill__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.NWS.Observation.Currentobservation();
            if(__id__b__) ____result.id = __id__;
            if(__name__b__) ____result.name = __name__;
            if(__elev__b__) ____result.elev = __elev__;
            if(__latitude__b__) ____result.latitude = __latitude__;
            if(__longitude__b__) ____result.longitude = __longitude__;
            if(__Date__b__) ____result.Date = __Date__;
            if(__Temp__b__) ____result.Temp = __Temp__;
            if(__Dewp__b__) ____result.Dewp = __Dewp__;
            if(__Relh__b__) ____result.Relh = __Relh__;
            if(__Winds__b__) ____result.Winds = __Winds__;
            if(__Windd__b__) ____result.Windd = __Windd__;
            if(__Gust__b__) ____result.Gust = __Gust__;
            if(__Weather__b__) ____result.Weather = __Weather__;
            if(__Weatherimage__b__) ____result.Weatherimage = __Weatherimage__;
            if(__Visibility__b__) ____result.Visibility = __Visibility__;
            if(__Altimeter__b__) ____result.Altimeter = __Altimeter__;
            if(__SLP__b__) ____result.SLP = __SLP__;
            if(__timezone__b__) ____result.timezone = __timezone__;
            if(__state__b__) ____result.state = __state__;
            if(__WindChill__b__) ____result.WindChill = __WindChill__;

            return ____result;
        }
    }


    public sealed class ForecastRootobjectFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.NWS.Observation.ForecastRootobject>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public ForecastRootobjectFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("operationalMode"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("srsName"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("creationDate"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("creationDateLocal"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("productionCenter"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("credit"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("moreInformation"), 6},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("location"), 7},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("time"), 8},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("data"), 9},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("currentobservation"), 10},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("operationalMode"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("srsName"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("creationDate"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("creationDateLocal"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("productionCenter"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("credit"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("moreInformation"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("location"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("time"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("data"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("currentobservation"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.NWS.Observation.ForecastRootobject value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.operationalMode);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.srsName);
            writer.WriteRaw(this.____stringByteKeys[2]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset>().Serialize(ref writer, value.creationDate, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteString(value.creationDateLocal);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteString(value.productionCenter);
            writer.WriteRaw(this.____stringByteKeys[5]);
            writer.WriteString(value.credit);
            writer.WriteRaw(this.____stringByteKeys[6]);
            writer.WriteString(value.moreInformation);
            writer.WriteRaw(this.____stringByteKeys[7]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.NWS.Observation.Location>().Serialize(ref writer, value.location, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[8]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.NWS.Observation.Time>().Serialize(ref writer, value.time, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[9]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.NWS.Observation.Data>().Serialize(ref writer, value.data, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[10]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.NWS.Observation.Currentobservation>().Serialize(ref writer, value.currentobservation, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.NWS.Observation.ForecastRootobject Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __operationalMode__ = default(string);
            var __operationalMode__b__ = false;
            var __srsName__ = default(string);
            var __srsName__b__ = false;
            var __creationDate__ = default(global::System.DateTimeOffset);
            var __creationDate__b__ = false;
            var __creationDateLocal__ = default(string);
            var __creationDateLocal__b__ = false;
            var __productionCenter__ = default(string);
            var __productionCenter__b__ = false;
            var __credit__ = default(string);
            var __credit__b__ = false;
            var __moreInformation__ = default(string);
            var __moreInformation__b__ = false;
            var __location__ = default(global::SimpleWeather.NWS.Observation.Location);
            var __location__b__ = false;
            var __time__ = default(global::SimpleWeather.NWS.Observation.Time);
            var __time__b__ = false;
            var __data__ = default(global::SimpleWeather.NWS.Observation.Data);
            var __data__b__ = false;
            var __currentobservation__ = default(global::SimpleWeather.NWS.Observation.Currentobservation);
            var __currentobservation__b__ = false;

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
                        __operationalMode__ = reader.ReadString();
                        __operationalMode__b__ = true;
                        break;
                    case 1:
                        __srsName__ = reader.ReadString();
                        __srsName__b__ = true;
                        break;
                    case 2:
                        __creationDate__ = formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset>().Deserialize(ref reader, formatterResolver);
                        __creationDate__b__ = true;
                        break;
                    case 3:
                        __creationDateLocal__ = reader.ReadString();
                        __creationDateLocal__b__ = true;
                        break;
                    case 4:
                        __productionCenter__ = reader.ReadString();
                        __productionCenter__b__ = true;
                        break;
                    case 5:
                        __credit__ = reader.ReadString();
                        __credit__b__ = true;
                        break;
                    case 6:
                        __moreInformation__ = reader.ReadString();
                        __moreInformation__b__ = true;
                        break;
                    case 7:
                        __location__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.NWS.Observation.Location>().Deserialize(ref reader, formatterResolver);
                        __location__b__ = true;
                        break;
                    case 8:
                        __time__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.NWS.Observation.Time>().Deserialize(ref reader, formatterResolver);
                        __time__b__ = true;
                        break;
                    case 9:
                        __data__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.NWS.Observation.Data>().Deserialize(ref reader, formatterResolver);
                        __data__b__ = true;
                        break;
                    case 10:
                        __currentobservation__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.NWS.Observation.Currentobservation>().Deserialize(ref reader, formatterResolver);
                        __currentobservation__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.NWS.Observation.ForecastRootobject();
            if(__operationalMode__b__) ____result.operationalMode = __operationalMode__;
            if(__srsName__b__) ____result.srsName = __srsName__;
            if(__creationDate__b__) ____result.creationDate = __creationDate__;
            if(__creationDateLocal__b__) ____result.creationDateLocal = __creationDateLocal__;
            if(__productionCenter__b__) ____result.productionCenter = __productionCenter__;
            if(__credit__b__) ____result.credit = __credit__;
            if(__moreInformation__b__) ____result.moreInformation = __moreInformation__;
            if(__location__b__) ____result.location = __location__;
            if(__time__b__) ____result.time = __time__;
            if(__data__b__) ____result.data = __data__;
            if(__currentobservation__b__) ____result.currentobservation = __currentobservation__;

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

namespace SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.NWS.Hourly
{
    using System;
    using Utf8Json;


    public sealed class LocationFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.NWS.Hourly.Location>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public LocationFormatter()
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

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.NWS.Hourly.Location value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteDouble(value.latitude);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteDouble(value.longitude);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.NWS.Hourly.Location Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __latitude__ = default(double);
            var __latitude__b__ = false;
            var __longitude__ = default(double);
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
                        __latitude__ = reader.ReadDouble();
                        __latitude__b__ = true;
                        break;
                    case 1:
                        __longitude__ = reader.ReadDouble();
                        __longitude__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.NWS.Hourly.Location();
            if(__latitude__b__) ____result.latitude = __latitude__;
            if(__longitude__b__) ____result.longitude = __longitude__;

            return ____result;
        }
    }


    public sealed class PeriodsItemFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.NWS.Hourly.PeriodsItem>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public PeriodsItemFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("time"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("unixtime"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("windChill"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("windGust"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("periodName"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("pop"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("iconLink"), 6},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("relativeHumidity"), 7},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("temperature"), 8},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("weather"), 9},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("windDirection"), 10},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("windSpeed"), 11},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("cloudAmount"), 12},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("time"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("unixtime"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("windChill"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("windGust"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("periodName"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("pop"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("iconLink"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("relativeHumidity"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("temperature"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("weather"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("windDirection"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("windSpeed"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("cloudAmount"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.NWS.Hourly.PeriodsItem value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<string>>().Serialize(ref writer, value.time, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<string>>().Serialize(ref writer, value.unixtime, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[2]);
            formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<string>>().Serialize(ref writer, value.windChill, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[3]);
            formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<string>>().Serialize(ref writer, value.windGust, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteString(value.periodName);
            writer.WriteRaw(this.____stringByteKeys[5]);
            formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<string>>().Serialize(ref writer, value.pop, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[6]);
            formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<string>>().Serialize(ref writer, value.iconLink, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[7]);
            formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<string>>().Serialize(ref writer, value.relativeHumidity, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[8]);
            formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<string>>().Serialize(ref writer, value.temperature, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[9]);
            formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<string>>().Serialize(ref writer, value.weather, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[10]);
            formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<string>>().Serialize(ref writer, value.windDirection, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[11]);
            formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<string>>().Serialize(ref writer, value.windSpeed, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[12]);
            formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<string>>().Serialize(ref writer, value.cloudAmount, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.NWS.Hourly.PeriodsItem Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __time__ = default(global::System.Collections.Generic.List<string>);
            var __time__b__ = false;
            var __unixtime__ = default(global::System.Collections.Generic.List<string>);
            var __unixtime__b__ = false;
            var __windChill__ = default(global::System.Collections.Generic.List<string>);
            var __windChill__b__ = false;
            var __windGust__ = default(global::System.Collections.Generic.List<string>);
            var __windGust__b__ = false;
            var __periodName__ = default(string);
            var __periodName__b__ = false;
            var __pop__ = default(global::System.Collections.Generic.List<string>);
            var __pop__b__ = false;
            var __iconLink__ = default(global::System.Collections.Generic.List<string>);
            var __iconLink__b__ = false;
            var __relativeHumidity__ = default(global::System.Collections.Generic.List<string>);
            var __relativeHumidity__b__ = false;
            var __temperature__ = default(global::System.Collections.Generic.List<string>);
            var __temperature__b__ = false;
            var __weather__ = default(global::System.Collections.Generic.List<string>);
            var __weather__b__ = false;
            var __windDirection__ = default(global::System.Collections.Generic.List<string>);
            var __windDirection__b__ = false;
            var __windSpeed__ = default(global::System.Collections.Generic.List<string>);
            var __windSpeed__b__ = false;
            var __cloudAmount__ = default(global::System.Collections.Generic.List<string>);
            var __cloudAmount__b__ = false;

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
                        __time__ = formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<string>>().Deserialize(ref reader, formatterResolver);
                        __time__b__ = true;
                        break;
                    case 1:
                        __unixtime__ = formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<string>>().Deserialize(ref reader, formatterResolver);
                        __unixtime__b__ = true;
                        break;
                    case 2:
                        __windChill__ = formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<string>>().Deserialize(ref reader, formatterResolver);
                        __windChill__b__ = true;
                        break;
                    case 3:
                        __windGust__ = formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<string>>().Deserialize(ref reader, formatterResolver);
                        __windGust__b__ = true;
                        break;
                    case 4:
                        __periodName__ = reader.ReadString();
                        __periodName__b__ = true;
                        break;
                    case 5:
                        __pop__ = formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<string>>().Deserialize(ref reader, formatterResolver);
                        __pop__b__ = true;
                        break;
                    case 6:
                        __iconLink__ = formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<string>>().Deserialize(ref reader, formatterResolver);
                        __iconLink__b__ = true;
                        break;
                    case 7:
                        __relativeHumidity__ = formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<string>>().Deserialize(ref reader, formatterResolver);
                        __relativeHumidity__b__ = true;
                        break;
                    case 8:
                        __temperature__ = formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<string>>().Deserialize(ref reader, formatterResolver);
                        __temperature__b__ = true;
                        break;
                    case 9:
                        __weather__ = formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<string>>().Deserialize(ref reader, formatterResolver);
                        __weather__b__ = true;
                        break;
                    case 10:
                        __windDirection__ = formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<string>>().Deserialize(ref reader, formatterResolver);
                        __windDirection__b__ = true;
                        break;
                    case 11:
                        __windSpeed__ = formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<string>>().Deserialize(ref reader, formatterResolver);
                        __windSpeed__b__ = true;
                        break;
                    case 12:
                        __cloudAmount__ = formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<string>>().Deserialize(ref reader, formatterResolver);
                        __cloudAmount__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.NWS.Hourly.PeriodsItem();
            if(__time__b__) ____result.time = __time__;
            if(__unixtime__b__) ____result.unixtime = __unixtime__;
            if(__windChill__b__) ____result.windChill = __windChill__;
            if(__windGust__b__) ____result.windGust = __windGust__;
            if(__periodName__b__) ____result.periodName = __periodName__;
            if(__pop__b__) ____result.pop = __pop__;
            if(__iconLink__b__) ____result.iconLink = __iconLink__;
            if(__relativeHumidity__b__) ____result.relativeHumidity = __relativeHumidity__;
            if(__temperature__b__) ____result.temperature = __temperature__;
            if(__weather__b__) ____result.weather = __weather__;
            if(__windDirection__b__) ____result.windDirection = __windDirection__;
            if(__windSpeed__b__) ____result.windSpeed = __windSpeed__;
            if(__cloudAmount__b__) ____result.cloudAmount = __cloudAmount__;

            return ____result;
        }
    }


    public sealed class HourlyForecastResponseFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.NWS.Hourly.HourlyForecastResponse>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public HourlyForecastResponseFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("creationDate"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("location"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("periodsItems"), 2},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("creationDate"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("location"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("periodsItems"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.NWS.Hourly.HourlyForecastResponse value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset>().Serialize(ref writer, value.creationDate, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.NWS.Hourly.Location>().Serialize(ref writer, value.location, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[2]);
            formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<global::SimpleWeather.NWS.Hourly.PeriodsItem>>().Serialize(ref writer, value.periodsItems, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.NWS.Hourly.HourlyForecastResponse Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __creationDate__ = default(global::System.DateTimeOffset);
            var __creationDate__b__ = false;
            var __location__ = default(global::SimpleWeather.NWS.Hourly.Location);
            var __location__b__ = false;
            var __periodsItems__ = default(global::System.Collections.Generic.List<global::SimpleWeather.NWS.Hourly.PeriodsItem>);
            var __periodsItems__b__ = false;

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
                        __creationDate__ = formatterResolver.GetFormatterWithVerify<global::System.DateTimeOffset>().Deserialize(ref reader, formatterResolver);
                        __creationDate__b__ = true;
                        break;
                    case 1:
                        __location__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.NWS.Hourly.Location>().Deserialize(ref reader, formatterResolver);
                        __location__b__ = true;
                        break;
                    case 2:
                        __periodsItems__ = formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<global::SimpleWeather.NWS.Hourly.PeriodsItem>>().Deserialize(ref reader, formatterResolver);
                        __periodsItems__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.NWS.Hourly.HourlyForecastResponse();
            if(__creationDate__b__) ____result.creationDate = __creationDate__;
            if(__location__b__) ____result.location = __location__;
            if(__periodsItems__b__) ____result.periodsItems = __periodsItems__;

            return ____result;
        }
    }


    public sealed class PeriodItemFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.NWS.Hourly.PeriodItem>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public PeriodItemFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("unixTime"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("windChill"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("windSpeed"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("cloudAmount"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("pop"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("relativeHumidity"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("windGust"), 6},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("temperature"), 7},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("windDirection"), 8},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("iconLink"), 9},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("weather"), 10},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("unixTime"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("windChill"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("windSpeed"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("cloudAmount"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("pop"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("relativeHumidity"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("windGust"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("temperature"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("windDirection"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("iconLink"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("weather"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.NWS.Hourly.PeriodItem value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.unixTime);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.windChill);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.windSpeed);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteString(value.cloudAmount);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteString(value.pop);
            writer.WriteRaw(this.____stringByteKeys[5]);
            writer.WriteString(value.relativeHumidity);
            writer.WriteRaw(this.____stringByteKeys[6]);
            writer.WriteString(value.windGust);
            writer.WriteRaw(this.____stringByteKeys[7]);
            writer.WriteString(value.temperature);
            writer.WriteRaw(this.____stringByteKeys[8]);
            writer.WriteString(value.windDirection);
            writer.WriteRaw(this.____stringByteKeys[9]);
            writer.WriteString(value.iconLink);
            writer.WriteRaw(this.____stringByteKeys[10]);
            writer.WriteString(value.weather);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.NWS.Hourly.PeriodItem Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __unixTime__ = default(string);
            var __unixTime__b__ = false;
            var __windChill__ = default(string);
            var __windChill__b__ = false;
            var __windSpeed__ = default(string);
            var __windSpeed__b__ = false;
            var __cloudAmount__ = default(string);
            var __cloudAmount__b__ = false;
            var __pop__ = default(string);
            var __pop__b__ = false;
            var __relativeHumidity__ = default(string);
            var __relativeHumidity__b__ = false;
            var __windGust__ = default(string);
            var __windGust__b__ = false;
            var __temperature__ = default(string);
            var __temperature__b__ = false;
            var __windDirection__ = default(string);
            var __windDirection__b__ = false;
            var __iconLink__ = default(string);
            var __iconLink__b__ = false;
            var __weather__ = default(string);
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
                        __unixTime__ = reader.ReadString();
                        __unixTime__b__ = true;
                        break;
                    case 1:
                        __windChill__ = reader.ReadString();
                        __windChill__b__ = true;
                        break;
                    case 2:
                        __windSpeed__ = reader.ReadString();
                        __windSpeed__b__ = true;
                        break;
                    case 3:
                        __cloudAmount__ = reader.ReadString();
                        __cloudAmount__b__ = true;
                        break;
                    case 4:
                        __pop__ = reader.ReadString();
                        __pop__b__ = true;
                        break;
                    case 5:
                        __relativeHumidity__ = reader.ReadString();
                        __relativeHumidity__b__ = true;
                        break;
                    case 6:
                        __windGust__ = reader.ReadString();
                        __windGust__b__ = true;
                        break;
                    case 7:
                        __temperature__ = reader.ReadString();
                        __temperature__b__ = true;
                        break;
                    case 8:
                        __windDirection__ = reader.ReadString();
                        __windDirection__b__ = true;
                        break;
                    case 9:
                        __iconLink__ = reader.ReadString();
                        __iconLink__b__ = true;
                        break;
                    case 10:
                        __weather__ = reader.ReadString();
                        __weather__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.NWS.Hourly.PeriodItem(__unixTime__, __windChill__, __windSpeed__, __cloudAmount__, __pop__, __relativeHumidity__, __windGust__, __temperature__, __windDirection__, __iconLink__, __weather__);
            if(__unixTime__b__) ____result.unixTime = __unixTime__;
            if(__windChill__b__) ____result.windChill = __windChill__;
            if(__windSpeed__b__) ____result.windSpeed = __windSpeed__;
            if(__cloudAmount__b__) ____result.cloudAmount = __cloudAmount__;
            if(__pop__b__) ____result.pop = __pop__;
            if(__relativeHumidity__b__) ____result.relativeHumidity = __relativeHumidity__;
            if(__windGust__b__) ____result.windGust = __windGust__;
            if(__temperature__b__) ____result.temperature = __temperature__;
            if(__windDirection__b__) ____result.windDirection = __windDirection__;
            if(__iconLink__b__) ____result.iconLink = __iconLink__;
            if(__weather__b__) ____result.weather = __weather__;

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


    public sealed class CoordFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.OpenWeather.Coord>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public CoordFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("lon"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("lat"), 1},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("lon"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("lat"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.OpenWeather.Coord value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteSingle(value.lon);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteSingle(value.lat);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.OpenWeather.Coord Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __lon__ = default(float);
            var __lon__b__ = false;
            var __lat__ = default(float);
            var __lat__b__ = false;

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
                        __lon__ = reader.ReadSingle();
                        __lon__b__ = true;
                        break;
                    case 1:
                        __lat__ = reader.ReadSingle();
                        __lat__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.OpenWeather.Coord();
            if(__lon__b__) ____result.lon = __lon__;
            if(__lat__b__) ____result.lat = __lat__;

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


    public sealed class MainFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.OpenWeather.Main>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public MainFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("temp"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("feels_like"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("temp_min"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("temp_max"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("pressure"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("humidity"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("sea_level"), 6},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("grnd_level"), 7},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("temp"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("feels_like"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("temp_min"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("temp_max"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("pressure"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("humidity"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("sea_level"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("grnd_level"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.OpenWeather.Main value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteSingle(value.temp);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.feels_like, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteSingle(value.temp_min);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteSingle(value.temp_max);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteSingle(value.pressure);
            writer.WriteRaw(this.____stringByteKeys[5]);
            writer.WriteInt32(value.humidity);
            writer.WriteRaw(this.____stringByteKeys[6]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.sea_level, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[7]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.grnd_level, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.OpenWeather.Main Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __temp__ = default(float);
            var __temp__b__ = false;
            var __feels_like__ = default(float?);
            var __feels_like__b__ = false;
            var __temp_min__ = default(float);
            var __temp_min__b__ = false;
            var __temp_max__ = default(float);
            var __temp_max__b__ = false;
            var __pressure__ = default(float);
            var __pressure__b__ = false;
            var __humidity__ = default(int);
            var __humidity__b__ = false;
            var __sea_level__ = default(float?);
            var __sea_level__b__ = false;
            var __grnd_level__ = default(float?);
            var __grnd_level__b__ = false;

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
                        __temp__ = reader.ReadSingle();
                        __temp__b__ = true;
                        break;
                    case 1:
                        __feels_like__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __feels_like__b__ = true;
                        break;
                    case 2:
                        __temp_min__ = reader.ReadSingle();
                        __temp_min__b__ = true;
                        break;
                    case 3:
                        __temp_max__ = reader.ReadSingle();
                        __temp_max__b__ = true;
                        break;
                    case 4:
                        __pressure__ = reader.ReadSingle();
                        __pressure__b__ = true;
                        break;
                    case 5:
                        __humidity__ = reader.ReadInt32();
                        __humidity__b__ = true;
                        break;
                    case 6:
                        __sea_level__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __sea_level__b__ = true;
                        break;
                    case 7:
                        __grnd_level__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __grnd_level__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.OpenWeather.Main();
            if(__temp__b__) ____result.temp = __temp__;
            if(__feels_like__b__) ____result.feels_like = __feels_like__;
            if(__temp_min__b__) ____result.temp_min = __temp_min__;
            if(__temp_max__b__) ____result.temp_max = __temp_max__;
            if(__pressure__b__) ____result.pressure = __pressure__;
            if(__humidity__b__) ____result.humidity = __humidity__;
            if(__sea_level__b__) ____result.sea_level = __sea_level__;
            if(__grnd_level__b__) ____result.grnd_level = __grnd_level__;

            return ____result;
        }
    }


    public sealed class WindFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.OpenWeather.Wind>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public WindFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("speed"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("deg"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("gust"), 2},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("speed"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("deg"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("gust"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.OpenWeather.Wind value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteSingle(value.speed);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteSingle(value.deg);
            writer.WriteRaw(this.____stringByteKeys[2]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.gust, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.OpenWeather.Wind Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __speed__ = default(float);
            var __speed__b__ = false;
            var __deg__ = default(float);
            var __deg__b__ = false;
            var __gust__ = default(float?);
            var __gust__b__ = false;

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
                        __speed__ = reader.ReadSingle();
                        __speed__b__ = true;
                        break;
                    case 1:
                        __deg__ = reader.ReadSingle();
                        __deg__b__ = true;
                        break;
                    case 2:
                        __gust__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __gust__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.OpenWeather.Wind();
            if(__speed__b__) ____result.speed = __speed__;
            if(__deg__b__) ____result.deg = __deg__;
            if(__gust__b__) ____result.gust = __gust__;

            return ____result;
        }
    }


    public sealed class CloudsFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.OpenWeather.Clouds>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public CloudsFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("all"), 0},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("all"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.OpenWeather.Clouds value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteInt32(value.all);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.OpenWeather.Clouds Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __all__ = default(int);
            var __all__b__ = false;

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
                        __all__ = reader.ReadInt32();
                        __all__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.OpenWeather.Clouds();
            if(__all__b__) ____result.all = __all__;

            return ____result;
        }
    }


    public sealed class RainFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.OpenWeather.Rain>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public RainFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("_1h"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("_3h"), 1},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("_1h"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("_3h"),
                
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
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value._1h, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value._3h, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.OpenWeather.Rain Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var ___1h__ = default(float?);
            var ___1h__b__ = false;
            var ___3h__ = default(float?);
            var ___3h__b__ = false;

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
                        ___1h__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        ___1h__b__ = true;
                        break;
                    case 1:
                        ___3h__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        ___3h__b__ = true;
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
            if(___3h__b__) ____result._3h = ___3h__;

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
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("_3h"), 1},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("_1h"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("_3h"),
                
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
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value._1h, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value._3h, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.OpenWeather.Snow Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var ___1h__ = default(float?);
            var ___1h__b__ = false;
            var ___3h__ = default(float?);
            var ___3h__b__ = false;

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
                        ___1h__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        ___1h__b__ = true;
                        break;
                    case 1:
                        ___3h__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        ___3h__b__ = true;
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
            if(___3h__b__) ____result._3h = ___3h__;

            return ____result;
        }
    }


    public sealed class CurrentSysFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.OpenWeather.CurrentSys>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public CurrentSysFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("country"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("sunrise"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("sunset"), 2},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("country"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("sunrise"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("sunset"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.OpenWeather.CurrentSys value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.country);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<long?>().Serialize(ref writer, value.sunrise, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[2]);
            formatterResolver.GetFormatterWithVerify<long?>().Serialize(ref writer, value.sunset, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.OpenWeather.CurrentSys Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __country__ = default(string);
            var __country__b__ = false;
            var __sunrise__ = default(long?);
            var __sunrise__b__ = false;
            var __sunset__ = default(long?);
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
                        __country__ = reader.ReadString();
                        __country__b__ = true;
                        break;
                    case 1:
                        __sunrise__ = formatterResolver.GetFormatterWithVerify<long?>().Deserialize(ref reader, formatterResolver);
                        __sunrise__b__ = true;
                        break;
                    case 2:
                        __sunset__ = formatterResolver.GetFormatterWithVerify<long?>().Deserialize(ref reader, formatterResolver);
                        __sunset__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.OpenWeather.CurrentSys();
            if(__country__b__) ____result.country = __country__;
            if(__sunrise__b__) ____result.sunrise = __sunrise__;
            if(__sunset__b__) ____result.sunset = __sunset__;

            return ____result;
        }
    }


    public sealed class CurrentRootobjectFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.OpenWeather.CurrentRootobject>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public CurrentRootobjectFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("coord"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("weather"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("_base"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("main"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("visibility"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("wind"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("clouds"), 6},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("rain"), 7},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("snow"), 8},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("dt"), 9},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("sys"), 10},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("timezone"), 11},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("id"), 12},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("name"), 13},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("coord"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("weather"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("_base"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("main"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("visibility"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("wind"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("clouds"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("rain"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("snow"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("dt"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("sys"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("timezone"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("id"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("name"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.OpenWeather.CurrentRootobject value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Coord>().Serialize(ref writer, value.coord, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Weather[]>().Serialize(ref writer, value.weather, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value._base);
            writer.WriteRaw(this.____stringByteKeys[3]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Main>().Serialize(ref writer, value.main, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteInt32(value.visibility);
            writer.WriteRaw(this.____stringByteKeys[5]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Wind>().Serialize(ref writer, value.wind, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[6]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Clouds>().Serialize(ref writer, value.clouds, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[7]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Rain>().Serialize(ref writer, value.rain, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[8]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Snow>().Serialize(ref writer, value.snow, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[9]);
            writer.WriteInt64(value.dt);
            writer.WriteRaw(this.____stringByteKeys[10]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.CurrentSys>().Serialize(ref writer, value.sys, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[11]);
            writer.WriteInt32(value.timezone);
            writer.WriteRaw(this.____stringByteKeys[12]);
            writer.WriteInt32(value.id);
            writer.WriteRaw(this.____stringByteKeys[13]);
            writer.WriteString(value.name);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.OpenWeather.CurrentRootobject Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __coord__ = default(global::SimpleWeather.OpenWeather.Coord);
            var __coord__b__ = false;
            var __weather__ = default(global::SimpleWeather.OpenWeather.Weather[]);
            var __weather__b__ = false;
            var ___base__ = default(string);
            var ___base__b__ = false;
            var __main__ = default(global::SimpleWeather.OpenWeather.Main);
            var __main__b__ = false;
            var __visibility__ = default(int);
            var __visibility__b__ = false;
            var __wind__ = default(global::SimpleWeather.OpenWeather.Wind);
            var __wind__b__ = false;
            var __clouds__ = default(global::SimpleWeather.OpenWeather.Clouds);
            var __clouds__b__ = false;
            var __rain__ = default(global::SimpleWeather.OpenWeather.Rain);
            var __rain__b__ = false;
            var __snow__ = default(global::SimpleWeather.OpenWeather.Snow);
            var __snow__b__ = false;
            var __dt__ = default(long);
            var __dt__b__ = false;
            var __sys__ = default(global::SimpleWeather.OpenWeather.CurrentSys);
            var __sys__b__ = false;
            var __timezone__ = default(int);
            var __timezone__b__ = false;
            var __id__ = default(int);
            var __id__b__ = false;
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
                        __coord__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Coord>().Deserialize(ref reader, formatterResolver);
                        __coord__b__ = true;
                        break;
                    case 1:
                        __weather__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Weather[]>().Deserialize(ref reader, formatterResolver);
                        __weather__b__ = true;
                        break;
                    case 2:
                        ___base__ = reader.ReadString();
                        ___base__b__ = true;
                        break;
                    case 3:
                        __main__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Main>().Deserialize(ref reader, formatterResolver);
                        __main__b__ = true;
                        break;
                    case 4:
                        __visibility__ = reader.ReadInt32();
                        __visibility__b__ = true;
                        break;
                    case 5:
                        __wind__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Wind>().Deserialize(ref reader, formatterResolver);
                        __wind__b__ = true;
                        break;
                    case 6:
                        __clouds__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Clouds>().Deserialize(ref reader, formatterResolver);
                        __clouds__b__ = true;
                        break;
                    case 7:
                        __rain__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Rain>().Deserialize(ref reader, formatterResolver);
                        __rain__b__ = true;
                        break;
                    case 8:
                        __snow__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Snow>().Deserialize(ref reader, formatterResolver);
                        __snow__b__ = true;
                        break;
                    case 9:
                        __dt__ = reader.ReadInt64();
                        __dt__b__ = true;
                        break;
                    case 10:
                        __sys__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.CurrentSys>().Deserialize(ref reader, formatterResolver);
                        __sys__b__ = true;
                        break;
                    case 11:
                        __timezone__ = reader.ReadInt32();
                        __timezone__b__ = true;
                        break;
                    case 12:
                        __id__ = reader.ReadInt32();
                        __id__b__ = true;
                        break;
                    case 13:
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

            var ____result = new global::SimpleWeather.OpenWeather.CurrentRootobject();
            if(__coord__b__) ____result.coord = __coord__;
            if(__weather__b__) ____result.weather = __weather__;
            if(___base__b__) ____result._base = ___base__;
            if(__main__b__) ____result.main = __main__;
            if(__visibility__b__) ____result.visibility = __visibility__;
            if(__wind__b__) ____result.wind = __wind__;
            if(__clouds__b__) ____result.clouds = __clouds__;
            if(__rain__b__) ____result.rain = __rain__;
            if(__snow__b__) ____result.snow = __snow__;
            if(__dt__b__) ____result.dt = __dt__;
            if(__sys__b__) ____result.sys = __sys__;
            if(__timezone__b__) ____result.timezone = __timezone__;
            if(__id__b__) ____result.id = __id__;
            if(__name__b__) ____result.name = __name__;

            return ____result;
        }
    }


    public sealed class ForecastSysFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.OpenWeather.ForecastSys>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public ForecastSysFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("pod"), 0},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("pod"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.OpenWeather.ForecastSys value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.pod);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.OpenWeather.ForecastSys Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __pod__ = default(string);
            var __pod__b__ = false;

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
                        __pod__ = reader.ReadString();
                        __pod__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.OpenWeather.ForecastSys();
            if(__pod__b__) ____result.pod = __pod__;

            return ____result;
        }
    }


    public sealed class ListFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.OpenWeather.List>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public ListFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("dt"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("main"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("weather"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("clouds"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("wind"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("visibility"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("pop"), 6},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("sys"), 7},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("dt_txt"), 8},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("rain"), 9},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("snow"), 10},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("dt"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("main"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("weather"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("clouds"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("wind"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("visibility"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("pop"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("sys"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("dt_txt"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("rain"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("snow"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.OpenWeather.List value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteInt64(value.dt);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Main>().Serialize(ref writer, value.main, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[2]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Weather[]>().Serialize(ref writer, value.weather, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[3]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Clouds>().Serialize(ref writer, value.clouds, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[4]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Wind>().Serialize(ref writer, value.wind, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[5]);
            formatterResolver.GetFormatterWithVerify<int?>().Serialize(ref writer, value.visibility, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[6]);
            formatterResolver.GetFormatterWithVerify<float?>().Serialize(ref writer, value.pop, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[7]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.ForecastSys>().Serialize(ref writer, value.sys, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[8]);
            writer.WriteString(value.dt_txt);
            writer.WriteRaw(this.____stringByteKeys[9]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Rain>().Serialize(ref writer, value.rain, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[10]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Snow>().Serialize(ref writer, value.snow, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.OpenWeather.List Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __dt__ = default(long);
            var __dt__b__ = false;
            var __main__ = default(global::SimpleWeather.OpenWeather.Main);
            var __main__b__ = false;
            var __weather__ = default(global::SimpleWeather.OpenWeather.Weather[]);
            var __weather__b__ = false;
            var __clouds__ = default(global::SimpleWeather.OpenWeather.Clouds);
            var __clouds__b__ = false;
            var __wind__ = default(global::SimpleWeather.OpenWeather.Wind);
            var __wind__b__ = false;
            var __visibility__ = default(int?);
            var __visibility__b__ = false;
            var __pop__ = default(float?);
            var __pop__b__ = false;
            var __sys__ = default(global::SimpleWeather.OpenWeather.ForecastSys);
            var __sys__b__ = false;
            var __dt_txt__ = default(string);
            var __dt_txt__b__ = false;
            var __rain__ = default(global::SimpleWeather.OpenWeather.Rain);
            var __rain__b__ = false;
            var __snow__ = default(global::SimpleWeather.OpenWeather.Snow);
            var __snow__b__ = false;

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
                        __main__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Main>().Deserialize(ref reader, formatterResolver);
                        __main__b__ = true;
                        break;
                    case 2:
                        __weather__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Weather[]>().Deserialize(ref reader, formatterResolver);
                        __weather__b__ = true;
                        break;
                    case 3:
                        __clouds__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Clouds>().Deserialize(ref reader, formatterResolver);
                        __clouds__b__ = true;
                        break;
                    case 4:
                        __wind__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Wind>().Deserialize(ref reader, formatterResolver);
                        __wind__b__ = true;
                        break;
                    case 5:
                        __visibility__ = formatterResolver.GetFormatterWithVerify<int?>().Deserialize(ref reader, formatterResolver);
                        __visibility__b__ = true;
                        break;
                    case 6:
                        __pop__ = formatterResolver.GetFormatterWithVerify<float?>().Deserialize(ref reader, formatterResolver);
                        __pop__b__ = true;
                        break;
                    case 7:
                        __sys__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.ForecastSys>().Deserialize(ref reader, formatterResolver);
                        __sys__b__ = true;
                        break;
                    case 8:
                        __dt_txt__ = reader.ReadString();
                        __dt_txt__b__ = true;
                        break;
                    case 9:
                        __rain__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Rain>().Deserialize(ref reader, formatterResolver);
                        __rain__b__ = true;
                        break;
                    case 10:
                        __snow__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Snow>().Deserialize(ref reader, formatterResolver);
                        __snow__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.OpenWeather.List();
            if(__dt__b__) ____result.dt = __dt__;
            if(__main__b__) ____result.main = __main__;
            if(__weather__b__) ____result.weather = __weather__;
            if(__clouds__b__) ____result.clouds = __clouds__;
            if(__wind__b__) ____result.wind = __wind__;
            if(__visibility__b__) ____result.visibility = __visibility__;
            if(__pop__b__) ____result.pop = __pop__;
            if(__sys__b__) ____result.sys = __sys__;
            if(__dt_txt__b__) ____result.dt_txt = __dt_txt__;
            if(__rain__b__) ____result.rain = __rain__;
            if(__snow__b__) ____result.snow = __snow__;

            return ____result;
        }
    }


    public sealed class CityFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.OpenWeather.City>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public CityFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("id"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("name"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("coord"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("country"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("sunrise"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("sunset"), 5},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("id"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("name"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("coord"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("country"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("sunrise"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("sunset"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.OpenWeather.City value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteInt32(value.id);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.name);
            writer.WriteRaw(this.____stringByteKeys[2]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Coord>().Serialize(ref writer, value.coord, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteString(value.country);
            writer.WriteRaw(this.____stringByteKeys[4]);
            formatterResolver.GetFormatterWithVerify<int?>().Serialize(ref writer, value.sunrise, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[5]);
            formatterResolver.GetFormatterWithVerify<int?>().Serialize(ref writer, value.sunset, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.OpenWeather.City Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __id__ = default(int);
            var __id__b__ = false;
            var __name__ = default(string);
            var __name__b__ = false;
            var __coord__ = default(global::SimpleWeather.OpenWeather.Coord);
            var __coord__b__ = false;
            var __country__ = default(string);
            var __country__b__ = false;
            var __sunrise__ = default(int?);
            var __sunrise__b__ = false;
            var __sunset__ = default(int?);
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
                        __id__ = reader.ReadInt32();
                        __id__b__ = true;
                        break;
                    case 1:
                        __name__ = reader.ReadString();
                        __name__b__ = true;
                        break;
                    case 2:
                        __coord__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.Coord>().Deserialize(ref reader, formatterResolver);
                        __coord__b__ = true;
                        break;
                    case 3:
                        __country__ = reader.ReadString();
                        __country__b__ = true;
                        break;
                    case 4:
                        __sunrise__ = formatterResolver.GetFormatterWithVerify<int?>().Deserialize(ref reader, formatterResolver);
                        __sunrise__b__ = true;
                        break;
                    case 5:
                        __sunset__ = formatterResolver.GetFormatterWithVerify<int?>().Deserialize(ref reader, formatterResolver);
                        __sunset__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.OpenWeather.City();
            if(__id__b__) ____result.id = __id__;
            if(__name__b__) ____result.name = __name__;
            if(__coord__b__) ____result.coord = __coord__;
            if(__country__b__) ____result.country = __country__;
            if(__sunrise__b__) ____result.sunrise = __sunrise__;
            if(__sunset__b__) ____result.sunset = __sunset__;

            return ____result;
        }
    }


    public sealed class ForecastRootobjectFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.OpenWeather.ForecastRootobject>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public ForecastRootobjectFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("list"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("city"), 1},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("list"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("city"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.OpenWeather.ForecastRootobject value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.List[]>().Serialize(ref writer, value.list, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.City>().Serialize(ref writer, value.city, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.OpenWeather.ForecastRootobject Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __list__ = default(global::SimpleWeather.OpenWeather.List[]);
            var __list__b__ = false;
            var __city__ = default(global::SimpleWeather.OpenWeather.City);
            var __city__b__ = false;

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
                        __list__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.List[]>().Deserialize(ref reader, formatterResolver);
                        __list__b__ = true;
                        break;
                    case 1:
                        __city__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.OpenWeather.City>().Deserialize(ref reader, formatterResolver);
                        __city__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.OpenWeather.ForecastRootobject();
            if(__list__b__) ____result.list = __list__;
            if(__city__b__) ____result.city = __city__;

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
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("cod"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("message"), 1},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("cod"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("message"),
                
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
            writer.WriteInt32(value.cod);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.message);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.OpenWeather.Rootobject Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __cod__ = default(int);
            var __cod__b__ = false;
            var __message__ = default(string);
            var __message__b__ = false;

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
                        __cod__ = reader.ReadInt32();
                        __cod__b__ = true;
                        break;
                    case 1:
                        __message__ = reader.ReadString();
                        __message__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.OpenWeather.Rootobject();
            if(__cod__b__) ____result.cod = __cod__;
            if(__message__b__) ____result.message = __message__;

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

namespace SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.WeatherApi
{
    using System;
    using Utf8Json;


    public sealed class LocationItemFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.WeatherApi.LocationItem>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public LocationItemFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("id"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("name"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("region"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("country"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("lat"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("lon"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("url"), 6},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("id"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("name"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("region"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("country"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("lat"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("lon"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("url"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.WeatherApi.LocationItem value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteInt32(value.id);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.name);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.region);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteString(value.country);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteSingle(value.lat);
            writer.WriteRaw(this.____stringByteKeys[5]);
            writer.WriteSingle(value.lon);
            writer.WriteRaw(this.____stringByteKeys[6]);
            writer.WriteString(value.url);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.WeatherApi.LocationItem Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __id__ = default(int);
            var __id__b__ = false;
            var __name__ = default(string);
            var __name__b__ = false;
            var __region__ = default(string);
            var __region__b__ = false;
            var __country__ = default(string);
            var __country__b__ = false;
            var __lat__ = default(float);
            var __lat__b__ = false;
            var __lon__ = default(float);
            var __lon__b__ = false;
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
                        __id__ = reader.ReadInt32();
                        __id__b__ = true;
                        break;
                    case 1:
                        __name__ = reader.ReadString();
                        __name__b__ = true;
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
                        __lat__ = reader.ReadSingle();
                        __lat__b__ = true;
                        break;
                    case 5:
                        __lon__ = reader.ReadSingle();
                        __lon__b__ = true;
                        break;
                    case 6:
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

            var ____result = new global::SimpleWeather.WeatherApi.LocationItem();
            if(__id__b__) ____result.id = __id__;
            if(__name__b__) ____result.name = __name__;
            if(__region__b__) ____result.region = __region__;
            if(__country__b__) ____result.country = __country__;
            if(__lat__b__) ____result.lat = __lat__;
            if(__lon__b__) ____result.lon = __lon__;
            if(__url__b__) ____result.url = __url__;

            return ____result;
        }
    }


    public sealed class RootobjectFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.WeatherApi.Rootobject>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public RootobjectFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("LocationItems"), 0},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("LocationItems"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.WeatherApi.Rootobject value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherApi.LocationItem[]>().Serialize(ref writer, value.LocationItems, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.WeatherApi.Rootobject Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __LocationItems__ = default(global::SimpleWeather.WeatherApi.LocationItem[]);
            var __LocationItems__b__ = false;

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
                        __LocationItems__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherApi.LocationItem[]>().Deserialize(ref reader, formatterResolver);
                        __LocationItems__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.WeatherApi.Rootobject();
            if(__LocationItems__b__) ____result.LocationItems = __LocationItems__;

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

namespace SimpleWeather.Utf8JsonGen.Formatters.SimpleWeather.WeatherUnlocked
{
    using System;
    using Utf8Json;


    public sealed class CurrentRootobjectFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.WeatherUnlocked.CurrentRootobject>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public CurrentRootobjectFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("lat"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("lon"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("alt_m"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("alt_ft"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("wx_desc"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("wx_code"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("wx_icon"), 6},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("temp_c"), 7},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("temp_f"), 8},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("feelslike_c"), 9},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("feelslike_f"), 10},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("humid_pct"), 11},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("windspd_mph"), 12},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("windspd_kmh"), 13},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("windspd_kts"), 14},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("windspd_ms"), 15},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("winddir_deg"), 16},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("winddir_compass"), 17},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("cloudtotal_pct"), 18},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("vis_km"), 19},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("vis_mi"), 20},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("slp_mb"), 21},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("slp_in"), 22},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("dewpoint_c"), 23},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("dewpoint_f"), 24},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("lat"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("lon"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("alt_m"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("alt_ft"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("wx_desc"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("wx_code"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("wx_icon"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("temp_c"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("temp_f"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("feelslike_c"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("feelslike_f"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("humid_pct"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("windspd_mph"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("windspd_kmh"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("windspd_kts"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("windspd_ms"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("winddir_deg"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("winddir_compass"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("cloudtotal_pct"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("vis_km"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("vis_mi"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("slp_mb"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("slp_in"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("dewpoint_c"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("dewpoint_f"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.WeatherUnlocked.CurrentRootobject value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
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
            writer.WriteSingle(value.alt_m);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteSingle(value.alt_ft);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteString(value.wx_desc);
            writer.WriteRaw(this.____stringByteKeys[5]);
            writer.WriteInt32(value.wx_code);
            writer.WriteRaw(this.____stringByteKeys[6]);
            writer.WriteString(value.wx_icon);
            writer.WriteRaw(this.____stringByteKeys[7]);
            writer.WriteSingle(value.temp_c);
            writer.WriteRaw(this.____stringByteKeys[8]);
            writer.WriteSingle(value.temp_f);
            writer.WriteRaw(this.____stringByteKeys[9]);
            writer.WriteSingle(value.feelslike_c);
            writer.WriteRaw(this.____stringByteKeys[10]);
            writer.WriteSingle(value.feelslike_f);
            writer.WriteRaw(this.____stringByteKeys[11]);
            writer.WriteSingle(value.humid_pct);
            writer.WriteRaw(this.____stringByteKeys[12]);
            writer.WriteSingle(value.windspd_mph);
            writer.WriteRaw(this.____stringByteKeys[13]);
            writer.WriteSingle(value.windspd_kmh);
            writer.WriteRaw(this.____stringByteKeys[14]);
            writer.WriteSingle(value.windspd_kts);
            writer.WriteRaw(this.____stringByteKeys[15]);
            writer.WriteSingle(value.windspd_ms);
            writer.WriteRaw(this.____stringByteKeys[16]);
            writer.WriteSingle(value.winddir_deg);
            writer.WriteRaw(this.____stringByteKeys[17]);
            writer.WriteString(value.winddir_compass);
            writer.WriteRaw(this.____stringByteKeys[18]);
            writer.WriteSingle(value.cloudtotal_pct);
            writer.WriteRaw(this.____stringByteKeys[19]);
            writer.WriteSingle(value.vis_km);
            writer.WriteRaw(this.____stringByteKeys[20]);
            writer.WriteSingle(value.vis_mi);
            writer.WriteRaw(this.____stringByteKeys[21]);
            writer.WriteSingle(value.slp_mb);
            writer.WriteRaw(this.____stringByteKeys[22]);
            writer.WriteSingle(value.slp_in);
            writer.WriteRaw(this.____stringByteKeys[23]);
            writer.WriteSingle(value.dewpoint_c);
            writer.WriteRaw(this.____stringByteKeys[24]);
            writer.WriteSingle(value.dewpoint_f);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.WeatherUnlocked.CurrentRootobject Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __lat__ = default(float);
            var __lat__b__ = false;
            var __lon__ = default(float);
            var __lon__b__ = false;
            var __alt_m__ = default(float);
            var __alt_m__b__ = false;
            var __alt_ft__ = default(float);
            var __alt_ft__b__ = false;
            var __wx_desc__ = default(string);
            var __wx_desc__b__ = false;
            var __wx_code__ = default(int);
            var __wx_code__b__ = false;
            var __wx_icon__ = default(string);
            var __wx_icon__b__ = false;
            var __temp_c__ = default(float);
            var __temp_c__b__ = false;
            var __temp_f__ = default(float);
            var __temp_f__b__ = false;
            var __feelslike_c__ = default(float);
            var __feelslike_c__b__ = false;
            var __feelslike_f__ = default(float);
            var __feelslike_f__b__ = false;
            var __humid_pct__ = default(float);
            var __humid_pct__b__ = false;
            var __windspd_mph__ = default(float);
            var __windspd_mph__b__ = false;
            var __windspd_kmh__ = default(float);
            var __windspd_kmh__b__ = false;
            var __windspd_kts__ = default(float);
            var __windspd_kts__b__ = false;
            var __windspd_ms__ = default(float);
            var __windspd_ms__b__ = false;
            var __winddir_deg__ = default(float);
            var __winddir_deg__b__ = false;
            var __winddir_compass__ = default(string);
            var __winddir_compass__b__ = false;
            var __cloudtotal_pct__ = default(float);
            var __cloudtotal_pct__b__ = false;
            var __vis_km__ = default(float);
            var __vis_km__b__ = false;
            var __vis_mi__ = default(float);
            var __vis_mi__b__ = false;
            var __slp_mb__ = default(float);
            var __slp_mb__b__ = false;
            var __slp_in__ = default(float);
            var __slp_in__b__ = false;
            var __dewpoint_c__ = default(float);
            var __dewpoint_c__b__ = false;
            var __dewpoint_f__ = default(float);
            var __dewpoint_f__b__ = false;

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
                        __alt_m__ = reader.ReadSingle();
                        __alt_m__b__ = true;
                        break;
                    case 3:
                        __alt_ft__ = reader.ReadSingle();
                        __alt_ft__b__ = true;
                        break;
                    case 4:
                        __wx_desc__ = reader.ReadString();
                        __wx_desc__b__ = true;
                        break;
                    case 5:
                        __wx_code__ = reader.ReadInt32();
                        __wx_code__b__ = true;
                        break;
                    case 6:
                        __wx_icon__ = reader.ReadString();
                        __wx_icon__b__ = true;
                        break;
                    case 7:
                        __temp_c__ = reader.ReadSingle();
                        __temp_c__b__ = true;
                        break;
                    case 8:
                        __temp_f__ = reader.ReadSingle();
                        __temp_f__b__ = true;
                        break;
                    case 9:
                        __feelslike_c__ = reader.ReadSingle();
                        __feelslike_c__b__ = true;
                        break;
                    case 10:
                        __feelslike_f__ = reader.ReadSingle();
                        __feelslike_f__b__ = true;
                        break;
                    case 11:
                        __humid_pct__ = reader.ReadSingle();
                        __humid_pct__b__ = true;
                        break;
                    case 12:
                        __windspd_mph__ = reader.ReadSingle();
                        __windspd_mph__b__ = true;
                        break;
                    case 13:
                        __windspd_kmh__ = reader.ReadSingle();
                        __windspd_kmh__b__ = true;
                        break;
                    case 14:
                        __windspd_kts__ = reader.ReadSingle();
                        __windspd_kts__b__ = true;
                        break;
                    case 15:
                        __windspd_ms__ = reader.ReadSingle();
                        __windspd_ms__b__ = true;
                        break;
                    case 16:
                        __winddir_deg__ = reader.ReadSingle();
                        __winddir_deg__b__ = true;
                        break;
                    case 17:
                        __winddir_compass__ = reader.ReadString();
                        __winddir_compass__b__ = true;
                        break;
                    case 18:
                        __cloudtotal_pct__ = reader.ReadSingle();
                        __cloudtotal_pct__b__ = true;
                        break;
                    case 19:
                        __vis_km__ = reader.ReadSingle();
                        __vis_km__b__ = true;
                        break;
                    case 20:
                        __vis_mi__ = reader.ReadSingle();
                        __vis_mi__b__ = true;
                        break;
                    case 21:
                        __slp_mb__ = reader.ReadSingle();
                        __slp_mb__b__ = true;
                        break;
                    case 22:
                        __slp_in__ = reader.ReadSingle();
                        __slp_in__b__ = true;
                        break;
                    case 23:
                        __dewpoint_c__ = reader.ReadSingle();
                        __dewpoint_c__b__ = true;
                        break;
                    case 24:
                        __dewpoint_f__ = reader.ReadSingle();
                        __dewpoint_f__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.WeatherUnlocked.CurrentRootobject();
            if(__lat__b__) ____result.lat = __lat__;
            if(__lon__b__) ____result.lon = __lon__;
            if(__alt_m__b__) ____result.alt_m = __alt_m__;
            if(__alt_ft__b__) ____result.alt_ft = __alt_ft__;
            if(__wx_desc__b__) ____result.wx_desc = __wx_desc__;
            if(__wx_code__b__) ____result.wx_code = __wx_code__;
            if(__wx_icon__b__) ____result.wx_icon = __wx_icon__;
            if(__temp_c__b__) ____result.temp_c = __temp_c__;
            if(__temp_f__b__) ____result.temp_f = __temp_f__;
            if(__feelslike_c__b__) ____result.feelslike_c = __feelslike_c__;
            if(__feelslike_f__b__) ____result.feelslike_f = __feelslike_f__;
            if(__humid_pct__b__) ____result.humid_pct = __humid_pct__;
            if(__windspd_mph__b__) ____result.windspd_mph = __windspd_mph__;
            if(__windspd_kmh__b__) ____result.windspd_kmh = __windspd_kmh__;
            if(__windspd_kts__b__) ____result.windspd_kts = __windspd_kts__;
            if(__windspd_ms__b__) ____result.windspd_ms = __windspd_ms__;
            if(__winddir_deg__b__) ____result.winddir_deg = __winddir_deg__;
            if(__winddir_compass__b__) ____result.winddir_compass = __winddir_compass__;
            if(__cloudtotal_pct__b__) ____result.cloudtotal_pct = __cloudtotal_pct__;
            if(__vis_km__b__) ____result.vis_km = __vis_km__;
            if(__vis_mi__b__) ____result.vis_mi = __vis_mi__;
            if(__slp_mb__b__) ____result.slp_mb = __slp_mb__;
            if(__slp_in__b__) ____result.slp_in = __slp_in__;
            if(__dewpoint_c__b__) ____result.dewpoint_c = __dewpoint_c__;
            if(__dewpoint_f__b__) ____result.dewpoint_f = __dewpoint_f__;

            return ____result;
        }
    }


    public sealed class TimeframeFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.WeatherUnlocked.Timeframe>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public TimeframeFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("date"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("time"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("utcdate"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("utctime"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("wx_desc"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("wx_code"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("wx_icon"), 6},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("temp_c"), 7},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("temp_f"), 8},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("feelslike_c"), 9},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("feelslike_f"), 10},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("winddir_deg"), 11},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("winddir_compass"), 12},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("windspd_mph"), 13},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("windspd_kmh"), 14},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("windspd_kts"), 15},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("windspd_ms"), 16},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("windgst_mph"), 17},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("windgst_kmh"), 18},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("windgst_kts"), 19},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("windgst_ms"), 20},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("cloud_low_pct"), 21},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("cloud_mid_pct"), 22},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("cloud_high_pct"), 23},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("cloudtotal_pct"), 24},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("precip_mm"), 25},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("precip_in"), 26},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("rain_mm"), 27},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("rain_in"), 28},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("snow_mm"), 29},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("snow_in"), 30},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("snow_accum_cm"), 31},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("snow_accum_in"), 32},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("prob_precip_pct"), 33},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("humid_pct"), 34},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("dewpoint_c"), 35},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("dewpoint_f"), 36},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("vis_km"), 37},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("vis_mi"), 38},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("slp_mb"), 39},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("slp_in"), 40},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("date"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("time"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("utcdate"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("utctime"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("wx_desc"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("wx_code"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("wx_icon"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("temp_c"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("temp_f"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("feelslike_c"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("feelslike_f"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("winddir_deg"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("winddir_compass"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("windspd_mph"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("windspd_kmh"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("windspd_kts"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("windspd_ms"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("windgst_mph"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("windgst_kmh"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("windgst_kts"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("windgst_ms"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("cloud_low_pct"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("cloud_mid_pct"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("cloud_high_pct"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("cloudtotal_pct"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("precip_mm"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("precip_in"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("rain_mm"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("rain_in"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("snow_mm"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("snow_in"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("snow_accum_cm"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("snow_accum_in"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("prob_precip_pct"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("humid_pct"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("dewpoint_c"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("dewpoint_f"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("vis_km"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("vis_mi"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("slp_mb"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("slp_in"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.WeatherUnlocked.Timeframe value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.date);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteInt32(value.time);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.utcdate);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteInt32(value.utctime);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteString(value.wx_desc);
            writer.WriteRaw(this.____stringByteKeys[5]);
            writer.WriteInt32(value.wx_code);
            writer.WriteRaw(this.____stringByteKeys[6]);
            writer.WriteString(value.wx_icon);
            writer.WriteRaw(this.____stringByteKeys[7]);
            writer.WriteSingle(value.temp_c);
            writer.WriteRaw(this.____stringByteKeys[8]);
            writer.WriteSingle(value.temp_f);
            writer.WriteRaw(this.____stringByteKeys[9]);
            writer.WriteSingle(value.feelslike_c);
            writer.WriteRaw(this.____stringByteKeys[10]);
            writer.WriteSingle(value.feelslike_f);
            writer.WriteRaw(this.____stringByteKeys[11]);
            writer.WriteSingle(value.winddir_deg);
            writer.WriteRaw(this.____stringByteKeys[12]);
            writer.WriteString(value.winddir_compass);
            writer.WriteRaw(this.____stringByteKeys[13]);
            writer.WriteSingle(value.windspd_mph);
            writer.WriteRaw(this.____stringByteKeys[14]);
            writer.WriteSingle(value.windspd_kmh);
            writer.WriteRaw(this.____stringByteKeys[15]);
            writer.WriteSingle(value.windspd_kts);
            writer.WriteRaw(this.____stringByteKeys[16]);
            writer.WriteSingle(value.windspd_ms);
            writer.WriteRaw(this.____stringByteKeys[17]);
            writer.WriteSingle(value.windgst_mph);
            writer.WriteRaw(this.____stringByteKeys[18]);
            writer.WriteSingle(value.windgst_kmh);
            writer.WriteRaw(this.____stringByteKeys[19]);
            writer.WriteSingle(value.windgst_kts);
            writer.WriteRaw(this.____stringByteKeys[20]);
            writer.WriteSingle(value.windgst_ms);
            writer.WriteRaw(this.____stringByteKeys[21]);
            writer.WriteSingle(value.cloud_low_pct);
            writer.WriteRaw(this.____stringByteKeys[22]);
            writer.WriteSingle(value.cloud_mid_pct);
            writer.WriteRaw(this.____stringByteKeys[23]);
            writer.WriteSingle(value.cloud_high_pct);
            writer.WriteRaw(this.____stringByteKeys[24]);
            writer.WriteSingle(value.cloudtotal_pct);
            writer.WriteRaw(this.____stringByteKeys[25]);
            writer.WriteSingle(value.precip_mm);
            writer.WriteRaw(this.____stringByteKeys[26]);
            writer.WriteSingle(value.precip_in);
            writer.WriteRaw(this.____stringByteKeys[27]);
            writer.WriteSingle(value.rain_mm);
            writer.WriteRaw(this.____stringByteKeys[28]);
            writer.WriteSingle(value.rain_in);
            writer.WriteRaw(this.____stringByteKeys[29]);
            writer.WriteSingle(value.snow_mm);
            writer.WriteRaw(this.____stringByteKeys[30]);
            writer.WriteSingle(value.snow_in);
            writer.WriteRaw(this.____stringByteKeys[31]);
            writer.WriteSingle(value.snow_accum_cm);
            writer.WriteRaw(this.____stringByteKeys[32]);
            writer.WriteSingle(value.snow_accum_in);
            writer.WriteRaw(this.____stringByteKeys[33]);
            writer.WriteString(value.prob_precip_pct);
            writer.WriteRaw(this.____stringByteKeys[34]);
            writer.WriteSingle(value.humid_pct);
            writer.WriteRaw(this.____stringByteKeys[35]);
            writer.WriteSingle(value.dewpoint_c);
            writer.WriteRaw(this.____stringByteKeys[36]);
            writer.WriteSingle(value.dewpoint_f);
            writer.WriteRaw(this.____stringByteKeys[37]);
            writer.WriteSingle(value.vis_km);
            writer.WriteRaw(this.____stringByteKeys[38]);
            writer.WriteSingle(value.vis_mi);
            writer.WriteRaw(this.____stringByteKeys[39]);
            writer.WriteSingle(value.slp_mb);
            writer.WriteRaw(this.____stringByteKeys[40]);
            writer.WriteSingle(value.slp_in);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.WeatherUnlocked.Timeframe Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __date__ = default(string);
            var __date__b__ = false;
            var __time__ = default(int);
            var __time__b__ = false;
            var __utcdate__ = default(string);
            var __utcdate__b__ = false;
            var __utctime__ = default(int);
            var __utctime__b__ = false;
            var __wx_desc__ = default(string);
            var __wx_desc__b__ = false;
            var __wx_code__ = default(int);
            var __wx_code__b__ = false;
            var __wx_icon__ = default(string);
            var __wx_icon__b__ = false;
            var __temp_c__ = default(float);
            var __temp_c__b__ = false;
            var __temp_f__ = default(float);
            var __temp_f__b__ = false;
            var __feelslike_c__ = default(float);
            var __feelslike_c__b__ = false;
            var __feelslike_f__ = default(float);
            var __feelslike_f__b__ = false;
            var __winddir_deg__ = default(float);
            var __winddir_deg__b__ = false;
            var __winddir_compass__ = default(string);
            var __winddir_compass__b__ = false;
            var __windspd_mph__ = default(float);
            var __windspd_mph__b__ = false;
            var __windspd_kmh__ = default(float);
            var __windspd_kmh__b__ = false;
            var __windspd_kts__ = default(float);
            var __windspd_kts__b__ = false;
            var __windspd_ms__ = default(float);
            var __windspd_ms__b__ = false;
            var __windgst_mph__ = default(float);
            var __windgst_mph__b__ = false;
            var __windgst_kmh__ = default(float);
            var __windgst_kmh__b__ = false;
            var __windgst_kts__ = default(float);
            var __windgst_kts__b__ = false;
            var __windgst_ms__ = default(float);
            var __windgst_ms__b__ = false;
            var __cloud_low_pct__ = default(float);
            var __cloud_low_pct__b__ = false;
            var __cloud_mid_pct__ = default(float);
            var __cloud_mid_pct__b__ = false;
            var __cloud_high_pct__ = default(float);
            var __cloud_high_pct__b__ = false;
            var __cloudtotal_pct__ = default(float);
            var __cloudtotal_pct__b__ = false;
            var __precip_mm__ = default(float);
            var __precip_mm__b__ = false;
            var __precip_in__ = default(float);
            var __precip_in__b__ = false;
            var __rain_mm__ = default(float);
            var __rain_mm__b__ = false;
            var __rain_in__ = default(float);
            var __rain_in__b__ = false;
            var __snow_mm__ = default(float);
            var __snow_mm__b__ = false;
            var __snow_in__ = default(float);
            var __snow_in__b__ = false;
            var __snow_accum_cm__ = default(float);
            var __snow_accum_cm__b__ = false;
            var __snow_accum_in__ = default(float);
            var __snow_accum_in__b__ = false;
            var __prob_precip_pct__ = default(string);
            var __prob_precip_pct__b__ = false;
            var __humid_pct__ = default(float);
            var __humid_pct__b__ = false;
            var __dewpoint_c__ = default(float);
            var __dewpoint_c__b__ = false;
            var __dewpoint_f__ = default(float);
            var __dewpoint_f__b__ = false;
            var __vis_km__ = default(float);
            var __vis_km__b__ = false;
            var __vis_mi__ = default(float);
            var __vis_mi__b__ = false;
            var __slp_mb__ = default(float);
            var __slp_mb__b__ = false;
            var __slp_in__ = default(float);
            var __slp_in__b__ = false;

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
                        __date__ = reader.ReadString();
                        __date__b__ = true;
                        break;
                    case 1:
                        __time__ = reader.ReadInt32();
                        __time__b__ = true;
                        break;
                    case 2:
                        __utcdate__ = reader.ReadString();
                        __utcdate__b__ = true;
                        break;
                    case 3:
                        __utctime__ = reader.ReadInt32();
                        __utctime__b__ = true;
                        break;
                    case 4:
                        __wx_desc__ = reader.ReadString();
                        __wx_desc__b__ = true;
                        break;
                    case 5:
                        __wx_code__ = reader.ReadInt32();
                        __wx_code__b__ = true;
                        break;
                    case 6:
                        __wx_icon__ = reader.ReadString();
                        __wx_icon__b__ = true;
                        break;
                    case 7:
                        __temp_c__ = reader.ReadSingle();
                        __temp_c__b__ = true;
                        break;
                    case 8:
                        __temp_f__ = reader.ReadSingle();
                        __temp_f__b__ = true;
                        break;
                    case 9:
                        __feelslike_c__ = reader.ReadSingle();
                        __feelslike_c__b__ = true;
                        break;
                    case 10:
                        __feelslike_f__ = reader.ReadSingle();
                        __feelslike_f__b__ = true;
                        break;
                    case 11:
                        __winddir_deg__ = reader.ReadSingle();
                        __winddir_deg__b__ = true;
                        break;
                    case 12:
                        __winddir_compass__ = reader.ReadString();
                        __winddir_compass__b__ = true;
                        break;
                    case 13:
                        __windspd_mph__ = reader.ReadSingle();
                        __windspd_mph__b__ = true;
                        break;
                    case 14:
                        __windspd_kmh__ = reader.ReadSingle();
                        __windspd_kmh__b__ = true;
                        break;
                    case 15:
                        __windspd_kts__ = reader.ReadSingle();
                        __windspd_kts__b__ = true;
                        break;
                    case 16:
                        __windspd_ms__ = reader.ReadSingle();
                        __windspd_ms__b__ = true;
                        break;
                    case 17:
                        __windgst_mph__ = reader.ReadSingle();
                        __windgst_mph__b__ = true;
                        break;
                    case 18:
                        __windgst_kmh__ = reader.ReadSingle();
                        __windgst_kmh__b__ = true;
                        break;
                    case 19:
                        __windgst_kts__ = reader.ReadSingle();
                        __windgst_kts__b__ = true;
                        break;
                    case 20:
                        __windgst_ms__ = reader.ReadSingle();
                        __windgst_ms__b__ = true;
                        break;
                    case 21:
                        __cloud_low_pct__ = reader.ReadSingle();
                        __cloud_low_pct__b__ = true;
                        break;
                    case 22:
                        __cloud_mid_pct__ = reader.ReadSingle();
                        __cloud_mid_pct__b__ = true;
                        break;
                    case 23:
                        __cloud_high_pct__ = reader.ReadSingle();
                        __cloud_high_pct__b__ = true;
                        break;
                    case 24:
                        __cloudtotal_pct__ = reader.ReadSingle();
                        __cloudtotal_pct__b__ = true;
                        break;
                    case 25:
                        __precip_mm__ = reader.ReadSingle();
                        __precip_mm__b__ = true;
                        break;
                    case 26:
                        __precip_in__ = reader.ReadSingle();
                        __precip_in__b__ = true;
                        break;
                    case 27:
                        __rain_mm__ = reader.ReadSingle();
                        __rain_mm__b__ = true;
                        break;
                    case 28:
                        __rain_in__ = reader.ReadSingle();
                        __rain_in__b__ = true;
                        break;
                    case 29:
                        __snow_mm__ = reader.ReadSingle();
                        __snow_mm__b__ = true;
                        break;
                    case 30:
                        __snow_in__ = reader.ReadSingle();
                        __snow_in__b__ = true;
                        break;
                    case 31:
                        __snow_accum_cm__ = reader.ReadSingle();
                        __snow_accum_cm__b__ = true;
                        break;
                    case 32:
                        __snow_accum_in__ = reader.ReadSingle();
                        __snow_accum_in__b__ = true;
                        break;
                    case 33:
                        __prob_precip_pct__ = reader.ReadString();
                        __prob_precip_pct__b__ = true;
                        break;
                    case 34:
                        __humid_pct__ = reader.ReadSingle();
                        __humid_pct__b__ = true;
                        break;
                    case 35:
                        __dewpoint_c__ = reader.ReadSingle();
                        __dewpoint_c__b__ = true;
                        break;
                    case 36:
                        __dewpoint_f__ = reader.ReadSingle();
                        __dewpoint_f__b__ = true;
                        break;
                    case 37:
                        __vis_km__ = reader.ReadSingle();
                        __vis_km__b__ = true;
                        break;
                    case 38:
                        __vis_mi__ = reader.ReadSingle();
                        __vis_mi__b__ = true;
                        break;
                    case 39:
                        __slp_mb__ = reader.ReadSingle();
                        __slp_mb__b__ = true;
                        break;
                    case 40:
                        __slp_in__ = reader.ReadSingle();
                        __slp_in__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.WeatherUnlocked.Timeframe();
            if(__date__b__) ____result.date = __date__;
            if(__time__b__) ____result.time = __time__;
            if(__utcdate__b__) ____result.utcdate = __utcdate__;
            if(__utctime__b__) ____result.utctime = __utctime__;
            if(__wx_desc__b__) ____result.wx_desc = __wx_desc__;
            if(__wx_code__b__) ____result.wx_code = __wx_code__;
            if(__wx_icon__b__) ____result.wx_icon = __wx_icon__;
            if(__temp_c__b__) ____result.temp_c = __temp_c__;
            if(__temp_f__b__) ____result.temp_f = __temp_f__;
            if(__feelslike_c__b__) ____result.feelslike_c = __feelslike_c__;
            if(__feelslike_f__b__) ____result.feelslike_f = __feelslike_f__;
            if(__winddir_deg__b__) ____result.winddir_deg = __winddir_deg__;
            if(__winddir_compass__b__) ____result.winddir_compass = __winddir_compass__;
            if(__windspd_mph__b__) ____result.windspd_mph = __windspd_mph__;
            if(__windspd_kmh__b__) ____result.windspd_kmh = __windspd_kmh__;
            if(__windspd_kts__b__) ____result.windspd_kts = __windspd_kts__;
            if(__windspd_ms__b__) ____result.windspd_ms = __windspd_ms__;
            if(__windgst_mph__b__) ____result.windgst_mph = __windgst_mph__;
            if(__windgst_kmh__b__) ____result.windgst_kmh = __windgst_kmh__;
            if(__windgst_kts__b__) ____result.windgst_kts = __windgst_kts__;
            if(__windgst_ms__b__) ____result.windgst_ms = __windgst_ms__;
            if(__cloud_low_pct__b__) ____result.cloud_low_pct = __cloud_low_pct__;
            if(__cloud_mid_pct__b__) ____result.cloud_mid_pct = __cloud_mid_pct__;
            if(__cloud_high_pct__b__) ____result.cloud_high_pct = __cloud_high_pct__;
            if(__cloudtotal_pct__b__) ____result.cloudtotal_pct = __cloudtotal_pct__;
            if(__precip_mm__b__) ____result.precip_mm = __precip_mm__;
            if(__precip_in__b__) ____result.precip_in = __precip_in__;
            if(__rain_mm__b__) ____result.rain_mm = __rain_mm__;
            if(__rain_in__b__) ____result.rain_in = __rain_in__;
            if(__snow_mm__b__) ____result.snow_mm = __snow_mm__;
            if(__snow_in__b__) ____result.snow_in = __snow_in__;
            if(__snow_accum_cm__b__) ____result.snow_accum_cm = __snow_accum_cm__;
            if(__snow_accum_in__b__) ____result.snow_accum_in = __snow_accum_in__;
            if(__prob_precip_pct__b__) ____result.prob_precip_pct = __prob_precip_pct__;
            if(__humid_pct__b__) ____result.humid_pct = __humid_pct__;
            if(__dewpoint_c__b__) ____result.dewpoint_c = __dewpoint_c__;
            if(__dewpoint_f__b__) ____result.dewpoint_f = __dewpoint_f__;
            if(__vis_km__b__) ____result.vis_km = __vis_km__;
            if(__vis_mi__b__) ____result.vis_mi = __vis_mi__;
            if(__slp_mb__b__) ____result.slp_mb = __slp_mb__;
            if(__slp_in__b__) ____result.slp_in = __slp_in__;

            return ____result;
        }
    }


    public sealed class DayFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.WeatherUnlocked.Day>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public DayFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("date"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("sunrise_time"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("sunset_time"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("moonrise_time"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("moonset_time"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("temp_max_c"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("temp_max_f"), 6},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("temp_min_c"), 7},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("temp_min_f"), 8},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("precip_total_mm"), 9},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("precip_total_in"), 10},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("rain_total_mm"), 11},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("rain_total_in"), 12},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("snow_total_mm"), 13},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("snow_total_in"), 14},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("prob_precip_pct"), 15},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("humid_max_pct"), 16},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("humid_min_pct"), 17},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("windspd_max_mph"), 18},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("windspd_max_kmh"), 19},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("windspd_max_kts"), 20},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("windspd_max_ms"), 21},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("windgst_max_mph"), 22},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("windgst_max_kmh"), 23},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("windgst_max_kts"), 24},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("windgst_max_ms"), 25},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("slp_max_in"), 26},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("slp_max_mb"), 27},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("slp_min_in"), 28},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("slp_min_mb"), 29},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("Timeframes"), 30},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("date"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("sunrise_time"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("sunset_time"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("moonrise_time"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("moonset_time"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("temp_max_c"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("temp_max_f"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("temp_min_c"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("temp_min_f"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("precip_total_mm"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("precip_total_in"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("rain_total_mm"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("rain_total_in"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("snow_total_mm"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("snow_total_in"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("prob_precip_pct"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("humid_max_pct"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("humid_min_pct"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("windspd_max_mph"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("windspd_max_kmh"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("windspd_max_kts"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("windspd_max_ms"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("windgst_max_mph"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("windgst_max_kmh"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("windgst_max_kts"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("windgst_max_ms"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("slp_max_in"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("slp_max_mb"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("slp_min_in"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("slp_min_mb"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("Timeframes"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.WeatherUnlocked.Day value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.date);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.sunrise_time);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.sunset_time);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteString(value.moonrise_time);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteString(value.moonset_time);
            writer.WriteRaw(this.____stringByteKeys[5]);
            writer.WriteSingle(value.temp_max_c);
            writer.WriteRaw(this.____stringByteKeys[6]);
            writer.WriteSingle(value.temp_max_f);
            writer.WriteRaw(this.____stringByteKeys[7]);
            writer.WriteSingle(value.temp_min_c);
            writer.WriteRaw(this.____stringByteKeys[8]);
            writer.WriteSingle(value.temp_min_f);
            writer.WriteRaw(this.____stringByteKeys[9]);
            writer.WriteSingle(value.precip_total_mm);
            writer.WriteRaw(this.____stringByteKeys[10]);
            writer.WriteSingle(value.precip_total_in);
            writer.WriteRaw(this.____stringByteKeys[11]);
            writer.WriteSingle(value.rain_total_mm);
            writer.WriteRaw(this.____stringByteKeys[12]);
            writer.WriteSingle(value.rain_total_in);
            writer.WriteRaw(this.____stringByteKeys[13]);
            writer.WriteSingle(value.snow_total_mm);
            writer.WriteRaw(this.____stringByteKeys[14]);
            writer.WriteSingle(value.snow_total_in);
            writer.WriteRaw(this.____stringByteKeys[15]);
            writer.WriteSingle(value.prob_precip_pct);
            writer.WriteRaw(this.____stringByteKeys[16]);
            writer.WriteSingle(value.humid_max_pct);
            writer.WriteRaw(this.____stringByteKeys[17]);
            writer.WriteSingle(value.humid_min_pct);
            writer.WriteRaw(this.____stringByteKeys[18]);
            writer.WriteSingle(value.windspd_max_mph);
            writer.WriteRaw(this.____stringByteKeys[19]);
            writer.WriteSingle(value.windspd_max_kmh);
            writer.WriteRaw(this.____stringByteKeys[20]);
            writer.WriteSingle(value.windspd_max_kts);
            writer.WriteRaw(this.____stringByteKeys[21]);
            writer.WriteSingle(value.windspd_max_ms);
            writer.WriteRaw(this.____stringByteKeys[22]);
            writer.WriteSingle(value.windgst_max_mph);
            writer.WriteRaw(this.____stringByteKeys[23]);
            writer.WriteSingle(value.windgst_max_kmh);
            writer.WriteRaw(this.____stringByteKeys[24]);
            writer.WriteSingle(value.windgst_max_kts);
            writer.WriteRaw(this.____stringByteKeys[25]);
            writer.WriteSingle(value.windgst_max_ms);
            writer.WriteRaw(this.____stringByteKeys[26]);
            writer.WriteSingle(value.slp_max_in);
            writer.WriteRaw(this.____stringByteKeys[27]);
            writer.WriteSingle(value.slp_max_mb);
            writer.WriteRaw(this.____stringByteKeys[28]);
            writer.WriteSingle(value.slp_min_in);
            writer.WriteRaw(this.____stringByteKeys[29]);
            writer.WriteSingle(value.slp_min_mb);
            writer.WriteRaw(this.____stringByteKeys[30]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherUnlocked.Timeframe[]>().Serialize(ref writer, value.Timeframes, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.WeatherUnlocked.Day Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __date__ = default(string);
            var __date__b__ = false;
            var __sunrise_time__ = default(string);
            var __sunrise_time__b__ = false;
            var __sunset_time__ = default(string);
            var __sunset_time__b__ = false;
            var __moonrise_time__ = default(string);
            var __moonrise_time__b__ = false;
            var __moonset_time__ = default(string);
            var __moonset_time__b__ = false;
            var __temp_max_c__ = default(float);
            var __temp_max_c__b__ = false;
            var __temp_max_f__ = default(float);
            var __temp_max_f__b__ = false;
            var __temp_min_c__ = default(float);
            var __temp_min_c__b__ = false;
            var __temp_min_f__ = default(float);
            var __temp_min_f__b__ = false;
            var __precip_total_mm__ = default(float);
            var __precip_total_mm__b__ = false;
            var __precip_total_in__ = default(float);
            var __precip_total_in__b__ = false;
            var __rain_total_mm__ = default(float);
            var __rain_total_mm__b__ = false;
            var __rain_total_in__ = default(float);
            var __rain_total_in__b__ = false;
            var __snow_total_mm__ = default(float);
            var __snow_total_mm__b__ = false;
            var __snow_total_in__ = default(float);
            var __snow_total_in__b__ = false;
            var __prob_precip_pct__ = default(float);
            var __prob_precip_pct__b__ = false;
            var __humid_max_pct__ = default(float);
            var __humid_max_pct__b__ = false;
            var __humid_min_pct__ = default(float);
            var __humid_min_pct__b__ = false;
            var __windspd_max_mph__ = default(float);
            var __windspd_max_mph__b__ = false;
            var __windspd_max_kmh__ = default(float);
            var __windspd_max_kmh__b__ = false;
            var __windspd_max_kts__ = default(float);
            var __windspd_max_kts__b__ = false;
            var __windspd_max_ms__ = default(float);
            var __windspd_max_ms__b__ = false;
            var __windgst_max_mph__ = default(float);
            var __windgst_max_mph__b__ = false;
            var __windgst_max_kmh__ = default(float);
            var __windgst_max_kmh__b__ = false;
            var __windgst_max_kts__ = default(float);
            var __windgst_max_kts__b__ = false;
            var __windgst_max_ms__ = default(float);
            var __windgst_max_ms__b__ = false;
            var __slp_max_in__ = default(float);
            var __slp_max_in__b__ = false;
            var __slp_max_mb__ = default(float);
            var __slp_max_mb__b__ = false;
            var __slp_min_in__ = default(float);
            var __slp_min_in__b__ = false;
            var __slp_min_mb__ = default(float);
            var __slp_min_mb__b__ = false;
            var __Timeframes__ = default(global::SimpleWeather.WeatherUnlocked.Timeframe[]);
            var __Timeframes__b__ = false;

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
                        __date__ = reader.ReadString();
                        __date__b__ = true;
                        break;
                    case 1:
                        __sunrise_time__ = reader.ReadString();
                        __sunrise_time__b__ = true;
                        break;
                    case 2:
                        __sunset_time__ = reader.ReadString();
                        __sunset_time__b__ = true;
                        break;
                    case 3:
                        __moonrise_time__ = reader.ReadString();
                        __moonrise_time__b__ = true;
                        break;
                    case 4:
                        __moonset_time__ = reader.ReadString();
                        __moonset_time__b__ = true;
                        break;
                    case 5:
                        __temp_max_c__ = reader.ReadSingle();
                        __temp_max_c__b__ = true;
                        break;
                    case 6:
                        __temp_max_f__ = reader.ReadSingle();
                        __temp_max_f__b__ = true;
                        break;
                    case 7:
                        __temp_min_c__ = reader.ReadSingle();
                        __temp_min_c__b__ = true;
                        break;
                    case 8:
                        __temp_min_f__ = reader.ReadSingle();
                        __temp_min_f__b__ = true;
                        break;
                    case 9:
                        __precip_total_mm__ = reader.ReadSingle();
                        __precip_total_mm__b__ = true;
                        break;
                    case 10:
                        __precip_total_in__ = reader.ReadSingle();
                        __precip_total_in__b__ = true;
                        break;
                    case 11:
                        __rain_total_mm__ = reader.ReadSingle();
                        __rain_total_mm__b__ = true;
                        break;
                    case 12:
                        __rain_total_in__ = reader.ReadSingle();
                        __rain_total_in__b__ = true;
                        break;
                    case 13:
                        __snow_total_mm__ = reader.ReadSingle();
                        __snow_total_mm__b__ = true;
                        break;
                    case 14:
                        __snow_total_in__ = reader.ReadSingle();
                        __snow_total_in__b__ = true;
                        break;
                    case 15:
                        __prob_precip_pct__ = reader.ReadSingle();
                        __prob_precip_pct__b__ = true;
                        break;
                    case 16:
                        __humid_max_pct__ = reader.ReadSingle();
                        __humid_max_pct__b__ = true;
                        break;
                    case 17:
                        __humid_min_pct__ = reader.ReadSingle();
                        __humid_min_pct__b__ = true;
                        break;
                    case 18:
                        __windspd_max_mph__ = reader.ReadSingle();
                        __windspd_max_mph__b__ = true;
                        break;
                    case 19:
                        __windspd_max_kmh__ = reader.ReadSingle();
                        __windspd_max_kmh__b__ = true;
                        break;
                    case 20:
                        __windspd_max_kts__ = reader.ReadSingle();
                        __windspd_max_kts__b__ = true;
                        break;
                    case 21:
                        __windspd_max_ms__ = reader.ReadSingle();
                        __windspd_max_ms__b__ = true;
                        break;
                    case 22:
                        __windgst_max_mph__ = reader.ReadSingle();
                        __windgst_max_mph__b__ = true;
                        break;
                    case 23:
                        __windgst_max_kmh__ = reader.ReadSingle();
                        __windgst_max_kmh__b__ = true;
                        break;
                    case 24:
                        __windgst_max_kts__ = reader.ReadSingle();
                        __windgst_max_kts__b__ = true;
                        break;
                    case 25:
                        __windgst_max_ms__ = reader.ReadSingle();
                        __windgst_max_ms__b__ = true;
                        break;
                    case 26:
                        __slp_max_in__ = reader.ReadSingle();
                        __slp_max_in__b__ = true;
                        break;
                    case 27:
                        __slp_max_mb__ = reader.ReadSingle();
                        __slp_max_mb__b__ = true;
                        break;
                    case 28:
                        __slp_min_in__ = reader.ReadSingle();
                        __slp_min_in__b__ = true;
                        break;
                    case 29:
                        __slp_min_mb__ = reader.ReadSingle();
                        __slp_min_mb__b__ = true;
                        break;
                    case 30:
                        __Timeframes__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherUnlocked.Timeframe[]>().Deserialize(ref reader, formatterResolver);
                        __Timeframes__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.WeatherUnlocked.Day();
            if(__date__b__) ____result.date = __date__;
            if(__sunrise_time__b__) ____result.sunrise_time = __sunrise_time__;
            if(__sunset_time__b__) ____result.sunset_time = __sunset_time__;
            if(__moonrise_time__b__) ____result.moonrise_time = __moonrise_time__;
            if(__moonset_time__b__) ____result.moonset_time = __moonset_time__;
            if(__temp_max_c__b__) ____result.temp_max_c = __temp_max_c__;
            if(__temp_max_f__b__) ____result.temp_max_f = __temp_max_f__;
            if(__temp_min_c__b__) ____result.temp_min_c = __temp_min_c__;
            if(__temp_min_f__b__) ____result.temp_min_f = __temp_min_f__;
            if(__precip_total_mm__b__) ____result.precip_total_mm = __precip_total_mm__;
            if(__precip_total_in__b__) ____result.precip_total_in = __precip_total_in__;
            if(__rain_total_mm__b__) ____result.rain_total_mm = __rain_total_mm__;
            if(__rain_total_in__b__) ____result.rain_total_in = __rain_total_in__;
            if(__snow_total_mm__b__) ____result.snow_total_mm = __snow_total_mm__;
            if(__snow_total_in__b__) ____result.snow_total_in = __snow_total_in__;
            if(__prob_precip_pct__b__) ____result.prob_precip_pct = __prob_precip_pct__;
            if(__humid_max_pct__b__) ____result.humid_max_pct = __humid_max_pct__;
            if(__humid_min_pct__b__) ____result.humid_min_pct = __humid_min_pct__;
            if(__windspd_max_mph__b__) ____result.windspd_max_mph = __windspd_max_mph__;
            if(__windspd_max_kmh__b__) ____result.windspd_max_kmh = __windspd_max_kmh__;
            if(__windspd_max_kts__b__) ____result.windspd_max_kts = __windspd_max_kts__;
            if(__windspd_max_ms__b__) ____result.windspd_max_ms = __windspd_max_ms__;
            if(__windgst_max_mph__b__) ____result.windgst_max_mph = __windgst_max_mph__;
            if(__windgst_max_kmh__b__) ____result.windgst_max_kmh = __windgst_max_kmh__;
            if(__windgst_max_kts__b__) ____result.windgst_max_kts = __windgst_max_kts__;
            if(__windgst_max_ms__b__) ____result.windgst_max_ms = __windgst_max_ms__;
            if(__slp_max_in__b__) ____result.slp_max_in = __slp_max_in__;
            if(__slp_max_mb__b__) ____result.slp_max_mb = __slp_max_mb__;
            if(__slp_min_in__b__) ____result.slp_min_in = __slp_min_in__;
            if(__slp_min_mb__b__) ____result.slp_min_mb = __slp_min_mb__;
            if(__Timeframes__b__) ____result.Timeframes = __Timeframes__;

            return ____result;
        }
    }


    public sealed class ForecastRootobjectFormatter : global::Utf8Json.IJsonFormatter<global::SimpleWeather.WeatherUnlocked.ForecastRootobject>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public ForecastRootobjectFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("Days"), 0},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("Days"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SimpleWeather.WeatherUnlocked.ForecastRootobject value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherUnlocked.Day[]>().Serialize(ref writer, value.Days, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SimpleWeather.WeatherUnlocked.ForecastRootobject Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __Days__ = default(global::SimpleWeather.WeatherUnlocked.Day[]);
            var __Days__b__ = false;

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
                        __Days__ = formatterResolver.GetFormatterWithVerify<global::SimpleWeather.WeatherUnlocked.Day[]>().Deserialize(ref reader, formatterResolver);
                        __Days__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SimpleWeather.WeatherUnlocked.ForecastRootobject();
            if(__Days__b__) ____result.Days = __Days__;

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
