using SimpleWeather.Icons;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace SimpleWeather.Controls
{
    public class PollenViewModel
    {
        public string TreePollenDescription { get; private set; }
        public Color? TreePollenDescriptionColor { get; private set; }

        public string GrassPollenDescription { get; private set; }
        public Color? GrassPollenDescriptionColor { get; private set; }

        public string RagweedPollenDescription { get; private set; }
        public Color? RagweedPollenDescriptionColor { get; private set; }

        public PollenViewModel(Pollen pollenData)
        {
            var treePollenDesc = GetPollenCountDescription(pollenData.treePollenCount);
            TreePollenDescription = treePollenDesc.Item1;
            TreePollenDescriptionColor = treePollenDesc.Item2;

            var grassPollenDesc = GetPollenCountDescription(pollenData.grassPollenCount);
            GrassPollenDescription = grassPollenDesc.Item1;
            GrassPollenDescriptionColor = grassPollenDesc.Item2;

            var ragweedPollenDesc = GetPollenCountDescription(pollenData.ragweedPollenCount);
            RagweedPollenDescription = ragweedPollenDesc.Item1;
            RagweedPollenDescriptionColor = ragweedPollenDesc.Item2;
        }

        private (String, Color?) GetPollenCountDescription(Pollen.PollenCount? pollenCount)
        {
            switch (pollenCount)
            {
                default:
                case Pollen.PollenCount.Unknown:
                    return (WeatherIcons.EM_DASH, null);
                case Pollen.PollenCount.Low:
                    return ("Low", Colors.LimeGreen);
                case Pollen.PollenCount.Moderate:
                    return ("Moderate", Colors.Orange);
                case Pollen.PollenCount.High:
                    return ("High", Colors.OrangeRed);
                case Pollen.PollenCount.VeryHigh:
                    return ("Very High", Colors.Red);
            }
        }
    }
}
