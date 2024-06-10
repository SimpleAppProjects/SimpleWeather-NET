using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.DependencyInjection;
using CoreGraphics;
using Microsoft.Maui.Platform;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API;
using SimpleWeather.Weather_API.WeatherData;
using SimpleWeather.WeatherData.Auth;
using UIKit;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.Maui.Controls;

public class KeyEntryPopup
{
    public string Key => ProviderKey?.ToString();
    public string APIProvider { get; private set; }

    private AuthType AuthType { get; set; } = AuthType.ApiKey;
    private ProviderKey ProviderKey { get; set; }

    private readonly WeatherProviderManager wm = WeatherModule.Instance.WeatherManager;
    private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

    public event EventHandler PrimaryButtonClick;
    public event EventHandler SecondaryButtonClick;

    private readonly UIWindow Window;
    private readonly UIAlertController AlertController;

    public KeyEntryPopup(string APIProvider)
    {
        this.APIProvider = APIProvider;

        Window = new UIWindow { BackgroundColor = Colors.Transparent.ToPlatform() };
        AlertController = UIAlertController.Create(ResStrings.key_hint, ResStrings.message_enter_apikey, UIAlertControllerStyle.Alert);

        Initialize();
    }

    private void Initialize()
    {
        var oldFrame = AlertController.View.Frame;
        AlertController.View.Frame = new CGRect(oldFrame.X, oldFrame.Y, oldFrame.Width, oldFrame.Height - 20);

        AlertController.AddAction(UIAlertAction.Create(ResStrings.Label_Cancel, UIAlertActionStyle.Cancel, (action) =>
        {
            if (SecondaryButtonClick != null)
            {
                SecondaryButtonClick?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                this.Close();
            }
        }));
        AlertController.AddAction(UIAlertAction.Create(ResStrings.ConfirmDialog_PrimaryButtonText, UIAlertActionStyle.Default, (action) =>
        {
            PrimaryButtonClick?.Invoke(this, EventArgs.Empty);
        }));

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

                    AlertController.AddTextField((KeyEntry1) =>
                    {
                        KeyEntry1.Placeholder = ResStrings.hint_appid;
                        KeyEntry1.Text = credentials?.AppID ?? string.Empty;
                        KeyEntry1.Ended += KeyEntry1_TextChanged;
                    });
                    AlertController.AddTextField((KeyEntry2) =>
                    {
                        KeyEntry2.Placeholder = ResStrings.hint_appcode;
                        KeyEntry2.Text = credentials?.AppCode ?? string.Empty;
                        KeyEntry2.Ended += KeyEntry2_TextChanged;
                    });
                }
                break;
            case AuthType.Basic:
                {
                    var credentials = ProviderKey as BasicAuthProviderKey;

                    AlertController.AddTextField((KeyEntry1) =>
                    {
                        KeyEntry1.Placeholder = ResStrings.hint_username;
                        KeyEntry1.Text = credentials?.UserName ?? string.Empty;
                        KeyEntry1.Ended += KeyEntry1_TextChanged;
                    });
                    AlertController.AddTextField((PasswordEntry) =>
                    {
                        PasswordEntry.SecureTextEntry = true;
                        PasswordEntry.Placeholder = ResStrings.hint_password;
                        PasswordEntry.Text = credentials?.Password ?? string.Empty;
                        PasswordEntry.Ended += PasswordEntry_TextChanged;
                    });
                }
                break;
            default:
                {
                    AlertController.AddTextField((KeyEntry1) =>
                    {
                        KeyEntry1.Placeholder = ResStrings.key_hint;
                        KeyEntry1.Text = key ?? string.Empty;
                        KeyEntry1.Ended += KeyEntry1_TextChanged;
                    });
                }
                break;
        }
    }

    private void KeyEntry1_TextChanged(object sender, EventArgs e)
    {
        var KeyEntry1 = sender as UITextField;

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

    private void KeyEntry2_TextChanged(object sender, EventArgs e)
    {
        var KeyEntry2 = sender as UITextField;

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

    private void PasswordEntry_TextChanged(object sender, EventArgs e)
    {
        var PasswordEntry = sender as UITextField;

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

    public void Show()
    {
        Window.RootViewController = new UIViewController();
        Window.RootViewController.View.BackgroundColor = Colors.Transparent.ToPlatform();
        Window.WindowLevel = UIWindowLevel.Alert + 1;
        Window.MakeKeyAndVisible();

        Window.RootViewController.PresentViewController(AlertController, true, null);
    }

    public void Close()
    {
        Window.Hidden = true;
    }
}

internal static class UITextFieldExtensions
{
    internal static void Hide(this UITextField textField)
    {
        textField.Hidden = true;
        textField.Frame = new CGRect(textField.Frame.X, textField.Frame.Y, textField.Frame.Width, 0);
    }

    internal static void Show(this UITextField textField)
    {
        textField.Frame = new CGRect(textField.Frame.X, textField.Frame.Y, textField.Frame.Width, -1);
        textField.Hidden = false;
    }
}