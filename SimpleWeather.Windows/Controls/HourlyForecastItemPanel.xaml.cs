using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SimpleWeather.NET.Helpers;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.NET.Controls
{
    public sealed partial class HourlyForecastItemPanel : UserControl
    {
        public ICollection<HourlyForecastNowViewModel> ForecastData
        {
            get => (ICollection<HourlyForecastNowViewModel>)GetValue(ForecastDataProperty);
            set => SetValue(ForecastDataProperty, value);
        }

        public static readonly DependencyProperty ForecastDataProperty =
                    DependencyProperty.Register(nameof(ForecastData), typeof(ICollection<HourlyForecastNowViewModel>),
                    typeof(HourlyForecastItemPanel), new PropertyMetadata(null));

        // Hooks
        private ScrollViewer HorizontalScroller;
        public event ItemClickEventHandler ItemClick
        {
            add
            {
                HourlyForecastControl.ItemClick += value;
            }
            remove
            {
                HourlyForecastControl.ItemClick -= value;
            }
        }

        public int GetItemPosition(object item)
        {
            return HourlyForecastControl.Items?.IndexOf(item) ?? 0;
        }

        public HourlyForecastItemPanel()
        {
            this.InitializeComponent();
        }

        private void HourlyForecastControl_Loaded(object sender, RoutedEventArgs e)
        {
            HorizontalScroller = VisualTreeHelperExtensions.FindChild<ScrollViewer>(HourlyForecastControl);
            HorizontalScroller.ViewChanged += HorizontalScroller_ViewChanged;
            HorizontalScroller.SizeChanged += HorizontalScroller_SizeChanged;
            UpdateScrollButtons(HorizontalScroller);
        }

        private void HorizontalScroller_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            UpdateScrollButtons(HorizontalScroller);
        }

        private void HorizontalScroller_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateScrollButtons(HorizontalScroller);
        }

        private void UpdateScrollButtons(ScrollViewer scroller)
        {
            CanScrollToStart = ScrollViewerHelper.CanScrollToStart(scroller);
            CanScrollToEnd = ScrollViewerHelper.CanScrollToEnd(scroller);
            if (scroller.ExtentWidth > scroller.ViewportWidth)
            {
                LeftButton.Visibility = Visibility.Visible;
                RightButton.Visibility = Visibility.Visible;
            }
            else
            {
                LeftButton.Visibility = Visibility.Collapsed;
                RightButton.Visibility = Visibility.Collapsed;
            }
        }

        public bool CanScrollToStart
        {
            get { return (bool)GetValue(CanScrollToStartProperty); }
            set { SetValue(CanScrollToStartProperty, value); }
        }

        public static readonly DependencyProperty CanScrollToStartProperty =
            DependencyProperty.Register(nameof(CanScrollToStart), typeof(bool),
            typeof(HourlyForecastItemPanel), new PropertyMetadata(false));

        public bool CanScrollToEnd
        {
            get { return (bool)GetValue(CanScrollToEndProperty); }
            set { SetValue(CanScrollToEndProperty, value); }
        }

        public static readonly DependencyProperty CanScrollToEndProperty =
            DependencyProperty.Register(nameof(CanScrollToEnd), typeof(bool),
            typeof(HourlyForecastItemPanel), new PropertyMetadata(false));

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            ScrollViewerHelper.ScrollLeft(HorizontalScroller);
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            ScrollViewerHelper.ScrollRight(HorizontalScroller);
        }

        private void HourlyForecastControl_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            args.RegisterUpdateCallback(HourlyForecastControl_Phase1);
            args.Handled = true;
        }

        private void HourlyForecastControl_Phase1(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            args.RegisterUpdateCallback(HourlyForecastControl_Phase2);
        }

        private void HourlyForecastControl_Phase2(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.Phase == 2 && args.ItemContainer?.ContentTemplateRoot is HourlyForecastItem forecastItem && args.Item is HourlyForecastNowViewModel model)
            {
                forecastItem.DataContext = model;
            }
            args.RegisterUpdateCallback(HourlyForecastControl_Phase3);
        }

        private void HourlyForecastControl_Phase3(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            // Phase 3, icon update time
            if (args.Phase == 3 && args.ItemContainer?.ContentTemplateRoot is HourlyForecastItem forecastItem && args.Item is HourlyForecastNowViewModel model)
            {
                forecastItem.WeatherIcon = model.Icon;
            }
        }
    }
}
