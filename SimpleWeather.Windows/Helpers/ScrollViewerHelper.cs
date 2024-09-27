using Microsoft.UI.Xaml.Controls;

namespace SimpleWeather.NET.Helpers
{
    public static class ScrollViewerHelper
    {
        public static bool CanScrollToStart(ScrollViewer scrollViewer)
        {
            return scrollViewer?.HorizontalOffset > 0;
        }

        public static bool CanScrollToStart(ScrollView scrollViewer)
        {
            return scrollViewer?.HorizontalOffset > 0;
        }

        public static bool CanScrollToEnd(ScrollViewer scrollViewer)
        {
            var distanceToEnd = scrollViewer?.ExtentWidth - (scrollViewer?.HorizontalOffset + scrollViewer?.ViewportWidth);
            return distanceToEnd > 0;
        }

        public static bool CanScrollToEnd(ScrollView scrollViewer)
        {
            var distanceToEnd = scrollViewer?.ExtentWidth - (scrollViewer?.HorizontalOffset + scrollViewer?.ViewportWidth);
            return distanceToEnd > 0;
        }

        public static bool CanScrollToTop(ScrollViewer scrollViewer)
        {
            return scrollViewer?.VerticalOffset > 0;
        }

        public static bool CanScrollToTop(ScrollView scrollViewer)
        {
            return scrollViewer?.VerticalOffset > 0;
        }

        public static bool CanScrollToBottom(ScrollViewer scrollViewer)
        {
            var distanceToEnd = scrollViewer?.ExtentHeight - (scrollViewer?.VerticalOffset + scrollViewer?.ViewportHeight);
            return distanceToEnd > 0;
        }

        public static bool CanScrollToBottom(ScrollView scrollViewer)
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

        public static void ScrollLeft(ScrollView scrollViewer)
        {
            scrollViewer?.ScrollTo(scrollViewer.HorizontalOffset - scrollViewer.ViewportWidth, scrollViewer.VerticalOffset);
        }

        public static void ScrollRight(ScrollViewer scrollViewer)
        {
            scrollViewer?.ChangeView(scrollViewer?.HorizontalOffset + scrollViewer?.ViewportWidth, null, null);
        }

        public static void ScrollRight(ScrollView scrollViewer)
        {
            scrollViewer?.ScrollTo(scrollViewer.HorizontalOffset + scrollViewer.ViewportWidth, scrollViewer.VerticalOffset);
        }

        public static void ScrollUp(ScrollViewer scrollViewer)
        {
            scrollViewer?.ChangeView(null, scrollViewer?.VerticalOffset - scrollViewer?.ViewportHeight, null);
        }

        public static void ScrollUp(ScrollView scrollViewer)
        {
            scrollViewer?.ScrollTo(scrollViewer.HorizontalOffset, scrollViewer.VerticalOffset - scrollViewer.ViewportHeight);
        }

        public static void ScrollDown(ScrollViewer scrollViewer)
        {
            scrollViewer?.ChangeView(null, scrollViewer?.VerticalOffset + scrollViewer?.ViewportHeight, null);
        }

        public static void ScrollDown(ScrollView scrollViewer)
        {
            scrollViewer?.ScrollTo(scrollViewer.HorizontalOffset, scrollViewer.VerticalOffset + scrollViewer.ViewportHeight);
        }
    }
}