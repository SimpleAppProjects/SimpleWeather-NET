using CommunityToolkit.Mvvm.ComponentModel;
using SimpleWeather.ComponentModel;
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

namespace SimpleWeather.UWP.Controls
{
    public partial class ForecastsNowViewModel : BaseViewModel, IDisposable
    {
        private LocationData locationData;
        private string unitCode;
        private string iconProvider;

        private ObservableItem<Forecasts> currentForecastsData;
        private ObservableItem<IList<HourlyForecast>> currentHrForecastsData;

        [ObservableProperty]
        private RangeBarGraphData forecastGraphData;

        [ObservableProperty]
        private List<HourlyForecastNowViewModel> hourlyForecastData;

        [ObservableProperty]
        private LineViewData hourlyPrecipitationGraphData;

        [ObservableProperty]
        private LineViewData minutelyPrecipitationGraphData;

        [ObservableProperty]
        private bool isPrecipitationDataPresent;

        public ForecastsNowViewModel()
        {
            currentForecastsData = new ObservableItem<Forecasts>();
            currentForecastsData.ItemValueChanged += CurrentForecastsData_ItemValueChanged;
            currentHrForecastsData = new ObservableItem<IList<HourlyForecast>>();
            currentHrForecastsData.ItemValueChanged += CurrentHrForecastsData_ItemValueChanged;
            this.PropertyChanged += ForecastsNowViewModel_PropertyChanged;
        }

