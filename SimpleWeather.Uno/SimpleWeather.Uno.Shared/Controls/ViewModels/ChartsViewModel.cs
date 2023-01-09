using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.ComponentModel;
using SimpleWeather.Database;
using SimpleWeather.LocationData;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Uno.Controls.Graphs;
using SimpleWeather.WeatherData;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
#if NETSTANDARD2_0
using MathF = SimpleWeather.Utils.MathF;
#else
using MathF = System.MathF;
#endif

namespace SimpleWeather.Uno.Controls
{
    [Bindable(true)]
    public partial class ChartsViewModel : BaseViewModel, IDisposable
    {
        private LocationData.LocationData locationData;
        private string unitCode;
        private string iconProvider;

        private ObservableItem<Forecasts> currentForecastsData;
        private ObservableItem<IList<HourlyForecast>> currentHrForecastsData;

        [ObservableProperty]
        private ICollection<object> graphModels;

        private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();
        private readonly WeatherDatabase WeatherDB = WeatherDatabase.Instance;

        public ChartsViewModel()
        {
            currentHrForecastsData = new ObservableItem<IList<HourlyForecast>>();
            currentHrForecastsData.ItemValueChanged += CurrentHrForecastsData_ItemValueChanged;
            currentForecastsData = new ObservableItem<Forecasts>();
            currentForecastsData.ItemValueChanged += CurrentForecastsData_ItemValueChanged;
        }

        public void UpdateForecasts(LocationData.LocationData location)
        {
            if (this.locationData == null || !Equals(this.locationData?.query, location?.query))
            {
                Task.Run((Func<Task>)(async () =>
                {
                    WeatherDB.Connection.GetConnection().TableChanged -= ChartsViewModel_TableChanged;

                    // Clone location data
                    this.locationData = new LocationQuery(location).ToLocationData();

                    this.unitCode = SettingsManager.UnitString;
                    this.iconProvider = SettingsManager.IconProvider;

                    var date = DateTimeOffset.Now.ToOffset(location.tz_offset).Trim(TimeSpan.TicksPerHour);
                    currentHrForecastsData.SetValue(await WeatherDB.GetHourlyWeatherForecastDataByPageIndexByLimitFilterByDate(location.query, 0, 12, date));

                    currentForecastsData.SetValue(await WeatherDB.GetForecastData(location.query));

                    WeatherDB.Connection.GetConnection().TableChanged += ChartsViewModel_TableChanged;
                }));
            }
            else if (!Equals(unitCode, SettingsManager.UnitString) || !Equals(iconProvider, SettingsManager.IconProvider))
            {
                this.unitCode = SettingsManager.UnitString;
                this.iconProvider = SettingsManager.IconProvider;

                RefreshGraphModelData(currentForecastsData?.GetValue(), currentHrForecastsData?.GetValue());
            }
        }

        private void ChartsViewModel_TableChanged(object sender, NotifyTableChangedEventArgs e)
        {
            if (locationData == null) return;

            Task.Run((Func<Task>)(async () =>
            {
                if (e?.Table?.TableName == HourlyForecasts.TABLE_NAME)
                {
                    var date = DateTimeOffset.Now.ToOffset(locationData.tz_offset).Trim(TimeSpan.TicksPerHour);
                    currentHrForecastsData.SetValue(await WeatherDB.GetHourlyWeatherForecastDataByPageIndexByLimitFilterByDate(locationData.query, 0, 12, date));
                }
                else if (e?.Table?.TableName == Forecasts.TABLE_NAME)
                {
                    currentForecastsData.SetValue(await WeatherDB.GetForecastData(locationData.query));
                }
            }));
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
            GraphModels = null;

            if (forecasts?.min_forecast?.Count > 0 || hrfcasts?.Count > 0)
            {
                var now = DateTimeOffset.Now.ToOffset(locationData?.tz_offset ?? TimeSpan.Zero);
                GraphModels = CreateGraphModelData(forecasts?.min_forecast?.Where(m => m.date >= now)?.Take(60), hrfcasts);
            }
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
                            switch (SettingsManager.PrecipitationUnit)
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
                            switch (SettingsManager.PrecipitationUnit)
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
                WeatherDB.Connection.GetConnection().TableChanged -= ChartsViewModel_TableChanged;
            }

            isDisposed = true;
        }
    }
}
