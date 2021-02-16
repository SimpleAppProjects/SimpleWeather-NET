﻿using SimpleWeather.Controls;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.UWP.Controls;
using SimpleWeather.UWP.Helpers;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SimpleWeather.UWP.Main
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WeatherChartsPage : CustomPage, IBackRequestedPage, IWeatherErrorListener
    {
        private LocationData location { get; set; }
        public WeatherNowViewModel WeatherView { get; set; }
        public ChartsViewModel ChartsView { get; set; }

        public List<ForecastGraphViewModel> GraphModels
        {
            get { return (List<ForecastGraphViewModel>)GetValue(GraphModelsProperty); }
            set { SetValue(GraphModelsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GraphModels.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GraphModelsProperty =
            DependencyProperty.Register("GraphModels", typeof(List<ForecastGraphViewModel>), typeof(WeatherChartsPage), new PropertyMetadata(null));

        public WeatherChartsPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Disabled;

            // CommandBar
            CommandBarLabel = App.ResLoader.GetString("Label_Forecast/Header");
            AnalyticsLogger.LogEvent("WeatherChartsPage");
        }

        public void OnWeatherError(WeatherException wEx)
        {
            Dispatcher.RunOnUIThread(() =>
            {
                switch (wEx.ErrorStatus)
                {
                    case WeatherUtils.ErrorStatus.NetworkError:
                    case WeatherUtils.ErrorStatus.NoWeather:
                        // Show error message and prompt to refresh
                        ShowSnackbar(Snackbar.Make(wEx.Message, SnackbarDuration.Long));
                        break;

                    case WeatherUtils.ErrorStatus.QueryNotFound:
                        if (WeatherAPI.NWS.Equals(Settings.API))
                        {
                            ShowSnackbar(Snackbar.Make(App.ResLoader.GetString("Error_WeatherUSOnly"), SnackbarDuration.Long));
                        }
                        break;

                    default:
                        // Show error message
                        ShowSnackbar(Snackbar.Make(wEx.Message, SnackbarDuration.Long));
                        break;
                }
            });
        }

        public Task<bool> OnBackRequested()
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        /// <summary>
        /// OnNavigatedTo
        /// </summary>
        /// <param name="e"></param>
        /// <exception cref="WeatherException">Ignore.</exception>
        /// <exception cref="ObjectDisposedException">Ignore.</exception>
        /// <exception cref="InvalidOperationException">Ignore.</exception>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e?.Parameter is WeatherPageArgs args)
            {
                location = args.Location;
                WeatherView = args.WeatherNowView;

                if (WeatherView == null)
                    WeatherView = new WeatherNowViewModel(Dispatcher);
                if (ChartsView == null)
                    ChartsView = new ChartsViewModel();

                Task.Run(async () =>
                {
                    if (location == null)
                        location = await Settings.GetHomeData();

                    await ChartsView.UpdateForecasts(location);
                }).ContinueWith((t) =>
                {
                    SetBinding(GraphModelsProperty, new Binding()
                    {
                        Mode = BindingMode.OneWay,
                        Source = ChartsView.GraphModels
                    });

                    if (WeatherView?.IsValid == false)
                    {
                        new WeatherDataLoader(location)
                            .LoadWeatherData(new WeatherRequest.Builder()
                                .ForceLoadSavedData()
                                .SetErrorListener(this)
                                .Build())
                                .ContinueWith((t2) =>
                                {
                                    WeatherView.UpdateView(t2.Result);
                                });
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext()).ConfigureAwait(true);
            }
        }
    }
}
