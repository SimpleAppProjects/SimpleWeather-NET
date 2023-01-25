using CommunityToolkit.Maui.Markup;
using SimpleToolkit.Core;
using SimpleWeather.Utils;
using System.Collections;

namespace SimpleWeather.Maui.Controls;

public partial class ProgressAutoSuggestBox : TemplatedView
{
    private bool suppressTextChangedEvent;
    private Border HeaderContainer;
    private SearchBar LocationSearchBox;
    private Border PopupContainer;
    private ListView SearchList;
    private IndeterminateProgressBar ProgressBarControl;

    #region Progress Controls
    public bool IsLoading
    {
        get { return (bool)GetValue(IsLoadingProperty); }
        set { SetValue(IsLoadingProperty, value); }
    }

    public static readonly BindableProperty IsLoadingProperty =
        BindableProperty.Create(nameof(IsLoading), typeof(bool), typeof(ProgressAutoSuggestBox), false);

    public Color ProgressBarTrackColor
    {
        get => (Color) GetValue(ProgressBarTrackColorProperty);
        set => SetValue(ProgressBarTrackColorProperty, value);
    }

    public static readonly BindableProperty ProgressBarTrackColorProperty =
        BindableProperty.Create(nameof(ProgressBarTrackColor), typeof(Color), typeof(ProgressAutoSuggestBox), default);

    public Color ProgressBarIndicatorColor
    {
        get => (Color) GetValue(ProgressBarIndicatorColorProperty);
        set => SetValue(ProgressBarIndicatorColorProperty, value);
    }

    public static readonly BindableProperty ProgressBarIndicatorColorProperty =
        BindableProperty.Create(nameof(ProgressBarIndicatorColor), typeof(Color), typeof(ProgressAutoSuggestBox), default);
    #endregion

    #region AutoSuggestBox Properties
    //
    // Summary:
    //     Raised before the text content of the editable control component is updated.
    public event EventHandler<AutoSuggestBoxSuggestionChosenEventArgs> SuggestionChosen;

    //
    // Summary:
    //     Raised after the text content of the editable control component is updated.
    public event EventHandler<AutoSuggestBoxTextChangedEventArgs> TextChanged;

