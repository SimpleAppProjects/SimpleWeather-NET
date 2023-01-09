using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;

namespace SimpleWeather.Uno.Helpers
{
    public static class ScrollViewerHelper
    {
        public static bool CanScrollToStart(ScrollViewer scrollViewer)
        {
            return scrollViewer?.HorizontalOffset > 0;
        }

        public static bool CanScrollToEnd(ScrollViewer scrollViewer)
        {
            var distanceToEnd = scrollViewer?.ExtentWidth - (scrollViewer?.HorizontalOffset + scrollViewer?.ViewportWidth);
            return distanceToEnd > 0;
        }

        public static bool CanScrollToTop(ScrollViewer scrollViewer)
        {
            return scrollViewer?.VerticalOffset > 0;
        }

        public static bool CanScrollToBottom(ScrollViewer scrollViewer)
        {
            var distanceToEnd = scrollViewer?.ExtentHeight - (scrollViewer?.VerticalOffset + scrollViewer?.ViewportHeight);
            return distanceToEnd > 0;
        }

        public static bool CanScrollToStart(double? HorizontalOffset)
        {
            return HorizontalOffset > 0;
        }

        public static bool CanScrollToEnd(double? HorizontalOffset, double? ViewportWidth, double? ExtentWidth)
        {
            var distanceToEnd = ExtentWidth - (HorizontalOffset + ViewportWidth);
            return distanceToEnd > 0;
        }

        public static bool CanScrollToTop(double? VerticalOffset)
        {
            return VerticalOffset > 0;
        }

        public static bool CanScrollToBottom(double? VerticalOffset, double? ViewportHeight, double? ExtentHeight)
        {
            var distanceToEnd = ExtentHeight - (VerticalOffset + ViewportHeight);
            return distanceToEnd > 0;
        }

        public static void ScrollLeft(ScrollViewer scrollViewer)
        {
            scrollViewer?.ChangeView(scrollViewer?.HorizontalOffset - scrollViewer?.ViewportWidth, null, null);
        }

        public static void ScrollRight(ScrollViewer scrollViewer)
        {
            scrollViewer?.ChangeView(scrollViewer?.HorizontalOffset + scrollViewer?.ViewportWidth, null, null);
        }

        public static void ScrollUp(ScrollViewer scrollViewer)
        {
            scrollViewer?.ChangeView(scrollViewer?.VerticalOffset - scrollViewer?.ViewportHeight, null, null);
        }

        public static void ScrollDown(ScrollViewer scrollViewer)
        {
            scrollViewer?.ChangeView(scrollViewer?.VerticalOffset + scrollViewer?.ViewportHeight, null, null);
        }
    }
}