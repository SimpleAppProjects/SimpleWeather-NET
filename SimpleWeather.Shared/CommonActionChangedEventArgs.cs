using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather
{
    public sealed class CommonActionChangedEventArgs
    {
        public String Action { get; private set; }

        public IDictionary<String, object> Extras { get; private set; }

        public CommonActionChangedEventArgs(string action, IDictionary<string, object> extrasMap = null)
        {
            this.Action = action;
            this.Extras = extrasMap;
        }

        /// <summary>
        /// The delegate to use for handlers that receive the CommonActionChanged event.
        /// </summary>
        public delegate void CommonActionChangedEventHandler(object sender, CommonActionChangedEventArgs e);
    }
}
