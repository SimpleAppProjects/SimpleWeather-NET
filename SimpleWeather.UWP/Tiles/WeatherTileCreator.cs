using Microsoft.Toolkit.Uwp.Notifications;
using SimpleWeather.Controls;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
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

        private static WeatherManager wm = WeatherManager.GetInstance();
        public static bool TileUpdated { get; private set; }

        public enum ForecastTileType
        {
            Small,
            Medium,
            Wide,
            Large
        }

        private static TileBindingContentAdaptive GenerateForecast(WeatherNowViewModel weather, ForecastTileType forecastTileType)
        {
            var userlang = GlobalizationPreferences.Languages[0];
            var culture = new CultureInfo(userlang);

            var content = new TileBindingContentAdaptive
            {
                // Background URI
                BackgroundImage = new TileBackgroundImage()
                {
                    Source = weather.BackgroundURI,
                    HintOverlay = 50
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
                for (int i = 0; i < MEDIUM_FORECAST_LENGTH; i++)
                {
                    var forecast = weather.Forecasts[i];

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
                                Source = wm.GetWeatherIconURI(forecast.WeatherIcon)
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

                int forecastLength = WIDE_FORECAST_LENGTH;
                if (weather.Forecasts.Count < forecastLength)
                    forecastLength = weather.Forecasts.Count;

                for (int i = 0; i < forecastLength; i++)
                {
                    var forecast = weather.Forecasts[i];

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
                                Source = wm.GetWeatherIconURI(forecast.WeatherIcon)
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

                int forecastLength = LARGE_FORECAST_LENGTH;
                if (weather.Forecasts.Count < forecastLength)
                    forecastLength = weather.Forecasts.Count;

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
                                    Source = wm.GetWeatherIconURI(weather.WeatherIcon)
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
                                    Text = weather.CurCondition,
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
                    var forecast = weather.Forecasts[i];

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
                                Source = wm.GetWeatherIconURI(forecast.WeatherIcon)
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

        private static TileBindingContentAdaptive GenerateHrForecast(WeatherNowViewModel weather, ForecastTileType forecastTileType)
        {
            var userlang = GlobalizationPreferences.Languages[0];
            var culture = new CultureInfo(userlang);

            var content = new TileBindingContentAdaptive
            {
                // Background URI
                BackgroundImage = new TileBackgroundImage()
                {
                    Source = weather.BackgroundURI,
                    HintOverlay = 50
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

                int forecastLength = MEDIUM_FORECAST_LENGTH;
                if (weather.Extras.HourlyForecast.Count < forecastLength)
                    forecastLength = weather.Extras.HourlyForecast.Count;

                // 3hr forecast
                for (int i = 0; i < forecastLength; i++)
                {
                    var hrforecast = weather.Extras.HourlyForecast[i];

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
                                Source = wm.GetWeatherIconURI(hrforecast.WeatherIcon)
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

                int forecastLength = LARGE_FORECAST_LENGTH;
                if (weather.Extras.HourlyForecast.Count < forecastLength)
                    forecastLength = weather.Extras.HourlyForecast.Count;

                for (int i = 0; i < forecastLength; i++)
                {
                    var hrforecast = weather.Extras.HourlyForecast[i];

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
                                Source = wm.GetWeatherIconURI(hrforecast.WeatherIcon)
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

                int forecastLength = LARGE_FORECAST_LENGTH;
                if (weather.Extras.HourlyForecast.Count < forecastLength)
                    forecastLength = weather.Extras.HourlyForecast.Count;

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
                                    Source = wm.GetWeatherIconURI(weather.WeatherIcon)
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
                                    Text = weather.CurCondition,
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
                    var hrforecast = weather.Extras.HourlyForecast[i];

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
                                Source = wm.GetWeatherIconURI(hrforecast.WeatherIcon)
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

        private static TileBindingContentAdaptive GenerateCondition(WeatherNowViewModel weather, ForecastTileType forecastTileType)
        {
            var userlang = GlobalizationPreferences.Languages[0];
            var culture = new CultureInfo(userlang);

            var content = new TileBindingContentAdaptive
            {
                // Background URI
                BackgroundImage = new TileBackgroundImage()
                {
                    Source = weather.BackgroundURI,
                    HintOverlay = 50
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
                                    Source = wm.GetWeatherIconURI(weather.WeatherIcon)
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
                            Source = wm.GetWeatherIconURI(weather.WeatherIcon)
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
                            Text = weather.CurCondition,
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
                var forecastGroup = new AdaptiveGroup();

                int forecastLength = LARGE_FORECAST_LENGTH;
                if (weather.Forecasts.Count < forecastLength)
                    forecastLength = weather.Forecasts.Count;

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
                                    Source = wm.GetWeatherIconURI(weather.WeatherIcon)
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
                                    Text = weather.CurCondition,
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
                    var forecast = weather.Forecasts[i];

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
                                Source = wm.GetWeatherIconURI(forecast.WeatherIcon)
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

        private static void UpdateContent(TileUpdater tileUpdater, WeatherNowViewModel weather)
        {
            bool hasHourly = weather.Extras.HourlyForecast.Count > 0;
            bool hasForecast = weather.Forecasts.Count > 0;
            TileContent forecastTileContent = null;
            TileContent currentTileContent = null;
            TileContent hrforecastTileContent = null;

            if (hasHourly)
            {
                hrforecastTileContent = new TileContent()
                {
                    Visual = new TileVisual()
                    {
                        DisplayName = weather.Location,
                        TileSmall = new TileBinding()
                        {
                            Branding = TileBranding.None,
                            Content = GenerateHrForecast(weather, ForecastTileType.Small),
                        },
                        TileMedium = new TileBinding()
                        {
                            // Mini forecast (3-hr)
                            Branding = TileBranding.Name,
                            Content = GenerateHrForecast(weather, ForecastTileType.Medium),
                        },
                        TileWide = new TileBinding()
                        {
                            // 5-hr forecast
                            Branding = TileBranding.Name,
                            Content = GenerateHrForecast(weather, ForecastTileType.Wide),
                        },
                        TileLarge = new TileBinding()
                        {
                            Branding = TileBranding.Name,
                            Content = GenerateHrForecast(weather, ForecastTileType.Large),
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
                        DisplayName = weather.Location,
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
                    DisplayName = weather.Location,
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

        public static async Task TileUpdater(LocationData location)
        {
            var wloader = new WeatherDataLoader(location);
            await AsyncTask.RunAsync(wloader.LoadWeatherData(
                new WeatherRequest.Builder()
                    .ForceRefresh(false)
                    .LoadForecasts()
                    .Build()));

            if (wloader.GetWeather() != null)
            {
                TileUpdater(location, new WeatherNowViewModel(wloader.GetWeather()));
            }
        }

        public static void TileUpdater(LocationData location, WeatherNowViewModel weather)
        {
            // And send the notification to the tile
            if (location.Equals(Settings.HomeData))
            {
                // Update both primary and secondary tile if it exists
                var appTileUpdater = TileUpdateManager.CreateTileUpdaterForApplication();
                // Lock instance to avoid rare concurrency issue
                // (when BGTask is running and tile is updated via WeatherNowPage)
                lock (appTileUpdater)
                {
                    UpdateContent(appTileUpdater, weather);
                }
                if (SecondaryTileUtils.Exists(location.query))
                {
                    var tileUpdater = TileUpdateManager.CreateTileUpdaterForSecondaryTile(
                            SecondaryTileUtils.GetTileId(location.query));
                    lock (tileUpdater)
                    {
                        UpdateContent(tileUpdater, weather);
                    }
                }

                TileUpdated = true;
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
                        UpdateContent(tileUpdater, weather);
                    }
                }
            }
        }
    }
}