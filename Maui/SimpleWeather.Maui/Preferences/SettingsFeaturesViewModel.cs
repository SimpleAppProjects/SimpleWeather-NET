using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SimpleWeather.ComponentModel;
using SimpleWeather.NET.Utils;
using SimpleWeather.Utils;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.Maui.Preferences;

[Bindable(true)]
public partial class SettingsFeaturesViewModel : BaseViewModel
{
    [ObservableProperty] private List<FeatureGroup> features;

    private readonly List<Feature> OrderableFeatures;
    private readonly List<Feature> NonOrderableFeatures;
    private readonly IDictionary<string, Feature> OrderableFeaturesMap;

    public ICommand ResetCommand { get; }

    public SettingsFeaturesViewModel()
    {
        OrderableFeatures =
        [
            new Feature { Key = FeatureSettings.KEY_FORECAST, Title = ResStrings.pref_title_feature_forecast }
                .Bind(Feature.IsEnabledProperty, static vm => vm.Forecast, static (vm, v) => vm.Forecast = v,
                    source: this, mode: BindingMode.TwoWay),

            new Feature { Key = FeatureSettings.KEY_HRFORECAST, Title = ResStrings.pref_title_feature_hrforecast }
                .Bind(Feature.IsEnabledProperty, static vm => vm.HourlyForecast,
                    static (vm, v) => vm.HourlyForecast = v, source: this, mode: BindingMode.TwoWay),

            new Feature { Key = FeatureSettings.KEY_CHARTS, Title = ResStrings.pref_title_feature_charts }
                .Bind(Feature.IsEnabledProperty, static vm => vm.Charts, static (vm, v) => vm.Charts = v, source: this,
                    mode: BindingMode.TwoWay),

            new Feature { Key = FeatureSettings.KEY_WEATHERDETAILS, Title = ResStrings.pref_title_feature_details }
                .Bind(Feature.IsEnabledProperty, static vm => vm.WeatherDetails,
                    static (vm, v) => vm.WeatherDetails = v, source: this, mode: BindingMode.TwoWay),

            new Feature { Key = FeatureSettings.KEY_UVINDEX, Title = ResStrings.pref_title_feature_uv }
                .Bind(Feature.IsEnabledProperty, static vm => vm.UV, static (vm, v) => vm.UV = v, source: this,
                    mode: BindingMode.TwoWay),

            new Feature { Key = FeatureSettings.KEY_BEAUFORT, Title = ResStrings.pref_title_feature_beaufort }
                .Bind(Feature.IsEnabledProperty, static vm => vm.Beaufort, static (vm, v) => vm.Beaufort = v,
                    source: this, mode: BindingMode.TwoWay),

            new Feature { Key = FeatureSettings.KEY_AQINDEX, Title = ResStrings.pref_title_feature_aqindex }
                .Bind(Feature.IsEnabledProperty, static vm => vm.AQIndex, static (vm, v) => vm.AQIndex = v,
                    source: this, mode: BindingMode.TwoWay),

            new Feature { Key = FeatureSettings.KEY_POLLEN, Title = ResStrings.label_pollen_count }
                .Bind(Feature.IsEnabledProperty, static vm => vm.PollenEnabled, static (vm, v) => vm.PollenEnabled = v,
                    source: this, mode: BindingMode.TwoWay),

            new Feature { Key = FeatureSettings.KEY_MOONPHASE, Title = ResStrings.pref_title_feature_moonphase }
                .Bind(Feature.IsEnabledProperty, static vm => vm.MoonPhase, static (vm, v) => vm.MoonPhase = v,
                    source: this, mode: BindingMode.TwoWay),

            new Feature { Key = FeatureSettings.KEY_SUNPHASE, Title = ResStrings.pref_title_feature_sunphase }
                .Bind(Feature.IsEnabledProperty, static vm => vm.SunPhase, static (vm, v) => vm.SunPhase = v,
                    source: this, mode: BindingMode.TwoWay),

            new Feature { Key = FeatureSettings.KEY_RADAR, Title = ResStrings.pref_title_feature_radar }
                .Bind(Feature.IsEnabledProperty, static vm => vm.WeatherRadar, static (vm, v) => vm.WeatherRadar = v,
                    source: this, mode: BindingMode.TwoWay)
        ];

        NonOrderableFeatures =
        [
            new Feature
                {
                    Key = FeatureSettings.KEY_BGIMAGE, Title = ResStrings.pref_title_feature_bgimage, CanReorder = false
                }
                .Bind(Feature.IsEnabledProperty, static vm => vm.BackgroundImage,
                    static (vm, v) => vm.BackgroundImage = v, source: this, mode: BindingMode.TwoWay),

            new Feature
                {
                    Key = FeatureSettings.KEY_WEATHERSUMMARY, Title = ResStrings.pref_title_feature_summary,
                    CanReorder = false
                }
                .Bind(Feature.IsEnabledProperty, static vm => vm.WeatherSummary,
                    static (vm, v) => vm.WeatherSummary = v, source: this, mode: BindingMode.TwoWay),

            new Feature
                {
                    Key = FeatureSettings.KEY_LOCPANELBGIMAGE, Title = ResStrings.pref_title_feature_locpanelimg,
                    CanReorder = false
                }
                .Bind(Feature.IsEnabledProperty, static vm => vm.LocationPanelBackgroundImage,
                    static (vm, v) => vm.LocationPanelBackgroundImage = v, source: this, mode: BindingMode.TwoWay)
        ];

        OrderableFeaturesMap = OrderableFeatures.ToDictionary(f => f.Key);

        var userOrderedFeatures = FeatureSettings.GetFeatureOrder()?.Intersect(OrderableFeaturesMap.Keys)
            ?.Select(key => OrderableFeaturesMap[key]);

        Features = new List<FeatureGroup>
        {
            new FeatureGroup(userOrderedFeatures ?? OrderableFeatures, true),
            new FeatureGroup(NonOrderableFeatures, false)
        };

        ResetCommand = new RelayCommand(() =>
        {
            // Reset
            Features = new List<FeatureGroup>
            {
                new FeatureGroup(OrderableFeatures, true),
                new FeatureGroup(NonOrderableFeatures, false)
            };

            FeatureSettings.SetFeatureOrder(null);

            // Re-enable all features
            OrderableFeatures.Union(NonOrderableFeatures).ForEach(f => { f.IsEnabled = true; });
        });
    }
}