using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleWeather.Utils
{
    public static partial class WeatherUtils
    {
        public class AstroData
        {
            public DateTime sunriseUTC { get; set; }
            public DateTime sunsetUTC { get; set; }
        }

        public static AstroData GetSunriseSetTimeUTC(DateTimeOffset date, double lat, double lon)
        {
            var now = date;
            var mins = now.TimeOfDay.TotalMinutes; // time_local

            var jday = GetJD(now.Year, now.Month, now.Day);
            var total = jday + mins / 1440.0;
            var T = CalcTimeJulianCent(total);
            var azel = CalcAzEl(T, mins, lat, lon);
            var solnoon = CalcSolNoon(jday, lon);
            var rise = CalcSunriseSet(true, jday, lat, lon);
            var set = CalcSunriseSet(false, jday, lat, lon);

            var sunriseTime = timeString(rise.timeLocal, 2);
            var sunsetTime = timeString(set.timeLocal, 2);

            var astroData = new AstroData();

            if (DateTime.TryParse(sunriseTime, out DateTime sunrise))
                astroData.sunriseUTC = sunrise;
            if (DateTime.TryParse(sunsetTime, out DateTime sunset))
                astroData.sunsetUTC = sunset;

            return astroData;
        }

        private static double CalcSolNoon(double jd, double longitude)
        {
            var tnoon = CalcTimeJulianCent(jd - longitude / 360.0);

            var eqTime = CalcEquationOfTime(tnoon);

            var solNoonOffset = 720.0 - (longitude * 4) - eqTime; // in minutes

            var newt = CalcTimeJulianCent(jd + solNoonOffset / 1440.0);

            eqTime = CalcEquationOfTime(newt);

            var solNoonLocal = 720 - (longitude * 4) - eqTime; // in minutes

            while (solNoonLocal < 0.0)
            {
                solNoonLocal += 1440.0;
            }
            while (solNoonLocal >= 1440.0)
            {
                solNoonLocal -= 1440.0;
            }

            return solNoonLocal;
        }

        internal class SunriseSet
        {
            internal double jday { get; set; }
            internal double timeLocal { get; set; }
            internal double azimuth { get; set; }
        }

        // rise = 1 for sunrise, 0 for sunset
        private static SunriseSet CalcSunriseSet(bool rise, double JD, double latitude, double longitude)
        {
            var timeUTC = CalcSunriseSetUTC(rise, JD, latitude, longitude);
            var newTimeUTC = CalcSunriseSetUTC(rise, JD + timeUTC / 1440.0, latitude, longitude);
            var jday = JD;
            double timeLocal, azimuth;

            if (isNumber(newTimeUTC))
            {
                timeLocal = newTimeUTC;

                var riseT = CalcTimeJulianCent(JD + newTimeUTC / 1440.0);

                var riseAzEl = CalcAzEl(riseT, timeLocal, latitude, longitude);

                azimuth = riseAzEl.azimuth;

                if ((timeLocal < 0.0) || (timeLocal >= 1440.0))
                {
                    var increment = ((timeLocal < 0) ? 1 : -1);

                    while ((timeLocal < 0.0) || (timeLocal >= 1440.0))
                    {
                        timeLocal += increment * 1440.0;

                        jday -= increment;
                    }
                }
            }
            else
            { // no sunrise/set found
                azimuth = -1.0;

                timeLocal = 0.0;

                var doy = CalcDoyFromJD(JD);

                if (((latitude > 66.4) && (doy > 79) && (doy < 267)) ||
                     ((latitude < -66.4) && ((doy < 83) || (doy > 263))))
                {
                    //previous sunrise/next sunset
                    jday = CalcJDofNextPrevRiseSet(!rise, rise, JD, latitude, longitude);
                }
                else
                {   //previous sunset/next sunrise
                    jday = CalcJDofNextPrevRiseSet(rise, rise, JD, latitude, longitude);
                }
            }

            return new SunriseSet()
            {
                jday = jday,
                timeLocal = timeLocal,
                azimuth = azimuth
            };
        }

        private static double CalcJDofNextPrevRiseSet(bool next, bool rise, double JD, double latitude, double longitude)
        {
            var julianday = JD;
            var increment = ((next) ? 1.0 : -1.0);
            var time = CalcSunriseSetUTC(rise, julianday, latitude, longitude);

            while (!isNumber(time))
            {
                julianday += increment;
                time = CalcSunriseSetUTC(rise, julianday, latitude, longitude);
            }
            var timeLocal = time;

            while ((timeLocal < 0.0) || (timeLocal >= 1440.0))
            {
                var incr = ((timeLocal < 0) ? 1 : -1);

                timeLocal += (incr * 1440.0);

                julianday -= incr;
            }

            return julianday;
        }

        private static double CalcDoyFromJD(double jd)
        {
            var date = CalcDateFromJD(jd);

            var k = (IsLeapYear(date.Year) ? 1 : 2);
            var doy = Math.Floor(((double)275 * date.Month) / 9) - k * Math.Floor(((double)date.Month + 9) / 12) + date.Day - 30;

            return doy;
        }

        private static bool IsLeapYear(int yr)
        {
            return ((yr % 4 == 0 && yr % 100 != 0) || yr % 400 == 0);
        }

        private static DateTime CalcDateFromJD(double jd)
        {
            var z = Math.Floor(jd + 0.5);
            var f = (jd + 0.5) - z;
            double A;
            if (z < 2299161)
            {
                A = z;
            }
            else
            {
                var alpha = Math.Floor((z - 1867216.25) / 36524.25);
                A = z + 1 + alpha - Math.Floor(alpha / 4);
            }
            var B = A + 1524;
            var C = Math.Floor((B - 122.1) / 365.25);
            var D = Math.Floor(365.25 * C);
            var E = Math.Floor((B - D) / 30.6001);
            var day = B - D - Math.Floor(30.6001 * E) + f;
            var month = (E < 14) ? E - 1 : E - 13;
            var year = (month > 2) ? C - 4716 : C - 4715;

            return new DateTime((int)year, (int)month, (int)day);
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

        internal class AzEl
        {
            internal double azimuth { get; set; }
            internal double elevation { get; set; }
        }

        private static AzEl CalcAzEl(double T, double localtime, double lat, double lon)
        {
            var eqTime = CalcEquationOfTime(T);
            var theta = CalcSunDeclination(T);

            var solarTimeFix = eqTime + 4.0 * lon;
            var earthRadVec = CalcSunRadVector(T);
            var trueSolarTime = localtime + solarTimeFix;
            while (trueSolarTime > 1440)
            {
                trueSolarTime -= 1440;
            }
            var hourAngle = trueSolarTime / 4.0 - 180.0;
            if (hourAngle < -180)
            {
                hourAngle += 360.0;
            }
            var haRad = ConversionMethods.ToRadians(hourAngle);
            var csz = Math.Sin(ConversionMethods.ToRadians(lat)) * Math.Sin(ConversionMethods.ToRadians(theta))
                 + Math.Cos(ConversionMethods.ToRadians(lat)) * Math.Cos(ConversionMethods.ToRadians(theta)) * Math.Cos(haRad);
            if (csz > 1.0)
            {
                csz = 1.0;
            }
            else if (csz < -1.0)
            {
                csz = -1.0;
            }
            var zenith = ConversionMethods.ToDegrees(Math.Acos(csz));
            var azDenom = Math.Cos(ConversionMethods.ToRadians(lat)) * Math.Sin(ConversionMethods.ToRadians(zenith));
            double azimuth;
            if (Math.Abs(azDenom) > 0.001)
            {
                var azRad = (Math.Sin(ConversionMethods.ToRadians(lat)) * Math.Cos(ConversionMethods.ToRadians(zenith))) -
                    Math.Sin(ConversionMethods.ToRadians(theta)) / azDenom;
                if (Math.Abs(azRad) > 1.0)
                {
                    if (azRad < 0)
                    {
                        azRad = -1.0;
                    }
                    else
                    {
                        azRad = 1.0;
                    }
                }
                azimuth = 180.0 - ConversionMethods.ToDegrees(Math.Acos(azRad));
                if (hourAngle > 0.0)
                {
                    azimuth = -azimuth;
                }
            }
            else
            {
                if (lat > 0.0)
                {
                    azimuth = 180.0;
                }
                else
                {
                    azimuth = 0.0;
                }
            }
            if (azimuth < 0.0)
            {
                azimuth += 360.0;
            }
            var exoatmElevation = 90.0 - zenith;

            // Atmospheric Refraction correction
            var refractionCorrection = CalcRefraction(exoatmElevation);

            var solarZen = zenith - refractionCorrection;
            var elevation = 90.0 - solarZen;

            return new AzEl
            {
                azimuth = azimuth,
                elevation = elevation
            };
        }

        private static double CalcSunRadVector(double t)
        {
            var v = CalcSunTrueAnomaly(t);
            var e = CalcEccentricityEarthOrbit(t);
            var R = (1.000001018 * (1 - e * e)) / (1 + e * Math.Cos(ConversionMethods.ToRadians(v)));
            return R;		// in AUs
        }

        private static double CalcSunTrueAnomaly(double t)
        {
            var m = CalcGeomMeanAnomalySun(t);
            var c = CalcSunEqOfCenter(t);
            var v = m + c;
            return v;		// in degrees
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

        private static double CalcRefraction(double elev)
        {
            double correction;

            if (elev > 85.0)
            {
                correction = 0.0;
            }
            else
            {
                var te = Math.Tan(ConversionMethods.ToRadians(elev));
                if (elev > 5.0)
                {
                    correction = 58.1 / te - 0.07 / (te * te * te) + 0.000086 / (te * te * te * te * te);
                }
                else if (elev > -0.575)
                {
                    correction = 1735.0 + elev * (-518.2 + elev * (103.4 + elev * (-12.79 + elev * 0.711)));
                }
                else
                {
                    correction = -20.774 / te;
                }
                correction /= 3600.0;
            }

            return correction;
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

        private static bool isNumber(double inputVal)
        {
            var oneDecimal = false;
            var inputStr = "" + inputVal;
            for (var i = 0; i < inputStr.Length; i++)
            {
                var oneChar = inputStr[i];
                if (i == 0 && (oneChar == '-' || oneChar == '+'))
                {
                    continue;
                }
                if (oneChar == '.' && !oneDecimal)
                {
                    oneDecimal = true;
                    continue;
                }
                if (oneChar < '0' || oneChar > '9')
                {
                    return false;
                }
            }
            return true;
        }

        private static String timeString(double minutes, int flag)
        {
            String output;

            if ((minutes >= 0) && (minutes < 1440))
            {
                var floatHour = minutes / 60.0;
                var hour = Math.Floor(floatHour);
                var floatMinute = 60.0 * (floatHour - Math.Floor(floatHour));
                var minute = Math.Floor(floatMinute);
                var floatSec = 60.0 * (floatMinute - Math.Floor(floatMinute));
                var second = Math.Floor(floatSec + 0.5);
                if (second > 59)
                {
                    second = 0;

                    minute += 1;
                }
                if ((flag == 2) && (second >= 30)) minute++;
                if (minute > 59)
                {
                    minute = 0;

                    hour += 1;
                }
                output = zeroPad(hour, 2) + ":" + zeroPad(minute, 2);
                if (flag > 2) output = output + ":" + zeroPad(second, 2);
            }
            else
            {
                output = "error";
            }

            return output;
        }

        private static string zeroPad(double n, int digits)
        {
            var nStr = n.ToString();
            while (nStr.Length < digits)
            {
                nStr = '0' + nStr;
            }
            return nStr;
        }
    }
}