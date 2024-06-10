using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.Maui.Controls;
using SimpleWeather.Preferences;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.Maui.Preferences;

public partial class DevKeyEntryCell : TextViewCell
{
    public string API
    {
        get { return (string)GetValue(APIProperty); }
        set { SetValue(APIProperty, value); }
    }

    public static readonly BindableProperty APIProperty =
        BindableProperty.Create(nameof(API), typeof(string), typeof(DevKeyEntryCell), string.Empty, propertyChanged: (obj, _, _) => (obj as DevKeyEntryCell)?.UpdateKeyEntry());

    private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

    public void UpdateKeyEntry()
    {
        Detail = SettingsManager.APIKeys[API] ?? "null";
    }

    protected override void OnTapped()
    {
        base.OnTapped();
        ItemClicked();
    }

    private void ItemClicked()
    {
        var keyDialog = new KeyEntryPopup(API);

        keyDialog.PrimaryButtonClick += (s, e) =>
        {
            var diag = s as KeyEntryPopup;

            string key = diag.Key;
            Detail = key ?? string.Empty;
            SettingsManager.APIKeys[API] = key;

            diag.Close();
        };

        keyDialog.Show();
    }
}