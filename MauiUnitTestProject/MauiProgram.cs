using SimpleWeather.Weather_API.Keys;
using Xunit.Runners.Maui;

namespace MauiUnitTestProject
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            return MauiApp.CreateBuilder()
                .ConfigureTests(new TestOptions()
                {
                    Assemblies =
                    {
                        typeof(MauiProgram).Assembly
                    }
                })
                .ConfigureEssentials(essentials =>
                {
#if WINDOWS
                    essentials.UseMapServiceToken(APIKeys.GetBingMapsKey());
#endif
                })
                .UseVisualRunner()
                .Build();
        }
    }
}