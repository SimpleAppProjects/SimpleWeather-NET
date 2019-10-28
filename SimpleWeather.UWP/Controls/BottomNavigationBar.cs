using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace SimpleWeather.UWP.Controls
{
    public partial class BottomNavigationBar : ListViewBase
    {
        public BottomNavigationBar()
        {
            DefaultStyleKey = typeof(BottomNavigationBar);
            CanDragItems = false;
            CanReorderItems = false;
            SelectionMode = ListViewSelectionMode.Single;
            RegisterPropertyChangedCallback(BackgroundProperty, BottomNavigationBar_PropertyChangedCallback);
        }

        private void BottomNavigationBar_PropertyChangedCallback(DependencyObject sender, DependencyProperty property)
        {
            if (BackgroundProperty == property)
            {
                if (Background is SolidColorBrush colorBrush && ColorUtils.IsSuperLight(colorBrush.Color) ||
                    Background is RevealBackgroundBrush revealBrush && ColorUtils.IsSuperLight(revealBrush.Color))
                {
                    RequestedTheme = ElementTheme.Light;
                }
                else
                {
                    RequestedTheme = ElementTheme.Dark;
                }
            }
        }
    }
}
