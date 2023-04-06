using Microsoft.Extensions.DependencyInjection;
using SimpleWeather.Common.Controls;
using SimpleWeather.Icons;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API;
using SimpleWeather.WeatherData;
using SimpleWeather.WeatherData.Images;
using SimpleWeather.WeatherData.Images.Model;
using System;
using System.IO;
using System.Threading.Tasks;
#if WINUI
using Windows.Storage;
#else
using Microsoft.Maui.Storage;
#endif

namespace SimpleWeather.Common.Images
{
    public static partial class WeatherUtils
    {
        public static async Task<ImageDataViewModel> GetImageData(this Weather weather)
        {
            String icon = weather.condition.icon;
            String backgroundCode = null;
            var wm = WeatherModule.Instance.WeatherManager;

            // Apply background based on weather condition
            switch (icon)
            {
                // Rain
                case WeatherIcons.DAY_HAIL:
                case WeatherIcons.DAY_RAIN:
                case WeatherIcons.DAY_RAIN_MIX:
                case WeatherIcons.DAY_RAIN_WIND:
                case WeatherIcons.DAY_SHOWERS:
                case WeatherIcons.DAY_SLEET:
                case WeatherIcons.DAY_SPRINKLE:
                    backgroundCode = WeatherBackground.RAIN;
                    break;

                case WeatherIcons.NIGHT_ALT_HAIL:
                case WeatherIcons.NIGHT_ALT_RAIN:
                case WeatherIcons.NIGHT_ALT_RAIN_MIX:
                case WeatherIcons.NIGHT_ALT_RAIN_WIND:
                case WeatherIcons.NIGHT_ALT_SHOWERS:
                case WeatherIcons.NIGHT_ALT_SLEET:
                case WeatherIcons.NIGHT_ALT_SPRINKLE:
                case WeatherIcons.RAIN:
                case WeatherIcons.RAIN_MIX:
                case WeatherIcons.RAIN_WIND:
                case WeatherIcons.SHOWERS:
                case WeatherIcons.SLEET:
                case WeatherIcons.SPRINKLE:
                    backgroundCode = WeatherBackground.RAIN_NIGHT;
                    break;
                // Tornado / Hurricane / Thunderstorm / Tropical Storm
                case WeatherIcons.DAY_LIGHTNING:
                case WeatherIcons.DAY_THUNDERSTORM:
                case WeatherIcons.NIGHT_ALT_LIGHTNING:
                case WeatherIcons.NIGHT_ALT_THUNDERSTORM:
                case WeatherIcons.LIGHTNING:
                case WeatherIcons.THUNDERSTORM:
                    backgroundCode = WeatherBackground.TSTORMS_NIGHT;
                    break;

                case WeatherIcons.DAY_STORM_SHOWERS:
                case WeatherIcons.DAY_SLEET_STORM:
                case WeatherIcons.STORM_SHOWERS:
                case WeatherIcons.SLEET_STORM:
                case WeatherIcons.NIGHT_ALT_STORM_SHOWERS:
                case WeatherIcons.NIGHT_ALT_SLEET_STORM:
                case WeatherIcons.HAIL:
                case WeatherIcons.HURRICANE:
                case WeatherIcons.TORNADO:
                    backgroundCode = WeatherBackground.STORMS;
                    break;

                // Dust
                case WeatherIcons.DUST:
                case WeatherIcons.SANDSTORM:
                    backgroundCode = WeatherBackground.DUST;
                    break;

                // Foggy / Haze
                case WeatherIcons.DAY_FOG:
                case WeatherIcons.DAY_HAZE:
                case WeatherIcons.FOG:
                case WeatherIcons.HAZE:
                case WeatherIcons.NIGHT_FOG:
                case WeatherIcons.NIGHT_HAZE:
                case WeatherIcons.SMOG:
                case WeatherIcons.SMOKE:
                    backgroundCode = WeatherBackground.FOG;
                    break;

                // Snow / Snow Showers/Storm
                case WeatherIcons.DAY_SNOW:
                case WeatherIcons.DAY_SNOW_THUNDERSTORM:
                case WeatherIcons.DAY_SNOW_WIND:
                case WeatherIcons.NIGHT_ALT_SNOW:
                case WeatherIcons.NIGHT_ALT_SNOW_THUNDERSTORM:
                case WeatherIcons.NIGHT_ALT_SNOW_WIND:
                case WeatherIcons.SNOW:
                case WeatherIcons.SNOW_THUNDERSTORM:
                case WeatherIcons.SNOW_WIND:
                    backgroundCode = WeatherBackground.SNOW;
                    break;

                /* Ambigious weather conditions */
                // (Mostly) Cloudy
                case WeatherIcons.CLOUD:
                case WeatherIcons.CLOUDY:
                case WeatherIcons.CLOUDY_GUSTS:
                case WeatherIcons.CLOUDY_WINDY:
                case WeatherIcons.DAY_CLOUDY:
                case WeatherIcons.DAY_CLOUDY_GUSTS:
                case WeatherIcons.DAY_CLOUDY_HIGH:
                case WeatherIcons.DAY_CLOUDY_WINDY:
                case WeatherIcons.NIGHT_ALT_CLOUDY:
                case WeatherIcons.NIGHT_ALT_CLOUDY_GUSTS:
                case WeatherIcons.NIGHT_ALT_CLOUDY_HIGH:
                case WeatherIcons.NIGHT_ALT_CLOUDY_WINDY:
                case WeatherIcons.DAY_SUNNY_OVERCAST:
                case WeatherIcons.NIGHT_OVERCAST:
                case WeatherIcons.OVERCAST:
                    if (wm.IsNight(weather))
                        backgroundCode = WeatherBackground.MOSTLYCLOUDY_NIGHT;
                    else
                        backgroundCode = WeatherBackground.MOSTLYCLOUDY_DAY;
                    break;

                // Partly Cloudy
                case WeatherIcons.DAY_PARTLY_CLOUDY:
                case WeatherIcons.NIGHT_ALT_PARTLY_CLOUDY:
                    if (wm.IsNight(weather))
                        backgroundCode = WeatherBackground.PARTLYCLOUDY_NIGHT;
                    else
                        backgroundCode = WeatherBackground.PARTLYCLOUDY_DAY;
                    break;

                case WeatherIcons.DAY_SUNNY:
                case WeatherIcons.NA:
                case WeatherIcons.NIGHT_CLEAR:
                case WeatherIcons.SNOWFLAKE_COLD:
                case WeatherIcons.DAY_HOT:
                case WeatherIcons.NIGHT_HOT:
                case WeatherIcons.HOT:
                case WeatherIcons.DAY_WINDY:
                case WeatherIcons.NIGHT_WINDY:
                case WeatherIcons.WINDY:
                case WeatherIcons.DAY_LIGHT_WIND:
                case WeatherIcons.NIGHT_LIGHT_WIND:
                case WeatherIcons.LIGHT_WIND:
                case WeatherIcons.STRONG_WIND:
                default:
                    // Set background based using sunset/rise times
                    if (wm.IsNight(weather))
                        backgroundCode = WeatherBackground.NIGHT;
                    else
                        backgroundCode = WeatherBackground.DAY;
                    break;
            }

            // Check cache for image data
            var imageHelper = SharedModule.Instance.Services.GetService<ImageDataHelperImpl>();
            var imageData = await imageHelper.GetCachedImageData(backgroundCode);
            // Check if cache is available and valid
#if WINUI
            var imgDataValid = imageData != null && imageData.IsValid();
#else
            var imgDataValid = imageData != null && await imageData.IsValidAsync();
#endif
            // Validate image header/contents
            var imgValid = imgDataValid && (await imageData?.IsImageValid());
            if (imgValid)
                return new ImageDataViewModel(imageData);
            else
            {
                // Delete invalid file
                if (imgDataValid && !imgValid && imageData != null)
                {
                    var uri = new Uri(imageData.ImageUrl);
                    if (uri.Scheme == "file")
                    {
                        try
                        {
                            if (File.Exists(imageData.ImageUrl))
                            {
                                File.Delete(imageData.ImageUrl);
                            }
                        }
                        catch { }
                    }
                }

#if !UNIT_TEST
                if (!FeatureSettings.IsUpdateAvailable)
                {
#endif
                    imageData = await imageHelper.GetRemoteImageData(backgroundCode);
#if WINUI
                    if (imageData?.IsValid() == true)
#else
                    if (await imageData?.IsValidAsync() == true)
#endif
                        return new ImageDataViewModel(imageData);
                    else
                    {
                        imageData = imageHelper.GetDefaultImageData(backgroundCode);
#if WINUI
                        if (imageData?.IsValid() == true)
#else
                        if (await imageData?.IsValidAsync() == true)
#endif
                            return new ImageDataViewModel(imageData);
                    }
#if !UNIT_TEST
                }
                else
                {
                    imageData = imageHelper.GetDefaultImageData(backgroundCode);
#if WINUI
                    if (imageData?.IsValid() == true)
#else
                    if (await imageData?.IsValidAsync() == true)
#endif
                        return new ImageDataViewModel(imageData);
                }
#endif
            }

            return null;
        }

