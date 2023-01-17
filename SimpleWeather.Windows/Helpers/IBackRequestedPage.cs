using System.Threading.Tasks;

namespace SimpleWeather.NET.Helpers
{
    interface IBackRequestedPage
    {
        Task<bool> OnBackRequested();
    }
}
