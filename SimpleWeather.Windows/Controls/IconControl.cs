using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.WinUI;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using SimpleWeather.Icons;
using SimpleWeather.Preferences;
using SimpleWeather.SkiaSharp;
using SimpleWeather.Utils;
using SkiaSharp;
using SkiaSharp.Views.Windows;
using Windows.Storage;
using Windows.UI;
using Binding = Microsoft.UI.Xaml.Data.Binding;
using BindingMode = Microsoft.UI.Xaml.Data.BindingMode;
using Color = Windows.UI.Color;
using Colors = Microsoft.UI.Colors;
using SolidColorBrush = Microsoft.UI.Xaml.Media.SolidColorBrush;

namespace SimpleWeather.NET.Controls
{
    [TemplatePart(Name = nameof(IconBox), Type = typeof(Viewbox))]
    public sealed partial class IconControl : Control
    {
        private Viewbox IconBox;

        public string WeatherIcon
        {
            get => (string)GetValue(WeatherIconProperty);
            set => SetValue(WeatherIconProperty, value);
        }

        // Using a DependencyProperty as the backing store for WeatherIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WeatherIconProperty =
            DependencyProperty.Register(nameof(WeatherIcon), typeof(string), typeof(IconControl), new PropertyMetadata(null, OnPropertyChanged));

        public bool ForceDarkTheme
        {
            get => (bool)GetValue(ForceDarkThemeProperty);
            set => SetValue(ForceDarkThemeProperty, value);
        }

        // Using a DependencyProperty as the backing store for ForceDarkTheme.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ForceDarkThemeProperty =
            DependencyProperty.Register(nameof(ForceDarkTheme), typeof(bool), typeof(IconControl), new PropertyMetadata(false, OnPropertyChanged));

        public string IconProvider
        {
            get => (string)GetValue(IconProviderProperty);
            set => SetValue(IconProviderProperty, value);
        }

        // Using a DependencyProperty as the backing store for IconProvider.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProviderProperty =
            DependencyProperty.Register(nameof(IconProvider), typeof(string), typeof(IconControl), new PropertyMetadata(null, OnPropertyChanged));

