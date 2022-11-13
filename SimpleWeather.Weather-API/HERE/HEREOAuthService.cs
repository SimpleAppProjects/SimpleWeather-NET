// NOTE: Using .NET HttpClient; UWP HttpClient doesn't work for some reason
using System;
using System.Threading.Tasks;

namespace SimpleWeather.Weather_API.HERE
{
    public interface IHEREOAuthService
    {
        Task<string> GetBearerToken(bool forceRefresh = false);
    }

    public sealed class Auth
    {
        private static readonly Lazy<IHEREOAuthService> hereOAuthServiceLazy = new(() =>
        {
            return new HEREOAuthServiceImpl();
        });

        public static IHEREOAuthService HEREOAuthService = hereOAuthServiceLazy.Value;
    }
}