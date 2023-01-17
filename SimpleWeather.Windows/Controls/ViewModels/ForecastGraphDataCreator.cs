using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.UI;
using SimpleWeather.NET.Controls.Graphs;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Windows.UI;

namespace SimpleWeather.NET.Controls
{
    public enum ForecastGraphType
    {
        Temperature,
        Minutely,
        Precipitation,
        Wind,
        Humidity,
        UVIndex,
        Rain,
        Snow
    }

    public class ForecastGraphDataCreator
    {
        internal IGraphData GraphData { get; set; }

        public ForecastGraphType GraphType { get; set; }

        public bool IsEmpty
        {
            get
            {
                return GraphData?.IsEmpty ?? true;
            }
        }

        private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

        public void AddForecastData<T>(T forecast, ForecastGraphType graphType) where T : BaseForecast
        {
            if (graphType != ForecastGraphType.UVIndex)
            {
                if (GraphData == null)
                {
                    LineDataSeries series = CreateSeriesData(new List<LineGraphEntry>(), graphType);
                    AddEntryData(forecast, series, graphType);
                    this.GraphData = CreateGraphData(ImmutableList.Create(series), graphType);
                }
                else
                {
                    AddEntryData(forecast, (LineDataSeries)GraphData.GetDataSetByIndex(0), graphType);
                }
            }
            else
            {
                if (GraphData == null)
                {
                    BarGraphDataSet dataSet = CreateDataSet(new List<BarGraphEntry>(), graphType);
                    AddEntryData(forecast, dataSet, graphType);
                    this.GraphData = CreateGraphData(dataSet, graphType);
                }
                else
                {
                    AddEntryData(forecast, (BarGraphDataSet)GraphData.GetDataSetByIndex(0), graphType);
                }
            }

            // Re-calc min/max
            this.GraphData.NotifyDataChanged();
        }

