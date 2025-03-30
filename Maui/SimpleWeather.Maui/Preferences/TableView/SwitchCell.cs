using System;
using System.Globalization;
using CommunityToolkit.Maui.Markup;
using Microsoft.Maui;
using Microsoft.Maui.Graphics.Text;
using SimpleWeather.Maui.Helpers;
using SimpleWeather.Utils;

namespace SimpleWeather.Maui.Preferences
{
	public class SwitchCell : ViewCell
	{
        public bool On
        {
            get { return (bool)GetValue(OnProperty); }
            set { SetValue(OnProperty, value); }
        }

        public static readonly BindableProperty OnProperty =
            BindableProperty.Create(nameof(On), typeof(bool), typeof(SwitchCell), false, propertyChanged: (obj, oldValue, newValue) =>
        {
            var switchCell = (SwitchCell)obj;
            switchCell.OnChanged?.Invoke(obj, new ToggledEventArgs((bool)newValue));
        }, defaultBindingMode: BindingMode.TwoWay);

        public Color OnColor
        {
            get { return (Color)GetValue(OnColorProperty); }
            set { SetValue(OnColorProperty, value); }
        }

        public static readonly BindableProperty OnColorProperty =
            BindableProperty.Create(nameof(OnColor), typeof(Color), typeof(SwitchCell), null);

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(SwitchCell), default(string));

        public Color TextColor
        {
            get { return (Color)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        public static readonly BindableProperty TextColorProperty =
            BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(SwitchCell), null);

        public string Detail
        {
            get { return (string)GetValue(DetailProperty); }
            set { SetValue(DetailProperty, value); }
        }

        public static readonly BindableProperty DetailProperty =
            BindableProperty.Create(nameof(Detail), typeof(string), typeof(SwitchCell), default(string));

        public Color DetailColor
        {
            get { return (Color)GetValue(DetailColorProperty); }
            set { SetValue(DetailColorProperty, value); }
        }

        public static readonly BindableProperty DetailColorProperty =
            BindableProperty.Create(nameof(DetailColor), typeof(Color), typeof(SwitchCell), null);

        public event EventHandler<ToggledEventArgs> OnChanged;

        public SwitchCell()
		{
            this.View = new Grid()
            {
                RowDefinitions =
                {
                    new RowDefinition(GridLength.Star),
                    new RowDefinition(GridLength.Star)
                        .Bind(RowDefinition.HeightProperty, static src => src.Detail, mode: BindingMode.OneWay, source: this,
                            convert: (s) => !string.IsNullOrWhiteSpace(s) ? GridLength.Star : new GridLength(0)),
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Auto)
                },
                Padding = new Thickness(16, 8),
                Children =
                {
                    new Label()
                    {
                        FontSize = 17,
                        FontFamily = Microsoft.Maui.Font.Default.Family
                    }
                    .Bind(Label.TextProperty, static src => src.Text, mode: BindingMode.OneWay, source: this)
                    .Bind(Label.TextColorProperty, static src => src.TextColor, mode: BindingMode.OneWay, source: this)
                    .Bind(Label.IsEnabledProperty, static src => src.IsEnabled, mode: BindingMode.OneWay, source: this)
                    .Bind(Label.OpacityProperty, static src => src.IsEnabled, mode: BindingMode.OneWay, source: this,
                        convert: (enabled) => enabled ? 1.0d : 0.5d)
                    .CenterVertical()
                    .Column(0)
                    .Row(0),
                    new Label()
                    {
                        FontSize = 12,
                        FontFamily = Microsoft.Maui.Font.Default.Family
                    }
                    .Paddings(top: 4)
                    .Bind(Label.TextProperty, static src => src.Detail, mode: BindingMode.OneWay, source: this)
                    .Bind(Label.TextColorProperty, static src => src.DetailColor, mode: BindingMode.OneWay, source: this)
                    .Bind(Label.IsVisibleProperty, static src => src.Detail, mode: BindingMode.OneWay, source: this,
                        convert: (s) => !string.IsNullOrWhiteSpace(s?.ToString()))
                    .Bind(Label.IsEnabledProperty, static src => src.IsEnabled, mode: BindingMode.OneWay, source: this)
                    .Bind(Label.OpacityProperty, static src => src.IsEnabled, mode: BindingMode.OneWay, source: this,
                        convert: (enabled) => enabled ? 1.0d : 0.5d)
                    .CenterVertical()
                    .Column(0)
                    .Row(1),
                    new Switch()
                        .Bind(Switch.OnColorProperty, static src => src.OnColor, mode: BindingMode.OneWay, source: this)
                        .Bind(Switch.IsToggledProperty, static src => src.On, (cell, value) => cell.On = value, mode: BindingMode.TwoWay, source: this)
                        .Bind(Switch.IsEnabledProperty, static src => src.IsEnabled, mode: BindingMode.OneWay, source: this)
                        .CenterVertical()
                        .Column(1)
                        .RowSpan(2)
                }
            };

            this.View.TapGesture(OnTapped);
        }

        protected override void OnTapped()
        {
            if (!IsEnabled) return;

            base.OnTapped();

            this.On = !this.On;
        }

        private class DetailValueVisibleConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return !string.IsNullOrWhiteSpace(value?.ToString());
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    }
}

