using SimpleWeather.UWP.Helpers;
using SimpleWeather.WeatherData;
using System;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace SimpleWeather.UWP.Controls
{
    [TemplatePart(Name = nameof(SuggestBox), Type = typeof(AutoSuggestBox))]
    [TemplatePart(Name = nameof(SourceFooter), Type = typeof(TextBlock))]
    [TemplatePart(Name = nameof(SuggestBoxHeader), Type = typeof(TextBlock))]
    public sealed partial class ProgressAutoSuggestBox : Control
    {
        private AutoSuggestBox SuggestBox;
        private TextBlock SourceFooter;
        private TextBlock SuggestBoxHeader;

        public ProgressAutoSuggestBox()
        {
            DefaultStyleKey = typeof(ProgressAutoSuggestBox);
        }

        public ProgressRing ProgressRing
        {
            get { return SuggestBox?.Tag as ProgressRing; }
        }

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
            get { return (string)GetValue(PlaceholderTextProperty); }
            set { SetValue(PlaceholderTextProperty, value); }
        }

        public static readonly DependencyProperty PlaceholderTextProperty =
            DependencyProperty.Register(nameof(PlaceholderText), typeof(string), typeof(ProgressAutoSuggestBox), new PropertyMetadata(""));

        public string HeaderText
        {
            get { return (string)GetValue(HeaderTextProperty); }
            set { SetValue(HeaderTextProperty, value); }
        }

        public static readonly DependencyProperty HeaderTextProperty =
            DependencyProperty.Register(nameof(HeaderText), typeof(string), typeof(ProgressAutoSuggestBox), new PropertyMetadata(""));

        public Style TextBoxStyle
        {
            get { return (Style)GetValue(TextBoxStyleProperty); }
            set { SetValue(TextBoxStyleProperty, value); }
        }

        public static readonly DependencyProperty TextBoxStyleProperty =
            DependencyProperty.Register(nameof(TextBoxStyle), typeof(Style), typeof(ProgressAutoSuggestBox), new PropertyMetadata(null));

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            SuggestBox = GetTemplateChild(nameof(SuggestBox)) as AutoSuggestBox;
            SourceFooter = GetTemplateChild(nameof(SourceFooter)) as TextBlock;

            SuggestBox.ApplyTemplate();
            SuggestBoxHeader = VisualTreeHelperExtensions.FindChild<TextBlock>(SuggestBox, nameof(SuggestBoxHeader));

            SuggestBox.SuggestionChosen += SuggestBox_SuggestionChosen;
            SuggestBox.TextChanged += SuggestBox_TextChanged;
            SuggestBox.QuerySubmitted += SuggestBox_QuerySubmitted;

            var wm = WeatherManager.GetInstance();
            var LocationAPI = wm.LocationProvider.LocationAPI;
            var creditPrefix = App.ResLoader.GetString("Credit_Prefix");

            SourceFooter.Text = String.Format("{0} {1}",
                creditPrefix, WeatherAPI.LocationAPIs.First(LApi => LocationAPI.Equals(LApi.Value)));

            Binding txtBinding = new Binding()
            {
                Mode = BindingMode.OneWay,
                Source = HeaderText
            };
            SuggestBoxHeader?.SetBinding(TextBlock.TextProperty, txtBinding);

            Binding placeholderTxtBinding = new Binding()
            {
                Mode = BindingMode.OneWay,
                Source = PlaceholderText
            };
            SuggestBox?.SetBinding(AutoSuggestBox.PlaceholderTextProperty, placeholderTxtBinding);

            Binding txtBoxStyleBinding = new Binding()
            {
                Mode = BindingMode.TwoWay,
                Source = TextBoxStyle,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                FallbackValue = SuggestBox?.TextBoxStyle,
                TargetNullValue = SuggestBox?.TextBoxStyle
            };
            SuggestBox?.SetBinding(AutoSuggestBox.TextBoxStyleProperty, txtBoxStyleBinding);
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