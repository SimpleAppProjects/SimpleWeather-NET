using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.Utils
{
    public static class CommonActions
    {
        public const string ACTION_WEATHER_UPDATETILELOCATION = "SimpleWeather.action.WEATHER_UPDATETILELOCATION";
        public const string ACTION_IMAGES_UPDATETASK = "SimpleWeather.action.IMAGES_UPDATE_TASK";

        public const string ACTION_SETTINGS_UPDATEAPI = "SimpleWeather.action.SETTINGS_UPDATEAPI";
        public const string ACTION_SETTINGS_UPDATEGPS = "SimpleWeather.action.SETTINGS_UPDATEGPS";
        public const string ACTION_SETTINGS_UPDATEUNIT = "SimpleWeather.action.SETTINGS_UPDATEUNIT";
        public const string ACTION_SETTINGS_UPDATEREFRESH = "SimpleWeather.action.SETTINGS_UPDATEREFRESH";
        public const string ACTION_WEATHER_UPDATE = "SimpleWeather.action.WEATHER_UPDATE";
        public const string ACTION_WEATHER_REREGISTERTASK = "SimpleWeather.action.WEATHER_REREGISTERTASK";

        public const string ACTION_WEATHER_SENDLOCATIONUPDATE = "SimpleWeather.action.WEATHER_SENDLOCATIONUPDATE";

        public const string ACTION_SETTINGS_UPDATEDAILYNOTIFICATION = "SimpleWeather.action.SETTINGS_UPDATEDAILYNOTIFICATION";

        public const string ACTION_LOCALE_CHANGED = "SimpleWeather.action.LOCALE_CHANGED";

        public const string EXTRA_FORCEUPDATE = "SimpleWeather.extra.FORCE_UPDATE";
    }
}
