using CommunityToolkit.Maui.Markup;
using SimpleWeather.Maui.MaterialIcons;

namespace SimpleWeather.Maui.Radar;

public partial class RadarToolbar : ContentView
{
    public bool IsPlaying
    {
        get => (bool)GetValue(IsPlayingProperty);
        set => SetValue(IsPlayingProperty, value);
    }

    public static readonly BindableProperty IsPlayingProperty =
        BindableProperty.Create(nameof(IsPlaying), typeof(bool), typeof(RadarToolbar), false);

    public RadarToolbar()
    {
        this.InitializeComponent();
        PlayIcon.Bind(
            MaterialIcon.SymbolProperty, static src => src.IsPlaying, mode: BindingMode.OneWay, source: this,
            convert: (isPlaying) => isPlaying ? MaterialSymbol.Pause : MaterialSymbol.Play,
            targetNullValue: MaterialSymbol.Play,
            fallbackValue: MaterialSymbol.Play
        );
    }

    private void PlayButton_Clicked(object sender, EventArgs e)
    {
        var button = sender as ImageButton;
        IsPlaying = !IsPlaying;

        if (IsPlaying)
        {
            OnPlayAnimation?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            OnPauseAnimation?.Invoke(this, EventArgs.Empty);
        }
    }

    public View? MapContainerChild
    {
        get { return MapContainer.Content; }
        set { MapContainer.Content = value; }
    }

    public bool IsToolbarVisible
    {
        get { return AnimationToolbar.IsVisible; }
        set { AnimationToolbar.IsVisible = value; }
    }

    public event EventHandler OnPlayAnimation;
    public event EventHandler OnPauseAnimation;

    public event EventHandler<ValueChangedEventArgs> OnSliderValueChanged;

    public void UpdateSeekbarRange(int minimumPosition, int maxPosition)
    {
        AnimationSlider.Maximum = Math.Max(maxPosition, 1);
        AnimationSlider.Minimum = minimumPosition;
    }

    public void UpdateTimestamp(int position, long timestamp)
    {
        AnimationSlider.Value = position;

        var dateTime = DateTimeOffset.FromUnixTimeSeconds(timestamp).ToLocalTime();
        TimestampBlock.Text = string.Format("{0} {1}", dateTime.ToString("ddd"), dateTime.ToString("t"));
    }

    public void UpdateTimestamp(int position, DateTime dateTime)
    {
        AnimationSlider.Value = position;
        TimestampBlock.Text = string.Format("{0} {1}", dateTime.ToString("ddd"), dateTime.ToString("t"));
    }

    private void AnimationSlider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        OnSliderValueChanged?.Invoke(sender, e);
    }
}