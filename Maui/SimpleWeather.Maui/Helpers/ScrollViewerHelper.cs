using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.Maui.Helpers
{
    public static class ScrollViewerHelper
    {
        public static bool CanScrollToStart(ScrollView scrollViewer)
        {
            return scrollViewer?.ScrollX > 0;
        }

        public static bool CanScrollToEnd(ScrollView scrollViewer)
        {
            var distanceToEnd = scrollViewer?.Content?.Width - (scrollViewer?.ScrollX + scrollViewer?.Width);
            return distanceToEnd > 0;
        }

        public static bool CanScrollToTop(ScrollView scrollViewer)
        {
            return scrollViewer?.ScrollY > 0;
        }

        public static bool CanScrollToBottom(ScrollView scrollViewer)
        {
            var distanceToEnd = scrollViewer?.Content?.Height - (scrollViewer?.ScrollY + scrollViewer?.Height);
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

        public static async void ScrollLeft(ScrollView scrollViewer)
        {
            await scrollViewer?.ScrollToAsync(scrollViewer.ScrollX - scrollViewer.Width, scrollViewer.ScrollY, true);
        }

        public static async void ScrollRight(ScrollView scrollViewer)
        {
            await scrollViewer?.ScrollToAsync(scrollViewer.ScrollX + scrollViewer.Width, scrollViewer.ScrollY, true);
        }

        public static async void ScrollUp(ScrollView scrollViewer)
        {
            await scrollViewer?.ScrollToAsync(scrollViewer.ScrollX, scrollViewer.ScrollY - scrollViewer.Height, true);
        }

        public static async void ScrollDown(ScrollView scrollViewer)
        {
            await scrollViewer?.ScrollToAsync(scrollViewer.ScrollX, scrollViewer.ScrollY + scrollViewer.Height, true);
        }
    }
}