using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using SimpleWeather.NET.Utils;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.NET.Radar
{
    public sealed partial class RadarToolbar : UserControl
    {
        public RadarToolbar()
        {
            this.InitializeComponent();
            AnimationSlider.AddHandler(UIElement.PointerPressedEvent, new PointerEventHandler(AnimationSlider_PointerPressed), true);
            AnimationSlider.AddHandler(UIElement.PointerMovedEvent, new PointerEventHandler(AnimationSlider_PointerMoved), true);
            AnimationSlider.AddHandler(UIElement.PointerReleasedEvent, new PointerEventHandler(AnimationSlider_PointerReleased), true);
        }

        public bool IsPlaying
        {
            get
            {
                return PlayButton.IsChecked ?? false;
            }
            set
            {
                PlayButton.IsChecked = value;
            }
        }

        private void PlayButton_Checked(object sender, RoutedEventArgs e)
        {
            var button = sender as ToggleButton;
            button.Content = new SymbolIcon(Symbol.Pause);
            OnPlayAnimation?.Invoke(this, EventArgs.Empty);
        }

        private void PlayButton_Unchecked(object sender, RoutedEventArgs e)
        {
            var button = sender as ToggleButton;
            button.Content = new SymbolIcon(Symbol.Play);
            OnPauseAnimation?.Invoke(this, EventArgs.Empty);
        }

        public UIElement MapContainerChild
        {
            get { return MapContainer.Child; }
            set { MapContainer.Child = value; }
        }

        public Visibility ToolbarVisibility
        {
            get { return AnimationToolbar.Visibility; }
            set { AnimationToolbar.Visibility = value; }
        }

        public event EventHandler OnPlayAnimation;
        public event EventHandler OnPauseAnimation;

        public event EventHandler<SliderValueChangedArgs> OnSliderValueChanged;

        private bool isPointerPressed = false;
        private bool isUserDraggingSlider = false;

        public void UpdateSeekbarRange(int minimumPosition, int maxPosition)
        {
            AnimationSlider.Maximum = Math.Max(maxPosition, 1);
            AnimationSlider.TickFrequency = 1;
            AnimationSlider.TickPlacement = TickPlacement.Inline;
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

        private void AnimationSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            var slider = sender as Slider;
            OnSliderValueChanged?.Invoke(sender, new SliderValueChangedArgs(e.OldValue, e.NewValue, isUserDraggingSlider));
        }

        private void AnimationSlider_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (!e.IsGenerated)
            {
                isPointerPressed = true;
            }
        }

        private void AnimationSlider_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (isPointerPressed && !e.IsGenerated)
            {
                isUserDraggingSlider = true;
            }
        }

        private void AnimationSlider_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            isPointerPressed = false;
            isUserDraggingSlider = false;
        }
    }
}
