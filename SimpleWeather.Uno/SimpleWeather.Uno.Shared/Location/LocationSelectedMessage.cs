using System;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace SimpleWeather.Uno.Location
{
    public class LocationSelectedMessage : ValueChangedMessage<LocationData.LocationData>
    {
        public LocationSelectedMessage(LocationData.LocationData value) : base(value)
        {
        }
    }
}

