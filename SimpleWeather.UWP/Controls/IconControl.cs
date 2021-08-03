using SimpleWeather.Icons;
using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using muxc = Microsoft.UI.Xaml.Controls;
using lottie = Microsoft.Toolkit.Uwp.UI.Lottie;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Markup;

namespace SimpleWeather.UWP.Controls
{
    [TemplatePart(Name = nameof(IconBox), Type = typeof(Viewbox))]
    public sealed class IconControl : Control
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

            var wip = WeatherIconsManager.GetProvider(IconProvider ?? Settings.IconProvider);
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
            /*
            else if (wip is ILottieGenWeatherIconProvider lottieGenProvider)
            {
                var sourceGen = lottieGenProvider.GetLottieGenSource(WeatherIcon);
                if (sourceGen == null)
                {
                    iconElement = CreateBitmapIcon(wip);
                }
                else
                {
                    try
                    {
                        iconElement = CreateLottiePlayer(sourceGen);
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
                        iconElement = CreateLottiePlayer(iconUri);
                    }
                    catch (Exception e)
                    {
                        Logger.WriteLine(LoggerLevel.Error, e);
                        Logger.WriteLine(LoggerLevel.Info, "Falling back to bitmap icon...");
                        iconElement = CreateBitmapIcon(wip);
                    }
                }
            }
            */
            else
            {
                iconElement = CreateBitmapIcon(wip);
            }

            //DestroyLottiePlayer();

            IconBox.Child = iconElement;
        }

        private BitmapIcon CreateBitmapIcon(IWeatherIconsProvider provider)
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

        /*
        private muxc.AnimatedVisualPlayer CreateLottiePlayer(string lottieJsonURI)
        {
            return new muxc.AnimatedVisualPlayer()
            {
                AutoPlay = true,
                Source = new lottie.LottieVisualSource()
                {
                    Options = lottie.LottieVisualOptions.All,
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
                if (src is lottie.LottieVisualSource lottieSrc)
                {
                    lottieSrc.UriSource = null;
                }

                player.Source = null;
            }
        }
        */

        private async Task<UIElement> CreateXAMLIconElement(string uri)
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(uri));
            var xaml = await FileIO.ReadTextAsync(file);
            return await Dispatcher.RunOnUIThread(() =>
            {
                return XamlReader.Load(xaml) as UIElement;
            });
        }
    }
}
