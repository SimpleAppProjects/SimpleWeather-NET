using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.Preferences;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.Maui.Controls;

public partial class DevKeyEntry : ContentView
{

    public string API
    {
        get { return (string)GetValue(APIProperty); }
        set { SetValue(APIProperty, value); }
    }

    public static readonly BindableProperty APIProperty =
        BindableProperty.Create(nameof(API), typeof(string), typeof(DevKeyEntry), string.Empty, propertyChanged: (obj, _, _) => (obj as DevKeyEntry)?.UpdateKeyEntry());

    private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

    public DevKeyEntry()
    {
        InitializeComponent();
    }

    public void UpdateKeyEntry()
    {
        KeyEntryTextBlock.Text = SettingsManager.APIKeys[API] ?? ResStrings.key_hint;
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        ItemClicked();
    }

    private void ClickGestureRecognizer_Clicked(object sender, EventArgs e)
    {
#if WINDOWS || MACCATALYST
        ItemClicked();
#endif
    }

    private void ItemClicked()
    {
        var keyDialog = new KeyEntryPopup(API);

        keyDialog.PrimaryButtonClick += (s, e) =>
        {
            var diag = s as KeyEntryPopup;

            string key = diag.Key;
            KeyEntryTextBlock.Text = key ?? string.Empty;
            SettingsManager.APIKeys[API] = key;

            diag.Close();
        };

        App.Current.CurrentPage?.ShowPopup(keyDialog);
    }
}