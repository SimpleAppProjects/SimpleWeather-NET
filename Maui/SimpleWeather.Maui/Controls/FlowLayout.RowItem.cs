namespace SimpleWeather.Maui.Controls.Flow
{
    internal class RowItem
    {
        internal double ItemStart { get; set; }
        internal double ItemEnd { get; set; }
        internal double ItemTop { get; set; }
        internal double ItemBottom { get; set; }

        internal double ItemWidth { get; set; }
        internal double ItemHeight { get; set; }

        internal Rect ToRect()
            => new Rect(ItemStart, ItemTop, ItemEnd - ItemStart, ItemBottom - ItemTop);
    }
}

