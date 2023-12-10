using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Windows.Widgets.Providers;
using Newtonsoft.Json;
using SimpleWeather.Common.Controls;
using SimpleWeather.Icons;
using SimpleWeather.NET.Widgets.Model;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.NET.Widgets.Templates
{
    internal class WeatherWidgetCreator : AbstractWidgetCreator
    {
        private const int MAX_FORECAST_LENGTH = 5;

        public override Task<string> BuildUpdate(string widgetId, WidgetInfo info)
        {
            if (widgetId == null || info == null || !(WidgetUtils.IdExists(widgetId) || WidgetUtils.IsGPS(widgetId))) return Task.FromResult<string>(null);

            return Task.Run(() =>
            {
                try
                {
                    var str = Minify($@"
                        {{
                            ""$schema"": ""http://adaptivecards.io/schemas/adaptive-card.json"",
                            ""type"": ""AdaptiveCard"",
                            ""verticalContentAlignment"": ""Top"",
                            ""version"": ""1.5"",
                            ""backgroundImage"": {GenerateContentBackground()},
                            ""body"": [
                                {GenerateCondition()},
                                {{
                                    ""type"": ""TextBlock"",
                                    ""size"": ""Small"",
                                    ""horizontalAlignment"": ""Center"",
                                    ""spacing"": ""None"",
                                    ""text"": ""&#8201;"",
                                    ""$when"": ""${{$host.widgetSize==\""medium\"" || $host.widgetSize==\""large\""}}""
                                }},
                                {GenerateForecast()},
                                {{
                                    ""type"": ""TextBlock"",
                                    ""size"": ""Small"",
                                    ""horizontalAlignment"": ""Center"",
                                    ""spacing"": ""None"",
                                    ""text"": ""&#8201;"",
                                    ""$when"": ""${{$host.widgetSize==\""large\""}}""
                                }},
                                {GenerateHourlyForecast()},
                                {{
                                    ""type"": ""TextBlock"",
                                    ""size"": ""Default"",
                                    ""weight"": ""Bolder"",
                                    ""text"": ""${{current.location}}"",
                                    ""horizontalAlignment"": ""Center"",
                                    ""fontType"": ""Default"",
                                    ""spacing"": ""Medium"",
                                }},
                            ]
                        }}
                    ");

                    return str;
                }
                catch (Exception e)
                {
                    Logger.WriteLine(LoggerLevel.Error, e);
                }

                return null;
            });
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

        private static async Task<string> BuildWidgetData(
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

            var json = System.Text.Json.JsonSerializer.Serialize(widgetData, new System.Text.Json.JsonSerializerOptions()
            {
                IncludeFields = true
            });

            return json;
        }

        private static string GenerateCondition()
        {
            return $@"
                {{
                    ""type"": ""ColumnSet"",
                    ""columns"": [
                        {{
                            ""type"": ""Column"",
                            ""width"": ""auto"",
                            ""items"": [
                                {{
                                    ""type"": ""Image"",
                                    ""style"": ""Default"",
                                    ""size"": ""Medium"",
                                    ""url"": ""${{current.weatherIcon.light}}"",
                                    ""$when"": ""${{$host.hostTheme==\""light\""}}""
                                }},
                                {{
                                    ""type"": ""Image"",
                                    ""style"": ""Default"",
                                    ""size"": ""Medium"",
                                    ""url"": ""${{current.weatherIcon.dark}}"",
                                    ""$when"": ""${{$host.hostTheme==\""dark\""}}""
                                }}
                            ]
                        }},
                        {{
                            ""type"": ""Column"",
                            ""width"": ""stretch"",
                            ""verticalContentAlignment"": ""Center"",
                            ""height"": ""stretch"",
                            ""items"": [
                                {{
                                    ""type"": ""TextBlock"",
                                    ""weight"": ""Default"",
                                    ""text"": ""${{current.temp}}"",
                                    ""size"": ""ExtraLarge"",
                                    ""fontType"": ""Default"",
                                    ""style"": ""heading"",
                                    ""horizontalAlignment"": ""Left"",
                                    ""wrap"": true,
                                    ""isSubtle"": true,
                                    ""spacing"": ""None""
                                }}
                            ]
                        }},
                        {{
                            ""type"": ""Column"",
                            ""width"": ""auto"",
                            ""verticalContentAlignment"": ""Center"",
                            ""spacing"": ""Small"",
                            ""items"": [
                                {{
                                    ""type"": ""RichTextBlock"",
                                    ""inlines"": [
                                        {{
                                            ""type"": ""TextRun"",
                                            ""text"": ""{ResStrings.label_feelslike}: "",
                                            ""size"": ""Small"",
                                            ""weight"": ""Bolder""
                                        }},
                                        {{
                                            ""type"": ""TextRun"",
                                            ""text"": ""${{current.feelsLike}}"",
                                            ""size"": ""Small"",
                                            ""weight"": ""Default""
                                        }}
                                    ],
                                    ""spacing"": ""None""
                                }},
                                {{
                                    ""type"": ""RichTextBlock"",
                                    ""inlines"": [
                                        {{
                                            ""type"": ""TextRun"",
                                            ""text"": ""{ResStrings.label_wind}: "",
                                            ""size"": ""Small"",
                                            ""weight"": ""Bolder""
                                        }},
                                        {{
                                            ""type"": ""TextRun"",
                                            ""text"": ""${{current.windSpeed}}"",
                                            ""size"": ""Small"",
                                            ""weight"": ""Default""
                                        }}
                                    ],
                                    ""spacing"": ""None""
                                }},
                                {{
                                    ""type"": ""RichTextBlock"",
                                    ""inlines"": [
                                        {{
                                            ""type"": ""TextRun"",
                                            ""text"": ""{ResStrings.label_chance}: "",
                                            ""size"": ""Small"",
                                            ""weight"": ""Bolder""
                                        }},
                                        {{
                                            ""type"": ""TextRun"",
                                            ""text"": ""${{current.chance}}"",
                                            ""size"": ""Small"",
                                            ""weight"": ""Default""
                                        }}
                                    ],
                                    ""spacing"": ""None""
                                }},
                            ]
                        }}
                    ]
                }}
            ";
        }

        private static string GenerateContentBackground()
        {
            return @"
                {
                    ""fillMode"": ""Cover"",
                    ""horizontalAlignment"": ""Center"",
                    ""verticalAlignment"": ""Center"",
                    ""url"": ""${current.background}""
                }
            ";
        }

        private static string GenerateForecast()
        {
            return $@"
                {{
                    ""type"": ""ColumnSet"",
                    ""columns"": [
                        {{
                            ""type"": ""Column"",
                            ""width"": 1,
                            ""items"": [
                                {{
                                    ""type"": ""TextBlock"",
                                    ""text"": ""${{date}}"",
                                    ""horizontalAlignment"": ""Center"",
                                    ""size"": ""Default"",
                                    ""spacing"": ""None"",
                                    ""weight"": ""Bolder""
                                }},
                                {{
                                    ""type"": ""Image"",
                                    ""url"": ""${{weatherIcon.light}}"",
                                    ""width"": ""40px"",
                                    ""height"": ""40px"",
                                    ""horizontalAlignment"": ""Center"",
                                    ""spacing"": ""Medium"",
                                    ""size"": ""Medium"",
                                    ""$when"": ""${{$host.hostTheme==\""light\""}}""
                                }},
                                {{
                                    ""type"": ""Image"",
                                    ""url"": ""${{weatherIcon.dark}}"",
                                    ""width"": ""40px"",
                                    ""height"": ""40px"",
                                    ""horizontalAlignment"": ""Center"",
                                    ""spacing"": ""Medium"",
                                    ""size"": ""Medium"",
                                    ""$when"": ""${{$host.hostTheme==\""dark\""}}""
                                }},
                                {{
                                    ""type"": ""TextBlock"",
                                    ""size"": ""Small"",
                                    ""horizontalAlignment"": ""Center"",
                                    ""spacing"": ""None"",
                                    ""text"": ""&#8201;""
                                }},
                                {{
                                    ""type"": ""TextBlock"",
                                    ""text"": ""${{hi}}"",
                                    ""horizontalAlignment"": ""Center"",
                                    ""spacing"": ""None"",
                                    ""weight"": ""Bolder"",
                                    ""size"": ""Default"",
                                    ""height"": ""Stretch"",
                                }},
                                {{
                                    ""type"": ""TextBlock"",
                                    ""text"": ""${{lo}}"",
                                    ""horizontalAlignment"": ""Center"",
                                    ""spacing"": ""None"",
                                    ""weight"": ""Bolder"",
                                    ""size"": ""Default"",
                                    ""height"": ""Stretch"",
                                    ""isSubtle"": true
                                }},
                            ],
                            ""spacing"": ""None"",
                            ""verticalContentAlignment"": ""Center"",
                            ""$data"": ""${{forecast}}""
                        }}
                    ],
                    ""$when"": ""${{($host.widgetSize==\""medium\"" || $host.widgetSize==\""large\"") && showForecast == true}}""
                }}
            ";
        }

        private static string GenerateHourlyForecast()
        {
            return @"
            {
                ""type"": ""ColumnSet"",
                ""columns"": [
                    {
                        ""type"": ""Column"",
                        ""width"": 1,
                        ""items"": [
                            {
                                ""type"": ""TextBlock"",
                                ""text"": ""${date}"",
                                ""horizontalAlignment"": ""Center"",
                                ""size"": ""Default"",
                                ""spacing"": ""None"",
                                ""weight"": ""Bolder""
                            },
                            {
                                ""type"": ""Image"",
                                ""url"": ""${weatherIcon.light}"",
                                ""width"": ""40px"",
                                ""height"": ""40px"",
                                ""horizontalAlignment"": ""Center"",
                                ""spacing"": ""Medium"",
                                ""size"": ""Medium"",
                                ""$when"": ""${$host.hostTheme==\""light\""}""
                            },
                            {
                                ""type"": ""Image"",
                                ""url"": ""${weatherIcon.dark}"",
                                ""width"": ""40px"",
                                ""height"": ""40px"",
                                ""horizontalAlignment"": ""Center"",
                                ""spacing"": ""Medium"",
                                ""size"": ""Medium"",
                                ""$when"": ""${$host.hostTheme==\""dark\""}""
                            },
                            {
                                ""type"": ""TextBlock"",
                                ""size"": ""Small"",
                                ""horizontalAlignment"": ""Center"",
                                ""spacing"": ""None"",
                                ""text"": ""&#8201;""
                            },
                            {
                                ""type"": ""TextBlock"",
                                ""text"": ""${hi}"",
                                ""horizontalAlignment"": ""Center"",
                                ""spacing"": ""None"",
                                ""weight"": ""Bolder"",
                                ""size"": ""Default"",
                                ""height"": ""stretch""
                            },
                            {
                                ""type"": ""ColumnSet"",
                                ""spacing"": ""Small"",
                                ""columns"": [
                                    {
                                        ""type"": ""Column"",
                                        ""width"": 1,
                                        ""spacing"": ""None"",
                                        ""verticalContentAlignment"": ""Center"",
                                        ""items"": [
                                            {
                                                ""type"": ""Image"",
                                                ""horizontalAlignment"": ""Center"",
                                                ""spacing"": ""None"",
                                                ""size"": ""Small"",
                                                ""url"": ""${chanceIcon.light}"",
                                                ""$when"": ""${$host.hostTheme==\""light\""}""
                                            },
                                            {
                                                ""type"": ""Image"",
                                                ""horizontalAlignment"": ""Center"",
                                                ""spacing"": ""None"",
                                                ""size"": ""Small"",
                                                ""url"": ""${chanceIcon.dark}"",
                                                ""$when"": ""${$host.hostTheme==\""dark\""}""
                                            }
                                        ]
                                    },
                                    {
                                        ""type"": ""Column"",
                                        ""width"": 2,
                                        ""spacing"": ""None"",
                                        ""verticalContentAlignment"": ""Center"",
                                        ""items"": [
                                            {
                                                ""type"": ""TextBlock"",
                                                ""text"": ""${chance}"",
                                                ""horizontalAlignment"": ""Center"",
                                                ""spacing"": ""None"",
                                                ""size"": ""Small""
                                            }
                                        ]
                                    }
                                ]
                            },
                        ],
                        ""spacing"": ""None"",
                        ""verticalContentAlignment"": ""Center"",
                        ""$data"": ""${hr_forecast}""
                    }
                ],
                ""$when"": ""${$host.widgetSize==\""large\"" && showHrForecast == true}""
            }";
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

        private static string Minify(string json)
        {
            using var jsonReader = new JsonTextReader(new StringReader(json));
            using var stringWriter = new StringWriter();
            using var jsonWriter = new JsonTextWriter(stringWriter);
            jsonWriter.Formatting = Formatting.None;
            jsonWriter.WriteToken(jsonReader);
            return stringWriter.ToString();
        }
    }
}
