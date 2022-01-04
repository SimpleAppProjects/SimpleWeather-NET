using SimpleWeather.Controls;
using SimpleWeather.ComponentModel;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.UWP.Controls.Graphs;
using SimpleWeather.UWP.Utils;
using SimpleWeather.WeatherData;
using SQLite;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace SimpleWeather.UWP.Controls
{
    public class AirQualityForecastViewModel : DependencyObject, IViewModel, IDisposable
    {
        private LocationData locationData;

        private ObservableItem<Forecasts> currentForecastsData;

        public ICollection<BarGraphData> AQIGraphData
        {
            get { return (ICollection<BarGraphData>)GetValue(AQIGraphDataProperty); }
            set { SetValue(AQIGraphDataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AQIGraphData.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AQIGraphDataProperty =
            DependencyProperty.Register("AQIGraphData", typeof(ICollection<BarGraphData>), typeof(AirQualityForecastViewModel), new PropertyMetadata(null));

        public ICollection<AirQualityViewModel> AQIForecastData
        {
            get { return (ICollection<AirQualityViewModel>)GetValue(AQIForecastDataProperty); }
            set { SetValue(AQIForecastDataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AQIForecastData.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AQIForecastDataProperty =
            DependencyProperty.Register("AQIForecastData", typeof(ICollection<AirQualityViewModel>), typeof(AirQualityForecastViewModel), new PropertyMetadata(null));

        public AirQualityForecastViewModel()
        {
            currentForecastsData = new ObservableItem<Forecasts>();
            currentForecastsData.ItemValueChanged += CurrentForecastsData_ItemValueChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            Dispatcher.LaunchOnUIThread(() =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            });
        }

        public void UpdateForecasts(LocationData location)
        {
            if (this.locationData == null || !Equals(this.locationData?.query, location?.query))
            {
                Task.Run(async () =>
                {
                    Settings.UnregisterWeatherDBChangedEvent(AirQualityForecastViewModel_TableChanged);

                    // Clone location data
                    this.locationData = new LocationData(new LocationQueryViewModel(location));

                    currentForecastsData.SetValue(await Settings.GetWeatherForecastData(location.query));

                    Settings.RegisterWeatherDBChangedEvent(AirQualityForecastViewModel_TableChanged);
                });
            }
        }

        private void AirQualityForecastViewModel_TableChanged(object sender, NotifyTableChangedEventArgs e)
        {
            if (locationData == null) return;

            Task.Run(async () =>
            {
                if (e?.Table?.TableName == WeatherData.Forecasts.TABLE_NAME)
                {
                    currentForecastsData.SetValue(await Settings.GetWeatherForecastData(locationData.query));
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
            Dispatcher.LaunchOnUIThread(() =>
            {
                var now = DateTime.Now.Date;
                var enumerable = forecasts?.WhereNot(item => item.date?.CompareTo(now) < 0);
                AQIGraphData = CreateGraphData(enumerable);
                AQIForecastData = enumerable?.Select(it => new AirQualityViewModel(it))?.ToList();
            });
        }

        private ICollection<BarGraphData> CreateGraphData(IEnumerable<AirQuality> enumerable)
        {
            var graphDataList = new List<BarGraphData>(7);
            BarGraphData aqiIndexData = null;
            BarGraphData pm25Data = null;
            BarGraphData pm10Data = null;
            BarGraphData o3Data = null;
            BarGraphData coData = null;
            BarGraphData no2Data = null;
            BarGraphData so2Data = null;

            enumerable?.ForEach(aqi =>
            {
                if (aqi.index.HasValue)
                {
                    if (aqiIndexData == null)
                    {
                        aqiIndexData = new()
                        {
                            GraphLabel = App.ResLoader.GetString("label_airquality")
                        };
                    }

                    if (aqiIndexData?.GetDataSet() == null)
                    {
                        aqiIndexData?.SetDataSet(new BarGraphDataSet(new List<BarGraphEntry>()).Apply(it =>
                        {
                            it.SetMinMax(0f);
                        }));
                    }

                    aqiIndexData?.GetDataSet()?.AddEntry(new BarGraphEntry()
                    {
                        XLabel = aqi.date?.ToString("ddd", CultureUtils.UserCulture),
                        EntryData = new(aqi.index.Value, aqi.index.Value.ToInvariantString()),
                        FillColor = AirQualityUtils.GetColorFromIndex(aqi.index.Value)
                    });
                }

                if (aqi.pm25.HasValue)
                {
                    if (pm25Data == null)
                    {
                        pm25Data = new()
                        {
                            GraphLabel = App.ResLoader.GetString("/AQIndex/units_pm25")
                        };
                    }

                    if (pm25Data?.GetDataSet() == null)
                    {
                        pm25Data?.SetDataSet(new BarGraphDataSet(new List<BarGraphEntry>()).Apply(it =>
                        {
                            it.SetMinMax(0f);
                        }));
                    }

                    pm25Data?.GetDataSet()?.AddEntry(new BarGraphEntry()
                    {
                        XLabel = aqi.date?.ToString("ddd", CultureUtils.UserCulture),
                        EntryData = new(aqi.pm25.Value, aqi.pm25.Value.ToInvariantString()),
                        FillColor = AirQualityUtils.GetColorFromIndex(aqi.pm25.Value)
                    });
                }

                if (aqi.pm10.HasValue)
                {
                    if (pm10Data == null)
                    {
                        pm10Data = new()
                        {
                            GraphLabel = App.ResLoader.GetString("/AQIndex/units_pm25")
                        };
                    }

                    if (pm10Data?.GetDataSet() == null)
                    {
                        pm10Data?.SetDataSet(new BarGraphDataSet(new List<BarGraphEntry>()).Apply(it =>
                        {
                            it.SetMinMax(0f);
                        }));
                    }

                    pm10Data?.GetDataSet()?.AddEntry(new BarGraphEntry()
                    {
                        XLabel = aqi.date?.ToString("ddd", CultureUtils.UserCulture),
                        EntryData = new(aqi.pm10.Value, aqi.pm10.Value.ToInvariantString()),
                        FillColor = AirQualityUtils.GetColorFromIndex(aqi.pm10.Value)
                    });
                }

                if (aqi.o3.HasValue)
                {
                    if (o3Data == null)
                    {
                        o3Data = new()
                        {
                            GraphLabel = App.ResLoader.GetString("/AQIndex/units_o3")
                        };
                    }

                    if (o3Data?.GetDataSet() == null)
                    {
                        o3Data?.SetDataSet(new BarGraphDataSet(new List<BarGraphEntry>()).Apply(it =>
                        {
                            it.SetMinMax(0f);
                        }));
                    }

                    o3Data?.GetDataSet()?.AddEntry(new BarGraphEntry()
                    {
                        XLabel = aqi.date?.ToString("ddd", CultureUtils.UserCulture),
                        EntryData = new(aqi.o3.Value, aqi.o3.Value.ToInvariantString()),
                        FillColor = AirQualityUtils.GetColorFromIndex(aqi.o3.Value)
                    });
                }

                if (aqi.co.HasValue)
                {
                    if (coData == null)
                    {
                        coData = new()
                        {
                            GraphLabel = App.ResLoader.GetString("/AQIndex/units_co")
                        };
                    }

                    if (coData?.GetDataSet() == null)
                    {
                        coData?.SetDataSet(new BarGraphDataSet(new List<BarGraphEntry>()).Apply(it =>
                        {
                            it.SetMinMax(0f);
                        }));
                    }

                    coData?.GetDataSet()?.AddEntry(new BarGraphEntry()
                    {
                        XLabel = aqi.date?.ToString("ddd", CultureUtils.UserCulture),
                        EntryData = new(aqi.co.Value, aqi.co.Value.ToInvariantString()),
                        FillColor = AirQualityUtils.GetColorFromIndex(aqi.co.Value)
                    });
                }

                if (aqi.no2.HasValue)
                {
                    if (no2Data == null)
                    {
                        no2Data = new()
                        {
                            GraphLabel = App.ResLoader.GetString("/AQIndex/units_no2")
                        };
                    }

                    if (no2Data?.GetDataSet() == null)
                    {
                        no2Data?.SetDataSet(new BarGraphDataSet(new List<BarGraphEntry>()).Apply(it =>
                        {
                            it.SetMinMax(0f);
                        }));
                    }

                    no2Data?.GetDataSet()?.AddEntry(new BarGraphEntry()
                    {
                        XLabel = aqi.date?.ToString("ddd", CultureUtils.UserCulture),
                        EntryData = new(aqi.no2.Value, aqi.no2.Value.ToInvariantString()),
                        FillColor = AirQualityUtils.GetColorFromIndex(aqi.no2.Value)
                    });
                }

                if (aqi.so2.HasValue)
                {
                    if (so2Data == null)
                    {
                        so2Data = new()
                        {
                            GraphLabel = App.ResLoader.GetString("/AQIndex/units_pm25")
                        };
                    }

                    if (so2Data?.GetDataSet() == null)
                    {
                        so2Data?.SetDataSet(new BarGraphDataSet(new List<BarGraphEntry>()).Apply(it =>
                        {
                            it.SetMinMax(0f);
                        }));
                    }

                    so2Data?.GetDataSet()?.AddEntry(new BarGraphEntry()
                    {
                        XLabel = aqi.date?.ToString("ddd", CultureUtils.UserCulture),
                        EntryData = new(aqi.so2.Value, aqi.so2.Value.ToInvariantString()),
                        FillColor = AirQualityUtils.GetColorFromIndex(aqi.so2.Value)
                    });
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
                Settings.UnregisterWeatherDBChangedEvent(AirQualityForecastViewModel_TableChanged);
            }

            isDisposed = true;
        }
    }
}
