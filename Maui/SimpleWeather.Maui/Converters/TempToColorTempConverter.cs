using CommunityToolkit.Maui.Converters;
using SimpleWeather.Common.Controls;
using SimpleWeather.Utils;
using System.Globalization;

namespace SimpleWeather.Maui.Converters
{
    public class TempToColorTempConverter : BaseConverterOneWay<WeatherUiModel, Color>
    {
        public Color FallbackColor
        {
            get { return (Color)GetValue(FallbackColorProperty); }
            set { SetValue(FallbackColorProperty, value); }
        }

        public static readonly BindableProperty FallbackColorProperty =
            BindableProperty.Create(nameof(FallbackColor), typeof(Color), typeof(TempToColorTempConverter), Colors.White);

        public override Color DefaultConvertReturnValue { get; set; } = Colors.White;

        public override Color ConvertFrom(WeatherUiModel value, CultureInfo culture)
        {
            if (value is WeatherUiModel weather)
            {
                string temp = weather.CurTemp?.ToString();
                string temp_str = temp?.RemoveNonDigitChars();

                if (float.TryParse(temp_str, out float temp_f))
                {
                    var tempUnit = weather.TempUnit;

                    if (Equals(tempUnit, Units.CELSIUS) || temp.EndsWith(Units.CELSIUS))
                    {
                        temp_f = ConversionMethods.CtoF(temp_f);
                    }

                    return WeatherUtils.GetColorFromTempF(temp_f, FallbackColor);
                }
            }

            return FallbackColor;
        }
    }
    public class TempToColorTempBrushConverter : BaseConverterOneWay<WeatherUiModel, SolidColorBrush>
    {
        public Color FallbackColor
        {
            get { return (Color)GetValue(FallbackColorProperty); }
            set { SetValue(FallbackColorProperty, value); }
        }

        public static readonly BindableProperty FallbackColorProperty =
            BindableProperty.Create(nameof(FallbackColor), typeof(Color), typeof(TempToColorTempConverter), Colors.White);

        public override SolidColorBrush DefaultConvertReturnValue { get; set; } = new SolidColorBrush(Colors.White);

        public override SolidColorBrush ConvertFrom(WeatherUiModel value, CultureInfo culture)
        {
            if (value is WeatherUiModel weather)
            {
                string temp = weather.CurTemp?.ToString();
                string temp_str = temp?.RemoveNonDigitChars();

                if (float.TryParse(temp_str, out float temp_f))
                {
                    var tempUnit = weather.TempUnit;

                    if (Equals(tempUnit, Units.CELSIUS) || temp.EndsWith(Units.CELSIUS))
                    {
                        temp_f = ConversionMethods.CtoF(temp_f);
                    }

                    return new SolidColorBrush(WeatherUtils.GetColorFromTempF(temp_f, FallbackColor));
                }
            }

            return new SolidColorBrush(FallbackColor);
        }
    }
}