        public bool IsLightTheme
        {
            get => (bool)GetValue(IsLightThemeProperty);
            set => SetValue(IsLightThemeProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsLightTheme.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLightThemeProperty =
            DependencyProperty.Register(nameof(IsLightTheme), typeof(bool), typeof(IconControl), new PropertyMetadata(false, OnPropertyChanged));

        /// <summary>
        /// Gets or sets a value that indicates whether the bitmap is shown in a single color.
        /// </summary>
        public bool ShowAsMonochrome
        {
            get => (bool)GetValue(ShowAsMonochromeProperty);
            set => SetValue(ShowAsMonochromeProperty, value);
        }

        // Using a DependencyProperty as the backing store for ShowAsMonochrome.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowAsMonochromeProperty =
            DependencyProperty.Register(nameof(ShowAsMonochrome), typeof(bool), typeof(IconControl), new PropertyMetadata(false, OnDrawablePropertyChanged));

        public bool ForceBitmapIcon
        {
            get { return (bool)GetValue(ForceBitmapIconProperty); }
            set { SetValue(ForceBitmapIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ForceBitmapIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ForceBitmapIconProperty =
            DependencyProperty.Register(nameof(ForceBitmapIcon), typeof(bool), typeof(IconControl), new PropertyMetadata(false, OnDrawablePropertyChanged));

        public double IconHeight
        {
            get => (double)GetValue(IconHeightProperty);
            set => SetValue(IconHeightProperty, value);
        }

        public static readonly DependencyProperty IconHeightProperty =
            DependencyProperty.Register(nameof(IconHeight), typeof(double), typeof(IconControl), new PropertyMetadata(double.NaN, OnDrawablePropertyChanged));

        public double IconWidth
        {
            get => (double)GetValue(IconWidthProperty);
            set => SetValue(IconWidthProperty, value);
        }

        public static readonly DependencyProperty IconWidthProperty =
            DependencyProperty.Register(nameof(IconWidth), typeof(double), typeof(IconControl), new PropertyMetadata(double.NaN, OnDrawablePropertyChanged));

        private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

        public IconControl()
        {
            this.DefaultStyleKey = typeof(IconControl);
            this.RegisterPropertyChangedCallback(PaddingProperty, (obj, _) =>
            {
                (obj as IconControl)?.UpdateDrawable();
            });
            this.Loaded += IconControl_Loaded;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            IconBox = GetTemplateChild(nameof(IconBox)) as Viewbox;

            UpdateWeatherIcon();
        }

        private static void OnPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (!Equals(e.OldValue, e.NewValue))
            {
                (obj as IconControl)?.UpdateWeatherIcon();
            }
        }

        private static void OnDrawablePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (!Equals(e.OldValue, e.NewValue))
            {
                (obj as IconControl)?.UpdateDrawable();
            }
        }

        private void IconControl_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateWeatherIcon();
        }

        public async void UpdateWeatherIcon()
        {
            if (IconBox == null) return;

            var iconElement = IconBox.Child;
            var wicon = WeatherIcon;

            // Remove any animatable drawables
            RemoveAnimatedDrawables();

            {
                if (iconElement is SkDrawableCanvas canvas)
                {
                    var drawable = canvas.Drawable;
                    canvas.UpdateDrawable(null, invalidate: false);
                    
                    if (drawable is IDisposable disposable)
                    {
                        this.RunCatching(() => disposable.Dispose());
                    }
                }
            }

            if (wicon == null)
            {
                IconBox.Child = null;
                return;
            }

            var wip = SharedModule.Instance.WeatherIconsManager.GetIconProvider(IconProvider ?? SettingsManager.IconProvider);

            if (ForceBitmapIcon)
            {
                iconElement = CreateBitmapIcon(wip, wicon);
            }
            else if (wip is IXamlWeatherIconProvider xamlProvider)
            {
                var iconUri = xamlProvider.GetXamlIconUri(wicon);
                if (iconUri == null || !iconUri.EndsWith(".xaml"))
                {
                    iconElement = CreateBitmapIcon(wip, wicon);
                }
                else
                {
                    try
                    {
                        iconElement = await CreateXAMLIconElement(iconUri);
                    }
                    catch (Exception e)
                    {
                        Logger.WriteLine(LoggerLevel.Error, e);
                        Logger.WriteLine(LoggerLevel.Info, "Falling back to bitmap icon...");
                        iconElement = CreateBitmapIcon(wip, wicon);
                    }
                }
            }
            else if (wip is ILottieWeatherIconProvider lottieProvider)
            {
                var isLight = ForceDarkTheme ? false : IsLightTheme;
                var iconUri = lottieProvider.GetLottieIconURI(wicon, isLight);
                if (iconUri == null || !iconUri.EndsWith(".json"))
                {
                    iconElement = CreateBitmapIcon(wip, wicon);
                }
                else
                {
                    try
                    {
                        var drawable = await wip.GetDrawable(wicon, isLight: isLight);

                        if (iconElement is SkDrawableCanvas canvas)
                        {
                            canvas.UpdateDrawable(drawable);
                        }
                        else
                        {
                            canvas = new SkDrawableCanvas(drawable);
                            canvas.SetBinding(SkDrawableCanvas.DrawablePaddingProperty, new Binding()
                            {
                                Source = this,
                                Path = new PropertyPath(nameof(Padding)),
                                Mode = BindingMode.OneWay,
                            });
                            canvas.SetBinding(HeightProperty, new Binding()
                            {
                                Source = this,
                                Path = new PropertyPath(nameof(IconHeight)),
                                Mode = BindingMode.OneWay,
                            });
                            canvas.SetBinding(WidthProperty, new Binding()
                            {
                                Source = this,
                                Path = new PropertyPath(nameof(IconWidth)),
                                Mode = BindingMode.OneWay,
                            });
                            canvas.Invalidate();
                            iconElement = canvas;
                        }

                        if (drawable is SKLottieDrawable lottieDrawable)
                        {
                            AddAnimatedDrawable(lottieDrawable);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.WriteLine(LoggerLevel.Error, e);
                        Logger.WriteLine(LoggerLevel.Info, "Falling back to bitmap icon...");
                        iconElement = CreateBitmapIcon(wip, wicon);
                    }
                }
            }
            else if (wip is ISVGWeatherIconProvider svgProvider)
            {
                try
                {
                    var drawable = await svgProvider.GetSVGDrawable(wicon, isLight: ForceDarkTheme ? false : IsLightTheme);
                    
                    if (ShouldUseFilter() && Foreground is SolidColorBrush colorBrush && drawable is ITintableDrawable tintable)
                    {
                        tintable.TintColor = colorBrush.Color.ToSKColor();
                    }

                    if (iconElement is SkDrawableCanvas canvas)
                    {
                        canvas.UpdateDrawable(drawable);
                    }
                    else
                    {
                        canvas = new SkDrawableCanvas(drawable);
                        canvas.SetBinding(SkDrawableCanvas.DrawablePaddingProperty, new Binding()
                        {
                            Source = this,
                            Path = new PropertyPath(nameof(Padding)),
                            Mode = BindingMode.OneWay,
                        });
                        canvas.SetBinding(HeightProperty, new Binding()
                        {
                            Source = this,
                            Path = new PropertyPath(nameof(IconHeight)),
                            Mode = BindingMode.OneWay,
                        });
                        canvas.SetBinding(WidthProperty, new Binding()
                        {
                            Source = this,
                            Path = new PropertyPath(nameof(IconWidth)),
                            Mode = BindingMode.OneWay,
                        });
                        canvas.Invalidate();
                        iconElement = canvas;
                    }
                }
                catch (Exception e)
                {
                    Logger.WriteLine(LoggerLevel.Error, e);
                    Logger.WriteLine(LoggerLevel.Info, "Falling back to bitmap icon...");
                    iconElement = CreateBitmapIcon(wip, wicon);
                }
            }
            else
            {
                iconElement = CreateBitmapIcon(wip, wicon);
            }

            IconBox.Child = iconElement;
        }

        private void UpdateDrawable()
        {
            if (IconBox?.Child is { } iconElement)
            {
                if (ForceBitmapIcon && iconElement is not (BitmapIcon or SkDrawableCanvas { Drawable: ITintableDrawable }))
                {
                    UpdateWeatherIcon();
                    return;
                }

                if (iconElement is SkDrawableCanvas canvas)
                {
                    if (canvas.Drawable is ITintableDrawable tintable)
                    {
                        if (ShouldUseFilter() && Foreground is SolidColorBrush colorBrush)
                        {
                            tintable.TintColor = colorBrush.Color.ToSKColor();
                        }
                        else
                        {
                            tintable.TintColor = null;
                        }
                    }
                    
                    canvas.Invalidate();
                }
                else if (iconElement is BitmapIcon bmpIcon)
                {
                    UpdateBitmapScaling(bmpIcon);
                }
            }
            else
            {
                UpdateWeatherIcon();
            }
        }

        private bool ShouldUseFilter()
        {
            return ShowAsMonochrome && Foreground is SolidColorBrush brush && brush.Color != Colors.Transparent && !IsBlackOrWhiteColor(brush.Color);
        }

        private static bool IsBlackOrWhiteColor(Color c)
        {
            return c == Colors.White || c == Colors.Black;
        }

        private BitmapIcon CreateBitmapIcon(IWeatherIconsProvider provider)
        {
            return CreateBitmapIcon(provider, WeatherIcon);
        }

        private BitmapIcon CreateBitmapIcon(IWeatherIconsProvider provider, string wicon)
        {
            var bmpIcon = new BitmapIcon()
            {
                UriSource = new Uri(provider.GetWeatherIconURI(wicon, true, ForceDarkTheme ? false : IsLightTheme))
            };
            bmpIcon.SetBinding(BitmapIcon.ForegroundProperty, new Binding()
            {
                Mode = BindingMode.OneWay,
                Source = Foreground
            });
            bmpIcon.SetBinding(BitmapIcon.ShowAsMonochromeProperty, new Binding()
            {
                Mode = BindingMode.OneWay,
                Source = ShowAsMonochrome
            });
            bmpIcon.SetBinding(HeightProperty, new Binding()
            {
                Source = this,
                Path = new PropertyPath(nameof(IconHeight)),
                Mode = BindingMode.OneWay,
            });
            bmpIcon.SetBinding(WidthProperty, new Binding()
            {
                Source = this,
                Path = new PropertyPath(nameof(IconWidth)),
                Mode = BindingMode.OneWay,
            });
            
            UpdateBitmapScaling(bmpIcon);
            
            return bmpIcon;
        }

        private void UpdateBitmapScaling(BitmapIcon bmpIcon)
        {
            var padding = Math.Max(Padding.Left + Padding.Right, Padding.Top + Padding.Bottom) / 2;
            bmpIcon.RenderTransformOrigin = new Windows.Foundation.Point(0.5, 0.5);
            bmpIcon.RenderTransform = new ScaleTransform()
            {
                ScaleX = (float)Math.Max((IconHeight - padding) / IconHeight, (IconWidth - padding) / IconWidth),
                ScaleY = (float)Math.Max((IconHeight - padding) / IconHeight, (IconWidth - padding) / IconWidth)
            };
        }

        /* Canvas */
        private async Task<UIElement> CreateXAMLIconElement(string uri)
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(uri));
            var xaml = await FileIO.ReadTextAsync(file);
            return await DispatcherQueue.EnqueueAsync(() =>
            {
                return XamlReader.Load(xaml) as UIElement;
            });
        }
    }
}
