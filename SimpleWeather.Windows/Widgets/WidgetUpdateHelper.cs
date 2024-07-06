using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Windows.Widgets.Providers;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;

namespace SimpleWeather.NET.Widgets
{
    public static class WidgetUpdateHelper
    {
        public static bool WidgetsExist()
        {
            try
            {
                var widgetIds = WidgetManager.GetDefault().GetWidgetIds();
                return widgetIds?.Any() == true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static Task RefreshWidgets()
        {
            return Task.Run(async () =>
            {
                var tasks = new List<Task>();

                var widgetManager = WidgetManager.GetDefault();
                var widgets = widgetManager.GetWidgetInfos() ?? Array.Empty<WidgetInfo>();

                foreach (var widget in widgets)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        await RefreshWidget(widget, widgetManager, widget.WidgetContext.Id);
                    }));
                }

                await Task.WhenAll(tasks);
            });
        }

        private static async Task ResetWidget(WidgetInfo widgetInfo, WidgetManager widgetManager, string widgetId)
        {
            var settingsMgr = Ioc.Default.GetService<SettingsManager>();

            if (settingsMgr.FollowGPS)
            {
                WidgetUtils.DeleteWidget(widgetId);
                WidgetUtils.SaveLocationData(widgetId, null);
                WidgetUtils.AddWidgetId(Constants.KEY_GPS, widgetId);
                await RefreshWidget(widgetInfo, widgetManager, widgetId);
            }
            else
            {
                var location = await settingsMgr.GetHomeData();

                if (location != null)
                {
                    WidgetUtils.DeleteWidget(widgetId);
                    WidgetUtils.SaveLocationData(widgetId, location);
                    WidgetUtils.AddWidgetId(location.query, widgetId);
                    await RefreshWidget(widgetInfo, widgetManager, widgetId);
                }
                else
                {
                    // Show error message
                }
            }
        }

        public static async Task ResetGPSWidgets()
        {
            var widgetIds = WidgetUtils.GetWidgetIds(Constants.KEY_GPS);
            await ResetGPSWidgets(widgetIds);
        }

        internal static async Task ResetGPSWidgets(IList<string> widgetIds)
        {
            var widgetManager = WidgetManager.GetDefault();
            var tasks = new List<Task>();

            foreach (var widgetId in widgetIds)
            {
                tasks.Add(Task.Run(async () =>
                {
                    var widgetInfo = widgetManager.GetWidgetInfo(widgetId);

                    if (widgetInfo != null)
                    {
                        await ResetWidget(widgetInfo, widgetManager, widgetId);
                    }
                }));
            }

            await Task.WhenAll(tasks);
        }

        public static async Task RefreshWidgets(string locationQuery)
        {
            var widgetManager = WidgetManager.GetDefault();
            var widgetIds = WidgetUtils.GetWidgetIds(locationQuery);
            var tasks = new List<Task>();

            foreach (var widgetId in widgetIds)
            {
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        var widgetInfo = widgetManager.GetWidgetInfo(widgetId);

                        if (widgetInfo != null)
                        {
                            await RefreshWidget(widgetInfo, widgetManager, widgetId);
                        }
                    }
                    catch { }
                }));
            }

            await Task.WhenAll(tasks);
        }

        public static async Task RefreshWidget(WidgetInfo widgetInfo, WidgetManager widgetManager, string widgetId)
        {
            var creator = WidgetUtils.GetWidgetCreator(widgetId);
            var template = await creator.BuildUpdate(widgetId, widgetInfo);
            var data = await creator.BuildWidgetData(widgetId, widgetInfo);

            if (template != null)
            {
                widgetManager.UpdateWidget(new WidgetUpdateRequestOptions(widgetId)
                {
                    Template = template,
                    Data = data,
                    CustomState = WidgetUtils.GetWidgetKey(widgetId)
                });
            }
            else
            {
                Logger.WriteLine(LoggerLevel.Debug, $"{nameof(WidgetUpdateHelper)}: widget: {widgetInfo.WidgetContext.DefinitionId}; widgetId: {widgetId}; Template not provided");
                await ResetWidget(widgetInfo, widgetManager, widgetId);
            }
        }

        public static async Task CustomizeWidget(WidgetInfo widgetInfo, WidgetManager widgetManager, string widgetId)
        {
            var creator = WidgetUtils.GetWidgetCreator(widgetId);
            var template = await creator.BuildCustomization(widgetId, widgetInfo);
            var data = await creator.BuildCustomizeData(widgetId, widgetInfo);

            widgetManager.UpdateWidget(new WidgetUpdateRequestOptions(widgetId)
            {
                Template = template,
                Data = data,
                CustomState = WidgetUtils.GetWidgetKey(widgetId)
            });
        }
    }
}
