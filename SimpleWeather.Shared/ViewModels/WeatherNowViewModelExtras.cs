using SimpleWeather.Controls;
using SimpleWeather.Utils;
using System.Threading.Tasks;

namespace SimpleWeather.ViewModels
{
    public static class WeatherNowViewModelExtras
    {
        public static Task<ImageDataViewModel> GetImageData(this WeatherUiModel model)
        {
            return model.WeatherData.GetImageData();
        }
    }
}
