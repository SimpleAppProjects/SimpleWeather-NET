using CommunityToolkit.Mvvm.DependencyInjection;
#if WINDOWS
using Microsoft.UI;
using Windows.UI;
#endif
using SimpleWeather.NET.Controls.Graphs;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.NET.Controls
{
    public enum GraphType
    {
        Line, Bar
    }

    public class ForecastGraphViewModel
    {
        public IGraphData GraphData { get; set; }

        public ForecastType ForecastType { get; set; }

        public GraphType GraphType { get; set; }

        public bool IsEmpty
        {
            get
            {
                return GraphData?.IsEmpty ?? true;
            }
        }

        private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

        public void AddForecastData<T>(T forecast, ForecastType forecastType, GraphType graphType) where T : BaseForecast
        {
            if (graphType == GraphType.Bar)
            {
                if (GraphData == null)
                {
                    BarGraphDataSet dataSet = CreateDataSet([], forecastType);
                    AddEntryData(forecast, dataSet, forecastType);
                    this.GraphData = CreateGraphData(dataSet, forecastType);
                    this.GraphType = graphType;
                }
                else
                {
                    AddEntryData(forecast, (BarGraphDataSet)GraphData.GetDataSetByIndex(0), forecastType);
                }
            }
            else
            {
                if (GraphData == null)
                {
                    LineDataSeries series = CreateSeriesData([], forecastType);
                    AddEntryData(forecast, series, forecastType);
                    this.GraphData = CreateGraphData([series], forecastType);
                    this.GraphType = graphType;
                }
                else
                {
                    AddEntryData(forecast, (LineDataSeries)GraphData.GetDataSetByIndex(0), forecastType);
                }
            }

            // Re-calc min/max
            this.GraphData.NotifyDataChanged();
        }

        public void SetForecastData<T>(IList<T> forecasts, ForecastType forecastType, GraphType graphType) where T : BaseForecast
        {
            if (graphType == GraphType.Bar)
            {
                BarGraphDataSet dataSet = CreateDataSet(new List<BarGraphEntry>(forecasts.Count), forecastType);

                foreach (var forecast in forecasts)
                {
                    AddEntryData(forecast, dataSet, forecastType);
                }

                this.GraphData = CreateGraphData(dataSet, forecastType);
            }
            else
            {
                LineDataSeries series = CreateSeriesData(new List<LineGraphEntry>(forecasts.Count), forecastType);

                foreach (var forecast in forecasts)
                {
                    AddEntryData(forecast, series, forecastType);
                }

                this.GraphData = CreateGraphData([series], forecastType);
            }

            this.ForecastType = forecastType;
            this.GraphType = graphType;
            UpdateDataSetMinMax();
        }

        public void SetMinutelyForecastData(IEnumerable<MinutelyForecast> forecasts, GraphType graphType)
        {
            if (graphType == GraphType.Bar)
            {
                BarGraphDataSet dataSet = CreateDataSet(new List<BarGraphEntry>(forecasts.Count()), ForecastType.Minutely);

                foreach (var forecast in forecasts)
                {
                    AddMinutelyEntryData(forecast, dataSet);
                }

                this.GraphData = CreateGraphData(dataSet, ForecastType.Minutely);
            }
            else
            {
                LineDataSeries series = CreateSeriesData(new List<LineGraphEntry>(forecasts.Count()), ForecastType.Minutely);

                foreach (var forecast in forecasts)
                {
                    AddMinutelyEntryData(forecast, series);
                }

                this.GraphData = CreateGraphData([series], ForecastType.Minutely);
            }

            this.ForecastType = ForecastType.Minutely;
            this.GraphType = graphType;
            UpdateDataSetMinMax();
        }

        private void AddEntryData<T>(T forecast, LineDataSeries series, ForecastType forecastType) where T : BaseForecast
        {
            var isFahrenheit = Units.FAHRENHEIT.Equals(SettingsManager.TemperatureUnit);
            var culture = LocaleUtils.GetLocale();

            string date = GetDateFromForecast(forecast);

            switch (forecastType)
            {
                case ForecastType.Temperature:
                    if (forecast.high_f.HasValue && forecast.high_c.HasValue)
                    {
                        int value = (int)(isFahrenheit ? Math.Round(forecast.high_f.Value) : Math.Round(forecast.high_c.Value));
                        var hiTemp = string.Format(culture, "{0}°", value);
                        series.AddEntry(new LineGraphEntry(date, new YEntryData(value, hiTemp)));
                    }
                    break;
                default:
                case ForecastType.Precipitation:
                    if (forecast.extras?.pop.HasValue == true && forecast.extras.pop >= 0)
                    {
                        series.AddEntry(new LineGraphEntry(date, new YEntryData(forecast.extras.pop.Value, forecast.extras.pop.Value + "%")));
                    }
                    else
                    {
                        series.AddEntry(new LineGraphEntry(date, new YEntryData(0f, "0%")));
                    }
                    break;
                case ForecastType.Wind:
                    if (forecast.extras?.wind_mph.HasValue == true && forecast.extras.wind_mph >= 0)
                    {
                        string unit = SettingsManager.SpeedUnit;
                        int speedVal;

                        switch (unit)
                        {
                            case Units.MILES_PER_HOUR:
                            default:
                                speedVal = (int)Math.Round(forecast.extras.wind_mph.Value);
                                break;
                            case Units.KILOMETERS_PER_HOUR:
                                speedVal = (int)Math.Round(forecast.extras.wind_kph.Value);
                                break;
                            case Units.METERS_PER_SECOND:
                                speedVal = (int)Math.Round(ConversionMethods.KphToMSec(forecast.extras.wind_kph.Value));
                                break;
                            case Units.KNOTS:
                                speedVal = (int)Math.Round(ConversionMethods.MphToKts(forecast.extras.wind_mph.Value));
                                break;
                        }

                        var windSpeed = string.Format(culture, "{0}", speedVal);

                        series.AddEntry(new LineGraphEntry(date, new YEntryData(speedVal, windSpeed)));
                    }
                    break;
                case ForecastType.Rain:
                    if (forecast.extras?.qpf_rain_in.HasValue == true && forecast.extras?.qpf_rain_mm.HasValue == true)
                    {
                        string unit = SettingsManager.PrecipitationUnit;
                        float precipValue;

                        switch (unit)
                        {
                            case Units.INCHES:
                            default:
                                precipValue = forecast.extras.qpf_rain_in.Value;
                                break;
                            case Units.MILLIMETERS:
                                precipValue = forecast.extras.qpf_rain_mm.Value;
                                break;
                        }

                        series.AddEntry(new LineGraphEntry(date, new YEntryData(precipValue, String.Format(culture, "{0:0.##}", precipValue))));
                    }
                    break;
                case ForecastType.Snow:
                    if (forecast.extras?.qpf_snow_in.HasValue == true && forecast.extras?.qpf_snow_cm.HasValue == true)
                    {
                        string unit = SettingsManager.PrecipitationUnit;
                        float precipValue;

                        switch (unit)
                        {
                            case Units.INCHES:
                            default:
                                precipValue = forecast.extras.qpf_snow_in.Value;
                                break;
                            case Units.MILLIMETERS:
                                precipValue = forecast.extras.qpf_snow_cm.Value * 10;
                                break;
                        }

                        series.AddEntry(new LineGraphEntry(date, new YEntryData(precipValue, String.Format(culture, "{0:0.##}", precipValue))));
                    }
                    break;
                case ForecastType.UVIndex:
                    if (forecast.extras?.uv_index.HasValue == true)
                    {
                        series.AddEntry(new LineGraphEntry(date, new YEntryData(forecast.extras.uv_index.Value, String.Format(culture, "{0:0.#}", forecast.extras.uv_index.Value))));
                    }
                    break;
                case ForecastType.Humidity:
                    if (forecast.extras?.humidity.HasValue == true)
                    {
                        series.AddEntry(new LineGraphEntry(date, new YEntryData(forecast.extras.humidity.Value, String.Format(culture, "{0}%", forecast.extras.humidity.Value))));
                    }
                    break;
            }
        }

        private void AddMinutelyEntryData(MinutelyForecast forecast, LineDataSeries series)
        {
            if (forecast.rain_mm.HasValue && forecast.rain_mm >= 0)
            {
                var culture = LocaleUtils.GetLocale();

                string date;
                if (culture.DateTimeFormat.ShortTimePattern.Contains("H"))
                {
                    date = forecast.date.ToString("HH:mm", culture);
                }
                else
                {
                    date = forecast.date.ToString("h:mm tt", culture);
                }

                string unit = SettingsManager.PrecipitationUnit;
                float precipValue;

                switch (unit)
                {
                    case Units.INCHES:
                    default:
                        precipValue = ConversionMethods.MMToIn(forecast.rain_mm.Value);
                        break;
                    case Units.MILLIMETERS:
                        precipValue = forecast.rain_mm.Value;
                        break;
                }

                series.AddEntry(new LineGraphEntry(date, new YEntryData(precipValue, String.Format(culture, "{0:0.##}", precipValue))));
            }
        }

        private LineDataSeries CreateSeriesData(IList<LineGraphEntry> entryData, ForecastType forecastType)
        {
            LineDataSeries series;

            switch (forecastType)
            {
                case ForecastType.Temperature:
                    series = new LineDataSeries(entryData);
                    series.SetSeriesColors(Colors.OrangeRed);
                    break;
                default:
                case ForecastType.Precipitation:
                    series = new LineDataSeries(entryData);
#if WINDOWS
                    series.SetSeriesColors(Color.FromArgb(0xFF, 0, 0x70, 0xC0));
#else
                    series.SetSeriesColors(Color.FromRgba(0, 0x70, 0xC0, 0xFF));
#endif
                    series.SetSeriesMinMax(0f, 100f);
                    break;
                case ForecastType.Wind:
                    series = new LineDataSeries(entryData);
                    series.SetSeriesColors(Colors.SeaGreen);
                    break;
                case ForecastType.Minutely:
                case ForecastType.Rain:
                    series = new LineDataSeries(entryData);
                    series.SetSeriesColors(Colors.DeepSkyBlue);
                    break;
                case ForecastType.Snow:
                    series = new LineDataSeries(entryData);
                    series.SetSeriesColors(Colors.SkyBlue);
                    break;
                case ForecastType.UVIndex:
                    series = new LineDataSeries(entryData);
                    series.SetSeriesColors(Colors.Orange);
                    series.SetSeriesMinMax(0f, 12f);
                    break;
                case ForecastType.Humidity:
                    series = new LineDataSeries(entryData);
                    series.SetSeriesColors(Colors.MediumPurple);
                    series.SetSeriesMinMax(0f, 100f);
                    break;
            }

            switch (forecastType)
            {
                case ForecastType.Temperature:
                case ForecastType.Precipitation:
                case ForecastType.Humidity:
                case ForecastType.UVIndex:
                    break;
                case ForecastType.Wind:
                    {
                        var unit = SettingsManager.SpeedUnit;
                        series.SeriesLabel = Units.GetUnitString(unit);
                    }
                    break;
                case ForecastType.Rain:
                case ForecastType.Minutely:
                case ForecastType.Snow:
                    {
                        var unit = SettingsManager.PrecipitationUnit;
                        series.SeriesLabel = Units.GetUnitString(unit);
                    }
                    break;
            }

            return series;
        }

        private LineViewData CreateGraphData(IList<LineDataSeries> seriesData, ForecastType forecastType)
        {
            string graphLabel = GetLabelForGraphType(forecastType);
            this.ForecastType = forecastType;

            return new LineViewData(graphLabel, seriesData);
        }

        private BarGraphDataSet CreateDataSet(IList<BarGraphEntry> entryData, ForecastType forecastType)
        {
            BarGraphDataSet dataSet = new(entryData);

            switch (forecastType)
            {
                case ForecastType.Precipitation:
                case ForecastType.Humidity:
                    dataSet.SetMinMax(0, 100);
                    break;
                case ForecastType.UVIndex:
                    dataSet.SetMinMax(0, 12);
                    break;
                case ForecastType.Temperature:
                    break;
                case ForecastType.Wind:
                    {
                        var unit = SettingsManager.SpeedUnit;
                        // max: 75mph
                        float? maxValue = unit switch
                        {
                            Units.MILES_PER_HOUR => 75f,
                            Units.KILOMETERS_PER_HOUR => 121f,
                            Units.METERS_PER_SECOND => 34f,
                            Units.KNOTS => 65f,
                            _ => null,
                        };

                        dataSet.SetMinMax(0f, maxValue);
                    }
                    break;
                case ForecastType.Snow:
                case ForecastType.Minutely:
                case ForecastType.Rain:
                    dataSet.SetMinMax(0f);
                    break;
            }

            switch (forecastType)
            {
                case ForecastType.Temperature:
                case ForecastType.Precipitation:
                case ForecastType.Humidity:
                case ForecastType.UVIndex:
                    break;
                case ForecastType.Wind:
                    {
                        var unit = SettingsManager.SpeedUnit;
                        dataSet.Label = Units.GetUnitString(unit);
                    }
                    break;
                case ForecastType.Rain:
                case ForecastType.Snow:
                case ForecastType.Minutely:
                    {
                        var unit = SettingsManager.PrecipitationUnit;
                        dataSet.Label = Units.GetUnitString(unit);
                    }
                    break;
            }

            return dataSet;
        }

        private BarGraphData CreateGraphData(BarGraphDataSet dataSet, ForecastType forecastType)
        {
            string graphLabel = GetLabelForGraphType(forecastType);
            this.ForecastType = forecastType;

            return new BarGraphData(graphLabel, dataSet);
        }

        private void AddEntryData<T>(T forecast, BarGraphDataSet dataSet, ForecastType forecastType) where T : BaseForecast
        {
            var isFahrenheit = Units.FAHRENHEIT.Equals(SettingsManager.TemperatureUnit);
            var culture = LocaleUtils.GetLocale();

            string date = GetDateFromForecast(forecast);

            switch (forecastType)
            {
                case ForecastType.Temperature:
                    if (forecast.high_f.HasValue && forecast.high_c.HasValue)
                    {
                        int value = (int)(isFahrenheit ? Math.Round(forecast.high_f.Value) : Math.Round(forecast.high_c.Value));
                        var hiTemp = string.Format(culture, "{0}°", value);

                        var entry = new BarGraphEntry(date, new YEntryData(value, hiTemp));
                        entry.FillColor = WeatherUtils.GetColorFromTempF(forecast.high_f.Value);
                        dataSet.AddEntry(entry);
                    }
                    break;
                default:
                case ForecastType.Precipitation:
                    {
                        BarGraphEntry entry;

                        if (forecast.extras?.pop.HasValue == true && forecast.extras.pop >= 0)
                        {
                            entry = new BarGraphEntry(date, new YEntryData(forecast.extras.pop.Value, forecast.extras.pop.Value + "%"));
                        }
                        else
                        {
                            entry = new BarGraphEntry(date, new YEntryData(0f, "0%"));
                        }

                        entry.FillColor = Colors.DeepSkyBlue;
                        dataSet.AddEntry(entry);
                        break;
                    }
                case ForecastType.Wind:
                    if (forecast.extras?.wind_mph.HasValue == true && forecast.extras.wind_mph >= 0)
                    {
                        string unit = SettingsManager.SpeedUnit;
                        int speedVal;

                        switch (unit)
                        {
                            case Units.MILES_PER_HOUR:
                            default:
                                speedVal = (int)Math.Round(forecast.extras.wind_mph.Value);
                                break;
                            case Units.KILOMETERS_PER_HOUR:
                                speedVal = (int)Math.Round(forecast.extras.wind_kph.Value);
                                break;
                            case Units.METERS_PER_SECOND:
                                speedVal = (int)Math.Round(ConversionMethods.KphToMSec(forecast.extras.wind_kph.Value));
                                break;
                            case Units.KNOTS:
                                speedVal = (int)Math.Round(ConversionMethods.MphToKts(forecast.extras.wind_mph.Value));
                                break;
                        }

                        var windSpeed = string.Format(culture, "{0}", speedVal);

                        var entry = new BarGraphEntry(date, new YEntryData(speedVal, windSpeed));
                        if (forecast.extras.wind_degrees.HasValue)
                        {
                            entry.XIconRotation = forecast.extras.wind_degrees.Value + 180;
                        }
                        entry.FillColor = Colors.SeaGreen;
                        dataSet.AddEntry(entry);
                    }
                    break;
                case ForecastType.Rain:
                    if (forecast.extras?.qpf_rain_in.HasValue == true && forecast.extras?.qpf_rain_mm.HasValue == true)
                    {
                        string unit = SettingsManager.PrecipitationUnit;
                        float precipValue;

                        switch (unit)
                        {
                            case Units.INCHES:
                            default:
                                precipValue = forecast.extras.qpf_rain_in.Value;
                                break;
                            case Units.MILLIMETERS:
                                precipValue = forecast.extras.qpf_rain_mm.Value;
                                break;
                        }

                        var entry = new BarGraphEntry(date, new YEntryData(precipValue, String.Format(culture, "{0:0.##}", precipValue)));
                        entry.FillColor = Colors.DeepSkyBlue;
                        dataSet.AddEntry(entry);
                    }
                    break;
                case ForecastType.Snow:
                    if (forecast.extras?.qpf_snow_in.HasValue == true && forecast.extras?.qpf_snow_cm.HasValue == true)
                    {
                        string unit = SettingsManager.PrecipitationUnit;
                        float precipValue;

                        switch (unit)
                        {
                            case Units.INCHES:
                            default:
                                precipValue = forecast.extras.qpf_snow_in.Value;
                                break;
                            case Units.MILLIMETERS:
                                precipValue = forecast.extras.qpf_snow_cm.Value * 10;
                                break;
                        }

                        var entry = new BarGraphEntry(date, new YEntryData(precipValue, String.Format(culture, "{0:0.##}", precipValue)));
                        entry.FillColor = Colors.SkyBlue;
                        dataSet.AddEntry(entry);
                    }
                    break;
                case ForecastType.UVIndex:
                    if (forecast.extras?.uv_index.HasValue == true)
                    {
                        var entry = new BarGraphEntry(date, new YEntryData(forecast.extras.uv_index.Value, String.Format(culture, "{0:0.#}", forecast.extras.uv_index.Value)));
                        entry.FillColor = WeatherUtils.GetColorFromUVIndex(forecast.extras.uv_index.Value);
                        dataSet.AddEntry(entry);
                    }
                    break;
                case ForecastType.Humidity:
                    if (forecast.extras?.humidity.HasValue == true)
                    {
                        var entry = new BarGraphEntry(date, new YEntryData(forecast.extras.humidity.Value, String.Format(culture, "{0}%", forecast.extras.humidity.Value)));
                        entry.FillColor = Colors.MediumPurple;
                        dataSet.AddEntry(entry);
                    }
                    break;
            }
        }

        private void AddMinutelyEntryData(MinutelyForecast forecast, BarGraphDataSet dataSet)
        {
            if (forecast.rain_mm.HasValue && forecast.rain_mm >= 0)
            {
                var culture = LocaleUtils.GetLocale();

                string date;
                if (culture.DateTimeFormat.ShortTimePattern.Contains("H"))
                {
                    date = forecast.date.ToString("HH:mm", culture);
                }
                else
                {
                    date = forecast.date.ToString("h:mm tt", culture);
                }

                string unit = SettingsManager.PrecipitationUnit;
                float precipValue;

                switch (unit)
                {
                    case Units.INCHES:
                    default:
                        precipValue = ConversionMethods.MMToIn(forecast.rain_mm.Value);
                        break;
                    case Units.MILLIMETERS:
                        precipValue = forecast.rain_mm.Value;
                        break;
                }

                var entry = new BarGraphEntry(date, new YEntryData(precipValue, String.Format(culture, "{0:0.##}", precipValue)));
                entry.FillColor = Colors.DeepSkyBlue;
                dataSet.AddEntry(entry);
            }
        }

        private static string GetDateFromForecast<T>(T forecast) where T : BaseForecast
        {
            string date;
            var culture = LocaleUtils.GetLocale();

            if (forecast is Forecast fcast)
            {
                date = fcast.date.ToString("ddd dd", culture);
            }
            else if (forecast is HourlyForecast hrfcast)
            {
                if (culture.DateTimeFormat.ShortTimePattern.Contains("H"))
                {
                    date = hrfcast.date.ToString("HH:00", culture);
                }
                else
                {
                    date = hrfcast.date.ToString("h tt", culture);
                }
            }
            else
            {
                date = string.Empty;
            }

            return date;
        }

        private static string GetLabelForGraphType(ForecastType forecastType)
        {
            return forecastType switch
            {
                ForecastType.Temperature => ResStrings.label_temperature,
                ForecastType.Minutely or ForecastType.Precipitation => ResStrings.label_precipitation,
                ForecastType.Wind => ResStrings.label_wind,
                ForecastType.Rain => ResStrings.label_qpf_rain,
                ForecastType.Snow => ResStrings.label_qpf_snow,
                ForecastType.UVIndex => ResStrings.label_uv,
                ForecastType.Humidity => ResStrings.label_humidity,
                ForecastType.AirQuality => ResStrings.label_airquality,
                _ => null,
            };
        }

        public void UpdateDataSetMinMax()
        {
            var graphData = this.GraphData;

            if (graphData is BarGraphData barGraphData)
            {
                foreach (var dataSet in barGraphData.DataSets)
                {
                    switch (ForecastType)
                    {
                        case ForecastType.Rain:
                        case ForecastType.Snow:
                            {
                                var unit = SettingsManager.PrecipitationUnit;

                                // Heavy rain — rate is >= 7.6 mm (0.30 in) per hr
                                // Snow will often accumulate at a rate of 0.5in (12.7mm) an hour
                                switch (unit)
                                {
                                    case Units.INCHES:
                                    default:
                                        if (ForecastType == ForecastType.Snow)
                                        {
                                            dataSet.SetMinMax(0f, Math.Max(dataSet.YMax, 0.5f));
                                        }
                                        else
                                        {
                                            dataSet.SetMinMax(0f, Math.Max(dataSet.YMax, 0.3f));
                                        }
                                        break;
                                    case Units.MILLIMETERS:
                                        if (ForecastType == ForecastType.Snow)
                                        {
                                            dataSet.SetMinMax(0f, Math.Max(dataSet.YMax, 12.7f));
                                        }
                                        else
                                        {
                                            dataSet.SetMinMax(0f, Math.Max(dataSet.YMax, 7.6f));
                                        }
                                        break;
                                }
                            }
                            break;
                        case ForecastType.Minutely:
                            {
                                // Heavy rain — rate is >= 7.6 mm (0.30 in) per hr
                                var unit = SettingsManager.PrecipitationUnit;
                                switch (unit)
                                {
                                    case Units.INCHES:
                                    default:
                                        dataSet.SetMinMax(0f, Math.Max(dataSet.YMax, 0.3f));
                                        break;
                                    case Units.MILLIMETERS:
                                        dataSet.SetMinMax(0f, Math.Max(dataSet.YMax, 7.6f));
                                        break;
                                }
                            }
                            break;
                    }
                }
            }
            else if (graphData is LineViewData lineViewData)
            {
                foreach (var series in lineViewData.DataSets)
                {
                    switch (ForecastType)
                    {
                        case ForecastType.Rain:
                        case ForecastType.Snow:
                            {
                                var unit = SettingsManager.PrecipitationUnit;
                                // Heavy rain — rate is >= 7.6 mm (0.30 in) per hr
                                // Snow will often accumulate at a rate of 0.5in (12.7mm) an hour
                                switch (unit)
                                {
                                    case Units.INCHES:
                                    default:
                                        if (ForecastType == ForecastType.Snow)
                                        {
                                            series.SetSeriesMinMax(0f, Math.Max(series.YMax, 0.5f));
                                        }
                                        else
                                        {
                                            series.SetSeriesMinMax(0f, Math.Max(series.YMax, 0.3f));
                                        }
                                        break;
                                    case Units.MILLIMETERS:
                                        if (ForecastType == ForecastType.Snow)
                                        {
                                            series.SetSeriesMinMax(0f, Math.Max(series.YMax, 12.7f));
                                        }
                                        else
                                        {
                                            series.SetSeriesMinMax(0f, Math.Max(series.YMax, 7.6f));
                                        }
                                        break;
                                }
                            }
                            break;
                        case ForecastType.Minutely:
                            {
                                // Heavy rain — rate is >= 7.6 mm (0.30 in) per hr
                                var unit = SettingsManager.PrecipitationUnit;
                                switch (unit)
                                {
                                    case Units.INCHES:
                                    default:
                                        series.SetSeriesMinMax(0f, Math.Max(series.YMax, 0.3f));
                                        break;
                                    case Units.MILLIMETERS:
                                        series.SetSeriesMinMax(0f, Math.Max(series.YMax, 7.6f));
                                        break;
                                }
                            }
                            break;
                    }
                }
            }

            graphData?.NotifyDataChanged();
        }
    }
}