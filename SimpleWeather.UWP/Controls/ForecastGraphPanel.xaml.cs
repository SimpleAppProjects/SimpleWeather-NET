using SimpleWeather.Controls;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

                // Remove handler
                if (_forecasts is INotifyCollectionChanged && _forecasts is IObservableLoadingCollection)
                {
                    (_forecasts as INotifyCollectionChanged).CollectionChanged -= NotifyCollection_CollectionChanged;
                }

                _forecasts = value as IEnumerable<BaseForecastItemViewModel>;

                // Add new handler
                if (_forecasts is INotifyCollectionChanged && _forecasts is IObservableLoadingCollection)
                {
                    (_forecasts as INotifyCollectionChanged).CollectionChanged += NotifyCollection_CollectionChanged;
                }
            }
        }

        private async void NotifyCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_forecasts is IObservableLoadingCollection)
                await UpdateLineView(false);
        }

        private IEnumerable<BaseForecastItemViewModel> _forecasts;

        public ScrollViewer ScrollViewer { get { return GraphView?.ScrollViewer; } }

        private ToggleButton SelectedButton { get; set; }

        private const int MAX_FETCH_SIZE = 20;
        private uint DataFetchSize = 1;

        public ForecastGraphPanel()
        {
            this.InitializeComponent();
            ViewChanged += ForecastGraphPanel_ViewChanged;
            GraphView.ItemWidthChanged += ForecastGraphPanel_SizeChanged;
        }

        private async void ForecastGraphPanel_SizeChanged(object sender, ItemSizeChangedEventArgs e)
        {
            if (ScrollViewer.ViewportWidth > 0 && e.NewSize.Width > 0)
            {
                var desiredFetchSize = (uint)Math.Round(ScrollViewer.ViewportWidth / e.NewSize.Width);
                if (desiredFetchSize > MAX_FETCH_SIZE)
                    DataFetchSize = MAX_FETCH_SIZE;
                else
                    DataFetchSize = desiredFetchSize;

                // trigger if (LineView) drawing does not fit viewport
                if (e.NewSize.Width <= ScrollViewer.ViewportWidth &&
                    _forecasts is IObservableLoadingCollection _collection && _collection.HasMoreItems && !_collection.IsLoading)
                {
                    await _collection.LoadMoreItemsAsync(DataFetchSize);
                }
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var size = base.ArrangeOverride(finalSize);

            if (!_forecasts.Any() && _forecasts is IObservableLoadingCollection _collection && _collection.HasMoreItems)
            {
                _collection.LoadMoreItemsAsync(1);
            }

            return size;
        }

        private async void ForecastGraphPanel_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var distanceToEnd = ScrollViewer.ExtentWidth - (ScrollViewer.HorizontalOffset + ScrollViewer.ViewportWidth);

            // trigger if (LineView) drawing does not fit viewport
            // or
            // if scrolling within 2 viewports of the end
            if (distanceToEnd <= 2.0 * ScrollViewer.ViewportWidth &&
                _forecasts is IObservableLoadingCollection _collection && _collection.HasMoreItems && !_collection.IsLoading)
            {
                await _collection.LoadMoreItemsAsync(DataFetchSize);
            }
        }

        private async void ToggleButton_Checked(object sender, RoutedEventArgs e)
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
                        other.IsChecked = false;
                        other.IsEnabled = true;
                    }
                }
            }

            // Update line view
            await UpdateLineView(true);
        }

        private async void GraphLineView_Loaded(object sender, RoutedEventArgs e)
        {
            if (_forecasts as IObservableLoadingCollection == null)
                await UpdateLineView(true);
        }

        private async Task UpdateLineView(bool resetOffset)
        {
            if (_forecasts?.Count() > 0)
            {
                UpdateToggles();

                switch (SelectedButton?.Tag)
                {
                    case "Temp":
                    default:
                        if (_forecasts?.Count() > 0 && GraphView != null)
                        {
                            GraphView.DrawGridLines = false;
                            GraphView.DrawDotLine = false;
                            GraphView.DrawDataLabels = true;
                            GraphView.DrawIconLabels = true;
                            GraphView.DrawGraphBackground = true;
                            GraphView.DrawDotPoints = false;
                            GraphView.DrawSeriesLabels = false;

                            List<XLabelData> labelData = new List<XLabelData>();
                            List<LineDataSeries> tempDataSeries = new List<LineDataSeries>();
                            List<YEntryData> hiTempSeries = new List<YEntryData>();
                            List<YEntryData> loTempSeries = null;

                            if (_forecasts?.First() is ForecastItemViewModel)
                            {
                                loTempSeries = new List<YEntryData>();
                                GraphView.DrawSeriesLabels = true;
                            }

                            foreach (BaseForecastItemViewModel forecastItemViewModel in _forecasts)
                            {
                                try
                                {
                                    float hiTemp = float.Parse(forecastItemViewModel.HiTemp.RemoveNonDigitChars());
                                    YEntryData hiTempData = new YEntryData(hiTemp, forecastItemViewModel.HiTemp.Trim());
                                    hiTempSeries.Add(hiTempData);

                                    if (loTempSeries != null && forecastItemViewModel is ForecastItemViewModel fVM)
                                    {
                                        float loTemp = float.Parse(fVM.LoTemp.RemoveNonDigitChars());
                                        YEntryData loTempData = new YEntryData(loTemp, fVM.LoTemp.Trim());
                                        loTempSeries.Add(loTempData);
                                    }

                                    XLabelData xLabelData = new XLabelData(forecastItemViewModel.Date, forecastItemViewModel.WeatherIcon);
                                    labelData.Add(xLabelData);
                                }
                                catch (FormatException ex)
                                {
                                    Logger.WriteLine(LoggerLevel.Debug, ex, "WeatherNow: error!!");
                                }
                            }

                            tempDataSeries.Add(new LineDataSeries("High", hiTempSeries));
                            if (loTempSeries != null)
                            {
                                tempDataSeries.Add(new LineDataSeries("Low", loTempSeries));
                            }

                            while (GraphView == null || (bool)!GraphView?.ReadyToDraw)
                            {
                                await Task.Delay(1);
                            }

                            GraphView.SetData(labelData, tempDataSeries);
                        }
                        break;

                    case "Wind":
                        if (_forecasts?.Count() > 0 && GraphView != null)
                        {
                            GraphView.DrawGridLines = false;
                            GraphView.DrawDotLine = false;
                            GraphView.DrawDataLabels = true;
                            GraphView.DrawIconLabels = false;
                            GraphView.DrawGraphBackground = true;
                            GraphView.DrawDotPoints = false;
                            GraphView.DrawSeriesLabels = false;

                            List<XLabelData> labelData = new List<XLabelData>();
                            List<LineDataSeries> windDataList = new List<LineDataSeries>();
                            List<YEntryData> windDataSeries = new List<YEntryData>();

                            foreach (BaseForecastItemViewModel forecastItemViewModel in _forecasts)
                            {
                                try
                                {
                                    float wind = float.Parse(forecastItemViewModel.WindSpeed.RemoveNonDigitChars());
                                    YEntryData windData = new YEntryData(wind, forecastItemViewModel.WindSpeed.Trim());

                                    windDataSeries.Add(windData);
                                    XLabelData xLabelData = new XLabelData(forecastItemViewModel.Date, WeatherIcons.WIND_DIRECTION, forecastItemViewModel.WindDirection);
                                    labelData.Add(xLabelData);
                                }
                                catch (FormatException ex)
                                {
                                    Logger.WriteLine(LoggerLevel.Debug, ex, "WeatherNow: error!!");
                                }
                            }

                            windDataList.Add(new LineDataSeries(windDataSeries));

                            while (GraphView == null || (bool)!GraphView?.ReadyToDraw)
                            {
                                await Task.Delay(1);
                            }

                            GraphView.SetData(labelData, windDataList);
                        }
                        break;

                    case "Rain":
                        if (_forecasts?.Count() > 0 && GraphView != null)
                        {
                            GraphView.DrawGridLines = false;
                            GraphView.DrawDotLine = false;
                            GraphView.DrawDataLabels = true;
                            GraphView.DrawIconLabels = false;
                            GraphView.DrawGraphBackground = true;
                            GraphView.DrawDotPoints = false;
                            GraphView.DrawSeriesLabels = false;

                            List<XLabelData> labelData = new List<XLabelData>();
                            List<LineDataSeries> popDataList = new List<LineDataSeries>();
                            List<YEntryData> popDataSeries = new List<YEntryData>();

                            foreach (BaseForecastItemViewModel forecastItemViewModel in _forecasts)
                            {
                                try
                                {
                                    float pop = float.Parse(forecastItemViewModel.PoP.RemoveNonDigitChars());
                                    YEntryData popData = new YEntryData(pop, forecastItemViewModel.PoP.Trim());

                                    popDataSeries.Add(popData);
                                    XLabelData xLabelData = new XLabelData(forecastItemViewModel.Date, WeatherIcons.RAINDROP, 0);
                                    labelData.Add(xLabelData);
                                }
                                catch (FormatException ex)
                                {
                                    Logger.WriteLine(LoggerLevel.Debug, ex, "WeatherNow: error!!");
                                }
                            }

                            popDataList.Add(new LineDataSeries(popDataSeries));

                            while (GraphView == null || (bool)!GraphView?.ReadyToDraw)
                            {
                                await Task.Delay(1);
                            }

                            GraphView.SetData(labelData, popDataList);
                        }
                        break;
                }

                if (resetOffset) ScrollViewer?.ChangeView(0, null, null);
            }
        }

        private void UpdateToggles()
        {
            int count = 1;
            var first = _forecasts?.First();
            if (first is ForecastItemViewModel)
            {
                if (!String.IsNullOrWhiteSpace(first.WindSpeed) &&
                    !String.IsNullOrWhiteSpace(first.PoP.Replace("%", "")))
                {
                    count = 3;
                }
            }

            if (first is HourlyForecastItemViewModel)
            {
                if (Settings.API.Equals(WeatherAPI.OpenWeatherMap) || Settings.API.Equals(WeatherAPI.MetNo) ||
                    Settings.API.Equals(WeatherAPI.NWS))
                {
                    count = 2;
                }
                else
                {
                    count = 3;
                }
            }

            switch (count)
            {
                case 1:
                default:
                    TempToggleButton.Visibility = Visibility.Visible;
                    RainToggleButton.Visibility = Visibility.Collapsed;
                    WindToggleButton.Visibility = Visibility.Collapsed;

                    TempToggleButton.IsChecked = true;
                    TempToggleButton.IsEnabled = false;
                    RainToggleButton.IsChecked = false;
                    RainToggleButton.IsEnabled = true;
                    WindToggleButton.IsChecked = false;
                    WindToggleButton.IsEnabled = true;
                    break;

                case 2:
                    if (RainToggleButton.Visibility == Visibility.Visible)
                    {
                        RainToggleButton.Visibility = Visibility.Collapsed;
                        if ((bool)RainToggleButton.IsChecked)
                        {
                            RainToggleButton.IsChecked = false;
                            RainToggleButton.IsEnabled = true;
                            TempToggleButton.IsChecked = true;
                            TempToggleButton.IsEnabled = false;
                        }
                    }
                    break;

                case 3:
                    TempToggleButton.Visibility = Visibility.Visible;
                    RainToggleButton.Visibility = Visibility.Visible;
                    WindToggleButton.Visibility = Visibility.Visible;
                    break;
            }
        }
    }
}