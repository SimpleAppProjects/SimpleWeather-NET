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
    private CancellationTokenSource reorderToken;

    internal SettingsFeaturesViewModel ViewModel { get; }

    public Settings_Features()
    {
        InitializeComponent();

        BindingContext = ViewModel = new SettingsFeaturesViewModel();
    }

    private void ReorderableCollectionView_LongPressed(object sender, Controls.LongPressEventArgs e)
    {
        var group = ViewModel.Features[e.Section];

        e.Cancel = !group.CanReorder;
    }

    private void ReorderableCollectionView_ReorderCompleted(object sender, EventArgs e)
    {
        reorderToken?.Cancel();

        var orderableFeatures = ViewModel.Features.FirstOrDefault();

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