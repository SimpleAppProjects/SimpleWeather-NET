using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace SimpleWeather.Uno.Controls
{
    [TemplatePart(Name = nameof(RootGrid), Type = typeof(Grid))]
    [TemplatePart(Name = nameof(BackBtn), Type = typeof(Button))]
    [TemplatePart(Name = nameof(BackBtnLabel), Type = typeof(TextBlock))]
    [TemplatePart(Name = nameof(BackBtnIconPresenter), Type = typeof(ContentPresenter))]
#if WINDOWS
    [TemplatePart(Name = nameof(IndicatorBox), Type = typeof(ListBox))]
#else
    [TemplatePart(Name = nameof(IndicatorBox), Type = typeof(StackPanel))]
#endif
    [TemplatePart(Name = nameof(NextBtn), Type = typeof(Button))]
    [TemplatePart(Name = nameof(NextBtnLabel), Type = typeof(TextBlock))]
    [TemplatePart(Name = nameof(NextBtnIconPresenter), Type = typeof(ContentPresenter))]
    public sealed partial class BottomStepperNavigationBar : Control
    {
        private Grid RootGrid;
        private Button BackBtn;
        private TextBlock BackBtnLabel;
        private ContentPresenter BackBtnIconPresenter;
#if WINDOWS
        private ListBox IndicatorBox;
#else
        private StackPanel IndicatorBox;
#endif
        private Button NextBtn;
        private TextBlock NextBtnLabel;
        private ContentPresenter NextBtnIconPresenter;

        private int SelectedIdx;
        private int ItemCnt;

        public BottomStepperNavigationBar()
        {
            DefaultStyleKey = typeof(BottomStepperNavigationBar);
            this.Loaded += BottomStepperNavigationBar_Loaded;
            this.Unloaded += BottomStepperNavigationBar_Unloaded;
        }

        public string StartButtonLabel
        {
            get => (string)GetValue(StartButtonLabelProperty);
            set => SetValue(StartButtonLabelProperty, value);
        }

        // Using a DependencyProperty as the backing store for StartButtonLabel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartButtonLabelProperty =
            DependencyProperty.Register("StartButtonLabel", typeof(string), typeof(BottomStepperNavigationBar), new PropertyMetadata(""));

        public string BackButtonLabel
        {
            get => (string)GetValue(BackButtonLabelProperty);
            set => SetValue(BackButtonLabelProperty, value);
        }

        // Using a DependencyProperty as the backing store for BackButtonLabel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackButtonLabelProperty =
            DependencyProperty.Register("BackButtonLabel", typeof(string), typeof(BottomStepperNavigationBar), new PropertyMetadata(""));

        public string NextButtonLabel
        {
            get => (string)GetValue(NextButtonLabelProperty);
            set => SetValue(NextButtonLabelProperty, value);
        }

        // Using a DependencyProperty as the backing store for NextButtonLabel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NextButtonLabelProperty =
            DependencyProperty.Register("NextButtonLabel", typeof(string), typeof(BottomStepperNavigationBar), new PropertyMetadata(""));

        public string CompleteButtonLabel
        {
            get => (string)GetValue(CompleteButtonLabelProperty);
            set => SetValue(CompleteButtonLabelProperty, value);
        }

        // Using a DependencyProperty as the backing store for CompleteButtonLabel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CompleteButtonLabelProperty =
            DependencyProperty.Register("CompleteButtonLabel", typeof(string), typeof(BottomStepperNavigationBar), new PropertyMetadata(""));

        public IconElement BackButtonIcon
        {
            get => (IconElement)GetValue(BackButtonIconProperty);
            set => SetValue(BackButtonIconProperty, value);
        }

        // Using a DependencyProperty as the backing store for BackButtonIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackButtonIconProperty =
            DependencyProperty.Register("BackButtonIcon", typeof(IconElement), typeof(BottomStepperNavigationBar), new PropertyMetadata(new SymbolIcon(Symbol.Back)));

        public IconElement NextButtonIcon
        {
            get => (IconElement)GetValue(NextButtonIconProperty);
            set => SetValue(NextButtonIconProperty, value);
        }

        // Using a DependencyProperty as the backing store for NextButtonIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NextButtonIconProperty =
            DependencyProperty.Register("NextButtonIcon", typeof(IconElement), typeof(BottomStepperNavigationBar), new PropertyMetadata(new SymbolIcon(Symbol.Forward)));

        public IconElement CompleteButtonIcon
        {
            get => (IconElement)GetValue(CompleteButtonIconProperty);
            set => SetValue(CompleteButtonIconProperty, value);
        }

        // Using a DependencyProperty as the backing store for CompleteButtonIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CompleteButtonIconProperty =
            DependencyProperty.Register("CompleteButtonIcon", typeof(IconElement), typeof(BottomStepperNavigationBar), new PropertyMetadata(new SymbolIcon(Symbol.Accept)));

        /// <summary>
        /// Occurs when back button is clicked.
        /// </summary>
        public event RoutedEventHandler BackButtonClicked;
        /// <summary>
        /// Occurs when either start, next or complete button is clicked.
        /// </summary>
        public event RoutedEventHandler NextButtonClicked;

        public int SelectedIndex
        {
            get => SelectedIdx;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(SelectedIndex), "value must be >= 0");
                }

                SelectedIdx = value;
                SetSelectedItem(value, true);
            }
        }

        public int ItemCount
        {
            get => ItemCnt;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(ItemCount), "value must be >= 0");
                }

                if (ItemCnt != value)
                {
                    ItemCnt = value;
                    SetItemCount(value, true);
                }
            }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            RootGrid = GetTemplateChild(nameof(RootGrid)) as Grid;
            BackBtn = GetTemplateChild(nameof(BackBtn)) as Button;
            BackBtnLabel = GetTemplateChild(nameof(BackBtnLabel)) as TextBlock;
            BackBtnIconPresenter = GetTemplateChild(nameof(BackBtnIconPresenter)) as ContentPresenter;
