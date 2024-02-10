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

        public static T OnIdiom<T>(this T bindableObject, BindableProperty property,
            object Default, object Desktop = null, object Phone = null, object Tablet = null, object TV = null, object Watch = null
        ) where T : BindableObject
        {
            var idiom = DeviceInfo.Idiom;

            if (idiom == DeviceIdiom.Desktop)
            {
                bindableObject.SetValue(property, Desktop ?? Default);
            }
            else if (idiom == DeviceIdiom.Phone)
            {
                bindableObject.SetValue(property, Phone ?? Default);
            }
            else if (idiom == DeviceIdiom.Tablet)
            {
                bindableObject.SetValue(property, Tablet ?? Default);
            }
            else if (idiom == DeviceIdiom.TV)
            {
                bindableObject.SetValue(property, TV ?? Default);
            }
            else if (idiom == DeviceIdiom.Watch)
            {
                bindableObject.SetValue(property, Watch ?? Default);
            }
            else
            {
                bindableObject.SetValue(property, Default);
            }

            return bindableObject;
        }

        public static T OnDeviceWidth<T>(this T bindableObject, BindableProperty property, double MinWidth,
            object Default, object GreaterThanEq = null
        ) where T : BindableObject
        {
            if (DeviceDisplay.Current.MainDisplayInfo.Width >= MinWidth)
            {
                bindableObject.SetValue(property, GreaterThanEq ?? Default);
            }
            else
            {
                bindableObject.SetValue(property, Default);
            }

            return bindableObject;
        }
    }
}
