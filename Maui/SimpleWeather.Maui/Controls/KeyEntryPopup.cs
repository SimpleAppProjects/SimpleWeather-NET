using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.DependencyInjection;
using CoreGraphics;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Platform;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API;
using SimpleWeather.Weather_API.WeatherData;
using SimpleWeather.WeatherData;
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

#if IOS
    private readonly UIWindow Window;
    private readonly UIAlertController AlertController;
#else
    private readonly Popup<string> _keyEntryPopup;
    private Entry KeyEntry1;
    private Entry KeyEntry2;
    private Button PrimaryButton;
    private Button SecondaryButton;
#endif

    public KeyEntryPopup(string APIProvider)
    {
        this.APIProvider = APIProvider;

#if IOS
        Window = new UIWindow { BackgroundColor = Colors.Transparent.ToPlatform() };
        AlertController =
 UIAlertController.Create(ResStrings.key_hint, ResStrings.message_enter_apikey, UIAlertControllerStyle.Alert);
#else
        _keyEntryPopup = new Popup<string>()
        {
            MinimumWidthRequest = (Shell.Current?.Window?.Width / 3) ?? -1,
            Content = new VerticalStackLayout()
            {
                Spacing = 12,
                Children =
                {
                    new Label()
                    {
                        Text = ResStrings.key_hint,
                        FontSize = 18,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalTextAlignment = TextAlignment.Center,
                        HorizontalOptions = LayoutOptions.Center
                    }.AppThemeColorBinding(Label.TextColorProperty, Colors.Black, Colors.White),
                    new Label()
                    {
                        Text = ResStrings.message_enter_apikey,
                        FontSize = 14,
                        HorizontalTextAlignment = TextAlignment.Center,
                        HorizontalOptions = LayoutOptions.Center
                    }.AppThemeColorBinding(Label.TextColorProperty, Colors.Black, Colors.White),
                    new VerticalStackLayout()
                    {
                        Spacing = 4,
                        Children =
                        {
                            new Entry()
                                {
                                    HorizontalOptions = LayoutOptions.Fill,
                                    IsVisible = false
                                }
                                .AppThemeColorBinding(Entry.TextColorProperty, Colors.Black, Colors.White)
                                .AppThemeColorBinding(Entry.PlaceholderColorProperty, Colors.Black.WithAlpha(0.75f),
                                    Colors.White.WithAlpha(0.75f))
                                .Apply(it => KeyEntry1 = it),
                            new Entry()
                                {
                                    HorizontalOptions = LayoutOptions.Fill,
                                    IsVisible = false
                                }
                                .AppThemeColorBinding(Entry.TextColorProperty, Colors.Black, Colors.White)
                                .AppThemeColorBinding(Entry.PlaceholderColorProperty, Colors.Black.WithAlpha(0.75f),
                                    Colors.White.WithAlpha(0.75f))
                                .Apply(it => KeyEntry2 = it)
                        }
                    },
                    new VerticalStackLayout()
                    {
                        Spacing = 2,
                        Children =
                        {
                            new Button
                                {
                                    HorizontalOptions = LayoutOptions.Fill,
                                }
                                .Apply(it =>
                                {
                                    PrimaryButton = it;
                                    it.Behaviors(
                                        new TouchBehavior()
                                            .Bind(
                                                TouchBehavior.HoveredBackgroundColorProperty,
                                                static src => src.BackgroundColor, mode: BindingMode.OneWay,
                                                source: it,
                                                convert: color => color.WithLuminosity(0.85f))
                                            .Bind(
                                                TouchBehavior.PressedBackgroundColorProperty,
                                                static src => src.BackgroundColor, mode: BindingMode.OneWay,
                                                source: it,
                                                convert: color => color.WithLuminosity(0.80f))
                                    );
                                }),
                            new Button
                                {
                                    HorizontalOptions = LayoutOptions.Fill,
                                }
                                .Apply(it =>
                                {
                                    SecondaryButton = it;
                                    it.Behaviors(
                                        new TouchBehavior()
                                            .Bind(
                                                TouchBehavior.HoveredBackgroundColorProperty,
                                                static src => src.BackgroundColor, mode: BindingMode.OneWay,
                                                source: it,
                                                convert: color => color.WithLuminosity(0.85f))
                                            .Bind(
                                                TouchBehavior.PressedBackgroundColorProperty,
                                                static src => src.BackgroundColor, mode: BindingMode.OneWay,
                                                source: it,
                                                convert: color => color.WithLuminosity(0.80f))
                                    );
                                }),
                        }
                    }
                }
            },
        }.AppThemeColorBinding(Popup.BackgroundColorProperty, App.Current.Resources["LightPopoverBackground"] as Color,
            App.Current.Resources["DarkPopoverBackground"] as Color);
#endif

        Initialize();
    }

    private void Initialize()
    {
#if IOS
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
#else
        PrimaryButton.Text = ResStrings.ConfirmDialog_PrimaryButtonText;
        PrimaryButton.Clicked += (s, e) => { PrimaryButtonClick?.Invoke(this, EventArgs.Empty); };

        SecondaryButton.Text = ResStrings.Label_Cancel;
        SecondaryButton.Clicked += (s, e) =>
        {
            if (SecondaryButtonClick != null)
            {
                SecondaryButtonClick?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                this.Close();
            }
        };
#endif

        try
        {
            AuthType = wm.GetAuthType(APIProvider);
        }
        catch
        {
        }

        var key = SettingsManager.APIKeys[APIProvider];
        ProviderKey = AuthType switch
        {
            AuthType.AppID_AppCode => new ProviderAppKey().Apply((it) => { key?.Let(input => it.FromString(input)); }),
            AuthType.Basic => new BasicAuthProviderKey().Apply((it) => { key?.Let(input => it.FromString(input)); }),
            _ => new ProviderApiKey().Apply((it) => { key?.Let(input => it.FromString(input)); }),
        };

        switch (AuthType)
        {
            case AuthType.AppID_AppCode:
            {
                var credentials = ProviderKey as ProviderAppKey;

#if IOS
                    AlertController.AddTextField((KeyEntry1) =>
                    {
#endif
                if (APIProvider == WeatherAPI.Here)
                {
                    KeyEntry1.Placeholder = "Access Key ID";
                }
                else
                {
                    KeyEntry1.Placeholder = ResStrings.hint_appid;
                }

                KeyEntry1.Text = credentials?.AppID ?? string.Empty;
#if IOS
                        KeyEntry1.Ended += KeyEntry1_TextChanged;
#else
                KeyEntry1.TextChanged += KeyEntry1_TextChanged;
                KeyEntry1.IsVisible = true;
#endif
#if IOS
                    });
                    AlertController.AddTextField((KeyEntry2) =>
                    {
#endif
                if (APIProvider == WeatherAPI.Here)
                {
                    KeyEntry2.Placeholder = "Access Key Secret";
                }
                else
                {
                    KeyEntry2.Placeholder = ResStrings.hint_appcode;
                }

                KeyEntry2.Text = credentials?.AppCode ?? string.Empty;
#if IOS
                        KeyEntry2.Ended += KeyEntry2_TextChanged;
#else
                KeyEntry2.TextChanged += KeyEntry2_TextChanged;
                KeyEntry2.IsVisible = true;
#endif
#if IOS
                    });
#endif
            }
                break;
            case AuthType.Basic:
            {
                var credentials = ProviderKey as BasicAuthProviderKey;

#if IOS
                    AlertController.AddTextField((KeyEntry1) =>
                    {
#endif
                KeyEntry1.Placeholder = ResStrings.hint_username;
                KeyEntry1.Text = credentials?.UserName ?? string.Empty;
#if IOS
                        KeyEntry1.Ended += KeyEntry1_TextChanged;
#else
                KeyEntry1.TextChanged += KeyEntry1_TextChanged;
                KeyEntry1.IsVisible = true;
#endif
#if IOS
                    });
                    AlertController.AddTextField((PasswordEntry) =>
                    {
#endif
#if IOS
                        PasswordEntry.SecureTextEntry = true;
                        PasswordEntry.Placeholder = ResStrings.hint_password;
                        PasswordEntry.Text = credentials?.Password ?? string.Empty;
                        PasswordEntry.Ended += PasswordEntry_TextChanged;
#else
                KeyEntry2.IsPassword = true;
                KeyEntry2.Placeholder = ResStrings.hint_password;
                KeyEntry2.Text = credentials?.Password ?? string.Empty;
                KeyEntry2.TextChanged += PasswordEntry_TextChanged;
                KeyEntry2.IsVisible = true;
#endif
#if IOS
                    });
#endif
            }
                break;
            default:
            {
#if IOS
                    AlertController.AddTextField((KeyEntry1) =>
                    {
#endif
                KeyEntry1.Placeholder = ResStrings.key_hint;
                KeyEntry1.Text = key ?? string.Empty;
#if IOS
                        KeyEntry1.Ended += KeyEntry1_TextChanged;
#else
                KeyEntry1.TextChanged += KeyEntry1_TextChanged;
                KeyEntry1.IsVisible = true;
                KeyEntry2.IsVisible = false;
#endif
#if IOS
                    });
#endif
            }
                break;
        }
    }

