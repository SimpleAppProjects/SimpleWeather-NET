using SimpleWeather.Json;
using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace SimpleWeather.Weather_API.WeatherKit
{
    /// <summary>
    /// The collection of all requested weather data.
    /// </summary>
    public class Weather
    {
        /// <summary>
        /// The current weather for the requested location.
        /// </summary>
        public CurrentWeather currentWeather { get; set; }
        /// <summary>
        /// The daily forecast for the requested location.
        /// </summary>
        public DailyForecast forecastDaily { get; set; }
        /// <summary>
        /// The hourly forecast for the requested location.
        /// </summary>
        public HourlyForecast forecastHourly { get; set; }
        /// <summary>
        /// The next hour forecast for the requested location.
        /// </summary>
        public NextHourForecast forecastNextHour { get; set; }
        /// <summary>
        /// Weather alerts for the requested location.
        /// </summary>
        public WeatherAlertCollection weatherAlerts { get; set; }
    }

    public class ProductData
    {
        public string name { get; set; }
        /// <summary>
        /// (Required) Descriptive information about the weather data.
        /// </summary>
        public Metadata metadata { get; set; }
    }

    public class CurrentWeather : ProductData
    {
        /// <summary>
        /// (Required) The date and time.
        /// </summary>
        public DateTimeOffset asOf { get; set; }
        /// <summary>
        /// The percentage of the sky covered with clouds during the period, from 0 to 1.
        /// </summary>
        public float? cloudCover { get; set; }
        public float? cloudCoverLowAltPct { get; set; }
        public float? cloudCoverMidAltPct { get; set; }
        public float? cloudCoverHighAltPct { get; set; }
        /// <summary>
        /// (Required) An enumeration value indicating the condition at the time.
        /// </summary>
        public string conditionCode { get; set; }
        /// <summary>
        /// A Boolean value indicating whether there is daylight.
        /// </summary>
        public bool? daylight { get; set; }
        /// <summary>
        /// (Required) The relative humidity, from 0 to 1.
        /// </summary>
        public float humidity { get; set; }
        /// <summary>
        /// (Required) The precipitation intensity, in millimeters per hour.
        /// </summary>
        public float precipitationIntensity { get; set; }
        /// <summary>
        /// (Required) The sea level air pressure, in millibars.
        /// </summary>
        public float pressure { get; set; }
        /// <summary>
        /// (Required) The direction of change of the sea-level air pressure.
        /// </summary>
        public PressureTrend pressureTrend { get; set; }
        /// <summary>
        /// (Required) The current temperature, in degrees Celsius.
        /// </summary>
        public float temperature { get; set; }
        /// <summary>
        /// (Required) The feels-like temperature when factoring wind and humidity, in degrees Celsius.
        /// </summary>
        public float temperatureApparent { get; set; }
        /// <summary>
        /// (Required) The temperature at which relative humidity is 100%, in Celsius.
        /// </summary>
        public float temperatureDewPoint { get; set; }
        /// <summary>
        /// (Required) The level of ultraviolet radiation.
        /// </summary>
        public int uvIndex { get; set; }
        /// <summary>
        /// (Required) The distance at which terrain is visible, in meters.
        /// </summary>
        public float visibility { get; set; }
        /// <summary>
        /// The direction of the wind, in degrees.
        /// </summary>
        public int? windDirection { get; set; }
        /// <summary>
        /// The maximum wind gust speed, in kilometers per hour.
        /// </summary>
        public float? windGust { get; set; }
        /// <summary>
        /// (Required) The wind speed, in kilometers per hour.
        /// </summary>
        public float windSpeed { get; set; }
    }

    /// <summary>
    /// Descriptive information about the weather data.
    /// </summary>
    public class Metadata
    {
        /// <summary>
        /// The URL of the legal attribution for the data source.
        /// </summary>
        public string attributionURL { get; set; }
        /// <summary>
        /// (Required) The time when the weather data is no longer valid.
        /// </summary>
        public DateTimeOffset expireTime { get; set; }
        /// <summary>
        /// The ISO language code for localizable fields.
        /// </summary>
        public string language { get; set; }
        /// <summary>
        /// (Required) The latitude of the relevant location.
        /// </summary>
        public float latitude { get; set; }
        /// <summary>
        /// (Required) The longitude of the relevant location.
        /// </summary>
        public float longitude { get; set; }
        /// <summary>
        /// The URL of a logo for the data provider.
        /// </summary>
        public string providerLogo { get; set; }
        /// <summary>
        /// The name of the data provider.
        /// </summary>
        public string providerName { get; set; }
        /// <summary>
        /// (Required) The time the weather data was procured.
        /// </summary>
        public DateTimeOffset readTime { get; set; }
        /// <summary>
        /// The time the provider reported the weather data.
        /// </summary>
        public DateTimeOffset? reportedTime { get; set; }
        /// <summary>
        /// The weather data is temporarily unavailable from the provider.
        /// </summary>
        public bool? temporarilyUnavailable { get; set; }
        /// <summary>
        /// The system of units that the weather data is reported in. This is set to metric.
        /// </summary>
        public UnitsSystem? units { get; set; }
        /// <summary>
        /// (Required) The data format version.
        /// </summary>
        public int version { get; set; }
    }

    /// <summary>
    /// A collection of day forecasts for a specified range of days.
    /// </summary>
    public class DailyForecast : ProductData
    {
        /// <summary>
        /// (Required) An array of the day forecast weather conditions.
        /// </summary>
        public DayWeatherConditions[] days { get; set; }
        /// <summary>
        /// A URL that provides more information about the forecast.
        /// </summary>
        public string learnMoreURL { get; set; }
    }

    /// <summary>
    /// The historical or forecasted weather conditions for a specified day.
    /// </summary>
    public class DayWeatherConditions
    {
        /// <summary>
        /// (Required) An enumeration value indicating the condition at the time.
        /// </summary>
        public string conditionCode { get; set; }
        /// <summary>
        /// The forecast between 7 AM and 7 PM for the day.
        /// </summary>
        public DayPartForecast daytimeForecast { get; set; }
        /// <summary>
        /// (Required) The ending date and time of the day.
        /// </summary>
        public DateTimeOffset forecastEnd { get; set; }
        /// <summary>
        /// (Required) The starting date and time of the day.
        /// </summary>
        public DateTimeOffset forecastStart { get; set; }
        /// <summary>
        /// (Required) The maximum ultraviolet index value during the day.
        /// </summary>
        public int maxUvIndex { get; set; }
        /// <summary>
        /// (Required) The phase of the moon on the specified day.
        /// </summary>
        public MoonPhase moonPhase { get; set; }
        /// <summary>
        /// The time of moonrise on the specified day.
        /// </summary>
        public DateTimeOffset? moonrise { get; set; }
        /// <summary>
        /// The time of moonset on the specified day.
        /// </summary>
        public DateTimeOffset? moonset { get; set; }
        /// <summary>
        /// The day part forecast between 7 PM and 7 AM for the overnight.
        /// </summary>
        public DayPartForecast overnightForecast { get; set; }
        /// <summary>
        /// (Required) The amount of precipitation forecasted to occur during the day, in millimeters.
        /// </summary>
        public float precipitationAmount { get; set; }
        /// <summary>
        /// (Required) The chance of precipitation forecasted to occur during the day.
        /// </summary>
        public float precipitationChance { get; set; }
        /// <summary>
        /// (Required) The type of precipitation forecasted to occur during the day.
        /// </summary>
        public PrecipitationType precipitationType { get; set; }
        //public object precipitationAmountByType { get; set; }
        /// <summary>
        /// (Required) The depth of snow as ice crystals forecasted to occur during the day, in millimeters.
        /// </summary>
        public float snowfallAmount { get; set; }
        /// <summary>
        /// The time when the sun is lowest in the sky.
        /// </summary>
        public DateTimeOffset? solarMidnight { get; set; }
        /// <summary>
        /// The time when the sun is highest in the sky.
        /// </summary>
        public DateTimeOffset? solarNoon { get; set; }
        /// <summary>
        /// The time when the top edge of the sun reaches the horizon in the morning.
        /// </summary>
        public DateTimeOffset? sunrise { get; set; }
        /// <summary>
        /// The time when the sun is 18 degrees below the horizon in the morning.
        /// </summary>
        public DateTimeOffset? sunriseAstronomical { get; set; }
        /// <summary>
        /// The time when the sun is 6 degrees below the horizon in the morning.
        /// </summary>
        public DateTimeOffset? sunriseCivil { get; set; }
        /// <summary>
        /// The time when the sun is 12 degrees below the horizon in the morning.
        /// </summary>
        public DateTimeOffset? sunriseNautical { get; set; }
        /// <summary>
        /// The time when the top edge of the sun reaches the horizon in the evening.
        /// </summary>
        public DateTimeOffset? sunset { get; set; }
        /// <summary>
        /// The time when the sun is 18 degrees below the horizon in the evening.
        /// </summary>
        public DateTimeOffset? sunsetAstronomical { get; set; }
        /// <summary>
        /// The time when the sun is 6 degrees below the horizon in the evening.
        /// </summary>
        public DateTimeOffset? sunsetCivil { get; set; }
        /// <summary>
        /// The time when the sun is 12 degrees below the horizon in the evening.
        /// </summary>
        public DateTimeOffset? sunsetNautical { get; set; }
        /// <summary>
        /// (Required) The maximum temperature forecasted to occur during the day, in degrees Celsius.
        /// </summary>
        public float temperatureMax { get; set; }
        /// <summary>
        /// (Required) The minimum temperature forecasted to occur during the day, in degrees Celsius.
        /// </summary>
        public float temperatureMin { get; set; }
        //public Restofdayforecast restOfDayForecast { get; set; }
    }

    /// <summary>
    /// A summary forecast for a daytime or overnight period.
    /// </summary>
    public class DayPartForecast
    {
        /// <summary>
        /// (Required) The percentage of the sky covered with clouds during the period, from 0 to 1.
        /// </summary>
        public float cloudCover { get; set; }
        /// <summary>
        /// (Required) An enumeration value indicating the condition at the time.
        /// </summary>
        public string conditionCode { get; set; }
        /// <summary>
        /// (Required) The ending date and time of the forecast.
        /// </summary>
        public DateTimeOffset forecastEnd { get; set; }
        /// <summary>
        /// (Required) The starting date and time of the forecast.
        /// </summary>
        public DateTimeOffset forecastStart { get; set; }
        /// <summary>
        /// (Required) The relative humidity during the period, from 0 to 1.
        /// </summary>
        public float humidity { get; set; }
        /// <summary>
        /// (Required) The amount of precipitation forecasted to occur during the period, in millimeters.
        /// </summary>
        public float precipitationAmount { get; set; }
        //public object precipitationAmountByType { get; set; }
        /// <summary>
        /// (Required) The chance of precipitation forecasted to occur during the period.
        /// </summary>
        public float precipitationChance { get; set; }
        /// <summary>
        /// (Required) The type of precipitation forecasted to occur during the period.
        /// </summary>
        public PrecipitationType precipitationType { get; set; }
        /// <summary>
        /// (Required) The depth of snow as ice crystals forecasted to occur during the period, in millimeters.
        /// </summary>
        public float snowfallAmount { get; set; }
        /// <summary>
        /// The direction the wind is forecasted to come from during the period, in degrees.
        /// </summary>
        public int? windDirection { get; set; }
        /// <summary>
        /// (Required) The average speed the wind is forecasted to be during the period, in kilometers per hour.
        /// </summary>
        public float windSpeed { get; set; }
    }

    /// <summary>
    /// The hourly forecast information.
    /// </summary>
    public class HourlyForecast : ProductData
    {
        /// <summary>
        /// (Required) An array of hourly forecasts.
        /// </summary>
        public HourWeatherConditions[] hours { get; set; }
    }

    /// <summary>
    /// The historical or forecasted weather conditions for a specified hour.
    /// </summary>
    public class HourWeatherConditions
    {
        /// <summary>
        /// (Required) The percentage of the sky covered with clouds during the period, from 0 to 1.
        /// </summary>
        public float cloudCover { get; set; }
        /// <summary>
        /// (Required) An enumeration value indicating the condition at the time.
        /// </summary>
        public string conditionCode { get; set; }
        /// <summary>
        /// Indicates whether the hour starts during the day or night.
        /// </summary>
        public bool? daylight { get; set; }
        /// <summary>
        /// (Required) The starting date and time of the forecast.
        /// </summary>
        public DateTimeOffset forecastStart { get; set; }
        /// <summary>
        /// (Required) The relative humidity at the start of the hour, from 0 to 1.
        /// </summary>
        public float humidity { get; set; }
        /// <summary>
        /// (Required) The chance of precipitation forecasted to occur during the hour, from 0 to 1.
        /// </summary>
        public float precipitationChance { get; set; }
        /// <summary>
        /// (Required) The type of precipitation forecasted to occur during the period.
        /// </summary>
        public PrecipitationType precipitationType { get; set; }
        /// <summary>
        /// (Required) The sea-level air pressure, in millibars.
        /// </summary>
        public float pressure { get; set; }
        /// <summary>
        /// The direction of change of the sea-level air pressure.
        /// </summary>
        public PressureTrend? pressureTrend { get; set; }
        /// <summary>
        /// The rate at which snow crystals are falling, in millimeters per hour.
        /// </summary>
        public float? snowfallIntensity { get; set; }
        //public float? snowfallAmount { get; set; }
        /// <summary>
        /// (Required) The temperature at the start of the hour, in degrees Celsius.
        /// </summary>
        public float temperature { get; set; }
        /// <summary>
        /// (Required) The feels-like temperature when considering wind and humidity, at the start of the hour, in degrees Celsius.
        /// </summary>
        public float temperatureApparent { get; set; }
        /// <summary>
        /// The temperature at which relative humidity is 100% at the top of the hour, in degrees Celsius.
        /// </summary>
        public float? temperatureDewPoint { get; set; }
        /// <summary>
        /// (Required) The level of ultraviolet radiation at the start of the hour.
        /// </summary>
        public int uvIndex { get; set; }
        /// <summary>
        /// (Required) The distance at which terrain is visible at the start of the hour, in meters.
        /// </summary>
        public float visibility { get; set; }
        /// <summary>
        /// The direction of the wind at the start of the hour, in degrees.
        /// </summary>
        public int? windDirection { get; set; }
        /// <summary>
        /// The maximum wind gust speed during the hour, in kilometers per hour.
        /// </summary>
        public float? windGust { get; set; }
        /// <summary>
        /// (Required) The wind speed at the start of the hour, in kilometers per hour.
        /// </summary>
        public float windSpeed { get; set; }
        /// <summary>
        /// The amount of precipitation forecasted to occur during period, in millimeters.
        /// </summary>
        public float? precipitationAmount { get; set; }
        //public float? precipitationIntensity { get; set; }
    }

    /// <summary>
    /// The next hour forecast information.
    /// </summary>
    public class NextHourForecast : ProductData
    {
        /// <summary>
        /// The time the forecast ends.
        /// </summary>
        public DateTimeOffset? forecastEnd { get; set; }
        /// <summary>
        /// The time the forecast starts.
        /// </summary>
        public DateTimeOffset? forecastStart { get; set; }
        /// <summary>
        /// (Required) An array of the forecast minutes.
        /// </summary>
        public ForecastMinute[] minutes { get; set; }
        /// <summary>
        /// (Required) An array of the forecast summaries.
        /// </summary>
        public ForecastPeriodSummary[] summary { get; set; }
    }

    /// <summary>
    /// The precipitation forecast for a specified minute.
    /// </summary>
    public class ForecastMinute
    {
        /// <summary>
        /// (Required) The probability of precipitation during this minute.
        /// </summary>
        public float precipitationChance { get; set; }
        /// <summary>
        /// (Required) The precipitation intensity in millimeters per hour.
        /// </summary>
        public float precipitationIntensity { get; set; }
        /// <summary>
        /// (Required) The start time of the minute.
        /// </summary>
        public DateTimeOffset startTime { get; set; }
    }

    /// <summary>
    /// The summary for a specified period in the minute forecast.
    /// </summary>
    public class ForecastPeriodSummary
    {
        /// <summary>
        /// (Required) The type of precipitation forecasted.
        /// </summary>
        public PrecipitationType condition { get; set; }
        /// <summary>
        /// The end time of the forecast.
        /// </summary>
        public DateTimeOffset? endTime { get; set; }
        /// <summary>
        /// (Required) The probability of precipitation during this period.
        /// </summary>
        public float precipitationChance { get; set; }
        /// <summary>
        /// (Required) The precipitation intensity in millimeters per hour.
        /// </summary>
        public float precipitationIntensity { get; set; }
        /// <summary>
        /// (Required) The start time of the forecast.
        /// </summary>
        public DateTimeOffset startTime { get; set; }
    }

    /// <summary>
    /// A collection of severe weather alerts for a specified location.
    /// </summary>
    public class WeatherAlertCollection : ProductData
    {
        /// <summary>
        /// (Required) An array of weather alert summaries.
        /// </summary>
        public WeatherAlertSummary[] alerts { get; set; }
        /// <summary>
        /// A URL that provides more information about the alerts.
        /// </summary>
        public string detailsUrl { get; set; }
    }

    public class WeatherAlertSummary
    {
        /// <summary>
        /// An official designation of the affected area.
        /// </summary>
        public string areaId { get; set; }
        /// <summary>
        /// A human-readable name of the affected area.
        /// </summary>
        public string areaName { get; set; }
        /// <summary>
        /// (Required) How likely the event is to occur.
        /// </summary>
        public Certainty certainty { get; set; }
        /// <summary>
        /// (Required) The ISO code of the reporting country.
        /// </summary>
        public string countryCode { get; set; }
        /// <summary>
        /// (Required) A human-readable description of the event.
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// The URL to a page containing detailed information about the event.
        /// </summary>
        public string detailsUrl { get; set; }
        /// <summary>
        /// (Required) The time the event went into effect.
        /// </summary>
        public DateTimeOffset effectiveTime { get; set; }
        /// <summary>
        /// The time when the underlying weather event is projected to end.
        /// </summary>
        public DateTimeOffset? eventEndTime { get; set; }
        /// <summary>
        /// The time when the underlying weather event is projected to start.
        /// </summary>
        public DateTimeOffset? eventOnsetTime { get; set; }
        /// <summary>
        /// (Required) The time when the event expires.
        /// </summary>
        public DateTimeOffset expireTime { get; set; }
        /// <summary>
        /// (Required) A unique identifier of the event.
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// (Required) The time that event was issued by the reporting agency.
        /// </summary>
        public DateTimeOffset issuedTime { get; set; }
        /// <summary>
        /// (Required) An array of recommended actions from the reporting agency.
        /// </summary>
        public ResponseType[] responses { get; set; }
        /// <summary>
        /// (Required) The level of danger to life and property.
        /// </summary>
        public Severity severity { get; set; }
        /// <summary>
        /// (Required) The name of the reporting agency.
        /// </summary>
        public string source { get; set; }
        /// <summary>
        /// An indication of urgency of action from the reporting agency.
        /// </summary>
        public Urgency urgency { get; set; }
    }

    /// <summary>
    /// How likely the event is to occur.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter<Certainty>))]
    [DataContract]
    public enum Certainty
    {
        /// <summary>
        /// The event has already occurred or is ongoing.
        /// </summary>
        [EnumMember(Value = "observed")]
        Observed,
        /// <summary>
        /// The event is likely to occur (greater than 50% probability).
        /// </summary>
        [EnumMember(Value = "likely")]
        Likely,
        /// <summary>
        /// The event is unlikley to occur (less than 50% probability).
        /// </summary>
        [EnumMember(Value = "possible")]
        Possible,
        /// <summary>
        /// The event is not expected to occur (approximately 0% probability).
        /// </summary>
        [EnumMember(Value = "unlikely")]
        Unlikely,
        /// <summary>
        /// It is unknown if the event will occur.
        /// </summary>
        [EnumMember(Value = "unknown")]
        Unknown,
    }

    /// <summary>
    /// The recommended action from a reporting agency.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter<ResponseType>))]
    [DataContract]
    public enum ResponseType
    {
        /// <summary>
        /// Take shelter in place.
        /// </summary>
        [EnumMember(Value = "shelter")]
        Shelter,
        /// <summary>
        /// Relocate.
        /// </summary>
        [EnumMember(Value = "evacuate")]
        Evacuate,
        /// <summary>
        /// Make preparations.
        /// </summary>
        [EnumMember(Value = "prepare")]
        Prepare,
        /// <summary>
        /// Execute a pre-planned activity.
        /// </summary>
        [EnumMember(Value = "execute")]
        Execute,
        /// <summary>
        /// Avoid the event.
        /// </summary>
        [EnumMember(Value = "avoid")]
        Avoid,
        /// <summary>
        /// Monitor the situation.
        /// </summary>
        [EnumMember(Value = "monitor")]
        Monitor,
        /// <summary>
        /// Assess the situation.
        /// </summary>
        [EnumMember(Value = "assess")]
        Assess,
        /// <summary>
        /// The event no longer poses a threat.
        /// </summary>
        [EnumMember(Value = "allClear")]
        AllClear,
        /// <summary>
        /// No action recommended.
        /// </summary>
        [EnumMember(Value = "none")]
        None,
    }

    /// <summary>
    /// The level of danger to life and property.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter<Severity>))]
    [DataContract]
    public enum Severity
    {
        /// <summary>
        /// Extraordinary threat.
        /// </summary>
        [EnumMember(Value = "extreme")]
        Extreme,
        /// <summary>
        /// Significant threat.
        /// </summary>
        [EnumMember(Value = "severe")]
        Severe,
        /// <summary>
        /// Possible threat.
        /// </summary>
        [EnumMember(Value = "moderate")]
        Moderate,
        /// <summary>
        /// Minimal or no known threat.
        /// </summary>
        [EnumMember(Value = "minor")]
        Minor,
        /// <summary>
        /// Unknown threat.
        /// </summary>
        [EnumMember(Value = "unknown")]
        Unknown,
    }

    /// <summary>
    /// An indication of urgency of action from the reporting agency.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter<Urgency>))]
    [DataContract]
    public enum Urgency
    {
        /// <summary>
        /// Take responsive action immediately.
        /// </summary>
        [EnumMember(Value = "immediate")]
        Immediate,
        /// <summary>
        /// Take responsive action in the next hour.
        /// </summary>
        [EnumMember(Value = "expected")]
        Expected,
        /// <summary>
        /// Take responsive action in the near future.
        /// </summary>
        [EnumMember(Value = "future")]
        Future,
        /// <summary>
        /// Responsive action is no longer required.
        /// </summary>
        [EnumMember(Value = "past")]
        Minor,
        /// <summary>
        /// The urgency is unknown.
        /// </summary>
        [EnumMember(Value = "unknown")]
        Unknown,
    }

    /// <summary>
    /// The system of units that the weather data is reported in.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter<UnitsSystem>))]
    [DataContract]
    public enum UnitsSystem
    {
        /// <summary>
        /// The metric system.
        /// </summary>
        [EnumMember(Value = "m")]
        Metric
    }

    /// <summary>
    /// The shape of the moon as seen by an observer on the ground at a given time.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter<MoonPhase>))]
    [DataContract]
    public enum MoonPhase
    {
        /// <summary>
        /// The moon isn’t visible.
        /// </summary>
        [EnumMember(Value = "new")]
        New,
        /// <summary>
        /// A crescent-shaped sliver of the moon is visible, and increasing in size.
        /// </summary>
        [EnumMember(Value = "waxingCrescent")]
        WaxingCrescent,
        /// <summary>
        /// Approximately half of the moon is visible, and increasing in size.
        /// </summary>
        [EnumMember(Value = "firstQuarter")]
        FirstQuarter,
        /// <summary>
        /// The entire disc of the moon is visible.
        /// </summary>
        [EnumMember(Value = "full")]
        Full,
        /// <summary>
        /// More than half of the moon is visible, and increasing in size.
        /// </summary>
        [EnumMember(Value = "waxingGibbous")]
        WaxingGibbous,
        /// <summary>
        /// More than half of the moon is visible, and decreasing in size.
        /// </summary>
        [EnumMember(Value = "waningGibbous")]
        WaningGibbous,
        /// <summary>
        /// Approximately half of the moon is visible, and decreasing in size.
        /// </summary>
        [EnumMember(Value = "thirdQuarter")]
        ThirdQuarter,
        /// <summary>
        /// A crescent-shaped sliver of the moon is visible, and decreasing in size.
        /// </summary>
        [EnumMember(Value = "waningCrescent")]
        WaningCrescent,
    }

    /// <summary>
    /// The type of precipitation forecasted to occur during the day.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter<PrecipitationType>))]
    [DataContract]
    public enum PrecipitationType
    {
        /// <summary>
        /// No precipitation is occurring.
        /// </summary>
        [EnumMember(Value = "clear")]
        Clear,
        /// <summary>
        /// An unknown type of precipitation is occuring.
        /// </summary>
        [EnumMember(Value = "precipitation")]
        Precipitation,
        /// <summary>
        /// Rain or freezing rain is falling.
        /// </summary>
        [EnumMember(Value = "rain")]
        Rain,
        /// <summary>
        /// Snow is falling.
        /// </summary>
        [EnumMember(Value = "snow")]
        Snow,
        /// <summary>
        /// Sleet or ice pellets are falling.
        /// </summary>
        [EnumMember(Value = "sleet")]
        Sleet,
        /// <summary>
        /// Hail is falling.
        /// </summary>
        [EnumMember(Value = "hail")]
        Hail,
        /// <summary>
        /// Winter weather (wintery mix or wintery showers) is falling.
        /// </summary>
        [EnumMember(Value = "mixed")]
        Mixed,
    }

    /// <summary>
    /// The direction of change of the sea level air pressure.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter<PressureTrend>))]
    [DataContract]
    public enum PressureTrend
    {
        /// <summary>
        /// The sea level air pressure is increasing.
        /// </summary>
        [EnumMember(Value = "rising")]
        Rising,
        /// <summary>
        /// The sea level air pressure is decreasing.
        /// </summary>
        [EnumMember(Value = "falling")]
        Falling,
        /// <summary>
        /// The sea level air pressure is remaining about the same.
        /// </summary>
        [EnumMember(Value = "steady")]
        Steady
    }
}
