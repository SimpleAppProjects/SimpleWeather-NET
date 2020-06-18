using Microsoft.Toolkit.Uwp.Notifications;
using SimpleWeather.Controls;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.UWP.Shared.Helpers;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Windows.System.UserProfile;
using Windows.UI.Notifications;

namespace SimpleWeather.UWP.Tiles
{
    public static class WeatherTileCreator
    {
        private const int MEDIUM_FORECAST_LENGTH = 3;
        private const int WIDE_FORECAST_LENGTH = 3;
        private const int LARGE_FORECAST_LENGTH = 5;

        public enum ForecastTileType
        {
            Small,
            Medium,
            Wide,
            Large
        }

        private static void SetContentBackground(TileBindingContentAdaptive content, WeatherNowViewModel weather)
        {
            // Background URI
            if (weather.ImageData?.ImageUri != null)
            {
                content.BackgroundImage = new TileBackgroundImage()
                {
                    Source = weather.ImageData?.ImageUri?.ToString(),
                    HintOverlay = 50
                };
            }
        }

        private static TileBindingContentAdaptive GenerateForecast(WeatherNowViewModel weather, List<ForecastItemViewModel> forecasts, ForecastTileType forecastTileType)
        {
            var userlang = GlobalizationPreferences.Languages[0];
            var culture = new CultureInfo(userlang);

            var content = new TileBindingContentAdaptive();
            SetContentBackground(content, weather);

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
                                    Text = weather.CurTemp.RemoveNonDigitChars() + "º",
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
                                Source = WeatherUtils.GetWeatherIconURI(forecast.WeatherIcon, false)
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
                                Text = forecast.HiTemp,
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
                                Source = WeatherUtils.GetWeatherIconURI(forecast.WeatherIcon, false)
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
                                HintStyle = AdaptiveTextStyle.Caption,
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
                var forecastGroup = new AdaptiveGroup();

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
                                    Source = WeatherUtils.GetWeatherIconURI(weather.WeatherIcon, false)
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
                                    HintStyle = AdaptiveTextStyle.Caption
                                },
                                new AdaptiveText()
                                {
                                    Text = string.Format("{0}: {1}", App.ResLoader.GetString("Temp_Label"), weather.CurTemp.RemoveNonDigitChars() + "º"),
                                    HintStyle = AdaptiveTextStyle.Caption
                                },
                                new AdaptiveText()
                                {
                                    Text = string.Format("{0}: {1}", App.ResLoader.GetString("FeelsLike_Label"), weather.WeatherDetails.FirstOrDefault(detail => detail.DetailsType == WeatherDetailsType.FeelsLike)?.Value),
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                                new AdaptiveText()
                                {
                                    Text = string.Format("{0}: {1}", App.ResLoader.GetString("Wind_Label"), weather.WeatherDetails.FirstOrDefault(detail => detail.DetailsType == WeatherDetailsType.WindSpeed)?.Value),
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
                                Source = WeatherUtils.GetWeatherIconURI(forecast.WeatherIcon, false)
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

                content.Children.Add(conditionGroup);
                content.Children.Add(text);
                content.Children.Add(forecastGroup);
            }

            return content;
        }

