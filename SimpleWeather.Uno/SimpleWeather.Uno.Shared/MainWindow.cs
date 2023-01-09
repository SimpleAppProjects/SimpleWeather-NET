// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SimpleWeather.Uno
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow
#if !HAS_UNO
        : Window
#endif
    {
#if WINDOWS
        public static new Window Current { get; internal set; }
#else
        public static Window Current => Window.Current;
#endif

        public MainWindow()
        {
#if WINDOWS
            TrySetSystemBackdrop();
#endif
        }
    }
}
