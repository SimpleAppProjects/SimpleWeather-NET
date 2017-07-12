using SimpleWeather.Controls;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
namespace SimpleWeather.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WeatherNow : Page, IWeatherLoadedListener
    {
        WeatherDataLoader wLoader = null;
        WeatherNowViewModel WeatherView { get; set; }

        KeyValuePair<int, string> pair;

        public void OnWeatherLoaded(int locationIdx, Weather weather)
        {
            // Save index before update
            int index = TextForecastControl.SelectedIndex;

            if (weather != null)
                WeatherView.UpdateView(weather);

            // Set saved index from before update
            // Note: needed since ItemSource is cleared and index is reset
            if (index == 0) // Note: UWP Mobile Bug
                TextForecastControl.SelectedIndex = index + 1;
            TextForecastControl.SelectedIndex = index;

            if (WeatherView.WUExtras.HourlyForecast.Count >= 1)
            {
                if (HourlyForecastPanel.Visibility != Visibility.Visible)
                    HourlyForecastPanel.Visibility = Visibility.Visible;
            }
            if (WeatherView.WUExtras.TextForecast.Count >= 1)
            {
                if (ForecastSwitch.Visibility != Visibility.Visible)
                    ForecastSwitch.Visibility = Visibility.Visible;
            }
            if (!String.IsNullOrWhiteSpace(WeatherView.WUExtras.Chance))
            {
                if (PrecipitationPanel.Visibility != Visibility.Visible)
                    PrecipitationPanel.Visibility = Visibility.Visible;
            }
            else
            {
                DetailsWrapGrid.Children.Remove(PrecipitationPanel);
            }

            LoadingRing.IsActive = false;
        }

        public WeatherNow()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;

            WeatherView = new WeatherNowViewModel();
            DetailsPanel.SizeChanged += DetailsPanel_SizeChanged;

            HeaderLeft.Click += delegate { ScrollTxtPanel(false); };
            HeaderRight.Click += delegate { ScrollTxtPanel(true); };

            // Additional Details (WUExtras)
            ForecastSwitch.Visibility = Visibility.Collapsed;
            TextForecastPanel.Visibility = Visibility.Collapsed;
            HourlyForecastPanel.Visibility = Visibility.Collapsed;
            PrecipitationPanel.Visibility = Visibility.Collapsed;
        }

        private void DetailsPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.ActualWidth <= 600)
            {
                // Keep as one column on smaller screens
                DetailsWrapGrid.MaximumRowsOrColumns = 1;
                // Increase card width based on screen size
                DetailsWrapGrid.ItemWidth = e.NewSize.Width - DetailsPanel.Padding.Right;
            }
            else
            {
                // Minimum width for ea. card
                int minWidth = 250;
                // Size of the view
                int viewWidth = (int)(e.NewSize.Width - DetailsPanel.Padding.Right - DetailsPanel.Padding.Left);
                // Available columns based on min card width
                int availColumns = (viewWidth / minWidth) == 0 ? 1 : viewWidth / minWidth;
                // Maximum columns to use
                int maxColumns = (availColumns > DetailsWrapGrid.Children.Count) ? DetailsWrapGrid.Children.Count : availColumns;

                int freeSpace = viewWidth - (minWidth * maxColumns);
                // Increase card width to fill available space
                int itemWidth = minWidth + (freeSpace / maxColumns);

                DetailsWrapGrid.MaximumRowsOrColumns = maxColumns;
                DetailsWrapGrid.ItemWidth = itemWidth;
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            // Clear cache if nav'ing back
            if (e.NavigationMode == NavigationMode.Back)
                NavigationCacheMode = NavigationCacheMode.Disabled;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Did home change?
            CoreApplication.Properties.TryGetValue("HomeChanged", out object homeChanged);

            // Update view on resume
            // ex. If temperature unit changed
            if ((wLoader != null) && e.NavigationMode != NavigationMode.New && (homeChanged == null || homeChanged != null && !(bool)homeChanged))
            {
                if (wLoader != null)
                {
                    // Save index before update
                    int index = TextForecastControl.SelectedIndex;

                    if (wLoader.GetWeather() != null)
                        WeatherView.UpdateView(wLoader.GetWeather());

                    // Set saved index from before update
                    // Note: needed since ItemSource is cleared and index is reset
                    if (index == 0) // Note: UWP Mobile Bug
                        TextForecastControl.SelectedIndex = index + 1;
                    TextForecastControl.SelectedIndex = index;
                }

                if (pair.Key == App.HomeIdx)
                {
                    // Clear backstack since we're home
                    Frame.BackStack.Clear();
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                }

                return;
            }

            // Reset loader if new page instance created
            if (e.NavigationMode == NavigationMode.New)
            {
                wLoader = null;
            }

            // New page instance created, so restore
            if (e.Parameter != null && (wLoader == null || e.NavigationMode == NavigationMode.New))
            {
                if (e.Parameter.GetType() == typeof(KeyValuePair<int, string>))
                {
                    pair = (KeyValuePair<int, string>)e.Parameter;

                    wLoader = new WeatherDataLoader(this, pair.Value, pair.Key);

                    if (pair.Key == App.HomeIdx)
                    {
                        // Clear backstack since we're home
                        Frame.BackStack.Clear();
                        SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                    }
                }
            }

            Restore();
        }

        private async void Restore()
        {
            if (wLoader == null)
            {
                // Weather was loaded before. Lets load it up...
                List<string> locations = await Settings.GetLocations();
                string local = locations[App.HomeIdx];

                wLoader = new WeatherDataLoader(this, local, App.HomeIdx);

                // Clear backstack since we're home
                Frame.BackStack.Clear();
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            }

            // Load up weather data
            RefreshWeather(false);
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshWeather(true);
        }

        private async void RefreshWeather(bool forceRefresh)
        {
            LoadingRing.IsActive = true;
            await wLoader.LoadWeatherData(forceRefresh);
        }

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            var controlName = (sender as FrameworkElement).Name;

            if (controlName.Contains("Hourly"))
                ScrollLeft(HourlyForecastViewer);
            else
                ScrollLeft(ForecastViewer);
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            var controlName = (sender as FrameworkElement).Name;

            if (controlName.Contains("Hourly"))
                ScrollRight(HourlyForecastViewer);
            else
                ScrollRight(ForecastViewer);
        }

        private void ForecastViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            ScrollViewer viewer = sender as ScrollViewer;
            var controlName = (sender as FrameworkElement).Name;

            if (viewer.HorizontalOffset == 0)
            {
                if (controlName.Contains("Hourly"))
                    HourlyLeftButton.IsEnabled = false;
                else
                    LeftButton.IsEnabled = false;
            }
            else if (viewer.HorizontalOffset == viewer.ScrollableWidth)
            {
                if (controlName.Contains("Hourly"))
                    HourlyRightButton.IsEnabled = false;
                else
                    RightButton.IsEnabled = false;
            }
            else
            {
                if (controlName.Contains("Hourly"))
                {
                    if (!HourlyLeftButton.IsEnabled)
                        HourlyLeftButton.IsEnabled = true;
                    if (!HourlyRightButton.IsEnabled)
                        HourlyRightButton.IsEnabled = true;
                }
                else
                {
                    if (!LeftButton.IsEnabled)
                        LeftButton.IsEnabled = true;
                    if (!RightButton.IsEnabled)
                        RightButton.IsEnabled = true;
                }
            }
        }

        private void ScrollTxtPanel(bool right)
        {
            if (right && TextForecastControl.SelectedIndex < TextForecastControl.Items.Count - 1)
            {
                TextForecastControl.SelectedIndex++;
            }
            else if (!right && TextForecastControl.SelectedIndex > 0)
            {
                TextForecastControl.SelectedIndex--;
            }
        }

        private void TextForecastControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is Pivot control && control.ItemsSource != null)
            {
                var source = (ObservableCollection<TextForecastItemViewModel>)control.ItemsSource;

                if (source != null && source.Count > 1)
                {
                    if (control.SelectedItem == null)
                    {
                        control.SelectedIndex = 0;
                    }
                    else if (control.SelectedIndex == 0)
                    {
                        HeaderLeft.IsEnabled = false;

                        HeaderLeft.Content = String.Empty;
                        Header.Text = source[control.SelectedIndex].Title;
                        HeaderRight.Content = source[control.SelectedIndex + 1].Title;

                        if (!HeaderRight.IsEnabled)
                            HeaderRight.IsEnabled = true;
                    }
                    else if (control.SelectedIndex == source.Count - 1)
                    {
                        if (!HeaderLeft.IsEnabled)
                            HeaderLeft.IsEnabled = true;

                        HeaderLeft.Content = source[control.SelectedIndex - 1].Title;
                        Header.Text = source[control.SelectedIndex].Title;
                        HeaderRight.Content = String.Empty;

                        HeaderRight.IsEnabled = false;
                    }
                    else
                    {
                        if (!HeaderLeft.IsEnabled)
                            HeaderLeft.IsEnabled = true;
                        if (!HeaderRight.IsEnabled)
                            HeaderRight.IsEnabled = true;

                        HeaderLeft.Content = source[control.SelectedIndex - 1].Title;
                        Header.Text = source[control.SelectedIndex].Title;
                        HeaderRight.Content = source[control.SelectedIndex + 1].Title;
                    }
                }
            }
        }

        private void ScrollLeft(ScrollViewer viewer)
        {
            viewer.ChangeView(viewer.HorizontalOffset - viewer.ActualWidth, null, null);
        }

        private void ScrollRight(ScrollViewer viewer)
        {
            viewer.ChangeView(viewer.HorizontalOffset + viewer.ActualWidth, null, null);
        }

        private void ForecastSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ForecastGrid.Visibility = ForecastSwitch.IsOn ?
                Visibility.Collapsed : Visibility.Visible;
            TextForecastPanel.Visibility = ForecastSwitch.IsOn ? 
                Visibility.Visible : Visibility.Collapsed;

            if (ForecastSwitch.IsOn)
                TextForecastControl_SelectionChanged(TextForecastControl, null);
            else
                ForecastViewer_ViewChanged(ForecastViewer, null);
        }
    }
}