        public static async Task<bool> IsImageValid(this ImageData imgData)
        {
#if WINUI
            if (imgData?.IsValid() == true)
#else
            if (await imgData?.IsValidAsync() == true)
#endif
            {
                var uri = new Uri(imgData.ImageUrl);
                if (uri.Scheme == "file" || uri.Scheme == "ms-appx" || uri.Scheme == "maui-appx")
                {
                    Stream stream;
#if !WINUI
                    stream = null;
#endif

                    if (uri.Scheme == "ms-appx")
                    {
#if WINUI
                        try
                        {
                            var file = await StorageFile.GetFileFromApplicationUriAsync(uri);

                            if (file.IsAvailable)
                            {
                                while (FileUtils.IsFileLocked(file))
                                {
                                    await Task.Delay(250);
                                }
                                var fs = await file.OpenStreamForReadAsync();
                                var bs = new BufferedStream(fs);
                                stream = bs;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.WriteLine(LoggerLevel.Error, e, "ImageData: unable to open file");
                            // Assume we're ok
                            return true;
                        }
#endif
                    }
#if !WINUI
                    else if (uri.Scheme == "maui-appx")
                    {
                        var filePath = imgData.ImageUrl.ReplaceFirst("maui-appx://", "");

                        try
                        {
                            while (await FileSystem.Current.IsAppPackageFileLockedAsync(filePath))
                            {
                                await Task.Delay(250);
                            }
                            var fs = await FileSystemUtils.OpenAppPackageFileAsync(filePath);
                            var bs = new BufferedStream(fs);
                            stream = bs;
                        }
                        catch (Exception e)
                        {
                            Logger.WriteLine(LoggerLevel.Error, e, "ImageData: unable to open file");
                            // Assume we're ok
                            return true;
                        }
                    }
#endif
                    else
                    {
                        try
                        {
                            while (FileUtils.IsFileLocked(imgData.ImageUrl))
                            {
                                await Task.Delay(250);
                            }
                            var fs = File.OpenRead(imgData.ImageUrl);
                            var bs = new BufferedStream(fs);
                            stream = bs;
                        }
                        catch (Exception e)
                        {
                            Logger.WriteLine(LoggerLevel.Error, e, "ImageData: unable to open file");
                            // Assume we're ok
                            return true;
                        }
                    }

                    if (stream != null)
                    {
                        using (stream)
                        {
                            return ImageUtils.GuessImageType(stream) != ImageUtils.ImageType.Unknown;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
    }
}