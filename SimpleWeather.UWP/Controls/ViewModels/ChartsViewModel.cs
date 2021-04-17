using SimpleWeather.Controls;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.UWP.Controls.Graphs;
using SimpleWeather.UWP.Utils;
using SimpleWeather.WeatherData;
using SQLite;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace SimpleWeather.UWP.Controls
{
    public class ChartsViewModel : DependencyObject, IDisposable
    {
        private LocationData locationData;
        private string unitCode;
        private string iconProvider;

        private ObservableItem<IList<HourlyForecast>> currentHrForecastsData;

        public ICollection<ForecastGraphViewModel> GraphModels
        {
            get { return (ICollection<ForecastGraphViewModel>)GetValue(GraphModelsProperty); }
            set { SetValue(GraphModelsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GraphModels.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GraphModelsProperty =
            DependencyProperty.Register("GraphModels", typeof(ICollection<ForecastGraphViewModel>), typeof(ChartsViewModel), new PropertyMetadata(null));

        public ChartsViewModel()
        {
            currentHrForecastsData = new ObservableItem<IList<HourlyForecast>>();
            currentHrForecastsData.ItemValueChanged += CurrentHrForecastsData_ItemValueChanged;
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

                    Settings.RegisterWeatherDBChangedEvent(ChartsViewModel_TableChanged);
                });
            }
            else if (!Equals(unitCode, Settings.UnitString) || !Equals(iconProvider, Settings.IconProvider))
            {
                this.unitCode = Settings.UnitString;
                this.iconProvider = Settings.IconProvider;

                RefreshHourlyForecasts(currentHrForecastsData.GetValue());
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
            });
        }

        private void CurrentHrForecastsData_ItemValueChanged(object sender, ObservableItemChangedEventArgs e)
        {
            if (e.NewValue is IList<HourlyForecast> hrfcasts)
            {
                RefreshHourlyForecasts(hrfcasts);
            }
        }

        private void RefreshHourlyForecasts(IList<HourlyForecast> hrfcasts)
        {
            Dispatcher.LaunchOnUIThread(() =>
            {
                GraphModels = null;

                if (hrfcasts?.Count > 0)
                {
                    GraphModels = CreateGraphModelData(hrfcasts);
                }
            });
        }

        private ICollection<ForecastGraphViewModel> CreateGraphModelData(IList<HourlyForecast> hrfcasts)
        {
            IReadOnlyList<ForecastGraphType> graphTypes = 
                Enum.GetValues(typeof(ForecastGraphType)).Cast<ForecastGraphType>().ToList();
            var data = new List<ForecastGraphViewModel>(graphTypes.Count);

            // TODO: replace with [Sorted||Ordered]Dictionary
            ForecastGraphViewModel /*tempData = null, */popData = null, windData = null, rainData = null, snowData = null, uviData = null, humidityData = null;

            for (int i = 0; i < hrfcasts.Count; i++)
            {
                var hrfcast = hrfcasts[i];

                if (i == 0)
                {
                    //tempData = new ForecastGraphViewModel();

                    if (hrfcast.extras?.pop.HasValue == true)
                    {
                        popData = new ForecastGraphViewModel();
                    }
                    if (hrfcast.wind_mph.HasValue && hrfcast.wind_kph.HasValue)
                    {
                        windData = new ForecastGraphViewModel();
                    }
                    if (hrfcast.extras?.qpf_rain_in.HasValue == true && hrfcast.extras?.qpf_rain_mm.HasValue == true)
                    {
                        rainData = new ForecastGraphViewModel();
                    }
                    if (hrfcast.extras?.qpf_snow_in.HasValue == true && hrfcast.extras?.qpf_snow_cm.HasValue == true)
                    {
                        snowData = new ForecastGraphViewModel();
                    }
                    if (hrfcast.extras?.uv_index.HasValue == true)
                    {
                        uviData = new ForecastGraphViewModel();
                    }
                    if (hrfcast.extras?.humidity.HasValue == true)
                    {
                        humidityData = new ForecastGraphViewModel();
                    }
                }

                /*
                if (tempData != null)
                {
                    tempData.AddForecastData(hrfcast, ForecastGraphType.Temperature);
                }
                */
                if (popData != null)
                {
                    if (hrfcast.extras?.pop.HasValue == true && hrfcast.extras.pop >= 0)
                    {
                        popData.AddForecastData(hrfcast, ForecastGraphType.Precipitation);
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
            if (popData?.SeriesData?.Count > 0)
            {
                data.Add(popData);
            }
            if (windData?.SeriesData?.Count > 0)
            {
                data.Add(windData);
            }
            if (humidityData?.SeriesData?.Count > 0)
            {
                data.Add(humidityData);
            }
            if (uviData?.SeriesData?.Count > 0)
            {
                data.Add(uviData);
            }
            if (rainData?.SeriesData?.Count > 0)
            {
                data.Add(rainData);
            }
            if (snowData?.SeriesData?.Count > 0)
            {
                data.Add(snowData);
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
