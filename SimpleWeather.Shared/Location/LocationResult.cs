using SimpleWeather.ViewModels;

namespace SimpleWeather.Location
{
    public interface LocationResult
    {
        LocationData Data { get; }
        bool LocationChanged { get; }

        public sealed record Changed(LocationData Data, bool LocationChanged = true) : LocationResult;

        public sealed record ChangedInvalid(LocationData Data, bool LocationChanged = true) : LocationResult;

        public sealed record NotChanged(LocationData Data, bool LocationChanged = false) : LocationResult;

        public sealed record PermissionDenied(LocationData Data = null, bool LocationChanged = false) : LocationResult;

        public sealed record Error(ErrorMessage ErrorMessage, LocationData Data = null, bool LocationChanged = false) : LocationResult;
    }
}
