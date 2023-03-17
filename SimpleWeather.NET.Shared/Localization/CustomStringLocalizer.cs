using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.Localization;
using SimpleWeather.Resources.Strings;
using System.Collections.Generic;
using System.Linq;
using ResBackgrounds = SimpleWeather.Backgrounds.Resources.Strings.Backgrounds;
using ResExtras = SimpleWeather.Extras.Resources.Strings.Extras;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.NET.Localization
{
    public class CustomStringLocalizer : IStringLocalizer
    {
        private readonly IStringLocalizer<ResStrings> ResStrings = Ioc.Default.GetService<IStringLocalizer<ResStrings>>();
        private readonly IStringLocalizer<ResBackgrounds> ResBackgrounds = Ioc.Default.GetService<IStringLocalizer<ResBackgrounds>>();
        private readonly IStringLocalizer<ResExtras> ResExtras = Ioc.Default.GetService<IStringLocalizer<ResExtras>>();
        private readonly IStringLocalizer<AQIndex> ResAQIndex = Ioc.Default.GetService<IStringLocalizer<AQIndex>>();
        private readonly IStringLocalizer<Beaufort> ResBeaufort = Ioc.Default.GetService<IStringLocalizer<Beaufort>>();
        private readonly IStringLocalizer<Config> ResConfig = Ioc.Default.GetService<IStringLocalizer<Config>>();
        private readonly IStringLocalizer<MoonPhases> ResMoonPhases = Ioc.Default.GetService<IStringLocalizer<MoonPhases>>();
        private readonly IStringLocalizer<Units> ResUnits = Ioc.Default.GetService<IStringLocalizer<Units>>();
        private readonly IStringLocalizer<UVIndex> ResUVIndex = Ioc.Default.GetService<IStringLocalizer<UVIndex>>();
        private readonly IStringLocalizer<WeatherConditions> ResWeatherConditions = Ioc.Default.GetService<IStringLocalizer<WeatherConditions>>();
        private readonly IStringLocalizer<WindDirection> ResWindDir = Ioc.Default.GetService<IStringLocalizer<WindDirection>>();

        public LocalizedString this[string name]
        {
            get
            {
                if (name.StartsWith("/Backgrounds/"))
                {
                    return ResBackgrounds[name.Substring(13)];
                }
                else if (name.StartsWith("/Extras/"))
                {
                    var key = name.Substring(8);
                    var value = ResExtras[key];
                    return value;
                }
                else if (name.StartsWith("/AQIndex/"))
                {
                    return ResAQIndex[name.Substring(9)];
                }
                else if (name.StartsWith("/Beaufort/"))
                {
                    return ResBeaufort[name.Substring(10)];
                }
                else if (name.StartsWith("/Config/"))
                {
                    return ResConfig[name.Substring(8)];
                }
                else if (name.StartsWith("/MoonPhases/"))
                {
                    return ResMoonPhases[name.Substring(12)];
                }
                else if (name.StartsWith("/Units/"))
                {
                    return ResUnits[name.Substring(7)];
                }
                else if (name.StartsWith("/UVIndex/"))
                {
                    return ResUVIndex[name.Substring(9)];
                }
                else if (name.StartsWith("/WeatherConditions/"))
                {
                    return ResWeatherConditions[name.Substring(19)];
                }
                else if (name.StartsWith("/WindDirection/"))
                {
                    return ResWindDir[name.Substring(15)];
                }
                else
                {
                    return ResStrings[name];
                }
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                if (name.StartsWith("/Backgrounds/"))
                {
                    return ResBackgrounds[name.Substring(13), arguments];
                }
                else if (name.StartsWith("/Extras/"))
                {
                    return ResExtras[name.Substring(8), arguments];
                }
                else if (name.StartsWith("/AQIndex/"))
                {
                    return ResAQIndex[name.Substring(10), arguments];
                }
                else if (name.StartsWith("/Beaufort/"))
                {
                    return ResBeaufort[name.Substring(10), arguments];
                }
                else if (name.StartsWith("/Config/"))
                {
                    return ResConfig[name.Substring(8), arguments];
                }
                else if (name.StartsWith("/MoonPhases/"))
                {
                    return ResMoonPhases[name.Substring(12), arguments];
                }
                else if (name.StartsWith("/Units/"))
                {
                    return ResUnits[name.Substring(7), arguments];
                }
                else if (name.StartsWith("/UVIndex/"))
                {
                    return ResUVIndex[name.Substring(9), arguments];
                }
                else if (name.StartsWith("/WeatherConditions/"))
                {
                    return ResWeatherConditions[name.Substring(19), arguments];
                }
                else if (name.StartsWith("/WindDirection/"))
                {
                    return ResWindDir[name.Substring(15), arguments];
                }
                else
                {
                    return ResStrings[name, arguments];
                }
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return ResStrings.GetAllStrings(includeParentCultures)
                .Concat(ResBackgrounds.GetAllStrings(includeParentCultures))
                .Concat(ResExtras.GetAllStrings(includeParentCultures))
                .Concat(ResAQIndex.GetAllStrings(includeParentCultures))
                .Concat(ResBeaufort.GetAllStrings(includeParentCultures))
                .Concat(ResConfig.GetAllStrings(includeParentCultures))
                .Concat(ResMoonPhases.GetAllStrings(includeParentCultures))
                .Concat(ResUnits.GetAllStrings(includeParentCultures))
                .Concat(ResUVIndex.GetAllStrings(includeParentCultures))
                .Concat(ResWeatherConditions.GetAllStrings(includeParentCultures))
                .Concat(ResWindDir.GetAllStrings(includeParentCultures));
        }
    }
}
