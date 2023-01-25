namespace SimpleWeather.Maui.Controls
{
    public sealed partial class Banner : BindableObject
    {
        private Banner()
        {
        }

        public string Message { get; set; }
        public string ButtonLabel { get; set; }
        public Action ButtonAction { get; set; }

        public string Title { get; set; }
        public ImageSource Icon { get; set; }

        public static Banner Make(String message)
        {
            Banner Banner = new Banner()
            {
                Message = message,
            };

            return Banner;
        }

        public static Task<Banner> MakeAsync(IDispatcher dispatcher, String message)
        {
            return DispatcherExtensions.DispatchAsync(dispatcher, () =>
            {
                return Banner.Make(message);
            });
        }

        public void SetAction(String buttonTxt, Action action)
        {
            ButtonLabel = buttonTxt;
            ButtonAction = action;
        }
    }
}
