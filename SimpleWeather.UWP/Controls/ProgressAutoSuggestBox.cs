using SimpleWeather.UWP.Helpers;
using SimpleWeather.WeatherData;
using System;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using muxc = Microsoft.UI.Xaml.Controls;
using mtuconv = Microsoft.Toolkit.Uwp.UI.Converters;

namespace SimpleWeather.UWP.Controls
{
    [TemplatePart(Name = nameof(SuggestBox), Type = typeof(AutoSuggestBox))]
    public sealed partial class ProgressAutoSuggestBox : Control
    {
        private AutoSuggestBox SuggestBox;
        private ListView SuggestionsList;

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

        public IconElement QueryIcon
        {
            get => (IconElement)GetValue(QueryIconProperty);
            set => SetValue(QueryIconProperty, value);
        }

        // Using a DependencyProperty as the backing store for QueryIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty QueryIconProperty =
            DependencyProperty.Register(nameof(QueryIcon), typeof(IconElement), typeof(ProgressAutoSuggestBox), new PropertyMetadata(new SymbolIcon(Symbol.Find)));

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        // Using a DependencyProperty as the backing store for ItemTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(ProgressAutoSuggestBox), new PropertyMetadata(null));
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
            }
        }

        private void SuggestBox_Loaded(object sender, RoutedEventArgs e)
        {
            BindSuggestBox(true);
        }

        private void SuggestBox_Unloaded(object sender, RoutedEventArgs e)
        {
            BindSuggestBox(false);
        }

        private void BindSuggestBox(bool bind)
        {
            if (bind)
            {
                SuggestBox.SuggestionChosen += SuggestBox_SuggestionChosen;
                SuggestBox.TextChanged += SuggestBox_TextChanged;
                SuggestBox.QuerySubmitted += SuggestBox_QuerySubmitted;

                SuggestionsList = VisualTreeHelperExtensions.FindChild<ListView>(SuggestBox, nameof(SuggestionsList));
                if (SuggestionsList != null)
                {
                    SuggestionsList.SetBinding(ListViewBase.FooterProperty, new Binding()
                    {
                        Mode = BindingMode.OneWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.Default,
                        Source = Footer
                    });
                    SuggestionsList.SetBinding(ListViewBase.FooterTemplateProperty, new Binding()
                    {
                        Mode = BindingMode.OneWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.Default,
                        Source = FooterTemplate
                    });
                }
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
    }
}