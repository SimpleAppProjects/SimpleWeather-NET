﻿#if IOS || MACCATALYST

using CoreAnimation;
using CoreGraphics;
using Foundation;
using Microsoft.Maui.Platform;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using UIKit;

// Partially based on the .NET MAUI Community Toolkit Popup control - https://github.com/CommunityToolkit/Maui

namespace SimpleToolkit.Core.Handlers
{
    public class PopoverViewController : UIViewController
    {
        private readonly IMauiContext mauiContext;
        private Microsoft.Maui.Controls.Grid contentView = null;

        internal UIViewController ViewController { get; private set; }
        public IPopover VirtualView { get; private set; }


        public PopoverViewController(IMauiContext mauiContext)
        {
            this.mauiContext = mauiContext ?? throw new ArgumentNullException(nameof(mauiContext));
        }


        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (View.Superview is null)
                return;

            View.Superview.ClipsToBounds = false;
            View.Superview.Layer.CornerRadius = 0f;
            View.Superview.Layer.BackgroundColor = Colors.Transparent.ToCGColor();
            View.Superview.Layer.ShadowColor = null;
            View.Superview.Layer.ShadowOpacity = 0f;
            View.Layer.ShadowColor = null;
            View.Layer.ShadowOpacity = 0f;
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            if (VirtualView.Content is null)
                return;

            var measure = (VirtualView.Content as IView).Measure(double.PositiveInfinity, double.PositiveInfinity);
            PreferredContentSize = new CGSize(measure.Width, measure.Height);

            foreach (var subview in View.Subviews)
            {
                subview.SizeToFit();
            }
        }

        public override void ViewWillDisappear(bool animated)
        {
            if (ViewController?.View is UIView view)
            {
                view.Alpha = 1f;
            }
            base.ViewWillDisappear(animated);
        }

        [MemberNotNull(nameof(VirtualView), nameof(ViewController))]
        public void SetElement(IPopover element)
        {
            VirtualView = element;
            ModalPresentationStyle = UIModalPresentationStyle.Popover;

            _ = View ?? throw new InvalidOperationException($"{nameof(View)} cannot be null");
            _ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} cannot be null.");

