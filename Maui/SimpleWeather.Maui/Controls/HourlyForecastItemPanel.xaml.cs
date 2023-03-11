using System;
using System.Linq;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using SimpleWeather.NET.Controls;
using SimpleWeather.Maui.Helpers;
using SimpleWeather.Utils;
using SimpleWeather.Helpers;
#if IOS || MACCATALYST
using Foundation;
using UIKit;
#endif

namespace SimpleWeather.Maui.Controls;

public partial class HourlyForecastItemPanel : ContentView
{
	public ICollection<HourlyForecastNowViewModel> ForecastData
	{
		get => (ICollection<HourlyForecastNowViewModel>)GetValue(ForecastDataProperty);
		set => SetValue(ForecastDataProperty, value);
	}

	public static readonly BindableProperty ForecastDataProperty =
		BindableProperty.Create(nameof(ForecastData), typeof(ICollection<HourlyForecastNowViewModel>), typeof(HourlyForecastItemPanel), null);

    // Hooks
#if IOS || MACCATALYST
    private UICollectionViewController PlatformScroller;
    private NSObject ScrollerObserver;
#else
    private object PlatformScroller;
#endif
    public event EventHandler<ItemTappedEventArgs> ItemClick;

	public int GetItemPosition(object item)
	{
		return HourlyForecastControl.ItemsSource?.IndexOf(item) ?? 0;
    }

    public HourlyForecastItemPanel()
    {
        InitializeComponent();
    }

    private void HourlyForecastControl_Loaded(object sender, EventArgs e)
    {
        HourlyForecastControl.Scrolled += HorizontalScroller_Scrolled;
        HourlyForecastControl.SizeChanged += HourlyForecastControl_SizeChanged; ;
        HourlyForecastControl.HandlerChanged += HourlyForecastControl_HandlerChanged;
        UpdateHourlyForecastControlHandler();
    }

    private void HourlyForecastControl_Unloaded(object sender, EventArgs e)
    {
        HourlyForecastControl.Scrolled -= HorizontalScroller_Scrolled;
        HourlyForecastControl.SizeChanged -= HourlyForecastControl_SizeChanged; ;
        HourlyForecastControl.HandlerChanged -= HourlyForecastControl_HandlerChanged;

#if IOS || MACCATALYST
        if (ScrollerObserver != null)
        {
            ScrollerObserver?.Dispose();
            ScrollerObserver = null;
        }
#endif
    }

    private void HourlyForecastControl_HandlerChanged(object sender, EventArgs e)
    {
        UpdateHourlyForecastControlHandler();
    }

    private void UpdateHourlyForecastControlHandler()
    {
#if IOS || MACCATALYST
        PlatformScroller = (HourlyForecastControl.Handler as IPlatformViewHandler)?.PlatformView?.ParentFocusEnvironment as UICollectionViewController;
        if (ScrollerObserver != null)
        {
            PlatformScroller?.CollectionView?.RemoveObserver(ScrollerObserver, "contentSize");
            ScrollerObserver?.Dispose();
            ScrollerObserver = null;
        }
        ScrollerObserver = PlatformScroller?.CollectionView?.AddObserver("contentSize", NSKeyValueObservingOptions.OldNew, (e) =>
        {
            UpdateScrollButtons(PlatformScroller);
        }) as NSObject;
#else
        PlatformScroller = HourlyForecastControl.Handler?.PlatformView;
#endif

        UpdateScrollButtons(PlatformScroller);
    }

    private void HorizontalScroller_Scrolled(object sender, ItemsViewScrolledEventArgs e)
    {
        UpdateScrollButtons(PlatformScroller);
    }

    private void HourlyForecastControl_SizeChanged(object sender, EventArgs e)
    {
        UpdateScrollButtons(PlatformScroller);
    }

#if IOS || MACCATALYST
    private void UpdateScrollButtons(UICollectionViewController controller)
    {
        if (controller?.CollectionView is UICollectionView collectionView)
        {
            CanScrollToStart = collectionView.CanScrollToStart();
            CanScrollToEnd = collectionView.CanScrollToEnd();

            if (DeviceInfo.Current.Idiom == DeviceIdiom.Desktop)
            {
                if (collectionView.ContentSize.Width > collectionView.VisibleSize.Width)
                {
                    LeftButton.IsVisible = RightButton.IsVisible = true;
                }
                else
                {
                    LeftButton.IsVisible = RightButton.IsVisible = false;
                }
            }
            else
            {
                LeftButton.IsVisible = RightButton.IsVisible = false;
            }
        }
        else
        {
            CanScrollToStart = CanScrollToEnd = false;
            LeftButton.IsVisible = RightButton.IsVisible = false;
        }
    }
#else
    private void UpdateScrollButtons(object controller)
    {
    }
#endif

    public bool CanScrollToStart
    {
        get => (bool)GetValue(CanScrollToStartProperty);
        set => SetValue(CanScrollToStartProperty, value);
    }

    public static readonly BindableProperty CanScrollToStartProperty =
        BindableProperty.Create(nameof(CanScrollToStart), typeof(bool),
        typeof(HourlyForecastItemPanel), false);

    public bool CanScrollToEnd
    {
        get => (bool)GetValue(CanScrollToEndProperty);
        set => SetValue(CanScrollToEndProperty, value);
    }

    public static readonly BindableProperty CanScrollToEndProperty =
        BindableProperty.Create(nameof(CanScrollToEnd), typeof(bool),
        typeof(HourlyForecastItemPanel), false);

    private void LeftButton_Click(object sender, EventArgs e)
    {
#if IOS || MACCATALYST
        PlatformScroller?.CollectionView?.ScrollLeft();
#endif
    }

    private void RightButton_Click(object sender, EventArgs e)
    {
#if IOS || MACCATALYST
        PlatformScroller?.CollectionView?.ScrollRight();
#endif
    }

    private void HourlyItem_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is BindableObject obj)
        {
            var item = obj.BindingContext;
            var itemIdx = GetItemPosition(item);

            ItemClick?.Invoke(sender, new ItemTappedEventArgs(HourlyForecastControl.ItemsSource, sender, itemIdx));
        }
    }
}
