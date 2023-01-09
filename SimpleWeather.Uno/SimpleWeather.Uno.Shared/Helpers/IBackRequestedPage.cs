using System.Threading.Tasks;

namespace SimpleWeather.Uno.Helpers
{
    interface IBackRequestedPage
    {
        Task<bool> OnBackRequested();
    }
}
