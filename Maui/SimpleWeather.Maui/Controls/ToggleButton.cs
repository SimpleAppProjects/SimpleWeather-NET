using System;
using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Controls.Xaml;
using SimpleToolkit.Core;
using SimpleWeather.Maui.MaterialIcons;
using SimpleWeather.Utils;

namespace SimpleWeather.Maui.Controls;

public class ToggleButton : ContentButton
{
    public bool IsChecked
    {
        get => (bool)GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }

    public static readonly BindableProperty IsCheckedProperty =
        BindableProperty.Create(nameof(IsChecked), typeof(bool), typeof(ToggleButton), false, propertyChanged: OnCheckedPropertyChanged);

    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    public static readonly BindableProperty TextColorProperty =
        BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(ToggleButton), Colors.Black);

    public Color CheckedTextColor
    {
        get => (Color)GetValue(CheckedTextColorProperty);
        set => SetValue(CheckedTextColorProperty, value);
    }

    public static readonly BindableProperty CheckedTextColorProperty =
        BindableProperty.Create(nameof(CheckedTextColor), typeof(Color), typeof(ToggleButton), Colors.Black);

    public Color IconTint
    {
        get => (Color)GetValue(IconTintProperty);
        set => SetValue(IconTintProperty, value);
    }

    public static readonly BindableProperty IconTintProperty =
        BindableProperty.Create(nameof(IconTint), typeof(Color), typeof(ToggleButton), Colors.Black);

    public Color CheckedIconTint
    {
        get => (Color)GetValue(CheckedIconTintProperty);
        set => SetValue(CheckedIconTintProperty, value);
    }

    public static readonly BindableProperty CheckedIconTintProperty =
        BindableProperty.Create(nameof(CheckedIconTint), typeof(Color), typeof(ToggleButton), Colors.Black);

    public Color CheckedBackgroundColor
    {
        get => (Color)GetValue(CheckedBackgroundColorProperty);
        set => SetValue(CheckedBackgroundColorProperty, value);
    }

    public static readonly BindableProperty CheckedBackgroundColorProperty =
        BindableProperty.Create(nameof(CheckedBackgroundColor), typeof(Color), typeof(ToggleButton), Colors.LightSkyBlue);

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(ToggleButton), string.Empty);

    public ImageSource IconSource
    {
        get => (ImageSource)GetValue(IconSourceProperty);
        set => SetValue(IconSourceProperty, value);
    }

    public static readonly BindableProperty IconSourceProperty =
        BindableProperty.Create(nameof(IconSource), typeof(ImageSource), typeof(ToggleButton), null);

    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public static readonly BindableProperty CornerRadiusProperty =
        BindableProperty.Create(nameof(CornerRadius), typeof(CornerRadius), typeof(ToggleButton), new CornerRadius(0));

    public EventHandler<CheckedChangedEventArgs> CheckedChanged;

    internal bool DisableClickToggle { get; set; } = false;

    private Icon Icon;
    private Label Label;

    public ToggleButton()
    {
        StrokeShape = new RoundRectangle()
            .Bind(RoundRectangle.CornerRadiusProperty, static src => src.CornerRadius, mode: BindingMode.OneWay, source: this);
        StrokeThickness = 0.5;
        this.AppThemeBinding(StrokeProperty, new SolidColorBrush(Color.FromArgb("#73777F")), new SolidColorBrush(Color.FromArgb("#8D9199")));

        Padding = new Thickness(24, 0);
        Content = new HorizontalStackLayout()
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            MinimumWidthRequest = 44,
            MinimumHeightRequest = 44,
            Spacing = 4,
            Padding = 4,
            Children =
            {
                new Icon()
                    .Margins(4, 4, 8, 4)
                    .Bind(Icon.SourceProperty, static src => src.IconSource, mode: BindingMode.OneWay, source: this)
                    .Bind(Icon.TintColorProperty, static src => src.IconTint, mode: BindingMode.OneWay, source: this)
                    .Apply(it => Icon = it),
                new Label()
                    .Padding(4)
                    .Bind(Label.TextProperty, static src => src.Text, mode: BindingMode.OneWay, source: this)
                    .Bind(Label.TextColorProperty, static src => src.TextColor, mode: BindingMode.OneWay, source: this)
                    .Apply(it => Label = it),
            }
        };

        VisualStateManager.SetVisualStateGroups(Icon, new VisualStateGroupList()
        {
            new VisualStateGroup()
            {
                States =
                {
                    new VisualState()
                    {
                        Name = "Checked",
                        Setters =
                        {
                            new Setter()
                            {
                                Property = Icon.TintColorProperty,
                                Value = BindingBase.Create(static (ToggleButton src) => src.CheckedIconTint, mode: BindingMode.OneWay, source: this),
                            }
                        }
                    },
                    new VisualState()
                    {
                        Name = "Unchecked",
                        Setters =
                        {
                            new Setter()
                            {
                                Property = Icon.TintColorProperty,
                                Value = BindingBase.Create(static (ToggleButton src) => src.IconTint, mode: BindingMode.OneWay, source: this),
                            }
                        }
                    },
                }
            }
        });

        VisualStateManager.SetVisualStateGroups(Label, new VisualStateGroupList()
        {
            new VisualStateGroup()
            {
                States =
                {
                    new VisualState()
                    {
                        Name = "Checked",
                        Setters =
                        {
                            new Setter()
                            {
                                Property = Label.TextColorProperty,
                                Value = BindingBase.Create(static (ToggleButton src) => src.CheckedTextColor, mode: BindingMode.OneWay, source: this),
                            }
                        }
                    },
                    new VisualState()
                    {
                        Name = "Unchecked",
                        Setters =
                        {
                            new Setter()
                            {
                                Property = Label.TextColorProperty,
                                Value = BindingBase.Create(static (ToggleButton src) => src.TextColor, mode: BindingMode.OneWay, source: this),
                            }
                        }
                    },
                }
            }
        });

        VisualStateManager.SetVisualStateGroups(this, new VisualStateGroupList()
        {
            new VisualStateGroup()
            {
                States =
                {
                    new VisualState()
                    {
                        Name = "Checked",
                        Setters =
                        {
                            new Setter()
                            {
                                Property = VisualElement.BackgroundColorProperty,
                                Value = BindingBase.Create(static (ToggleButton src) => src.CheckedBackgroundColor, mode: BindingMode.OneWay, source: this),
                            }
                        }
                    },
                    new VisualState()
                    {
                        Name = "Unchecked",
                        Setters =
                        {
                            new Setter()
                            {
                                Property = VisualElement.BackgroundColorProperty,
                                Value = BindingBase.Create(static (ToggleButton src) => src.BackgroundColor, BindingMode.OneWay, source: this),
                            }
                        }
                    },
                }
            }
        });
    }

    private static void OnCheckedPropertyChanged(BindableObject obj, object oldValue, object newValue)
    {
        if (obj is ToggleButton btn)
        {
            btn.OnCheckedChanged((bool)newValue);
        }
    }

    public override void OnClicked()
    {
        base.OnClicked();
        if (!DisableClickToggle)
        {
            IsChecked = !IsChecked;
        }
    }

    protected virtual void OnCheckedChanged(bool isChecked)
    {
        CheckedChanged?.Invoke(this, new CheckedChangedEventArgs(isChecked));

        var state = isChecked ? "Checked" : "Unchecked";

        VisualStateManager.GoToState(Icon, state);
        VisualStateManager.GoToState(Label, state);
        VisualStateManager.GoToState(this, state);
    }
}