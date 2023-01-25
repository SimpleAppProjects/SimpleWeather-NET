namespace SimpleWeather.Maui.Helpers
{
    public class SolidColorAlphaBrush : SolidColorBrush
    {
        public double Opacity
        {
            get { return (double)GetValue(OpacityProperty); }
            set { SetValue(OpacityProperty, value); }
        }

        public static readonly BindableProperty OpacityProperty =
            BindableProperty.Create(nameof(Opacity), typeof(double), typeof(SolidColorAlphaBrush), 1.0, propertyChanged: OnOpacityChanged);

        private static void OnOpacityChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is SolidColorAlphaBrush brush)
            {
                brush.Color = brush.Color; // This will be corrected in the setter
            }
        }

        public override Color Color { get => base.Color; set => base.Color = value?.WithAlpha((float)Opacity); }
    }
}
