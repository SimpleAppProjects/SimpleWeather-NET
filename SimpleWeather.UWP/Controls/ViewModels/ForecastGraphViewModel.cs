using SimpleWeather.Icons;
using SimpleWeather.Utils;
using SimpleWeather.UWP.Controls.Graphs;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
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

    public class ForecastGraphViewModel : DependencyObject
    {
        public List<XLabelData> LabelData
        {
            get { return (List<XLabelData>)GetValue(LabelDataProperty); }
            set { SetValue(LabelDataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelData.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelDataProperty =
            DependencyProperty.Register("LabelData", typeof(List<XLabelData>), typeof(ForecastGraphViewModel), new PropertyMetadata(null));

        public List<LineDataSeries> SeriesData
        {
            get { return (List<LineDataSeries>)GetValue(SeriesDataProperty); }
            set { SetValue(SeriesDataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SeriesData.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SeriesDataProperty =
            DependencyProperty.Register("SeriesData", typeof(List<LineDataSeries>), typeof(ForecastGraphViewModel), new PropertyMetadata(null));

        public string GraphLabel
        {
            get { return (string)GetValue(GraphLabelProperty); }
            set { SetValue(GraphLabelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GraphLabel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GraphLabelProperty =
            DependencyProperty.Register("GraphLabel", typeof(string), typeof(ForecastGraphViewModel), new PropertyMetadata(null));

        public ForecastGraphViewModel()
        {

        }

        public void AddForecastData<T>(T forecast, ForecastGraphType graphType) where T : BaseForecast
        {
            var culture = CultureUtils.UserCulture;

            if (LabelData == null)
            {
                LabelData = new List<XLabelData>();
            }

            if (SeriesData == null)
            {
                var yEntryData = new List<YEntryData>();
                AddEntryData(forecast, LabelData, yEntryData, graphType);
                SeriesData = CreateSeriesData(yEntryData, graphType);
            }
            else
            {
                AddEntryData(forecast, LabelData, SeriesData.First().SeriesData, graphType);
            }
        }

        public void SetForecastData<T>(IList<T> forecasts, ForecastGraphType graphType) where T : BaseForecast
        {
            var isFahrenheit = Units.FAHRENHEIT.Equals(Settings.TemperatureUnit);
            var culture = CultureUtils.UserCulture;

            var xData = new List<XLabelData>(forecasts.Count);
            var yData = new List<YEntryData>(forecasts.Count);

            foreach (var forecast in forecasts)
            {
                AddEntryData(forecast, xData, yData, graphType);
            }
            LabelData = xData;
            SeriesData = CreateSeriesData(yData, graphType);
        }

        private void AddEntryData<T>(T forecast, IList<XLabelData> xData, IList<YEntryData> yData, ForecastGraphType graphType) where T : BaseForecast
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

            xData.Add(new XLabelData(date));

            switch (graphType)
            {
                /*
                case ForecastGraphType.Temperature:
                    if (forecast.high_f.HasValue && forecast.high_c.HasValue)
                    {
                        int value = (int)(isFahrenheit ? Math.Round(forecast.high_f.Value) : Math.Round(forecast.high_c.Value));
                        var hiTemp = string.Format(culture, "{0}°", value);
                        yData.Add(new YEntryData(value, hiTemp));
                    }
                    break;
                */
                default:
                case ForecastGraphType.Precipitation:
                    if (forecast.extras?.pop.HasValue == true && forecast.extras.pop >= 0)
                    {
                        yData.Add(new YEntryData(forecast.extras.pop.Value, forecast.extras.pop.Value + "%"));
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

                        yData.Add(new YEntryData(speedVal, windSpeed));
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

                        yData.Add(new YEntryData(precipValue, String.Format(culture, "{0:0.##} {1}", precipValue, precipUnit)));
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

                        yData.Add(new YEntryData(precipValue, String.Format(culture, "{0:0.##} {1}", precipValue, precipUnit)));
                    }
                    break;
                case ForecastGraphType.UVIndex:
                    if (forecast.extras?.uv_index.HasValue == true)
                    {
                        yData.Add(new YEntryData(forecast.extras.uv_index.Value, String.Format(culture, "{0:0.#}", forecast.extras.uv_index.Value)));
                    }
                    break;
                case ForecastGraphType.Humidity:
                    if (forecast.extras?.humidity.HasValue == true)
                    {
                        yData.Add(new YEntryData(forecast.extras.humidity.Value, String.Format(culture, "{0}%", forecast.extras.humidity.Value)));
                    }
                    break;
            }
        }

        private List<LineDataSeries> CreateSeriesData(List<YEntryData> yData, ForecastGraphType graphType)
        {
            LineDataSeries series;

            switch (graphType)
            {
                /*
                case ForecastGraphType.Temperature:
                    GraphLabel = App.ResLoader.GetString("Label_Temperature/Text");
                    series = new LineDataSeries(yData);
                    series.SetSeriesColors(Colors.OrangeRed);
                    break;
                */
                default:
                case ForecastGraphType.Precipitation:
                    GraphLabel = App.ResLoader.GetString("Label_Precipitation/Text");
                    series = new LineDataSeries(yData);
                    series.SetSeriesColors(Color.FromArgb(0xFF, 0, 0x70, 0xC0));
                    series.SetSeriesMinMax(0f, 100f);
                    break;
                case ForecastGraphType.Wind:
                    GraphLabel = App.ResLoader.GetString("Label_Wind/Text");
                    series = new LineDataSeries(yData);
                    series.SetSeriesColors(Colors.SeaGreen);
                    break;
                case ForecastGraphType.Rain:
                    GraphLabel = App.ResLoader.GetString("Label_Rain/Text");
                    series = new LineDataSeries(yData);
                    series.SetSeriesColors(Colors.DeepSkyBlue);
                    break;
                case ForecastGraphType.Snow:
                    GraphLabel = App.ResLoader.GetString("Label_Snow/Text");
                    series = new LineDataSeries(yData);
                    series.SetSeriesColors(Colors.SkyBlue);
                    break;
                case ForecastGraphType.UVIndex:
                    GraphLabel = App.ResLoader.GetString("UV_Label");
                    series = new LineDataSeries(yData);
                    series.SetSeriesColors(Colors.Orange);
                    series.SetSeriesMinMax(0f, 12f);
                    break;
                case ForecastGraphType.Humidity:
                    GraphLabel = App.ResLoader.GetString("Label_Humidity/Text");
                    series = new LineDataSeries(yData);
                    series.SetSeriesColors(Colors.MediumPurple);
                    series.SetSeriesMinMax(0f, 100f);
                    break;
            }

            return new List<LineDataSeries>(1) { series };
        }
    }
}
