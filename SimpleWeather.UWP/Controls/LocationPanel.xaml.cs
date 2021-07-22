﻿using SimpleWeather.Controls;
using SimpleWeather.Icons;
using SimpleWeather.Utils;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.UWP.Controls
{
    public sealed partial class LocationPanel : UserControl
    {
        public LocationPanelViewModel ViewModel
        {
            get { return this.DataContext as LocationPanelViewModel; }
        }

        private double ControlShadowOpacity
        {
            get { return (double)GetValue(ControlShadowOpacityProperty); }
            set { SetValue(ControlShadowOpacityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ControlShadowOpacity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ControlShadowOpacityProperty =
            DependencyProperty.Register("ControlShadowOpacity", typeof(double), typeof(LocationPanel), new PropertyMetadata(0d));

        private ElementTheme ControlTheme
        {
            get { return (ElementTheme)GetValue(ControlThemeProperty); }
            set { SetValue(ControlThemeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ControlTheme.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ControlThemeProperty =
            DependencyProperty.Register("ControlTheme", typeof(ElementTheme), typeof(LocationPanel), new PropertyMetadata(ElementTheme.Default));

        private readonly Helpers.IconThemeConverter weatherIcoConverter;
        private readonly WeatherIconsManager wim = WeatherIconsManager.GetInstance();

        public LocationPanel()
        {
            this.InitializeComponent();
            this.DataContextChanged += (sender, args) =>
            {
                UpdateControlTheme();
                //this.Bindings.Update(); // Update control theme updates bindings
            };

            weatherIcoConverter = this.Resources["weatherIconThemeConverter"] as Helpers.IconThemeConverter;
        }

        private void BackgroundOverlay_ImageExOpened(object sender, Microsoft.Toolkit.Uwp.UI.Controls.ImageExOpenedEventArgs e)
        {
            UpdateControlTheme(true);
        }

        private void BackgroundOverlay_ImageExFailed(object sender, Microsoft.Toolkit.Uwp.UI.Controls.ImageExFailedEventArgs e)
        {
            UpdateControlTheme(false);
        }

        private void UpdateControlTheme()
        {
            UpdateControlTheme(Utils.FeatureSettings.LocationPanelBackgroundImage);
        }

        private void UpdateControlTheme(bool backgroundEnabled)
        {
            if (backgroundEnabled)
            {
                if (GradientOverlay != null)
                {
                    GradientOverlay.Visibility = Visibility.Visible;
                }
                ControlTheme = ElementTheme.Dark;
                ControlShadowOpacity = 1;
                weatherIcoConverter.ForceDarkTheme = true;
            }
            else
            {
                if (GradientOverlay != null)
                {
                    GradientOverlay.Visibility = Visibility.Collapsed;
                }
                ControlTheme = ElementTheme.Default;
                ControlShadowOpacity = 0;
                weatherIcoConverter.ForceDarkTheme = false;
            }
            this.Bindings.Update();
        }
    }
}
