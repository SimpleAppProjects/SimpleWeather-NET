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
