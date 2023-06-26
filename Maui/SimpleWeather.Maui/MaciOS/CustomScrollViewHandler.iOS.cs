#if __IOS__
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using CoreGraphics;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Layouts;
using Microsoft.Maui.Platform;
using ObjCRuntime;
using UIKit;
using Size = Microsoft.Maui.Graphics.Size;
using ContentView = SimpleWeather.Maui.CustomContentView;
using CoreAnimation;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics.Platform;
using Microsoft.Maui.Primitives;

namespace SimpleWeather.Maui
{
    /* NOTE: Temporary until .NET 8 */
    public partial class CustomScrollViewHandler : ViewHandler<IScrollView, UIScrollView>, ICrossPlatformLayout
    {
        const nint ContentPanelTag = 0x845fed;

        readonly ScrollEventProxy _eventProxy = new();

        public override bool NeedsContainer
        {
            get
            {
                //if we are being wrapped by a BorderView we need a container
                //so we can handle masks and clip shapes
                if (VirtualView?.Parent is IBorderView)
                {
                    return true;
                }
                return base.NeedsContainer;
            }
        }

        protected override UIScrollView CreatePlatformView()
        {
            return new UIScrollView();
        }

        protected override void ConnectHandler(UIScrollView platformView)
        {
            base.ConnectHandler(platformView);

            _eventProxy.Connect(VirtualView, platformView);
        }

        protected override void DisconnectHandler(UIScrollView platformView)
        {
            base.DisconnectHandler(platformView);

            _eventProxy.Disconnect(platformView);
        }

        public static void MapContent(IScrollViewHandler handler, IScrollView scrollView)
        {
            if (handler.PlatformView == null || handler.MauiContext == null)
                return;

            if (handler is not ICrossPlatformLayout crossPlatformLayout)
            {
                return;
            }

            // We'll use the local cross-platform layout methods defined in our handler (which wrap the ScrollView's default methods)
            // so we can normalize the behavior of the scrollview to match the other platforms
            UpdateContentView(scrollView, handler, crossPlatformLayout);
        }

        // We don't actually have this mapped because we don't need it, but we can't remove it because it's public
        public static void MapContentSize(IScrollViewHandler handler, IScrollView scrollView)
        {
            handler.PlatformView?.UpdateContentSize(scrollView.ContentSize);
        }

        public static void MapIsEnabled(IScrollViewHandler handler, IScrollView scrollView)
        {
            handler.PlatformView?.UpdateIsEnabled(scrollView);
        }

        public static void MapHorizontalScrollBarVisibility(IScrollViewHandler handler, IScrollView scrollView)
        {
            handler.PlatformView?.UpdateHorizontalScrollBarVisibility(scrollView.HorizontalScrollBarVisibility);
        }

        public static void MapVerticalScrollBarVisibility(IScrollViewHandler handler, IScrollView scrollView)
        {
            handler.PlatformView?.UpdateVerticalScrollBarVisibility(scrollView.VerticalScrollBarVisibility);
        }

        public static void MapOrientation(IScrollViewHandler handler, IScrollView scrollView)
        {
            if (handler?.PlatformView is not UIScrollView uiScrollView)
            {
                return;
            }

            // If the UIScrollView hasn't been laid out yet, this will basically do nothing.
            // If it has been, we can just update the ContentSize here and get the new orientation working
            // without having to re-layout the ScrollView

            var fullContentSize = scrollView.PresentedContent?.DesiredSize ?? Size.Zero;
            var viewportBounds = uiScrollView.Bounds;
            var viewportWidth = viewportBounds.Width;
            var viewportHeight = viewportBounds.Height;
            SetContentSizeForOrientation(uiScrollView, viewportWidth, viewportHeight, scrollView.Orientation, fullContentSize);
        }

        public static void MapRequestScrollTo(IScrollViewHandler handler, IScrollView scrollView, object? args)
        {
            if (args is ScrollToRequest request)
            {
                var uiScrollView = handler.PlatformView;
                var availableScrollHeight = uiScrollView.ContentSize.Height - uiScrollView.Frame.Height;
                var availableScrollWidth = uiScrollView.ContentSize.Width - uiScrollView.Frame.Width;
                var minScrollHorizontal = Math.Min(request.HorizontalOffset, availableScrollWidth);
                var minScrollVertical = Math.Min(request.VerticalOffset, availableScrollHeight);
                uiScrollView.SetContentOffset(new CoreGraphics.CGPoint(minScrollHorizontal, minScrollVertical), !request.Instant);

                if (request.Instant)
                {
                    scrollView.ScrollFinished();
                }
            }
        }

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

