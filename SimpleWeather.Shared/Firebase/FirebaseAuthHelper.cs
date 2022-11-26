using Firebase.Auth;
using Newtonsoft.Json;
using SimpleWeather.Preferences;
using System;
using System.Threading.Tasks;

namespace SimpleWeather.Firebase
{
    public static class FirebaseAuthHelper
    {
        private const String FirebaseContainerKey = "firebase";
        private const String KEY_AUTHTOKEN = "auth_token";

        // Shared Settings
        private readonly static SettingsContainer FirebaseContainer = new SettingsContainer(FirebaseContainerKey);

        public static async Task<FirebaseAuthLink> GetAuthLink()
        {
            // specify your app’s client key when creating the auth provider
            var config = new FirebaseConfig(Keys.FirebaseConfig.GetGoogleAPIKey());
            using (var ap = new FirebaseAuthProvider(config))
            {
                var token = await GetTokenFromStorage();
                if (token != null)
                {
                    var authLink = new FirebaseAuthLink(ap, token);
                    authLink.FirebaseAuthRefreshed += (s, e) => StoreToken(e.FirebaseAuth);
                    authLink = await authLink.GetFreshAuthAsync();
                    if (!authLink.IsExpired() && authLink.FirebaseToken != null)
                    {
                        return authLink;
                    }
                }

                // sign in anonymously
                var authTokenLink = await ap.SignInAnonymouslyAsync();
                StoreToken(authTokenLink);
                authTokenLink.FirebaseAuthRefreshed += (s, e) => StoreToken(e.FirebaseAuth);
                return authTokenLink;
            }
        }

        private static async Task<FirebaseAuth> GetTokenFromStorage()
        {
            if (FirebaseContainer.ContainsKey(KEY_AUTHTOKEN))
            {
                var tokenJSON = FirebaseContainer.GetValue<string>(KEY_AUTHTOKEN);
                if (tokenJSON != null)
                {
                    try
                    {
                        return await Task.Run(() => JsonConvert.DeserializeObject<FirebaseAuth>(tokenJSON));
                    }
                    catch (Exception) { }
                }
            }

            return null;
        }

        private static void StoreToken(FirebaseAuth authToken)
        {
            FirebaseContainer.SetValue(KEY_AUTHTOKEN, JsonConvert.SerializeObject(authToken));
        }
    }
}
