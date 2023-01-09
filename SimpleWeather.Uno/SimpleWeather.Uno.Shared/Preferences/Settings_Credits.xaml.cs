using SimpleWeather.Icons;
using System;
using System.Linq;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SimpleWeather.Uno.Preferences
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Settings_Credits : Page
    {
        public Settings_Credits()
        {
            this.InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            IconCreditsContainer.Children.Clear();

            var providers = SharedModule.Instance.WeatherIconsManager.GetIconProviders();

            foreach (var provider in providers)
            {
                var textBlock = new RichTextBlock()
                {
                    FontSize = 16,
                    Padding = new Microsoft.UI.Xaml.Thickness(0, 10, 0, 10),
                    HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Left
                };

                var title = new Paragraph();
                title.Inlines.Add(new Run()
                {
                    Text = provider.Value.DisplayName
                });
                var subtitle = new Paragraph() { FontSize = 13 };
                if (provider.Value.AttributionLink != null)
                {
                    var link = new Hyperlink()
                    {
                        NavigateUri = provider.Value.AttributionLink,
                    };
                    link.Inlines.Add(new Run()
                    {
                        Text = provider.Value.AuthorName
                    });
                    subtitle.Inlines.Add(link);
                }
                else
                {
                    subtitle.Inlines.Add(new Run()
                    {
                        Text = provider.Value.AuthorName
                    });
                }

                textBlock.Blocks.Add(title);
                textBlock.Blocks.Add(subtitle);

                IconCreditsContainer.Children.Add(textBlock);
            }
        }
    }
}
