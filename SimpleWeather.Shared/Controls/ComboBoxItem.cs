namespace SimpleWeather.Controls
{
    public class ComboBoxItem
    {
        public string Display { get; set; }
        public string Value { get; set; }

        public ComboBoxItem() { }

        public ComboBoxItem(string Display, string Value)
        {
            this.Display = Display;
            this.Value = Value;
        }

        public override string ToString()
        {
            return Display;
        }
    }
}
