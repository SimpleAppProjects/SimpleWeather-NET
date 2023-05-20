using AdaptiveCards.ObjectModel.WinUI3;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Windows.Widgets.Providers;
using SimpleWeather.Common.Controls;
using SimpleWeather.Icons;
using SimpleWeather.NET.Widgets.Model;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API;
using Windows.Data.Json;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.NET.Widgets.Templates
{
    internal class WeatherWidgetCreator : AbstractWidgetCreator
    {
        private const int MAX_FORECAST_LENGTH = 5;

        public override Task<string> BuildUpdate(string widgetId, WidgetInfo info)
        {
            if (widgetId == null || info == null || !(WidgetUtils.IdExists(widgetId) || WidgetUtils.IsGPS(widgetId))) return Task.FromResult<string>(null);

            return Task.FromResult(new AdaptiveCard()
            {
                VerticalContentAlignment = VerticalContentAlignment.Top,
                Version = "1.5",
                BackgroundImage = GenerateContentBackground(),
                Body =
                {
                    // Condition
                    GenerateCondition(),
                    // Spacer
                    new AdaptiveTextBlock()
                    {
                        Size = TextSize.Small,
                        HorizontalAlignment = HAlignment.Center,
                        Spacing = Spacing.None,
                        Text = "&#8201;",
                        AdditionalProperties =
                        {
                            ["$when"] = JsonValue.CreateStringValue("${$host.widgetSize==\"medium\" || $host.widgetSize==\"large\"}")
                        }
                    },
                    // Daily Forecast
                    GenerateForecast(),
                    // Spacer
                    new AdaptiveTextBlock()
                    {
                        Size = TextSize.Small,
                        HorizontalAlignment = HAlignment.Center,
                        Spacing = Spacing.None,
                        Text = "&#8201;",
                        AdditionalProperties =
                        {
                            ["$when"] = JsonValue.CreateStringValue("${$host.widgetSize==\"large\"}")
                        }
                    },
                    // Hourly Forecast
                    GenerateHourlyForecast(),
                    // Location name
                    new AdaptiveTextBlock()
                    {
                        Size = TextSize.Default,
                        Weight = TextWeight.Bolder,
                        Text = "${current.location}",
                        HorizontalAlignment = HAlignment.Center,
                        FontType = FontType.Default,
                        Spacing = Spacing.Medium
                    }
                }
            }.ToJson().ToString());
        }

        public override async Task<string> BuildWidgetData(string widgetId, WidgetInfo info)
        {
            if (widgetId == null || info == null) return "${}";

            var locData = await GetLocation(widgetId);
            if (locData == null) return "${}";

            var weather = await LoadWeather(locData);
            if (weather == null) return "${}";

            var viewModel = weather.ToUiModel();

            return await BuildWidgetData(widgetId, viewModel, locData, info);
        }

        private async Task<string> BuildWidgetData(
            string widgetId, WeatherUiModel weather,
            LocationData.LocationData location, WidgetInfo info)
        {
            var forecast = await GetForecasts(location);
            var hr_forecast = await GetHourlyForecasts(location);

            var widgetData = new WeatherWidgetData()
            {
                current = await weather.ToCurrent(),
                forecast = await forecast?.ToForecasts(),
                showForecast = forecast?.Count > 0,
                hr_forecast = await hr_forecast?.ToForecasts(),
                showHrForecast = hr_forecast?.Count > 0,
                chanceIcon = await WeatherWidgetDataExtensions.GetWeatherIcon(WeatherIcons.RAINDROP)
            };

            return System.Text.Json.JsonSerializer.Serialize(widgetData, new System.Text.Json.JsonSerializerOptions()
            {
                IncludeFields = true
            });
        }

        private IAdaptiveCardElement GenerateCondition()
        {
            return new AdaptiveColumnSet()
            {
                Columns =
                {
                    new AdaptiveColumn()
                    {
                        Width = "auto",
                        Items =
                        {
                            new AdaptiveImage()
                            {
                                Style = ImageStyle.Default,
                                Size = ImageSize.Medium,
                                // Weather icon
                                Url = "${current.weatherIcon.light}",
                                AdditionalProperties =
                                {
                                    ["$when"] = JsonValue.CreateStringValue("${$host.hostTheme==\"light\"}")
                                }
                            },
                            new AdaptiveImage()
                            {
                                Style = ImageStyle.Default,
                                Size = ImageSize.Medium,
                                // Weather icon
                                Url = "${current.weatherIcon.dark}",
                                AdditionalProperties =
                                {
                                    ["$when"] = JsonValue.CreateStringValue("${$host.hostTheme==\"dark\"}")
                                }
                            },
                        }
                    },
                    new AdaptiveColumn()
                    {
                        Width = "stretch",
                        VerticalContentAlignment = VerticalContentAlignment.Center,
                        Height = HeightType.Stretch,
                        Items =
                        {
                            new AdaptiveTextBlock()
                            {
                                Weight = TextWeight.Default,
                                Text = "${current.temp}",
                                Size = TextSize.ExtraLarge,
                                FontType = FontType.Default,
                                Style = TextStyle.Heading,
                                HorizontalAlignment = HAlignment.Left,
                                Wrap = true,
                                IsSubtle = true,
                                Spacing = Spacing.None
                            }
                        }
                    },
                    new AdaptiveColumn()
                    {
                        Width = "auto",
                        VerticalContentAlignment = VerticalContentAlignment.Center,
                        Spacing = Spacing.Small,
                        Items =
                        {
                            new AdaptiveRichTextBlock()
                            {
                                Inlines =
                                {
                                    new AdaptiveTextRun()
                                    {
                                        Text = $"{ResStrings.label_feelslike}: ",
                                        Size = TextSize.Small,
                                        Weight = TextWeight.Bolder,
                                    },
                                    new AdaptiveTextRun()
                                    {
                                        Text = "${current.feelsLike}",
                                        Size = TextSize.Small,
                                        Weight = TextWeight.Default
                                    }
                                },
                                Spacing = Spacing.None
                            },
                            new AdaptiveRichTextBlock()
                            {
                                Inlines =
                                {
                                    new AdaptiveTextRun()
                                    {
                                        Text = $"{ResStrings.label_wind}: ",
                                        Size = TextSize.Small,
                                        Weight = TextWeight.Bolder,
                                    },
                                    new AdaptiveTextRun()
                                    {
                                        Text = "${current.windSpeed}",
                                        Size = TextSize.Small,
                                        Weight = TextWeight.Default
                                    }
                                },
                                Spacing = Spacing.None
                            },
                            new AdaptiveRichTextBlock()
                            {
                                Inlines =
                                {
                                    new AdaptiveTextRun()
                                    {
                                        Text = $"{ResStrings.label_chance}: ",
                                        Size = TextSize.Small,
                                        Weight = TextWeight.Bolder,
                                    },
                                    new AdaptiveTextRun()
                                    {
                                        Text = "${current.chance}",
                                        Size = TextSize.Small,
                                        Weight = TextWeight.Default
                                    }
                                },
                                Spacing = Spacing.None
                            },
                        }
                    }
                }
            };
        }

        private AdaptiveBackgroundImage GenerateContentBackground()
        {
            return new AdaptiveBackgroundImage()
            {
                FillMode = BackgroundImageFillMode.Cover,
                HorizontalAlignment = HAlignment.Center,
                VerticalAlignment = VAlignment.Center,
                Url = "${current.background}",
            };
        }

        private IAdaptiveCardElement GenerateForecast()
        {
            return new AdaptiveColumnSet()
            {
                Columns =
                {
                    new AdaptiveColumn()
                    {
                        Width = "1",
                        Items =
                        {
                            new AdaptiveTextBlock()
                            {
                                Text = "${date}",
                                HorizontalAlignment = HAlignment.Center,
                                Size = TextSize.Default,
                                Spacing = Spacing.None,
                                Weight = TextWeight.Bolder
                            },
                            new AdaptiveImage()
                            {
                                // Weather icon
                                Url = "${weatherIcon.light}",
                                PixelHeight = 40,
                                PixelWidth = 40,
                                HorizontalAlignment = HAlignment.Center,
                                Spacing = Spacing.Medium,
                                Size = ImageSize.Medium,
                                AdditionalProperties =
                                {
                                    ["$when"] = JsonValue.CreateStringValue("${$host.hostTheme==\"light\"}")
                                }
                            },
                            new AdaptiveImage()
                            {
                                // Weather icon
                                Url = "${weatherIcon.dark}",
                                PixelHeight = 40,
                                PixelWidth = 40,
                                HorizontalAlignment = HAlignment.Center,
                                Spacing = Spacing.Medium,
                                Size = ImageSize.Medium,
                                AdditionalProperties =
                                {
                                    ["$when"] = JsonValue.CreateStringValue("${$host.hostTheme==\"dark\"}")
                                }
                            },
                            new AdaptiveTextBlock()
                            {
                                Size = TextSize.Small,
                                HorizontalAlignment = HAlignment.Center,
                                Spacing = Spacing.None,
                                Text = "&#8201;"
                            },
                            new AdaptiveTextBlock()
                            {
                                Text = "${hi}",
                                HorizontalAlignment = HAlignment.Center,
                                Spacing = Spacing.None,
                                Weight = TextWeight.Bolder,
                                Size = TextSize.Default,
                                Height = HeightType.Stretch
                            },
                            new AdaptiveTextBlock()
                            {
                                Text = "${lo}",
                                HorizontalAlignment = HAlignment.Center,
                                Spacing = Spacing.None,
                                Weight = TextWeight.Bolder,
                                Size = TextSize.Default,
                                Height = HeightType.Stretch,
                                IsSubtle = true,
                            }
                        },
                        Spacing = Spacing.None,
                        VerticalContentAlignment = VerticalContentAlignment.Center,
                        AdditionalProperties =
                        {
                            ["$data"] = JsonValue.CreateStringValue("${forecast}")
                        }
                    }
                },
                AdditionalProperties =
                {
                    ["$when"] = JsonValue.CreateStringValue("${($host.widgetSize==\"medium\" || $host.widgetSize==\"large\") && showForecast == true}")
                }
            };
        }

        private IAdaptiveCardElement GenerateHourlyForecast()
        {
            return new AdaptiveColumnSet()
            {
                Columns =
                {
                    new AdaptiveColumn()
                    {
                        Width = "1",
                        Items =
                        {
                            new AdaptiveTextBlock()
                            {
                                Text = "${date}",
                                HorizontalAlignment = HAlignment.Center,
                                Size = TextSize.Default,
                                Spacing = Spacing.None,
                                Weight = TextWeight.Bolder
                            },
                            new AdaptiveImage()
                            {
                                // Weather icon
                                Url = "${weatherIcon.light}",
                                PixelHeight = 40,
                                PixelWidth = 40,
                                HorizontalAlignment = HAlignment.Center,
                                Spacing = Spacing.Medium,
                                Size = ImageSize.Medium,
                                AdditionalProperties =
                                {
                                    ["$when"] = JsonValue.CreateStringValue("${$host.hostTheme==\"light\"}")
                                }
                            },
                            new AdaptiveImage()
                            {
                                // Weather icon
                                Url = "${weatherIcon.dark}",
                                PixelHeight = 40,
                                PixelWidth = 40,
                                HorizontalAlignment = HAlignment.Center,
                                Spacing = Spacing.Medium,
                                Size = ImageSize.Medium,
                                AdditionalProperties =
                                {
                                    ["$when"] = JsonValue.CreateStringValue("${$host.hostTheme==\"dark\"}")
                                }
                            },
                            new AdaptiveTextBlock()
                            {
                                Size = TextSize.Small,
                                HorizontalAlignment = HAlignment.Center,
                                Spacing = Spacing.None,
                                Text = "&#8201;"
                            },
                            new AdaptiveTextBlock()
                            {
                                Text = "${hi}",
                                HorizontalAlignment = HAlignment.Center,
                                Spacing = Spacing.None,
                                Weight = TextWeight.Bolder,
                                Size = TextSize.Default,
                                Height = HeightType.Stretch
                            },
                            new AdaptiveColumnSet()
                            {
                                Spacing = Spacing.Small,
                                Columns =
                                {
                                    new AdaptiveColumn()
                                    {
                                        Width = "1",
                                        Spacing = Spacing.None,
                                        VerticalContentAlignment = VerticalContentAlignment.Center,
                                        Items =
                                        {
                                            new AdaptiveImage()
                                            {
                                                HorizontalAlignment = HAlignment.Center,
                                                Spacing = Spacing.None,
                                                Size = ImageSize.Small,
                                                Url = "${chanceIcon.light}",
                                                AdditionalProperties =
                                                {
                                                    ["$when"] = JsonValue.CreateStringValue("${$host.hostTheme==\"light\"}")
                                                }
                                            },
                                            new AdaptiveImage()
                                            {
                                                HorizontalAlignment = HAlignment.Center,
                                                Spacing = Spacing.None,
                                                Size = ImageSize.Small,
                                                Url = "${chanceIcon.dark}",
                                                AdditionalProperties =
                                                {
                                                    ["$when"] = JsonValue.CreateStringValue("${$host.hostTheme==\"dark\"}")
                                                }
                                            }
                                        }
                                    },
                                    new AdaptiveColumn()
                                    {
                                        Width = "2",
                                        Spacing = Spacing.None,
                                        VerticalContentAlignment = VerticalContentAlignment.Center,
                                        Items =
                                        {
                                            new AdaptiveTextBlock()
                                            {
                                                Text = "${chance}",
                                                HorizontalAlignment = HAlignment.Center,
                                                Spacing = Spacing.None,
                                                Size = TextSize.Small
                                            }
                                        }
                                    }
                                }
                            }
                        },
                        Spacing = Spacing.None,
                        VerticalContentAlignment = VerticalContentAlignment.Center,
                        AdditionalProperties =
                        {
                            ["$data"] = JsonValue.CreateStringValue("${hr_forecast}")
                        }
                    }
                },
                AdditionalProperties =
                {
                    ["$when"] = JsonValue.CreateStringValue("${$host.widgetSize==\"large\" && showHrForecast == true}")
                }
            };
        }

        private static async Task<List<ForecastItemViewModel>> GetForecasts(LocationData.LocationData location)
        {
            var SettingsManager = Ioc.Default.GetService<SettingsManager>();
            var forecasts = await SettingsManager.GetWeatherForecastData(location.query);
            var result = new List<ForecastItemViewModel>(MAX_FORECAST_LENGTH);

            if (forecasts?.forecast?.Count > 0)
            {
                for (int i = 0; i < Math.Min(MAX_FORECAST_LENGTH, forecasts.forecast.Count); i++)
                {
                    result.Add(new ForecastItemViewModel(forecasts.forecast[i]));
                }
            }

            return result;
        }

        private static async Task<List<HourlyForecastItemViewModel>> GetHourlyForecasts(LocationData.LocationData location)
        {
            var SettingsManager = Ioc.Default.GetService<SettingsManager>();
            var now = DateTimeOffset.Now.ToOffset(location.tz_offset);
            var hrInterval = WeatherModule.Instance.WeatherManager.HourlyForecastInterval;
            var date = now.AddHours(-(long)(hrInterval * 0.5d)).Trim(TimeSpan.TicksPerHour);
            var forecasts = await SettingsManager.GetHourlyWeatherForecastDataByPageIndexByLimitFilterByDate(location.query, 0, MAX_FORECAST_LENGTH, date);
            var result = new List<HourlyForecastItemViewModel>(MAX_FORECAST_LENGTH);

            if (forecasts?.Count > 0)
            {
                var count = 0;
                foreach (var fcast in forecasts)
                {
                    result.Add(new HourlyForecastItemViewModel(fcast));
                    count++;

                    if (count >= MAX_FORECAST_LENGTH) break;
                }
            }

            return result;
        }
    }
}
