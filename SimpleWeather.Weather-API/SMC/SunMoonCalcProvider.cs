using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace SimpleWeather.Weather_API.SMC
{
    public class SunMoonCalcProvider : IAstroDataProvider, IAstroDataProviderDate
    {
        private const string DATE_FORMAT = "yyyy/MM/dd HH:mm:ss 'UT'";

        public Task<Astronomy> GetAstronomyData(SimpleWeather.LocationData.LocationData location)
        {
            return GetAstronomyData(location, DateTimeOffset.UtcNow);
        }

        public async Task<Astronomy> GetAstronomyData(SimpleWeather.LocationData.LocationData location, DateTimeOffset date)
        {
            Astronomy astroData = new Astronomy();

            try
            {
                var utc = date.UtcDateTime;

                var smc = new SunMoonCalculator(utc.Year, utc.Month, utc.Day,
                    utc.Hour, utc.Minute, utc.Second,
                    location.longitude * SunMoonCalculator.DEG_TO_RAD, location.latitude * SunMoonCalculator.DEG_TO_RAD);

                smc.calcSunAndMoon();

                var sunrise = this.RunCatching(() =>
                {
                    return DateTime.ParseExact(SunMoonCalculator.getDateAsString(smc.sun.rise), DATE_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                }).GetOrElse(_ =>
                {
                    return DateTime.Now.Date.AddYears(1).AddTicks(-1);
                });

                var sunset = this.RunCatching(() =>
                {
                    return DateTime.ParseExact(SunMoonCalculator.getDateAsString(smc.sun.set), DATE_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                }).GetOrElse(_ =>
                {
                    return DateTime.Now.Date.AddYears(1).AddTicks(-1);
                });

                var moonrise = this.RunCatching(() =>
                {
                    return DateTime.ParseExact(SunMoonCalculator.getDateAsString(smc.moon.rise), DATE_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                }).GetOrElse(_ =>
                {
                    return DateTime.MinValue;
                });

                var moonset = this.RunCatching(() =>
                {
                    return DateTime.ParseExact(SunMoonCalculator.getDateAsString(smc.moon.set), DATE_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                }).GetOrElse(_ =>
                {
                    return DateTime.MinValue;
                });

                astroData.sunrise = sunrise.Let(dt =>
                {
                    if (dt > DateTime.MinValue && dt.TimeOfDay < DateTimeUtils.MaxTimeOfDay())
                    {
                        return dt.Add(location.tz_offset);
                    }
                    else
                    {
                        return dt;
                    }
                });
                astroData.sunset = sunset.Let(dt =>
                {
                    if (dt > DateTime.MinValue && dt.TimeOfDay < DateTimeUtils.MaxTimeOfDay())
                    {
                        return dt.Add(location.tz_offset);
                    }
                    else
                    {
                        return dt;
                    }
                });
                astroData.moonrise = moonrise.Let(dt =>
                {
                    if (dt > DateTime.MinValue && dt.TimeOfDay < DateTimeUtils.MaxTimeOfDay())
                    {
                        return dt.Add(location.tz_offset);
                    }
                    else
                    {
                        return dt;
                    }
                });
                astroData.moonset = moonset.Let(dt =>
                {
                    if (dt > DateTime.MinValue && dt.TimeOfDay < DateTimeUtils.MaxTimeOfDay())
                    {
                        return dt.Add(location.tz_offset);
                    }
                    else
                    {
                        return dt;
                    }
                });

                var moonPhaseType = GetMoonPhase(smc.moonAge);
                astroData.moonphase = new MoonPhase(moonPhaseType);
            }
            catch (Exception ex)
            {
                throw new WeatherException(WeatherUtils.ErrorStatus.Unknown, ex);
            }

            return astroData;
        }

        // Based on calculations from: https://github.com/mourner/suncalc
        private MoonPhase.MoonPhaseType GetMoonPhase(SunMoonCalculator.Ephemeris sun, SunMoonCalculator.Ephemeris moon)
        {
            double phi = Math.Acos(Math.Sin(sun.declination) * Math.Sin(moon.declination) + Math.Cos(sun.declination) * Math.Cos(moon.declination) * Math.Cos(sun.rightAscension - moon.rightAscension));
            double inc = Math.Atan2((sun.distance * SunMoonCalculator.AU) * Math.Sin(phi), (moon.distance * SunMoonCalculator.AU) - (sun.distance * SunMoonCalculator.AU) * Math.Cos(phi));
            double angle = Math.Atan2(Math.Cos(sun.declination) * Math.Sin(sun.rightAscension - moon.rightAscension), Math.Sin(sun.declination) * Math.Cos(moon.declination) - Math.Cos(sun.declination) * Math.Sin(moon.declination) * Math.Cos(sun.rightAscension - moon.rightAscension));
            double illuminationFraction = 0.5 + 0.5 * inc * (angle < 0 ? -1 : 1) / Math.PI;
            double phasePct = illuminationFraction * 100;

            MoonPhase.MoonPhaseType moonPhaseType;
            if (phasePct >= 2 && phasePct < 23)
            {
                moonPhaseType = MoonPhase.MoonPhaseType.WaxingCrescent;
            }
            else if (phasePct >= 23 && phasePct < 26)
            {
                moonPhaseType = MoonPhase.MoonPhaseType.FirstQtr;
            }
            else if (phasePct >= 26 && phasePct < 48)
            {
                moonPhaseType = MoonPhase.MoonPhaseType.WaxingGibbous;
            }
            else if (phasePct >= 48 && phasePct < 52)
            {
                moonPhaseType = MoonPhase.MoonPhaseType.FullMoon;
            }
            else if (phasePct >= 52 && phasePct < 73)
            {
                moonPhaseType = MoonPhase.MoonPhaseType.WaningGibbous;
            }
            else if (phasePct >= 73 && phasePct < 76)
            {
                moonPhaseType = MoonPhase.MoonPhaseType.LastQtr;
            }
            else if (phasePct >= 76 && phasePct < 98)
            {
                moonPhaseType = MoonPhase.MoonPhaseType.WaningCrescent;
            }
            else
            { // 0, 1, 98, 99, 100
                moonPhaseType = MoonPhase.MoonPhaseType.NewMoon;
            }

            return moonPhaseType;
        }

        private MoonPhase.MoonPhaseType GetMoonPhase(double moonAge)
        {
            const double moonCycle = 29.530588853;
            const double moonPhaseLength = (moonCycle / 8);
            const double newMoonStart = moonCycle - (moonPhaseLength / 2);
            const double newMoonEnd = moonPhaseLength / 2;
            const double waxingCrescentStart = newMoonEnd;
            const double waxingCrescentEnd = waxingCrescentStart + moonPhaseLength;
            const double firstQuarterStart = waxingCrescentEnd;
            const double firstQuarterEnd = firstQuarterStart + moonPhaseLength;
            const double waxingGibbousStart = firstQuarterEnd;
            const double waxingGibbousEnd = waxingGibbousStart + moonPhaseLength;
            const double fullMoonStart = waxingGibbousEnd;
            const double fullMoonEnd = fullMoonStart + moonPhaseLength;
            const double waningGibbousStart = fullMoonEnd;
            const double waningGibbousEnd = waningGibbousStart + moonPhaseLength;
            const double lastQuarterStart = waningGibbousEnd;
            const double lastQuarterEnd = lastQuarterStart + moonPhaseLength;
            const double waningCrescentStart = lastQuarterEnd;
            const double waningCrescentEnd = newMoonStart;

            MoonPhase.MoonPhaseType moonPhaseType;
            if ((moonAge >= waxingCrescentStart) && (moonAge <= waxingCrescentEnd))
            {
                moonPhaseType = MoonPhase.MoonPhaseType.WaxingCrescent;
            }
            else if ((moonAge >= firstQuarterStart) && (moonAge <= firstQuarterEnd))
            {
                moonPhaseType = MoonPhase.MoonPhaseType.FirstQtr;
            }
            else if ((moonAge >= waxingGibbousStart) && (moonAge <= waxingGibbousEnd))
            {
                moonPhaseType = MoonPhase.MoonPhaseType.WaxingGibbous;
            }
            else if ((moonAge >= fullMoonStart) && (moonAge <= fullMoonEnd))
            {
                moonPhaseType = MoonPhase.MoonPhaseType.FullMoon;
            }
            else if (moonAge >= waningGibbousStart && moonAge <= waningGibbousEnd)
            {
                moonPhaseType = MoonPhase.MoonPhaseType.WaningGibbous;
            }
            else if (moonAge >= lastQuarterStart && moonAge <= lastQuarterEnd)
            {
                moonPhaseType = MoonPhase.MoonPhaseType.LastQtr;
            }
            else if (moonAge >= waningCrescentStart && moonAge <= waningCrescentEnd)
            {
                moonPhaseType = MoonPhase.MoonPhaseType.WaningCrescent;
            }
            else
            {
                moonPhaseType = MoonPhase.MoonPhaseType.NewMoon;
            }

            return moonPhaseType;
        }
    }
}
