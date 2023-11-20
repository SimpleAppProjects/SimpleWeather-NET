#if __IOS__
#nullable enable
using System;
using Microsoft.Maui.Handlers;
using PlatformView = UIKit.UIScrollView;

namespace SimpleWeather.Maui
{
    public partial class CustomScrollViewHandler : ScrollViewHandler
    {
    }
}
#endif