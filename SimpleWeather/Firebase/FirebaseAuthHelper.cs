using Firebase.Auth;
using SimpleWeather.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace SimpleWeather.Firebase
{
    public static class FirebaseAuthHelper
    {
        private const String FirebaseContainerKey = "firebase";
        private const String KEY_AUTHTOKEN = "auth_token";

        // Shared Settings
        private static ApplicationDataContainer FirebaseContainer = ApplicationData.Current.LocalSettings.
            CreateContainer(FirebaseContainerKey, ApplicationDataCreateDisposition.Always);

        public static Task<FirebaseAuthLink> GetAuthLink()
        {
            return Task.Run(async () =>
            {
                // specify your app’s client key when creating the auth provider
                var config = new FirebaseConfig(Keys.FirebaseConfig.GetGoogleAPIKey());
                using (var ap = new FirebaseAuthProvider(config))
                {
                    var token = await GetTokenFromStorage();
                    if (token != null)
                    {
                        var authLink = await new FirebaseAuthLink(ap, token).GetFreshAuthAsync();
                        if (!authLink.IsExpired() && authLink.FirebaseToken != null)
                        {
                            return authLink;
                        }
                    }

                    // sign in anonymously
                    var authTokenLink = await ap.SignInAnonymouslyAsync();
                    StoreToken(authTokenLink);
                    return authTokenLink;
                }
            });
        }

        private static async Task<FirebaseAuth> GetTokenFromStorage()
        {
            if (FirebaseContainer.Values.ContainsKey(KEY_AUTHTOKEN))
            {
                var tokenJSON = FirebaseContainer.Values[KEY_AUTHTOKEN]?.ToString();
                if (tokenJSON != null)
                {
                    return await JSONParser.DeserializerAsync<FirebaseAuth>(tokenJSON);
                }
            }

            return null;
        }

        private static void StoreToken(FirebaseAuth authToken)
        {
            FirebaseContainer.Values[KEY_AUTHTOKEN] = JSONParser.Serializer(authToken);
        }
    }
}
