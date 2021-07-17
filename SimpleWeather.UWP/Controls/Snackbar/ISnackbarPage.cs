using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.UWP.Controls
{
    public interface ISnackbarPage
    {
        public void ShowSnackbar(Snackbar snackbar);
    }
}
