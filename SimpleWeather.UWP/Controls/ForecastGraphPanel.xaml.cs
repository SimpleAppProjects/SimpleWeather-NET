using SimpleWeather.Controls;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.UWP.Controls
{
    public sealed partial class ForecastGraphPanel : UserControl
    {
        //
        // Summary:
        //     Occurs when manipulations such as scrolling and zooming cause the view to change.
        public event TappedEventHandler GraphViewTapped
        {
            add
            {
                GraphView.Tapped += value;
            }
            remove
            {
                GraphView.Tapped -= value;
            }
        }

        //
        // Summary:
        //     Occurs when manipulations such as scrolling and zooming have caused the view
        //     to change.
        public event EventHandler<ScrollViewerViewChangedEventArgs> ViewChanged
        {
            add
            {
                ScrollViewer.ViewChanged += value;
            }
            remove
            {
                ScrollViewer.ViewChanged -= value;
            }
        }

        public static readonly DependencyProperty ForecastsProperty =
            DependencyProperty.Register("Forecasts", typeof(object),
            typeof(ForecastGraphPanel), new PropertyMetadata(null));

        public object Forecasts
        {
            get { return GetValue(ForecastsProperty); }
            set
            {
                SetValue(ForecastsProperty, value);

                _forecasts = value as IEnumerable<GraphItemViewModel>;

                UpdateLineView(false);
            }
        }

        private IEnumerable<GraphItemViewModel> _forecasts;

        public ScrollViewer ScrollViewer { get { return GraphView?.ScrollViewer; } }

        private ToggleButton SelectedButton { get; set; }

        private const int MAX_FETCH_SIZE = 24; // 24hrs

        public ForecastGraphPanel()
        {
            this.InitializeComponent();
            TempToggleButton.IsChecked = true;
            TempToggleButton.Checked += ToggleButton_Checked;
            WindToggleButton.Checked += ToggleButton_Checked;
            RainToggleButton.Checked += ToggleButton_Checked;
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            var btn = sender as ToggleButton;
            SelectedButton = btn;
            btn.IsEnabled = false;

            if (btn.Parent is FrameworkElement parent && VisualTreeHelper.GetChildrenCount(parent) > 1)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
                {
                    if (VisualTreeHelper.GetChild(parent, i) is ToggleButton other && !other.Tag.Equals(btn.Tag))
                    {
                        other.Deselect();
                    }
                }
            }

            // Update line view
            UpdateLineView(true);
        }

        private void GraphLineView_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateLineView(true);
        }

        private void UpdateLineView(bool resetOffset)
        {
            UpdateToggles();
            GraphView?.ResetData();

            if (_forecasts?.Any() == true && GraphView != null)
            {
                var forecasts = new List<GraphItemViewModel>(_forecasts);
                var itemCount = Math.Min(forecasts.Count, MAX_FETCH_SIZE);

                switch (SelectedButton?.Tag)
                {
                    case "Temp":
                    default:
                        {
                            GraphView.DrawGridLines = false;
                            GraphView.DrawDotLine = false;
                            GraphView.DrawDataLabels = true;
                            GraphView.DrawIconLabels = true;
                            GraphView.DrawGraphBackground = true;
                            GraphView.DrawDotPoints = false;
                            GraphView.DrawSeriesLabels = false;

                            List<XLabelData> labelData = new List<XLabelData>(itemCount);
                            List<LineDataSeries> tempDataSeries = new List<LineDataSeries>(2);
                            List<YEntryData> hiTempSeries = new List<YEntryData>(itemCount);
                            List<YEntryData> loTempSeries = null;

                            if (forecasts.FirstOrDefault()?.TempEntryData?.Data.LoTempData != null)
                            {
                                loTempSeries = new List<YEntryData>(itemCount);
                                GraphView.DrawSeriesLabels = true;
                            }

                            for (int i = 0; i < itemCount; i++)
                            {
                                var graphModel = forecasts[i];
                                try
                                {
                                    if (graphModel?.TempEntryData.Data?.HiTempData != null)
                                    {
                                        hiTempSeries.Add(graphModel.TempEntryData.Data.HiTempData);
                                    }
                                    // For NWS, which contains bi-daily forecasts
                                    else if (i == 0 && i + 1 < itemCount)
                                    {
                                        var nextVM = forecasts[i + 1];
                                        if (nextVM?.TempEntryData?.Data?.HiTempData != null)
                                            hiTempSeries.Add(nextVM.TempEntryData.Data.HiTempData);
                                    }
                                    else if (i > 0 && i == itemCount - 1)
                                    {
                                        var prevVM = forecasts[i - 1];
                                        if (prevVM?.TempEntryData?.Data?.HiTempData != null)
                                            hiTempSeries.Add(prevVM.TempEntryData.Data.HiTempData);
                                    }

                                    if (loTempSeries != null)
                                    {
                                        if (graphModel?.TempEntryData.Data?.LoTempData != null)
                                        {
                                            loTempSeries.Add(graphModel.TempEntryData.Data.LoTempData);
                                        }
                                        // For NWS, which contains bi-daily forecasts
                                        else if (i == 0 && i + 1 < itemCount)
                                        {
                                            var nextVM = forecasts[i + 1];
                                            if (nextVM?.TempEntryData?.Data?.LoTempData != null)
                                                loTempSeries.Add(nextVM.TempEntryData.Data.LoTempData);
                                        }
                                        else if (i > 0 && i == itemCount - 1)
                                        {
                                            var prevVM = forecasts[i - 1];
                                            if (prevVM?.TempEntryData?.Data?.LoTempData != null)
                                                loTempSeries.Add(prevVM.TempEntryData.Data.LoTempData);
                                        }
                                    }

                                    labelData.Add(graphModel.TempEntryData.LabelData);
                                }
                                catch (FormatException ex)
                                {
                                    Logger.WriteLine(LoggerLevel.Debug, ex, "WeatherNow: error!!");
                                }
                            }

                            if (hiTempSeries?.Count > 0)
                            {
                                var hiTempSeriesLabel = App.ResLoader.GetString("Label_High");
                                tempDataSeries.Add(new LineDataSeries(hiTempSeriesLabel, hiTempSeries));
                            }

                            if (loTempSeries?.Count > 0)
                            {
                                var loTempSeriesLabel = App.ResLoader.GetString("Label_Low");
                                tempDataSeries.Add(new LineDataSeries(loTempSeriesLabel, loTempSeries));
                            }

                            Task.Run(async () =>
                            {
                                while (GraphView == null || await Dispatcher.RunOnUIThread(() => (bool)!GraphView?.ReadyToDraw))
                                {
                                    await Task.Delay(1);
                                }

                                await Dispatcher.RunOnUIThread(() =>
                                {
                                    GraphView.SetData(labelData, tempDataSeries);
                                });
                            });
                        }
                        break;

                    case "Wind":
                        {
                            GraphView.DrawGridLines = false;
                            GraphView.DrawDotLine = false;
                            GraphView.DrawDataLabels = true;
                            GraphView.DrawIconLabels = false;
                            GraphView.DrawGraphBackground = true;
                            GraphView.DrawDotPoints = false;
                            GraphView.DrawSeriesLabels = false;

                            List<XLabelData> labelData = new List<XLabelData>(itemCount);
                            List<LineDataSeries> windDataList = new List<LineDataSeries>(1);
                            List<YEntryData> windDataSeries = new List<YEntryData>(itemCount);

                            for (int i = 0; i < itemCount; i++)
                            {
                                var graphModel = forecasts[i];
                                if (graphModel.WindEntryData != null)
                                {
                                    windDataSeries.Add(graphModel.WindEntryData.Data);
                                    labelData.Add(graphModel.WindEntryData.LabelData);
                                }
                            }

                            if (windDataSeries?.Count > 0)
                            {
                                windDataList.Add(new LineDataSeries(windDataSeries));
                            }

                            Task.Run(async () =>
                            {
                                while (GraphView == null || await Dispatcher.RunOnUIThread(() => (bool)!GraphView?.ReadyToDraw))
                                {
                                    await Task.Delay(1);
                                }

                                await Dispatcher.RunOnUIThread(() =>
                                {
                                    GraphView.SetData(labelData, windDataList);
                                });
                            });
                        }
                        break;

                    case "Rain":
                        {
                            GraphView.DrawGridLines = false;
                            GraphView.DrawDotLine = false;
                            GraphView.DrawDataLabels = true;
                            GraphView.DrawIconLabels = false;
                            GraphView.DrawGraphBackground = true;
                            GraphView.DrawDotPoints = false;
                            GraphView.DrawSeriesLabels = false;

                            List<XLabelData> labelData = new List<XLabelData>(itemCount);
                            List<LineDataSeries> popDataList = new List<LineDataSeries>(1);
                            List<YEntryData> popDataSeries = new List<YEntryData>(itemCount);

                            for (int i = 0; i < itemCount; i++)
                            {
                                var graphModel = forecasts[i];
                                if (graphModel.ChanceEntryData != null)
                                {
                                    popDataSeries.Add(graphModel.ChanceEntryData.Data);
                                    labelData.Add(graphModel.ChanceEntryData.LabelData);
                                }
                            }

                            if (popDataSeries?.Count > 0)
                            {
                                popDataList.Add(new LineDataSeries(popDataSeries));
                            }

                            Task.Run(async () =>
                            {
                                while (GraphView == null || await Dispatcher.RunOnUIThread(() => (bool)!GraphView?.ReadyToDraw))
                                {
                                    await Task.Delay(1);
                                }

                                await Dispatcher.RunOnUIThread(() =>
                                {
                                    GraphView.SetData(labelData, popDataList);
                                });
                            });
                        }
                        break;
                }

                if (resetOffset) ScrollViewer?.ChangeView(0, null, null);
            }
        }

        private void UpdateToggles()
        {
            bool isForecast = false, isWind = false, isChance = false;
            var first = _forecasts?.FirstOrDefault();

            if (first != null)
            {
                if (first.TempEntryData != null)
                {
                    isForecast = true;
                }
                if (first.WindEntryData != null)
                {
                    isWind = true;
                }
                if (first.ChanceEntryData != null)
                {
                    isChance = true;
                }
            }

            TempToggleButton.Visibility = isForecast ? Visibility.Visible : Visibility.Collapsed;
            WindToggleButton.Visibility = isWind ? Visibility.Visible : Visibility.Collapsed;
            RainToggleButton.Visibility = isChance ? Visibility.Visible : Visibility.Collapsed;

            if (!isForecast || !isWind || !isChance)
            {
                if (TempToggleButton.IsChecked == true)
                {
                    if (!isForecast)
                    {
                        if (isWind)
                        {
                            WindToggleButton.Select();
                        }
                        else if (isChance)
                        {
                            RainToggleButton.Select();
                        }
                    }
                }
                else if (WindToggleButton.IsChecked == true)
                {
                    if (!isWind)
                    {
                        if (isForecast)
                        {
                            TempToggleButton.Select();
                        }
                        else if (isChance)
                        {
                            RainToggleButton.Select();
                        }
                    }
                }
                else if (RainToggleButton.IsChecked == true)
                {
                    if (!isChance)
                    {
                        if (isForecast)
                        {
                            TempToggleButton.Select();
                        }
                        else if (isWind)
                        {
                            WindToggleButton.Select();
                        }
                    }
                }
            }
        }
    }

    internal static class ToggleButtonExtensions
    {
        internal static void Select(this ToggleButton @toggle)
        {
            toggle.IsChecked = true;
            toggle.IsEnabled = false;
        }

        internal static void Deselect(this ToggleButton @toggle)
        {
            toggle.IsChecked = false;
            toggle.IsEnabled = true;
        }
    }
}