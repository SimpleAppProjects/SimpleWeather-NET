using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using SimpleWeather.NET.Helpers;
using Windows.Foundation;

namespace SimpleWeather.NET.Controls
{
    [TemplatePart(Name = nameof(SuggestBox), Type = typeof(AutoSuggestBox))]
    public sealed partial class ProgressAutoSuggestBox : Control
    {
        private AutoSuggestBox SuggestBox;
        private ListView SuggestionsList;
        private Border SuggestionsContainer;
        private Popup SuggestionsPopup;

        public ProgressAutoSuggestBox()
        {
            DefaultStyleKey = typeof(ProgressAutoSuggestBox);
        }

        #region Progress Controls
        public bool IsLoading
        {
            get => ProgressVisibility == Visibility.Visible;
            set => ProgressVisibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        private Visibility ProgressVisibility
        {
            get => (Visibility)GetValue(ProgressVisibilityProperty);
            set => SetValue(ProgressVisibilityProperty, value);
        }

        // Using a DependencyProperty as the backing store for ProgressVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProgressVisibilityProperty =
            DependencyProperty.Register(nameof(ProgressVisibility), typeof(Visibility), typeof(ProgressAutoSuggestBox), new PropertyMetadata(Visibility.Collapsed));
        #endregion

        #region AutoSuggestBox Properties
        //
        // Summary:
        //     Raised before the text content of the editable control component is updated.
        public event TypedEventHandler<AutoSuggestBox, AutoSuggestBoxSuggestionChosenEventArgs> SuggestionChosen;

        //
        // Summary:
        //     Raised after the text content of the editable control component is updated.
        public event TypedEventHandler<AutoSuggestBox, AutoSuggestBoxTextChangedEventArgs> TextChanged;

        //
        // Summary:
        //     Occurs when the user submits a search query.
        public event TypedEventHandler<AutoSuggestBox, AutoSuggestBoxQuerySubmittedEventArgs> QuerySubmitted;

        public string PlaceholderText
        {
            get => (string)GetValue(PlaceholderTextProperty);
            set => SetValue(PlaceholderTextProperty, value);
        }

        public static readonly DependencyProperty PlaceholderTextProperty =
            DependencyProperty.Register(nameof(PlaceholderText), typeof(string), typeof(ProgressAutoSuggestBox), new PropertyMetadata(""));

        public Style TextBoxStyle
        {
            get => (Style)GetValue(TextBoxStyleProperty);
            set => SetValue(TextBoxStyleProperty, value);
        }

        public static readonly DependencyProperty TextBoxStyleProperty =
            DependencyProperty.Register(nameof(TextBoxStyle), typeof(Style), typeof(ProgressAutoSuggestBox), new PropertyMetadata(null));

        public IconElement QueryIcon { get { return _queryIcon; } set { _queryIcon = value; UpdateQueryIcon(); } }
        private IconElement _queryIcon;
        private readonly IconElement _defaultIcon = new SymbolIcon(Symbol.Find);

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        // Using a DependencyProperty as the backing store for ItemTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(ProgressAutoSuggestBox), new PropertyMetadata(null));

        public bool IsSuggestionListOpen
        {
            get { return (bool)GetValue(IsSuggestionListOpenProperty); }
            set { SetValue(IsSuggestionListOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSuggestionListOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSuggestionListOpenProperty =
            DependencyProperty.Register("IsSuggestionListOpen", typeof(bool), typeof(ProgressAutoSuggestBox), new PropertyMetadata(false));

        public object ItemsSource
        {
            get { return (object)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(object), typeof(ProgressAutoSuggestBox), new PropertyMetadata(null));
        #endregion

        #region Header Properties
        public object Header
        {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(nameof(Header), typeof(string), typeof(ProgressAutoSuggestBox), new PropertyMetadata(""));

        public DataTemplate HeaderTemplate
        {
            get => (DataTemplate)GetValue(HeaderTemplateProperty);
            set => SetValue(HeaderTemplateProperty, value);
        }

        // Using a DependencyProperty as the backing store for HeaderTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(ProgressAutoSuggestBox), new PropertyMetadata(null));
        #endregion

        #region Footer Properties
        public object Footer
        {
            get => GetValue(FooterProperty);
            set => SetValue(FooterProperty, value);
        }

        // Using a DependencyProperty as the backing store for Footer.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FooterProperty =
            DependencyProperty.Register("Footer", typeof(object), typeof(ProgressAutoSuggestBox), new PropertyMetadata(null));

        public DataTemplate FooterTemplate
        {
            get => (DataTemplate)GetValue(FooterTemplateProperty);
            set => SetValue(FooterTemplateProperty, value);
        }

        // Using a DependencyProperty as the backing store for FooterTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FooterTemplateProperty =
            DependencyProperty.Register("FooterTemplate", typeof(DataTemplate), typeof(ProgressAutoSuggestBox), new PropertyMetadata(null));
        #endregion

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            SuggestBox = GetTemplateChild(nameof(SuggestBox)) as AutoSuggestBox;

            if (SuggestBox != null)
            {
                SuggestBox.ApplyTemplate();
                SuggestBox.Loaded += SuggestBox_Loaded;
                SuggestBox.Unloaded += SuggestBox_Unloaded;
                SuggestBox.SizeChanged += SuggestBox_SizeChanged;
                UpdateQueryIcon();
            }
        }

        private void SuggestBox_Loaded(object sender, RoutedEventArgs e)
        {
            BindSuggestBox(true);
            UpdateQueryIcon();
            SuggestBox_SizeChanged(SuggestBox, null);
        }

        private void SuggestBox_Unloaded(object sender, RoutedEventArgs e)
        {
            BindSuggestBox(false);
        }

        private void SuggestBox_SizeChanged(object sender, SizeChangedEventArgs args)
        {
            if (SuggestionsList != null)
            {
                var window = MainWindow.Current ?? Microsoft.UI.Xaml.Window.Current;

                if (window != null)
                {
                    var suggestedHeight = SuggestBox.GetBestPopupHeight(window);
                    SuggestionsList.MaxHeight = suggestedHeight;
                }
            }
        }

        private void BindSuggestBox(bool bind)
        {
            if (bind)
            {
                SuggestBox.SuggestionChosen += SuggestBox_SuggestionChosen;
                SuggestBox.TextChanged += SuggestBox_TextChanged;
                SuggestBox.QuerySubmitted += SuggestBox_QuerySubmitted;

                SuggestionsList = SuggestBox.FindChild<ListView>(nameof(SuggestionsList));
                SuggestionsContainer = SuggestBox.FindChild<Border>(nameof(SuggestionsContainer));
                SuggestionsPopup = SuggestBox.FindChild<Popup>(nameof(SuggestionsPopup));
                if (SuggestionsList != null)
                {
                    SuggestionsList.SetBinding(ListViewBase.FooterProperty, new Binding()
                    {
                        Mode = BindingMode.OneWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.Default,
                        Source = this,
                        Path = new PropertyPath("Footer")
                    });
                    SuggestionsList.SetBinding(ListViewBase.FooterTemplateProperty, new Binding()
                    {
                        Mode = BindingMode.OneWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.Default,
                        Source = FooterTemplate
                    });
                }

                SuggestionsContainer?.SetBinding(Border.MaxWidthProperty, new Binding()
                {
                    Mode = BindingMode.OneWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.Default,
                    Source = SuggestBox.ActualWidth
                });
            }
            else
            {
                SuggestBox.SuggestionChosen -= SuggestBox_SuggestionChosen;
                SuggestBox.TextChanged -= SuggestBox_TextChanged;
                SuggestBox.QuerySubmitted -= SuggestBox_QuerySubmitted;

                if (SuggestionsList != null)
                {
                    SuggestionsList.ClearValue(ListViewBase.FooterProperty);
                    SuggestionsList.ClearValue(ListViewBase.FooterTemplateProperty);
                    SuggestionsList = null;
                }

                SuggestionsContainer?.ClearValue(Border.MaxWidthProperty);
                SuggestionsContainer = null;
            }
        }

        private void SuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            SuggestionChosen?.Invoke(sender, args);
        }

        private void SuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            TextChanged?.Invoke(sender, args);
        }

        private void SuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            QuerySubmitted?.Invoke(sender, args);
        }

        private void UpdateQueryIcon()
        {
            if (SuggestBox != null)
            {
                if (QueryIcon == null)
                {
                    SuggestBox.QueryIcon = _defaultIcon;
                }
                else
                {
                    SuggestBox.QueryIcon = QueryIcon;
                }
            }
        }
    }
}