using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.Icons;
using SimpleWeather.Preferences;
using SimpleWeather.SkiaSharp;
using SimpleWeather.Utils;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using SKDrawable = SimpleWeather.SkiaSharp.SKDrawable;

namespace SimpleWeather.Maui.Controls
{
    public partial class IconControl : TemplatedView
    {
        private SKCanvasView Canvas;

        public string WeatherIcon
        {
            get => (string)GetValue(WeatherIconProperty);
            set => SetValue(WeatherIconProperty, value);
        }

        public static readonly BindableProperty WeatherIconProperty =
            BindableProperty.Create(nameof(WeatherIcon), typeof(string), typeof(IconControl), null, propertyChanged: (obj, _, _) => (obj as IconControl)?.UpdateWeatherIcon());

        public bool ForceDarkTheme
        {
            get => (bool)GetValue(ForceDarkThemeProperty);
            set => SetValue(ForceDarkThemeProperty, value);
        }

        public static readonly BindableProperty ForceDarkThemeProperty =
            BindableProperty.Create(nameof(ForceDarkTheme), typeof(bool), typeof(IconControl), false, propertyChanged: (obj, _, _) => (obj as IconControl)?.UpdateWeatherIcon());

        public string IconProvider
        {
            get => (string)GetValue(IconProviderProperty);
            set => SetValue(IconProviderProperty, value);
        }

        public static readonly BindableProperty IconProviderProperty =
            BindableProperty.Create(nameof(IconProvider), typeof(string), typeof(IconControl), null, propertyChanged: (obj, _, _) => (obj as IconControl)?.UpdateWeatherIcon());

        public bool IsLightTheme
        {
            get => (bool)GetValue(IsLightThemeProperty);
            set => SetValue(IsLightThemeProperty, value);
        }

        public static readonly BindableProperty IsLightThemeProperty =
            BindableProperty.Create(nameof(IsLightTheme), typeof(bool), typeof(IconControl), false, propertyChanged: (obj, _, _) => (obj as IconControl)?.UpdateWeatherIcon());

        public bool ShowAsMonochrome
        {
            get => (bool)GetValue(ShowAsMonochromeProperty);
            set => SetValue(ShowAsMonochromeProperty, value);
        }

        public static readonly BindableProperty ShowAsMonochromeProperty =
            BindableProperty.Create(nameof(ShowAsMonochrome), typeof(bool), typeof(IconControl), false, propertyChanged: (obj, _, _) => (obj as IconControl)?.UpdateWeatherIcon());

        public bool ForceBitmapIcon
        {
            get => (bool)GetValue(ForceBitmapIconProperty);
            set => SetValue(ForceBitmapIconProperty, value);
        }

        public static readonly BindableProperty ForceBitmapIconProperty =
            BindableProperty.Create(nameof(ForceBitmapIcon), typeof(bool), typeof(IconControl), false, propertyChanged: (obj, _, _) => (obj as IconControl)?.UpdateWeatherIcon());

        public Color IconColor
        {
            get => (Color)GetValue(IconColorProperty);
            set => SetValue(IconColorProperty, value);
        }

        public static readonly BindableProperty IconColorProperty =
            BindableProperty.Create(nameof(IconColor), typeof(Color), typeof(IconControl), Colors.Transparent, propertyChanged: (obj, _, _) => (obj as IconControl)?.UpdateWeatherIcon());

        public double IconHeight
        {
            get => (double)GetValue(IconHeightProperty);
            set => SetValue(IconHeightProperty, value);
        }

        public static readonly BindableProperty IconHeightProperty =
            BindableProperty.Create(nameof(IconHeight), typeof(double), typeof(IconControl), -1d, propertyChanged: (obj, _, _) => (obj as IconControl)?.UpdateWeatherIcon());

        public double IconWidth
        {
            get => (double)GetValue(IconWidthProperty);
            set => SetValue(IconWidthProperty, value);
        }

        public static readonly BindableProperty IconWidthProperty =
            BindableProperty.Create(nameof(IconWidth), typeof(double), typeof(IconControl), -1d, propertyChanged: (obj, _, _) => (obj as IconControl)?.UpdateWeatherIcon());

        private readonly SettingsManager _settingsManager = Ioc.Default.GetService<SettingsManager>();
        
        private SKDrawable? Drawable = null;

        public IconControl()
        {
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Canvas = GetTemplateChild(nameof(Canvas)) as SKCanvasView;
            if (Canvas != null) 
                Canvas.PaintSurface += CanvasOnPaintSurface;

            UpdateWeatherIcon();
        }

