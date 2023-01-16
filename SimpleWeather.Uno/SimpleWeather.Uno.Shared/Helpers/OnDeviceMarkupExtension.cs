using Microsoft.UI.Xaml.Markup;
using Windows.System.Profile;

namespace SimpleWeather.Uno.Helpers
{
    public class OnDevice : MarkupExtension
    {
        private string DeviceForm { get; } = AnalyticsInfo.DeviceForm;

        public object Desktop { get; set; }
        public object Mobile { get; set; }
        public object Tablet { get; set; }
        public object Default { get; set; }

        protected override object ProvideValue()
        {
            return DeviceForm switch
            {
                "Mobile" or "Phone" => Mobile ?? Default,
                "Tablet" => Tablet ?? Default,
                "Notebook" or "Desktop" => Desktop ?? Default,
                _ => Default,
            };
        }
    }
}
