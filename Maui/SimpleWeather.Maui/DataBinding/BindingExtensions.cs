namespace SimpleWeather.Maui.DataBinding
{
    public static class BindingExtensions
    {
        public static double Multiply(double value, double ratio)
        {
            return value * ratio;
        }

        public static bool IsNotNull(object obj)
        {
            return obj != null;
        }

        public static bool AreBothTrue(bool value1, bool value2)
        {
            return value1 && value2;
        }

        public static bool AreBothFalse(bool value1, bool value2)
        {
            return !value1 && !value2;
        }
    }
}