        private static TileBindingContentAdaptive GenerateHrForecast(WeatherNowViewModel weather, List<HourlyForecastItemViewModel> forecasts, ForecastTileType forecastTileType)
        {
            var userlang = GlobalizationPreferences.Languages[0];
            var culture = new CultureInfo(userlang);

            var content = new TileBindingContentAdaptive();
            SetContentBackground(content, weather);

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
                                    Text = weather.CurTemp.RemoveNonDigitChars() + "º",
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
                                Source = WeatherUtils.GetWeatherIconURI(hrforecast.WeatherIcon, false)
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
                                Source = WeatherUtils.GetWeatherIconURI(hrforecast.WeatherIcon, false)
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

                string poplabel = weather.WeatherSource.Equals(WeatherAPI.OpenWeatherMap) || weather.WeatherSource.Equals(WeatherAPI.MetNo) ?
                    App.ResLoader.GetString("Cloudiness_Label") : App.ResLoader.GetString("Precipitation_Label"); // Cloudiness or Precipitation
                var popDetail = weather.WeatherDetails.FirstOrDefault(detail => detail.DetailsType == WeatherDetailsType.PoPCloudiness || detail.DetailsType == WeatherDetailsType.PoPChance);

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
                                    Source = WeatherUtils.GetWeatherIconURI(weather.WeatherIcon, false)
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
                                    HintStyle = AdaptiveTextStyle.Caption
                                },
                                new AdaptiveText()
                                {
                                    Text = string.Format("{0}: {1}", App.ResLoader.GetString("Temp_Label"), weather.CurTemp.RemoveNonDigitChars() + "º"),
                                    HintStyle = AdaptiveTextStyle.Caption
                                },
                                new AdaptiveText()
                                {
                                    Text = string.Format("{0}: {1}", App.ResLoader.GetString("FeelsLike_Label"), weather.WeatherDetails.FirstOrDefault(detail => detail.DetailsType == WeatherDetailsType.FeelsLike)?.Value),
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                                new AdaptiveText()
                                {
                                    Text = string.Format("{0}: {1}", App.ResLoader.GetString("Wind_Label"), weather.WeatherDetails.FirstOrDefault(detail => detail.DetailsType == WeatherDetailsType.WindSpeed)?.Value),
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
                                Source = WeatherUtils.GetWeatherIconURI(hrforecast.WeatherIcon, false)
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

        private static TileBindingContentAdaptive GenerateCondition(WeatherNowViewModel weather, List<ForecastItemViewModel> forecasts, ForecastTileType forecastTileType)
        {
            var userlang = GlobalizationPreferences.Languages[0];
            var culture = new CultureInfo(userlang);

            var content = new TileBindingContentAdaptive();
            SetContentBackground(content, weather);

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
                                    Text = weather.CurTemp.RemoveNonDigitChars() + "º",
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
                                    Source = WeatherUtils.GetWeatherIconURI(weather.WeatherIcon, false)
                                },
                                new AdaptiveText()
                                {
                                    Text = weather.CurTemp.RemoveNonDigitChars() + "º",
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
                            Source = WeatherUtils.GetWeatherIconURI(weather.WeatherIcon, false)
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
                            HintStyle = AdaptiveTextStyle.Caption
                        },
                        new AdaptiveText()
                        {
                            Text = string.Format("{0}: {1}", App.ResLoader.GetString("Temp_Label"), weather.CurTemp.RemoveNonDigitChars() + "º"),
                            HintStyle = AdaptiveTextStyle.Caption
                        },
                        new AdaptiveText()
                        {
                            Text = string.Format("{0}: {1}", App.ResLoader.GetString("FeelsLike_Label"), weather.WeatherDetails.FirstOrDefault(detail => detail.DetailsType == WeatherDetailsType.FeelsLike)?.Value),
                            HintStyle = AdaptiveTextStyle.CaptionSubtle
                        },
                        new AdaptiveText()
                        {
                            Text = string.Format("{0}: {1}", App.ResLoader.GetString("Wind_Label"), weather.WeatherDetails.FirstOrDefault(detail => detail.DetailsType == WeatherDetailsType.WindSpeed)?.Value),
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
                                    Source = WeatherUtils.GetWeatherIconURI(weather.WeatherIcon, false)
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
                                    HintStyle = AdaptiveTextStyle.Caption
                                },
                                new AdaptiveText()
                                {
                                    Text = string.Format("{0}: {1}", App.ResLoader.GetString("Temp_Label"), weather.CurTemp.RemoveNonDigitChars() + "º"),
                                    HintStyle = AdaptiveTextStyle.Caption
                                },
                                new AdaptiveText()
                                {
                                    Text = string.Format("{0}: {1}", App.ResLoader.GetString("FeelsLike_Label"), weather.WeatherDetails.FirstOrDefault(detail => detail.DetailsType == WeatherDetailsType.FeelsLike)?.Value),
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                                new AdaptiveText()
                                {
                                    Text = string.Format("{0}: {1}", App.ResLoader.GetString("Wind_Label"), weather.WeatherDetails.FirstOrDefault(detail => detail.DetailsType == WeatherDetailsType.WindSpeed)?.Value),
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

                var forecastGroup = new AdaptiveGroup();
                bool hasForecast = forecasts?.Count > 0;

                if (hasForecast)
                {
                    int forecastLength = Math.Min(forecasts.Count, LARGE_FORECAST_LENGTH);

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
                                Source = WeatherUtils.GetWeatherIconURI(forecast.WeatherIcon, false)
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

                content.Children.Add(conditionGroup);
                content.Children.Add(text);
                if (hasForecast)
                    content.Children.Add(forecastGroup);
            }

            return content;
        }

        private static async Task UpdateContent(TileUpdater tileUpdater, LocationData location, WeatherNowViewModel weather)
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
                        BaseUri = new Uri("Assets/WeatherIcons/png/", UriKind.Relative),
                        DisplayName = weather.Location?.Ellipsize(40),
                        TileSmall = new TileBinding()
                        {
                            Branding = TileBranding.None,
                            Content = GenerateHrForecast(weather, hrforecasts, ForecastTileType.Small),
                        },
                        TileMedium = new TileBinding()
                        {
                            // Mini forecast (3-hr)
                            Branding = TileBranding.Name,
                            Content = GenerateHrForecast(weather, hrforecasts, ForecastTileType.Medium),
                        },
                        TileWide = new TileBinding()
                        {
                            // 5-hr forecast
                            Branding = TileBranding.Name,
                            Content = GenerateHrForecast(weather, hrforecasts, ForecastTileType.Wide),
                        },
                        TileLarge = new TileBinding()
                        {
                            Branding = TileBranding.Name,
                            Content = GenerateHrForecast(weather, hrforecasts, ForecastTileType.Large),
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
                        BaseUri = new Uri("Assets/WeatherIcons/png/", UriKind.Relative),
                        DisplayName = weather.Location?.Ellipsize(40),
                        TileSmall = new TileBinding()
                        {
                            Branding = TileBranding.None,
                            Content = GenerateForecast(weather, forecasts, ForecastTileType.Small),
                        },
                        TileMedium = new TileBinding()
                        {
                            // Mini forecast (2-day)
                            Branding = TileBranding.Name,
                            Content = GenerateForecast(weather, forecasts, ForecastTileType.Medium),
                        },
                        TileWide = new TileBinding()
                        {
                            // 5-day forecast
                            Branding = TileBranding.Name,
                            Content = GenerateForecast(weather, forecasts, ForecastTileType.Wide),
                        },
                        /*
                         * All ready shown in current tile
                        TileLarge = new TileBinding()
                        {
                            Branding = TileBranding.Name,
                            Content = GenerateForecast(weather, ForecastTileType.Large),
                        }
                        */
                    }
                };
            }

            currentTileContent = new TileContent()
            {
                Visual = new TileVisual()
                {
                    BaseUri = new Uri("Assets/WeatherIcons/png/", UriKind.Relative),
                    DisplayName = weather.Location?.Ellipsize(40),
                    TileSmall = new TileBinding()
                    {
                        Branding = TileBranding.None,
                        Content = GenerateCondition(weather, forecasts, ForecastTileType.Small),
                    },
                    TileMedium = new TileBinding()
                    {
                        // Mini forecast (2-day)
                        Branding = TileBranding.Name,
                        Content = GenerateCondition(weather, forecasts, ForecastTileType.Medium),
                    },
                    TileWide = new TileBinding()
                    {
                        // 5-day forecast
                        Branding = TileBranding.Name,
                        Content = GenerateCondition(weather, forecasts, ForecastTileType.Wide),
                    },
                    TileLarge = new TileBinding()
                    {
                        Branding = TileBranding.Name,
                        Content = GenerateCondition(weather, forecasts, ForecastTileType.Large),
                    },
                    LockDetailedStatus1 = weather.Location?.Ellipsize(40),
                    LockDetailedStatus2 = String.Format("{0} - {1}", weather.CurTemp?.RemoveNonDigitChars() + "º", weather.CurCondition?.Ellipsize(33)),
                    LockDetailedStatus3 = String.Format("{0} | {1}", weather.HiTemp, weather.LoTemp)
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

        private static async Task<List<ForecastItemViewModel>> GetForecasts(LocationData location)
        {
            var forecasts = await Settings.GetWeatherForecastData(location.query);
            var result = new List<ForecastItemViewModel>(LARGE_FORECAST_LENGTH);

            if (forecasts?.forecast?.Count > 0)
            {
                foreach (var fcast in forecasts?.forecast)
                {
                    result.Add(new ForecastItemViewModel(fcast));
                }
            }

            return result;
        }

        private static async Task<List<HourlyForecastItemViewModel>> GetHourlyForecasts(LocationData location)
        {
            var forecasts = await Settings.GetHourlyWeatherForecastDataByPageIndexByLimit(location.query, 0, LARGE_FORECAST_LENGTH);
            var result = new List<HourlyForecastItemViewModel>(LARGE_FORECAST_LENGTH);

            if (forecasts?.Count > 0)
            {
                foreach (var fcast in forecasts)
                {
                    result.Add(new HourlyForecastItemViewModel(fcast));
                }
            }

            return result;
        }

        public static Task TileUpdater(LocationData location)
        {
            // Check if Tile service is available
            if (!DeviceTypeHelper.IsTileSupported())
                return Task.CompletedTask;

            return Task.Run(async () =>
            {
                try
                {
                    var wloader = new WeatherDataLoader(location);
                    var weather = await AsyncTask.RunAsync(wloader.LoadWeatherData(
                                new WeatherRequest.Builder()
                                    .ForceRefresh(false)
                                    .LoadForecasts()
                                    .Build()));

                    if (weather != null)
                    {
                        var weatherView = new WeatherNowViewModel(weather);
                        await weatherView.UpdateBackground();
                        TileUpdater(location, weatherView);
                    }
                }
                catch (WeatherException wEx)
                {
                    Logger.WriteLine(LoggerLevel.Error, wEx);
                }
            });
        }

        public static Task TileUpdater(LocationData location, WeatherNowViewModel weather)
        {
            // Check if Tile service is available
            if (!DeviceTypeHelper.IsTileSupported())
                return Task.CompletedTask;

            return Task.Run(async () =>
            {
                if (weather.ImageData == null)
                    await weather.UpdateBackground();

                // And send the notification to the tile
                if (Equals(await Settings.GetHomeData(), location))
                {
                    // Update both primary and secondary tile if it exists
                    var appTileUpdater = TileUpdateManager.CreateTileUpdaterForApplication();
                    // Lock instance to avoid rare concurrency issue
                    // (when BGTask is running and tile is updated via WeatherNowPage)
                    lock (appTileUpdater)
                    {
                        UpdateContent(appTileUpdater, location, weather).Wait();
                    }
                    if (SecondaryTileUtils.Exists(location.query))
                    {
                        var tileUpdater = TileUpdateManager.CreateTileUpdaterForSecondaryTile(
                                SecondaryTileUtils.GetTileId(location.query));
                        lock (tileUpdater)
                        {
                            UpdateContent(tileUpdater, location, weather).Wait();
                        }
                    }
                }
                else
                {
                    // Update secondary tile
                    if (SecondaryTileUtils.Exists(location.query))
                    {
                        var tileUpdater = TileUpdateManager.CreateTileUpdaterForSecondaryTile(
                                SecondaryTileUtils.GetTileId(location.query));
                        // Lock instance to avoid rare concurrency issue
                        // (when BGTask is running and tile is updated via WeatherNowPage)
                        lock (tileUpdater)
                        {
                            UpdateContent(tileUpdater, location, weather).Wait();
                        }
                    }
                }
            });
        }
    }
}