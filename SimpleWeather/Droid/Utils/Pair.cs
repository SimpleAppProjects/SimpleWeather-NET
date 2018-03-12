#if __ANDROID__
namespace SimpleWeather.Droid.Utils
{
    public class Pair<TKey, TValue> : Java.Lang.Object
    {
        [Newtonsoft.Json.JsonProperty]
        private TKey key;
        [Newtonsoft.Json.JsonProperty]
        private TValue value;

        public Pair(TKey key, TValue value)
        {
            this.key = key;
            this.value = value;
        }

        public TKey Key
        {
            get { return key; }
        }

        public TValue Value
        {
            get { return value; }
        }
    }
}
#endif