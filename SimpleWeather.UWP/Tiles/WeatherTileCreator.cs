using Microsoft.Toolkit.Uwp.Notifications;
using SimpleWeather.Controls;
using SimpleWeather.Icons;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.UWP.Shared.Helpers;
using SimpleWeather.UWP.Utils;
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

        private static readonly WeatherIconsManager wim = WeatherIconsManager.GetInstance();

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
            if (UWP.Utils.FeatureSettings.TileBackgroundImage && weather.ImageData?.ImageUri != null)
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
            var culture = CultureUtils.UserCulture;

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
                                    Text = string.Format("{0}: {1}", App.ResLoader.GetString("label_temp"), weather.CurTemp.RemoveNonDigitChars() + "°"),
                                    //HintStyle = AdaptiveTextStyle.Caption
                                },
                                new AdaptiveText()
                                {
                                    Text = string.Format("{0}: {1}", App.ResLoader.GetString("label_feelslike"), weather.WeatherDetails.FirstOrDefault(detail => detail.DetailsType == WeatherDetailsType.FeelsLike)?.Value ?? WeatherIcons.EM_DASH),
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                                new AdaptiveText()
                                {
                                    Text = string.Format("{0}: {1}", App.ResLoader.GetString("label_wind"), weather.WeatherDetails.FirstOrDefault(detail => detail.DetailsType == WeatherDetailsType.WindSpeed)?.Value ?? WeatherIcons.EM_DASH),
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

        private static TileBindingContentAdaptive GenerateHrForecast(WeatherNowViewModel weather, List<HourlyForecastItemViewModel> forecasts, ForecastTileType forecastTileType)
        {
            var culture = CultureUtils.UserCulture;

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

                string poplabel = weather.WeatherSource.Equals(WeatherAPI.OpenWeatherMap) || weather.WeatherSource.Equals(WeatherAPI.MetNo) ?
                    App.ResLoader.GetString("label_cloudiness") : App.ResLoader.GetString("label_precipitation"); // Cloudiness or Precipitation
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
                                    Text = string.Format("{0}: {1}", App.ResLoader.GetString("label_temp"), weather.CurTemp.RemoveNonDigitChars() + "°"),
                                    //HintStyle = AdaptiveTextStyle.Caption
                                },
                                new AdaptiveText()
                                {
                                    Text = string.Format("{0}: {1}", App.ResLoader.GetString("label_feelslike"), weather.WeatherDetails.FirstOrDefault(detail => detail.DetailsType == WeatherDetailsType.FeelsLike)?.Value ?? WeatherIcons.EM_DASH),
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                                new AdaptiveText()
                                {
                                    Text = string.Format("{0}: {1}", App.ResLoader.GetString("label_wind"), weather.WeatherDetails.FirstOrDefault(detail => detail.DetailsType == WeatherDetailsType.WindSpeed)?.Value ?? WeatherIcons.EM_DASH),
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

        private static TileBindingContentAdaptive GenerateCondition(WeatherNowViewModel weather, List<ForecastItemViewModel> forecasts, ForecastTileType forecastTileType)
        {
            var culture = CultureUtils.UserCulture;

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
                            Text = string.Format("{0}: {1}", App.ResLoader.GetString("label_temp"), weather.CurTemp.RemoveNonDigitChars() + "°"),
                            //HintStyle = AdaptiveTextStyle.Caption
                        },
                        new AdaptiveText()
                        {
                            Text = string.Format("{0}: {1}", App.ResLoader.GetString("label_feelslike"), weather.WeatherDetails.FirstOrDefault(detail => detail.DetailsType == WeatherDetailsType.FeelsLike)?.Value ?? WeatherIcons.EM_DASH),
                            HintStyle = AdaptiveTextStyle.CaptionSubtle
                        },
                        new AdaptiveText()
                        {
                            Text = string.Format("{0}: {1}", App.ResLoader.GetString("label_wind"), weather.WeatherDetails.FirstOrDefault(detail => detail.DetailsType == WeatherDetailsType.WindSpeed)?.Value ?? WeatherIcons.EM_DASH),
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
                                    Text = string.Format("{0}: {1}", App.ResLoader.GetString("label_temp"), weather.CurTemp.RemoveNonDigitChars() + "°"),
                                    //HintStyle = AdaptiveTextStyle.Caption
                                },
                                new AdaptiveText()
                                {
                                    Text = string.Format("{0}: {1}", App.ResLoader.GetString("label_feelslike"), weather.WeatherDetails.FirstOrDefault(detail => detail.DetailsType == WeatherDetailsType.FeelsLike)?.Value ?? WeatherIcons.EM_DASH),
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                                new AdaptiveText()
                                {
                                    Text = string.Format("{0}: {1}", App.ResLoader.GetString("label_wind"), weather.WeatherDetails.FirstOrDefault(detail => detail.DetailsType == WeatherDetailsType.WindSpeed)?.Value ?? WeatherIcons.EM_DASH),
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
                        BaseUri = new Uri(WeatherIconsManager.GetPNGBaseUri(), UriKind.Absolute),
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
                        BaseUri = new Uri(WeatherIconsManager.GetPNGBaseUri(), UriKind.Absolute),
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
                    BaseUri = new Uri(WeatherIconsManager.GetPNGBaseUri(), UriKind.Absolute),
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

        private static async Task<List<ForecastItemViewModel>> GetForecasts(LocationData location)
        {
            var forecasts = await Settings.GetWeatherForecastData(location.query);
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

        private static async Task<List<HourlyForecastItemViewModel>> GetHourlyForecasts(LocationData location)
        {
            var now = DateTimeOffset.Now.ToOffset(location.tz_offset);
            var hrInterval = WeatherManager.GetInstance().HourlyForecastInterval;
            var dateBlob = now.AddHours(-(long)(hrInterval * 0.5d)).Trim(TimeSpan.TicksPerHour).ToString("yyyy-MM-dd HH:mm:ss zzzz", CultureInfo.InvariantCulture);
            var forecasts = await Settings.GetHourlyWeatherForecastDataByPageIndexByLimitFilterByDate(location.query, 0, LARGE_FORECAST_LENGTH, dateBlob);
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

        public static async Task TileUpdater(LocationData location)
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
                    var weatherView = new WeatherNowViewModel(weather);
                    await TileUpdater(location, weatherView);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Error, ex);
            }
        }

        private static async Task TileUpdater(LocationData location, WeatherNowViewModel weather)
        {
            if (weather.ImageData == null)
                await weather.UpdateBackground();

            // And send the notification to the tile
            if (location.locationType == LocationType.GPS || Equals(await Settings.GetHomeData(), location))
            {
                // Update primary tile and lockscreen info (if exists)
                var appTileUpdater = TileUpdateManager.CreateTileUpdaterForApplication();
                // Lock instance to avoid rare concurrency issue
                // (when BGTask is running and tile is updated via WeatherNowPage)
                lock (appTileUpdater)
                {
                    UpdateContent(appTileUpdater, location, weather).Wait();
                }

                // Update secondary tile if exists
                var query = location.locationType == LocationType.GPS ? Constants.KEY_GPS : location.query;
                if (SecondaryTileUtils.Exists(query))
                {
                    var tileUpdater = TileUpdateManager.CreateTileUpdaterForSecondaryTile(
                            SecondaryTileUtils.GetTileId(query));
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
        }
    }
}