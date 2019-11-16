using System.Threading.Tasks;

namespace SimpleWeather.UWP.Helpers
{
    interface IBackRequestedPage
    {
        Task<bool> OnBackRequested();
    }
}
