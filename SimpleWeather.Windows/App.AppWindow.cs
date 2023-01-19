using Microsoft.UI.Xaml;
using SimpleWeather.NET.Utils;
using System.IO;
using Windows.ApplicationModel;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.NET
{
    public sealed partial class App
    {
        private static void InitializeWindow(Window _window)
        {
            _window.SetMinSize(new Windows.Foundation.Size(500, 500));
            _window.SetIconFromFilesystem(Path.Combine(Package.Current.InstalledLocation.Path, "Assets/logo.ico"));
            _window.Title = ResStrings.app_name;
        }
    }
}
