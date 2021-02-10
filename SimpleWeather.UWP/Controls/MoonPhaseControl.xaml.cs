using SimpleWeather.Controls;
using SimpleWeather.Icons;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using muxc = Microsoft.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.UWP.Controls
{
    public sealed partial class MoonPhaseControl : UserControl
    {
        public MoonPhaseViewModel ViewModel
        {
            get { return (this.DataContext as MoonPhaseViewModel); }
        }

        public MoonPhaseControl()
        {
            this.InitializeComponent();
            this.DataContextChanged += (sender, args) =>
            {
                this.Bindings.Update();

                if (ViewModel != null)
                {
                    switch (ViewModel.PhaseType)
                    {
                        case MoonPhase.MoonPhaseType.NewMoon:
                            foreach (BitmapIcon icon in MoonStack.Children)
                            {
                                if (icon.Tag == WeatherIcons.MOON_NEW)
                                    icon.Style = (Style)Resources["MoonStyle"];
                                else
                                    icon.Style = (Style)Resources["DisabledMoonStyle"];
                            }
                            break;
                        case MoonPhase.MoonPhaseType.WaxingCrescent:
                            foreach (BitmapIcon icon in MoonStack.Children)
                            {
                                if (icon.Tag == WeatherIcons.MOON_WAXING_CRESCENT_3)
                                    icon.Style = (Style)Resources["MoonStyle"];
                                else
                                    icon.Style = (Style)Resources["DisabledMoonStyle"];
                            }
                            break;
                        case MoonPhase.MoonPhaseType.FirstQtr:
                            foreach (BitmapIcon icon in MoonStack.Children)
                            {
                                if (icon.Tag == WeatherIcons.MOON_FIRST_QUARTER)
                                    icon.Style = (Style)Resources["MoonStyle"];
                                else
                                    icon.Style = (Style)Resources["DisabledMoonStyle"];
                            }
                            break;
                        case MoonPhase.MoonPhaseType.WaxingGibbous:
                            foreach (BitmapIcon icon in MoonStack.Children)
                            {
                                if (icon.Tag == WeatherIcons.MOON_WAXING_GIBBOUS_3)
                                    icon.Style = (Style)Resources["MoonStyle"];
                                else
                                    icon.Style = (Style)Resources["DisabledMoonStyle"];
                            }
                            break;
                        case MoonPhase.MoonPhaseType.FullMoon:
                            foreach (BitmapIcon icon in MoonStack.Children)
                            {
                                if (icon.Tag == WeatherIcons.MOON_FULL)
                                    icon.Style = (Style)Resources["MoonStyle"];
                                else
                                    icon.Style = (Style)Resources["DisabledMoonStyle"];
                            }
                            break;
                        case MoonPhase.MoonPhaseType.WaningGibbous:
                            foreach (BitmapIcon icon in MoonStack.Children)
                            {
                                if (icon.Tag == WeatherIcons.MOON_WANING_GIBBOUS_3)
                                    icon.Style = (Style)Resources["MoonStyle"];
                                else
                                    icon.Style = (Style)Resources["DisabledMoonStyle"];
                            }
                            break;
                        case MoonPhase.MoonPhaseType.LastQtr:
                            foreach (BitmapIcon icon in MoonStack.Children)
                            {
                                if (icon.Tag == WeatherIcons.MOON_THIRD_QUARTER)
                                    icon.Style = (Style)Resources["MoonStyle"];
                                else
                                    icon.Style = (Style)Resources["DisabledMoonStyle"];
                            }
                            break;
                        case MoonPhase.MoonPhaseType.WaningCrescent:
                            foreach (BitmapIcon icon in MoonStack.Children)
                            {
                                if (icon.Tag == WeatherIcons.MOON_WANING_CRESCENT_3)
                                    icon.Style = (Style)Resources["MoonStyle"];
                                else
                                    icon.Style = (Style)Resources["DisabledMoonStyle"];
                            }
                            break;
                    }
                }
            };
        }
    }
}