            var rootViewController = WindowStateManager.Default.GetCurrentUIViewController();
            ViewController ??= rootViewController?.ModalViewController ?? rootViewController;
            SetDimmingBackgroundEffect();
        }

        public void CleanUp()
        {
            if (VirtualView is null)
                return;

            VirtualView = null;

            View.ClearSubviews();

            if (PresentationController is UIPopoverPresentationController presentationController)
                presentationController.Delegate = null;
        }

        [MemberNotNull(nameof(ViewController))]
        public void InitializeView(in IPopover virtualView, in IElement anchor)
        {
            UpdateContainerGrid(virtualView);

            SetPresentationController();

            _ = View ?? throw new InvalidOperationException($"{nameof(View)} cannot be null");
            SetView(View);

            _ = ViewController ?? throw new InvalidOperationException($"{nameof(ViewController)} cannot be null");
            AddToCurrentPageViewController(ViewController);

            SetLayout(virtualView, anchor);
        }

        public void SetLayout(IPopover popover, IElement anchor)
        {
            if (View is null)
                return;

            var view = anchor.ToPlatform(popover.Handler?.MauiContext ?? throw new NullReferenceException());
            PopoverPresentationController.SourceView = view;
            PopoverPresentationController.SourceRect = view.Bounds;
        }

        public void UpdateContent()
        {
            UpdateContainerGrid(VirtualView);

            SetView(View);
        }

        private void UpdateContainerGrid(IPopover virtualView)
        {
            if (contentView?.Children.Any() == true)
                contentView.Children.Clear();

            // I do not understand how sizing on iOS works. This is the only hopefully working solution I came up with
            contentView = new Microsoft.Maui.Controls.Grid
            {
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Start,
                RowDefinitions = new RowDefinitionCollection(new RowDefinition(GridLength.Auto)),
                ColumnDefinitions = new ColumnDefinitionCollection(new ColumnDefinition(GridLength.Auto)),
            };

            contentView.Children.Add(virtualView.Content);
        }

        private void SetDimmingBackgroundEffect()
        {
            if (ViewController?.View is UIView view)
                view.Alpha = 1f;
        }

        private void SetView(UIView view)
        {
            view.ClearSubviews();
            var subview = contentView?.ToHandler(mauiContext)?.PlatformView;

            if (subview is not null)
                view.AddSubview(subview);
        }

        private void SetPresentationController()
        {
            var popOverDelegate = new PopoverDelegate();
            popOverDelegate.PopoverDismissedEvent += PopOverDelegate_PopoverDismissedEvent;

            var presentationController = ((UIPopoverPresentationController)PresentationController);
            presentationController.SourceView = ViewController?.View ?? throw new InvalidOperationException($"{nameof(ViewController.View)} cannot be null");
            presentationController.Delegate = popOverDelegate;
            presentationController.PermittedArrowDirections = 0; // Because of this the popover is above the anchor
            presentationController.BackgroundColor = Colors.Transparent.ToPlatform();

            presentationController.PopoverBackgroundViewType = typeof(PopoverBackgroundView);
        }

        private void PopOverDelegate_PopoverDismissedEvent(object sender, UIPresentationController e)
        {
            if (this.VirtualView is Popover popover)
            {
                popover.OnClosed();
            }
        }

        private void AddToCurrentPageViewController(UIViewController viewController)
        {
            viewController.PresentViewController(this, true, null);
        }

        private class PopoverDelegate : UIPopoverPresentationControllerDelegate
        {
            readonly WeakEventManager popoverDismissedEventmanager = new();

            public event EventHandler<UIPresentationController> PopoverDismissedEvent
            {
                add => popoverDismissedEventmanager.AddEventHandler(value);
                remove => popoverDismissedEventmanager.RemoveEventHandler(value);
            }

            public override UIModalPresentationStyle GetAdaptivePresentationStyle(UIPresentationController forPresentationController) =>
                UIModalPresentationStyle.None;

            public override void DidDismiss(UIPresentationController presentationController) =>
                popoverDismissedEventmanager.HandleEvent(this, presentationController, nameof(PopoverDismissedEvent));
        }

        // Helps to remove default styling of the popover container
        private class PopoverBackgroundView : UIPopoverBackgroundView
        {
            [Export("arrowHeight")]
            static new NFloat GetArrowHeight()
            {
                return 0f;
            }

            [Export("arrowBase")]
            static new NFloat GetArrowBase()
            {
                return 0f;
            }

            [Export("contentViewInsets")]
            static new UIEdgeInsets GetContentViewInsets()
            {
                return UIEdgeInsets.Zero;
            }

            [Export("wantsDefaultContentAppearance")]
            static new bool WantsDefaultContentAppearance
            {
                get => false;
            }

            public override UIPopoverArrowDirection ArrowDirection { get; set; }

            public override NFloat ArrowOffset { get; set; }


            public PopoverBackgroundView(IntPtr handle) : base(handle)
            {
                ArrowOffset = 0f;
                ArrowDirection = 0;
                BackgroundColor = UIColor.Clear;

                Layer.ShadowColor = Colors.Transparent.ToCGColor();
                Layer.ShadowOpacity = 0f;
                Layer.CornerRadius = 0f;
                Layer.BackgroundColor = Colors.Transparent.ToCGColor();
                Layer.MasksToBounds = false;
            }

            public override void DrawLayer(CALayer layer, CGContext context)
            {
                layer.ShadowColor = Colors.Transparent.ToCGColor();
                layer.ShadowOpacity = 0f;
                layer.BackgroundColor = Colors.Transparent.ToCGColor();
                layer.CornerRadius = 0f;
                layer.MasksToBounds = false;

                base.DrawLayer(layer, context);
            }

            public override void Draw(CGRect rect)
            {
                base.Draw(rect);
            }

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();
            }
        }
    }
}

#endif