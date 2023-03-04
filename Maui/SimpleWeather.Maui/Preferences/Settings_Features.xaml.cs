namespace SimpleWeather.Maui.Preferences;

public partial class Settings_Features : ContentPage
{
	public Settings_Features()
	{
		InitializeComponent();
	}

    private void FeatureSetting_Tapped(object sender, TappedEventArgs e)
    {
		var view = sender as VisualElement;
		var checkboxElement = view.GetVisualTreeDescendants()?.FirstOrDefault(e => e.GetType() == typeof(CheckBox));
		if (checkboxElement is CheckBox checkbox)
		{
			checkbox.IsChecked = !checkbox.IsChecked;
		}
    }
}