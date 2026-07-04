// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using SimpleWeather.NET.Utils;
using Windows.ApplicationModel;
using WinUIEx;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SimpleWeather.NET
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : WindowEx
    {
        public static new Window Current { get; internal set; }

        public MainWindow()
        {
            PersistenceId = "MainWindow";
            SystemBackdrop = new MicaBackdrop();

            MinHeight = 500;
            MinWidth = 500;
            Title = ResStrings.app_name;
            this.SetIconFromFilesystem(Path.Combine(Package.Current.InstalledLocation.Path, "Assets/logo.ico"));
        }
    }
}
