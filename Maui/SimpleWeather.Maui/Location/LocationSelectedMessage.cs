using CommunityToolkit.Mvvm.Messaging.Messages;
using SimpleWeather.Common.Location;
using SimpleWeather.Common.ViewModels;

namespace SimpleWeather.Maui.Location
{
    public class LocationSelectedMessage : ValueChangedMessage<LocationSearchResult>
    {
        public LocationSelectedMessage(LocationSearchResult result) : base(result)
        {
        }
    }
}
