using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.Extras;
using SimpleWeather.Icons;
using SimpleWeather.Maui.Controls;

namespace SimpleWeather.Maui.Preferences;

public partial class IconRadioPreference : ContentView
{
    private readonly String[] PREVIEW_ICONS =
    {
        WeatherIcons.DAY_SUNNY, WeatherIcons.NIGHT_CLEAR, WeatherIcons.DAY_SUNNY_OVERCAST,
        WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY, WeatherIcons.RAIN
    };

    public String Key { get; private set; }

    private IExtrasService ExtrasService = Ioc.Default.GetService<IExtrasService>();

    private bool IsPremiumIcon
    {
        get => (bool)GetValue(IsPremiumIconProperty);
        set => SetValue(IsPremiumIconProperty, value);
    }

    private static readonly BindableProperty IsPremiumIconProperty =
        BindableProperty.Create(nameof(IsPremiumIcon), typeof(bool), typeof(IconRadioPreference), false);

    public IconRadioPreference()
    {
        InitializeComponent();
    }

    public IconRadioPreference(WeatherIconProvider provider) : this()
    {
        SetIconProvider(provider);
    }

    public event EventHandler RadioButtonChecked;

    public bool IsChecked
    {
        get { return PreferenceRadioButton.IsChecked; }
        set { PreferenceRadioButton.IsChecked = value; }
    }

    public void SetIconProvider(WeatherIconProvider provider)
    {
        IconPreference.Text = provider.DisplayName;
        Key = provider.Key;
        IsPremiumIcon = !ExtrasService.IsIconPackSupported(Key);

        IconContainer.Children.Clear();
        foreach (var icon in PREVIEW_ICONS)
        {
            IconContainer.Children.Add(new IconControl()
            {
                IconHeight = 30,
                IconWidth = 30,
                Margin = new Thickness(5, 0, 5, 0),
                IconProvider = provider.Key,
                WeatherIcon = icon
            });
        }
    }

    private void PreferenceRadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        RadioButtonChecked?.Invoke(this, e);
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        RadioButtonChecked?.Invoke(this, e);
    }

    private void ClickGestureRecognizer_Clicked(object sender, EventArgs e)
    {
#if WINDOWS || MACCATALYST
        RadioButtonChecked?.Invoke(this, e);
#endif
    }
}