using System;
using SimpleWeather.Weather_API;
using SimpleWeather.WeatherData;
using SimpleWeather.WeatherData.Images.Model;

namespace SimpleWeather.Maui.Images;

public static class ImageDataUtils
{
    public static ImageData GetDefaultImageData(string backgroundCode, Weather weather)
    {
        var wm = WeatherModule.Instance.WeatherManager;

        // Fallback to assets
        // day, night, rain, snow
        var imageData = new ImageData();

#if WINDOWS
        var basePath = "ms-appx:///SimpleWeather.Shared/Resources/Images/Backgrounds/";
#else
        var basePath = "maui-appx://Images/Backgrounds/";
#endif

        switch (backgroundCode)
        {
            case WeatherBackground.SNOW:
            case WeatherBackground.SNOW_WINDY:
                imageData.ImageUrl = $"{basePath}snow.jpg";
                imageData.HexColor = "#ffb8d0f0";
                break;
            case WeatherBackground.RAIN:
            case WeatherBackground.RAIN_NIGHT:
                imageData.ImageUrl = $"{basePath}rain.jpg";
                imageData.HexColor = "#ff102030";
                break;
            case WeatherBackground.TSTORMS_DAY:
            case WeatherBackground.TSTORMS_NIGHT:
            case WeatherBackground.STORMS:
                imageData.ImageUrl = $"{basePath}storms.jpg";
                imageData.HexColor = "#ff182830";
                break;
            case WeatherBackground.FOG:
                imageData.ImageUrl = $"{basePath}fog.jpg";
                imageData.HexColor = "#ff808080";
                break;
            case WeatherBackground.PARTLYCLOUDY_DAY:
                imageData.ImageUrl = $"{basePath}day_partlycloudy.jpg";
                imageData.HexColor = "#ff88b0c8";
                break;
            case WeatherBackground.PARTLYCLOUDY_NIGHT:
                imageData.ImageUrl = $"{basePath}night_partlycloudy.jpg";
                imageData.HexColor = "#ff182020";
                break;
            case WeatherBackground.MOSTLYCLOUDY_DAY:
                imageData.ImageUrl = $"{basePath}day_cloudy.jpg";
                imageData.HexColor = "#ff88b0c8";
                break;
            case WeatherBackground.MOSTLYCLOUDY_NIGHT:
                imageData.ImageUrl = $"{basePath}night_cloudy.jpg";
                imageData.HexColor = "#ff182020";
                break;
            default:
                if (wm.IsNight(weather))
                {
                    //case WeatherBackground.NIGHT:
                    imageData.ImageUrl = $"{basePath}night.jpg";
                    imageData.HexColor = "#ff182020";
                }
                else
                {
                    //case WeatherBackground.DAY:
                    imageData.ImageUrl = $"{basePath}day.jpg";
                    imageData.HexColor = "#ff88b0c8";
                }
                break;
        }

        return imageData;
    }
}