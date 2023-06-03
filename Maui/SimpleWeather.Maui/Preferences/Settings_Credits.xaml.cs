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
    }

    private Command HyperlinkButtonCommand = new Command(async (parameter) =>
    {
        if (parameter is string url)
        {
            try
            {
                await Launcher.TryOpenAsync(url);
            }
            catch { }
        }
    });

    private void Initialize()
    {
        IconCreditsContainer.Children.Clear();

        var providers = SharedModule.Instance.WeatherIconsManager.GetIconProviders();

        foreach (var provider in providers)
        {
            var textBlock = new VerticalStackLayout()
            {
                Padding = new Thickness(0, 10, 0, 10),
                HorizontalOptions = LayoutOptions.Start
            };

            var title = new Label()
            {
                FontSize = 16,
                Text = provider.Value.DisplayName
            };
            var subtitle = new Label()
            {
                FontSize = 13
            }.Apply(it =>
            {
                it.Text = provider.Value.AuthorName;

                if (provider.Value.AttributionLink != null)
                {
                    it.TapGesture(async () =>
                    {
                        try
                        {
                            await Launcher.TryOpenAsync(provider.Value.AttributionLink);
                        }
                        catch { }
                    });
                }
            });

            textBlock.Add(title);
            textBlock.Add(subtitle);

            IconCreditsContainer.Children.Add(textBlock);
        }
    }
}