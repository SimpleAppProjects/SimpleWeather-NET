using Microsoft.UI.Xaml.Navigation;
using SimpleWeather.NET.Main;
using SimpleWeather.NET.Radar;
using SimpleWeather.Resources.Strings;
using ResBackgrounds = SimpleWeather.Backgrounds.Resources.Strings.Backgrounds;
using ResExtras = SimpleWeather.Extras.Resources.Strings.Extras;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.NET
{
    public partial class App
    {
        private void UpdateAppLocale()
        {
            var locale = SimpleWeather.Utils.LocaleUtils.GetLocale();

            ResStrings.Culture = locale;
            ResBackgrounds.Culture = locale;
            ResExtras.Culture = locale;
            AQIndex.Culture = locale;
            Beaufort.Culture = locale;
            Config.Culture = locale;
            ConfigiOS.Culture = locale;
            MoonPhases.Culture = locale;
            Units.Culture = locale;
            UVIndex.Culture = locale;
            WeatherConditions.Culture = locale;
            AQIndex.Culture = locale;
        }

        internal void RefreshAppShell()
        {
            var currentPageType = Shell.Instance.AppFrame.CurrentSourcePageType;

            var navigationStack = Shell.Instance.AppFrame.BackStack.Skip(1).ToList();
            navigationStack.Add(new PageStackEntry(currentPageType, null, null));

            // Note: Workaround: destroy MapControl before replacing shell
            MapControlCreator.Instance?.RemoveMapControl();

            RootFrame.Content = null;
            RootFrame.Navigate(typeof(Shell));

            foreach (var entry in navigationStack)
            {
                Shell.Instance.AppFrame.Navigate(entry.SourcePageType, entry.Parameter, entry.NavigationTransitionInfo);
            }
        }
    }
}

