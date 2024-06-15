#if false
using System;
using CoreGraphics;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Compatibility.Platform.iOS;
using Microsoft.Maui.Controls.Platform;
using System.ComponentModel;
using Microsoft.Maui.Handlers;
using SimpleWeather.Maui.Controls;
using UIKit;
using Microsoft.Maui.Platform;
using Platform = Microsoft.Maui.Controls.Compatibility.Platform.iOS.Platform;

namespace SimpleWeather.Maui;

public partial class SplitViewPageRenderer : UISplitViewController
{
    UIViewController _flyoutController;
    UIViewController _detailController;

    bool _disposed;
    SplitViewPage _splitViewPage;
    CGSize _previousSize = CGSize.Empty;
    Page PageController => Element as Page;

    protected SplitViewPage SplitViewPage => _splitViewPage ??= (SplitViewPage)Element;

    public SplitViewPageRenderer() : base(UISplitViewControllerStyle.DoubleColumn)
    {
        PreferredPrimaryColumnWidthFraction = 0.5f;
    }

    protected override void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (disposing)
        {
            if (Element != null)
            {
                PageController.SendDisappearing();
                Element.PropertyChanged -= HandlePropertyChanged;
                Element = null;
            }

            ClearControllers();
        }

        base.Dispose(disposing);
    }

    public VisualElement Element { get; private set; }

    public event EventHandler<VisualElementChangedEventArgs> ElementChanged;

    public SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
    {
        return NativeView.GetSizeRequest(widthConstraint, heightConstraint);
    }

    public UIView NativeView
    {
        get { return View; }
    }

    public void SetElement(VisualElement element)
    {
        var oldElement = Element;
        Element = element;

        ViewControllers = new[] { _flyoutController = new ChildViewController(), _detailController = new ChildViewController() };

        UpdateControllers();

        PresentsWithGesture = false;
        OnElementChanged(new VisualElementChangedEventArgs(oldElement, element));
    }

    public void SetElementSize(Size size)
    {
        Element.Layout(new Rect(Element.X, Element.Width, size.Width, size.Height));
    }

    public UIViewController ViewController
    {
        get { return this; }
    }

    public override void ViewDidAppear(bool animated)
    {
        PageController.SendAppearing();
        base.ViewDidAppear(animated);
    }

    public override void ViewDidDisappear(bool animated)
    {
        base.ViewDidDisappear(animated);
        PageController?.SendDisappearing();
    }

    public override void ViewDidLayoutSubviews()
    {
        base.ViewDidLayoutSubviews();

        bool layoutFlyout = _flyoutController?.View?.Superview != null;
        bool layoutDetails = _detailController?.View?.Superview != null;

        if (layoutFlyout)
        {
            var flyoutBounds = _flyoutController.View.Frame;
            if (!flyoutBounds.IsEmpty)
                SplitViewPage.Pane1Bounds = new Rect(0, 0, flyoutBounds.Width, flyoutBounds.Height);
        }

        if (layoutDetails)
        {
            var detailsBounds = _detailController.View.Frame;
            if (!detailsBounds.IsEmpty)
                SplitViewPage.Pane2Bounds = new Rect(0, 0, detailsBounds.Width, detailsBounds.Height);
        }
    }

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();
        UpdateFlowDirection();
        UpdateFlyoutLayoutBehavior(View.Bounds.Size);
    }

    void UpdateFlyoutLayoutBehavior(CGSize newBounds)
    {
        SplitViewPage flyoutDetailPage = _splitViewPage ?? Element as SplitViewPage;

        if (flyoutDetailPage == null)
            return;

        bool isPortrait = newBounds.Height > newBounds.Width;

        switch (flyoutDetailPage.FlyoutLayoutBehavior)
        {
#pragma warning disable CA1416, CA1422 // TODO:  UISplitViewControllerDisplayMode.AllVisible, PrimaryHidden is unsupported on: 'ios' 14.0 and late
            case FlyoutLayoutBehavior.Split:
                PreferredDisplayMode = UISplitViewControllerDisplayMode.AllVisible;
                break;
            case FlyoutLayoutBehavior.Popover:
                PreferredDisplayMode = UISplitViewControllerDisplayMode.PrimaryHidden;
                break;
            case FlyoutLayoutBehavior.SplitOnPortrait:
                PreferredDisplayMode = (isPortrait) ? UISplitViewControllerDisplayMode.AllVisible : UISplitViewControllerDisplayMode.PrimaryHidden;
                break;
            case FlyoutLayoutBehavior.SplitOnLandscape:
                PreferredDisplayMode = (!isPortrait) ? UISplitViewControllerDisplayMode.AllVisible : UISplitViewControllerDisplayMode.PrimaryHidden;
                break;
#pragma warning restore CA1416, CA1422
            default:
                PreferredDisplayMode = UISplitViewControllerDisplayMode.Automatic;
                break;
        }

        MaximumPrimaryColumnWidth = newBounds.Width * 0.5f;

        //SplitViewPage.UpdateFlyoutLayoutBehavior();
    }

    protected virtual void OnElementChanged(VisualElementChangedEventArgs e)
    {
        if (e.OldElement != null)
            e.OldElement.PropertyChanged -= HandlePropertyChanged;

        if (e.NewElement != null)
            e.NewElement.PropertyChanged += HandlePropertyChanged;

        var changed = ElementChanged;
        if (changed != null)
            changed(this, e);
    }

    void ClearControllers()
    {
        foreach (var controller in _flyoutController.ChildViewControllers)
        {
            controller.View.RemoveFromSuperview();
            controller.RemoveFromParentViewController();
        }

        foreach (var controller in _detailController.ChildViewControllers)
        {
            controller.View.RemoveFromSuperview();
            controller.RemoveFromParentViewController();
        }
    }

    void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "Pane1" || e.PropertyName == "Pane2")
            UpdateControllers();
        else if (e.PropertyName == VisualElement.FlowDirectionProperty.PropertyName)
            UpdateFlowDirection();
        else if (e.PropertyName == SplitViewPage.FlyoutLayoutBehaviorProperty.PropertyName)
            UpdateFlyoutLayoutBehavior(base.View.Bounds.Size);
    }

    public override void ViewWillTransitionToSize(CGSize toSize, IUIViewControllerTransitionCoordinator coordinator)
    {
        base.ViewWillTransitionToSize(toSize, coordinator);

        if (_previousSize != toSize)
        {
            _previousSize = toSize;
            UpdateFlyoutLayoutBehavior(toSize);
        }
    }

    public override void WillTransitionToTraitCollection(UITraitCollection traitCollection, IUIViewControllerTransitionCoordinator coordinator)
    {
        base.WillTransitionToTraitCollection(traitCollection, coordinator);
        UpdateFlyoutLayoutBehavior((coordinator?.ContainerView?.Bounds ?? View.Bounds).Size);
    }

    void UpdateControllers()
    {
        if (SplitViewPage.Pane1 is VisualElement pane1 && Platform.GetRenderer(pane1) == null)
            Platform.SetRenderer(pane1, Platform.CreateRenderer(pane1));
        if (SplitViewPage.Pane2 is VisualElement pane2 && Platform.GetRenderer(pane2) == null)
            Platform.SetRenderer(pane2, Platform.CreateRenderer(pane2));

        ClearControllers();

        if (SplitViewPage.Pane1 is not null)
        {
            var flyout = Platform.GetRenderer(SplitViewPage.Pane1).ViewController;
            _flyoutController.View.AddSubview(flyout.View);
            _flyoutController.AddChildViewController(flyout);
        }

        if (SplitViewPage.Pane2 is not null)
        {
            var detail = Platform.GetRenderer(SplitViewPage.Pane2).ViewController;
            _detailController.View.AddSubview(detail.View);
            _detailController.AddChildViewController(detail);
        }
    }

    void UpdateFlowDirection()
    {
        NativeView.UpdateFlowDirection(Element);

        if (NativeView.Superview != null)
        {
            var view = NativeView.Superview;
            NativeView.RemoveFromSuperview();
            view.AddSubview(NativeView);
        }
    }
}

internal class ChildViewController : UIViewController
{
    public override void ViewDidLayoutSubviews()
    {
        foreach (var vc in ChildViewControllers)
            vc.View.Frame = View.Bounds;
    }
}
#endif