    //
    // Summary:
    //     Occurs when the user submits a search query.
    public event EventHandler<AutoSuggestBoxQuerySubmittedEventArgs> QuerySubmitted;

    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(ProgressAutoSuggestBox), "", propertyChanged: OnTextPropertyChanged);

    public Color TextColor
    {
        get { return (Color)GetValue(TextColorProperty); }
        set { SetValue(TextColorProperty, value); }
    }

    public static readonly BindableProperty TextColorProperty =
        BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(ProgressAutoSuggestBox), default);


    public string PlaceholderText
    {
        get { return (string)GetValue(PlaceholderTextProperty); }
        set { SetValue(PlaceholderTextProperty, value); }
    }

    public static readonly BindableProperty PlaceholderTextProperty =
        BindableProperty.Create(nameof(PlaceholderText), typeof(string), typeof(ProgressAutoSuggestBox), null);

    public Color PlaceholderTextColor
    {
        get { return (Color)GetValue(PlaceholderTextColorProperty); }
        set { SetValue(PlaceholderTextColorProperty, value); }
    }

    public static readonly BindableProperty PlaceholderTextColorProperty =
        BindableProperty.Create(nameof(PlaceholderTextColor), typeof(Color), typeof(ProgressAutoSuggestBox), default);


    public bool IsSuggestionListOpen
    {
        get { return (bool)GetValue(IsSuggestionListOpenProperty); }
        set { SetValue(IsSuggestionListOpenProperty, value); }
    }

    public static readonly BindableProperty IsSuggestionListOpenProperty =
        BindableProperty.Create(nameof(IsSuggestionListOpen), typeof(bool), typeof(ProgressAutoSuggestBox), false, propertyChanged: OnIsSuggestionListOpenPropertyChanged);

    public bool UpdateTextOnSelect
    {
        get { return (bool)GetValue(UpdateTextOnSelectProperty); }
        set { SetValue(UpdateTextOnSelectProperty, value); }
    }

    public static readonly BindableProperty UpdateTextOnSelectProperty =
        BindableProperty.Create(nameof(UpdateTextOnSelect), typeof(bool), typeof(ProgressAutoSuggestBox), false);

    public IEnumerable ItemsSource
    {
        get { return (IEnumerable)GetValue(ItemsSourceProperty); }
        set { SetValue(ItemsSourceProperty, value); }
    }

    public static readonly BindableProperty ItemsSourceProperty =
        BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(ProgressAutoSuggestBox), null);

    public DataTemplate ItemTemplate
    {
        get { return (DataTemplate)GetValue(ItemTemplateProperty); }
        set { SetValue(ItemTemplateProperty, value); }
    }

    public static readonly BindableProperty ItemTemplateProperty =
        BindableProperty.Create(nameof(ItemTemplate), typeof(DataTemplate), typeof(ProgressAutoSuggestBox), null);
    #endregion

    #region Header Properties
    public object Header
    {
        get { return (object)GetValue(HeaderProperty); }
        set { SetValue(HeaderProperty, value); }
    }

    public static readonly BindableProperty HeaderProperty =
        BindableProperty.Create(nameof(Header), typeof(object), typeof(ProgressAutoSuggestBox), null, propertyChanged: OnHeaderPropertyChanged);

    public DataTemplate HeaderTemplate
    {
        get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
        set { SetValue(HeaderTemplateProperty, value); }
    }

    public static readonly BindableProperty HeaderTemplateProperty =
        BindableProperty.Create(nameof(HeaderTemplate), typeof(DataTemplate), typeof(ProgressAutoSuggestBox), null, propertyChanged: OnHeaderTemplatePropertyChanged);
    #endregion

    #region Footer Properties
    public object Footer
    {
        get { return (object)GetValue(FooterProperty); }
        set { SetValue(FooterProperty, value); }
    }

    public static readonly BindableProperty FooterProperty =
        BindableProperty.Create(nameof(Footer), typeof(object), typeof(ProgressAutoSuggestBox), null);

    public DataTemplate FooterTemplate
    {
        get { return (DataTemplate)GetValue(FooterTemplateProperty); }
        set { SetValue(FooterTemplateProperty, value); }
    }

    public static readonly BindableProperty FooterTemplateProperty =
        BindableProperty.Create(nameof(FooterTemplate), typeof(DataTemplate), typeof(ProgressAutoSuggestBox), null);
    #endregion

    public ProgressAutoSuggestBox()
    {
        this.Loaded += ProgressAutoSuggestBox_Loaded;
        this.Unloaded += ProgressAutoSuggestBox_Unloaded;
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        HeaderContainer = GetTemplateChild(nameof(HeaderContainer)) as Border;
        LocationSearchBox = GetTemplateChild(nameof(LocationSearchBox)) as SearchBar;
        PopupContainer = GetTemplateChild(nameof(PopupContainer)) as Border;
        SearchList = GetTemplateChild(nameof(SearchList)) as ListView;
        ProgressBarControl = GetTemplateChild(nameof(ProgressBarControl)) as IndeterminateProgressBar;
    }

    private void ProgressAutoSuggestBox_Loaded(object sender, EventArgs e)
    {
        LocationSearchBox.SearchButtonPressed += LocationSearchBox_SearchButtonPressed;
        LocationSearchBox.TextChanged += LocationSearchBox_TextChanged;
        LocationSearchBox.SizeChanged += LocationSearchBox_SizeChanged;
        SearchList.ItemSelected += SearchList_ItemSelected;

        if (Popover.GetAttachedPopover(LocationSearchBox) is Popover popover)
        {
            popover.Opened += PopoverExt_Opened;
            popover.Closed += PopoverExt_Closed;
        }
    }

    private void ProgressAutoSuggestBox_Unloaded(object sender, EventArgs e)
    {
        if (Popover.GetAttachedPopover(LocationSearchBox) is Popover popover)
        {
            popover.Opened -= PopoverExt_Opened;
            popover.Closed -= PopoverExt_Closed;
        }

        LocationSearchBox.SearchButtonPressed += LocationSearchBox_SearchButtonPressed;
        LocationSearchBox.TextChanged += LocationSearchBox_TextChanged;
        LocationSearchBox.SizeChanged += LocationSearchBox_SizeChanged;
    }

    private static void OnTextPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is ProgressAutoSuggestBox suggestBox)
        {
            suggestBox.suppressTextChangedEvent = true; //prevent loop of events raising, as setting this property will make it back into the native control
            suggestBox.Text = (string)newValue;
            suggestBox.suppressTextChangedEvent = false;
            suggestBox.TextChanged?.Invoke(suggestBox, new AutoSuggestBoxTextChangedEventArgs()
            {
                NewText = newValue?.ToString(),
                OldText = oldValue?.ToString()
            });
        }
    }

    private void LocationSearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (!suppressTextChangedEvent)
        {
            TextChanged?.Invoke(this, new AutoSuggestBoxTextChangedEventArgs()
            {
                NewText = e.NewTextValue,
                OldText = e.OldTextValue
            });
        }

        if (!IsSuggestionListOpen && ItemsSource?.GetEnumerator()?.MoveNext() == true)
        {
            IsSuggestionListOpen = true;
        }
    }

    private void LocationSearchBox_SearchButtonPressed(object sender, EventArgs e)
    {
        QuerySubmitted?.Invoke(this, new AutoSuggestBoxQuerySubmittedEventArgs()
        {
            ChosenSuggestion = SearchList?.SelectedItem,
            QueryText = LocationSearchBox?.Text
        });
    }

    private void SearchList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        SuggestionChosen?.Invoke(this, new AutoSuggestBoxSuggestionChosenEventArgs()
        {
            SelectedItem = e.SelectedItem
        });
    }
    private static void OnHeaderPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is ProgressAutoSuggestBox suggestBox)
        {
            suggestBox.OnHeaderChanged(newValue, suggestBox.HeaderTemplate, false);
        }
    }

    private static void OnHeaderTemplatePropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is ProgressAutoSuggestBox suggestBox)
        {
            suggestBox.OnHeaderChanged(suggestBox.Header, (DataTemplate)newValue, true);
        }
    }

    private void OnHeaderChanged(object dataObject, DataTemplate template, bool templateChanged)
    {
        if (dataObject == null)
        {
            if (!templateChanged)
            {
                HeaderContainer.Content = null;
            }

            return;
        }

        if (template == null)
        {
            var view = dataObject as View;
            if (view == null)
                view = new Label { Text = dataObject.ToString() };

            HeaderContainer.Content = view;
        }
        else if (HeaderContainer.Content == null || templateChanged)
        {
            HeaderContainer.Content = template.CreateContent() as View;
            if (HeaderContainer.Content != null)
            {
                HeaderContainer.Content.BindingContext = dataObject;
            }
        }
        else
        {
            if (HeaderContainer.Content != null)
            {
                HeaderContainer.Content.BindingContext = dataObject;
            }
        }
    }

    private static void OnIsSuggestionListOpenPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is ProgressAutoSuggestBox suggestBox)
        {
            var isVisible = (bool)newValue;

            var popover = Popover.GetAttachedPopover(suggestBox.LocationSearchBox);
            if (popover != null)
            {
                try
                {
                    if (isVisible)
                    {

                        var window = suggestBox.LocationSearchBox.Window ?? suggestBox.LocationSearchBox.GetVisualElementWindow();

                        if (window != null)
                        {
                            var suggestedHeight = suggestBox.LocationSearchBox.GetBestPopupHeight(window);
#if IOS || MACCATALYST
                            suggestBox.PopupContainer.HeightRequest = suggestedHeight;
#else
                            suggestBox.PopupContainer.MaximumHeightRequest = suggestedHeight;
#endif
                        }

                        suggestBox.LocationSearchBox.ShowAttachedPopover();
                    }
                    else
                    {
                        suggestBox.LocationSearchBox.HideAttachedPopover();
                    }
                }
                catch { }
            }
        }
    }

    private void LocationSearchBox_SizeChanged(object sender, EventArgs e)
    {
        PopupContainer.Width(LocationSearchBox.Width);
    }

    private void PopoverExt_Opened(object sender, EventArgs e)
    {
        if (!IsSuggestionListOpen)
        {
            IsSuggestionListOpen = true;
        }
    }

    private void PopoverExt_Closed(object sender, EventArgs e)
    {
        if (IsSuggestionListOpen)
        {
            IsSuggestionListOpen = false;
        }
    }
}