        public void SetForecastData<T>(IList<T> forecasts, ForecastGraphType graphType) where T : BaseForecast
        {
            LineDataSeries series = CreateSeriesData(new List<LineGraphEntry>(forecasts.Count), graphType);

            foreach (var forecast in forecasts)
            {
                AddEntryData(forecast, series, graphType);
            }

            if (graphType == ForecastGraphType.Rain || graphType == ForecastGraphType.Snow)
            {
                var unit = SettingsManager.PrecipitationUnit;

                // Heavy rain — rate is >= 7.6 mm (0.30 in) per hr
                // Snow will often accumulate at a rate of 0.5in (12.7mm) an hour
                switch (unit)
                {
                    case Units.INCHES:
                    default:
                        if (graphType == ForecastGraphType.Snow)
                        {
                            series.SetSeriesMinMax(0f, Math.Max(series.YMax, 0.5f));
                        }
                        else
                        {
                            series.SetSeriesMinMax(0f, Math.Max(series.YMax, 0.3f));
                        }
                        break;
                    case Units.MILLIMETERS:
                        if (graphType == ForecastGraphType.Snow)
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

            this.GraphData = CreateGraphData(ImmutableList.Create(series), graphType);
            this.GraphType = graphType;
        }

        public void SetMinutelyForecastData(IEnumerable<MinutelyForecast> forecasts)
        {
            LineDataSeries series = CreateSeriesData(new List<LineGraphEntry>(forecasts.Count()), ForecastGraphType.Minutely);

            foreach (var forecast in forecasts)
            {
                AddMinutelyEntryData(forecast, series);
            }

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

            this.GraphData = CreateGraphData(ImmutableList.Create(series), ForecastGraphType.Minutely);
            this.GraphType = ForecastGraphType.Minutely;
        }

        private void AddEntryData<T>(T forecast, LineDataSeries series, ForecastGraphType graphType) where T : BaseForecast
        {
            var isFahrenheit = Units.FAHRENHEIT.Equals(SettingsManager.TemperatureUnit);
            var culture = CultureUtils.UserCulture;

            string date = GetDateFromForecast(forecast);

            switch (graphType)
            {
                case ForecastGraphType.Temperature:
                    if (forecast.high_f.HasValue && forecast.high_c.HasValue)
                    {
                        int value = (int)(isFahrenheit ? Math.Round(forecast.high_f.Value) : Math.Round(forecast.high_c.Value));
                        var hiTemp = string.Format(culture, "{0}°", value);
                        series.AddEntry(new LineGraphEntry(date, new YEntryData(value, hiTemp)));
                    }
                    break;
                default:
                case ForecastGraphType.Precipitation:
                    if (forecast.extras?.pop.HasValue == true && forecast.extras.pop >= 0)
                    {
                        series.AddEntry(new LineGraphEntry(date, new YEntryData(forecast.extras.pop.Value, forecast.extras.pop.Value + "%")));
                    }
                    else
                    {
                        series.AddEntry(new LineGraphEntry(date, new YEntryData(0f, "0%")));
                    }
                    break;
                case ForecastGraphType.Wind:
                    if (forecast.extras?.wind_mph.HasValue == true && forecast.extras.wind_mph >= 0)
                    {
                        string unit = SettingsManager.SpeedUnit;
                        int speedVal;
                        string speedUnit;

                        switch (unit)
                        {
                            case Units.MILES_PER_HOUR:
                            default:
                                speedVal = (int)Math.Round(forecast.extras.wind_mph.Value);
                                speedUnit = App.Current.ResLoader.GetString("/Units/unit_mph");
                                break;
                            case Units.KILOMETERS_PER_HOUR:
                                speedVal = (int)Math.Round(forecast.extras.wind_kph.Value);
                                speedUnit = App.Current.ResLoader.GetString("/Units/unit_kph");
                                break;
                            case Units.METERS_PER_SECOND:
                                speedVal = (int)Math.Round(ConversionMethods.KphToMSec(forecast.extras.wind_kph.Value));
                                speedUnit = App.Current.ResLoader.GetString("/Units/unit_msec");
                                break;
                        }

                        var windSpeed = string.Format(culture, "{0} {1}", speedVal, speedUnit);

                        series.AddEntry(new LineGraphEntry(date, new YEntryData(speedVal, windSpeed)));
                    }
                    break;
                case ForecastGraphType.Rain:
                    if (forecast.extras?.qpf_rain_in.HasValue == true && forecast.extras?.qpf_rain_mm.HasValue == true)
                    {
                        string unit = SettingsManager.PrecipitationUnit;
                        float precipValue;
                        string precipUnit;

                        switch (unit)
                        {
                            case Units.INCHES:
                            default:
                                precipValue = forecast.extras.qpf_rain_in.Value;
                                precipUnit = App.Current.ResLoader.GetString("/Units/unit_in");
                                break;
                            case Units.MILLIMETERS:
                                precipValue = forecast.extras.qpf_rain_mm.Value;
                                precipUnit = App.Current.ResLoader.GetString("/Units/unit_mm");
                                break;
                        }

                        series.AddEntry(new LineGraphEntry(date, new YEntryData(precipValue, String.Format(culture, "{0:0.##} {1}", precipValue, precipUnit))));
                    }
                    break;
                case ForecastGraphType.Snow:
                    if (forecast.extras?.qpf_snow_in.HasValue == true && forecast.extras?.qpf_snow_cm.HasValue == true)
                    {
                        string unit = SettingsManager.PrecipitationUnit;
                        float precipValue;
                        string precipUnit;

                        switch (unit)
                        {
                            case Units.INCHES:
                            default:
                                precipValue = forecast.extras.qpf_snow_in.Value;
                                precipUnit = App.Current.ResLoader.GetString("/Units/unit_in");
                                break;
                            case Units.MILLIMETERS:
                                precipValue = forecast.extras.qpf_snow_cm.Value * 10;
                                precipUnit = App.Current.ResLoader.GetString("/Units/unit_mm");
                                break;
                        }

                        series.AddEntry(new LineGraphEntry(date, new YEntryData(precipValue, String.Format(culture, "{0:0.##} {1}", precipValue, precipUnit))));
                    }
                    break;
                case ForecastGraphType.UVIndex:
                    if (forecast.extras?.uv_index.HasValue == true)
                    {
                        series.AddEntry(new LineGraphEntry(date, new YEntryData(forecast.extras.uv_index.Value, String.Format(culture, "{0:0.#}", forecast.extras.uv_index.Value))));
                    }
                    break;
                case ForecastGraphType.Humidity:
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
                var culture = CultureUtils.UserCulture;

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
                string precipUnit;

                switch (unit)
                {
                    case Units.INCHES:
                    default:
                        precipValue = ConversionMethods.MMToIn(forecast.rain_mm.Value);
                        precipUnit = App.Current.ResLoader.GetString("/Units/unit_in");
                        break;
                    case Units.MILLIMETERS:
                        precipValue = forecast.rain_mm.Value;
                        precipUnit = App.Current.ResLoader.GetString("/Units/unit_mm");
                        break;
                }

                series.AddEntry(new LineGraphEntry(date, new YEntryData(precipValue, String.Format(culture, "{0:0.##} {1}", precipValue, precipUnit))));
            }
        }

        private LineDataSeries CreateSeriesData(IList<LineGraphEntry> entryData, ForecastGraphType graphType)
        {
            LineDataSeries series;

            switch (graphType)
            {
                case ForecastGraphType.Temperature:
                    series = new LineDataSeries(entryData);
                    series.SetSeriesColors(Colors.OrangeRed);
                    break;
                default:
                case ForecastGraphType.Precipitation:
                    series = new LineDataSeries(entryData);
                    series.SetSeriesColors(Color.FromArgb(0xFF, 0, 0x70, 0xC0));
                    series.SetSeriesMinMax(0f, 100f);
                    break;
                case ForecastGraphType.Wind:
                    series = new LineDataSeries(entryData);
                    series.SetSeriesColors(Colors.SeaGreen);
                    break;
                case ForecastGraphType.Minutely:
                case ForecastGraphType.Rain:
                    series = new LineDataSeries(entryData);
                    series.SetSeriesColors(Colors.DeepSkyBlue);
                    break;
                case ForecastGraphType.Snow:
                    series = new LineDataSeries(entryData);
                    series.SetSeriesColors(Colors.SkyBlue);
                    break;
                case ForecastGraphType.UVIndex:
                    series = new LineDataSeries(entryData);
                    series.SetSeriesColors(Colors.Orange);
                    series.SetSeriesMinMax(0f, 12f);
                    break;
                case ForecastGraphType.Humidity:
                    series = new LineDataSeries(entryData);
                    series.SetSeriesColors(Colors.MediumPurple);
                    series.SetSeriesMinMax(0f, 100f);
                    break;
            }

            return series;
        }

        private LineViewData CreateGraphData(IList<LineDataSeries> seriesData, ForecastGraphType graphType)
        {
            string graphLabel = GetLabelForGraphType(graphType);
            this.GraphType = graphType;

            return new LineViewData(graphLabel, seriesData);
        }

        private BarGraphDataSet CreateDataSet(IList<BarGraphEntry> entryData, ForecastGraphType graphType)
        {
            BarGraphDataSet dataSet = new(entryData);

            switch (graphType)
            {
                case ForecastGraphType.UVIndex:
                    dataSet.SetMinMax(0, 12);
                    break;
            }

            return dataSet;
        }

        private BarGraphData CreateGraphData(BarGraphDataSet dataSet, ForecastGraphType graphType)
        {
            string graphLabel = GetLabelForGraphType(graphType);
            this.GraphType = graphType;

            return new BarGraphData(graphLabel, dataSet);
        }

        private void AddEntryData<T>(T forecast, BarGraphDataSet dataSet, ForecastGraphType graphType) where T : BaseForecast
        {
            string date = GetDateFromForecast(forecast);
            var culture = CultureUtils.UserCulture;

            if (graphType == ForecastGraphType.UVIndex)
            {
                if (forecast.extras?.uv_index.HasValue == true)
                {
                    BarGraphEntry entry = new(date, new(forecast.extras.uv_index.Value, string.Format(culture, "{0:0.#}", forecast.extras.uv_index.Value)))
                    {
                        FillColor = WeatherUtils.GetColorFromUVIndex(forecast.extras.uv_index)
                    };
                    dataSet.AddEntry(entry);
                }
            }
        }

        private string GetDateFromForecast<T>(T forecast) where T : BaseForecast
        {
            string date;
            var culture = CultureUtils.UserCulture;

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

        private string GetLabelForGraphType(ForecastGraphType graphType)
        {
            var ResLoader = App.Current.ResLoader;

            string graphLabel;

            switch (graphType)
            {
                case ForecastGraphType.Temperature:
                    graphLabel = ResLoader.GetString("label_temperature");
                    break;
                default:
                case ForecastGraphType.Minutely:
                case ForecastGraphType.Precipitation:
                    graphLabel = ResLoader.GetString("label_precipitation");
                    break;
                case ForecastGraphType.Wind:
                    graphLabel = ResLoader.GetString("label_wind");
                    break;
                case ForecastGraphType.Rain:
                    graphLabel = ResLoader.GetString("label_qpf_rain");
                    break;
                case ForecastGraphType.Snow:
                    graphLabel = ResLoader.GetString("label_qpf_snow");
                    break;
                case ForecastGraphType.UVIndex:
                    graphLabel = ResLoader.GetString("label_uv");
                    break;
                case ForecastGraphType.Humidity:
                    graphLabel = ResLoader.GetString("label_humidity");
                    break;
            }

            return graphLabel;
        }
    }
}