using SimpleWeather.Common.Utils;

namespace SimpleWeather.Common.Location
{
    public interface LocationResult
    {
        LocationData.LocationData Data { get; }
        bool LocationChanged { get; }

        public sealed record Changed(LocationData.LocationData Data, bool LocationChanged = true) : LocationResult;

        public sealed record ChangedInvalid(LocationData.LocationData Data, bool LocationChanged = true) : LocationResult;

        public sealed record NotChanged(LocationData.LocationData Data, bool LocationChanged = false) : LocationResult;

        public sealed record PermissionDenied(LocationData.LocationData Data = null, bool LocationChanged = false) : LocationResult;

        public sealed record Error(ErrorMessage ErrorMessage, LocationData.LocationData Data = null, bool LocationChanged = false) : LocationResult;
    }
}
