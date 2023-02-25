#if IOS || MACCATALYST
using System;
using CoreGraphics;
using UIKit;

namespace SimpleWeather.Maui.Helpers
{
	public static partial class CollectionViewHelper
	{
        public static bool CanScrollToStart(this UICollectionView collectionView)
        {
            return collectionView.ContentOffset.X > 0;
        }

        public static bool CanScrollToEnd(this UICollectionView collectionView)
        {
            var distanceToEnd = collectionView.ContentSize.Width - (collectionView.ContentOffset.X + collectionView.VisibleSize.Width);
            return distanceToEnd > 0;
        }

        public static bool CanScrollToTop(this UICollectionView collectionView)
        {
            return collectionView.ContentOffset.Y > 0;
        }

        public static bool CanScrollToBottom(this UICollectionView collectionView)
        {
            var distanceToEnd = collectionView.ContentSize.Height - (collectionView.ContentOffset.Y + collectionView.VisibleSize.Height);
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

        public static void ScrollLeft(this UICollectionView collectionView)
        {
            collectionView?.SetContentOffset(new CGPoint(Math.Max(0, collectionView.ContentOffset.X - collectionView.VisibleSize.Width), collectionView.ContentOffset.Y), true);
        }

        public static void ScrollRight(this UICollectionView collectionView)
        {
            collectionView?.SetContentOffset(new CGPoint(Math.Min(collectionView.ContentSize.Width - collectionView.VisibleSize.Width, collectionView.ContentOffset.X + collectionView.VisibleSize.Width), collectionView.ContentOffset.Y), true);
        }

        public static void ScrollUp(this UICollectionView collectionView)
        {
            collectionView?.SetContentOffset(new CGPoint(collectionView.ContentOffset.X, Math.Max(0, collectionView.ContentOffset.Y - collectionView.VisibleSize.Height)), true);
        }

        public static void ScrollDown(this UICollectionView collectionView)
        {
            collectionView?.SetContentOffset(new CGPoint(collectionView.ContentOffset.X, Math.Min(collectionView.ContentSize.Height - collectionView.VisibleSize.Height, collectionView.ContentOffset.Y + collectionView.VisibleSize.Height)), true);
        }
    }
}
#endif