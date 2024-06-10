using CommunityToolkit.Maui.Markup;
using SimpleWeather.Icons;
using SimpleWeather.Utils;
using System;
using System.Linq;

namespace SimpleWeather.Maui.Preferences;

public partial class Settings_Credits : ContentPage
{
	public Settings_Credits()
	{
		InitializeComponent();
        Initialize();
        UpdateSettingsTableTheme();
    }

    private readonly Command HyperlinkButtonCommand = new(async (parameter) =>
    {
        if (parameter is string url)
        {
            try
            {
                await Launcher.TryOpenAsync(url);
            }
            catch { }
        }
        else if (parameter is Uri uri)
        {
            try
            {
                await Launcher.TryOpenAsync(uri);
            }
            catch { }
        }
    });

    private void Initialize()
    {
        IconCreditsContainer.Clear();

        var providers = SharedModule.Instance.WeatherIconsManager.GetIconProviders();

        foreach (var provider in providers)
        {
            var tvc = new TextViewCell()
            {
                Text = provider.Value.DisplayName,
                Detail = provider.Value.AuthorName,
                CommandParameter = provider.Value.AttributionLink,
                Command = HyperlinkButtonCommand
            };

            IconCreditsContainer.Add(tvc);
        }
    }

    private void UpdateSettingsTableTheme()
    {
        App.Current.Resources.TryGetValue("LightPrimary", out var LightPrimary);
        App.Current.Resources.TryGetValue("DarkPrimary", out var DarkPrimary);
        SettingsTable.UpdateCellColors(
            Colors.Black, Colors.White, Color.Parse("#767676"), Color.Parse("#a2a2a2"),
            LightPrimary as Color, DarkPrimary as Color);
    }
}