using Microsoft.Maui.Controls.Shapes;
using SimpleWeather.Maui.Helpers;
using SimpleWeather.Maui.MaterialIcons;

namespace SimpleWeather.Maui.Stepper;

public partial class BottomStepperNavigationBar : TemplatedView
{
    private Grid RootGrid;
    private Button BackBtn;
    private HorizontalStackLayout IndicatorBox;
    private Button NextBtn;

    public Color ForegroundColor
    {
        get { return (Color)GetValue(ForegroundColorProperty); }
        set { SetValue(ForegroundColorProperty, value); }
    }

    public static readonly BindableProperty ForegroundColorProperty =
        BindableProperty.Create("ForegroundColor", typeof(Color), typeof(BottomStepperNavigationBar), Colors.White);

    public string StartButtonText
    {
        get { return (string)GetValue(StartButtonTextProperty); }
        set { SetValue(StartButtonTextProperty, value); }
    }

    public static readonly BindableProperty StartButtonTextProperty =
        BindableProperty.Create("StartButtonText", typeof(string), typeof(BottomStepperNavigationBar), string.Empty);

    public string BackButtonText
    {
        get { return (string)GetValue(BackButtonTextProperty); }
        set { SetValue(BackButtonTextProperty, value); }
    }

    public static readonly BindableProperty BackButtonTextProperty =
        BindableProperty.Create("BackButtonText", typeof(string), typeof(BottomStepperNavigationBar), string.Empty);

    public string NextButtonText
    {
        get { return (string)GetValue(NextButtonTextProperty); }
        set { SetValue(NextButtonTextProperty, value); }
    }

    public static readonly BindableProperty NextButtonTextProperty =
        BindableProperty.Create("NextButtonText", typeof(string), typeof(BottomStepperNavigationBar), string.Empty);

    public string CompleteButtonText
    {
        get { return (string)GetValue(CompleteButtonTextProperty); }
        set { SetValue(CompleteButtonTextProperty, value); }
    }

    public static readonly BindableProperty CompleteButtonTextProperty =
        BindableProperty.Create("CompleteButtonText", typeof(string), typeof(BottomStepperNavigationBar), string.Empty);

    public ImageSource BackButtonIcon
    {
        get { return (ImageSource)GetValue(BackButtonIconProperty); }
        set { SetValue(BackButtonIconProperty, value); }
    }

    public static readonly BindableProperty BackButtonIconProperty =
        BindableProperty.Create("BackButtonIcon", typeof(ImageSource), typeof(BottomStepperNavigationBar), new MaterialIcon(MaterialSymbol.ChevronLeft));

    public ImageSource NextButtonIcon
    {
        get { return (ImageSource)GetValue(NextButtonIconProperty); }
        set { SetValue(NextButtonIconProperty, value); }
    }

    public static readonly BindableProperty NextButtonIconProperty =
        BindableProperty.Create("NextButtonIcon", typeof(ImageSource), typeof(BottomStepperNavigationBar), new MaterialIcon(MaterialSymbol.ChevronRight));

    public ImageSource CompleteButtonIcon
    {
        get { return (ImageSource)GetValue(CompleteButtonIconProperty); }
        set { SetValue(CompleteButtonIconProperty, value); }
    }

    public static readonly BindableProperty CompleteButtonIconProperty =
        BindableProperty.Create("CompleteButtonIcon", typeof(ImageSource), typeof(BottomStepperNavigationBar), new MaterialIcon(MaterialSymbol.Done));

    public int SelectedIndex
    {
        get => (int)GetValue(SelectedIndexProperty);
        set => SetValue(SelectedIndexProperty, value);
    }

    public static readonly BindableProperty SelectedIndexProperty =
        BindableProperty.Create(nameof(SelectedIndex), typeof(int), typeof(BottomStepperNavigationBar), 0,
            validateValue: OnValidateSelectedIndex, propertyChanged: OnSelectedIndexChanged);

    private static bool OnValidateSelectedIndex(BindableObject bindable, object newValue)
    {
        if (newValue is int value && value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(SelectedIndex), "value must be >= 0");
        }

