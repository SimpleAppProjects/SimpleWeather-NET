using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.Common.Controls;
using SimpleWeather.Icons;
using SimpleWeather.Preferences;
namespace SimpleWeather.Maui.Controls;

public partial class UVControl : ContentView
{
    public UVIndexViewModel ViewModel
    {
        get { return (this.BindingContext as UVIndexViewModel); }
    }

    private readonly WeatherIconsManager wim;

    public bool UseMonochrome
    {
        get { return (bool)GetValue(UseMonochromeProperty); }
        set { SetValue(UseMonochromeProperty, value); }
    }

    public static readonly BindableProperty UseMonochromeProperty =
        BindableProperty.Create(nameof(UseMonochrome), typeof(bool), typeof(UVControl), false);

    private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

    public UVControl()
    {
        this.InitializeComponent();
        wim = SharedModule.Instance.WeatherIconsManager;
        this.BindingContextChanged += (sender, args) =>
        {
            ApplyBindings();
            UVIcon.ShowAsMonochrome = wim.ShouldUseMonochrome(UVIcon.IconProvider ?? SettingsManager.IconProvider);
        };
    }
}
