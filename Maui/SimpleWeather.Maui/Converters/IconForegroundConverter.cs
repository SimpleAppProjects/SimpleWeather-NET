using CommunityToolkit.Maui.Converters;
using System.Globalization;

namespace SimpleWeather.Maui.Converters
{
    public class IconForegroundConverter : BaseConverterOneWay<object, Color>
    {
        public object ConverterParameter
        {
            get { return (object)GetValue(ConverterParameterProperty); }
            set { SetValue(ConverterParameterProperty, value); }
        }

        public static readonly BindableProperty ConverterParameterProperty =
            BindableProperty.Create(nameof(ConverterParameter), typeof(object), typeof(IconForegroundConverter), null);

        public override Color DefaultConvertReturnValue { get; set; }

        public override Color ConvertFrom(object value, CultureInfo culture)
        {
            var wim = SharedModule.Instance.WeatherIconsManager;

            if (wim.IsFontIcon)
            {
                if (ConverterParameter is Color paramColor)
                {
                    return paramColor;
                }
                else
                {
                    return Colors.White;
                }
            }
            else
            {
                return Colors.Transparent;
            }
        }
    }
    public class IconForegroundBrushConverter : BaseConverterOneWay<object, SolidColorBrush>
    {
        public object ConverterParameter
        {
            get { return (object)GetValue(ConverterParameterProperty); }
            set { SetValue(ConverterParameterProperty, value); }
        }

        public static readonly BindableProperty ConverterParameterProperty =
            BindableProperty.Create(nameof(ConverterParameter), typeof(object), typeof(IconForegroundConverter), null);

        public override SolidColorBrush DefaultConvertReturnValue { get; set; }

        public override SolidColorBrush ConvertFrom(object value, CultureInfo culture)
        {
            var wim = SharedModule.Instance.WeatherIconsManager;

            if (wim.IsFontIcon)
            {
                if (ConverterParameter is Color paramColor)
                {
                    return new SolidColorBrush(paramColor);
                }
                else
                {
                    return new SolidColorBrush(Colors.White);
                }
            }
            else
            {
                return new SolidColorBrush(Colors.Transparent);
            }
        }
    }
}