        static void UpdateContentView(IScrollView scrollView, IScrollViewHandler handler, ICrossPlatformLayout crossPlatformLayout)
        {
            if (scrollView.PresentedContent == null || handler.MauiContext == null)
            {
                return;
            }

            var platformScrollView = handler.PlatformView;
            var nativeContent = scrollView.PresentedContent.ToPlatform(handler.MauiContext);

            if (GetContentView(platformScrollView) is ContentView currentContentContainer)
            {
                if (currentContentContainer.Subviews.Length == 0 || currentContentContainer.Subviews[0] != nativeContent)
                {
                    currentContentContainer.ClearSubviews();
                    currentContentContainer.AddSubview(nativeContent);
                    currentContentContainer.View = scrollView.PresentedContent;
                }
            }
            else
            {
                InsertContentView(platformScrollView, scrollView, nativeContent, crossPlatformLayout);
            }
        }

        static void InsertContentView(UIScrollView platformScrollView, IScrollView scrollView, UIView platformContent, ICrossPlatformLayout crossPlatformLayout)
        {
            if (scrollView.PresentedContent == null)
            {
                return;
            }

            var contentContainer = new CustomContentView()
            {
                View = scrollView.PresentedContent,
                Tag = ContentPanelTag
            };

            // This is where we normally would inject the cross-platform ScrollView's layout logic; instead, we're injecting the
            // methods from this handler so it can make some adjustments for things like Padding before the default logic is invoked
            contentContainer.CrossPlatformLayout = crossPlatformLayout;

            platformScrollView.ClearSubviews();
            contentContainer.AddSubview(platformContent);
            platformScrollView.AddSubview(contentContainer);
        }

        static Size MeasureScrollViewContent(double widthConstraint, double heightConstraint, Func<double, double, Size> internalMeasure, UIScrollView platformScrollView, IScrollView scrollView)
        {
            var presentedContent = scrollView.PresentedContent;
            if (presentedContent == null)
            {
                return Size.Zero;
            }

            var scrollViewBounds = platformScrollView.Bounds;
            var padding = scrollView.Padding;

            if (widthConstraint == 0)
            {
                widthConstraint = scrollViewBounds.Width;
            }

            if (heightConstraint == 0)
            {
                heightConstraint = scrollViewBounds.Height;
            }

            // Account for the ScrollView Padding before measuring the content
            widthConstraint = AccountForPadding(widthConstraint, padding.HorizontalThickness);
            heightConstraint = AccountForPadding(heightConstraint, padding.VerticalThickness);

            var result = internalMeasure.Invoke(widthConstraint, heightConstraint);

            return result.AdjustForFill(new Rect(0, 0, widthConstraint, heightConstraint), presentedContent);
        }

        public override Size GetDesiredSize(double widthConstraint, double heightConstraint)
        {
            var virtualView = VirtualView;
            var platformView = PlatformView;

            if (platformView == null || virtualView == null)
            {
                return new Size(widthConstraint, heightConstraint);
            }

            var padding = virtualView.Padding;

            // Account for the ScrollView Padding before measuring the content
            widthConstraint = AccountForPadding(widthConstraint, padding.HorizontalThickness);
            heightConstraint = AccountForPadding(heightConstraint, padding.VerticalThickness);

            var crossPlatformContentSize = virtualView.CrossPlatformMeasure(widthConstraint, heightConstraint);

            // Add the padding back in for the final size
            crossPlatformContentSize.Width += padding.HorizontalThickness;
            crossPlatformContentSize.Height += padding.VerticalThickness;

            var viewportWidth = Math.Min(crossPlatformContentSize.Width, widthConstraint);
            var viewportHeight = Math.Min(crossPlatformContentSize.Height, heightConstraint);

            // Since the UIScrollView might not be laid out yet, we can't rely on its Bounds for the viewport height/width
            // So we'll use the constraints instead.
            SetContentSizeForOrientation(platformView, widthConstraint, heightConstraint, virtualView.Orientation, crossPlatformContentSize);

            var finalWidth = ViewHandlerExtensions.ResolveConstraints(viewportWidth, virtualView.Width, virtualView.MinimumWidth, virtualView.MaximumWidth);
            var finalHeight = ViewHandlerExtensions.ResolveConstraints(viewportHeight, virtualView.Height, virtualView.MinimumHeight, virtualView.MaximumHeight);

            return new Size(finalWidth, finalHeight);
        }

