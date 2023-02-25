using CommunityToolkit.Maui.Converters;
using System;
using System.Globalization;

namespace SimpleWeather.Maui.Converters
{
#nullable enable
    public class NullableColorConverter : BaseConverterOneWay<Color?, Color>
    {
        public Color FallbackColor
        {
            get => (Color)GetValue(FallbackColorProperty);
            set => SetValue(FallbackColorProperty, value);
        }

        public static readonly BindableProperty FallbackColorProperty =
            BindableProperty.Create(nameof(FallbackColor), typeof(Color), typeof(NullableColorConverter), Colors.White, propertyChanged: OnFallbackColorPropertyChanged);

        public override Color DefaultConvertReturnValue { get; set; } = Colors.White;

        public override Color ConvertFrom(Color? value, CultureInfo? culture)
        {
            return value ?? FallbackColor;
        }

        private static void OnFallbackColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is NullableColorConverter conv && newValue is Color newColor)
            {
                conv.DefaultConvertReturnValue = newColor;
            }
        }
    }
#nullable disable
}

