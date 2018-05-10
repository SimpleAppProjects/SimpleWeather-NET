using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleWeather.Controls
{
    public class ProviderEntry : ComboBoxItem
    {
        public string MainURL { get; set; }
        public string APIRegisterURL { get; set; }

        public ProviderEntry(string Display, string Value, string MainURL, string APIRegisterURL)
            : base(Display, Value)
        {
            this.MainURL = MainURL;
            this.APIRegisterURL = APIRegisterURL;
        }
    }
}
