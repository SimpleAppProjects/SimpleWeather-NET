using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace SimpleWeather.Utils
{
    /**
     * AQI Calculations based on AirNow AQI Calculator
     * https://www.airnow.gov/aqi/aqi-calculator-concentration/
     */
    public static class AirQualityUtils
    {
        public static double CO_ugm3_TO_ppm(double value)
        {
            // 1ppm = 1.15mg/m3 = 1150ug/m3 (CO)
            return value / 1150;
        }

        public static double SO2_ugm3_to_ppb(double value)
        {
            // 1ppb = 2.62ug/m3 (SO2)
            return value / 2.62;
        }

        public static double NO2_ugm3_to_ppb(double value)
        {
            // 1ppb = 1.88ug/m3 (NO2)
            return value / 1.88;
        }

        public static double NO_ugm3_to_ppb(double value)
        {
            // 1ppb = 1.25ug/m3 (NO)
            return value / 1.88;
        }

        public static double O3_ugm3_to_ppb(double value)
        {
            // 1ppb = 2.00ug/m3 (O3)
            return value / 2.00;
        }

        private static int AQICalc(double aqiHi, double aqiLo, double concHi, double concLo, double concVal)
        {
            var aqiValue = ((concVal - concLo) / (concHi - concLo)) * (aqiHi - aqiLo) + aqiLo;
            return (int)Math.Round(aqiValue);
        }

        public static int AQIO3(double value)
        {
            if (Math.Floor(value) <= 200)
            {
                return AQIO3_8hr(value);
            }
            else
            {
                return AQIO3_1hr(value);
            }
        }

        public static int AQIO3_8hr(double value)
        {
            var conc = Math.Floor(value) / 1000;

            if (conc >= 0 && conc < 0.055D)
            {
                return AQICalc(50.0D, 0.0D, 0.054D, 0.0D, conc);
            }
            else if (conc >= 0.055D && conc < 0.071D)
            {
                return AQICalc(100.0D, 51.0D, 0.07D, 0.055D, conc);
            }
            else if (conc >= 0.071D && conc < 0.086D)
            {
                return AQICalc(150.0D, 101.0D, 0.085D, 0.071D, conc);
            }
            else if (conc >= 0.086D && conc < 0.106D)
            {
                return AQICalc(200.0D, 151.0D, 0.105D, 0.086D, conc);
            }
            else if (conc >= 0.106D && conc < 0.201D)
            {
                return AQICalc(300.0D, 201.0D, 0.2D, 0.106D, conc);
            }
            else if (conc >= 0.201D && conc < 0.605D)
            {
                throw new ArgumentOutOfRangeException(nameof(conc), "8-hour ozone values do not define higher AQI values (>=301); calculate using 1-hour O3 conc");
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(conc), "value out of range");
            }
        }

        public static int AQIO3_1hr(double value)
        {
            double conc = Math.Floor(value) / 1000;

            if (conc >= 0.0D && conc <= 124.0D)
            {
                throw new ArgumentOutOfRangeException(nameof(conc), "1-hour ozone values do not define lower AQI values (<= 100); AQI values of 100 or lower are calculated with 8-hour ozone concentrations.");
            }
            else if (conc >= 0.125D && conc < 0.165D)
            {
                return AQICalc(150.0D, 101.0D, 0.164D, 0.125D, conc);
            }
            else if (conc >= 0.165D && conc < 0.205D)
            {
                return AQICalc(200.0D, 151.0D, 0.204D, 0.165D, conc);
            }
            else if (conc >= 0.205D && conc < 0.405D)
            {
                return AQICalc(300.0D, 201.0D, 0.404D, 0.205D, conc);
            }
            else if (conc >= 0.405D && conc < 0.505D)
            {
                return AQICalc(400.0D, 301.0D, 0.504D, 0.405D, conc);
            }
            else if (conc >= 0.505D && conc < 0.605D)
            {
                return AQICalc(500.0D, 401.0D, 0.604D, 0.505D, conc);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(conc), "value out of range");
            }
        }

        public static int AQIPM2_5(double value)
        {
            double conc = Math.Floor(value * 10) / 10;

            if (conc >= 0 && conc < 12.1D)
            {
                return AQICalc(50.0D, 0.0D, 12.0D, 0.0D, conc);
            }
            else if (conc >= 12.1D && conc < 35.5D)
            {
                return AQICalc(100.0D, 51.0D, 35.4D, 12.1D, conc);
            }
            else if (conc >= 35.5D && conc < 55.5D)
            {
                return AQICalc(150.0D, 101.0D, 55.4D, 35.5D, conc);
            }
            else if (conc >= 55.5D && conc < 150.5D)
            {
                return AQICalc(200.0D, 151.0D, 150.4D, 55.5D, conc);
            }
            else if (conc >= 150.5D && conc < 250.5D)
            {
                return AQICalc(300.0D, 201.0D, 250.4D, 150.5D, conc);
            }
            else if (conc >= 250.5D && conc < 350.5D)
            {
                return AQICalc(400.0D, 301.0D, 350.4D, 250.5D, conc);
            }
            else if (conc >= 350.5D && conc < 500.5D)
            {
                return AQICalc(500.0D, 401.0D, 500.4D, 350.5D, conc);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(conc), "value out of range");
            }
        }

        public static int AQIPM10(double value)
        {
            double conc = Math.Floor(value * 10) / 10;

            if (conc >= 0 && conc < 12.1D)
            {
                return AQICalc(50.0D, 0.0D, 12.0D, 0.0D, conc);
            }
            else if (conc >= 12.1D && conc < 35.5D)
            {
                return AQICalc(100.0D, 51.0D, 35.4D, 12.1D, conc);
            }
            else if (conc >= 35.5D && conc < 55.5D)
            {
                return AQICalc(150.0D, 101.0D, 55.4D, 35.5D, conc);
            }
            else if (conc >= 55.5D && conc < 150.5D)
            {
                return AQICalc(200.0D, 151.0D, 150.4D, 55.5D, conc);
            }
            else if (conc >= 150.5D && conc < 250.5D)
            {
                return AQICalc(300.0D, 201.0D, 250.4D, 150.5D, conc);
            }
            else if (conc >= 250.5D && conc < 350.5D)
            {
                return AQICalc(400.0D, 301.0D, 350.4D, 250.5D, conc);
            }
            else if (conc >= 350.5D && conc < 500.5D)
            {
                return AQICalc(500.0D, 401.0D, 500.4D, 350.5D, conc);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(conc), "value out of range");
            }
        }

        public static int AQICO(double value)
        {
            var conc = Math.Floor(10 * value) / 10;

            if (conc >= 0 && conc < 4.5D)
            {
                return AQICalc(50.0D, 0.0D, 4.4D, 0.0D, conc);
            }
            else if (conc >= 4.5D && conc < 9.5D)
            {
                return AQICalc(100.0D, 51.0D, 9.4D, 4.5D, conc);
            }
            else if (conc >= 9.5D && conc < 12.5D)
            {
                return AQICalc(150.0D, 101.0D, 12.4D, 9.5D, conc);
            }
            else if (conc >= 12.5D && conc < 15.5D)
            {
                return AQICalc(200.0D, 151.0D, 15.4D, 12.5D, conc);
            }
            else if (conc >= 15.5D && conc < 30.5D)
            {
                return AQICalc(300.0D, 201.0D, 30.4D, 15.5D, conc);
            }
            else if (conc >= 30.5D && conc < 40.5D)
            {
                return AQICalc(400.0D, 301.0D, 40.4D, 30.5D, conc);
            }
            else if (conc >= 40.5D && conc < 50.5D)
            {
                return AQICalc(500.0D, 401.0D, 50.4D, 40.5D, conc);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(conc), "value out of range");
            }
        }

        public static int AQISO2(double value)
        {
            if (Math.Floor(value) >= 305)
            {
                return AQISO2_24hr(value);
            }
            else
            {
                return AQISO2_1hr(value);
            }
        }

        public static int AQISO2_1hr(double value)
        {
            double conc = Math.Floor(value);

            if (conc >= 0 && conc < 36)
            {
                return AQICalc(50.0D, 0.0D, 35.0D, 0.0D, conc);
            }
            else if (conc >= 36 && conc < 76)
            {
                return AQICalc(100.0D, 51.0D, 75.0D, 36.0D, conc);
            }
            else if (conc >= 76 && conc < 186)
            {
                return AQICalc(150.0D, 101.0D, 185.0D, 76.0D, conc);
            }
            else if (conc >= 186 && conc < 305)
            {
                return AQICalc(200.0D, 151.0D, 304.0D, 186.0D, conc);
            }
            else if (conc >= 305.0D && conc <= 604.0D)
            {
                throw new ArgumentOutOfRangeException(nameof(conc), "AQI values of 201 or greater are calculated with 24-hour SO2 concentrations");
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(conc), "value out of range");
            }
        }

        public static int AQISO2_24hr(double value)
        {
            double conc = Math.Floor(value);

            if (conc >= 0 && conc <= 304)
            {
                throw new ArgumentOutOfRangeException(nameof(conc), "AQI values less than 201 are calculated with 1-hour SO2 concentrations");
            }
            else if (conc >= 305 && conc < 605)
            {
                return AQICalc(300.0D, 201.0D, 604.0D, 305.0D, conc);
            }
            else if (conc >= 605 && conc < 804)
            {
                return AQICalc(400.0D, 301.0D, 804.0D, 605.0D, conc);
            }
            else if (conc >= 805 && conc < 1005)
            {
                return AQICalc(500.0D, 401.0D, 1004.0D, 805.0D, conc);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(conc), "value out of range");
            }
        }

        public static int AQINO2(double value)
        {
            double conc = Math.Floor(value) / 1000;

            if (conc >= 0 && conc < 0.054D)
            {
                return AQICalc(50.0D, 0.0D, 0.053D, 0.0D, conc);
            }
            else if (conc >= 0.054D && conc < 0.101D)
            {
                return AQICalc(100.0D, 51.0D, 0.1D, 0.054D, conc);
            }
            else if (conc >= 0.101D && conc < 0.361D)
            {
                return AQICalc(150.0D, 101.0D, 0.36D, 0.101D, conc);
            }
            else if (conc >= 0.361D && conc < 0.65D)
            {
                return AQICalc(200.0D, 151.0D, 0.649D, 0.361D, conc);
            }
            else if (conc >= 0.65D && conc < 1.25D)
            {
                return AQICalc(300.0D, 201.0D, 1.249D, 0.65D, conc);
            }
            else if (conc >= 1.25D && conc < 1.65D)
            {
                return AQICalc(400.0D, 301.0D, 1.649D, 1.25D, conc);
            }
            else if (conc >= 1.65D && conc <= 2.049D)
            {
                return AQICalc(500.0D, 401.0D, 2.049D, 1.65D, conc);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(conc), "value out of range");
            }
        }

        public static int? GetIndexFromData(this WeatherData.AirQuality aqi)
        {
            var idx = NumberUtils.MaxOf(
                    aqi.no2 ?? -1,
                    aqi.o3 ?? -1,
                    aqi.so2 ?? -1,
                    aqi.pm25 ?? -1,
                    aqi.pm10 ?? -1,
                    aqi.co ?? -1
                );

            return idx <= 0 ? null : idx;
        }

        public static Color GetColorFromIndex(int index)
        {
            return index switch
            {
                < 51 => Colors.LimeGreen,
                < 101 => Color.FromArgb(0xff, 0xff, 0xde, 0x33),
                < 151 => Color.FromArgb(0xff, 0xff, 0x99, 0x33),
                < 201 => Color.FromArgb(0xff, 0xcc, 0x00, 0x33),
                < 301 => Color.FromArgb(0xff, 0xaa, 0x00, 0xff),    // 0xff660099
                _ => Color.FromArgb(0xff, 0xbd, 0x00, 0x35),        // 0xff7e0023
            };
        }

        public static Windows.UI.Xaml.Media.SolidColorBrush GetBrushFromIndex(int? index)
        {
            return index.HasValue ? new Windows.UI.Xaml.Media.SolidColorBrush(GetColorFromIndex(index.Value)) : null;
        }
    }
}