        public override void PlatformArrange(Rect rect)
        {
            base.PlatformArrange(rect);

            // Ensure that the content container for the ScrollView gets arranged, and is large enough
            // to contain the ScrollView's content

            var contentView = GetContentView(PlatformView);

            if (contentView == null)
            {
                return;
            }

            var desiredSize = VirtualView.PresentedContent?.DesiredSize ?? Size.Zero;
            var scrollViewPadding = VirtualView.Padding;
            var platformViewBounds = PlatformView.Bounds;

            var contentBounds = new CGRect(0, 0,
                Math.Max(desiredSize.Width + scrollViewPadding.HorizontalThickness, platformViewBounds.Width),
                Math.Max(desiredSize.Height + scrollViewPadding.VerticalThickness, platformViewBounds.Height));

            contentView.Bounds = contentBounds;
            contentView.Center = new CGPoint(contentBounds.GetMidX(), contentBounds.GetMidY());
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

        Size ICrossPlatformLayout.CrossPlatformMeasure(double widthConstraint, double heightConstraint)
        {
            var scrollView = VirtualView;
            var platformScrollView = PlatformView;

            var presentedContent = scrollView.PresentedContent;
            if (presentedContent == null)
            {
                return Size.Zero;
            }

            var scrollViewBounds = platformScrollView.Bounds;
            var padding = scrollView.Padding;

            if (widthConstraint == 0)
            {
                widthConstraint = scrollViewBounds.Width;
            }

            if (heightConstraint == 0)
            {
                heightConstraint = scrollViewBounds.Height;
            }

            // Account for the ScrollView Padding before measuring the content
            widthConstraint = AccountForPadding(widthConstraint, padding.HorizontalThickness);
            heightConstraint = AccountForPadding(heightConstraint, padding.VerticalThickness);

            // Now handle the actual cross-platform measurement of the ScrollView's content
            var result = scrollView.CrossPlatformMeasure(widthConstraint, heightConstraint);

            return result.AdjustForFill(new Rect(0, 0, widthConstraint, heightConstraint), presentedContent);
        }

        Size ICrossPlatformLayout.CrossPlatformArrange(Rect bounds)
        {
            var scrollView = VirtualView;
            var platformScrollView = PlatformView;
            var container = GetContentView(platformScrollView);

            if (container?.Superview is UIScrollView uiScrollView)
            {
                // Ensure the container is at least the size of the UIScrollView itself, so that the 
                // cross-platform layout logic makes sense and the contents don't arrange outside the 
                // container. (Everything will look correct if they do, but hit testing won't work properly.)

                var scrollViewBounds = uiScrollView.Bounds;
                var containerBounds = container.Bounds;

                container.Bounds = new CGRect(0, 0,
                    Math.Max(containerBounds.Width, scrollViewBounds.Width),
                    Math.Max(containerBounds.Height, scrollViewBounds.Height));
                container.Center = new CGPoint(container.Bounds.GetMidX(), container.Bounds.GetMidY());
            }

            var contentSize = scrollView.CrossPlatformArrange(bounds);

            // The UIScrollView's bounds are available, so we can use them to make sure the ContentSize makes sense
            // for the ScrollView orientation
            var viewportBounds = platformScrollView.Bounds;
            var viewportHeight = viewportBounds.Height;
            var viewportWidth = viewportBounds.Width;
            SetContentSizeForOrientation(platformScrollView, viewportWidth, viewportHeight, scrollView.Orientation, contentSize);

            return contentSize;
        }

        class ScrollEventProxy
        {
            WeakReference<IScrollView>? _virtualView;

            IScrollView? VirtualView => _virtualView is not null && _virtualView.TryGetTarget(out var v) ? v : null;

            public void Connect(IScrollView virtualView, UIScrollView platformView)
            {
                _virtualView = new(virtualView);

                platformView.Scrolled += Scrolled;
                platformView.ScrollAnimationEnded += ScrollAnimationEnded;
            }

            public void Disconnect(UIScrollView platformView)
            {
                _virtualView = null;

                platformView.Scrolled -= Scrolled;
                platformView.ScrollAnimationEnded -= ScrollAnimationEnded;
            }

            void OnButtonTouchUpInside(object? sender, EventArgs e)
            {
                if (VirtualView is IButton virtualView)
                {
                    virtualView.Released();
                    virtualView.Clicked();
                }
            }

            void ScrollAnimationEnded(object? sender, EventArgs e)
            {
                VirtualView?.ScrollFinished();
            }

            void Scrolled(object? sender, EventArgs e)
            {
                if (VirtualView == null)
                {
                    return;
                }

                if (sender is not UIScrollView platformView)
                {
                    return;
                }

                VirtualView.HorizontalOffset = platformView.ContentOffset.X;
                VirtualView.VerticalOffset = platformView.ContentOffset.Y;
            }
        }
    }

