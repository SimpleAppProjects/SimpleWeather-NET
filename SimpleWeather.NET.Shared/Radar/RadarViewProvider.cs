using SimpleWeather.Utils;
#if WINDOWS
using Border = Microsoft.UI.Xaml.Controls.Border;
#endif

namespace SimpleWeather.NET.Radar
{
    public abstract class RadarViewProvider
    {
        protected Border RadarContainer { get; private set; }
        private bool enableInteractions = true;

        public RadarViewProvider(Border container)
        {
            this.RadarContainer = container;
        }

        public abstract void UpdateCoordinates(WeatherUtils.Coordinate coordinates, bool updateView = false);

        public abstract void UpdateRadarView();

        public void EnableInteractions(bool enable)
        {
            enableInteractions = enable;
        }

        public bool InteractionsEnabled()
        {
            return enableInteractions;
        }

        public virtual void OnDestroyView()
        {

        }
    }
}
