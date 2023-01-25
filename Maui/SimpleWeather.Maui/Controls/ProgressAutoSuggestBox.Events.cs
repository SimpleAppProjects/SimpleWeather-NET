namespace SimpleWeather.Maui.Controls
{
    public sealed class AutoSuggestBoxQuerySubmittedEventArgs
    {
        public object ChosenSuggestion { get; init; }

        public string QueryText { get; init; }

        public AutoSuggestBoxQuerySubmittedEventArgs() : base()
        {
        }

        internal AutoSuggestBoxQuerySubmittedEventArgs(object chosenSuggestion, string queryText)
        {
            ChosenSuggestion = chosenSuggestion;
            QueryText = queryText;
        }
    }

    public sealed class AutoSuggestBoxSuggestionChosenEventArgs
    {
        public object SelectedItem { get; init; }

        public AutoSuggestBoxSuggestionChosenEventArgs() : base()
        {
        }

        internal AutoSuggestBoxSuggestionChosenEventArgs(object selectedItem)
        {
            SelectedItem = selectedItem;
        }
    }


    public sealed class AutoSuggestBoxTextChangedEventArgs : BindableObject
    {
        public string NewText { get; init; }
        public string OldText { get; init; }
    }

    public enum AutoSuggestionBoxTextChangeReason
    {
        UserInput,
        ProgrammaticChange,
        SuggestionChosen,
    }

    /*
    public sealed class AutoSuggestBoxTextChangedEventArgs : BindableObject
    {
        private ProgressAutoSuggestBox _owner;
        private string _originalText;


        public AutoSuggestionBoxTextChangeReason Reason
        {
            get { return (AutoSuggestionBoxTextChangeReason)GetValue(ReasonProperty); }
            set { SetValue(ReasonProperty, value); }
        }

        public static readonly BindableProperty ReasonProperty =
            BindableProperty.Create(nameof(Reason), typeof(AutoSuggestionBoxTextChangeReason), typeof(AutoSuggestBoxTextChangedEventArgs), default);

        internal ProgressAutoSuggestBox Owner
        {
            get => _owner;
            set
            {
                _owner = value;
                _originalText = _owner.Text;
            }
        }

        public AutoSuggestBoxTextChangedEventArgs() : base()
        {
        }

        public bool CheckCurrent() => string.Equals(_originalText, _owner.Text);
    }

    public enum AutoSuggestionBoxTextChangeReason
    {
        UserInput,
        ProgrammaticChange,
        SuggestionChosen,
    }
    */
}
