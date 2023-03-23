using System;
using Microsoft.Maui.Controls;
using SimpleWeather.Utils;

namespace SimpleWeather.Maui.Controls
{
    public class HyperlinkSpan : Span
    {
        public string NavigateUri
        {
            get { return (string)GetValue(NavigateUriProperty); }
            set { SetValue(NavigateUriProperty, value); }
        }

        public static readonly BindableProperty NavigateUriProperty =
            BindableProperty.Create(nameof(NavigateUri), typeof(string), typeof(HyperlinkSpan), null);

        public HyperlinkSpan()
        {
            GestureRecognizers.Add(new TapGestureRecognizer
            {
                // Launcher.OpenAsync is provided by Essentials.
                Command = new Command(() =>
                {
                    NavigateUri?.Let(async url => await Launcher.OpenAsync(url));
                })
            });
        }
    }
}

