using SimpleWeather.NET.Utils;

namespace SimpleWeather.Maui.Preferences;

public partial class SettingsFeaturesViewModel
{
    #region Settings Properties

    public bool BackgroundImage
    {
        get => FeatureSettings.BackgroundImage;
        set => FeatureSettings.BackgroundImage = value;
    }

    public bool Forecast
    {
        get => FeatureSettings.Forecast;
        set => FeatureSettings.Forecast = value;
    }

    public bool HourlyForecast
    {
        get => FeatureSettings.HourlyForecast;
        set => FeatureSettings.HourlyForecast = value;
    }

    public bool Charts
    {
        get => FeatureSettings.Charts;
        set => FeatureSettings.Charts = value;
    }

    public bool WeatherSummary
    {
        get => FeatureSettings.WeatherSummary;
        set => FeatureSettings.WeatherSummary = value;
    }

    public bool WeatherDetails
    {
        get => FeatureSettings.WeatherDetails;
        set => FeatureSettings.WeatherDetails = value;
    }

    public bool UV
    {
        get => FeatureSettings.UV;
        set => FeatureSettings.UV = value;
    }

    public bool Beaufort
    {
        get => FeatureSettings.Beaufort;
        set => FeatureSettings.Beaufort = value;
    }

    public bool AQIndex
    {
        get => FeatureSettings.AQIndex;
        set => FeatureSettings.AQIndex = value;
    }

    public bool MoonPhase
    {
        get => FeatureSettings.MoonPhase;
        set => FeatureSettings.MoonPhase = value;
    }

    public bool SunPhase
    {
        get => FeatureSettings.SunPhase;
        set => FeatureSettings.SunPhase = value;
    }

    public bool WeatherRadar
    {
        get => FeatureSettings.WeatherRadar;
        set => FeatureSettings.WeatherRadar = value;
    }

    public bool DetailsEnabled => WeatherDetails || ExtraDetailsEnabled;
    public bool ExtraDetailsEnabled => UV || Beaufort || AQIndex || MoonPhase;

    public bool LocationPanelBackgroundImage
    {
        get => FeatureSettings.LocationPanelBackgroundImage;
        set => FeatureSettings.LocationPanelBackgroundImage = value;
    }

    public bool PollenEnabled
    {
        get => FeatureSettings.PollenEnabled;
        set => FeatureSettings.PollenEnabled = value;
    }

    #endregion
}