using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Markup;

namespace SimpleWeather.UWP.Helpers
{
    [MarkupExtensionReturnType(ReturnType = typeof(string))]
    public class StringRes : MarkupExtension
    {
        public string Name { get; set; }

        protected override object ProvideValue()
        {
            return GetResourceLoader().GetString(Name);
        }

        private ResourceLoader GetResourceLoader()
        {
            try
            {
                if (App.Current.ResLoader != null)
                {
                    return App.Current.ResLoader;
                }
            }
            catch { }

            if (Windows.UI.Core.CoreWindow.GetForCurrentThread() != null)
            {
                return ResourceLoader.GetForCurrentView();
            }
            else
            {
                return ResourceLoader.GetForViewIndependentUse();
            }
        }
    }
}
