using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Notifications;
using SimpleWeather.Utils;

namespace SimpleWeather.UWP.Helpers
{
    public static class WeatherTileCreator
    {
        const int MEDIUM_FORECAST_LENGTH = 2;
        const int WIDE_FORECAST_LENGTH = 3;
        const int LARGE_FORECAST_LENGTH = 5;

        public enum ForecastTileType
        {
            Small,
            Medium,
            Wide,
            Large
        }

        private static TileBindingContentAdaptive GenerateForecast(WeatherData.Weather weather, ForecastTileType forecastTileType)
        {
            var content = new TileBindingContentAdaptive
            {
                // Background URI
                BackgroundImage = new TileBackgroundImage()
                {
                    Source = WeatherUtils.GetBackgroundURI(weather)
                }
            };

            if (forecastTileType == ForecastTileType.Small)
            {
                content.Children.Add(new AdaptiveGroup()
                {
                    Children =
                    {
                        new AdaptiveSubgroup()
                        {
                            Children =
                            {
                                new AdaptiveText()
                                {
                                    Text = weather.update_time.ToString("ddd"),
                                    HintStyle = AdaptiveTextStyle.Base,
                                    HintAlign = AdaptiveTextAlign.Center
                                },
                                new AdaptiveText()
                                {
                                    Text = (Settings.IsFahrenheit ? weather.condition.temp_f : weather.condition.temp_c) + "º",
                                    HintStyle = AdaptiveTextStyle.Body,
                                    HintAlign = AdaptiveTextAlign.Center
                                }
                            }
                        }
                    }
                });
            }
            else if (forecastTileType == ForecastTileType.Medium)
            {
                var dateGroup = new AdaptiveGroup();
                var forecastGroup = new AdaptiveGroup();
                var tempGroup = new AdaptiveGroup();

                // 2day forecast
                for (int i = 0; i < MEDIUM_FORECAST_LENGTH + 1; i++)
                {
                    var forecast = weather.forecast[i];

                    var dateSubgroup = new AdaptiveSubgroup()
                    {
                        HintWeight = 1,
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = forecast.date.ToString("ddd"),
                                HintAlign = AdaptiveTextAlign.Center
                            },
                        }
                    };
                    var iconSubgroup = new AdaptiveSubgroup()
                    {
                        HintWeight = 1,
                        Children =
                        {
                            new AdaptiveImage()
                            {
                                HintRemoveMargin = true,
                                Source = WeatherUtils.GetWeatherIconURI(forecast.icon)
                            },
                        }
                    };
                    var tempSubgroup = new AdaptiveSubgroup()
                    {
                        HintWeight = 1,
                        HintTextStacking = AdaptiveSubgroupTextStacking.Bottom,
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = (Settings.IsFahrenheit ? forecast.high_f : forecast.high_c) + "º",
                                HintStyle = AdaptiveTextStyle.Caption,
                                HintAlign = AdaptiveTextAlign.Center
                            }
                        }
                    };

