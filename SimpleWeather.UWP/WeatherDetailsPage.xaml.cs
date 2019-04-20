using SimpleWeather.Controls;
using SimpleWeather.UWP.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SimpleWeather.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WeatherDetailsPage : Page, ICommandBarPage, IBackRequestedPage
    {
        public WeatherNowViewModel WeatherView = null;
        public bool IsHourly { get; set; }

        public string CommandBarLabel { get; set; }
        public List<ICommandBarElement> PrimaryCommands { get; set; }

        public WeatherDetailsPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Disabled;

            // CommandBar
            CommandBarLabel = App.ResLoader.GetString("Label_Forecast/Text");
        }

        public Task<bool> OnBackRequested()
        {
            var tcs = new TaskCompletionSource<bool>();

            if (Frame.CanGoBack)
            {
                Frame.GoBack();
                tcs.SetResult(true);
                return tcs.Task;
            }

            tcs.SetResult(false);
            return tcs.Task;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is DetailsPageArgs args)
            {
                WeatherView = args.WeatherNowView;
                IsHourly = args.IsHourly;

                ListControl.Loaded += (sender, ev) =>
                {
                    if (IsHourly)
                    {
                        ListControl.ItemsSource = WeatherView.Extras.HourlyForecast;
                        if (args.ScrollToPosition <= WeatherView.Extras.HourlyForecast.Count - 1)
                            ListControl.ScrollIntoView(ListControl.Items[args.ScrollToPosition], ScrollIntoViewAlignment.Leading);
                    }
                    else
                    {
                        ListControl.ItemsSource = WeatherView.Forecasts;
                        if (args.ScrollToPosition <= WeatherView.Forecasts.Count - 1)
                            ListControl.ScrollIntoView(ListControl.Items[args.ScrollToPosition], ScrollIntoViewAlignment.Leading);
                    }
                };
            }
        }

        public class DetailsPageArgs
        {
            public WeatherNowViewModel WeatherNowView { get; set; }
            public bool IsHourly { get; set; }
            public int ScrollToPosition { get; set; }
        }
    }
}
