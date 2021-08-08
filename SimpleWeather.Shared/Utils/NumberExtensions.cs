using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.Utils
{
    public static class NumberExtensions
    {
        public static int RoundToInt(this float @num)
        {
            return (int)MathF.Round(num);
        }
    }
}