        return true;
    }

    private static void OnSelectedIndexChanged(BindableObject obj, object oldValue, object newValue)
    {
        if (obj is BottomStepperNavigationBar bar)
        {
            bar.SetSelectedItem((int)newValue, true);
        }
    }

    public int ItemCount
    {
        get => (int)GetValue(ItemCountProperty);
        set => SetValue(ItemCountProperty, value);
    }

    public static readonly BindableProperty ItemCountProperty =
        BindableProperty.Create(nameof(ItemCount), typeof(int), typeof(BottomStepperNavigationBar), 0,
            validateValue: OnValidateItemCount, propertyChanged: OnItemCountChanged);

    private static bool OnValidateItemCount(BindableObject bindable, object newValue)
    {
        if (newValue is int value && value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(ItemCount), "value must be >= 0");
        }

        return true;
    }

    private static void OnItemCountChanged(BindableObject obj, object oldValue, object newValue)
    {
        if (oldValue != newValue && obj is BottomStepperNavigationBar bar)
        {
            bar.SetItemCount((int)newValue, true);
        }
    }

    public event EventHandler OnBackButtonClicked;
    public event EventHandler OnNextButtonClicked;

    public BottomStepperNavigationBar()
    {
        this.Loaded += BottomStepperNavigationBar_Loaded;
        this.Unloaded += BottomStepperNavigationBar_Unloaded;
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        this.RootGrid = this.GetTemplateChild(nameof(RootGrid)) as Grid;
        this.BackBtn = this.GetTemplateChild(nameof(BackBtn)) as Button;
        this.IndicatorBox = this.GetTemplateChild(nameof(IndicatorBox)) as HorizontalStackLayout;
        this.NextBtn = this.GetTemplateChild(nameof(NextBtn)) as Button;
    }

    private void BindButtons(bool bind)
    {
        if (BackBtn != null)
        {
            if (bind)
                BackBtn.Clicked += BackBtn_Click;
            else
                BackBtn.Clicked -= BackBtn_Click;
        }
        if (NextBtn != null)
        {
            if (bind)
                NextBtn.Clicked += NextBtn_Click;
            else
                NextBtn.Clicked -= NextBtn_Click;
        }
    }

    private void NextBtn_Click(object sender, EventArgs e)
    {
        OnNextButtonClicked?.Invoke(this, e);
    }

    private void BackBtn_Click(object sender, EventArgs e)
    {
        OnBackButtonClicked?.Invoke(this, e);
    }

    private void BindIndicator(bool bind)
    {
        if (IndicatorBox != null)
        {
            if (bind)
                IndicatorBox.Loaded += IndicatorBox_Loaded;
            else
                IndicatorBox.Loaded -= IndicatorBox_Loaded;
        }
    }

    private void IndicatorBox_Loaded(object sender, EventArgs e)
    {
        SetItemCount(ItemCount);
        SetSelectedItem(SelectedIndex);
    }

    private void BottomStepperNavigationBar_Loaded(object sender, EventArgs e)
    {
        BindButtons(true);
        BindIndicator(true);

        SetItemCount(ItemCount);
        SetSelectedItem(SelectedIndex);
    }

    private void BottomStepperNavigationBar_Unloaded(object sender, EventArgs e)
    {
        BindButtons(false);
        BindIndicator(false);

        // Clear bindings
        if (NextBtn != null)
        {
            NextBtn.ClearValue(Button.ImageSourceProperty);
            NextBtn.ClearValue(Button.TextProperty);
        }
    }

    private void SetItemCount(int count, bool useTransitions = false)
    {
        if (IndicatorBox != null)
        {
            IndicatorBox.Children.Clear();
            for (int i = 0; i < count; i++)
            {
                var dot = new Ellipse()
                {
                    WidthRequest = 8,
                    HeightRequest = 8,
                    Fill = new SolidColorBrush(Colors.White),
                    Stroke = new SolidColorBrush(Colors.White),
                    Opacity = 1,
                    Margin = new Thickness(4)
                };
                IndicatorBox.Children.Add(dot);
            }
        }
        SetSelectedItem(0, useTransitions);
    }

    private void SetSelectedItem(int idx, bool useTransitions = false)
    {
        if (IndicatorBox != null)
        {
            for (int i = 0; i < ItemCount; i++)
            {
                var dot = IndicatorBox.Children[i] as VisualElement;
                if (i == SelectedIndex)
                {
                    dot.Opacity = 1;
                }
                else
                {
                    dot.Opacity = 0.25;
                }
            }
        }
        UpdateButtonsState();
    }

    private void UpdateButtonsState()
    {
        if (SelectedIndex == 0)
        {
            ShowBackButton(false);
            ShowNextButton(true);

            if (NextBtn != null)
            {
                NextBtn.SetOneWayBinding(Button.ImageSourceProperty, NextButtonIcon);
                NextBtn.SetOneWayBinding(Button.TextProperty, StartButtonText);
            }
        }
        else if (SelectedIndex == (ItemCount - 1))
        {
            ShowBackButton(true);
            ShowNextButton(true);

            if (NextBtn != null)
            {
                NextBtn.SetOneWayBinding(Button.ImageSourceProperty, CompleteButtonIcon);
                NextBtn.SetOneWayBinding(Button.TextProperty, CompleteButtonText);
            }
        }
        else
        {
            ShowBackButton(true);
            ShowNextButton(true);

            if (NextBtn != null)
            {
                NextBtn.SetOneWayBinding(Button.ImageSourceProperty, NextButtonIcon);
                NextBtn.SetOneWayBinding(Button.TextProperty, NextButtonText);
            }
        }
    }

    public void ShowBackButton(bool show)
    {
        if (BackBtn != null)
        {
            BackBtn.SetVisibility(show ? Visibility.Visible : Visibility.Hidden);
        }
    }

    public void ShowNextButton(bool show)
    {
        if (NextBtn != null)
        {
            NextBtn.SetVisibility(show ? Visibility.Visible : Visibility.Hidden);
        }
    }
}