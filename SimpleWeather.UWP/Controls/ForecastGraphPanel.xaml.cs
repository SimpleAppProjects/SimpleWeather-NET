using SimpleWeather.Controls;
using SimpleWeather.Utils;
using SimpleWeather.UWP.Controls.Graphs;
using SimpleWeather.UWP.Helpers;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
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
                LineGraphView.Tapped += value;
                BarChartView.Tapped += value;
            }
            remove
            {
                LineGraphView.Tapped -= value;
                BarChartView.Tapped -= value;
            }
        }

        private void GraphView_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (sender is ScrollViewer scroller)
            {
                UpdateScrollButtons(scroller);
            }
        }

        private void GraphView_ItemWidthChanged(object sender, ItemSizeChangedEventArgs e)
        {
            if (sender is IGraph graph && graph.GetControl().Visibility == Visibility.Visible)
            {
                UpdateScrollButtons(graph.GetScrollViewer());
            }
        }

        private void UpdateScrollButtons(ScrollViewer scroller)
        {
            CanScrollToStart = ScrollViewerHelper.CanScrollToStart(scroller);
            CanScrollToEnd = ScrollViewerHelper.CanScrollToEnd(scroller);
        }

        public static readonly DependencyProperty CanScrollToStartProperty =
            DependencyProperty.Register("CanScrollToStart", typeof(bool),
            typeof(ForecastGraphPanel), new PropertyMetadata(false));

        public static readonly DependencyProperty CanScrollToEndProperty =
            DependencyProperty.Register("CanScrollToEnd", typeof(bool),
            typeof(ForecastGraphPanel), new PropertyMetadata(false));

        public bool CanScrollToStart
        {
            get { return (bool)GetValue(CanScrollToStartProperty); }
            set { SetValue(CanScrollToStartProperty, value); }
        }

        public bool CanScrollToEnd
        {
            get { return (bool)GetValue(CanScrollToEndProperty); }
            set { SetValue(CanScrollToEndProperty, value); }
        }

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            if (LineGraphView.Visibility == Visibility.Visible)
            {
                ScrollViewerHelper.ScrollLeft(LineGraphView.ScrollViewer);
            }
            if (BarChartView.Visibility == Visibility.Visible)
            {
                ScrollViewerHelper.ScrollLeft(BarChartView.ScrollViewer);
            }
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            if (LineGraphView.Visibility == Visibility.Visible)
            {
                ScrollViewerHelper.ScrollRight(LineGraphView.ScrollViewer);
            }
            if (BarChartView.Visibility == Visibility.Visible)
            {
                ScrollViewerHelper.ScrollRight(BarChartView.ScrollViewer);
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

                UpdateView(false);
            }
        }

        private IEnumerable<GraphItemViewModel> _forecasts;

        private ToggleButton SelectedButton { get; set; }

        private const int MAX_FETCH_SIZE = 24; // 24hrs

        public ForecastGraphPanel()
        {
            this.InitializeComponent();
            TempToggleButton.IsChecked = true;
            TempToggleButton.IsEnabled = false;
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

            // Update view
            UpdateView(true);
        }

        private void GraphView_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateView(true);
        }

        private void ShowBarChart(bool show)
        {
            LineGraphView.Visibility = !show ? Visibility.Visible : Visibility.Collapsed;
            BarChartView.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UpdateView(bool resetOffset)
        {
            UpdateToggles();
            LineGraphView?.ResetData();
            BarChartView?.ResetData();

            switch (SelectedButton?.Tag)
            {
                case "Temp":
                default:
                    UpdateForecastGraph();
                    break;
                case "Wind":
                    UpdateWindForecastGraph();
                    break;
                case "Rain":
                    UpdatePrecipitationGraph();
                    break;
            }

            if (resetOffset)
            {
                LineGraphView.ScrollViewer.ChangeView(0, null, null);
                BarChartView.ScrollViewer.ChangeView(0, null, null);
            }
        }

        private void UpdateForecastGraph()
        {
            if (_forecasts?.Any() == true)
            {
                var forecasts = new List<GraphItemViewModel>(_forecasts);
                var itemCount = Math.Min(forecasts.Count, MAX_FETCH_SIZE);

                bool loTempDataPresent = false;

                if (forecasts.FirstOrDefault() is GraphItemViewModel first && first.TempEntryData?.Data?.LoTempData != null)
                {
                    loTempDataPresent = true;
                }

                if (loTempDataPresent)
                {
                    ShowBarChart(true);
                    BarChartView.DrawDataLabels = true;
                    BarChartView.DrawIconLabels = true;

                    List<XLabelData> labelDataSet = new List<XLabelData>(itemCount);
                    List<GraphTemperature> tempDataSet = new List<GraphTemperature>(itemCount);

                    for (int i = 0; i < itemCount; i++)
                    {
                        var graphModel = forecasts[i];

                        labelDataSet.Add(graphModel.TempEntryData.LabelData);
                        tempDataSet.Add(graphModel.TempEntryData.Data);
                    }

                    Task.Run(async () =>
                    {
                        while (BarChartView == null || await Dispatcher.RunOnUIThread(() => (bool)!BarChartView?.ReadyToDraw))
                        {
                            await Task.Delay(1);
                        }

                        await Dispatcher.RunOnUIThread(() =>
                        {
                            BarChartView.SetData(labelDataSet, tempDataSet);
                        });
                    });
                }
                else
                {
                    ShowBarChart(false);
                    LineGraphView.DrawGridLines = false;
                    LineGraphView.DrawDotLine = false;
                    LineGraphView.DrawDataLabels = true;
                    LineGraphView.DrawIconLabels = true;
                    LineGraphView.DrawGraphBackground = true;
                    LineGraphView.DrawDotPoints = false;
                    LineGraphView.DrawSeriesLabels = false;

                    List<XLabelData> labelDataset = new List<XLabelData>(itemCount);
                    List<YEntryData> hiTempDataset = new List<YEntryData>(itemCount);
                    List<LineDataSeries> tempDataSeries = new List<LineDataSeries>(1);

                    for (int i = 0; i < itemCount; i++)
                    {
                        var graphModel = forecasts[i];

                        if (graphModel?.TempEntryData.Data?.HiTempData != null)
                        {
                            hiTempDataset.Add(graphModel.TempEntryData.Data.HiTempData);
                        }
                        // For NWS, which contains bi-daily forecasts
                        else if (i == 0 && i + 1 < itemCount)
                        {
                            var nextVM = forecasts[i + 1];
                            if (nextVM?.TempEntryData?.Data?.HiTempData != null)
                                hiTempDataset.Add(nextVM.TempEntryData.Data.HiTempData);
                        }
                        else if (i > 0 && i == itemCount - 1)
                        {
                            var prevVM = forecasts[i - 1];
                            if (prevVM?.TempEntryData?.Data?.HiTempData != null)
                                hiTempDataset.Add(prevVM.TempEntryData.Data.HiTempData);
                        }
                        labelDataset.Add(graphModel.TempEntryData.LabelData);
                    }

                    if (hiTempDataset?.Count > 0)
                    {
                        var hiTempSeriesLabel = App.ResLoader.GetString("Label_High");
                        var series = new LineDataSeries(hiTempSeriesLabel, hiTempDataset);
                        series.SetSeriesColors(Colors.OrangeRed);
                        tempDataSeries.Add(series);
                    }

                    Task.Run(async () =>
                    {
                        while (LineGraphView == null || await Dispatcher.RunOnUIThread(() => (bool)!LineGraphView?.ReadyToDraw))
                        {
                            await Task.Delay(1);
                        }

                        await Dispatcher.RunOnUIThread(() =>
                        {
                            LineGraphView.SetData(labelDataset, tempDataSeries);
                        });
                    });
                }
            }
        }

        private void UpdateWindForecastGraph()
        {
            if (_forecasts?.Any() == true && LineGraphView != null)
            {
                var forecasts = new List<GraphItemViewModel>(_forecasts);
                var itemCount = Math.Min(forecasts.Count, MAX_FETCH_SIZE);

                ShowBarChart(false);
                LineGraphView.DrawGridLines = false;
                LineGraphView.DrawDotLine = false;
                LineGraphView.DrawDataLabels = true;
                LineGraphView.DrawIconLabels = true;
                LineGraphView.DrawGraphBackground = true;
                LineGraphView.DrawDotPoints = false;
                LineGraphView.DrawSeriesLabels = false;

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
                    var series = new LineDataSeries(windDataSeries);
                    series.SetSeriesColors(Colors.SeaGreen);
                    windDataList.Add(series);
                }

                Task.Run(async () =>
                {
                    while (LineGraphView == null || await Dispatcher.RunOnUIThread(() => (bool)!LineGraphView?.ReadyToDraw))
                    {
                        await Task.Delay(1);
                    }

                    await Dispatcher.RunOnUIThread(() =>
                    {
                        LineGraphView.SetData(labelData, windDataList);
                    });
                });
            }
        }

        private void UpdatePrecipitationGraph()
        {
            if (_forecasts?.Any() == true && LineGraphView != null)
            {
                var forecasts = new List<GraphItemViewModel>(_forecasts);
                var itemCount = Math.Min(forecasts.Count, MAX_FETCH_SIZE);

                ShowBarChart(false);
                LineGraphView.DrawGridLines = false;
                LineGraphView.DrawDotLine = false;
                LineGraphView.DrawDataLabels = true;
                LineGraphView.DrawIconLabels = true;
                LineGraphView.DrawGraphBackground = true;
                LineGraphView.DrawDotPoints = false;
                LineGraphView.DrawSeriesLabels = false;

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
                    var series = new LineDataSeries(popDataSeries);
                    series.SetSeriesColors(Color.FromArgb(0xFF, 0, 0x70, 0xC0)); // SimpleBlue
                    popDataList.Add(series);
                }

                Task.Run(async () =>
                {
                    while (LineGraphView == null || await Dispatcher.RunOnUIThread(() => (bool)!LineGraphView?.ReadyToDraw))
                    {
                        await Task.Delay(1);
                    }

                    await Dispatcher.RunOnUIThread(() =>
                    {
                        LineGraphView.SetData(labelData, popDataList);
                    });
                });
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