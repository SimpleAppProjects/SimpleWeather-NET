using SimpleWeather.ComponentModel;
using SimpleWeather.Icons;
using SimpleWeather.Utils;
using SimpleWeather.UWP.Controls.Graphs;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;

namespace SimpleWeather.UWP.Controls
{
    public enum ForecastGraphType
    {
        //Temperature,
        Precipitation,
        Wind,
        Humidity,
        UVIndex,
        Rain,
        Snow
    }

    public class ForecastGraphViewModel : DependencyObject, IViewModel
    {
        public LineViewData GraphData
        {
            get { return (LineViewData)GetValue(GraphDataProperty); }
            set { SetValue(GraphDataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GraphLabel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GraphDataProperty =
            DependencyProperty.Register("GraphData", typeof(LineViewData), typeof(ForecastGraphViewModel), new PropertyMetadata(null));

        public ForecastGraphType GraphType
        {
            get { return (ForecastGraphType)GetValue(GraphTypeProperty); }
            set { SetValue(GraphTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GraphType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GraphTypeProperty =
            DependencyProperty.Register("GraphType", typeof(ForecastGraphType), typeof(ForecastGraphViewModel), new PropertyMetadata(0));

        public bool IsEmpty => GraphData?.IsEmpty ?? true;

        public event PropertyChangedEventHandler PropertyChanged;
        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            Dispatcher.LaunchOnUIThread(() =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            });
        }

        public void AddForecastData<T>(T forecast, ForecastGraphType graphType) where T : BaseForecast
        {
            if (GraphData == null)
            {
                var series = CreateSeriesData(new List<LineGraphEntry>(), graphType);
                AddEntryData(forecast, series, graphType);
                this.GraphData = CreateGraphData(ImmutableList.Create(series), graphType);
            }
            else
            {
                AddEntryData(forecast, GraphData.GetDataSetByIndex(0), graphType);
            }
        }

        public void SetForecastData<T>(IList<T> forecasts, ForecastGraphType graphType) where T : BaseForecast
        {
            var series = CreateSeriesData(new List<LineGraphEntry>(forecasts.Count), graphType);

            foreach (var forecast in forecasts)
            {
                AddEntryData(forecast, series, graphType);
            }

            this.GraphData = CreateGraphData(ImmutableList.Create(series), graphType);
            this.GraphType = graphType;
        }

        public void SetMinutelyForecastData(IEnumerable<MinutelyForecast> forecasts)
        {
            var series = CreateSeriesData(new List<LineGraphEntry>(forecasts.Count()), ForecastGraphType.Precipitation);

            foreach (var forecast in forecasts)
            {
                AddMinutelyEntryData(forecast, series);
            }

            this.GraphData = CreateGraphData(ImmutableList.Create(series), ForecastGraphType.Precipitation);
            this.GraphType = ForecastGraphType.Precipitation;
        }

        private void AddEntryData<T>(T forecast, LineDataSeries series, ForecastGraphType graphType) where T : BaseForecast
        {
            var isFahrenheit = Units.FAHRENHEIT.Equals(Settings.TemperatureUnit);
            var culture = CultureUtils.UserCulture;

            string date;
            if (forecast is Forecast fcast)
            {
                date = fcast.date.ToString("ddd dd", culture);
            }
            else if (forecast is HourlyForecast hrfcast)
            {
                if (culture.DateTimeFormat.ShortTimePattern.Contains("H"))
                {
                    date = hrfcast.date.ToString("ddd HH:00", culture);
                }
                else
                {
                    date = hrfcast.date.ToString("ddd h tt", culture);
                }
            }
            else
            {
                date = string.Empty;
            }

            switch (graphType)
            {
                /*
                case ForecastGraphType.Temperature:
                    if (forecast.high_f.HasValue && forecast.high_c.HasValue)
                    {
                        int value = (int)(isFahrenheit ? Math.Round(forecast.high_f.Value) : Math.Round(forecast.high_c.Value));
                        var hiTemp = string.Format(culture, "{0}°", value);
                        yData.Add(new YEntryData(value, hiTemp));
                        xData.Add(new XLabelData(date));
                    }
                    break;
                */
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
                        string unit = Settings.SpeedUnit;
                        int speedVal;
                        string speedUnit;

                        switch (unit)
                        {
                            case Units.MILES_PER_HOUR:
                            default:
                                speedVal = (int)Math.Round(forecast.extras.wind_mph.Value);
                                speedUnit = SimpleLibrary.GetInstance().ResLoader.GetString("/Units/unit_mph");
                                break;
                            case Units.KILOMETERS_PER_HOUR:
                                speedVal = (int)Math.Round(forecast.extras.wind_kph.Value);
                                speedUnit = SimpleLibrary.GetInstance().ResLoader.GetString("/Units/unit_kph");
                                break;
                            case Units.METERS_PER_SECOND:
                                speedVal = (int)Math.Round(ConversionMethods.KphToMSec(forecast.extras.wind_kph.Value));
                                speedUnit = SimpleLibrary.GetInstance().ResLoader.GetString("/Units/unit_msec");
                                break;
                        }

                        var windSpeed = string.Format(culture, "{0} {1}", speedVal, speedUnit);

                        series.AddEntry(new LineGraphEntry(date, new YEntryData(speedVal, windSpeed)));
                    }
                    break;
                case ForecastGraphType.Rain:
                    if (forecast.extras?.qpf_rain_in.HasValue == true && forecast.extras?.qpf_rain_mm.HasValue == true)
                    {
                        string unit = Settings.PrecipitationUnit;
                        float precipValue;
                        string precipUnit;

                        switch (unit)
                        {
                            case Units.INCHES:
                            default:
                                precipValue = forecast.extras.qpf_rain_in.Value;
                                precipUnit = SimpleLibrary.GetInstance().ResLoader.GetString("/Units/unit_in");
                                break;
                            case Units.MILLIMETERS:
                                precipValue = forecast.extras.qpf_rain_mm.Value;
                                precipUnit = SimpleLibrary.GetInstance().ResLoader.GetString("/Units/unit_mm");
                                break;
                        }

                        series.AddEntry(new LineGraphEntry(date, new YEntryData(precipValue, String.Format(culture, "{0:0.##} {1}", precipValue, precipUnit))));
                    }
                    break;
                case ForecastGraphType.Snow:
                    if (forecast.extras?.qpf_snow_in.HasValue == true && forecast.extras?.qpf_snow_cm.HasValue == true)
                    {
                        string unit = Settings.PrecipitationUnit;
                        float precipValue;
                        string precipUnit;

                        switch (unit)
                        {
                            case Units.INCHES:
                            default:
                                precipValue = forecast.extras.qpf_snow_in.Value;
                                precipUnit = SimpleLibrary.GetInstance().ResLoader.GetString("/Units/unit_in");
                                break;
                            case Units.MILLIMETERS:
                                precipValue = forecast.extras.qpf_snow_cm.Value * 10;
                                precipUnit = SimpleLibrary.GetInstance().ResLoader.GetString("/Units/unit_mm");
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

                string unit = Settings.PrecipitationUnit;
                float precipValue;
                string precipUnit;

                switch (unit)
                {
                    case Units.INCHES:
                    default:
                        precipValue = ConversionMethods.MMToIn(forecast.rain_mm.Value);
                        precipUnit = SimpleLibrary.GetInstance().ResLoader.GetString("/Units/unit_in");
                        break;
                    case Units.MILLIMETERS:
                        precipValue = forecast.rain_mm.Value;
                        precipUnit = SimpleLibrary.GetInstance().ResLoader.GetString("/Units/unit_mm");
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
                /*
                case ForecastGraphType.Temperature:
                    series = new LineDataSeries(entryData);
                    series.SetSeriesColors(Colors.OrangeRed);
                    break;
                */
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
            String graphLabel;

            switch (graphType)
            {
                /*
                case ForecastGraphType.Temperature:
                    graphLabel = App.ResLoader.GetString("label_temperature");
                    break;
                */
                default:
                case ForecastGraphType.Precipitation:
                    graphLabel = App.ResLoader.GetString("label_precipitation");
                    break;
                case ForecastGraphType.Wind:
                    graphLabel = App.ResLoader.GetString("label_wind");
                    break;
                case ForecastGraphType.Rain:
                    graphLabel = App.ResLoader.GetString("label_qpf_rain");
                    break;
                case ForecastGraphType.Snow:
                    graphLabel = App.ResLoader.GetString("label_qpf_snow");
                    break;
                case ForecastGraphType.UVIndex:
                    graphLabel = App.ResLoader.GetString("label_uv");
                    break;
                case ForecastGraphType.Humidity:
                    graphLabel = App.ResLoader.GetString("label_humidity");
                    break;
            }

            this.GraphType = graphType;

            return new LineViewData(graphLabel, seriesData);
        }
    }
}