#if WINDOWS
            IndicatorBox = GetTemplateChild(nameof(IndicatorBox)) as ListBox;
#else
            IndicatorBox = GetTemplateChild(nameof(IndicatorBox)) as StackPanel;
#endif
            NextBtn = GetTemplateChild(nameof(NextBtn)) as Button;
            NextBtnLabel = GetTemplateChild(nameof(NextBtnLabel)) as TextBlock;
            NextBtnIconPresenter = GetTemplateChild(nameof(NextBtnIconPresenter)) as ContentPresenter;

            BindButtons(true);
            BindIndicator(true);
        }

        private void BindButtons(bool bind)
        {
            if (BackBtn != null)
            {
                if (bind)
                    BackBtn.Click += BackBtn_Click;
                else
                    BackBtn.Click -= BackBtn_Click;
            }
            if (NextBtn != null)
            {
                if (bind)
                    NextBtn.Click += NextBtn_Click;
                else
                    NextBtn.Click -= NextBtn_Click;
            }
        }

        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            NextButtonClicked?.Invoke(this, e);
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            BackButtonClicked?.Invoke(this, e);
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

        private void IndicatorBox_Loaded(object sender, RoutedEventArgs e)
        {
            SetItemCount(ItemCnt);
            SetSelectedItem(SelectedIdx);
        }

        private void BottomStepperNavigationBar_Loaded(object sender, RoutedEventArgs e)
        {
            SetItemCount(ItemCnt);
            SetSelectedItem(SelectedIdx);
        }

        private void BottomStepperNavigationBar_Unloaded(object sender, RoutedEventArgs e)
        {
            BindButtons(false);
            BindIndicator(false);

            // Clear bindings
            if (NextBtnIconPresenter != null)
                NextBtnIconPresenter.ClearValue(ContentPresenter.ContentProperty);
            if (NextBtnLabel != null)
                NextBtnLabel.ClearValue(TextBlock.TextProperty);
        }

        private void SetItemCount(int count, bool useTransitions = false)
        {
            if (IndicatorBox != null)
            {
#if WINDOWS
                IndicatorBox.ItemsSource = count > 0 ? Enumerable.Repeat(string.Empty, count) : new List<string>(0);
#else
                IndicatorBox.Children.Clear();
                for (int i = 0; i < count; i++)
                {
                    var dot = new Ellipse()
                    {
                        Width = 8,
                        Height = 8,
                        Fill = new SolidColorBrush(Colors.White),
                        Stroke = new SolidColorBrush(Colors.White),
                        Opacity = 1,
                        Margin = new Thickness(4)
                    };
                    IndicatorBox.Children.Add(dot);
                }
#endif
            }
            SetSelectedItem(0, useTransitions);
        }

        private void SetSelectedItem(int idx, bool useTransitions = false)
        {
            if (IndicatorBox != null)
            {
#if WINDOWS
                IndicatorBox.SelectedIndex = idx;
#else
                for (int i = 0; i < ItemCnt; i++)
                {
                    var dot = IndicatorBox.Children[i];
                    if (i == SelectedIdx)
                    {
                        dot.Opacity = 1;
                    }
                    else
                    {
                        dot.Opacity = 0.25;
                    }
                }
#endif
            }
            UpdateButtonsState();
        }

        private void UpdateButtonsState()
        {
            if (SelectedIdx == 0)
            {
                ShowBackButton(false);
                ShowNextButton(true);

                if (NextBtnIconPresenter != null)
                    NextBtnIconPresenter.SetBinding(ContentPresenter.ContentProperty, GetNextIconBinding());
                if (NextBtnLabel != null)
                    NextBtnLabel.SetBinding(TextBlock.TextProperty, GetStartLabelBinding());
            }
            else if (SelectedIdx == (ItemCnt - 1))
            {
                ShowBackButton(true);
                ShowNextButton(true);

                if (NextBtnIconPresenter != null)
                    NextBtnIconPresenter.SetBinding(ContentPresenter.ContentProperty, GetCompleteIconBinding());
                if (NextBtnLabel != null)
                    NextBtnLabel.SetBinding(TextBlock.TextProperty, GetCompleteLabelBinding());
            }
            else
            {
                ShowBackButton(true);
                ShowNextButton(true);

                if (NextBtnIconPresenter != null)
                    NextBtnIconPresenter.SetBinding(ContentPresenter.ContentProperty, GetNextIconBinding());
                if (NextBtnLabel != null)
                    NextBtnLabel.SetBinding(TextBlock.TextProperty, GetNextLabelBinding());
            }
        }

        private BindingBase GetStartLabelBinding()
        {
            return new Binding()
            {
                Source = StartButtonLabel,
                Mode = BindingMode.OneWay
            };
        }

        private BindingBase GetNextIconBinding()
        {
            return new Binding()
            {
                Source = NextButtonIcon,
                Mode = BindingMode.OneWay
            };
        }

        private BindingBase GetNextLabelBinding()
        {
            return new Binding()
            {
                Source = NextButtonLabel,
                Mode = BindingMode.OneWay
            };
        }

        private BindingBase GetCompleteIconBinding()
        {
            return new Binding()
            {
                Source = CompleteButtonIcon,
                Mode = BindingMode.OneWay
            };
        }

        private BindingBase GetCompleteLabelBinding()
        {
            return new Binding()
            {
                Source = CompleteButtonLabel,
                Mode = BindingMode.OneWay
            };
        }

        public void ShowBackButton(bool show)
        {
            if (BackBtn != null)
            {
                BackBtn.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public void ShowNextButton(bool show)
        {
            if (NextBtn != null)
            {
                NextBtn.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}
