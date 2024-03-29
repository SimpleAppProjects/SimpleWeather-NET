﻿using SimpleWeather.Common.Controls;
using SimpleWeather.Icons;
using SimpleWeather.WeatherData;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.NET.Controls
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
                            foreach (IconControl icon in MoonStack.Children)
                            {
                                if (Equals(icon.WeatherIcon, WeatherIcons.MOON_NEW))
                                    icon.Style = (Style)Resources["MoonStyle"];
                                else
                                    icon.Style = (Style)Resources["DisabledMoonStyle"];
                                icon.UpdateWeatherIcon();
                            }
                            break;
                        case MoonPhase.MoonPhaseType.WaxingCrescent:
                            foreach (IconControl icon in MoonStack.Children)
                            {
                                if (Equals(icon.WeatherIcon, WeatherIcons.MOON_WAXING_CRESCENT_3))
                                    icon.Style = (Style)Resources["MoonStyle"];
                                else
                                    icon.Style = (Style)Resources["DisabledMoonStyle"];
                                icon.UpdateWeatherIcon();
                            }
                            break;
                        case MoonPhase.MoonPhaseType.FirstQtr:
                            foreach (IconControl icon in MoonStack.Children)
                            {
                                if (Equals(icon.WeatherIcon, WeatherIcons.MOON_FIRST_QUARTER))
                                    icon.Style = (Style)Resources["MoonStyle"];
                                else
                                    icon.Style = (Style)Resources["DisabledMoonStyle"];
                                icon.UpdateWeatherIcon();
                            }
                            break;
                        case MoonPhase.MoonPhaseType.WaxingGibbous:
                            foreach (IconControl icon in MoonStack.Children)
                            {
                                if (Equals(icon.WeatherIcon, WeatherIcons.MOON_WAXING_GIBBOUS_3))
                                    icon.Style = (Style)Resources["MoonStyle"];
                                else
                                    icon.Style = (Style)Resources["DisabledMoonStyle"];
                                icon.UpdateWeatherIcon();
                            }
                            break;
                        case MoonPhase.MoonPhaseType.FullMoon:
                            foreach (IconControl icon in MoonStack.Children)
                            {
                                if (Equals(icon.WeatherIcon, WeatherIcons.MOON_FULL))
                                    icon.Style = (Style)Resources["MoonStyle"];
                                else
                                    icon.Style = (Style)Resources["DisabledMoonStyle"];
                                icon.UpdateWeatherIcon();
                            }
                            break;
                        case MoonPhase.MoonPhaseType.WaningGibbous:
                            foreach (IconControl icon in MoonStack.Children)
                            {
                                if (Equals(icon.WeatherIcon, WeatherIcons.MOON_WANING_GIBBOUS_3))
                                    icon.Style = (Style)Resources["MoonStyle"];
                                else
                                    icon.Style = (Style)Resources["DisabledMoonStyle"];
                                icon.UpdateWeatherIcon();
                            }
                            break;
                        case MoonPhase.MoonPhaseType.LastQtr:
                            foreach (IconControl icon in MoonStack.Children)
                            {
                                if (Equals(icon.WeatherIcon, WeatherIcons.MOON_THIRD_QUARTER))
                                    icon.Style = (Style)Resources["MoonStyle"];
                                else
                                    icon.Style = (Style)Resources["DisabledMoonStyle"];
                                icon.UpdateWeatherIcon();
                            }
                            break;
                        case MoonPhase.MoonPhaseType.WaningCrescent:
                            foreach (IconControl icon in MoonStack.Children)
                            {
                                if (Equals(icon.WeatherIcon, WeatherIcons.MOON_WANING_CRESCENT_3))
                                    icon.Style = (Style)Resources["MoonStyle"];
                                else
                                    icon.Style = (Style)Resources["DisabledMoonStyle"];
                                icon.UpdateWeatherIcon();
                            }
                            break;
                    }

                    MoonriseIcon.UpdateWeatherIcon();
                    MoonsetIcon.UpdateWeatherIcon();
                }
            };
        }
    }
}