    public interface ICrossPlatformLayout
    {
        /// <summary>
        /// Measures the desired size of the ICrossPlatformLayout within the given constraints.
        /// </summary>
        /// <param name="widthConstraint">The width limit for measuring the ICrossPlatformLayout.</param>
        /// <param name="heightConstraint">The height limit for measuring the ICrossPlatformLayout.</param>
        /// <returns>The desired size of the ILayout.</returns>
        Size CrossPlatformMeasure(double widthConstraint, double heightConstraint);

        /// <summary>
        /// Arranges the children of the ICrossPlatformLayout within the given bounds.
        /// </summary>
        /// <param name="bounds">The bounds in which the ICrossPlatformLayout's children should be arranged.</param>
        /// <returns>The actual size of the arranged ICrossPlatformLayout.</returns>
        Size CrossPlatformArrange(Rect bounds);
    }

    /// <summary>
	/// Indicates a control which supports cross-platform layout operations
	/// </summary>
	public interface ICrossPlatformLayoutBacking
    {
        /// <summary>
        /// Gets or sets the implementation of cross-platform layout operations to be carried out by this control
        /// </summary>
        /// <remarks>
        /// This property is the bridge between the platform-level backing control and the cross-platform-level
        /// layout. It is typically connected by the handler, which may add additional logic to normalize layout
        /// and content behaviors across the various platforms. 
        /// </remarks>
        public ICrossPlatformLayout? CrossPlatformLayout { get; set; }
    }

    public abstract class MauiView : UIView, ICrossPlatformLayoutBacking
    {
        static bool? _respondsToSafeArea;

        double _lastMeasureHeight = double.NaN;
        double _lastMeasureWidth = double.NaN;

        WeakReference<IView>? _reference;
        WeakReference<ICrossPlatformLayout>? _crossPlatformLayoutReference;

        public IView? View
        {
            get => _reference != null && _reference.TryGetTarget(out var v) ? v : null;
            set => _reference = value == null ? null : new(value);
        }

        bool RespondsToSafeArea()
        {
            if (_respondsToSafeArea.HasValue)
                return _respondsToSafeArea.Value;
            return (bool)(_respondsToSafeArea = RespondsToSelector(new Selector("safeAreaInsets")));
        }

        protected CGRect AdjustForSafeArea(CGRect bounds)
        {
            if (View is not ISafeAreaView sav || sav.IgnoreSafeArea || !RespondsToSafeArea())
            {
                return bounds;
            }

#pragma warning disable CA1416 // TODO 'UIView.SafeAreaInsets' is only supported on: 'ios' 11.0 and later, 'maccatalyst' 11.0 and later, 'tvos' 11.0 and later.
            return SafeAreaInsets.InsetRect(bounds);
#pragma warning restore CA1416
        }

        protected bool IsMeasureValid(double widthConstraint, double heightConstraint)
        {
            // Check the last constraints this View was measured with; if they're the same,
            // then the current measure info is already correct and we don't need to repeat it
            return heightConstraint == _lastMeasureHeight && widthConstraint == _lastMeasureWidth;
        }

        protected void InvalidateConstraintsCache()
        {
            _lastMeasureWidth = double.NaN;
            _lastMeasureHeight = double.NaN;
        }

        protected void CacheMeasureConstraints(double widthConstraint, double heightConstraint)
        {
            _lastMeasureWidth = widthConstraint;
            _lastMeasureHeight = heightConstraint;
        }

        public ICrossPlatformLayout? CrossPlatformLayout
        {
            get => _crossPlatformLayoutReference != null && _crossPlatformLayoutReference.TryGetTarget(out var v) ? v : null;
            set => _crossPlatformLayoutReference = value == null ? null : new WeakReference<ICrossPlatformLayout>(value);
        }

        Size CrossPlatformMeasure(double widthConstraint, double heightConstraint)
        {
            return CrossPlatformLayout?.CrossPlatformMeasure(widthConstraint, heightConstraint) ?? Size.Zero;
        }

        Size CrossPlatformArrange(Rect bounds)
        {
            return CrossPlatformLayout?.CrossPlatformArrange(bounds) ?? Size.Zero;
        }

        public override CGSize SizeThatFits(CGSize size)
        {
            if (_crossPlatformLayoutReference == null)
            {
                return base.SizeThatFits(size);
            }

            var widthConstraint = size.Width;
            var heightConstraint = size.Height;

            var crossPlatformSize = CrossPlatformMeasure(widthConstraint, heightConstraint);

            CacheMeasureConstraints(widthConstraint, heightConstraint);

            return crossPlatformSize.ToCGSize();
        }

