using SimpleWeather.UWP.Controls.Graphs;
using SimpleWeather.UWP.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.UWP.Controls
{
    public sealed partial class HourlyForecastItemPanel : UserControl
    {
        public static readonly DependencyProperty ForecastDataProperty =
                    DependencyProperty.Register("ForecastData", typeof(ICollection<HourlyForecastNowViewModel>),
                    typeof(HourlyForecastItemPanel), new PropertyMetadata(null));

        public ICollection<HourlyForecastNowViewModel> ForecastData
        {
            get => (ICollection<HourlyForecastNowViewModel>)GetValue(ForecastDataProperty);
            set => SetValue(ForecastDataProperty, value);
        }

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
        }

        public static readonly DependencyProperty CanScrollToStartProperty =
            DependencyProperty.Register("CanScrollToStart", typeof(bool),
            typeof(HourlyForecastItemPanel), new PropertyMetadata(false));

        public static readonly DependencyProperty CanScrollToEndProperty =
            DependencyProperty.Register("CanScrollToEnd", typeof(bool),
            typeof(HourlyForecastItemPanel), new PropertyMetadata(false));

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
            ScrollViewerHelper.ScrollLeft(HorizontalScroller);
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            ScrollViewerHelper.ScrollRight(HorizontalScroller);
        }
    }
}
