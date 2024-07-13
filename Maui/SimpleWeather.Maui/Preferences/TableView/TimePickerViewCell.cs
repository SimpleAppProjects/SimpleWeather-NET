using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Mvvm.Messaging;
using CoreGraphics;
using Foundation;
using MauiIcons.Core;
using MauiIcons.Cupertino;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using Microsoft.Maui.Platform;
using NetTopologySuite.Index.HPRtree;
using SimpleToolkit.SimpleShell.Controls;
using SimpleWeather.Maui.Controls;
using SimpleWeather.Utils;
using UIKit;
using TimePicker = Microsoft.Maui.Controls.TimePicker;

namespace SimpleWeather.Maui.Preferences
{
    public class TimePickerViewCell : ViewCell
	{
        public string PreferenceKey
        {
            get => (string)GetValue(PreferenceKeyProperty);
            set => SetValue(PreferenceKeyProperty, value);
        }

        public static readonly BindableProperty PreferenceKeyProperty =
            BindableProperty.Create(nameof(PreferenceKey), typeof(string), typeof(TimePickerViewCell), null);

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public static readonly BindableProperty ColorProperty =
            BindableProperty.Create(nameof(Color), typeof(Color), typeof(TimePickerViewCell), null);

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(TimePickerViewCell), default(string));

        public Color TextColor
        {
            get { return (Color)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        public static readonly BindableProperty TextColorProperty =
            BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(TimePickerViewCell), null);

        public string Detail
        {
            get { return (string)GetValue(DetailProperty); }
            set { SetValue(DetailProperty, value); }
        }

        public static readonly BindableProperty DetailProperty =
            BindableProperty.Create(nameof(Detail), typeof(string), typeof(TimePickerViewCell), default(string));

        public Color DetailColor
        {
            get { return (Color)GetValue(DetailColorProperty); }
            set { SetValue(DetailColorProperty, value); }
        }

        public static readonly BindableProperty DetailColorProperty =
            BindableProperty.Create(nameof(DetailColor), typeof(Color), typeof(TimePickerViewCell), null);

        public TimeSpan Time
        {
            get => (TimeSpan)GetValue(TimeProperty);
            set => SetValue(TimeProperty, value);
        }

        public static readonly BindableProperty TimeProperty =
            BindableProperty.Create(nameof(Time), typeof(TimeSpan), typeof(TimePickerViewCell), default);

        private TimePicker Picker;

        public TimePickerViewCell()
        {
            this.View = new Grid()
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Auto),
                },
                MinimumHeightRequest = 48,
                MaximumHeightRequest = double.PositiveInfinity,
                Padding = new Thickness(16, 4),
                Children =
                {
                    new Label()
                    {
                        FontSize = 17,
                        FontFamily = Microsoft.Maui.Font.Default.Family
                    }
                    .Bind(Label.TextProperty, nameof(Text), BindingMode.OneWay, source: this)
                    .Bind(Label.TextColorProperty, nameof(TextColor), BindingMode.OneWay, source: this)
                    .Bind(Label.IsEnabledProperty, nameof(IsEnabled), BindingMode.OneWay, source: this)
                    .Bind<Label, bool, double>(Label.OpacityProperty, nameof(IsEnabled), BindingMode.OneWay, source: this,
                        convert: (enabled) =>
                        {
                            return enabled ? 1.0d : 0.5d;
                        }
                     )
                    .CenterVertical()
                    .Column(0)
                    .Row(0),
                    new TimePicker()
                    {
                        HorizontalOptions = LayoutOptions.End,
                        FontSize = 15,
                        MinimumHeightRequest = 0,
                        MinimumWidthRequest = 0,
                        Margin = 0
                    }
                    .Bind(TimePicker.TimeProperty, nameof(Time), BindingMode.TwoWay, source: this)
                    .Bind(TimePicker.IsEnabledProperty, nameof(IsEnabled), BindingMode.OneWay, source: this)
                    .CenterVertical()
                    .Column(1)
                    .Row(0)
                    .Let(pckr => Picker = pckr)
                    .On<iOS>()
                        .SetUpdateMode(UpdateMode.WhenFinished)
                    .Element
                }
            };

            this.PropertyChanged += (s, e) =>
            {
                if (Equals(e.PropertyName, nameof(Time)))
                {
                    WeakReferenceMessenger.Default.Send(new SettingsChangedMessage(new SettingsChanged(PreferenceKey, Time)));
                }
            };
            Picker.PropertyChanged += (s, e) =>
            {
                if (Equals(e.PropertyName, nameof(Picker.IsEnabled)))
                {
                    Picker.Opacity = Picker.IsEnabled ? 1 : 0.25;
                }
            };

            this.View.TapGesture(OnTapped);
        }

        protected override void OnTapped()
        {
            if (!IsEnabled) return;

            base.OnTapped();
        }
    }
}