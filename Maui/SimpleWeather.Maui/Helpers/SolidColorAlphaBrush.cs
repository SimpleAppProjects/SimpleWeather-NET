namespace SimpleWeather.Maui.Helpers
{
    [ContentProperty(nameof(Color))]
    public class SolidColorAlphaBrush : SolidColorBrush
    {
        public float Opacity
        {
            get { return (float)GetValue(OpacityProperty); }
            set { SetValue(OpacityProperty, value); }
        }

        public static readonly BindableProperty OpacityProperty =
            BindableProperty.Create(nameof(Opacity), typeof(float), typeof(SolidColorAlphaBrush), 1.0f, propertyChanged: OnOpacityChanged);

        private static void OnOpacityChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is SolidColorAlphaBrush brush)
            {
                brush.Color = brush.Color?.WithAlpha((float)newValue);
            }
        }

        public override Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value?.WithAlpha(Opacity));
        }
    }
}
