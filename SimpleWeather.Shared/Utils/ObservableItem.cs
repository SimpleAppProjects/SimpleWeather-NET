using System.Collections.Generic;
using System.Text;

namespace SimpleWeather.Utils
{
    public sealed class ObservableItemChangedEventArgs
    {
        public object OldValue { get; internal set; }
        public object NewValue { get; internal set; }
    }

    public delegate void ObservableItemChangedEventHandler(object sender, ObservableItemChangedEventArgs e);

    public class ObservableItem<T>
    {
        private T value;
        public event ObservableItemChangedEventHandler ItemValueChanged;

        public T GetValue()
        {
            return value;
        }

        public void SetValue(T value)
        {
            if (!ReferenceEquals(this.value, value))
            {
                var oldVal = this.value;

                this.value = value;

                ItemValueChanged?.Invoke(this, new ObservableItemChangedEventArgs
                {
                    OldValue = oldVal,
                    NewValue = value
                });
            }
        }
    }
}
