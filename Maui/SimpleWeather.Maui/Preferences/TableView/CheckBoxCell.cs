using System;
using System.Globalization;
using CommunityToolkit.Maui.Markup;
using Microsoft.Maui;
using Microsoft.Maui.Graphics.Text;
using SimpleWeather.Maui.Helpers;
using SimpleWeather.Utils;

namespace SimpleWeather.Maui.Preferences
{
    public class CheckBoxCell : ViewCell
    {
        public bool IsChecked
        {
            get => (bool)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }

        public static readonly BindableProperty IsCheckedProperty =
            BindableProperty.Create(nameof(IsChecked), typeof(bool), typeof(CheckBoxCell), false, propertyChanged: (obj, oldValue, newValue) =>
            {
                var checkboxCell = (CheckBoxCell)obj;
                checkboxCell.OnChanged?.Invoke(obj, new ToggledEventArgs((bool)newValue));
            }, defaultBindingMode: BindingMode.TwoWay);

        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public static readonly BindableProperty ColorProperty =
            BindableProperty.Create(nameof(Color), typeof(Color), typeof(CheckBoxCell), null);

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(CheckBoxCell), default(string));

        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        public static readonly BindableProperty TextColorProperty =
            BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(CheckBoxCell), null);

        public string Detail
        {
            get => (string)GetValue(DetailProperty);
            set => SetValue(DetailProperty, value);
        }

        public static readonly BindableProperty DetailProperty =
            BindableProperty.Create(nameof(Detail), typeof(string), typeof(CheckBoxCell), default(string));

        public Color DetailColor
        {
            get => (Color)GetValue(DetailColorProperty);
            set => SetValue(DetailColorProperty, value);
        }

        public static readonly BindableProperty DetailColorProperty =
            BindableProperty.Create(nameof(DetailColor), typeof(Color), typeof(CheckBoxCell), null);

        public bool IsCompact
        {
            get => (bool)GetValue(IsCompactProperty);
            set => SetValue(IsCompactProperty, value);
        }

        public static readonly BindableProperty IsCompactProperty =
            BindableProperty.Create(nameof(IsCompact), typeof(bool), typeof(CheckBoxCell), false);

        public event EventHandler<ToggledEventArgs> OnChanged;

        public CheckBoxCell()
        {
            this.View = new Grid()
            {
                RowDefinitions =
                {
                    new RowDefinition(GridLength.Star),
                    new RowDefinition(GridLength.Star)
                        .Bind(RowDefinition.HeightProperty, static src => src.Detail, mode: BindingMode.OneWay, source: this,
                            convert: (s) =>
                            {
                                return !string.IsNullOrWhiteSpace(s) ? GridLength.Star : new GridLength(0);
                            }
                        ),
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
                        convert: (enabled) =>
                        {
                            return enabled ? 1.0d : 0.5d;
                        }
                     )
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
                        convert: (s) =>
                        {
                            return !string.IsNullOrWhiteSpace(s?.ToString());
                        }
                    )
                    .Bind(Label.IsEnabledProperty, static src => src.IsEnabled, mode: BindingMode.OneWay, source: this)
                    .Bind(Label.OpacityProperty, static src => src.IsEnabled, mode: BindingMode.OneWay, source: this,
                        convert: (enabled) =>
                        {
                            return enabled ? 1.0d : 0.5d;
                        }
                     )
                    .CenterVertical()
                    .Column(0)
                    .Row(1),
                    new CheckBox()
                        .Bind(CheckBox.ColorProperty, static src => src.Color, mode: BindingMode.OneWay, source: this)
                        .Bind(CheckBox.IsCheckedProperty, static src => src.IsChecked, (cell, value) => cell.IsChecked = value, mode: BindingMode.TwoWay, source: this)
                        .Bind(CheckBox.IsEnabledProperty, static src => src.IsEnabled, mode: BindingMode.OneWay, source: this)
                        .CenterVertical()
                        .Column(1)
                        .RowSpan(2)
                }
            }.Bind(Grid.PaddingProperty, static src => src.IsCompact, mode: BindingMode.OneWay, source: this,
                convert: (isCompact) =>
                {
                    return new Thickness(16, isCompact ? 0 : 8);
                }
            );

            this.View.TapGesture(OnTapped);
        }

        protected override void OnTapped()
        {
            if (!IsEnabled) return;

            base.OnTapped();

            this.IsChecked = !this.IsChecked;
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