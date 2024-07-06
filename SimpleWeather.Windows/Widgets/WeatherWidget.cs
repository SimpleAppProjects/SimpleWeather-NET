using Microsoft.Windows.Widgets.Providers;

namespace SimpleWeather.NET.Widgets
{
    internal partial class WeatherWidget : WidgetImplBase
    {
#if DEBUG
        public static string DefinitionId { get; } = "Weather_Widget_Debug";
#else
        public static string DefinitionId { get; } = "Weather_Widget";
#endif
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

        public override string GetCustomizationTemplateForWidget()
        {
            return GetDefaultTemplate();
        }

        public override void Activate(WidgetContext context)
        {
            Task.Run(async () =>
            {
                var widgetId = context.Id;
                var widgetMgr = WidgetManager.GetDefault();
                var widgetInfo = widgetMgr.GetWidgetInfo(widgetId);

                if (inCustomization)
                {
                    await WidgetUpdateHelper.CustomizeWidget(widgetInfo, widgetMgr, widgetId);
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(widgetInfo.Template))
                    {
                        await WidgetUpdateHelper.RefreshWidget(widgetInfo, widgetMgr, widgetId);
                    }
                }
            });
        }

        public override void OnActionInvoked(WidgetActionInvokedArgs args)
        {
            switch (args.Verb)
            {
                case "done":
                    {
                        try
                        {
                            var data = System.Text.Json.JsonDocument.Parse(args.Data);
                            var widgetLocation = data.RootElement.GetProperty("widgetLocation");
                            var newKey = widgetLocation.GetString();

                            var oldKey = args.CustomState ?? WidgetUtils.GetWidgetKey(args.WidgetContext.Id);
                            inCustomization = false;

                            Task.Run(async () =>
                            {
                                var widgetId = args.WidgetContext.Id;
                                var widgetMgr = WidgetManager.GetDefault();
                                var widgetInfo = widgetMgr.GetWidgetInfo(widgetId);

                                if (Equals(newKey, Constants.KEY_GPS))
                                {
                                    await PrepareGPSWidget(widgetId);
                                }
                                else
                                {
                                    await PrepareSearchWidget(newKey, widgetId);
                                }

                                await WidgetUpdateHelper.RefreshWidget(widgetInfo, widgetMgr, widgetId);
                            });
                        }
                        catch { }
                    }
                    break;
            }
        }

        public override void OnCustomizationRequested(WidgetContext context)
        {
            Task.Run(async () =>
            {
                var widgetId = context.Id;
                var widgetMgr = WidgetManager.GetDefault();
                var widgetInfo = widgetMgr.GetWidgetInfo(widgetId);

                await WidgetUpdateHelper.CustomizeWidget(widgetInfo, widgetMgr, widgetId);
            });
        }

        private Task PrepareGPSWidget(string widgetId)
        {
            // Save data for widget
            WidgetUtils.DeleteWidget(widgetId);
            WidgetUtils.SaveLocationData(widgetId, null);
            WidgetUtils.AddWidgetId(Constants.KEY_GPS, widgetId);

            return Task.CompletedTask;
        }

        private async Task PrepareSearchWidget(string query, string widgetId)
        {
            LocationData.LocationData locData = null;

            if (WidgetUtils.IdExists(widgetId))
            {
                locData = WidgetUtils.GetLocationData(widgetId);
            }

            if (locData == null || !Equals(query, locData.query))
            {
                var favorites = await DI.Utils.SettingsManager.GetFavorites();
                locData = favorites?.FirstOrDefault(l => Equals(l.query, query));
            }

            if (locData != null)
            {
                // Save data for widget
                WidgetUtils.DeleteWidget(widgetId);
                WidgetUtils.SaveLocationData(widgetId, locData);
                WidgetUtils.AddWidgetId(locData.query, widgetId);
            }
        }
    }
}
