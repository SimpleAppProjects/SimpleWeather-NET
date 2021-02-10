using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleWeather.Utils
{
    public static partial class WeatherUtils
    {
        public static String GetWindDirection(float angle)
        {
            if (angle >= 348.75 && angle <= 11.25)
            {
                return SimpleLibrary.GetInstance().ResLoader.GetString("/WindDirection/wind_dir_n");
            }
            else if (angle >= 11.25 && angle <= 33.75)
            {
                return SimpleLibrary.GetInstance().ResLoader.GetString("/WindDirection/wind_dir_nne");
            }
            else if (angle >= 33.75 && angle <= 56.25)
            {
                return SimpleLibrary.GetInstance().ResLoader.GetString("/WindDirection/wind_dir_ne");
            }
            else if (angle >= 56.25 && angle <= 78.75)
            {
                return SimpleLibrary.GetInstance().ResLoader.GetString("/WindDirection/wind_dir_ene");
            }
            else if (angle >= 78.75 && angle <= 101.25)
            {
                return SimpleLibrary.GetInstance().ResLoader.GetString("/WindDirection/wind_dir_e");
            }
            else if (angle >= 101.25 && angle <= 123.75)
            {
                return SimpleLibrary.GetInstance().ResLoader.GetString("/WindDirection/wind_dir_ese");
            }
            else if (angle >= 123.75 && angle <= 146.25)
            {
                return SimpleLibrary.GetInstance().ResLoader.GetString("/WindDirection/wind_dir_se");
            }
            else if (angle >= 146.25 && angle <= 168.75)
            {
                return SimpleLibrary.GetInstance().ResLoader.GetString("/WindDirection/wind_dir_sse");
            }
            else if (angle >= 168.75 && angle <= 191.25)
            {
                return SimpleLibrary.GetInstance().ResLoader.GetString("/WindDirection/wind_dir_s");
            }
            else if (angle >= 191.25 && angle <= 213.75)
            {
                return SimpleLibrary.GetInstance().ResLoader.GetString("/WindDirection/wind_dir_ssw");
            }
            else if (angle >= 213.75 && angle <= 236.25)
            {
                return SimpleLibrary.GetInstance().ResLoader.GetString("/WindDirection/wind_dir_sw");
            }
            else if (angle >= 236.25 && angle <= 258.75)
            {
                return SimpleLibrary.GetInstance().ResLoader.GetString("/WindDirection/wind_dir_wsw");
            }
            else if (angle >= 258.75 && angle <= 281.25)
            {
                return SimpleLibrary.GetInstance().ResLoader.GetString("/WindDirection/wind_dir_w");
            }
            else if (angle >= 281.25 && angle <= 303.75)
            {
                return SimpleLibrary.GetInstance().ResLoader.GetString("/WindDirection/wind_dir_wnw");
            }
            else if (angle >= 303.75 && angle <= 326.25)
            {
                return SimpleLibrary.GetInstance().ResLoader.GetString("/WindDirection/wind_dir_nw");
            }
            else/* if (angle >= 326.25 && angle <= 348.75)*/
            {
                return SimpleLibrary.GetInstance().ResLoader.GetString("/WindDirection/wind_dir_nnw");
            }
        }

        /* Used by NWS */
        public static int GetWindDirection(String direction)
        {
            if ("N".Equals(direction, StringComparison.InvariantCulture))
            {
                return 0;
            }
            else if ("NNE".Equals(direction, StringComparison.InvariantCulture))
            {
                return 22;
            }
            else if ("NE".Equals(direction, StringComparison.InvariantCulture))
            {
                return 45;
            }
            else if ("ENE".Equals(direction, StringComparison.InvariantCulture))
            {
                return 67;
            }
            else if ("E".Equals(direction, StringComparison.InvariantCulture))
            {
                return 90;
            }
            else if ("ESE".Equals(direction, StringComparison.InvariantCulture))
            {
                return 112;
            }
            else if ("SE".Equals(direction, StringComparison.InvariantCulture))
            {
                return 135;
            }
            else if ("SSE".Equals(direction, StringComparison.InvariantCulture))
            {
                return 157;
            }
            else if ("S".Equals(direction, StringComparison.InvariantCulture))
            {
                return 180;
            }
            else if ("SSW".Equals(direction, StringComparison.InvariantCulture))
            {
                return 202;
            }
            else if ("SW".Equals(direction, StringComparison.InvariantCulture))
            {
                return 225;
            }
            else if ("WSW".Equals(direction, StringComparison.InvariantCulture))
            {
                return 247;
            }
            else if ("W".Equals(direction, StringComparison.InvariantCulture))
            {
                return 270;
            }
            else if ("WNW".Equals(direction, StringComparison.InvariantCulture))
            {
                return 292;
            }
            else if ("NW".Equals(direction, StringComparison.InvariantCulture))
            {
                return 315;
            }
            else
            {
                return 337;
            }
        }
    }
}
