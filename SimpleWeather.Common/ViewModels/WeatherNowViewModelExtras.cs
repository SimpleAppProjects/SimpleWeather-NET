using SimpleWeather.Common.Controls;
using SimpleWeather.Common.Images;
using System.Threading.Tasks;

namespace SimpleWeather.Common.ViewModels
{
    public static class WeatherNowViewModelExtras
    {
        public static Task<ImageDataViewModel> GetImageData(this WeatherUiModel model)
        {
            return model.WeatherData.GetImageData();
        }
    }
}
