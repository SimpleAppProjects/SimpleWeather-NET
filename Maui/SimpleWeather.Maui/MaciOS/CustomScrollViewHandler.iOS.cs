#if __IOS__
using System;
using CoreGraphics;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Layouts;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Microsoft.Maui.Primitives;
using UIKit;
using Size = Microsoft.Maui.Graphics.Size;
using ContentView = Microsoft.Maui.Platform.ContentView;

namespace SimpleWeather.Maui
{
    public partial class CustomScrollViewHandler : ScrollViewHandler, ICrossPlatformLayout
    {
        const nint ContentPanelTag = 0x845fed;

        // Find the internal ContentView; it may not be Subviews[0] because of the scrollbars
        static ContentView? GetContentView(UIScrollView scrollView)
        {
            for (int n = 0; n < scrollView.Subviews.Length; n++)
            {
                if (scrollView.Subviews[n] is ContentView contentView)
                {
                    if (contentView.Tag is nint tag && tag == ContentPanelTag)
                    {
                        return contentView;
                    }
                }
            }

            return null;
        }

        static double AccountForPadding(double constraint, double padding)
        {
            // Remove the padding from the constraint, but don't allow it to go negative
            return Math.Max(0, constraint - padding);
        }

        static void SetContentSizeForOrientation(UIScrollView uiScrollView, double viewportWidth, double viewportHeight, ScrollOrientation orientation, Size contentSize)
        {
            if (orientation is ScrollOrientation.Vertical or ScrollOrientation.Neither)
            {
                contentSize.Width = Math.Min(contentSize.Width, viewportWidth);
            }

            if (orientation is ScrollOrientation.Horizontal or ScrollOrientation.Neither)
            {
                contentSize.Height = Math.Min(contentSize.Height, viewportHeight);
            }

            uiScrollView.ContentSize = contentSize;
        }

        static CGSize GetViewportSize(UIScrollView platformScrollView)
        {
            return platformScrollView.AdjustedContentInset.InsetRect(platformScrollView.Bounds).Size;
        }

        Size ICrossPlatformLayout.CrossPlatformMeasure(double widthConstraint, double heightConstraint)
        {
            var scrollView = VirtualView;
            var crossPlatformLayout = scrollView as ICrossPlatformLayout;
            var platformScrollView = PlatformView;

            var presentedContent = scrollView.PresentedContent;
            if (presentedContent == null)
            {
                return Size.Zero;
            }

            var viewportSize = GetViewportSize(platformScrollView);

            var padding = scrollView.Padding;

            if (widthConstraint == 0)
            {
                widthConstraint = viewportSize.Width;
            }

            if (heightConstraint == 0)
            {
                heightConstraint = viewportSize.Height;
            }

            // Account for the ScrollView Padding before measuring the content
            widthConstraint = AccountForPadding(widthConstraint, padding.HorizontalThickness);
            heightConstraint = AccountForPadding(heightConstraint, padding.VerticalThickness);

            // Now handle the actual cross-platform measurement of the ScrollView's content
            var result = crossPlatformLayout.CrossPlatformMeasure(widthConstraint, heightConstraint);

            return result.AdjustForFill(new Rect(0, 0, widthConstraint, heightConstraint), presentedContent);
        }

        Size ICrossPlatformLayout.CrossPlatformArrange(Rect bounds)
        {
            var scrollView = VirtualView;
            var crossPlatformLayout = scrollView as ICrossPlatformLayout;
            var platformScrollView = PlatformView;

            // The UIScrollView's bounds are available, so we can use them to make sure the ContentSize makes sense
            // for the ScrollView orientation
            var viewportSize = GetViewportSize(platformScrollView);

            // Get a Rect for doing the CrossPlatformArrange of the Content
            var viewportRect = new Rect(Point.Zero, viewportSize.ToSize());

            var contentSize = crossPlatformLayout.CrossPlatformArrange(viewportRect);

            var viewportHeight = viewportSize.Height;
            var viewportWidth = viewportSize.Width;
            SetContentSizeForOrientation(platformScrollView, viewportWidth, viewportHeight, scrollView.Orientation, contentSize);

            var container = GetContentView(platformScrollView);

            if (container?.Superview is UIScrollView uiScrollView)
            {
                // Ensure the container is at least the size of the UIScrollView itself, so that the 
                // cross-platform layout logic makes sense and the contents don't arrange outside the 
                // container. (Everything will look correct if they do, but hit testing won't work properly.)
                var containerBounds = contentSize;

                container.Bounds = new CGRect(0, 0,
                    Math.Max(containerBounds.Width, viewportSize.Width),
                    Math.Max(containerBounds.Height, viewportSize.Height));

                container.Center = new CGPoint(container.Bounds.GetMidX(), container.Bounds.GetMidY());
            }

            return contentSize;
        }
    }
}
#endif