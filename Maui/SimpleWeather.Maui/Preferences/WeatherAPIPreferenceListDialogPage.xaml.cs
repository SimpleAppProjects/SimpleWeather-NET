using SimpleWeather.Utils;

namespace SimpleWeather.Maui.Preferences;

public partial class WeatherAPIPreferenceListDialogPage : PreferenceListDialogPage
{
    public WeatherAPIPreferenceListDialogPage(ListViewCell listPreference) : base(listPreference)
    {
        try {
            this.InitializeComponent();

            if (this.Resources.TryGetValue("WeatherAPIItemTemplate", out var template)) {
                this._PreferenceListView.ItemTemplate = template as DataTemplate;
            }
        } catch (Exception e) {
            Logger.WriteLine(LoggerLevel.Error, e);
        }
    }
}