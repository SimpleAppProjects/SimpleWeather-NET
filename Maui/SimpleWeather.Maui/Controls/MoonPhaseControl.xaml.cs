using SimpleWeather.Common.Controls;
using SimpleWeather.Icons;
using SimpleWeather.WeatherData;

namespace SimpleWeather.Maui.Controls;

public partial class MoonPhaseControl : ContentView
{
    public MoonPhaseViewModel ViewModel
    {
        get { return (this.BindingContext as MoonPhaseViewModel); }
    }

    public MoonPhaseControl()
    {
        this.InitializeComponent();
        this.BindingContextChanged += (sender, args) =>
        {
            ApplyBindings();

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
        this.SizeChanged += (sender, args) =>
        {
            var margins = 0;
            var size = 36;

            if (this.Width >= 480)
            {
                margins = 5;
                size = 48;
            }

            foreach (View view in MoonStack.Children)
            {
                view.HeightRequest = view.WidthRequest = size;
                view.Margin = new Thickness(margins);
            }
        };
    }
}
