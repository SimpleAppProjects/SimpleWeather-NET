using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SimpleWeather.Utils;

namespace SimpleWeather.Maui.Preferences;

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

    public ICommand OnTappedCommand { get; } = new RelayCommand<Feature>(feature =>
    {
        feature?.IsEnabled = !feature.IsEnabled;
    });
}

public partial class FeatureGroup : List<Feature>
{
    public bool CanReorder { get; set; } = true;

    public FeatureGroup() : base()
    {
    }

    public FeatureGroup(IEnumerable<Feature> features, bool canReorder = true) : base(features)
    {
        CanReorder = canReorder;

        features?.ForEach(f => { f.CanReorder = canReorder; });
    }
}