using SimpleWeather.ComponentModel;
using SimpleWeather.Controls;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.UWP.Controls.Graphs;
using SimpleWeather.WeatherData;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace SimpleWeather.UWP.Controls
{
    public class ChartsViewModel : DependencyObject, IViewModel, IDisposable
    {
        private LocationData locationData;
        private string unitCode;
        private string iconProvider;

        private ObservableItem<Forecasts> currentForecastsData;
        private ObservableItem<IList<HourlyForecast>> currentHrForecastsData;

        public ICollection<object> GraphModels
        {
            get { return (ICollection<object>)GetValue(GraphModelsProperty); }
            set { SetValue(GraphModelsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GraphModels.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GraphModelsProperty =
            DependencyProperty.Register("GraphModels", typeof(ICollection<object>), typeof(ChartsViewModel), new PropertyMetadata(null));

        public ChartsViewModel()
        {
            currentHrForecastsData = new ObservableItem<IList<HourlyForecast>>();
            currentHrForecastsData.ItemValueChanged += CurrentHrForecastsData_ItemValueChanged;
            currentForecastsData = new ObservableItem<Forecasts>();
            currentForecastsData.ItemValueChanged += CurrentForecastsData_ItemValueChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            Dispatcher.LaunchOnUIThread(() =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            });
        }

        public void UpdateForecasts(LocationData location)
        {
            if (this.locationData == null || !Equals(this.locationData?.query, location?.query))
            {
                Task.Run(async () =>
                {
                    Settings.UnregisterWeatherDBChangedEvent(ChartsViewModel_TableChanged);

                    // Clone location data
                    this.locationData = new LocationData(new LocationQueryViewModel(location));

                    this.unitCode = Settings.UnitString;
                    this.iconProvider = Settings.IconProvider;

                    var dateBlob = DateTimeOffset.Now.ToOffset(location.tz_offset).Trim(TimeSpan.TicksPerHour).ToString("yyyy-MM-dd HH:mm:ss zzzz", CultureInfo.InvariantCulture);
                    currentHrForecastsData.SetValue(await Settings.GetHourlyWeatherForecastDataByPageIndexByLimitFilterByDate(location.query, 0, 12, dateBlob));

                    currentForecastsData.SetValue(await Settings.GetWeatherForecastData(location.query));

                    Settings.RegisterWeatherDBChangedEvent(ChartsViewModel_TableChanged);
                });
            }
            else if (!Equals(unitCode, Settings.UnitString) || !Equals(iconProvider, Settings.IconProvider))
            {
                this.unitCode = Settings.UnitString;
                this.iconProvider = Settings.IconProvider;

                RefreshGraphModelData(currentForecastsData?.GetValue(), currentHrForecastsData?.GetValue());
            }
        }

        private void ChartsViewModel_TableChanged(object sender, NotifyTableChangedEventArgs e)
        {
            if (locationData == null) return;

            Task.Run(async () =>
            {
                if (e?.Table?.TableName == WeatherData.HourlyForecasts.TABLE_NAME)
                {
                    currentHrForecastsData.SetValue(await Settings.GetHourlyWeatherForecastDataByPageIndexByLimit(locationData.query, 0, 12));
                }
                else if (e?.Table?.TableName == WeatherData.Forecasts.TABLE_NAME)
                {
                    currentForecastsData.SetValue(await Settings.GetWeatherForecastData(locationData.query));
                }
            });
        }

        private void CurrentHrForecastsData_ItemValueChanged(object sender, ObservableItemChangedEventArgs e)
        {
            if (e.NewValue is IList<HourlyForecast> hrfcasts)
            {
                RefreshGraphModelData(currentForecastsData?.GetValue(), hrfcasts);
            }
        }

        private void CurrentForecastsData_ItemValueChanged(object sender, ObservableItemChangedEventArgs e)
        {
            if (e.NewValue is Forecasts fcasts)
            {
                RefreshGraphModelData(fcasts, currentHrForecastsData?.GetValue());
            }
        }

        private void RefreshGraphModelData(Forecasts forecasts, IList<HourlyForecast> hrfcasts)
        {
            Dispatcher.LaunchOnUIThread(() =>
            {
                GraphModels = null;

                if (forecasts?.min_forecast?.Count > 0 || hrfcasts?.Count > 0)
                {
                    var now = DateTimeOffset.Now.ToOffset(locationData?.tz_offset ?? TimeSpan.Zero);
                    GraphModels = CreateGraphModelData(forecasts?.min_forecast?.Where(m => m.date >= now)?.Take(60), hrfcasts);
                }
            });
        }

        private ICollection<object> CreateGraphModelData(IEnumerable<MinutelyForecast> minfcasts, IList<HourlyForecast> hrfcasts)
        {
            IReadOnlyList<ForecastGraphType> graphTypes =
                Enum.GetValues(typeof(ForecastGraphType)).Cast<ForecastGraphType>().ToList();
            var data = new List<object>(graphTypes.Count + (minfcasts?.Any() == true ? 1 : 0));

            if (minfcasts?.Any() == true)
            {
                var model = new ForecastGraphDataCreator();
                model.SetMinutelyForecastData(minfcasts);
                data.Add(model.GraphData);
            }

            if (hrfcasts?.Any() == true)
            {
                // TODO: replace with [Sorted||Ordered]Dictionary
                //ForecastGraphViewModel tempData = null;
                ForecastGraphDataCreator popData = null;
                ForecastGraphDataCreator windData = null;
                ForecastGraphDataCreator rainData = null;
                ForecastGraphDataCreator snowData = null;
                ForecastGraphDataCreator uviData = null;
                ForecastGraphDataCreator humidityData = null;

                for (int i = 0; i < hrfcasts.Count; i++)
                {
                    var hrfcast = hrfcasts[i];

                    if (i == 0)
                    {
                        //tempData = new ForecastGraphViewModel();
                        if (hrfcasts.FirstOrDefault()?.extras?.pop.HasValue == true && hrfcasts.FirstOrDefault()?.extras?.pop.HasValue == true ||
                            hrfcasts.LastOrDefault()?.extras?.pop.HasValue == true && hrfcasts.LastOrDefault()?.extras?.pop.HasValue == true)
                        {
                            popData = new ForecastGraphDataCreator();
                        }
                        if (hrfcasts.FirstOrDefault()?.wind_mph.HasValue == true && hrfcasts.FirstOrDefault()?.wind_kph.HasValue == true ||
                            hrfcasts.LastOrDefault()?.wind_mph.HasValue == true && hrfcasts.LastOrDefault()?.wind_kph.HasValue == true)
                        {
                            windData = new ForecastGraphDataCreator();
                        }
                        if (hrfcasts.FirstOrDefault()?.extras?.qpf_rain_in.HasValue == true && hrfcasts.FirstOrDefault()?.extras?.qpf_rain_mm.HasValue == true ||
                            hrfcasts.LastOrDefault()?.extras?.qpf_rain_in.HasValue == true && hrfcasts.LastOrDefault()?.extras?.qpf_rain_mm.HasValue == true)
                        {
                            rainData = new ForecastGraphDataCreator();
                        }
                        if (hrfcasts.FirstOrDefault()?.extras?.qpf_snow_in.HasValue == true && hrfcasts.FirstOrDefault()?.extras?.qpf_snow_cm.HasValue == true ||
                            hrfcasts.LastOrDefault()?.extras?.qpf_snow_in.HasValue == true && hrfcasts.LastOrDefault()?.extras?.qpf_snow_cm.HasValue == true)
                        {
                            snowData = new ForecastGraphDataCreator();
                        }
                        if (hrfcasts.FirstOrDefault()?.extras?.uv_index.HasValue == true ||
                            hrfcasts.LastOrDefault()?.extras?.uv_index.HasValue == true)
                        {
                            uviData = new ForecastGraphDataCreator();
                        }
                        if (hrfcasts.FirstOrDefault()?.extras?.humidity.HasValue == true ||
                            hrfcasts.LastOrDefault()?.extras?.humidity.HasValue == true)
                        {
                            humidityData = new ForecastGraphDataCreator();
                        }
                    }

                    //tempData?.AddForecastData(hrfcast, ForecastGraphType.Temperature);
                    if (popData != null)
                    {
                        if (hrfcast.extras?.pop.HasValue == true)
                        {
                            popData?.AddForecastData(hrfcast, ForecastGraphType.Precipitation);
                        }
                    }
                    if (windData != null)
                    {
                        if (hrfcast.wind_mph.HasValue && hrfcast.wind_mph >= 0)
                        {
                            windData.AddForecastData(hrfcast, ForecastGraphType.Wind);
                        }
                    }
                    if (rainData != null)
                    {
                        if (hrfcast.extras?.qpf_rain_in.HasValue == true && hrfcast.extras?.qpf_rain_mm.HasValue == true)
                        {
                            rainData.AddForecastData(hrfcast, ForecastGraphType.Rain);
                        }
                    }
                    if (snowData != null)
                    {
                        if (hrfcast.extras?.qpf_snow_in.HasValue == true && hrfcast.extras?.qpf_snow_cm.HasValue == true)
                        {
                            snowData.AddForecastData(hrfcast, ForecastGraphType.Snow);
                        }
                    }
                    if (uviData != null)
                    {
                        if (hrfcast.extras?.uv_index.HasValue == true)
                        {
                            uviData.AddForecastData(hrfcast, ForecastGraphType.UVIndex);
                        }
                    }
                    if (humidityData != null)
                    {
                        if (hrfcast.extras?.humidity.HasValue == true)
                        {
                            humidityData.AddForecastData(hrfcast, ForecastGraphType.Humidity);
                        }
                    }
                }

                /*
                if (tempData?.SeriesData?.Count > 0)
                {
                    data.Add(tempData);
                }
                */
                if (popData?.GraphData?.DataCount > 0)
                {
                    data.Add(popData.GraphData);
                }
                if (windData?.GraphData?.DataCount > 0)
                {
                    data.Add(windData.GraphData);
                }
                if (humidityData?.GraphData?.DataCount > 0)
                {
                    data.Add(humidityData.GraphData);
                }
                if (uviData?.GraphData?.DataCount > 0)
                {
                    data.Add(uviData.GraphData);
                }
                if (rainData?.GraphData?.DataCount > 0)
                {
                    rainData?.GraphData?.ForEach(it =>
                    {
                        if (it is LineDataSeries series)
                        {
                            // Heavy rain — rate is >= 7.6 mm (0.30 in) per hr
                            switch (Settings.PrecipitationUnit)
                            {
                                default:
                                case Units.INCHES:
                                    series.SetSeriesMinMax(0f, MathF.Max(series.YMax, 0.3f));
                                    break;
                                case Units.MILLIMETERS:
                                    series.SetSeriesMinMax(0f, MathF.Max(series.YMax, 7.6f));
                                    break;
                            }
                        }

                    });
                    rainData?.GraphData?.NotifyDataChanged();
                    data.Add(rainData.GraphData);
                }
                if (snowData?.GraphData?.DataCount > 0)
                {
                    snowData?.GraphData?.ForEach(it =>
                    {
                        if (it is LineDataSeries series)
                        {
                            // Snow will often accumulate at a rate of 0.5in (12.7mm) an hour
                            switch (Settings.PrecipitationUnit)
                            {
                                default:
                                case Units.INCHES:
                                    series.SetSeriesMinMax(0f, MathF.Max(series.YMax, 0.5f));
                                    break;
                                case Units.MILLIMETERS:
                                    series.SetSeriesMinMax(0f, MathF.Max(series.YMax, 12.7f));
                                    break;
                            }
                        }

                    });
                    snowData?.GraphData?.NotifyDataChanged();
                    data.Add(snowData.GraphData);
                }
            }

            return data;
        }

        private bool isDisposed;
        // Dispose() calls Dispose(true)
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed) return;

            if (disposing)
            {
                // free managed resources
                Settings.UnregisterWeatherDBChangedEvent(ChartsViewModel_TableChanged);
            }

            isDisposed = true;
        }
    }
}
