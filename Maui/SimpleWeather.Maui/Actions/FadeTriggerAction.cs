namespace SimpleWeather.Maui.Actions;

public class FadeTriggerAction : TriggerAction<VisualElement>
{
    public int StartsFrom { get; set; }

    protected override void Invoke(VisualElement sender)
    {
        sender.Animate(nameof(FadeTriggerAction), new Animation((d) =>
            {
                var val = (StartsFrom == 1 ? d : 1) - d;
                sender.Opacity = val;
            }),
            length: 1500, // milliseconds
            easing: Easing.Linear,
            finished: (d, _) =>
            {
                sender.IsVisible = StartsFrom != 1;
            });
    }
}