using CommunityToolkit.Mvvm.ComponentModel;
using SimpleWeather.Common.Controls;
using SimpleWeather.ComponentModel;
using SimpleWeather.Database;
using SimpleWeather.LocationData;
using SimpleWeather.NET.Controls.Graphs;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using SQLite;
using System.ComponentModel;
using ResAQIndex = SimpleWeather.Resources.Strings.AQIndex;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.NET.Controls
{
    [Bindable(true)]
#if WINDOWS
    [WinRT.GeneratedBindableCustomProperty]
#endif
    public partial class AirQualityForecastViewModel : BaseViewModel, IDisposable
    {
        private LocationData.LocationData locationData;

        private ObservableItem<Forecasts> currentForecastsData;

        [ObservableProperty]
        public partial ICollection<IGraphData> AQIGraphData { get; set; }

        [ObservableProperty]
        public partial ICollection<AirQualityViewModel> AQIForecastData { get; set; }

        private readonly WeatherDatabase WeatherDB = WeatherDatabase.Instance;

        public AirQualityForecastViewModel()
        {
            currentForecastsData = new ObservableItem<Forecasts>();
            currentForecastsData.ItemValueChanged += CurrentForecastsData_ItemValueChanged;
        }

        public void UpdateForecasts(LocationData.LocationData location)
        {
            if (this.locationData == null || !Equals(this.locationData?.query, location?.query))
            {
                Task.Run(async () =>
                {
                    WeatherDB.Connection.GetConnection().TableChanged -= AirQualityForecastViewModel_TableChanged;

                    // Clone location data
                    this.locationData = new LocationQuery(location).ToLocationData();

                    currentForecastsData.SetValue(await WeatherDB.GetForecastData(location.query));

                    WeatherDB.Connection.GetConnection().TableChanged += AirQualityForecastViewModel_TableChanged;
                });
            }
        }

        private void AirQualityForecastViewModel_TableChanged(object sender, NotifyTableChangedEventArgs e)
        {
            if (locationData == null) return;

            Task.Run(async () =>
            {
                if (e?.Table?.TableName == Forecasts.TABLE_NAME)
                {
                    currentForecastsData.SetValue(await WeatherDB.GetForecastData(locationData.query));
                }
            });
        }

        private void CurrentForecastsData_ItemValueChanged(object sender, ObservableItemChangedEventArgs e)
        {
            if (e.NewValue is Forecasts fcasts)
            {
                RefreshAQIData(fcasts?.aqi_forecast);
            }
        }

        private void RefreshAQIData(IList<AirQuality> forecasts)
        {
            var now = DateTime.Now.Date;
            var enumerable = forecasts?.WhereNot(item => item.date?.CompareTo(now) < 0);
#if WINDOWS
            DispatcherQueue.TryEnqueue(() =>
#else
            Dispatcher.Dispatch(() =>
#endif
            {
                AQIGraphData = CreateGraphData(enumerable);
                AQIForecastData = enumerable?.Where(it => it.index.HasValue)?.Select(it => new AirQualityViewModel(it))?.ToList() ?? [];
            });
        }

        private ICollection<IGraphData> CreateGraphData(IEnumerable<AirQuality> enumerable)
        {
            var graphDataList = new List<IGraphData>(7);
            IGraphData aqiIndexData = null;
            IGraphData pm25Data = null;
            IGraphData pm10Data = null;
            IGraphData o3Data = null;
            IGraphData coData = null;
            IGraphData no2Data = null;
            IGraphData so2Data = null;
            
            var pm25UseRangeGraph = enumerable?.All(aqi => aqi.extras is { pm25_max: not null, pm25_min: not null }) ?? false;
            var pm10UseRangeGraph = enumerable?.All(aqi => aqi.extras is { pm10_max: not null, pm10_min: not null }) ?? false;
            var o3UseRangeGraph = enumerable?.All(aqi => aqi.extras is { o3_max: not null, o3_min: not null }) ?? false;

            enumerable?.ForEach(aqi =>
            {
                if (aqi.index.HasValue)
                {
                    if (aqiIndexData == null)
                    {
                        aqiIndexData = new BarGraphData()
                        {
                            GraphLabel = ResStrings.label_airquality
                        };
                    }

                    if (aqiIndexData is BarGraphData barGraphData)
                    {
                        if (barGraphData.GetDataSet() == null)
                        {
                            barGraphData.SetDataSet(new BarGraphDataSet(new List<BarGraphEntry>()).Apply(it =>
                            {
                                it.SetMinMax(0f);
                            }));
                        }

                        barGraphData.GetDataSet()?.AddEntry(new BarGraphEntry()
                        {
                            XLabel = aqi.date?.ToString("ddd", LocaleUtils.GetLocale()),
                            EntryData = new(aqi.index.Value, aqi.index.Value.ToInvariantString()),
                            FillColor = AirQualityUtils.GetColorFromIndex(aqi.index.Value)
                        });
                    }
                }

                if (aqi.pm25.HasValue)
                {
                    if (pm25Data == null)
                    {
                        pm25Data = pm25UseRangeGraph ? new ForecastRangeBarGraphData()
                        {
                            GraphLabel = ResAQIndex.units_pm25
                        } : new BarGraphData()
                        {
                            GraphLabel = ResAQIndex.units_pm25
                        };
                    }

                    if (pm25Data is ForecastRangeBarGraphData forecastRangeBarGraphData)
                    {
                        if (forecastRangeBarGraphData.GetDataSet() == null)
                        {
                            forecastRangeBarGraphData.SetDataSet(new ForecastRangeBarGraphDataSet(new List<ForecastRangeBarEntry>()));
                        }

                        forecastRangeBarGraphData.GetDataSet()?.AddEntry(new ForecastRangeBarEntry()
                        {
                            XLabel = aqi.date?.ToString("ddd", LocaleUtils.GetLocale())
                        }.Apply(entry =>
                        {
                            if (aqi.extras?.pm25_max.HasValue == true && aqi.extras?.pm25_min.HasValue == true)
                            {
                                entry.HiTempData = new YEntryData(aqi.extras.pm25_max.Value, aqi.extras.pm25_max.Value.ToInvariantString());
                                entry.LoTempData = new YEntryData(aqi.extras.pm25_min.Value, aqi.extras.pm25_min.Value.ToInvariantString());
                                entry.SetFillColors(AirQualityUtils.GetColorFromIndex(aqi.extras.pm25_max), AirQualityUtils.GetColorFromIndex(aqi.extras.pm25_min));
                            }
                            else if (aqi.extras?.pm25_max.HasValue == true)
                            {
                                entry.HiTempData = new YEntryData(aqi.extras.pm25_max.Value, aqi.extras.pm25_max.Value.ToInvariantString());
                                entry.SetFillColors(AirQualityUtils.GetColorFromIndex(aqi.extras.pm25_max));
                            }
                            else if (aqi.extras?.pm25_min.HasValue == true)
                            {
                                entry.LoTempData = new YEntryData(aqi.extras.pm25_min.Value, aqi.extras.pm25_min.Value.ToInvariantString());
                                entry.SetFillColors(AirQualityUtils.GetColorFromIndex(aqi.extras.pm25_min));
                            }
                            else
                            {
                                entry.HiTempData = new YEntryData(aqi.pm25.Value, aqi.pm25.Value.ToInvariantString());
                                entry.SetFillColors(AirQualityUtils.GetColorFromIndex(aqi.pm25));
                            }
                        }));
                    }
                    else if (pm25Data is BarGraphData barGraphData)
                    {
                        if (barGraphData.GetDataSet() == null)
                        {
                            barGraphData.SetDataSet(new BarGraphDataSet(new List<BarGraphEntry>()).Apply(it =>
                            {
                                it.SetMinMax(0f);
                            }));
                        }

                        barGraphData.GetDataSet()?.AddEntry(new BarGraphEntry()
                        {
                            XLabel = aqi.date?.ToString("ddd", LocaleUtils.GetLocale()),
                            EntryData = new(aqi.pm25.Value, aqi.pm25.Value.ToInvariantString()),
                            FillColor = AirQualityUtils.GetColorFromIndex(aqi.pm25.Value)
                        });
                    }
                }

                if (aqi.pm10.HasValue)
                {
                    if (pm10Data == null)
                    {
                        pm10Data = pm10UseRangeGraph ? new ForecastRangeBarGraphData()
                        {
                            GraphLabel = ResAQIndex.units_pm10
                        } : new BarGraphData()
                        {
                            GraphLabel = ResAQIndex.units_pm10
                        };
                    }

                    if (pm10Data is ForecastRangeBarGraphData forecastRangeBarGraphData)
                    {
                        if (forecastRangeBarGraphData.GetDataSet() == null)
                        {
                            forecastRangeBarGraphData.SetDataSet(new ForecastRangeBarGraphDataSet(new List<ForecastRangeBarEntry>()));
                        }

                        forecastRangeBarGraphData.GetDataSet()?.AddEntry(new ForecastRangeBarEntry()
                        {
                            XLabel = aqi.date?.ToString("ddd", LocaleUtils.GetLocale())
                        }.Apply(entry =>
                        {
                            if (aqi.extras?.pm10_max.HasValue == true && aqi.extras?.pm10_min.HasValue == true)
                            {
                                entry.HiTempData = new YEntryData(aqi.extras.pm10_max.Value, aqi.extras.pm10_max.Value.ToInvariantString());
                                entry.LoTempData = new YEntryData(aqi.extras.pm10_min.Value, aqi.extras.pm10_min.Value.ToInvariantString());
                                entry.SetFillColors(AirQualityUtils.GetColorFromIndex(aqi.extras.pm10_max), AirQualityUtils.GetColorFromIndex(aqi.extras.pm10_min));
                            }
                            else if (aqi.extras?.pm10_max.HasValue == true)
                            {
                                entry.HiTempData = new YEntryData(aqi.extras.pm10_max.Value, aqi.extras.pm10_max.Value.ToInvariantString());
                                entry.SetFillColors(AirQualityUtils.GetColorFromIndex(aqi.extras.pm10_max));
                            }
                            else if (aqi.extras?.pm10_min.HasValue == true)
                            {
                                entry.LoTempData = new YEntryData(aqi.extras.pm10_min.Value, aqi.extras.pm10_min.Value.ToInvariantString());
                                entry.SetFillColors(AirQualityUtils.GetColorFromIndex(aqi.extras.pm10_min));
                            }
                            else
                            {
                                entry.HiTempData = new YEntryData(aqi.pm10.Value, aqi.pm10.Value.ToInvariantString());
                                entry.SetFillColors(AirQualityUtils.GetColorFromIndex(aqi.pm10));
                            }
                        }));
                    }
                    else if (pm10Data is BarGraphData barGraphData)
                    {
                        if (barGraphData.GetDataSet() == null)
                        {
                            barGraphData.SetDataSet(new BarGraphDataSet(new List<BarGraphEntry>()).Apply(it =>
                            {
                                it.SetMinMax(0f);
                            }));
                        }

                        barGraphData.GetDataSet()?.AddEntry(new BarGraphEntry()
                        {
                            XLabel = aqi.date?.ToString("ddd", LocaleUtils.GetLocale()),
                            EntryData = new(aqi.pm10.Value, aqi.pm10.Value.ToInvariantString()),
                            FillColor = AirQualityUtils.GetColorFromIndex(aqi.pm10.Value)
                        });
                    }
                }

                if (aqi.o3.HasValue)
                {
                    if (o3Data == null)
                    {
                        o3Data = o3UseRangeGraph ? new ForecastRangeBarGraphData()
                        {
                            GraphLabel = ResAQIndex.units_o3
                        } : new BarGraphData()
                        {
                            GraphLabel = ResAQIndex.units_o3
                        };
                    }

                    if (o3Data is ForecastRangeBarGraphData forecastRangeBarGraphData)
                    {
                        if (forecastRangeBarGraphData.GetDataSet() == null)
                        {
                            forecastRangeBarGraphData.SetDataSet(new ForecastRangeBarGraphDataSet(new List<ForecastRangeBarEntry>()));
                        }

                        forecastRangeBarGraphData.GetDataSet()?.AddEntry(new ForecastRangeBarEntry()
                        {
                            XLabel = aqi.date?.ToString("ddd", LocaleUtils.GetLocale())
                        }.Apply(entry =>
                        {
                            if (aqi.extras?.o3_max.HasValue == true && aqi.extras?.o3_min.HasValue == true)
                            {
                                entry.HiTempData = new YEntryData(aqi.extras.o3_max.Value, aqi.extras.o3_max.Value.ToInvariantString());
                                entry.LoTempData = new YEntryData(aqi.extras.o3_min.Value, aqi.extras.o3_min.Value.ToInvariantString());
                                entry.SetFillColors(AirQualityUtils.GetColorFromIndex(aqi.extras.o3_max), AirQualityUtils.GetColorFromIndex(aqi.extras.o3_min));
                            }
                            else if (aqi.extras?.o3_max.HasValue == true)
                            {
                                entry.HiTempData = new YEntryData(aqi.extras.o3_max.Value, aqi.extras.o3_max.Value.ToInvariantString());
                                entry.SetFillColors(AirQualityUtils.GetColorFromIndex(aqi.extras.o3_max));
                            }
                            else if (aqi.extras?.o3_min.HasValue == true)
                            {
                                entry.LoTempData = new YEntryData(aqi.extras.o3_min.Value, aqi.extras.o3_min.Value.ToInvariantString());
                                entry.SetFillColors(AirQualityUtils.GetColorFromIndex(aqi.extras.o3_min));
                            }
                            else
                            {
                                entry.HiTempData = new YEntryData(aqi.o3.Value, aqi.o3.Value.ToInvariantString());
                                entry.SetFillColors(AirQualityUtils.GetColorFromIndex(aqi.o3));
                            }
                        }));
                    }
                    else if (o3Data is BarGraphData barGraphData)
                    {
                        if (barGraphData.GetDataSet() == null)
                        {
                            barGraphData.SetDataSet(new BarGraphDataSet(new List<BarGraphEntry>()).Apply(it =>
                            {
                                it.SetMinMax(0f);
                            }));
                        }

                        barGraphData.GetDataSet()?.AddEntry(new BarGraphEntry()
                        {
                            XLabel = aqi.date?.ToString("ddd", LocaleUtils.GetLocale()),
                            EntryData = new(aqi.o3.Value, aqi.o3.Value.ToInvariantString()),
                            FillColor = AirQualityUtils.GetColorFromIndex(aqi.o3.Value)
                        });
                    }
                }

                if (aqi.co.HasValue)
                {
                    if (coData == null)
                    {
                        coData = new BarGraphData()
                        {
                            GraphLabel = ResAQIndex.units_co
                        };
                    }

                    if (coData is BarGraphData barGraphData)
                    {
                        if (barGraphData.GetDataSet() == null)
                        {
                            barGraphData.SetDataSet(new BarGraphDataSet(new List<BarGraphEntry>()).Apply(it =>
                            {
                                it.SetMinMax(0f);
                            }));
                        }

                        barGraphData.GetDataSet()?.AddEntry(new BarGraphEntry()
                        {
                            XLabel = aqi.date?.ToString("ddd", LocaleUtils.GetLocale()),
                            EntryData = new(aqi.co.Value, aqi.co.Value.ToInvariantString()),
                            FillColor = AirQualityUtils.GetColorFromIndex(aqi.co.Value)
                        });
                    }                }

                if (aqi.no2.HasValue)
                {
                    if (no2Data == null)
                    {
                        no2Data = new BarGraphData()
                        {
                            GraphLabel = ResAQIndex.units_no2
                        };
                    }

                    if (no2Data is BarGraphData barGraphData)
                    {
                        if (barGraphData.GetDataSet() == null)
                        {
                            barGraphData.SetDataSet(new BarGraphDataSet(new List<BarGraphEntry>()).Apply(it =>
                            {
                                it.SetMinMax(0f);
                            }));
                        }

                        barGraphData.GetDataSet()?.AddEntry(new BarGraphEntry()
                        {
                            XLabel = aqi.date?.ToString("ddd", LocaleUtils.GetLocale()),
                            EntryData = new(aqi.no2.Value, aqi.no2.Value.ToInvariantString()),
                            FillColor = AirQualityUtils.GetColorFromIndex(aqi.no2.Value)
                        });
                    }
                }

                if (aqi.so2.HasValue)
                {
                    if (so2Data == null)
                    {
                        so2Data = new BarGraphData()
                        {
                            GraphLabel = ResAQIndex.units_so2
                        };
                    }

                    if (so2Data is BarGraphData barGraphData)
                    {
                        if (barGraphData.GetDataSet() == null)
                        {
                            barGraphData.SetDataSet(new BarGraphDataSet(new List<BarGraphEntry>()).Apply(it =>
                            {
                                it.SetMinMax(0f);
                            }));
                        }

                        barGraphData.GetDataSet()?.AddEntry(new BarGraphEntry()
                        {
                            XLabel = aqi.date?.ToString("ddd", LocaleUtils.GetLocale()),
                            EntryData = new(aqi.so2.Value, aqi.so2.Value.ToInvariantString()),
                            FillColor = AirQualityUtils.GetColorFromIndex(aqi.so2.Value)
                        });
                    }
                }
            });

            aqiIndexData?.Let(d =>
            {
                d.NotifyDataChanged();
                graphDataList.Add(d);
            });
            pm25Data?.Let(d =>
            {
                d.NotifyDataChanged();
                graphDataList.Add(d);
            });
            pm10Data?.Let(d =>
            {
                d.NotifyDataChanged();
                graphDataList.Add(d);
            });
            o3Data?.Let(d =>
            {
                d.NotifyDataChanged();
                graphDataList.Add(d);
            });
            coData?.Let(d =>
            {
                d.NotifyDataChanged();
                graphDataList.Add(d);
            });
            no2Data?.Let(d =>
            {
                d.NotifyDataChanged();
                graphDataList.Add(d);
            });
            so2Data?.Let(d =>
            {
                d.NotifyDataChanged();
                graphDataList.Add(d);
            });

            return graphDataList;
        }

        private bool isDisposed;
        // Dispose() calls Dispose(true)
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed) return;

            if (disposing)
            {
                // free managed resources
                WeatherDB.Connection.GetConnection().TableChanged -= AirQualityForecastViewModel_TableChanged;
            }

            isDisposed = true;
        }
    }
}
