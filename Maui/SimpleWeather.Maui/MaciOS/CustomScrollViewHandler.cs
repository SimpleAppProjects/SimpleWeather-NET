#if __IOS__
#nullable enable
using System;
using Microsoft.Maui.Handlers;
using PlatformView = UIKit.UIScrollView;

namespace SimpleWeather.Maui
{
    public partial class CustomScrollViewHandler : IScrollViewHandler
    {
        public static IPropertyMapper<IScrollView, IScrollViewHandler> Mapper = new PropertyMapper<IScrollView, IScrollViewHandler>(ViewMapper)
        {
            [nameof(IScrollView.Content)] = MapContent,
            [nameof(IScrollView.HorizontalScrollBarVisibility)] = MapHorizontalScrollBarVisibility,
            [nameof(IScrollView.VerticalScrollBarVisibility)] = MapVerticalScrollBarVisibility,
            [nameof(IScrollView.Orientation)] = MapOrientation,
            [nameof(IScrollView.IsEnabled)] = MapIsEnabled,
        };

        public static CommandMapper<IScrollView, IScrollViewHandler> CommandMapper = new(ViewCommandMapper)
        {
            [nameof(IScrollView.RequestScrollTo)] = MapRequestScrollTo
        };

        public CustomScrollViewHandler() : base(Mapper, CommandMapper)
        {

        }

        public CustomScrollViewHandler(IPropertyMapper? mapper)
            : base(mapper ?? Mapper, CommandMapper)
        {
        }

        public CustomScrollViewHandler(IPropertyMapper? mapper, CommandMapper? commandMapper)
            : base(mapper ?? Mapper, commandMapper ?? CommandMapper)
        {
        }

        IScrollView IScrollViewHandler.VirtualView => VirtualView;

        PlatformView IScrollViewHandler.PlatformView => PlatformView;
    }
}
#endif