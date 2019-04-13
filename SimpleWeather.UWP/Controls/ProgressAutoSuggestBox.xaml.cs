using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.UWP.Controls
{
    public sealed partial class ProgressAutoSuggestBox : UserControl
    {
        public ProgressRing ProgressRing
        {
            get { return SuggestBox.Tag as ProgressRing; }
        }

        //
        // Summary:
        //     Raised before the text content of the editable control component is updated.
        public event TypedEventHandler<AutoSuggestBox, AutoSuggestBoxSuggestionChosenEventArgs> SuggestionChosen
        {
            add { SuggestBox.SuggestionChosen += value; }
            remove { SuggestBox.SuggestionChosen -= value; }
        }
        //
        // Summary:
        //     Raised after the text content of the editable control component is updated.
        public event TypedEventHandler<AutoSuggestBox, AutoSuggestBoxTextChangedEventArgs> TextChanged
        {
            add { SuggestBox.TextChanged += value; }
            remove { SuggestBox.TextChanged -= value; }
        }
        //
        // Summary:
        //     Occurs when the user submits a search query.
        public event TypedEventHandler<AutoSuggestBox, AutoSuggestBoxQuerySubmittedEventArgs> QuerySubmitted
        {
            add { SuggestBox.QuerySubmitted += value; }
            remove { SuggestBox.QuerySubmitted -= value; }
        }

        public ProgressAutoSuggestBox()
        {
            this.InitializeComponent();
        }
    }
}
