using Microsoft.IdentityModel.Tokens;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API.Keys;
using SimpleWeather.WeatherData;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace SimpleWeather.Weather_API.WeatherKit
{
    public sealed class WeatherKitJwtServiceImpl : IWeatherKitJwtService
    {
        private const string KEY_TOKEN = "token";

        public async Task<string> GetBearerToken(bool forceRefresh = false)
        {
            var handler = new JwtSecurityTokenHandler();

            if (!forceRefresh)
            {
                var token = GetTokenFromStorage(handler);
                if (!String.IsNullOrWhiteSpace(token))
                    return token;
                else
                    forceRefresh = true;
            }

            if (forceRefresh)
            {
                try
                {
                    return GenerateBearerToken(handler);
                }
                catch (Exception e)
                {
                    Logger.WriteLine(LoggerLevel.Error, e, "WeatherKitJwtService: Error retrieving token");
                }
            }

            return null;
        }

        private static string GetTokenFromStorage(JwtSecurityTokenHandler handler)
        {
            // Shared Settings
            var WeatherKitContainer = new SettingsContainer(WeatherAPI.Apple);

            if (WeatherKitContainer.ContainsKey(KEY_TOKEN))
            {
                var token = WeatherKitContainer.GetValue<string>(KEY_TOKEN);
                if (!string.IsNullOrWhiteSpace(token) && handler.CanReadToken(token))
                {
                    var jwtToken = handler.ReadJwtToken(token);

                    // Add buffer before expiration to avoid any auth issues
                    if (jwtToken != null && jwtToken.ValidTo.AddMinutes(-1.5) > DateTime.UtcNow)
                    {
                        var tokenStr = handler.WriteToken(jwtToken);
                        StoreToken(tokenStr);
                        return tokenStr;
                    }
                }
            }

            return null;
        }

        private static void StoreToken(string token)
        {
            // Shared Settings
            var WeatherKitContainer = new SettingsContainer(WeatherAPI.Apple);
            WeatherKitContainer.SetValue(KEY_TOKEN, token);
        }

        private static string GenerateBearerToken(JwtSecurityTokenHandler handler)
        {
            var iat = (int)(DateTime.UtcNow - DateTime.UnixEpoch).TotalSeconds;
            var exp = iat + (int)TimeSpan.FromMinutes(60).TotalSeconds;

            var privKeyBytes = Convert.FromBase64String(WeatherKitConfig.GetPrivateKey());
            var privKey = ECDsa.Create();
            privKey.ImportPkcs8PrivateKey(privKeyBytes, out _);
            var securityKey = new ECDsaSecurityKey(privKey);

            var header = new JwtHeader(new SigningCredentials(securityKey, SecurityAlgorithms.EcdsaSha256))
            {
                { JwtHeaderParameterNames.Kid, WeatherKitConfig.GetKeyID() },
                { "id", $"{WeatherKitConfig.GetTeamID()}.{WeatherKitConfig.GetServiceID()}" }
            };

            var payload = new JwtPayload(new[]
            {
                new Claim(JwtRegisteredClaimNames.Iss, WeatherKitConfig.GetTeamID()),
                new Claim(JwtRegisteredClaimNames.Iat, iat.ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, exp.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, WeatherKitConfig.GetServiceID())
            });

            var token = new JwtSecurityToken(header, payload);
            return handler.WriteToken(token);
        }
    }
}
