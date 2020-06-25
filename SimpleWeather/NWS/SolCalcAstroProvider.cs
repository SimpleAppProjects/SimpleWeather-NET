using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.NWS
{
    public class SolCalcAstroProvider : IAstroDataProvider, IAstroDataProviderDate
    {
        // Calculations from NOAA Solar Calculator: https://www.esrl.noaa.gov/gmd/grad/solcalc/
        public Task<Astronomy> GetAstronomyData(LocationData location)
        {
            return GetAstronomyData(location, DateTimeOffset.UtcNow);
        }

        public Task<Astronomy> GetAstronomyData(LocationData location, DateTimeOffset date)
        {
            return Task.Run(() =>
            {
                var astroData = GetSunriseSetTimeUTC(date, location.latitude, location.longitude);
                return new Astronomy()
                {
                    sunrise = astroData.sunriseUTC.Add(location.tz_offset),
                    sunset = astroData.sunsetUTC.Add(location.tz_offset)
                };
            });
        }

        internal class AstroData
        {
            public DateTime sunriseUTC { get; set; }
            public DateTime sunsetUTC { get; set; }
        }

        // Calculations from NOAA: https://www.esrl.noaa.gov/gmd/grad/solcalc/
        private static AstroData GetSunriseSetTimeUTC(DateTimeOffset date, double lat, double lon)
        {
            var now = date;

            var jday = GetJD(now.Year, now.Month, now.Day);
            var rise = CalcSunriseSet(true, jday, lat, lon);
            var set = CalcSunriseSet(false, jday, lat, lon);

            var sunriseTime = TimeSpan.FromMinutes(rise);
            if (sunriseTime.Seconds >= 30)
            {
                // Round up to the next minute
                sunriseTime = sunriseTime.Add(TimeSpan.FromMinutes(1))
                    .Subtract(TimeSpan.FromSeconds(sunriseTime.Seconds));
            }

            var sunsetTime = TimeSpan.FromMinutes(set);
            if (sunsetTime.Seconds >= 30)
            {
                // Round up to the next minute
                sunsetTime = sunsetTime.Add(TimeSpan.FromMinutes(1))
                    .Subtract(TimeSpan.FromSeconds(sunsetTime.Seconds));
            }

            var astroData = new AstroData
            {
                // Add time in minutes
                sunriseUTC = now.Date.Add(sunriseTime),
                sunsetUTC = now.Date.Add(sunsetTime)
            };

            return astroData;
        }

        // rise = 1 for sunrise, 0 for sunset
        private static double CalcSunriseSet(bool rise, double JD, double latitude, double longitude)
        {
            var timeUTC = CalcSunriseSetUTC(rise, JD, latitude, longitude);	// in minutes
            var newTimeUTC = CalcSunriseSetUTC(rise, JD + timeUTC / 1440.0, latitude, longitude);	// in minutes

            return newTimeUTC;
        }

        private static double CalcSunriseSetUTC(bool rise, double JD, double latitude, double longitude)
        {
            var t = CalcTimeJulianCent(JD);
            var eqTime = CalcEquationOfTime(t);
            var solarDec = CalcSunDeclination(t);
            var hourAngle = CalcHourAngleSunrise(latitude, solarDec);
            if (!rise) hourAngle = -hourAngle;
            var delta = longitude + ConversionMethods.ToDegrees(hourAngle);
            var timeUTC = 720 - (4.0 * delta) - eqTime; // in minutes

            return timeUTC;
        }

        private static double CalcHourAngleSunrise(double lat, double solarDec)
        {
            var latRad = ConversionMethods.ToRadians(lat);
            var sdRad = ConversionMethods.ToRadians(solarDec);
            var HAarg = (Math.Cos(ConversionMethods.ToRadians(90.833)) / (Math.Cos(latRad) * Math.Cos(sdRad)) - Math.Tan(latRad) * Math.Tan(sdRad));
            var HA = Math.Acos(HAarg);
            return HA;		// in radians (for sunset, use -HA)
        }

        private static double GetJD(int year, int month, int day)
        {
            if (month <= 2)
            {
                year -= 1;
                month += 12;
            }
            var A = Math.Floor((double)year / 100);
            var B = 2 - A + Math.Floor(A / 4);
            var JD = Math.Floor(365.25 * (year + 4716)) + Math.Floor(30.6001 * (month + 1)) + day + B - 1524.5;
            return JD;
        }

        private static double CalcTimeJulianCent(double jd)
        {
            return (jd - 2451545.0) / 36525.0;
        }

        private static double CalcSunDeclination(double t)
        {
            var e = CalcObliquityCorrection(t);
            var lambda = CalcSunApparentLong(t);
            var sint = Math.Sin(ConversionMethods.ToRadians(e)) * Math.Sin(ConversionMethods.ToRadians(lambda));
            var theta = ConversionMethods.ToDegrees(Math.Asin(sint));
            return theta;		// in degrees
        }

        private static double CalcSunApparentLong(double t)
        {
            var o = CalcSunTrueLong(t);
            var omega = 125.04 - 1934.136 * t;
            var lambda = o - 0.00569 - 0.00478 * Math.Sin(ConversionMethods.ToRadians(omega));
            return lambda;		// in degrees
        }

        private static double CalcSunTrueLong(double t)
        {
            var l0 = CalcGeomMeanLongSun(t);
            var c = CalcSunEqOfCenter(t);
            var O = l0 + c;
            return O;		// in degrees
        }

        private static double CalcSunEqOfCenter(double t)
        {
            var m = CalcGeomMeanAnomalySun(t);
            var mrad = ConversionMethods.ToRadians(m);
            var sinm = Math.Sin(mrad);
            var sin2m = Math.Sin(mrad + mrad);
            var sin3m = Math.Sin(mrad + mrad + mrad);
            var C = sinm * (1.914602 - t * (0.004817 + 0.000014 * t)) + sin2m * (0.019993 - 0.000101 * t) + sin3m * 0.000289;
            return C;		// in degrees
        }

        private static double CalcEquationOfTime(double t)
        {
            var epsilon = CalcObliquityCorrection(t);
            var l0 = CalcGeomMeanLongSun(t);
            var e = CalcEccentricityEarthOrbit(t);
            var m = CalcGeomMeanAnomalySun(t);

            var y = Math.Tan(ConversionMethods.ToRadians(epsilon) / 2.0);
            y *= y;

            var sin2l0 = Math.Sin(2.0 * ConversionMethods.ToRadians(l0));
            var sinm = Math.Sin(ConversionMethods.ToRadians(m));
            var cos2l0 = Math.Cos(2.0 * ConversionMethods.ToRadians(l0));
            var sin4l0 = Math.Sin(4.0 * ConversionMethods.ToRadians(l0));
            var sin2m = Math.Sin(2.0 * ConversionMethods.ToRadians(m));

            var Etime = y * sin2l0 - 2.0 * e * sinm + 4.0 * e * y * sinm * cos2l0 - 0.5 * y * y * sin4l0 - 1.25 * e * e * sin2m;
            return ConversionMethods.ToDegrees(Etime) * 4.0;	// in minutes of time
        }

        private static double CalcGeomMeanAnomalySun(double t)
        {
            var M = 357.52911 + t * (35999.05029 - 0.0001537 * t);
            return M;		// in degrees
        }

        private static double CalcEccentricityEarthOrbit(double t)
        {
            var e = 0.016708634 - t * (0.000042037 + 0.0000001267 * t);
            return e;		// unitless
        }

        private static double CalcGeomMeanLongSun(double t)
        {
            var L0 = 280.46646 + t * (36000.76983 + t * (0.0003032));

            while (L0 > 360.0)
            {
                L0 -= 360.0;
            }
            while (L0 < 0.0)
            {
                L0 += 360.0;
            }
            return L0;		// in degrees
        }

        private static double CalcObliquityCorrection(double t)
        {
            var e0 = CalcMeanObliquityOfEcliptic(t);
            var omega = 125.04 - 1934.136 * t;
            var e = e0 + 0.00256 * Math.Cos(ConversionMethods.ToRadians(omega));
            return e;		// in degrees
        }

        private static double CalcMeanObliquityOfEcliptic(double t)
        {
            var seconds = 21.448 - t * (46.8150 + t * (0.00059 - t * (0.001813)));
            var e0 = 23.0 + (26.0 + (seconds / 60.0)) / 60.0;
            return e0;		// in degrees
        }
    }
}