using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API;
using SimpleWeather.Weather_API.WeatherData;
using SimpleWeather.WeatherData.Auth;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.Maui.Controls;

public partial class KeyEntryPopup : Popup
{
    public string Key => ProviderKey?.ToString();
    public string APIProvider { get; private set; }

    private AuthType AuthType { get; set; } = AuthType.ApiKey;
    private ProviderKey ProviderKey { get; set; }

    private readonly WeatherProviderManager wm = WeatherModule.Instance.WeatherManager;
    private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

    public event EventHandler PrimaryButtonClick;
    public event EventHandler SecondaryButtonClick;

    public KeyEntryPopup(string APIProvider)
    {
        this.APIProvider = APIProvider;
        Initialize();
    }

    private void Initialize()
    {
        InitializeComponent();
        KeyEntry1.TextChanged += KeyEntry1_TextChanged;
        KeyEntry2.TextChanged += KeyEntry2_TextChanged;
        PasswordEntry.TextChanged += PasswordEntry_TextChanged;
        UpdateAuthType();
    }

    private void UpdateAuthType()
    {
        AuthType = wm.GetAuthType(APIProvider);
        var key = SettingsManager.APIKeys[APIProvider];
        ProviderKey = AuthType switch
        {
            AuthType.AppID_AppCode => new ProviderAppKey().Apply((it) =>
            {
                key?.Let(input => it.FromString(input));
            }),
            AuthType.Basic => new BasicAuthProviderKey().Apply((it) =>
            {
                key?.Let(input => it.FromString(input));
            }),
            _ => new ProviderApiKey().Apply((it) =>
            {
                key?.Let(input => it.FromString(input));
            }),
        };

        switch (AuthType)
        {
            case AuthType.AppID_AppCode:
                {
                    var credentials = ProviderKey as ProviderAppKey;

                    KeyEntry1.Placeholder = ResStrings.hint_appid;
                    KeyEntry1.Text = credentials?.AppID ?? string.Empty;
                    KeyEntry1.IsVisible = true;

                    KeyEntry2.Placeholder = ResStrings.hint_appcode;
                    KeyEntry2.Text = credentials?.AppCode ?? string.Empty;
                    KeyEntry1.IsVisible = true;

                    PasswordEntry.IsVisible = false;
                }
                break;
            case AuthType.Basic:
                {
                    var credentials = ProviderKey as BasicAuthProviderKey;

                    KeyEntry1.Placeholder = ResStrings.hint_username;
                    KeyEntry1.Text = credentials?.UserName ?? string.Empty;
                    KeyEntry1.IsVisible = true;

                    KeyEntry2.IsVisible = false;

                    PasswordEntry.Text = credentials?.Password ?? string.Empty;
                    PasswordEntry.IsVisible = true;
                }
                break;
            default:
                {
                    KeyEntry1.Placeholder = ResStrings.key_hint;
                    KeyEntry1.Text = key ?? string.Empty;
                    KeyEntry1.IsVisible = true;

                    KeyEntry2.IsVisible = false;

                    PasswordEntry.IsVisible = false;
                }
                break;
        }
    }

    private void KeyEntry1_TextChanged(object sender, TextChangedEventArgs e)
    {
        switch (AuthType)
        {
            case AuthType.AppID_AppCode:
                {
                    if (ProviderKey is ProviderAppKey credentials)
                    {
                        credentials.AppID = KeyEntry1.Text ?? string.Empty;
                    }
                }
                break;
            case AuthType.Basic:
                {
                    if (ProviderKey is BasicAuthProviderKey credentials)
                    {
                        credentials.UserName = KeyEntry1.Text ?? string.Empty;
                    }
                }
                break;
            default:
                {
                    if (ProviderKey is ProviderApiKey credentials)
                    {
                        credentials.Key = KeyEntry1.Text ?? string.Empty;
                    }
                }
                break;
        }
    }

    private void KeyEntry2_TextChanged(object sender, TextChangedEventArgs e)
    {
        switch (AuthType)
        {
            case AuthType.AppID_AppCode:
                {
                    if (ProviderKey is ProviderAppKey credentials)
                    {
                        credentials.AppCode = KeyEntry2.Text ?? string.Empty;
                    }
                }
                break;
        }
    }

    private void PasswordEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        switch (AuthType)
        {
            case AuthType.Basic:
                {
                    if (ProviderKey is BasicAuthProviderKey credentials)
                    {
                        credentials.Password = PasswordEntry.Text ?? string.Empty;
                    }
                }
                break;
        }
    }

    private void PrimaryButton_Clicked(object sender, EventArgs e)
    {
        PrimaryButtonClick?.Invoke(this, e);
    }

    private void SecondaryButton_Clicked(object sender, EventArgs e)
    {
        if (SecondaryButtonClick != null)
        {
            SecondaryButtonClick?.Invoke(this, e);
        }
        else
        {
            this.Close(false);
        }
    }
}