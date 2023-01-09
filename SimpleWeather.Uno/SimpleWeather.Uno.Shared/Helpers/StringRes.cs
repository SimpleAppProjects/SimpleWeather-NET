using Microsoft.UI.Xaml.Markup;
using Windows.ApplicationModel.Resources;

namespace SimpleWeather.Uno.Helpers
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

            return ResourceLoader.GetForViewIndependentUse();
        }
    }
}
