namespace SimpleWeather.NET.Utils;

public class SliderValueChangedArgs: EventArgs
{
    /// <summary>Creates a new <see cref="SliderValueChangedArgs"/> with the old and new values.</summary>
    /// <param name="oldValue">The previous value.</param>
    /// <param name="newValue">The new value.</param>
    /// <param name="fromUser">Indicates whether the change was initiated by the user.</param>
    public SliderValueChangedArgs(double oldValue, double newValue, bool fromUser)
    {
        OldValue = oldValue;
        NewValue = newValue;
        FromUser = fromUser;
    }
    
    /// <summary>Whether the change was initiated by the user</summary>
    public bool FromUser { get; private set; }

    /// <summary>Gets the new value.</summary>
    public double NewValue { get; private set; }

    /// <summary>Gets the previous value.</summary>
    public double OldValue { get; private set; }
}