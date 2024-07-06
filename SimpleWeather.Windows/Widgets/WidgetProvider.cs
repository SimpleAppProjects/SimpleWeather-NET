using Microsoft.Windows.Widgets.Providers;
using SimpleWeather.NET.BackgroundTasks;
using SimpleWeather.Utils;
using System.Runtime.InteropServices;

namespace SimpleWeather.NET.Widgets
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("1D453922-87B3-41AB-9D61-1A73C4360E71")]
    internal partial class WidgetProvider : IWidgetProvider, IWidgetProvider2, IWidgetProviderAnalytics, IWidgetProviderErrors
    {
        private static bool HaveRecoveredWidgets { get; set; } = false;

        private static readonly Dictionary<string, WidgetCreateDelegate> WidgetImpls = new()
        {
            [WeatherWidget.DefinitionId] = (widgetId, initialState) => new WeatherWidget(widgetId, initialState)
        };

        private static readonly Dictionary<string, WidgetImplBase> WidgetInstances = new();

        public WidgetProvider()
        {
            RecoverRunningWidgets();
        }

        private static void RecoverRunningWidgets()
        {
            if (!HaveRecoveredWidgets)
            {
                try
                {
                    var widgetManager = WidgetManager.GetDefault();

                    var widgetInfos = widgetManager.GetWidgetInfos();

                    if (widgetInfos == null) return;

                    foreach (var widgetInfo in widgetInfos)
                    {
                        var context = widgetInfo.WidgetContext;
                        if (!WidgetInstances.ContainsKey(context.Id))
                        {
                            if (WidgetImpls.TryGetValue(context.DefinitionId, out WidgetCreateDelegate widget))
                            {
                                // Need to recover this instance
                                WidgetInstances[context.Id] = widget(context.Id, widgetInfo.CustomState);
                            }
                            else
                            {
                                // this provider doesn't know about this type of Widget (any more?) delete it
                                WidgetUtils.DeleteWidget(context.Id);
                                widgetManager.DeleteWidget(context.Id);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(LoggerLevel.Error, ex);
                }
                finally
                {
                    HaveRecoveredWidgets = true;
                }
            }
        }

        // Handle the CreateWidget call. During this function call you should store
        // the WidgetId value so you can use it to update corresponding widget.
        // It is our way of notifying you that the user has pinned your widget
        // and you should start pushing updates.
        public void CreateWidget(WidgetContext widgetContext)
        {
            if (!WidgetImpls.ContainsKey(widgetContext.DefinitionId))
            {
                Logger.WriteLine(LoggerLevel.Debug, $"{nameof(WidgetProvider)} - ERROR: Requested unknown Widget Definition ${widgetContext.DefinitionId}");
                return;
            }

            if (WidgetInstances.ContainsKey(widgetContext.Id))
            {
                Logger.WriteLine(LoggerLevel.Debug, $"{nameof(WidgetProvider)} - WARN: Widget already initialized. Def - ${widgetContext.DefinitionId}; Id - ${widgetContext.Id}");
                return;
            }

            var widgetInstance = WidgetImpls[widgetContext.DefinitionId](widgetContext.Id, "");
            WidgetInstances[widgetContext.Id] = widgetInstance;

            var options = new WidgetUpdateRequestOptions(widgetContext.Id)
            {
                Template = widgetInstance.GetTemplateForWidget(),
                Data = widgetInstance.GetDataForWidget(),
                CustomState = widgetInstance.State
            };

            WidgetManager.GetDefault().UpdateWidget(options);

            // Schedule updates
            Task.Run(() => WeatherTileUpdaterTask.RegisterBackgroundTask(false));
        }

        // Handle the DeleteWidget call. This is notifying you that
        // you don't need to provide new content for the given WidgetId
        // since the user has unpinned the widget or it was deleted by the Host
        // for any other reason.
        public void DeleteWidget(string widgetId, string _)
        {
            WidgetInstances.Remove(widgetId);
            WidgetUtils.DeleteWidget(widgetId);

            var widgetIds = WidgetManager.GetDefault().GetWidgetIds();
            if (widgetIds?.Length > 0)
            {
                // still contain widgets
            }
            else
            {
                // no more widgets activated
                Task.Run(WeatherTileUpdaterTask.UnregisterBackgroundTask);
            }
        }

        // Handle the OnActionInvoked call. This function call is fired when the user's
        // interaction with the widget resulted in an execution of one of the defined
        // actions that you've indicated in the template of the Widget.
        // For example: clicking a button or submitting input.
        public void OnActionInvoked(WidgetActionInvokedArgs actionInvokedArgs)
        {
            WidgetInstances.GetValueOrDefault(actionInvokedArgs.WidgetContext.Id)?.OnActionInvoked(actionInvokedArgs);
        }

        // Handle the WidgetContextChanged call. This function is called when the context a widget
        // has changed. Currently it only signals that the user has changed the size of the widget.
        // There are 2 ways to respond to this event:
        // 1) Call UpdateWidget() with the new data/template to fit the new requested size.
        // 2) If previously sent data/template accounts for various sizes based on $host.widgetSize - you can use this event solely for telemtry.
        public void OnWidgetContextChanged(WidgetContextChangedArgs contextChangedArgs)
        {
            WidgetInstances.GetValueOrDefault(contextChangedArgs.WidgetContext.Id)?.OnWidgetContextChanged(contextChangedArgs);
        }

        // Handle the Activate call. This function is called when widgets host starts listening
        // to the widget updates. It generally happens when the widget becomes visible and the updates
        // will be promptly displayed to the user. It's recommended to start sending updates from this moment
        // until Deactivate function was called.
        public void Activate(WidgetContext widgetContext)
        {
            if (!WidgetInstances.TryGetValue(widgetContext.Id, out WidgetImplBase widgetImpl))
            {
                Logger.WriteLine(LoggerLevel.Debug, $"{nameof(WidgetProvider)} - WARN: Activate called for unknown id. Attempting to recover... Def - ${widgetContext.DefinitionId}; Id - ${widgetContext.Id}");
                CreateWidget(widgetContext);
                return;
            }

            widgetImpl.Activate(widgetContext);
        }

        // Handle the Deactivate call. This function is called when widgets host stops listening
        // to the widget updates. It generally happens when the widget is not visible to the user
        // anymore and any further updates won't be displayed until the widget is visible again.
        // It's recommended to stop sending updates until Activate function was called.
        public void Deactivate(string widgetId)
        {
            WidgetInstances.GetValueOrDefault(widgetId)?.Deactivate();
        }

        public void OnCustomizationRequested(WidgetCustomizationRequestedArgs args)
        {
            if (WidgetInstances.TryGetValue(args.WidgetContext.Id, out WidgetImplBase widgetImpl))
            {
                widgetImpl.OnCustomizationRequested(args.WidgetContext);
            }
        }

        public void OnAnalyticsInfoReported(WidgetAnalyticsInfoReportedArgs args)
        {

        }

        public void OnErrorInfoReported(WidgetErrorInfoReportedArgs args)
        {
            Logger.WriteLine(LoggerLevel.Debug, $"{nameof(WidgetProvider)} - Widget Error: ${args.ErrorJson}");
        }
    }
}
