using SimpleWeather.Controls;
using System;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.UWP.Controls
{
    [TemplatePart(Name = "LocationName", Type = typeof(string))]
    [TemplatePart(Name = "LocationCountry", Type = typeof(string))]
    [TemplatePart(Name = "PinIconVisibility", Type = typeof(Visibility))]
    public sealed class LocationQuery : Control
    {
        public LocationQueryViewModel ViewModel
        {
            get { return (this.DataContext as LocationQueryViewModel); }
        }

        public static readonly DependencyProperty LocationNameProperty =
            DependencyProperty.Register("LocationName", typeof(String),
            typeof(LocationQuery), new PropertyMetadata(""));
        public static readonly DependencyProperty LocationCountryProperty =
            DependencyProperty.Register("LocationCountry", typeof(String),
            typeof(LocationQuery), new PropertyMetadata(""));
        public static readonly DependencyProperty PinIconVisibilityProperty =
            DependencyProperty.Register("PinIconVisibility", typeof(Visibility),
            typeof(LocationQuery), new PropertyMetadata(Visibility.Visible));

        public string LocationName
        {
            get { return (string)GetValue(LocationNameProperty); }
            set { SetValue(LocationNameProperty, value); }
        }
        public string LocationCountry
        {
            get { return (string)GetValue(LocationCountryProperty); }
            set { SetValue(LocationCountryProperty, value); }
        }
        public Visibility PinIconVisibility
        {
            get { return (Visibility)GetValue(PinIconVisibilityProperty); }
            set { SetValue(PinIconVisibilityProperty, value); }
        }

        private FontIcon PinIconElement;
        private TextBlock LocationNameElement;
        private TextBlock LocationCountryElement;

        public LocationQuery()
        {
            DefaultStyleKey = typeof(LocationQuery);
            this.DataContextChanged += LocationQuery_DataContextChanged;
        }

        private void LocationQuery_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (ViewModel != null)
                PinIconVisibility = String.IsNullOrWhiteSpace(ViewModel.LocationQuery) && String.IsNullOrWhiteSpace(ViewModel.LocationCountry) ? Visibility.Collapsed : Visibility.Visible;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PinIconElement = GetTemplateChild("PinIcon") as FontIcon;
            LocationNameElement = GetTemplateChild("LocationName") as TextBlock;
            LocationCountryElement = GetTemplateChild("LocationCountry") as TextBlock;
        }
    }
}
