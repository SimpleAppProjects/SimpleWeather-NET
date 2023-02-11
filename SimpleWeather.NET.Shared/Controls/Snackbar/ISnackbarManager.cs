namespace SimpleWeather.NET.Controls
{
    public interface ISnackbarManager : ISnackbarPage
    {
        void InitSnackManager();

        void DismissAllSnackbars();

        void UnloadSnackManager();
    }
}
