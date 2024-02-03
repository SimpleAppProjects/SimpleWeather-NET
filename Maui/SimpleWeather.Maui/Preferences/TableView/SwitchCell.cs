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
                        .Bind<RowDefinition, string, GridLength>(RowDefinition.HeightProperty, nameof(Detail), BindingMode.OneWay, source: this,
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
                    .Bind(Label.TextProperty, nameof(Text), BindingMode.OneWay, source: this)
                    .Bind(Label.TextColorProperty, nameof(TextColor), BindingMode.OneWay, source: this)
                    .Bind(Label.IsEnabledProperty, nameof(IsEnabled), BindingMode.OneWay, source: this)
                    .Bind<Label, bool, double>(Label.OpacityProperty, nameof(IsEnabled), BindingMode.OneWay, source: this,
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
                    .Bind(Label.TextProperty, nameof(Detail), BindingMode.OneWay, source: this)
                    .Bind(Label.TextColorProperty, nameof(DetailColor), BindingMode.OneWay, source: this)
                    .Bind<Label, string, bool>(Label.IsVisibleProperty, nameof(Detail), BindingMode.OneWay, source: this,
                        convert: (s) =>
                        {
                            return !string.IsNullOrWhiteSpace(s?.ToString());
                        }
                    )
                    .Bind(Label.IsEnabledProperty, nameof(IsEnabled), BindingMode.OneWay, source: this)
                    .Bind<Label, bool, double>(Label.OpacityProperty, nameof(IsEnabled), BindingMode.OneWay, source: this,
                        convert: (enabled) =>
                        {
                            return enabled ? 1.0d : 0.5d;
                        }
                     )
                    .CenterVertical()
                    .Column(0)
                    .Row(1),
                    new Switch()
                        .Bind(Switch.OnColorProperty, nameof(OnColor), BindingMode.OneWay, source: this)
                        .Bind(Switch.IsToggledProperty, nameof(On), BindingMode.TwoWay, source: this)
                        .Bind(Switch.IsEnabledProperty, nameof(IsEnabled), BindingMode.OneWay, source: this)
                        .CenterVertical()
                        .Column(1)
                        .RowSpan(2)
                }
            }
            .OnIdiom(Grid.PaddingProperty, Default: new Thickness(20, 8), Desktop: new Thickness(16, 8));
		}

        protected override void OnTapped()
        {
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

