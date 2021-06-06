using SimpleWeather.Controls;
using SimpleWeather.ComponentModel;
using SimpleWeather.Location;
using SimpleWeather.Utils;
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
    public class ForecastsNowViewModel : DependencyObject, IViewModel, IDisposable
    {
        private LocationData locationData;
        private string unitCode;
        private string iconProvider;

        private ObservableItem<Forecasts> currentForecastsData;
        private ObservableItem<IList<HourlyForecast>> currentHrForecastsData;

        public RangeBarGraphViewModel ForecastGraphData
        {
            get { return (RangeBarGraphViewModel)GetValue(ForecastGraphDataProperty); }
            set { SetValue(ForecastGraphDataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ForecastGraphData.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ForecastGraphDataProperty =
            DependencyProperty.Register("ForecastGraphData", typeof(RangeBarGraphViewModel), typeof(ForecastsNowViewModel), new PropertyMetadata(null));

        public List<HourlyForecastNowViewModel> HourlyForecastData
        {
            get { return (List<HourlyForecastNowViewModel>)GetValue(HourlyForecastDataProperty); }
            set { SetValue(HourlyForecastDataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HourlyForecastData.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HourlyForecastDataProperty =
            DependencyProperty.Register("HourlyForecastData", typeof(List<HourlyForecastNowViewModel>), typeof(ForecastsNowViewModel), new PropertyMetadata(null));

        public ForecastGraphViewModel HourlyPrecipitationGraphData
        {
            get { return (ForecastGraphViewModel)GetValue(HourlyPrecipitationGraphDataProperty); }
            set { SetValue(HourlyPrecipitationGraphDataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PrecipitationGraphData.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HourlyPrecipitationGraphDataProperty =
            DependencyProperty.Register("HourlyPrecipitationGraphData", typeof(ForecastGraphViewModel), typeof(ForecastsNowViewModel), new PropertyMetadata(null));

        public ForecastGraphViewModel MinutelyPrecipitationGraphData
        {
            get { return (ForecastGraphViewModel)GetValue(MinutelyPrecipitationGraphDataProperty); }
            set { SetValue(MinutelyPrecipitationGraphDataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinutelyPrecipitationGraphData.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinutelyPrecipitationGraphDataProperty =
            DependencyProperty.Register("MinutelyPrecipitationGraphData", typeof(ForecastGraphViewModel), typeof(ForecastsNowViewModel), new PropertyMetadata(null));

        public bool IsPrecipitationDataPresent
        {
            get { return (bool)GetValue(IsPrecipitationDataPresentProperty); }
            private set { SetValue(IsPrecipitationDataPresentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsPrecipitationDataPresent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPrecipitationDataPresentProperty =
            DependencyProperty.Register("IsPrecipitationDataPresent", typeof(bool), typeof(ForecastsNowViewModel), new PropertyMetadata(false));

        public ForecastsNowViewModel()
        {
            currentForecastsData = new ObservableItem<Forecasts>();
            currentForecastsData.ItemValueChanged += CurrentForecastsData_ItemValueChanged;
            currentHrForecastsData = new ObservableItem<IList<HourlyForecast>>();
            currentHrForecastsData.ItemValueChanged += CurrentHrForecastsData_ItemValueChanged;

            RegisterPropertyChangedCallback(HourlyPrecipitationGraphDataProperty, ForecastsNowViewModel_OnDependencyPropertyChanged);
            RegisterPropertyChangedCallback(MinutelyPrecipitationGraphDataProperty, ForecastsNowViewModel_OnDependencyPropertyChanged);
        }

        private void ForecastsNowViewModel_OnDependencyPropertyChanged(DependencyObject sender, DependencyProperty dp)
        {
            if (dp == HourlyPrecipitationGraphDataProperty || dp == MinutelyPrecipitationGraphDataProperty)
            {
                IsPrecipitationDataPresent = HourlyPrecipitationGraphData != null || MinutelyPrecipitationGraphData != null;
            }
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
                    Settings.UnregisterWeatherDBChangedEvent(ForecastGraphViewModel_TableChanged);

                    // Clone location data
                    this.locationData = new LocationData(new LocationQueryViewModel(location));

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
            Dispatcher.LaunchOnUIThread(() =>
            {
                ForecastGraphData = fcasts?.forecast?.Count > 0 ? new RangeBarGraphViewModel(fcasts.forecast) : null;
                RefreshMinutelyForecasts(fcasts?.min_forecast);
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
                HourlyForecastData = hrfcasts?.Select(hrf => new HourlyForecastNowViewModel(hrf)).ToList();

                if (hrfcasts != null)
                {
                    var model = new ForecastGraphViewModel();
                    model.SetForecastData(hrfcasts, ForecastGraphType.Precipitation);
                    HourlyPrecipitationGraphData = model;
                }
                else
                {
                    HourlyPrecipitationGraphData = null;
                }
            });
        }

        private void RefreshMinutelyForecasts(IList<MinutelyForecast> minfcasts)
        {
            Dispatcher.LaunchOnUIThread(() =>
            {
                var hrInterval = WeatherManager.GetInstance().HourlyForecastInterval;
                var now = DateTimeOffset.Now.ToOffset(locationData?.tz_offset ?? TimeSpan.Zero).AddHours(-(hrInterval * 0.5)).Trim(TimeSpan.TicksPerHour);

                var minfcastsFiltered = minfcasts?.Where(m => m.date >= now)?.Take(60);

                if (minfcastsFiltered?.Any() == true)
                {
                    var model = new ForecastGraphViewModel();
                    model.SetMinutelyForecastData(minfcastsFiltered);
                    MinutelyPrecipitationGraphData = model;
                }
                else
                {
                    MinutelyPrecipitationGraphData = null;
                }
            });
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
}
