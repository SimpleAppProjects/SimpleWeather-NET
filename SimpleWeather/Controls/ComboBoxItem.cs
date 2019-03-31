using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.Controls
{
    public class ComboBoxItem
    {
        public string Display { get; set; }
        public string Value { get; set; }

        public ComboBoxItem(string Display, string Value)
        {
            this.Display = Display;
            this.Value = Value;
        }

        public override string ToString()
        {
            return Display;
        }
    }
}
