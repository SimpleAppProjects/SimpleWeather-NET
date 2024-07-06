using Microsoft.Windows.Widgets.Providers;

namespace SimpleWeather.NET.Widgets
{
    internal delegate WidgetImplBase WidgetCreateDelegate(string widgetId, string initialState);
    internal abstract class WidgetImplBase
    {
        protected string id;
        protected string state;
        protected bool isActivated = false;
        protected bool inCustomization = false;

        protected WidgetImplBase(string widgetId, string initialState)
        {
            id = widgetId;
            state = initialState;
        }

        public string Id => id;
        public string State => state;
        public bool IsActivated => isActivated;

        public virtual void Activate(WidgetContext context) { }
        public virtual void Deactivate() { }
        public virtual void OnActionInvoked(WidgetActionInvokedArgs actionInvokedArgs) { }
        public virtual void OnWidgetContextChanged(WidgetContextChangedArgs contextChangedArgs) { }
        public virtual void OnCustomizationRequested(WidgetContext context) { }

        public abstract string GetTemplateForWidget();
        public abstract string GetCustomizationTemplateForWidget();
        public abstract string GetDataForWidget();
    }
}
