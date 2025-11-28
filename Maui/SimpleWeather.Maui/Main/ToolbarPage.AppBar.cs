using CommunityToolkit.Maui.Markup;
using SimpleWeather.Common.ViewModels;
using SimpleWeather.Maui.Controls.AppBar;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.Maui.Main;

public static class ToolbarPage
{
    public static View CreateTitleView(this ContentPage page, string title, WeatherNowViewModel WNowViewModel)
    {
        App.Current.Resources.TryGetValue("LightOnSurface", out var LightOnSurface);
        App.Current.Resources.TryGetValue("DarkOnSurface", out var DarkOnSurface);

        return new Grid()
        {
            MinimumHeightRequest = 52,
            Children =
            {
                new VerticalStackLayout()
                {
                    Margin = new Thickness(40, 0),
                    VerticalOptions = LayoutOptions.Center,
                    Children =
                    {
                        // Title
                        new Label()
                            {
                                HorizontalOptions = LayoutOptions.Fill,
                                HorizontalTextAlignment = TextAlignment.Center,
                                VerticalOptions = LayoutOptions.End,
                                VerticalTextAlignment = TextAlignment.End,
                                MaxLines = 1,
                                LineBreakMode = LineBreakMode.TailTruncation,
                                FontSize = AppBarSizes.GetDefaultFontSize(),
                                FontFamily = AppBarSizes.GetDefaultFontFamly(),
                                FontAttributes = FontAttributes.Bold,
                                CharacterSpacing = 0,
                                Padding = new Thickness(8, 0),
                                Text = title
                            }
                            .AppThemeColorBinding(Label.TextColorProperty, LightOnSurface as Color,
                                DarkOnSurface as Color),
                        // Location Name
                        new Label()
                            {
                                HorizontalOptions = LayoutOptions.Center,
                                HorizontalTextAlignment = TextAlignment.Center,
                                VerticalOptions = LayoutOptions.Center,
                                VerticalTextAlignment = TextAlignment.Center,
                                MaxLines = 1,
                                LineBreakMode = LineBreakMode.TailTruncation,
                                FontSize = 13,
                                CharacterSpacing = 0.00714286
                            }
                            .Bind(Label.TextProperty, getter: static viewModel => viewModel.Weather,
                                convert: weather => weather?.Location,
                                mode: BindingMode.OneWay, source: WNowViewModel
                            )
                            .AppThemeColorBinding(Label.TextColorProperty, LightOnSurface as Color,
                                DarkOnSurface as Color)
                    }
                }
            }
        };
    }
}