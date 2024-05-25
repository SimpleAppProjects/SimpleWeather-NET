using Firebase.Auth;
using Firebase.Database;
using SimpleWeather.Utils;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleWeather.Firebase
{
    public static partial class FirebaseHelper
    {
        private static bool sHasSignInFailed = false;

        private readonly static Lazy<FirebaseAuthClient> authClientLazy = new(() =>
        {
            return new FirebaseAuthClient(new FirebaseAuthConfig()
            {
                ApiKey = Keys.FirebaseConfig.GetGoogleAPIKey(),
                AuthDomain = $"{Keys.FirebaseConfig.GetProjectID()}.firebaseapp.com",
                UserRepository = new FirebaseAuthUserRepository()
            });
        });

        private readonly static Lazy<FirebaseAnalytics> analyticsLazy = new(() =>
        {
            return new FirebaseAnalytics(Keys.FirebaseConfig.GetMeasurementID(), Keys.FirebaseConfig.GetAnalyticsSecret(), GetAuthUser);
        });

        private static FirebaseAuthClient FirebaseAuth => authClientLazy.Value;

        public static async Task<FirebaseClient> GetFirebaseDatabase()
        {
            await CheckSignIn();

            return new FirebaseClient(Keys.FirebaseConfig.GetFirebaseDatabaseUrl(), new FirebaseOptions()
            {
                AuthTokenAsyncFactory = GetAccessToken
            });
        }

        public static async Task<FirebaseRemoteConfig> GetFirebaseRemoteConfig()
        {
            await CheckSignIn();

            return new FirebaseRemoteConfig(Keys.FirebaseConfig.GetProjectID(), Keys.FirebaseConfig.GetAppID(), Keys.FirebaseConfig.GetGoogleAPIKey(), GetAuthUser);
        }

        public static FirebaseAnalytics GetFirebaseAnalytics()
        {
            return analyticsLazy.Value;
        }

        public static async Task<string> GetAccessToken()
        {
            string token = null;

            try
            {
                var user = await GetAuthUser();
                token = await user?.GetIdTokenAsync(true);
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Debug, ex, "Error getting user token");
            }

            return token;
        }

        internal static async Task<User> GetAuthUser()
        {
            await CheckSignIn();

            var auth = FirebaseAuth;

            return auth.User;
        }

        private static async Task CheckSignIn()
        {
            var auth = FirebaseAuth;
            if (auth.User == null && !sHasSignInFailed)
            {
                try
                {
                    using var cts = new CancellationTokenSource(15000 /* 15s */);
                    await auth.SignInAnonymouslyAsync().WaitAsync(cts.Token);
                }
                catch (Exception ex)
                {
                    if (ex is TimeoutException)
                    {
                        sHasSignInFailed = true;
                    }
                }
            }
        }
    }
}