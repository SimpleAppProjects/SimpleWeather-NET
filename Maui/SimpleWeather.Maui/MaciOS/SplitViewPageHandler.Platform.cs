#if false
using System;
using UIKit;

namespace SimpleWeather.Maui;

public partial class SplitViewPageRenderer : IPlatformViewHandler
{
    IMauiContext _mauiContext;

    public UIView PlatformView => View;

    public UIView ContainerView => null;

    public bool HasContainer { get => false; set { } }

    public IView VirtualView => Element;

    public IMauiContext MauiContext => _mauiContext;

    object IViewHandler.ContainerView => null;

    object IElementHandler.PlatformView => View;

    IElement IElementHandler.VirtualView => Element;

    public void DisconnectHandler()
    {
        
    }

    public void Invoke(string command, object args = null)
    {
        
    }

    public void PlatformArrange(Rect frame)
    {
        
    }

    public void SetMauiContext(IMauiContext mauiContext)
    {
        _mauiContext = mauiContext;
    }

    public void SetVirtualView(IElement view)
    {
        SetElement((VisualElement)view);
    }

    public void UpdateValue(string property)
    {
        
    }

    Size IViewHandler.GetDesiredSize(double widthConstraint, double heightConstraint)
    {
        return GetDesiredSize(widthConstraint, heightConstraint).Request;
    }
}
#endif
