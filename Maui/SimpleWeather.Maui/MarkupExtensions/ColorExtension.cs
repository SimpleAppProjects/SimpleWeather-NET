namespace SimpleWeather.Maui.MarkupExtensions
{
    [AcceptEmptyServiceProvider]
    public class ColorExtension : BindableObject, IMarkupExtension<Color>
    {
        public float Alpha { get; set; } = 1f;

        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public static readonly BindableProperty ColorProperty =
            BindableProperty.Create(nameof(Color), typeof(Color), typeof(ColorExtension), Colors.Transparent);

        public Color ProvideValue(IServiceProvider serviceProvider)
        {
            return Color?.WithAlpha(Alpha) ?? Colors.Transparent;
        }

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        {
            return (this as IMarkupExtension<Color>).ProvideValue(serviceProvider);
        }
    }
}