#if IOS
    private void KeyEntry1_TextChanged(object sender, EventArgs e)
#else
    private void KeyEntry1_TextChanged(object sender, TextChangedEventArgs e)
#endif
    {
#if IOS
        var KeyEntry1 = sender as UITextField;
#endif

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

#if IOS
    private void KeyEntry2_TextChanged(object sender, EventArgs e)
#else
    private void KeyEntry2_TextChanged(object sender, TextChangedEventArgs e)
#endif
    {
#if IOS
        var KeyEntry2 = sender as UITextField;
#endif

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

#if IOS
    private void PasswordEntry_TextChanged(object sender, EventArgs e)
#else
    private void PasswordEntry_TextChanged(object sender, TextChangedEventArgs e)
#endif
    {
#if IOS
        var PasswordEntry = sender as UITextField;
#else
        var PasswordEntry = KeyEntry2;
#endif

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
#if IOS
        Window.RootViewController = new UIViewController();
        Window.RootViewController.View.BackgroundColor = Colors.Transparent.ToPlatform();
        Window.WindowLevel = UIWindowLevel.Alert + 1;
        Window.MakeKeyAndVisible();

        Window.RootViewController.PresentViewController(AlertController, true, null);
#else
        Shell.Current.ShowPopup(_keyEntryPopup, new PopupOptions()
        {
            Shape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(20),
                Stroke = Colors.Gray,
                StrokeThickness = 0.5
            }
        });
#endif
    }

    public void Close()
    {
#if IOS
        Window.Hidden = true;
#else
        _keyEntryPopup.CloseAsync();
#endif
    }
}