                    dateGroup.Children.Add(dateSubgroup);
                    forecastGroup.Children.Add(iconSubgroup);
                    tempGroup.Children.Add(tempSubgroup);
                }

                content.Children.Add(dateGroup);
                content.Children.Add(forecastGroup);
                content.Children.Add(tempGroup);
            }
            else if (forecastTileType == ForecastTileType.Wide)
            {
                // 3-day forecast
                var dateGroup = new AdaptiveGroup();
                var forecastGroup = new AdaptiveGroup();

                int forecastLength = WIDE_FORECAST_LENGTH;
                if (weather.forecast.Length < forecastLength)
                    forecastLength = weather.forecast.Length;

                for (int i = 0; i < forecastLength; i++)
                {
                    var forecast = weather.forecast[i];

                    var dateSubgroup = new AdaptiveSubgroup()
                    {
                        HintWeight = 1,
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = forecast.date.ToString("ddd"),
                                HintAlign = AdaptiveTextAlign.Left
                            },
                        }
                    };

                    var forecastSubgroup = new AdaptiveSubgroup()
                    {
                        HintWeight = 1,
                        HintTextStacking = AdaptiveSubgroupTextStacking.Center,
                        Children =
                        {
                            new AdaptiveImage()
                            {
                                HintRemoveMargin = true,
                                Source = WeatherUtils.GetWeatherIconURI(forecast.icon)
                            }
                        }
                    };

                    var tempSubgroup = new AdaptiveSubgroup()
                    {
                        HintWeight = 1,
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = (Settings.IsFahrenheit ? forecast.high_f : forecast.high_c) + "º",
                                HintStyle = AdaptiveTextStyle.Caption,
                                HintAlign = AdaptiveTextAlign.Center
                            },
                            new AdaptiveText()
                            {
                                Text = (Settings.IsFahrenheit ? forecast.low_f : forecast.low_c) + "º",
                                HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                HintAlign = AdaptiveTextAlign.Center
                            }
                        }
                    };

                    dateGroup.Children.Add(dateSubgroup);
                    forecastGroup.Children.Add(forecastSubgroup);
                    forecastGroup.Children.Add(tempSubgroup);
                }

                content.Children.Add(dateGroup);
                content.Children.Add(forecastGroup);
            }
            else if (forecastTileType == ForecastTileType.Large)
            {
                // Condition + 5-day forecast
                var forecastGroup = new AdaptiveGroup();
                var condition = weather.condition;

                int forecastLength = LARGE_FORECAST_LENGTH;
                if (weather.forecast.Length < forecastLength)
                    forecastLength = weather.forecast.Length;

                var conditionGroup = new AdaptiveGroup()
                {
                    Children =
                    {
                        new AdaptiveSubgroup()
                        {
                            HintWeight = 25,
                            HintTextStacking = AdaptiveSubgroupTextStacking.Center,
                            Children =
                            {
                                new AdaptiveImage()
                                {
                                    Source = WeatherUtils.GetWeatherIconURI(condition.icon)
                                }
                            }
                        },
                        new AdaptiveSubgroup()
                        {
                            HintWeight = 75,
                            Children =
                            {
                                new AdaptiveText()
                                {
                                    Text = (String.IsNullOrWhiteSpace(weather.condition.weather)) ? "---" : weather.condition.weather,
                                    HintStyle = AdaptiveTextStyle.Caption
                                },
                                new AdaptiveText()
                                {
                                    Text = string.Format("Temp: {0}", (Settings.IsFahrenheit ? weather.condition.temp_f : weather.condition.temp_c) + "º"),
                                    HintStyle = AdaptiveTextStyle.Caption
                                },
                                new AdaptiveText()
                                {
                                    Text = string.Format("Feels like: {0}", (Settings.IsFahrenheit ? weather.condition.feelslike_f : weather.condition.feelslike_c) + "º"),
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                                new AdaptiveText()
                                {
                                    Text = string.Format("Wind: {0}", Settings.IsFahrenheit ? weather.condition.wind_mph.ToString() + " mph" : weather.condition.wind_kph.ToString() + " kph"),
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                }
                            }
                        }
                    }
                };
                var text = new AdaptiveText()
                {
                    Text = ""
                };

                for (int i = 0; i < forecastLength; i++)
                {
                    var forecast = weather.forecast[i];

                    var subgroup = new AdaptiveSubgroup()
                    {
                        HintWeight = 1,
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = forecast.date.ToString("ddd"),
                                HintAlign = AdaptiveTextAlign.Center
                            },
                            new AdaptiveImage()
                            {
                                HintRemoveMargin = true,
                                Source = WeatherUtils.GetWeatherIconURI(forecast.icon)
                            },
                            new AdaptiveText()
                            {
                                Text = (Settings.IsFahrenheit ? forecast.high_f : forecast.high_c) + "º",
                                HintAlign = AdaptiveTextAlign.Center
                            },
                            new AdaptiveText()
                            {
                                Text = (Settings.IsFahrenheit ? forecast.low_f : forecast.low_c) + "º",
                                HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                HintAlign = AdaptiveTextAlign.Center
                            }
                        }
                    };

                    forecastGroup.Children.Add(subgroup);
                }

                content.Children.Add(conditionGroup);
                content.Children.Add(text);
                content.Children.Add(forecastGroup);
            }

            return content;
        }

        private static TileBindingContentAdaptive GenerateCondition(WeatherData.Weather weather, ForecastTileType forecastTileType)
        {
            var content = new TileBindingContentAdaptive
            {
                // Background URI
                BackgroundImage = new TileBackgroundImage()
                {
                    Source = WeatherUtils.GetBackgroundURI(weather)
                }
            };

            if (forecastTileType == ForecastTileType.Small)
            {
                content.Children.Add(new AdaptiveGroup()
                {
                    Children =
                    {
                        new AdaptiveSubgroup()
                        {
                            Children =
                            {
                                new AdaptiveText()
                                {
                                    Text = weather.update_time.ToString("ddd"),
                                    HintStyle = AdaptiveTextStyle.Base,
                                    HintAlign = AdaptiveTextAlign.Center
                                },
                                new AdaptiveText()
                                {
                                    Text = (Settings.IsFahrenheit ? weather.condition.temp_f : weather.condition.temp_c) + "º",
                                    HintStyle = AdaptiveTextStyle.Body,
                                    HintAlign = AdaptiveTextAlign.Center
                                }
                            }
                        }
                    }
                });
            }
            else if (forecastTileType == ForecastTileType.Medium)
            {
                content.Children.Add(new AdaptiveGroup()
                {
                    Children =
                    {
                        new AdaptiveSubgroup()
                        {
                            HintWeight = 1,
                            Children =
                            {
                                new AdaptiveText()
                                {
                                    Text = null
                                }
                            }
                        },
                        new AdaptiveSubgroup()
                        {
                            HintWeight = 3,
                            HintTextStacking = AdaptiveSubgroupTextStacking.Center,
                            Children =
                            {
                                new AdaptiveImage()
                                {
                                    HintRemoveMargin = true,
                                    Source = WeatherUtils.GetWeatherIconURI(weather.condition.icon)
                                },
                                new AdaptiveText()
                                {
                                    Text = (Settings.IsFahrenheit ? weather.condition.temp_f : weather.condition.temp_c) + "º",
                                    HintStyle = AdaptiveTextStyle.Body,
                                    HintAlign = AdaptiveTextAlign.Center,
                                }
                            }
                        },
                        new AdaptiveSubgroup()
                        {
                            HintWeight = 1,
                            Children =
                            {
                                new AdaptiveText()
                                {
                                    Text = null
                                }
                            }
                        }
                    }
                });
            }
            else if (forecastTileType == ForecastTileType.Wide)
            {
                // Current Condition (Extended)
                var forecastGroup = new AdaptiveGroup();

                var condition = new AdaptiveSubgroup()
                {
                    HintWeight = 25,
                    Children =
                    {
                        new AdaptiveImage()
                        {
                            HintRemoveMargin = true,
                            Source = WeatherUtils.GetWeatherIconURI(weather.condition.icon)
                        }
                    },
                    HintTextStacking = AdaptiveSubgroupTextStacking.Center
                };
                var details = new AdaptiveSubgroup()
                {
                    HintWeight = 75,
                    Children =
                    {
                        new AdaptiveText()
                        {
                            Text = (String.IsNullOrWhiteSpace(weather.condition.weather)) ? "---" : weather.condition.weather,
                            HintStyle = AdaptiveTextStyle.Caption
                        },
                        new AdaptiveText()
                        {
                            Text = string.Format("Temp: {0}", (Settings.IsFahrenheit ? weather.condition.temp_f : weather.condition.temp_c) + "º"),
                            HintStyle = AdaptiveTextStyle.Caption
                        },
                        new AdaptiveText()
                        {
                            Text = string.Format("Feels like: {0}", (Settings.IsFahrenheit ? weather.condition.feelslike_f : weather.condition.feelslike_c) + "º"),
                            HintStyle = AdaptiveTextStyle.CaptionSubtle
                        },
                        new AdaptiveText()
                        {
                            Text = string.Format("Wind: {0}", Settings.IsFahrenheit ? weather.condition.wind_mph.ToString() + " mph" : weather.condition.wind_kph.ToString() + " kph"),
                            HintStyle = AdaptiveTextStyle.CaptionSubtle
                        }
                    }
                };

                forecastGroup.Children.Add(condition);
                forecastGroup.Children.Add(details);
                content.Children.Add(forecastGroup);
            }
            else if (forecastTileType == ForecastTileType.Large)
            {
                // Condition + 5-day forecast
                var forecastGroup = new AdaptiveGroup();
                var condition = weather.condition;

                int forecastLength = LARGE_FORECAST_LENGTH;
                if (weather.forecast.Length < forecastLength)
                    forecastLength = weather.forecast.Length;

                var conditionGroup = new AdaptiveGroup()
                {
                    Children =
                    {
                        new AdaptiveSubgroup()
                        {
                            HintWeight = 25,
                            HintTextStacking = AdaptiveSubgroupTextStacking.Center,
                            Children =
                            {
                                new AdaptiveImage()
                                {
                                    Source = WeatherUtils.GetWeatherIconURI(weather.condition.icon)
                                }
                            }
                        },
                        new AdaptiveSubgroup()
                        {
                            HintWeight = 75,
                            Children =
                            {
                                new AdaptiveText()
                                {
                                    Text = (String.IsNullOrWhiteSpace(weather.condition.weather)) ? "---" : weather.condition.weather,
                                    HintStyle = AdaptiveTextStyle.Caption
                                },
                                new AdaptiveText()
                                {
                                    Text = string.Format("Temp: {0}", (Settings.IsFahrenheit ? weather.condition.temp_f : weather.condition.temp_c) + "º"),
                                    HintStyle = AdaptiveTextStyle.Caption
                                },
                                new AdaptiveText()
                                {
                                    Text = string.Format("Feels like: {0}", (Settings.IsFahrenheit ? weather.condition.feelslike_f : weather.condition.feelslike_c) + "º"),
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                                new AdaptiveText()
                                {
                                    Text = string.Format("Wind: {0}", Settings.IsFahrenheit ? weather.condition.wind_mph.ToString() + " mph" : weather.condition.wind_kph.ToString() + " kph"),
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                }
                            }
                        }
                    }
                };
                var text = new AdaptiveText()
                {
                    Text = ""
                };

                for (int i = 0; i < forecastLength; i++)
                {
                    var forecast = weather.forecast[i];

                    var subgroup = new AdaptiveSubgroup()
                    {
                        HintWeight = 1,
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = forecast.date.ToString("ddd"),
                                HintAlign = AdaptiveTextAlign.Center
                            },
                            new AdaptiveImage()
                            {
                                HintRemoveMargin = true,
                                Source = WeatherUtils.GetWeatherIconURI(forecast.icon)
                            },
                            new AdaptiveText()
                            {
                                Text = (Settings.IsFahrenheit ? forecast.high_f : forecast.high_c) + "º",
                                HintAlign = AdaptiveTextAlign.Center
                            },
                            new AdaptiveText()
                            {
                                Text = (Settings.IsFahrenheit ? forecast.low_f : forecast.low_c) + "º",
                                HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                HintAlign = AdaptiveTextAlign.Center
                            }
                        }
                    };

                    forecastGroup.Children.Add(subgroup);
                }

                content.Children.Add(conditionGroup);
                content.Children.Add(text);
                content.Children.Add(forecastGroup);
            }

            return content;
        }

        public static void TileUpdater(WeatherData.Weather weather)
        {
            var forecastTileContent = new TileContent()
            {
                Visual = new TileVisual()
                {
                    DisplayName = weather.location.name,
                    TileSmall = new TileBinding()
                    {
                        Branding = TileBranding.None,
                        Content = GenerateForecast(weather, ForecastTileType.Small),
                    },
                    TileMedium = new TileBinding()
                    {
                        // Mini forecast (2-day)
                        Branding = TileBranding.Name,
                        Content = GenerateForecast(weather, ForecastTileType.Medium),
                    },
                    TileWide = new TileBinding()
                    {
                        // 5-day forecast
                        Branding = TileBranding.Name,
                        Content = GenerateForecast(weather, ForecastTileType.Wide),
                    },
                    /*
                    TileLarge = new TileBinding()
                    {
                        Branding = TileBranding.Name,
                        Content = GenerateForecast(weather, ForecastTileType.Large),
                    }
                    */
                }
            };

            var currentTileContent = new TileContent()
            {
                Visual = new TileVisual()
                {
                    DisplayName = weather.location.name,
                    TileSmall = new TileBinding()
                    {
                        Branding = TileBranding.None,
                        Content = GenerateCondition(weather, ForecastTileType.Small),
                    },
                    TileMedium = new TileBinding()
                    {
                        // Mini forecast (2-day)
                        Branding = TileBranding.Name,
                        Content = GenerateCondition(weather, ForecastTileType.Medium),
                    },
                    TileWide = new TileBinding()
                    {
                        // 5-day forecast
                        Branding = TileBranding.Name,
                        Content = GenerateCondition(weather, ForecastTileType.Wide),
                    },
                    TileLarge = new TileBinding()
                    {
                        Branding = TileBranding.Name,
                        Content = GenerateCondition(weather, ForecastTileType.Large),
                    }
                }
            };

            // Create the tile notification
            var forecastTileNotif = new TileNotification(forecastTileContent.GetXml()) { Tag = "Forecast" };
            var currentTileNotif = new TileNotification(currentTileContent.GetXml()) { Tag = "Conditions" };

            // And send the notification to the primary tile
            var tileUpdater = TileUpdateManager.CreateTileUpdaterForApplication();
            tileUpdater.EnableNotificationQueue(true);
            tileUpdater.Clear();
            tileUpdater.Update(currentTileNotif);
            tileUpdater.Update(forecastTileNotif);
        }
    }
}
