﻿using SimpleWeather.Icons;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
#if WINUI
using Microsoft.UI;
using Windows.UI;
#else
using Microsoft.Maui.Graphics;
#endif
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.Common.Controls
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
                    return (ResStrings.label_count_low, Colors.LimeGreen);
                case Pollen.PollenCount.Moderate:
                    return (ResStrings.label_count_moderate, Colors.Orange);
                case Pollen.PollenCount.High:
                    return (ResStrings.label_count_high, Colors.OrangeRed);
                case Pollen.PollenCount.VeryHigh:
                    return (ResStrings.label_count_veryhigh, Colors.Red);
            }
        }

        public override bool Equals(object obj)
        {
            return obj is PollenViewModel model &&
                   TreePollenDescription == model.TreePollenDescription &&
                   EqualityComparer<Color?>.Default.Equals(TreePollenDescriptionColor, model.TreePollenDescriptionColor) &&
                   GrassPollenDescription == model.GrassPollenDescription &&
                   EqualityComparer<Color?>.Default.Equals(GrassPollenDescriptionColor, model.GrassPollenDescriptionColor) &&
                   RagweedPollenDescription == model.RagweedPollenDescription &&
                   EqualityComparer<Color?>.Default.Equals(RagweedPollenDescriptionColor, model.RagweedPollenDescriptionColor);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TreePollenDescription, TreePollenDescriptionColor, GrassPollenDescription, GrassPollenDescriptionColor, RagweedPollenDescription, RagweedPollenDescriptionColor);
        }
    }
}
