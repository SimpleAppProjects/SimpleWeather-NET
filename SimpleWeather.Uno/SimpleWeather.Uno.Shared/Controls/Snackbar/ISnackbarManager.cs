namespace SimpleWeather.UWP.Controls
{
    public interface ISnackbarManager : ISnackbarPage
    {
        void InitSnackManager();

        void DismissAllSnackbars();

        void UnloadSnackManager();
    }
}
