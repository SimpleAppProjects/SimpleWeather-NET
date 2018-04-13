using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Notifications;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using Windows.System.UserProfile;
using System.Globalization;

namespace SimpleWeather.UWP.Helpers
{
    public static class WeatherTileCreator
    {
        const int MEDIUM_FORECAST_LENGTH = 3;
        const int WIDE_FORECAST_LENGTH = 3;
        const int LARGE_FORECAST_LENGTH = 5;

        private static WeatherManager wm = WeatherManager.GetInstance();
        public static bool TileUpdated = false;

        public enum ForecastTileType
        {
            Small,
            Medium,
            Wide,
            Large
        }

        private static TileBindingContentAdaptive GenerateForecast(Weather weather, ForecastTileType forecastTileType)
        {
            var userlang = GlobalizationPreferences.Languages[0];
            var culture = new CultureInfo(userlang);

            var content = new TileBindingContentAdaptive
            {
                // Background URI
                BackgroundImage = new TileBackgroundImage()
                {
                    Source = wm.GetBackgroundURI(weather)
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
                                    Text = weather.update_time.ToString("ddd", culture),
                                    HintStyle = AdaptiveTextStyle.Base,
                                    HintAlign = AdaptiveTextAlign.Center
                                },
                                new AdaptiveText()
                                {
                                    Text = (Settings.IsFahrenheit ? Math.Round(weather.condition.temp_f) : Math.Round(weather.condition.temp_c)) + "º",
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

                // 3day forecast
                for (int i = 0; i < MEDIUM_FORECAST_LENGTH; i++)
                {
                    var forecast = weather.forecast[i];

                    var dateSubgroup = new AdaptiveSubgroup()
                    {
                        HintWeight = 1,
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = forecast.date.ToString("ddd", culture),
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
                                Source = wm.GetWeatherIconURI(forecast.icon)
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
                                Text = forecast.date.ToString("ddd", culture),
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
                                Source = wm.GetWeatherIconURI(forecast.icon)
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
                                    Source = wm.GetWeatherIconURI(condition.icon)
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
                                    Text = string.Format("{0}: {1}", App.ResLoader.GetString("Temp_Label"),
                                        (Settings.IsFahrenheit ? Math.Round(weather.condition.temp_f) : Math.Round(weather.condition.temp_c)) + "º"),
                                    HintStyle = AdaptiveTextStyle.Caption
                                },
                                new AdaptiveText()
                                {
                                    Text = string.Format("{0}: {1}", App.ResLoader.GetString("FeelsLike_Label"),
                                        (Settings.IsFahrenheit ? Math.Round(weather.condition.feelslike_f) : Math.Round(weather.condition.feelslike_c)) + "º"),
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                                new AdaptiveText()
                                {
                                    Text = string.Format("{0}: {1}", App.ResLoader.GetString("Wind_Label"),
                                        Settings.IsFahrenheit ? weather.condition.wind_mph.ToString() + " mph" : weather.condition.wind_kph.ToString() + " kph"),
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
                                Text = forecast.date.ToString("ddd", culture),
                                HintAlign = AdaptiveTextAlign.Center
                            },
                            new AdaptiveImage()
                            {
                                HintRemoveMargin = true,
                                Source = wm.GetWeatherIconURI(forecast.icon)
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

        private static TileBindingContentAdaptive GenerateHrForecast(Weather weather, ForecastTileType forecastTileType)
        {
            var userlang = GlobalizationPreferences.Languages[0];
            var culture = new CultureInfo(userlang);

            var content = new TileBindingContentAdaptive
            {
                // Background URI
                BackgroundImage = new TileBackgroundImage()
                {
                    Source = wm.GetBackgroundURI(weather)
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
                                    Text = weather.update_time.ToString("ddd", culture),
                                    HintStyle = AdaptiveTextStyle.Base,
                                    HintAlign = AdaptiveTextAlign.Center
                                },
                                new AdaptiveText()
                                {
                                    Text = (Settings.IsFahrenheit ? Math.Round(weather.condition.temp_f) : Math.Round(weather.condition.temp_c)) + "º",
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

                string timeformat = culture.DateTimeFormat.ShortTimePattern.Contains("H") ?
                    "HH" : "ht";

                int forecastLength = MEDIUM_FORECAST_LENGTH;
                if (weather.hr_forecast.Length < forecastLength)
                    forecastLength = weather.hr_forecast.Length;

                // 3hr forecast
                for (int i = 0; i < forecastLength; i++)
                {
                    var hrforecast = weather.hr_forecast[i];

                    var dateSubgroup = new AdaptiveSubgroup()
                    {
                        HintWeight = 1,
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = hrforecast.date.ToString(timeformat, culture).ToLower(),
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
                                Source = wm.GetWeatherIconURI(hrforecast.icon)
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
                                Text = (Settings.IsFahrenheit ? hrforecast.high_f : hrforecast.high_c) + "º",
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
                if (weather.hr_forecast.Length < forecastLength)
                    forecastLength = weather.hr_forecast.Length;

                for (int i = 0; i < forecastLength; i++)
                {
                    var hrforecast = weather.hr_forecast[i];

                    var subgroup = new AdaptiveSubgroup()
                    {
                        HintWeight = 1,
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = hrforecast.date.ToString(timeformat, culture).ToLower(),
                                HintAlign = AdaptiveTextAlign.Center
                            },
                            new AdaptiveImage()
                            {
                                HintRemoveMargin = true,
                                Source = wm.GetWeatherIconURI(hrforecast.icon)
                            },
                            new AdaptiveText()
                            {
                                Text = (Settings.IsFahrenheit ? hrforecast.high_f : hrforecast.high_c) + "º",
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

                string timeformat = culture.DateTimeFormat.ShortTimePattern.Contains("H") ?
                    "HH" : "ht";
                string poplabel = (weather.source.Equals(WeatherAPI.OpenWeatherMap) || weather.source.Equals(WeatherAPI.MetNo)) ?
                    App.ResLoader.GetString("Cloudiness_Label") : App.ResLoader.GetString("Precipitation_Label"); // Cloudiness or Precipitation

                int forecastLength = LARGE_FORECAST_LENGTH;
                if (weather.hr_forecast.Length < forecastLength)
                    forecastLength = weather.hr_forecast.Length;

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
                                    Source = wm.GetWeatherIconURI(weather.condition.icon)
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
                                    Text = string.Format("{0}: {1}", App.ResLoader.GetString("Temp_Label"),
                                        (Settings.IsFahrenheit ? Math.Round(weather.condition.temp_f) : Math.Round(weather.condition.temp_c)) + "º"),
                                    HintStyle = AdaptiveTextStyle.Caption
                                },
                                new AdaptiveText()
                                {
                                    Text = string.Format("{0}: {1}", App.ResLoader.GetString("FeelsLike_Label"),
                                        (Settings.IsFahrenheit ? Math.Round(weather.condition.feelslike_f) : Math.Round(weather.condition.feelslike_c)) + "º"),
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                                new AdaptiveText()
                                {
                                    Text = string.Format("{0}: {1}", App.ResLoader.GetString("Wind_Label"),
                                        Settings.IsFahrenheit ? weather.condition.wind_mph.ToString() + " mph" : weather.condition.wind_kph.ToString() + " kph"),
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                                new AdaptiveText()
                                {
                                    Text = weather.precipitation != null ?
                                        string.Format("{0}: {1}", poplabel, weather.precipitation.pop + "%") : "",
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
                    var hrforecast = weather.hr_forecast[i];

                    var subgroup = new AdaptiveSubgroup()
                    {
                        HintWeight = 1,
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = hrforecast.date.ToString(timeformat, culture).ToLower(),
                                HintAlign = AdaptiveTextAlign.Center
                            },
                            new AdaptiveImage()
                            {
                                HintRemoveMargin = true,
                                Source = wm.GetWeatherIconURI(hrforecast.icon)
                            },
                            new AdaptiveText()
                            {
                                Text = (Settings.IsFahrenheit ? hrforecast.high_f : hrforecast.high_c) + "º",
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

        private static TileBindingContentAdaptive GenerateCondition(Weather weather, ForecastTileType forecastTileType)
        {
            var userlang = GlobalizationPreferences.Languages[0];
            var culture = new CultureInfo(userlang);

            var content = new TileBindingContentAdaptive
            {
                // Background URI
                BackgroundImage = new TileBackgroundImage()
                {
                    Source = wm.GetBackgroundURI(weather)
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
                                    Text = weather.update_time.ToString("ddd", culture),
                                    HintStyle = AdaptiveTextStyle.Base,
                                    HintAlign = AdaptiveTextAlign.Center
                                },
                                new AdaptiveText()
                                {
                                    Text = (Settings.IsFahrenheit ? Math.Round(weather.condition.temp_f) : Math.Round(weather.condition.temp_c)) + "º",
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
                                    Source = wm.GetWeatherIconURI(weather.condition.icon)
                                },
                                new AdaptiveText()
                                {
                                    Text = (Settings.IsFahrenheit ? Math.Round(weather.condition.temp_f) : Math.Round(weather.condition.temp_c)) + "º",
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
                            Source = wm.GetWeatherIconURI(weather.condition.icon)
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
                            Text = string.Format("{0}: {1}", App.ResLoader.GetString("Temp_Label"),
                                (Settings.IsFahrenheit ? Math.Round(weather.condition.temp_f) : Math.Round(weather.condition.temp_c)) + "º"),
                            HintStyle = AdaptiveTextStyle.Caption
                        },
                        new AdaptiveText()
                        {
                            Text = string.Format("{0}: {1}", App.ResLoader.GetString("FeelsLike_Label"),
                                (Settings.IsFahrenheit ? Math.Round(weather.condition.feelslike_f) : Math.Round(weather.condition.feelslike_c)) + "º"),
                            HintStyle = AdaptiveTextStyle.CaptionSubtle
                        },
                        new AdaptiveText()
                        {
                            Text = string.Format("{0}: {1}", App.ResLoader.GetString("Wind_Label"),
                                Settings.IsFahrenheit ? weather.condition.wind_mph.ToString() + " mph" : weather.condition.wind_kph.ToString() + " kph"),
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
                                    Source = wm.GetWeatherIconURI(weather.condition.icon)
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
                                    Text = string.Format("{0}: {1}", App.ResLoader.GetString("Temp_Label"),
                                        (Settings.IsFahrenheit ? Math.Round(weather.condition.temp_f) : Math.Round(weather.condition.temp_c)) + "º"),
                                    HintStyle = AdaptiveTextStyle.Caption
                                },
                                new AdaptiveText()
                                {
                                    Text = string.Format("{0}: {1}", App.ResLoader.GetString("FeelsLike_Label"),
                                        (Settings.IsFahrenheit ? Math.Round(weather.condition.feelslike_f) : Math.Round(weather.condition.feelslike_c)) + "º"),
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                                new AdaptiveText()
                                {
                                    Text = string.Format("{0}: {1}", App.ResLoader.GetString("Wind_Label"),
                                        Settings.IsFahrenheit ? weather.condition.wind_mph.ToString() + " mph" : weather.condition.wind_kph.ToString() + " kph"),
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
                                Text = forecast.date.ToString("ddd", culture),
                                HintAlign = AdaptiveTextAlign.Center
                            },
                            new AdaptiveImage()
                            {
                                HintRemoveMargin = true,
                                Source = wm.GetWeatherIconURI(forecast.icon)
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

        public static void TileUpdater(Weather weather)
        {
            bool hasHourly = weather.hr_forecast != null && weather.hr_forecast.Length > 0;
            TileContent forecastTileContent = null;
            TileContent currentTileContent = null;
            TileContent hrforecastTileContent = null;

            if (hasHourly)
            {
                hrforecastTileContent = new TileContent()
                {
                    Visual = new TileVisual()
                    {
                        DisplayName = weather.location.name,
                        /*
                         * Too small
                        TileSmall = new TileBinding()
                        {
                            Branding = TileBranding.None,
                            Content = GenerateHrForecast(weather, ForecastTileType.Small),
                        },
                        */
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

            forecastTileContent = new TileContent()
            {
                Visual = new TileVisual()
                {
                    DisplayName = weather.location.name,
                    /*
                     * All ready shown in current tile
                    TileSmall = new TileBinding()
                    {
                        Branding = TileBranding.None,
                        Content = GenerateForecast(weather, ForecastTileType.Small),
                    },
                    */
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

            currentTileContent = new TileContent()
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
            if (hasHourly)
            {
                var hrforecastTileNotif = new TileNotification(hrforecastTileContent.GetXml()) { Tag = "Hourly Forecast" };
                tileUpdater.Update(hrforecastTileNotif);
            }
            TileUpdated = true;
        }
    }
}
