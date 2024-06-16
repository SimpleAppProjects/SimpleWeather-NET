using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SimpleWeather.Utils;
using UIKit;
using FeatureSettings = SimpleWeather.NET.Utils.FeatureSettings;

namespace SimpleWeather.Maui.Preferences;

public partial class Settings_Features : ContentPage
{
    public List<FeatureGroup> Features
    {
        get => (List<FeatureGroup>)GetValue(FeaturesProperty);
        set => SetValue(FeaturesProperty, value);
    }

    public static readonly BindableProperty FeaturesProperty =
        BindableProperty.Create(nameof(Features), typeof(List<FeatureGroup>), typeof(Settings_Features), new List<FeatureGroup>());

    private readonly IDictionary<string, Feature> OrderableFeaturesMap;

    private CancellationTokenSource reorderToken;

    public Settings_Features()
	{
		InitializeComponent();

        OrderableFeaturesMap = OrderableFeatures.ToDictionary(f => f.Key);

        var features = FeatureSettings.GetFeatureOrder()?.Intersect(OrderableFeaturesMap.Keys)?.Select(key =>
        {
            return OrderableFeaturesMap[key];
        });

        Features = new List<FeatureGroup>
        {
            new FeatureGroup(features ?? OrderableFeatures, true),
            new FeatureGroup(NonOrderableFeatures, false)
        };
    }

    private void OnItemTapped(object sender, TappedEventArgs e)
    {
        if (sender is BindableObject obj && obj.BindingContext is Feature feature)
        {
            feature.IsEnabled = !feature.IsEnabled;
        }
    }

    private void ResetButton_Clicked(object sender, EventArgs e)
    {
        // Reset
        Features = new List<FeatureGroup>
        {
            new FeatureGroup(OrderableFeatures, true),
            new FeatureGroup(NonOrderableFeatures, false)
        };

        FeatureSettings.SetFeatureOrder(null);
    }

    private void ReorderableCollectionView_LongPressed(object sender, Controls.LongPressEventArgs e)
    {
        var group = Features[e.Section];

        e.Cancel = !group.CanReorder;
    }

    private void ReorderableCollectionView_ReorderCompleted(object sender, EventArgs e)
    {
        reorderToken?.Cancel();

        var orderableFeatures = Features.FirstOrDefault();

        if (orderableFeatures is not null)
        {
            if (reorderToken?.TryReset() != true)
            {
                reorderToken = new CancellationTokenSource();
            }

            Dispatcher.Dispatch(async () =>
            {
                await Task.Delay(1000);

                if (!reorderToken.IsCancellationRequested)
                {
                    var keys = orderableFeatures.Select(f => f.Key);
                    FeatureSettings.SetFeatureOrder(keys);
                }
            });
        }
    }
}

public class FeatureGroup : List<Feature>
{
    public bool CanReorder { get; set; } = true;

    public FeatureGroup() : base() { }

    public FeatureGroup(IEnumerable<Feature> features, bool canReorder = true) : base(features)
    {
        CanReorder = canReorder;

        features?.ForEach(f =>
        {
            f.CanReorder = canReorder;
        });
    }
}

public class Feature : BindableObject
{
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(Feature), default);

    public string Key
    {
        get => (string)GetValue(KeyProperty);
        set => SetValue(KeyProperty, value);
    }

    public static readonly BindableProperty KeyProperty =
        BindableProperty.Create(nameof(Key), typeof(string), typeof(Feature), default);

    public bool IsEnabled
    {
        get => (bool)GetValue(IsEnabledProperty);
        set => SetValue(IsEnabledProperty, value);
    }

    public static readonly BindableProperty IsEnabledProperty =
        BindableProperty.Create(nameof(IsEnabled), typeof(bool), typeof(Feature), true);

    public bool CanReorder
    {
        get => (bool)GetValue(CanReorderProperty);
        set => SetValue(CanReorderProperty, value);
    }

    public static readonly BindableProperty CanReorderProperty =
        BindableProperty.Create(nameof(CanReorder), typeof(bool), typeof(Feature), true);
}