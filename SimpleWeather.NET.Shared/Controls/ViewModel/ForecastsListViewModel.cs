#if WINDOWS
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.Collections;
#else
using SimpleWeather.Maui.IncrementalLoadingCollection;
#endif
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.Common.Controls;
using SimpleWeather.ComponentModel;
using SimpleWeather.Database;
using SimpleWeather.LocationData;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API;
using SimpleWeather.WeatherData;
using System.ComponentModel;
#if !WINDOWS
using System.Collections.Generic;
using System.Collections;
#endif

namespace SimpleWeather.NET.Controls
{
    [Bindable(true)]
#if WINDOWS
    [WinRT.GeneratedBindableCustomProperty]
#endif
    public partial class ForecastsListViewModel : BaseViewModel, IDisposable
    {
        private LocationData.LocationData locationData;
        private string unitCode;

        [ObservableProperty]
        public partial IncrementalLoadingCollection<ForecastSource, ForecastItemViewModel> Forecasts { get; set; }

        [ObservableProperty]
        public partial IncrementalLoadingCollection<HourlyForecastSource, HourlyForecastItemViewModel> HourlyForecasts { get; set; }

        [ObservableProperty]
#if WINDOWS
        public partial object SelectedForecasts { get; set; }
#else
        public partial IEnumerable SelectedForecasts { get; set; }
#endif

        private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();
        private readonly WeatherDatabase WeatherDB = WeatherDatabase.Instance;

        public void SelectForecast(bool isHourly)
        {
            SelectedForecasts = isHourly ? HourlyForecasts : Forecasts;
        }

        public void UpdateForecasts(LocationData.LocationData location)
        {
            if (this.locationData == null || !Equals(this.locationData?.query, location?.query))
            {
                WeatherDB.Connection.GetConnection().TableChanged -= ForecastsViewModel_TableChanged;

                // Clone location data
                this.locationData = new LocationQuery(location).ToLocationData();

                this.unitCode = SettingsManager.UnitString;

                // Update forecasts from database
                ResetForecasts();
                ResetHourlyForecasts();

                WeatherDB.Connection.GetConnection().TableChanged += ForecastsViewModel_TableChanged;
            }
            else if (!Equals(unitCode, SettingsManager.UnitString))
            {
                this.unitCode = SettingsManager.UnitString;

                RefreshForecasts();
                RefreshHourlyForecasts();
            }
        }

        private void ForecastsViewModel_TableChanged(object sender, SQLite.NotifyTableChangedEventArgs e)
        {
            if (locationData == null) return;

            Task.Run(() =>
            {
                if (e?.Table?.TableName == WeatherData.Forecasts.TABLE_NAME)
                {
                    RefreshForecasts();
                }
                if (e?.Table?.TableName == WeatherData.HourlyForecasts.TABLE_NAME)
                {
                    RefreshHourlyForecasts();
                }
            });
        }

        private void ResetForecasts()
        {
            Forecasts = new IncrementalLoadingCollection<ForecastSource, ForecastItemViewModel>(new ForecastSource(locationData), 7);
        }

        public void RefreshForecasts()
        {
            if (Forecasts == null)
            {
                Forecasts = new IncrementalLoadingCollection<ForecastSource, ForecastItemViewModel>(new ForecastSource(locationData), 7);
            }
            else
            {
#if WINDOWS
                DispatcherQueue.EnqueueAsync(Forecasts.RefreshAsync);
#else
                MainThread.InvokeOnMainThreadAsync(Forecasts.RefreshAsync);
#endif
            }
        }

        private void ResetHourlyForecasts()
        {
            HourlyForecasts = new IncrementalLoadingCollection<HourlyForecastSource, HourlyForecastItemViewModel>(new HourlyForecastSource(locationData), 24);
        }

