using SimpleWeather.NET.Widgets;
using WinRT;

namespace SimpleWeather.NET
{
    public partial class App
    {
        private ClassFactory<WidgetProvider> widgetProviderFactory;

        private void RegisterWidgetProvider()
        {
            this.widgetProviderFactory = new ClassFactory<WidgetProvider>(
                () => new WidgetProvider(),
                new Dictionary<Guid, Func<object, IntPtr>>()
                {
                    { typeof(WidgetProvider).GUID, obj => MarshalInterface<WidgetProvider>.FromManaged((WidgetProvider)obj) },
                });

            COMUtilities.RegisterClass<WidgetProvider>(this.widgetProviderFactory);
        }
    }
}
