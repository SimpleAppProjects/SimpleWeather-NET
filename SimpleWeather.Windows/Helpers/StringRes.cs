using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml.Markup;
using SimpleWeather.NET.Localization;

namespace SimpleWeather.NET.Helpers
{
    [MarkupExtensionReturnType(ReturnType = typeof(string))]
    public partial class StringRes : MarkupExtension
    {
        private readonly CustomStringLocalizer localizer = Ioc.Default.GetService<CustomStringLocalizer>();

        public string Name { get; set; }

        protected override object ProvideValue()
        {
            return localizer[Name]?.ToString();
        }
    }
}
