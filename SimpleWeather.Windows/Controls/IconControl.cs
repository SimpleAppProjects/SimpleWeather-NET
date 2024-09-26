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
            DependencyProperty.Register(nameof(WeatherIcon), typeof(string), typeof(IconControl), new PropertyMetadata(null, (s, e) => (s as IconControl)?.UpdateWeatherIcon()));

        public bool ForceDarkTheme
        {
            get => (bool)GetValue(ForceDarkThemeProperty);
            set => SetValue(ForceDarkThemeProperty, value);
        }

        // Using a DependencyProperty as the backing store for ForceDarkTheme.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ForceDarkThemeProperty =
            DependencyProperty.Register(nameof(ForceDarkTheme), typeof(bool), typeof(IconControl), new PropertyMetadata(false, (s, e) => (s as IconControl)?.UpdateWeatherIcon()));

        public string IconProvider
        {
            get => (string)GetValue(IconProviderProperty);
            set => SetValue(IconProviderProperty, value);
        }

        // Using a DependencyProperty as the backing store for IconProvider.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProviderProperty =
            DependencyProperty.Register(nameof(IconProvider), typeof(string), typeof(IconControl), new PropertyMetadata(null, (s, e) => (s as IconControl)?.UpdateWeatherIcon()));

        public bool IsLightTheme
        {
            get => (bool)GetValue(IsLightThemeProperty);
            set => SetValue(IsLightThemeProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsLightTheme.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLightThemeProperty =
            DependencyProperty.Register(nameof(IsLightTheme), typeof(bool), typeof(IconControl), new PropertyMetadata(false, (s, e) => (s as IconControl)?.UpdateWeatherIcon()));

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
            DependencyProperty.Register(nameof(ShowAsMonochrome), typeof(bool), typeof(IconControl), new PropertyMetadata(false, (s, e) => (s as IconControl)?.UpdateWeatherIcon()));

        public bool ForceBitmapIcon
        {
            get { return (bool)GetValue(ForceBitmapIconProperty); }
            set { SetValue(ForceBitmapIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ForceBitmapIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ForceBitmapIconProperty =
            DependencyProperty.Register(nameof(ForceBitmapIcon), typeof(bool), typeof(IconControl), new PropertyMetadata(false, (s, e) => (s as IconControl)?.UpdateWeatherIcon()));

        private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

        public IconControl()
        {
            this.DefaultStyleKey = typeof(IconControl);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            IconBox = GetTemplateChild(nameof(IconBox)) as Viewbox;

            UpdateWeatherIcon();
        }

        public async void UpdateWeatherIcon()
        {
            if (IconBox == null) return;

            UIElement iconElement;
            var wicon = WeatherIcon;

            // Remove any animatable drawables
            RemoveAnimatedDrawables();

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
                        var canvas = new SKXamlCanvas();
                        canvas.PaintSurface += (s, e) =>
                        {
                            e.Surface.Canvas.Clear();

                            var bounds = new SKRect(0, 0, e.Info.Height, e.Info.Height);

                            var scaleX = e.Info.Width / ActualWidth;
                            var scaleY = e.Info.Height / ActualHeight;

                            var dX = (float)-((Padding.Left + Padding.Right) * 0.5 / scaleX);
                            var dY = (float)-((Padding.Top + Padding.Bottom) * 0.5 / scaleY);

                            bounds.Inflate(dX, dY);
                            drawable.Bounds = bounds;

                            var cnt = e.Surface.Canvas.Save();
                            e.Surface.Canvas.Translate(-dX, -dY);
                            drawable.Draw(e.Surface.Canvas);
                            e.Surface.Canvas.RestoreToCount(cnt);
                            e.Surface.Flush(true);
                        };
                        canvas.SetBinding(HeightProperty, new Binding()
                        {
                            Source = this,
                            Path = new PropertyPath(nameof(ActualHeight)),
                            Mode = BindingMode.OneWay,
                        });
                        canvas.SetBinding(WidthProperty, new Binding()
                        {
                            Source = this,
                            Path = new PropertyPath(nameof(ActualWidth)),
                            Mode = BindingMode.OneWay,
                        });
                        iconElement = canvas;

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
            else if (wip is ISVGWeatherIconProvider svgProvider && !ShouldUseBitmap())
            {
                try
                {
                    var drawable = await wip.GetSVGDrawable(wicon, isLight: ForceDarkTheme ? false : IsLightTheme);
                    var canvas = new SKXamlCanvas();
                    canvas.PaintSurface += (s, e) =>
                    {
                        e.Surface.Canvas.Clear();

                        var bounds = new SKRect(0, 0, e.Info.Height, e.Info.Height);

                        var scaleX = e.Info.Width / ActualWidth;
                        var scaleY = e.Info.Height / ActualHeight;

                        var dX = (float)-((Padding.Left + Padding.Right) * 0.5 / scaleX);
                        var dY = (float)-((Padding.Top + Padding.Bottom) * 0.5 / scaleY);

                        bounds.Inflate(dX, dY);
                        drawable.Bounds = bounds;

                        var cnt = e.Surface.Canvas.Save();
                        e.Surface.Canvas.Translate(-dX, -dY);
                        drawable.Draw(e.Surface.Canvas);
                        e.Surface.Canvas.RestoreToCount(cnt);
                        e.Surface.Flush(true);
                    };
                    canvas.SetBinding(HeightProperty, new Binding()
                    {
                        Source = this,
                        Path = new PropertyPath(nameof(ActualHeight)),
                        Mode = BindingMode.OneWay,
                    });
                    canvas.SetBinding(WidthProperty, new Binding()
                    {
                        Source = this,
                        Path = new PropertyPath(nameof(ActualWidth)),
                        Mode = BindingMode.OneWay,
                    });
                    iconElement = canvas;
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

        private bool ShouldUseBitmap()
        {
            return ShowAsMonochrome && Foreground is SolidColorBrush brush && brush.Color != Colors.Transparent && !IsBlackOrWhiteColor(brush.Color);
        }

        private bool IsBlackOrWhiteColor(Color c)
        {
            return (c.R == 0xFF && c.G == 0xFF && c.B == 0xFF) || (c.R == 0 && c.G == 0 && c.B == 0);
        }

        private IconElement CreateBitmapIcon(IWeatherIconsProvider provider)
        {
            return CreateBitmapIcon(provider, WeatherIcon);
        }

        private IconElement CreateBitmapIcon(IWeatherIconsProvider provider, string wicon)
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
            return bmpIcon;
        }

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
