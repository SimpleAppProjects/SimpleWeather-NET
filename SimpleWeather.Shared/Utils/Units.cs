using Microsoft.Toolkit.Parsers.Core;

namespace SimpleWeather.Utils
{
    public static class Units
    {
        public enum TemperatureUnits
        {
            [StringValue(FAHRENHEIT)]
            Fahrenheit,
            [StringValue(CELSIUS)]
            Celsuis
        }

        public enum SpeedUnits
        {
            [StringValue(MILES_PER_HOUR)]
            MilesPerHour,
            [StringValue(KILOMETERS_PER_HOUR)]
            KilometersPerHour,
            [StringValue(METERS_PER_SECOND)]
            MetersPerSecond,
            [StringValue(KNOTS)]
            Knots
        }

        public enum PressureUnits
        {
            [StringValue(INHG)]
            InHg,
            [StringValue(MILLIBAR)]
            Millibar,
            [StringValue(MMHG)]
            MmHg
        }

        public enum DistanceUnits
        {
            [StringValue(MILES)]
            Miles,
            [StringValue(KILOMETERS)]
            Kilometers
        }

        public enum PrecipitationUnits
        {
            [StringValue(INCHES)]
            Inches,
            [StringValue(MILLIMETERS)]
            Millimeters
        }

        public const string FAHRENHEIT = "F";
        public const string CELSIUS = "C";
        public const string MILES_PER_HOUR = "MPH";
        public const string KILOMETERS_PER_HOUR = "KMPH";
        public const string METERS_PER_SECOND = "MSEC";
        public const string KNOTS = "KNOTS";
        public const string INHG = "INMERCURY";
        public const string MILLIBAR = "MILLIBAR";
        public const string MMHG = "MMMERCURY";
        public const string MILES = "MILES";
        public const string KILOMETERS = "KILOMETERS";
        public const string INCHES = "INCHES";
        public const string MILLIMETERS = "MILLIMETERS";
    }
}
