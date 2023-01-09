using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.WinUI;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using SimpleWeather.Icons;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;
using muxc = Microsoft.UI.Xaml.Controls;
#if HAS_UNO
using Microsoft.Toolkit.Uwp.UI.Lottie;
#endif

namespace SimpleWeather.Uno.Controls
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
            DependencyProperty.Register("WeatherIcon", typeof(string), typeof(IconControl), new PropertyMetadata(null, (s, e) => (s as IconControl)?.UpdateWeatherIcon()));

        public bool ForceDarkTheme
        {
            get => (bool)GetValue(ForceDarkThemeProperty);
            set => SetValue(ForceDarkThemeProperty, value);
        }

        // Using a DependencyProperty as the backing store for ForceDarkTheme.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ForceDarkThemeProperty =
            DependencyProperty.Register("ForceDarkTheme", typeof(bool), typeof(IconControl), new PropertyMetadata(false, (s, e) => (s as IconControl)?.UpdateWeatherIcon()));

        public string IconProvider
        {
            get => (string)GetValue(IconProviderProperty);
            set => SetValue(IconProviderProperty, value);
        }

        // Using a DependencyProperty as the backing store for IconProvider.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProviderProperty =
            DependencyProperty.Register("IconProvider", typeof(string), typeof(IconControl), new PropertyMetadata(null, (s, e) => (s as IconControl)?.UpdateWeatherIcon()));

        public bool IsLightTheme
        {
            get => (bool)GetValue(IsLightThemeProperty);
            set => SetValue(IsLightThemeProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsLightTheme.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLightThemeProperty =
            DependencyProperty.Register("IsLightTheme", typeof(bool), typeof(IconControl), new PropertyMetadata(false, (s, e) => (s as IconControl)?.UpdateWeatherIcon()));

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
            DependencyProperty.Register("ShowAsMonochrome", typeof(bool), typeof(IconControl), new PropertyMetadata(false, (s, e) => (s as IconControl)?.UpdateWeatherIcon()));

        public bool ForceBitmapIcon
        {
            get { return (bool)GetValue(ForceBitmapIconProperty); }
            set { SetValue(ForceBitmapIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ForceBitmapIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ForceBitmapIconProperty =
            DependencyProperty.Register("ForceBitmapIcon", typeof(bool), typeof(IconControl), new PropertyMetadata(false, (s, e) => (s as IconControl)?.UpdateWeatherIcon()));

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

            var wip = SharedModule.Instance.WeatherIconsManager.GetIconProvider(IconProvider ?? SettingsManager.IconProvider);
            if (ForceBitmapIcon)
            {
                iconElement = CreateBitmapIcon(wip);
            }
            else if (wip is IXamlWeatherIconProvider xamlProvider)
            {
                var iconUri = xamlProvider.GetXamlIconUri(WeatherIcon);
                if (iconUri == null || !iconUri.EndsWith(".xaml"))
                {
                    iconElement = CreateBitmapIcon(wip);
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
                        iconElement = CreateBitmapIcon(wip);
                    }
                }
            }
            else if (wip is ILottieWeatherIconProvider lottieProvider)
            {
                var iconUri = lottieProvider.GetLottieIconURI(WeatherIcon);
                if (iconUri == null || !iconUri.EndsWith(".json"))
                {
                    iconElement = CreateBitmapIcon(wip);
                }
                else
                {
                    try
                    {
#if HAS_UNO
                        iconElement = CreateLottiePlayer(iconUri);
#else
                        iconElement = CreateBitmapIcon(wip);
#endif
                    }
                    catch (Exception e)
                    {
                        Logger.WriteLine(LoggerLevel.Error, e);
                        Logger.WriteLine(LoggerLevel.Info, "Falling back to bitmap icon...");
                        iconElement = CreateBitmapIcon(wip);
                    }
                }
            }
            else
            {
                iconElement = CreateBitmapIcon(wip);
            }

#if HAS_UNO
            DestroyLottiePlayer();
#endif

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
            if (provider is ISVGWeatherIconProvider svgProvider && !ShouldUseBitmap())
            {
                return new ImageIcon()
                {
                    Source = new SvgImageSource(new Uri(svgProvider.GetSVGIconUri(WeatherIcon, ForceDarkTheme ? false : IsLightTheme)))
                };
            }
            else
            {
                var bmpIcon = new BitmapIcon()
                {
                    UriSource = new Uri(provider.GetWeatherIconURI(WeatherIcon, true, ForceDarkTheme ? false : IsLightTheme))
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
        }

#if HAS_UNO
        private muxc.AnimatedVisualPlayer CreateLottiePlayer(string lottieJsonURI)
        {
            return new muxc.AnimatedVisualPlayer()
            {
                AutoPlay = true,
                Source = new LottieVisualSource()
                {
                    Options = LottieVisualOptions.All,
                    UriSource = new Uri(lottieJsonURI)
                }
            };
        }

        private muxc.AnimatedVisualPlayer CreateLottiePlayer(muxc.IAnimatedVisualSource sourceGen)
        {
            return new muxc.AnimatedVisualPlayer()
            {
                AutoPlay = true,
                Source = sourceGen
            };
        }

        private void DestroyLottiePlayer()
        {
            if (IconBox.Child is muxc.AnimatedVisualPlayer player)
            {
                player.Stop();

                var src = player.Source;
                if (src is LottieVisualSource lottieSrc)
                {
                    lottieSrc.UriSource = null;
                }

                player.Source = null;
            }
        }
#endif

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
