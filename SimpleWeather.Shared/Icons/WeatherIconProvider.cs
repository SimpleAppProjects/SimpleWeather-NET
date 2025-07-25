﻿using SimpleWeather.SkiaSharp;
using System;
using System.IO;
using System.Threading.Tasks;
#if WINUI
using Windows.Storage;
#else
using Microsoft.Maui.Storage;
#endif
using Animation = SkiaSharp.Skottie.Animation;
using SKBitmap = SkiaSharp.SKBitmap;
using SimpleWeather.Utils;
using Svg.Skia;

namespace SimpleWeather.Icons
{
    public abstract partial class WeatherIconProvider : IWeatherIconsProvider, ISVGWeatherIconProvider
    {
        public abstract string Key { get; }
        public abstract string DisplayName { get; }
        public abstract string AuthorName { get; }
        public abstract Uri AttributionLink { get; }
        public abstract bool IsFontIcon { get; }

        public abstract Uri GetWeatherIconURI(string icon);
        public abstract string GetWeatherIconURI(string icon, bool isAbsoluteUri, bool isLight = false);
        public abstract string GetSVGIconUri(string icon, bool isLight = false);

        public virtual Task<SKDrawable> GetDrawable(string icon, bool isLight = false)
        {
            return Task.Run(async () =>
            {
                SKDrawable drawable = null;

                if (this is ILottieWeatherIconProvider lottieProvider)
                {
                    try
                    {
#if WINUI
                        var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(lottieProvider.GetLottieIconURI(icon, isLight)));
                        var fStream = (await file.OpenReadAsync()).AsStreamForRead();
#else
                        var filePath = lottieProvider.GetLottieIconURI(icon, isLight);
                        var filename = Path.GetFileName(filePath);
                        var extension = (isLight ? "light" : "dark") + Path.GetExtension(filename);
                        var themeFile = $"{Path.GetFileNameWithoutExtension(filename)}_{extension}";
                        var fStream = await FileSystemUtils.OpenAppPackageFileAsync(themeFile);
#endif

                        drawable = Animation.Create(fStream)?.ToDrawable();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(nameof(WeatherIconProvider), "error loading lottie animation", ex);
                    }
                }

                drawable ??= await GetSVGDrawable(icon, isLight);

                drawable ??= await GetBitmapDrawable(icon, isLight);

                return drawable;
            });
        }

        public virtual Task<SKDrawable> GetSVGDrawable(string icon, bool isLight = false)
        {
            return Task.Run(async () =>
            {
                try
                {
#if WINUI
                    var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(GetSVGIconUri(icon, isLight)));
                    var fStream = (await file.OpenReadAsync()).AsStreamForRead();
#else
                    var filePath = GetSVGIconUri(icon, isLight);
                    var filename = Path.GetFileName(filePath);
                    var extension = (isLight ? "light" : "dark") + Path.GetExtension(filename);
                    var themeFile = $"{Path.GetFileNameWithoutExtension(filename)}_{extension}";
                    var fStream = await FileSystemUtils.OpenAppPackageFileAsync(themeFile);
#endif

                    var svg = SKSvg.CreateFromStream(fStream);
                    return svg.ToDrawable();
                }
                catch (Exception ex)
                {
                    Logger.Error(nameof(WeatherIconProvider), "error loading svg", ex);
                }
                
                return null;
            });
        }

        public virtual Task<SKDrawable> GetBitmapDrawable(string icon, bool isLight = false)
        {
            return Task.Run(async () =>
            {
                try
                {
#if WINUI
                    var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(GetWeatherIconURI(icon, isAbsoluteUri: true, isLight)));
                    var fStream = (await file.OpenReadAsync()).AsStreamForRead();
#else
                    var filePath = GetWeatherIconURI(icon, isAbsoluteUri: false, isLight);
                    var filename = Path.GetFileName(filePath);
                    var extension = (isLight ? "light" : "dark") + Path.GetExtension(filename);
                    var themeFile = $"{Path.GetFileNameWithoutExtension(filename)}_{extension}";
                    var fStream = await FileSystemUtils.OpenAppPackageFileAsync(themeFile);
#endif

                    return SKBitmap.Decode(fStream).ToDrawable();
                }
                catch (Exception ex)
                {
                    Logger.Error(nameof(WeatherIconProvider), "error loading bitmap", ex);
                }
                
                return null;
            });
        }
    }
}
