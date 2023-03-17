using System;
namespace SimpleWeather.Maui.Preferences
{
    public class PreferenceListItem
    {
        public string Display { get; set; }
        public object Value { get; set; }

        public string Detail { get; set; }

        public PreferenceListItem() { }

        public PreferenceListItem(string Display, object Value)
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

