namespace SimpleWeather.Utils;

public static class Pair
{
    public static Pair<TKey, TValue> Create<TKey, TValue>(TKey key, TValue value)
    {
        return new Pair<TKey, TValue>(key, value);
    }
}

public record Pair<TKey, TValue>(TKey Key, TValue Value)
{
    public override string ToString()
    {
        return $"[{Key}, {Value}]";
    }
}