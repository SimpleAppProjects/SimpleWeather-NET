using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.WinUI.Notifications;
using KeyedSemaphores;
using SimpleWeather.Common.Controls;
using SimpleWeather.Common.ViewModels;
using SimpleWeather.Common.WeatherData;
using SimpleWeather.Icons;
using SimpleWeather.LocationData;
using SimpleWeather.NET.Shared.Helpers;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.NET.Tiles
{
    public static class WeatherTileCreator
    {
        private const int MEDIUM_FORECAST_LENGTH = 3;
        private const int WIDE_FORECAST_LENGTH = 3;
        private const int LARGE_FORECAST_LENGTH = 5;

        private static readonly WeatherIconsManager wim = SharedModule.Instance.WeatherIconsManager;

        public enum ForecastTileType
        {
            Small,
            Medium,
            Wide,
            Large
        }

        private static void SetContentBackground(TileBindingContentAdaptive content, ImageDataViewModel imageData)
        {
            // Background URI
            if (NET.Utils.FeatureSettings.TileBackgroundImage && imageData?.ImageUri != null)
            {
                content.BackgroundImage = new TileBackgroundImage()
                {
                    Source = imageData?.ImageUri?.ToString(),
                    HintOverlay = 50
                };
            }
        }

        private static TileBindingContentAdaptive GenerateForecast(WeatherUiModel weather, List<ForecastItemViewModel> forecasts, ForecastTileType forecastTileType, ImageDataViewModel imageData)
        {
            var culture = CultureUtils.UserCulture;

            var content = new TileBindingContentAdaptive();
            SetContentBackground(content, imageData);

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
                                    Text = weather.CurTemp.RemoveNonDigitChars() + "°",
                                    HintStyle = AdaptiveTextStyle.Body,
                                    HintAlign = AdaptiveTextAlign.Center
                                },
                                new AdaptiveText()
                                {
                                    Text = weather.Location,
                                    HintStyle = AdaptiveTextStyle.Base,
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
                var tempHiGroup = new AdaptiveGroup();
                var tempLoGroup = new AdaptiveGroup();

                // 3day forecast
                for (int i = 0; i < Math.Min((int)forecasts?.Count, MEDIUM_FORECAST_LENGTH); i++)
                {
                    var forecast = forecasts[i];

                    var dateSubgroup = new AdaptiveSubgroup()
                    {
                        HintWeight = 1,
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = forecast.ShortDate,
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
                                Source = wim.GetWeatherIconURI(forecast.WeatherIcon, false)
                            },
                        }
                    };
                    var tempHiSubgroup = new AdaptiveSubgroup()
                    {
                        HintWeight = 1,
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = forecast.HiTemp,
                                //HintStyle = AdaptiveTextStyle.Caption,
                                HintAlign = AdaptiveTextAlign.Center
                            }
                        }
                    };
                    var tempLoSubgroup = new AdaptiveSubgroup()
                    {
                        HintWeight = 1,
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = forecast.LoTemp,
                                HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                HintAlign = AdaptiveTextAlign.Center
                            }
                        }
                    };

                    dateGroup.Children.Add(dateSubgroup);
                    forecastGroup.Children.Add(iconSubgroup);
                    tempHiGroup.Children.Add(tempHiSubgroup);
                    tempLoGroup.Children.Add(tempLoSubgroup);
                }

                content.Children.Add(dateGroup);
                content.Children.Add(forecastGroup);
                content.Children.Add(tempHiGroup);
                content.Children.Add(tempLoGroup);
            }
            else if (forecastTileType == ForecastTileType.Wide)
            {
                // 3-day forecast
                var dateGroup = new AdaptiveGroup();
                var forecastGroup = new AdaptiveGroup();

                int forecastLength = Math.Min((int)forecasts?.Count, WIDE_FORECAST_LENGTH);

                for (int i = 0; i < forecastLength; i++)
                {
                    var forecast = forecasts[i];

                    var dateSubgroup = new AdaptiveSubgroup()
                    {
                        HintWeight = 1,
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = forecast.ShortDate,
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
                                Source = wim.GetWeatherIconURI(forecast.WeatherIcon, false)
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
                                Text = forecast.HiTemp,
                                //HintStyle = AdaptiveTextStyle.Caption,
                                HintAlign = AdaptiveTextAlign.Center
                            },
                            new AdaptiveText()
                            {
                                Text = forecast.LoTemp,
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
                                    Source = wim.GetWeatherIconURI(weather.WeatherIcon, false)
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
                                    Text = weather.CurCondition?.Ellipsize(30),
                                    //HintStyle = AdaptiveTextStyle.Caption
                                },
                                new AdaptiveText()
                                {
                                    Text = string.Format("{0}: {1}", ResStrings.label_temp, weather.CurTemp.RemoveNonDigitChars() + "°"),
                                    //HintStyle = AdaptiveTextStyle.Caption
                                },
                                new AdaptiveText()
                                {
                                    Text = string.Format("{0}: {1}", ResStrings.label_feelslike, weather.WeatherDetailsMap[WeatherDetailsType.FeelsLike]?.Value ?? WeatherIcons.EM_DASH),
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                                new AdaptiveText()
                                {
                                    Text = string.Format("{0}: {1}", ResStrings.label_wind, weather.WeatherDetailsMap[WeatherDetailsType.WindSpeed]?.Value ?? WeatherIcons.EM_DASH),
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

                bool hasForecast = forecasts?.Count > 0;

                content.Children.Add(conditionGroup);
                content.Children.Add(text);
                if (hasForecast)
                    content.Children.Add(GenerateLargeForecastGroup(forecasts));
            }

            return content;
        }

        private static AdaptiveGroup GenerateLargeForecastGroup(List<ForecastItemViewModel> forecasts)
        {
            var forecastGroup = new AdaptiveGroup();
            bool hasForecast = forecasts?.Count > 0;

            if (hasForecast)
            {
                int forecastLength = Math.Min((int)forecasts?.Count, LARGE_FORECAST_LENGTH);

                if (forecastLength >= 4)
                {
                    for (int i = 0; i < forecastLength; i++)
                    {
                        var forecast = forecasts[i];

                        var subgroup = new AdaptiveSubgroup()
                        {
                            HintWeight = 1,
                            Children =
                            {
                                new AdaptiveText()
                                {
                                    Text = forecast.ShortDate,
                                    HintAlign = AdaptiveTextAlign.Center
                                },
                                new AdaptiveImage()
                                {
                                    HintRemoveMargin = true,
                                    Source = wim.GetWeatherIconURI(forecast.WeatherIcon, false)
                                },
                                new AdaptiveText()
                                {
                                    Text = forecast.HiTemp,
                                    HintAlign = AdaptiveTextAlign.Center
                                },
                                new AdaptiveText()
                                {
                                    Text = forecast.LoTemp,
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                    HintAlign = AdaptiveTextAlign.Center
                                }
                            }
                        };

                        forecastGroup.Children.Add(subgroup);
                    }
                }
                else
                {
                    if (forecastLength == 1)
                    {
                        forecastGroup.Children.Add(new AdaptiveSubgroup()
                        {
                            HintWeight = 2
                        });
                    }

                    for (int i = 0; i < forecastLength; i++)
                    {
                        var forecast = forecasts[i];

                        var subgroup = new AdaptiveSubgroup()
                        {
                            HintWeight = 1,
                            Children =
                            {
                                new AdaptiveText()
                                {
                                    Text = forecast.ShortDate,
                                    HintAlign = AdaptiveTextAlign.Center
                                },
                                new AdaptiveImage()
                                {
                                    HintRemoveMargin = true,
                                    Source = wim.GetWeatherIconURI(forecast.WeatherIcon, false)
                                },
                                new AdaptiveText()
                                {
                                    Text = forecast.HiTemp,
                                    HintAlign = AdaptiveTextAlign.Center
                                },
                                new AdaptiveText()
                                {
                                    Text = forecast.LoTemp,
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                    HintAlign = AdaptiveTextAlign.Center
                                }
                            }
                        };

                        forecastGroup.Children.Add(subgroup);

                        if (i < forecastLength - 1 || forecastLength == 1)
                        {
                            forecastGroup.Children.Add(new AdaptiveSubgroup()
                            {
                                HintWeight = forecastLength >= 3 ? 1 : 2
                            });
                        }
                    }
                }
            }

            return forecastGroup;
        }

        private static TileBindingContentAdaptive GenerateHrForecast(WeatherUiModel weather, List<HourlyForecastItemViewModel> forecasts, ForecastTileType forecastTileType, ImageDataViewModel imageData)
        {
            var culture = CultureUtils.UserCulture;

            var content = new TileBindingContentAdaptive();
            SetContentBackground(content, imageData);

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
                                    Text = weather.CurTemp.RemoveNonDigitChars() + "°",
                                    HintStyle = AdaptiveTextStyle.Body,
                                    HintAlign = AdaptiveTextAlign.Center
                                },
                                new AdaptiveText()
                                {
                                    Text = weather.Location,
                                    HintStyle = AdaptiveTextStyle.Base,
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

                int forecastLength = Math.Min((int)forecasts?.Count, MEDIUM_FORECAST_LENGTH);

                // 3hr forecast
                for (int i = 0; i < forecastLength; i++)
                {
                    var hrforecast = forecasts[i];

                    var dateSubgroup = new AdaptiveSubgroup()
                    {
                        HintWeight = 1,
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = hrforecast.ShortDate.ToLower(),
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
                                Source = wim.GetWeatherIconURI(hrforecast.WeatherIcon, false)
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
                                Text = hrforecast.HiTemp,
                                //HintStyle = AdaptiveTextStyle.Caption,
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
                // 5-hr forecast
                var forecastGroup = new AdaptiveGroup();

                string timeformat = culture.DateTimeFormat.ShortTimePattern.Contains("H") ?
                    "HH" : "ht";

                int forecastLength = Math.Min((int)forecasts?.Count, LARGE_FORECAST_LENGTH);

                for (int i = 0; i < forecastLength; i++)
                {
                    var hrforecast = forecasts[i];

                    var subgroup = new AdaptiveSubgroup()
                    {
                        HintWeight = 1,
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = hrforecast.ShortDate.ToLower(),
                                HintAlign = AdaptiveTextAlign.Center
                            },
                            new AdaptiveImage()
                            {
                                HintRemoveMargin = true,
                                Source = wim.GetWeatherIconURI(hrforecast.WeatherIcon, false)
                            },
                            new AdaptiveText()
                            {
                                Text = hrforecast.HiTemp,
                                HintAlign = AdaptiveTextAlign.Center
                            },
                        }
                    };

                    forecastGroup.Children.Add(subgroup);
                }

                content.Children.Add(forecastGroup);
            }
            else if (forecastTileType == ForecastTileType.Large)
            {
                // Condition + 5-hr forecast
                var forecastGroup = new AdaptiveGroup();

                var popDetail = weather.WeatherDetailsMap[WeatherDetailsType.PoPCloudiness] ?? weather.WeatherDetailsMap[WeatherDetailsType.PoPChance];

                int forecastLength = Math.Min((int)forecasts?.Count, LARGE_FORECAST_LENGTH);

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
                                    Source = wim.GetWeatherIconURI(weather.WeatherIcon, false)
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
                                    Text = weather.CurCondition?.Ellipsize(30),
                                    //HintStyle = AdaptiveTextStyle.Caption
                                },
                                new AdaptiveText()
                                {
                                    Text = string.Format("{0}: {1}", ResStrings.label_temp, weather.CurTemp.RemoveNonDigitChars() + "°"),
                                    //HintStyle = AdaptiveTextStyle.Caption
                                },
                                new AdaptiveText()
                                {
                                    Text = string.Format("{0}: {1}", ResStrings.label_feelslike, weather.WeatherDetailsMap[WeatherDetailsType.FeelsLike]?.Value ?? WeatherIcons.EM_DASH),
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                                new AdaptiveText()
                                {
                                    Text = string.Format("{0}: {1}", ResStrings.label_wind, weather.WeatherDetailsMap[WeatherDetailsType.WindSpeed]?.Value ?? WeatherIcons.EM_DASH),
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                                new AdaptiveText()
                                {
                                    Text = popDetail != null ? string.Format("{0}: {1}", popDetail.Label, popDetail.Value) : "",
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
                    var hrforecast = forecasts[i];

                    var subgroup = new AdaptiveSubgroup()
                    {
                        HintWeight = 1,
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = hrforecast.ShortDate.ToLower(),
                                HintAlign = AdaptiveTextAlign.Center
                            },
                            new AdaptiveImage()
                            {
                                HintRemoveMargin = true,
                                Source = wim.GetWeatherIconURI(hrforecast.WeatherIcon, false)
                            },
                            new AdaptiveText()
                            {
                                Text = hrforecast.HiTemp,
                                HintAlign = AdaptiveTextAlign.Center
                            },
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

        private static TileBindingContentAdaptive GenerateCondition(WeatherUiModel weather, List<ForecastItemViewModel> forecasts, ForecastTileType forecastTileType, ImageDataViewModel imageData)
        {
            var culture = CultureUtils.UserCulture;

            var content = new TileBindingContentAdaptive();
            SetContentBackground(content, imageData);

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
                                    Text = weather.CurTemp.RemoveNonDigitChars() + "°",
                                    HintStyle = AdaptiveTextStyle.Body,
                                    HintAlign = AdaptiveTextAlign.Center
                                },
                                new AdaptiveText()
                                {
                                    Text = weather.Location,
                                    HintStyle = AdaptiveTextStyle.Base,
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
                                    Source = wim.GetWeatherIconURI(weather.WeatherIcon, false)
                                },
                                new AdaptiveText()
                                {
                                    Text = weather.CurTemp.RemoveNonDigitChars() + "°",
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
                            Source = wim.GetWeatherIconURI(weather.WeatherIcon, false)
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
                            Text = weather.CurCondition?.Ellipsize(30),
                            //HintStyle = AdaptiveTextStyle.Caption
                        },
                        new AdaptiveText()
                        {
                            Text = string.Format("{0}: {1}", ResStrings.label_temp, weather.CurTemp.RemoveNonDigitChars() + "°"),
                            //HintStyle = AdaptiveTextStyle.Caption
                        },
                        new AdaptiveText()
                        {
                            Text = string.Format("{0}: {1}", ResStrings.label_feelslike, weather.WeatherDetailsMap[WeatherDetailsType.FeelsLike]?.Value ?? WeatherIcons.EM_DASH),
                            HintStyle = AdaptiveTextStyle.CaptionSubtle
                        },
                        new AdaptiveText()
                        {
                            Text = string.Format("{0}: {1}", ResStrings.label_wind, weather.WeatherDetailsMap[WeatherDetailsType.WindSpeed]?.Value ?? WeatherIcons.EM_DASH),
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
                                    Source = wim.GetWeatherIconURI(weather.WeatherIcon, false)
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
                                    Text = weather.CurCondition?.Ellipsize(30),
                                    //HintStyle = AdaptiveTextStyle.Caption
                                },
                                new AdaptiveText()
                                {
                                    Text = string.Format("{0}: {1}", ResStrings.label_temp, weather.CurTemp.RemoveNonDigitChars() + "°"),
                                    //HintStyle = AdaptiveTextStyle.Caption
                                },
                                new AdaptiveText()
                                {
                                    Text = string.Format("{0}: {1}", ResStrings.label_feelslike, weather.WeatherDetailsMap[WeatherDetailsType.FeelsLike]?.Value ?? WeatherIcons.EM_DASH),
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                                new AdaptiveText()
                                {
                                    Text = string.Format("{0}: {1}", ResStrings.label_wind, weather.WeatherDetailsMap[WeatherDetailsType.WindSpeed]?.Value ?? WeatherIcons.EM_DASH),
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

                bool hasForecast = forecasts?.Count > 0;

                content.Children.Add(conditionGroup);
                content.Children.Add(text);
                if (hasForecast)
                    content.Children.Add(GenerateLargeForecastGroup(forecasts));
            }

            return content;
        }

        private static async Task UpdateContent(TileUpdater tileUpdater, LocationData.LocationData location, WeatherUiModel weather, ImageDataViewModel imageData)
        {
            var forecasts = await GetForecasts(location);
            var hrforecasts = await GetHourlyForecasts(location);

            bool hasHourly = hrforecasts?.Count > 0;
            bool hasForecast = forecasts?.Count > 0;
            TileContent forecastTileContent = null;
            TileContent currentTileContent = null;
            TileContent hrforecastTileContent = null;

            if (hasHourly)
            {
                hrforecastTileContent = new TileContent()
                {
                    Visual = new TileVisual()
                    {
                        BaseUri = new Uri(WeatherIconsManager.GetPNGBaseUri(), UriKind.Absolute),
                        DisplayName = weather.Location?.Ellipsize(40),
                        TileSmall = new TileBinding()
                        {
                            Branding = TileBranding.None,
                            Content = GenerateHrForecast(weather, hrforecasts, ForecastTileType.Small, imageData),
                        },
                        TileMedium = new TileBinding()
                        {
                            // Mini forecast (3-hr)
                            Branding = TileBranding.Name,
                            Content = GenerateHrForecast(weather, hrforecasts, ForecastTileType.Medium, imageData),
                        },
                        TileWide = new TileBinding()
                        {
                            // 5-hr forecast
                            Branding = TileBranding.Name,
                            Content = GenerateHrForecast(weather, hrforecasts, ForecastTileType.Wide, imageData),
                        },
                        TileLarge = new TileBinding()
                        {
                            Branding = TileBranding.Name,
                            Content = GenerateHrForecast(weather, hrforecasts, ForecastTileType.Large, imageData),
                        }
                    }
                };
            }

            if (hasForecast)
            {
                forecastTileContent = new TileContent()
                {
                    Visual = new TileVisual()
                    {
                        BaseUri = new Uri(WeatherIconsManager.GetPNGBaseUri(), UriKind.Absolute),
                        DisplayName = weather.Location?.Ellipsize(40),
                        TileSmall = new TileBinding()
                        {
                            Branding = TileBranding.None,
                            Content = GenerateForecast(weather, forecasts, ForecastTileType.Small, imageData),
                        },
                        TileMedium = new TileBinding()
                        {
                            // Mini forecast (2-day)
                            Branding = TileBranding.Name,
                            Content = GenerateForecast(weather, forecasts, ForecastTileType.Medium, imageData),
                        },
                        TileWide = new TileBinding()
                        {
                            // 5-day forecast
                            Branding = TileBranding.Name,
                            Content = GenerateForecast(weather, forecasts, ForecastTileType.Wide, imageData),
                        },
                        /*
                         * All ready shown in current tile
                        TileLarge = new TileBinding()
                        {
                            Branding = TileBranding.Name,
                            Content = GenerateForecast(weather, forecasts, ForecastTileType.Large, imageData),
                        }
                        */
                    }
                };
            }

            currentTileContent = new TileContent()
            {
                Visual = new TileVisual()
                {
                    BaseUri = new Uri(WeatherIconsManager.GetPNGBaseUri(), UriKind.Absolute),
                    DisplayName = weather.Location?.Ellipsize(40),
                    TileSmall = new TileBinding()
                    {
                        Branding = TileBranding.None,
                        Content = GenerateCondition(weather, forecasts, ForecastTileType.Small, imageData),
                    },
                    TileMedium = new TileBinding()
                    {
                        // Mini forecast (2-day)
                        Branding = TileBranding.Name,
                        Content = GenerateCondition(weather, forecasts, ForecastTileType.Medium, imageData),
                    },
                    TileWide = new TileBinding()
                    {
                        // 5-day forecast
                        Branding = TileBranding.Name,
                        Content = GenerateCondition(weather, forecasts, ForecastTileType.Wide, imageData),
                    },
                    TileLarge = new TileBinding()
                    {
                        Branding = TileBranding.Name,
                        Content = GenerateCondition(weather, forecasts, ForecastTileType.Large, imageData),
                    },
                    LockDetailedStatus1 = weather.Location?.Ellipsize(40),
                    LockDetailedStatus2 = String.Format("{0} - {1}", (weather.CurTemp?.RemoveNonDigitChars() ?? WeatherIcons.PLACEHOLDER) + "°", weather.CurCondition?.Ellipsize(33)),
                    LockDetailedStatus3 = String.Format("{0} | {1}", weather.HiTemp ?? WeatherIcons.PLACEHOLDER, weather.LoTemp ?? WeatherIcons.PLACEHOLDER)
                }
            };

            // Create the tile notification
            tileUpdater.EnableNotificationQueue(true);
            tileUpdater.Clear();

            var currentTileNotif = new TileNotification(currentTileContent.GetXml()) { Tag = "Conditions" };
            tileUpdater.Update(currentTileNotif);

            if (hasForecast)
            {
                var forecastTileNotif = new TileNotification(forecastTileContent.GetXml()) { Tag = "Forecast" };
                tileUpdater.Update(forecastTileNotif);
            }
            if (hasHourly)
            {
                var hrforecastTileNotif = new TileNotification(hrforecastTileContent.GetXml()) { Tag = "Hourly Forecast" };
                tileUpdater.Update(hrforecastTileNotif);
            }
        }

        private static async Task<List<ForecastItemViewModel>> GetForecasts(LocationData.LocationData location)
        {
            var SettingsManager = Ioc.Default.GetService<SettingsManager>();
            var forecasts = await SettingsManager.GetWeatherForecastData(location.query);
            var result = new List<ForecastItemViewModel>(LARGE_FORECAST_LENGTH);

            if (forecasts?.forecast?.Count > 0)
            {
                for (int i = 0; i < Math.Min(LARGE_FORECAST_LENGTH, forecasts.forecast.Count); i++)
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
            var forecasts = await SettingsManager.GetHourlyWeatherForecastDataByPageIndexByLimitFilterByDate(location.query, 0, LARGE_FORECAST_LENGTH, date);
            var result = new List<HourlyForecastItemViewModel>(LARGE_FORECAST_LENGTH);

            if (forecasts?.Count > 0)
            {
                var count = 0;
                foreach (var fcast in forecasts)
                {
                    result.Add(new HourlyForecastItemViewModel(fcast));
                    count++;

                    if (count >= LARGE_FORECAST_LENGTH) break;
                }
            }

            return result;
        }

        public static async Task TileUpdater(LocationData.LocationData location)
        {
            // Check if Tile service is available
            if (!DeviceTypeHelper.IsTileSupported())
                return;

            try
            {
                var wloader = new WeatherDataLoader(location);
                var weather = await wloader.LoadWeatherData(
                            new WeatherRequest.Builder()
                                .ForceLoadSavedData()
                                .Build());

                if (weather != null)
                {
                    await TileUpdater(location, weather.ToUiModel());
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Error, ex);
            }
        }

        private static async Task TileUpdater(LocationData.LocationData location, WeatherUiModel weather)
        {
            var imageData = await weather.GetImageData();
            var SettingsManager = Ioc.Default.GetService<SettingsManager>();

            // And send the notification to the tile
            if (location.locationType == LocationType.GPS || Equals(await SettingsManager.GetHomeData(), location))
            {
                // Update primary tile and lockscreen info (if exists)
                using (await KeyedSemaphore.LockAsync("appTileUpdater"))
                {
                    var appTileUpdater = TileUpdateManager.CreateTileUpdaterForApplication();
                    try
                    {
                        await UpdateContent(appTileUpdater, location, weather, imageData);
                    }
                    catch (Exception e)
                    {
                        Logger.WriteLine(LoggerLevel.Error, e);
                    }
                }

                // Update secondary tile if exists
                var query = location.locationType == LocationType.GPS ? Constants.KEY_GPS : location.query;
                if (SecondaryTileUtils.Exists(query))
                {
                    var tileId = SecondaryTileUtils.GetTileId(query);
                    using (await KeyedSemaphore.LockAsync(tileId))
                    {
                        var tileUpdater = TileUpdateManager.CreateTileUpdaterForSecondaryTile(tileId);

                        try
                        {
                            await UpdateContent(tileUpdater, location, weather, imageData);
                        }
                        catch (Exception e)
                        {
                            Logger.WriteLine(LoggerLevel.Error, e);
                        }
                    }
                }
            }
            else
            {
                // Update secondary tile
                if (SecondaryTileUtils.Exists(location.query))
                {
                    var tileId = SecondaryTileUtils.GetTileId(location.query);
                    using (await KeyedSemaphore.LockAsync(tileId))
                    {
                        var tileUpdater = TileUpdateManager.CreateTileUpdaterForSecondaryTile(tileId);

                        try
                        {
                            await UpdateContent(tileUpdater, location, weather, imageData);
                        }
                        catch (Exception e)
                        {
                            Logger.WriteLine(LoggerLevel.Error, e);
                        }
                    }
                }
            }
        }
    }
}