        public void RefreshHourlyForecasts()
        {
            if (HourlyForecasts == null)
            {
                HourlyForecasts = new IncrementalLoadingCollection<HourlyForecastSource, HourlyForecastItemViewModel>(new HourlyForecastSource(locationData), 24);
            }
            else
            {
#if WINDOWS
                DispatcherQueue.EnqueueAsync(HourlyForecasts.RefreshAsync);
#else
                MainThread.InvokeOnMainThreadAsync(HourlyForecasts.RefreshAsync);
#endif
            }
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
                WeatherDB.Connection.GetConnection().TableChanged -= ForecastsViewModel_TableChanged;
            }

            isDisposed = true;
        }
    }

    public class ForecastSource : IIncrementalSource<ForecastItemViewModel>
    {
        private readonly WeatherDatabase WeatherDB = WeatherDatabase.Instance;

        private LocationData.LocationData locationData;

        public ForecastSource(LocationData.LocationData location) : base()
        {
            this.locationData = location;
        }

        public Task<IEnumerable<ForecastItemViewModel>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            return Task.Run(async () =>
            {
                if (locationData?.query == null)
                    return new List<ForecastItemViewModel>(0);

                Forecasts fcasts = null;
                try
                {
                    fcasts = await WeatherDB.GetForecastData(locationData.query);
                }
                catch (Exception)
                {
                    // Unable to get forecasts
                }

                int totalCount = fcasts?.forecast?.Count ?? 0;
                var models = new List<ForecastItemViewModel>(Math.Min(totalCount, pageSize));

                if (totalCount > 0)
                {
                    bool isDayAndNt = fcasts?.txt_forecast?.Count == (fcasts?.forecast?.Count * 2);
                    bool addTextFct = isDayAndNt || (fcasts?.txt_forecast?.Count == fcasts?.forecast?.Count && fcasts?.txt_forecast?.Count > 0);

                    int startPosition = pageIndex * pageSize;

                    for (int i = startPosition; i < Math.Min(totalCount, startPosition + pageSize); i++)
                    {
                        ForecastItemViewModel f;
                        var dataItem = fcasts.forecast[i];

                        if (addTextFct)
                        {
                            if (isDayAndNt)
                                f = new ForecastItemViewModel(dataItem, fcasts.txt_forecast[i * 2], fcasts.txt_forecast[(i * 2) + 1]);
                            else
                                f = new ForecastItemViewModel(dataItem, fcasts.txt_forecast[i]);
                        }
                        else
                        {
                            f = new ForecastItemViewModel(dataItem);
                        }

                        models.Add(f);
                    }
                }

                return models.AsEnumerable();
            });
        }
    }

    public class HourlyForecastSource : IIncrementalSource<HourlyForecastItemViewModel>
    {
        private readonly WeatherDatabase WeatherDB = WeatherDatabase.Instance;

        private LocationData.LocationData locationData;

        public HourlyForecastSource(LocationData.LocationData location) : base()
        {
            this.locationData = location;
        }

        public Task<IEnumerable<HourlyForecastItemViewModel>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            return Task.Run(async () =>
            {
                if (locationData?.query == null)
                    return new List<HourlyForecastItemViewModel>(0);

                var hrInterval = WeatherModule.Instance.WeatherManager.HourlyForecastInterval;
                var date = DateTimeOffset.Now.ToOffset(locationData.tz_offset).Trim(TimeSpan.TicksPerHour).AddHours(-(hrInterval * 0.5d));
                var result = await WeatherDB.GetHourlyWeatherForecastDataByPageIndexByLimitFilterByDate(locationData.query, pageIndex, pageSize, date);

                var models = new List<HourlyForecastItemViewModel>();

                if (result?.Count > 0)
                {
                    models.EnsureCapacity(Math.Min(pageSize, result.Count));

                    foreach (HourlyForecast forecast in result)
                    {
                        models.Add(new HourlyForecastItemViewModel(forecast));
                    }
                }

                return models.AsEnumerable();
            });
        }
    }
}