        private void ForecastsNowViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(HourlyPrecipitationGraphData) || e.PropertyName == nameof(MinutelyPrecipitationGraphData))
            {
                IsPrecipitationDataPresent = HourlyPrecipitationGraphData?.DataSets?.FirstOrDefault()?.EntryData?.Count > 0 || MinutelyPrecipitationGraphData?.DataSets?.FirstOrDefault()?.EntryData?.Count > 0;
            }
        }

        public void UpdateForecasts(LocationData location)
        {
            if (this.locationData == null || !Equals(this.locationData?.query, location?.query))
            {
                Task.Run(async () =>
                {
                    Settings.UnregisterWeatherDBChangedEvent(ForecastGraphViewModel_TableChanged);

                    // Clone location data
                    this.locationData = new LocationQuery(location).ToLocationData();

                    this.unitCode = Settings.UnitString;
                    this.iconProvider = Settings.IconProvider;

                    currentForecastsData.SetValue(await Settings.GetWeatherForecastData(location.query));

                    var hrInterval = WeatherManager.GetInstance().HourlyForecastInterval;
                    var dateBlob = DateTimeOffset.Now.ToOffset(location.tz_offset).Trim(TimeSpan.TicksPerHour).AddHours(-(hrInterval * 0.5d)).ToString("yyyy-MM-dd HH:mm:ss zzzz", CultureInfo.InvariantCulture);
                    currentHrForecastsData.SetValue(await Settings.GetHourlyWeatherForecastDataByPageIndexByLimitFilterByDate(location.query, 0, 12, dateBlob));

                    Settings.RegisterWeatherDBChangedEvent(ForecastGraphViewModel_TableChanged);
                });
            }
            else if (!Equals(unitCode, Settings.UnitString) || !Equals(iconProvider, Settings.IconProvider))
            {
                this.unitCode = Settings.UnitString;
                this.iconProvider = Settings.IconProvider;

                RefreshForecasts(currentForecastsData.GetValue());
                RefreshHourlyForecasts(currentHrForecastsData.GetValue());
            }
        }

        private void ForecastGraphViewModel_TableChanged(object sender, NotifyTableChangedEventArgs e)
        {
            if (locationData == null) return;

            Task.Run(async () =>
            {
                if (e?.Table?.TableName == WeatherData.Forecasts.TABLE_NAME)
                {
                    currentForecastsData.SetValue(await Settings.GetWeatherForecastData(locationData.query));
                }
                if (e?.Table?.TableName == WeatherData.HourlyForecasts.TABLE_NAME)
                {
                    currentHrForecastsData.SetValue(await Settings.GetHourlyWeatherForecastDataByPageIndexByLimit(locationData.query, 0, 12));
                }
            });
        }

        private void CurrentForecastsData_ItemValueChanged(object sender, ObservableItemChangedEventArgs e)
        {
            if (e.NewValue is Forecasts fcasts)
            {
                RefreshForecasts(fcasts);
            }
        }

        private void RefreshForecasts(Forecasts fcasts)
        {
            // At most 10 forecasts
            ForecastGraphData = CreateGraphData(fcasts?.forecast?.Take(10));
            RefreshMinutelyForecasts(fcasts?.min_forecast);
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
            HourlyForecastData = hrfcasts?.Select(hrf => new HourlyForecastNowViewModel(hrf)).ToList();

            if (hrfcasts != null)
            {
                var model = new ForecastGraphDataCreator();
                model.SetForecastData(hrfcasts, hrfcasts.GetRecommendedGraphType());
                HourlyPrecipitationGraphData = (LineViewData)model.GraphData;
            }
            else
            {
                HourlyPrecipitationGraphData = null;
            }
        }

        private void RefreshMinutelyForecasts(IList<MinutelyForecast> minfcasts)
        {
            var hrInterval = WeatherManager.GetInstance().HourlyForecastInterval;
            var now = DateTimeOffset.Now.ToOffset(locationData?.tz_offset ?? TimeSpan.Zero).AddHours(-(hrInterval * 0.5)).Trim(TimeSpan.TicksPerHour);

            var minfcastsFiltered = minfcasts?.Where(m => m.date >= now)?.Take(60);

            if (minfcastsFiltered?.Any() == true)
            {
                var model = new ForecastGraphDataCreator();
                model.SetMinutelyForecastData(minfcastsFiltered);
                MinutelyPrecipitationGraphData = (LineViewData)model.GraphData;
            }
            else
            {
                MinutelyPrecipitationGraphData = null;
            }
        }

        private RangeBarGraphData CreateGraphData(IEnumerable<Forecast> forecastData)
        {
            if (forecastData == null) return null;

            var isFahrenheit = Units.FAHRENHEIT.Equals(Settings.TemperatureUnit);
            var culture = CultureUtils.UserCulture;

            var entryData = new List<RangeBarGraphEntry>(forecastData.Count());

            foreach (var forecast in forecastData)
            {
                if (!forecast.high_f.HasValue && !forecast.low_f.HasValue)
                    continue;

                var entry = new RangeBarGraphEntry();
                string date = forecast.date.ToString("ddd dd", culture);

                entry.XLabel = date;
                entry.XIcon = forecast.icon;

                // Temp Data
                if (forecast.high_f.HasValue && forecast.high_c.HasValue)
                {
                    int value = (int)(isFahrenheit ? Math.Round(forecast.high_f.Value) : Math.Round(forecast.high_c.Value));
                    var hiTemp = string.Format(culture, "{0}°", value);
                    entry.HiTempData = new YEntryData(value, hiTemp);
                }
                if (forecast.low_f.HasValue && forecast.low_c.HasValue)
                {
                    int value = (int)(isFahrenheit ? Math.Round(forecast.low_f.Value) : Math.Round(forecast.low_c.Value));
                    var loTemp = string.Format(culture, "{0}°", value);
                    entry.LoTempData = new YEntryData(value, loTemp);
                }

                entryData.Add(entry);
            }

            return new RangeBarGraphData(new RangeBarGraphDataSet(entryData));
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
                Settings.UnregisterWeatherDBChangedEvent(ForecastGraphViewModel_TableChanged);
            }

            isDisposed = true;
        }
    }

    internal static class ForecastsNowViewModelExtensions
    {
        internal static ForecastGraphType GetRecommendedGraphType(this IList<HourlyForecast> forecasts)
        {
            if (forecasts?.FirstOrDefault()?.extras?.pop.HasValue == true && forecasts?.LastOrDefault()?.extras?.pop.HasValue == true)
            {
                return ForecastGraphType.Precipitation;
            }
            else if (forecasts?.FirstOrDefault()?.extras?.qpf_rain_mm.HasValue == true && forecasts?.LastOrDefault()?.extras?.qpf_rain_mm.HasValue == true)
            {
                return ForecastGraphType.Rain;
            }
            else
            {
                return ForecastGraphType.Wind;
            }
        }
    }
}
