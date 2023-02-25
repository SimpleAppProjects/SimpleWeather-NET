using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.Common.Controls;
using SimpleWeather.Icons;
using SimpleWeather.Preferences;

namespace SimpleWeather.Maui.Controls;

public partial class DetailItem : ContentView
{
    public DetailItemViewModel Details
    {
        get { return (this.BindingContext as DetailItemViewModel); }
    }

    private readonly WeatherIconsManager wim;
    private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

    public bool UseMonochrome
    {
        get { return (bool)GetValue(UseMonochromeProperty); }
        set { SetValue(UseMonochromeProperty, value); }
    }

    public static readonly BindableProperty UseMonochromeProperty =
        BindableProperty.Create(nameof(UseMonochrome), typeof(bool), typeof(DetailItem), false);

    public DetailItem()
    {
        this.InitializeComponent();
        wim = SharedModule.Instance.WeatherIconsManager;
        this.BindingContextChanged += (sender, args) =>
        {
            ApplyBindings();
            IconCtrl.ShowAsMonochrome = wim.ShouldUseMonochrome(IconCtrl.IconProvider ?? SettingsManager.IconProvider);
        };
    }
}
