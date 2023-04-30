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

        internal static FirebaseAuthClient FirebaseAuth => authClientLazy.Value;

        public static async Task<FirebaseClient> GetFirebaseDatabase()
        {
            await CheckSignIn();

            return new FirebaseClient(Keys.FirebaseConfig.GetFirebaseDatabaseUrl(), new FirebaseOptions()
            {
                AuthTokenAsyncFactory = GetAccessToken
            });
        }

        public static async Task<string> GetAccessToken()
        {
            await CheckSignIn();

            var auth = FirebaseAuth;

            string token = null;

            try
            {
                token = await auth.User?.GetIdTokenAsync(true);
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Debug, ex, "Error getting user token");
            }

            return token;
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