        // TODO: Possibly reconcile this code with ViewHandlerExtensions.LayoutVirtualView
        // If you make changes here please review if those changes should also
        // apply to ViewHandlerExtensions.LayoutVirtualView
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (_crossPlatformLayoutReference == null)
            {
                return;
            }

            var bounds = AdjustForSafeArea(Bounds).ToRectangle();

            var widthConstraint = bounds.Width;
            var heightConstraint = bounds.Height;

            // If the SuperView is a MauiView (backing a cross-platform ContentView or Layout), then measurement
            // has already happened via SizeThatFits and doesn't need to be repeated in LayoutSubviews. But we
            // _do_ need LayoutSubviews to make a measurement pass if the parent is something else (for example,
            // the window); there's no guarantee that SizeThatFits has been called in that case.

            if (!IsMeasureValid(widthConstraint, heightConstraint) && Superview is not MauiView)
            {
                CrossPlatformMeasure(widthConstraint, heightConstraint);
                CacheMeasureConstraints(widthConstraint, heightConstraint);
            }

            CrossPlatformArrange(bounds);
        }

        public override void SetNeedsLayout()
        {
            InvalidateConstraintsCache();
            base.SetNeedsLayout();
            Superview?.SetNeedsLayout();
        }
    }

    public class CustomContentView : MauiView
    {
        WeakReference<IBorderStroke>? _clip;
        CAShapeLayer? _childMaskLayer;
        internal event EventHandler? LayoutSubviewsChanged;

        public CustomContentView()
        {
            Layer.CornerCurve = CACornerCurve.Continuous;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            var bounds = AdjustForSafeArea(Bounds).ToRectangle();

            if (ChildMaskLayer != null)
                ChildMaskLayer.Frame = bounds;

            SetClip();

            LayoutSubviewsChanged?.Invoke(this, EventArgs.Empty);
        }

        internal IBorderStroke? Clip
        {
            get
            {
                if (_clip?.TryGetTarget(out IBorderStroke? target) == true)
                    return target;

                return null;
            }
            set
            {
                _clip = null;

                if (value != null)
                    _clip = new WeakReference<IBorderStroke>(value);

                SetClip();
            }
        }

        internal CAShapeLayer? ChildMaskLayer
        {
            get => _childMaskLayer;
            set
            {
                var layer = GetChildLayer();

                if (layer != null && _childMaskLayer != null)
                    layer.Mask = null;

                _childMaskLayer = value;

                if (layer != null)
                    layer.Mask = value;
            }
        }

        CALayer? GetChildLayer()
        {
            if (Subviews.Length == 0)
                return null;

            var child = Subviews[0];

            if (child.Layer is null)
                return null;

            return child.Layer;
        }

        void SetClip()
        {
            if (Subviews.Length == 0)
                return;

            var maskLayer = ChildMaskLayer;

            if (maskLayer is null && Clip is null)
                return;

            maskLayer ??= ChildMaskLayer = new CAShapeLayer();

            var frame = Frame;

            if (frame == CGRect.Empty)
                return;

            var strokeThickness = (float)(Clip?.StrokeThickness ?? 0);

            // In the MauiCALayer class, the Stroke is inner and we are clipping the outer, for that reason,
            // we use the double to get the correct value. Here, again, we use the double to get the correct clip shape size values.
            var strokeWidth = 2 * strokeThickness;

            var bounds = new RectF(0, 0, (float)frame.Width - strokeWidth, (float)frame.Height - strokeWidth);

            IShape? clipShape = Clip?.Shape;
            PathF? path;

            /*
            if (clipShape is IRoundRectangle roundRectangle)
                path = roundRectangle.InnerPathForBounds(bounds, strokeThickness);
            else*/
                path = clipShape?.PathForBounds(bounds);

            var nativePath = path?.AsCGPath();

            maskLayer.Path = nativePath;
        }
    }

    internal static partial class ViewHandlerExtensions
    {
        internal static double ResolveConstraints(double measured, double exact, double min, double max)
        {
            var resolved = measured;

            min = Dimension.ResolveMinimum(min);

            if (Dimension.IsExplicitSet(exact))
            {
                // If an exact value has been specified, try to use that
                resolved = exact;
            }

            if (resolved > max)
            {
                // Apply the max value constraint (if any)
                // If the exact value is in conflict with the max value, the max value should win
                resolved = max;
            }

            if (resolved < min)
            {
                // Apply the min value constraint (if any)
                // If the exact or max value is in conflict with the min value, the min value should win
                resolved = min;
            }

            return resolved;
        }
    }
}
#endif