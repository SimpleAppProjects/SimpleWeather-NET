using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.UWP.Controls
{
    public interface ISnackbarManager
    {
        void InitSnackManager();

        void ShowSnackbar(Snackbar snackbar);

        void DismissAllSnackbars();

        void UnloadSnackManager();
    }
}
