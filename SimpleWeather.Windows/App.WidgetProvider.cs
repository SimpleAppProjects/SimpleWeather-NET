using SimpleWeather.NET.Widgets;
using WinRT;

namespace SimpleWeather.NET
{
    public partial class App
    {
        private static uint _WidgetProviderRegistrationToken;

        private static void RegisterWidgetProvider()
        {
            var widgetProviderFactory = new ClassFactory<WidgetProvider>(
                () => new WidgetProvider(),
                new Dictionary<Guid, Func<object, IntPtr>>()
                {
                    { typeof(WidgetProvider).GUID, obj => MarshalInterface<WidgetProvider>.FromManaged((WidgetProvider)obj) },
                });

            _WidgetProviderRegistrationToken = COMUtilities.RegisterClass<WidgetProvider>(widgetProviderFactory);
        }

        private static void UnregisterWidgetProvider()
        {
            COMUtilities.UnregisterClassObject(_WidgetProviderRegistrationToken);
        }
    }
}
