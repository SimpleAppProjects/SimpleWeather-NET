using System;
namespace SimpleWeather.Maui.MarkupExtensions
{
    public class SolidColorBrushExtension : BindableObject, IMarkupExtension<SolidColorBrush>
    {
        public float Alpha { get; set; } = 1f;

        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public static readonly BindableProperty ColorProperty =
            BindableProperty.Create(nameof(Color), typeof(Color), typeof(SolidColorBrushExtension), SolidColorBrush.ColorProperty.DefaultValue);

        public SolidColorBrush ProvideValue(IServiceProvider serviceProvider)
        {
            return new SolidColorBrush(Color?.WithAlpha(Alpha));
        }

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        {
            return (this as IMarkupExtension<SolidColorBrush>).ProvideValue(serviceProvider);
        }
    }
}

