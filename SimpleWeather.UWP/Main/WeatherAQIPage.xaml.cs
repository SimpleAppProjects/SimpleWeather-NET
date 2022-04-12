﻿using SimpleWeather.Controls;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.UWP.Controls;
using SimpleWeather.UWP.Controls.Graphs;
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
    public sealed partial class WeatherAQIPage : Page, ICommandBarPage, ISnackbarPage, IBackRequestedPage, IWeatherErrorListener
    {
        public String CommandBarLabel { get; set; }
        public List<ICommandBarElement> PrimaryCommands { get; set; }

        private LocationData location { get; set; }
        public WeatherNowViewModel WeatherView { get; set; }
        public AirQualityForecastViewModel AQIView { get; private set; } = new AirQualityForecastViewModel();

        private readonly WeatherManager wm = WeatherManager.GetInstance();

        public WeatherAQIPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Disabled;

            // CommandBar
            CommandBarLabel = App.ResLoader.GetString("label_airquality");
            AnalyticsLogger.LogEvent("WeatherAQIPage");
        }

        public void OnWeatherError(WeatherException wEx)
        {
            Dispatcher.LaunchOnUIThread(() =>
            {
                switch (wEx.ErrorStatus)
                {
                    case WeatherUtils.ErrorStatus.NetworkError:
                    case WeatherUtils.ErrorStatus.NoWeather:
                        // Show error message and prompt to refresh
                        ShowSnackbar(Snackbar.MakeError(wEx.Message, SnackbarDuration.Long));
                        break;

                    case WeatherUtils.ErrorStatus.QueryNotFound:
                        if (location?.country_code?.Let(it => !wm.IsRegionSupported(it)) == true)
                        {
                            ShowSnackbar(Snackbar.MakeError(App.ResLoader.GetString("error_message_weather_region_unsupported"), SnackbarDuration.Long));
                        }
                        else
                        {
                            ShowSnackbar(Snackbar.MakeError(wEx.Message, SnackbarDuration.Long));
                        }
                        break;

                    default:
                        // Show error message
                        ShowSnackbar(Snackbar.MakeError(wEx.Message, SnackbarDuration.Long));
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

        public void ShowSnackbar(Snackbar snackbar)
        {
            Shell.Instance?.ShowSnackbar(snackbar);
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

                WeatherView = Shell.Instance.GetViewModel<WeatherNowViewModel>();

                if (AQIView == null)
                    AQIView = new AirQualityForecastViewModel();

                Task.Run(async () =>
                {
                    if (location == null)
                        location = await Settings.GetHomeData();

                    AQIView.UpdateForecasts(location);

                    if (WeatherView?.IsValid == false)
                    {
                        _ = new WeatherDataLoader(location)
                            .LoadWeatherData(new WeatherRequest.Builder()
                                .ForceLoadSavedData()
                                .SetErrorListener(this)
                                .Build())
                                .ContinueWith((t2) =>
                                {
                                    Dispatcher.LaunchOnUIThread(() =>
                                    {
                                        WeatherView.UpdateView(t2.Result);
                                    });
                                });
                    }
                });
            }
        }
    }

    public class AQIDataTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (container is FrameworkElement element && VisualTreeHelperExtensions.GetParent<ItemsControl>(element) is FrameworkElement elementParent)
            {
                if (item is BarGraphData)
                {
                    return elementParent.Resources["AQIGraphTemplate"] as DataTemplate;
                }
                else if (item is AirQualityViewModel)
                {
                    return elementParent.Resources["AQIForecastTemplate"] as DataTemplate;
                }
            }

            return null;
        }
    }
}
