using System;
namespace SimpleWeather.Maui.Preferences
{
	public class SwitchCell : Microsoft.Maui.Controls.SwitchCell
	{
		public SwitchCell()
		{
		}

        protected override void OnTapped()
        {
            base.OnTapped();
            this.On = !this.On;
        }
    }
}

