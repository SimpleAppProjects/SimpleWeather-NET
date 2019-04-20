using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.UWP.Helpers
{
    interface IBackRequestedPage
    {
        Task<bool> OnBackRequested();
    }
}
