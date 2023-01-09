using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;

namespace SimpleWeather.Uno.Radar
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
