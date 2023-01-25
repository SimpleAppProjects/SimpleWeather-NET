namespace SimpleWeather.Maui.Controls
{
    public class IndeterminateProgressBar : epj.ProgressBar.Maui.ProgressBar
    {
        private const string LowerKey = "lower";
        private const string UpperKey = "upper";

        private readonly Animation LowerAnimation;
        private readonly Animation UpperAnimation;

        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        public static readonly BindableProperty IsActiveProperty =
            BindableProperty.Create(nameof(IsActive), typeof(bool), typeof(IndeterminateProgressBar), false, propertyChanged: OnIsActiveChanged);

        public IndeterminateProgressBar()
        {
            this.UseRange = true;
            LowerAnimation = new Animation(v => LowerRangeValue = (float)v, -0.4, 1.0);
            UpperAnimation = new Animation(v => UpperRangeValue = (float)v, 0.0, 1.4);

            this.Loaded += IndeterminateProgressBar_Loaded;
            this.Unloaded += IndeterminateProgressBar_Unloaded;
        }

        private void IndeterminateProgressBar_Loaded(object sender, EventArgs e)
        {
            this.IsVisible = IsActive;
            if (IsActive)
            {
                StartAnimation();
            }
        }

        private void IndeterminateProgressBar_Unloaded(object sender, EventArgs e)
        {
            StopAnimation();
        }

        private void StartAnimation()
        {
            LowerAnimation.Commit(this, LowerKey, length: 1000, easing: Easing.CubicInOut, repeat: () => true);
            UpperAnimation.Commit(this, UpperKey, length: 1000, easing: Easing.CubicInOut, repeat: () => true);
        }

        private void StopAnimation()
        {
            this.AbortAnimation(LowerKey);
            this.AbortAnimation(UpperKey);
        }

        private static void OnIsActiveChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is IndeterminateProgressBar progressBar)
            {
                var isActive = (bool)newValue;
                if (isActive)
                {
                    progressBar.IsVisible = true;
                    progressBar.StartAnimation();
                }
                else
                {
                    progressBar.StopAnimation();
                    progressBar.IsVisible = false;
                }
            }
        }
    }
}
