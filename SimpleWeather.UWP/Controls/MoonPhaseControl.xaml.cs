using SimpleWeather.Controls;
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
                            foreach (TextBlock block in MoonStack.Children)
                            {
                                if (block.Text == WeatherIcons.MOON_NEW)
                                    block.Opacity = 1;
                                else
                                    block.Opacity = 0.25;
                            }
                            break;
                        case MoonPhase.MoonPhaseType.WaxingCrescent:
                            foreach (TextBlock block in MoonStack.Children)
                            {
                                if (block.Text == WeatherIcons.MOON_WAXING_CRESCENT_3)
                                    block.Opacity = 1;
                                else
                                    block.Opacity = 0.25;
                            }
                            break;
                        case MoonPhase.MoonPhaseType.FirstQtr:
                            foreach (TextBlock block in MoonStack.Children)
                            {
                                if (block.Text == WeatherIcons.MOON_FIRST_QUARTER)
                                    block.Opacity = 1;
                                else
                                    block.Opacity = 0.25;
                            }
                            break;
                        case MoonPhase.MoonPhaseType.WaxingGibbous:
                            foreach (TextBlock block in MoonStack.Children)
                            {
                                if (block.Text == WeatherIcons.MOON_WAXING_GIBBOUS_3)
                                    block.Opacity = 1;
                                else
                                    block.Opacity = 0.25;
                            }
                            break;
                        case MoonPhase.MoonPhaseType.FullMoon:
                            foreach (TextBlock block in MoonStack.Children)
                            {
                                if (block.Text == WeatherIcons.MOON_FULL)
                                    block.Opacity = 1;
                                else
                                    block.Opacity = 0.25;
                            }
                            break;
                        case MoonPhase.MoonPhaseType.WaningGibbous:
                            foreach (TextBlock block in MoonStack.Children)
                            {
                                if (block.Text == WeatherIcons.MOON_WANING_GIBBOUS_3)
                                    block.Opacity = 1;
                                else
                                    block.Opacity = 0.25;
                            }
                            break;
                        case MoonPhase.MoonPhaseType.LastQtr:
                            foreach (TextBlock block in MoonStack.Children)
                            {
                                if (block.Text == WeatherIcons.MOON_THIRD_QUARTER)
                                    block.Opacity = 1;
                                else
                                    block.Opacity = 0.25;
                            }
                            break;
                        case MoonPhase.MoonPhaseType.WaningCrescent:
                            foreach (TextBlock block in MoonStack.Children)
                            {
                                if (block.Text == WeatherIcons.MOON_WANING_CRESCENT_3)
                                    block.Opacity = 1;
                                else
                                    block.Opacity = 0.25;
                            }
                            break;
                    }
                }
            };
        }
    }
}
