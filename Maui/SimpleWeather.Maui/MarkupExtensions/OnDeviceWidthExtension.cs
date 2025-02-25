using System;
using System.Globalization;
using System.Reflection;
using SimpleWeather.Maui.Helpers;

namespace SimpleWeather.Maui.MarkupExtensions
{
    [ContentProperty(nameof(Default))]
    [RequireService([typeof(IProvideValueTarget), typeof(IXmlLineInfoProvider)])]
    public class OnDeviceWidthExtension : IMarkupExtension
    {
        private static readonly object s_notset = new();

        public double MinWidth { get; set; } = 0;

        public object Default { get; set; } = s_notset;
        public object GreaterThanEq { get; set; } = s_notset;

        public IValueConverter Converter { get; set; }

        public object ConverterParameter { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (GreaterThanEq == s_notset
                && Default == s_notset)
            {
                throw new XamlParseException("OnDeviceWidthExtension requires a value to be specified for at least width or Default");
            }

            // TODO: move value conversion to base class
            var valueProvider = serviceProvider?.GetService<IProvideValueTarget>() ?? throw new ArgumentException();

            BindableProperty bp;
            PropertyInfo pi = null;
            Type propertyType = null;

            if (valueProvider.TargetObject is Setter setter)
            {
                bp = setter.Property;
            }
            else
            {
                bp = valueProvider.TargetProperty as BindableProperty;
                pi = valueProvider.TargetProperty as PropertyInfo;
            }
            propertyType = bp?.ReturnType
                              ?? pi?.PropertyType
                              ?? throw new InvalidOperationException("Cannot determine property to provide the value for.");

            var value = GetValue();

            if (value == null && propertyType.IsValueType)
                return Activator.CreateInstance(propertyType);

            if (Converter != null)
                return Converter.Convert(value, propertyType, ConverterParameter, CultureInfo.CurrentUICulture);

            var ret = value.ConvertTo(propertyType, () => pi, serviceProvider, out Exception exception);

            if (exception != null)
                throw exception;

            return ret;
        }

        private object GetValue()
        {
            if (DeviceDisplay.Current.MainDisplayInfo.Width >= MinWidth && GreaterThanEq != s_notset)
            {
                return GreaterThanEq;
            }

            return Default;
        }
    }
}

