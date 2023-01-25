namespace SimpleWeather.Maui.Helpers
{
    public static class BindableObjectExtensions
    {
        public static void SetOneWayBinding(this BindableObject bindableObject, BindableProperty property, object source)
        {
            bindableObject.SetBinding(property, new Binding()
            {
                Mode = BindingMode.OneWay,
                Source = source,
            });
        }

        public static T SetOneWayBinding<T>(this T bindableObject, BindableProperty property, object source) where T : BindableObject
        {
            bindableObject.SetBinding(property, new Binding()
            {
                Mode = BindingMode.OneWay,
                Source = source,
            });

            return bindableObject;
        }
    }
}