internal static class ViewExtensions
{
    public static double GetBestPopupHeight(this VisualElement element, IWindow window)
    {
        var coords = element.GetNativeCoordinates();

        var distToTop = (coords.Y + element.Height) - window.Y;
        var distToBottom = window.Height - (coords.Y + element.Height);

        return Math.Min(distToTop, distToBottom);
    }

    /// <summary>
    /// Gets the coordinate of the top-left point of the view
    /// </summary>
    /// <param name="element">The element to locate</param>
    /// <returns></returns>
    public static Point GetNativeCoordinates(this VisualElement element)
    {
        var platformView = Microsoft.Maui.Platform.ElementExtensions.ToPlatform(element, element.Handler.MauiContext);
#if IOS || MACCATALYST
        var rect = platformView.Superview.ConvertPointFromView(platformView.Frame.Location, null);
        return new Point(Math.Abs(rect.X), Math.Abs(rect.Y));
#elif ANDROID
        var location = new int[2];
        var density = platformView.Context.Resources.DisplayMetrics.Density;
        platformView.GetLocationInWindow(location);
        return new Point(location[0] / density, location[1] / density);
#elif WINDOWS
        var xamlWindowContent = Microsoft.UI.Xaml.Window.Current?.Content;

        if (xamlWindowContent != null)
        {
            var visual = platformView.TransformToVisual(xamlWindowContent);
            var point = visual.TransformPoint(new Windows.Foundation.Point(0, 0));
            return new Point(point.X, point.Y);
        }
        else if (platformView.XamlRoot?.Content != null)
        {
            var rootView = platformView.XamlRoot.Content;
            var visual = platformView.TransformToVisual(rootView);
            var point = visual.TransformPoint(new Windows.Foundation.Point(0, 0));
            return new Point(point.X, point.Y);
        }
        else
        {
            var screenCoords = element.GetScreenCoords();
            return new Point(screenCoords.X, screenCoords.Y);
        }
#else
        return new Point();
#endif
    }

    /// <summary>
    /// A view's default X- and Y-coordinates are LOCAL with respect to the boundaries of its parent,
    /// and NOT with respect to the screen. This method calculates the SCREEN coordinates of a view.
    /// The coordinates returned refer to the top left corner of the view.
    /// </summary>
    public static Point GetScreenCoords(this VisualElement view)
    {
        var result = new Point(view.X, view.Y);
        while (view.Parent is VisualElement parent)
        {
            result = result.Offset(parent.X, parent.Y);
            view = parent;
        }
        return result;
    }
}