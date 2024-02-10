using System;
namespace SimpleWeather.Maui.MarkupExtensions
{
    [ContentProperty(nameof(Default))]
    [AcceptEmptyServiceProvider]
    public class OnDeviceWidthExtension : IMarkupExtension
    {
        private static readonly object s_notset = new();

        public double MinWidth { get; set; } = 0;

        public object Default { get; set; } = s_notset;
        public object GreaterThanEq { get; set; } = s_notset;

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (GreaterThanEq == s_notset
                && Default == s_notset)
            {
                throw new XamlParseException("OnDeviceWidthExtension requires a value to be specified for at least width or Default");
            }

            if (DeviceDisplay.Current.MainDisplayInfo.Width >= MinWidth && GreaterThanEq != s_notset)
            {
                return GreaterThanEq;
            }

            return Default;
        }
    }
}

