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

        public static async Task<FirebaseAuthLink> GetAuthLink()
        {
            // specify your app’s client key when creating the auth provider
            var config = new FirebaseConfig(Keys.FirebaseConfig.GetGoogleAPIKey());
            using (var ap = new FirebaseAuthProvider(config))
            {
                var token = await AsyncTask.RunAsync(GetTokenFromStorage);
                if (token != null)
                {
                    return await new FirebaseAuthLink(ap, token).GetFreshAuthAsync();
                }
                else
                {
                    // sign in anonymously
                    var authTokenLink = await ap.SignInAnonymouslyAsync();
                    StoreToken(authTokenLink);
                    return authTokenLink;
                }
            }
        }

        private static async Task<FirebaseAuth> GetTokenFromStorage()
        {
            if (FirebaseContainer.Values.ContainsKey(KEY_AUTHTOKEN))
            {
                var tokenJSON = FirebaseContainer.Values[KEY_AUTHTOKEN]?.ToString();
                if (tokenJSON != null)
                {
                    var token = await AsyncTask.RunAsync(() =>
                    {
                        return JSONParser.Deserializer<FirebaseAuth>(tokenJSON);
                    });
                }
            }

            return null;
        }

        private static void StoreToken(FirebaseAuth authToken)
        {
            AsyncTask.Run(() =>
            {
                FirebaseContainer.Values[KEY_AUTHTOKEN] = JSONParser.Serializer(authToken);
            });
        }
    }
}
