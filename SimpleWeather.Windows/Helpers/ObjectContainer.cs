using Microsoft.UI.Xaml;

namespace SimpleWeather.NET.Helpers
{
    public sealed partial class ObjectContainer : DependencyObject
    {
        public object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(ObjectContainer), new PropertyMetadata(null));
    }
}
