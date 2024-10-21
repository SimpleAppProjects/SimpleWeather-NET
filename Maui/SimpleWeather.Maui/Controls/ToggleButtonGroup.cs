using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using Microsoft.Maui.Controls;
using SceneKit;
using SimpleWeather.Utils;

namespace SimpleWeather.Maui.Controls;

[ContentProperty(nameof(ButtonItems))]
public class ToggleButtonGroup : TemplatedView
{
    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public static readonly BindableProperty CornerRadiusProperty =
        BindableProperty.Create(nameof(CornerRadius), typeof(CornerRadius), typeof(ToggleButtonGroup), new CornerRadius(0), propertyChanged: CornerRadiusPropertyChanged);

    public IList<ToggleButton> ButtonItems
    {
        get => (IList<ToggleButton>)GetValue(ButtonItemsProperty);
        set => SetValue(ButtonItemsProperty, value);
    }

    public static readonly BindableProperty ButtonItemsProperty =
        BindableProperty.Create(nameof(ButtonItems), typeof(IList<ToggleButton>), typeof(ToggleButtonGroup), null,
            propertyChanged: ButtonItemsPropertyChanged,
            defaultValueCreator: bindable =>
            {
                return new List<ToggleButton>();
            });

    public object SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public static readonly BindableProperty SelectedItemProperty =
        BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(ToggleButtonGroup), null, propertyChanged: SelectedItemPropertyChanged);

    private Layout Container;

    public event EventHandler<SelectedItemChangedEventArgs> SelectionChanged;

    public ToggleButtonGroup()
    {
        this.Loaded += ToggleButtonGroup_Loaded;
        this.Unloaded += ToggleButtonGroup_Unloaded;
    }

    private void ToggleButtonGroup_Loaded(object sender, EventArgs e)
    {
        UpdateToggleItems();
    }

    private void ToggleButtonGroup_Unloaded(object sender, EventArgs e)
    {
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        Container = GetTemplateChild(nameof(Container)) as Layout;
    }

    private void UpdateToggleItems()
    {
        if (Container != null)
        {
            Container.ForEach(v =>
            {
                if (v is ToggleButton btn)
                {
                    btn.DisableClickToggle = false;
                    btn.Clicked -= OnItemClicked;
                }
            });
            Container.Clear();

            ButtonItems?.ForEachIndexed((index, btn) =>
            {
                btn.Clicked += OnItemClicked;
                btn.DisableClickToggle = true;
                Container.Add(btn);
                UpdateButtonRadius(btn, index, ButtonItems.Count);
            });
        }
    }

    private void UpdateButtonRadius(ToggleButton button, int index, int itemCount)
    {
        if (index == 0 && itemCount == 1)
        {
            button.CornerRadius = new CornerRadius(CornerRadius.TopLeft, CornerRadius.TopRight, CornerRadius.BottomLeft, CornerRadius.BottomRight);
        }
        else if (index == 0)
        {
            button.CornerRadius = new CornerRadius(CornerRadius.TopLeft, 0, CornerRadius.BottomLeft, 0);
        }

        if (index > 0 && index != itemCount - 1)
        {
            button.CornerRadius = new CornerRadius(0);
        }

        if (index != 0 && index == itemCount - 1)
        {
            button.CornerRadius = new CornerRadius(0, CornerRadius.TopRight, 0, CornerRadius.BottomRight);
        }
    }

    private void OnItemClicked(object sender, EventArgs e)
    {
        if (SelectedItem != sender)
        {
            SelectedItem = sender;
            OnSelectionChanged(sender);
        }
    }

    private void OnSelectionChanged(object selectedItem)
    {
        var selectedIndex = -1;

        ButtonItems?.ForEachIndexed((index, btn) =>
        {
            btn.IsChecked = btn == selectedItem;
            if (btn.IsChecked) selectedIndex = index;
        });

        SelectionChanged?.Invoke(this, new SelectedItemChangedEventArgs(selectedItem, selectedIndex));
    }

    private static void SelectedItemPropertyChanged(BindableObject obj, object oldValue, object newValue)
    {
        if (obj is ToggleButtonGroup grp)
        {
            grp.OnSelectionChanged(newValue);
        }
    }

    private static void ButtonItemsPropertyChanged(BindableObject obj, object oldValue, object newValue)
    {
        if (obj is ToggleButtonGroup grp)
        {
            grp.UpdateToggleItems();
        }
    }

    private static void CornerRadiusPropertyChanged(BindableObject obj, object oldValue, object newValue)
    {
        if (obj is ToggleButtonGroup grp)
        {
            grp.ButtonItems?.ForEachIndexed((index, btn) =>
            {
                grp.UpdateButtonRadius(btn, index, grp.ButtonItems?.Count ?? 0);
            });
        }
    }
}