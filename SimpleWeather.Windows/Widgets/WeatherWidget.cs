using Microsoft.Windows.Widgets.Providers;

namespace SimpleWeather.NET.Widgets
{
    internal partial class WeatherWidget : WidgetImplBase
    {
        public static string DefinitionId { get; } = "Weather_Widget";
        public WeatherWidget(string widgetId, string initialState) : base(widgetId, initialState) { }

        private static string WidgetTemplate { get; set; } = "";

        private static string GetDefaultTemplate()
        {
            return WidgetTemplate;
        }

        public override string GetTemplateForWidget()
        {
            return GetDefaultTemplate();
        }

        public override string GetDataForWidget()
        {
            // Return empty JSON since we don't have any data that we want to use.
            return "{}";
        }

        public override void Activate(WidgetContext context)
        {
            Task.Run(async () =>
            {
                var widgetId = context.Id;
                var widgetMgr = WidgetManager.GetDefault();
                var widgetInfo = widgetMgr.GetWidgetInfo(widgetId);

                if (string.IsNullOrWhiteSpace(widgetInfo.Template))
                {
                    await WidgetUpdateHelper.RefreshWidget(widgetInfo, widgetMgr, widgetId);
                }
            });
        }

        public override void OnActionInvoked(WidgetActionInvokedArgs actionInvokedArgs)
        {
            switch (actionInvokedArgs.Verb)
            {

            }
        }
    }
}