        public async void UpdateWeatherIcon()
        {
            if (Canvas == null) return;

            Drawable = null;

            if (WeatherIcon == null)
            {
                Canvas?.InvalidateSurface();
                return;
            }

            // Remove any animatable drawables
            RemoveAnimatedDrawables();

            var wip = SharedModule.Instance.WeatherIconsManager.GetIconProvider(IconProvider ?? _settingsManager.IconProvider);

            if (ForceBitmapIcon)
            {
                Drawable = await CreateBitmapIcon(wip);
            }
            else if (wip is IXamlWeatherIconProvider xamlProvider)
            {
                var iconUri = xamlProvider.GetXamlIconUri(WeatherIcon);
                if (iconUri == null || !iconUri.EndsWith(".xaml"))
                {
                    Drawable = await CreateBitmapIcon(wip);
                }
                else
                {
                    try
                    {
                        Drawable = await CreateXAMLIconElement(iconUri);
                    }
                    catch (Exception e)
                    {
                        Logger.WriteLine(LoggerLevel.Error, e);
                        Logger.WriteLine(LoggerLevel.Info, "Falling back to bitmap icon...");
                        Drawable = await CreateBitmapIcon(wip);
                    }
                }
            }
            else if (wip is ILottieWeatherIconProvider lottieProvider)
            {
                var isLight = ForceDarkTheme ? false : IsLightTheme;
                var iconUri = lottieProvider.GetLottieIconURI(WeatherIcon, isLight: isLight);
                if (iconUri == null || !iconUri.EndsWith(".json"))
                {
                    Drawable = await CreateBitmapIcon(wip);
                }
                else
                {
                    try
                    {
                        var drawable = await wip.GetDrawable(WeatherIcon, isLight: isLight);

                        if (drawable is SKLottieDrawable lottieDrawable)
                        {
                            AddAnimatedDrawable(lottieDrawable);
                        }
                        
                        Drawable = drawable;
                    }
                    catch (Exception e)
                    {
                        Logger.WriteLine(LoggerLevel.Error, e);
                        Logger.WriteLine(LoggerLevel.Info, "Falling back to bitmap icon...");
                        Drawable = await CreateBitmapIcon(wip);
                    }
                }
            }
            else if (wip is ISVGWeatherIconProvider svgProvider && !ShouldUseBitmap())
            {
                try
                {
                    Drawable = await svgProvider.GetSVGDrawable(WeatherIcon, isLight: ForceDarkTheme ? false : IsLightTheme);
                }
                catch (Exception e)
                {
                    Logger.WriteLine(LoggerLevel.Error, e);
                    Logger.WriteLine(LoggerLevel.Info, "Falling back to bitmap icon...");
                    Drawable = await CreateBitmapIcon(wip);
                }
            }
            else
            {
                Drawable = await CreateBitmapIcon(wip);
            }

            Canvas?.InvalidateSurface();
        }

        private void CanvasOnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            e.Surface.Canvas.Clear();

            if (Drawable != null)
            {
                var padding = (float)Math.Max(Padding.HorizontalThickness, Padding.VerticalThickness) / 2;
                var bounds = new SKRect(0, 0, e.Info.Width - padding, e.Info.Height - padding);

                var cnt = e.Surface.Canvas.Save();

                Drawable.Bounds = bounds;

                if (padding > 0) e.Surface.Canvas.Translate(padding / 2, padding / 2);

                Drawable.Draw(e.Surface.Canvas);

                e.Surface.Canvas.RestoreToCount(cnt);
            }

            e.Surface.Flush(true);
        }

        private bool ShouldUseBitmap()
        {
            return ShowAsMonochrome && !Equals(IconColor, Colors.Transparent) && !IsBlackOrWhiteColor(IconColor);
        }

        private bool IsBlackOrWhiteColor(Color c)
        {
            return (c.Red == 1f && c.Green == 1f && c.Blue == 1f) || (c.Red == 0f && c.Green == 0f && c.Blue == 0f);
        }

        private async Task<SKDrawable> CreateBitmapIcon(IWeatherIconsProvider provider)
        {
            var drawable = await provider.GetBitmapDrawable(WeatherIcon, isLight: ForceDarkTheme ? false : IsLightTheme).ConfigureAwait(false);
            if (ShowAsMonochrome && drawable is SKBitmapDrawable bmpDrawable)
            {
                bmpDrawable.TintColor = IconColor.ToSKColor();
            }

            return drawable;
        }

        private Task<SKDrawable> CreateXAMLIconElement(string _)
        {
            return Task.FromResult<SKDrawable>(null);
        }
    }
}

