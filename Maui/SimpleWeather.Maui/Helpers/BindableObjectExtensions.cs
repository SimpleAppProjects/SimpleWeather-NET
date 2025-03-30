using System.Linq.Expressions;
using CommunityToolkit.Maui.Markup;

namespace SimpleWeather.Maui.Helpers
{
    public static class BindableObjectExtensions
    {
#nullable enable
        public static TBindable SetOneWayBinding<TBindable, TBindingContext, TSource>(
            this TBindable bindable,
            BindableProperty targetProperty,
            Expression<Func<TBindingContext, TSource>> getter,
            TBindingContext? source = default)
            where TBindable : BindableObject
            where TBindingContext : class?
        {
            return bindable.Bind(targetProperty, getter, mode: BindingMode.OneWay, source: source);
        }
#nullable restore

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
