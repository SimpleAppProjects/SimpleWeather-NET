using SimpleWeather.Utils;
using System;
using System.Threading;
using System.Threading.Tasks;
#if __IOS__
using Firebase.CloudFirestore;
using FirebaseFirestore = Firebase.CloudFirestore.Firestore;
using FirebaseStorage = Firebase.Storage.Storage;
using FirebaseDatabase = Firebase.Database.Database;
using FirebaseAuth = Firebase.Auth.Auth;
#else
using Firebase.Auth;
using Firebase.Database;
#endif

namespace SimpleWeather.Firebase
{
    public static partial class FirebaseHelper
    {
        private static bool sHasSignInFailed = false;

#if __IOS__
        private static bool sSetupFirebaseDB = false;

        public static async Task<FirebaseFirestore> GetFirestoreDB()
        {
            await CheckSignIn();

            var db = FirebaseFirestore.SharedInstance;
            db.Settings = new FirestoreSettings()
            {
                // Min: 1048576 bytes; should be -1 for unlimited but getting 0
                CacheSizeBytes = Math.Min(FirestoreSettings.CacheSizeUnlimited, -1),
                PersistenceEnabled = true,
                SslEnabled = true
            };
            return db;
        }

        public static async Task<FirebaseStorage> GetFirebaseStorage()
        {
            await CheckSignIn();

            var stor = FirebaseStorage.DefaultInstance;
            stor.MaxDownloadRetryTime = TimeSpan.FromHours(1).TotalSeconds; // time in seconds
            return stor;
        }

        public static async Task<FirebaseDatabase> GetFirebaseDB()
        {
            await CheckSignIn();

            var db = FirebaseDatabase.DefaultInstance;
            if (!sSetupFirebaseDB)
            {
                db.PersistenceEnabled = true;
                db.PersistenceCacheSizeBytes = 2 * 1024 * 1024; // 2 MB
                sSetupFirebaseDB = true;
            }
            return db;
        }

        public static async Task<string> GetAccessToken()
        {
            await CheckSignIn();

            string token = null;

            try
            {
                var auth = FirebaseAuth.DefaultInstance;
                token = await auth.CurrentUser?.GetIdTokenAsync();
            }
            catch (Exception ex)
            {
                Logger.WriteLine(LoggerLevel.Debug, ex, "Error getting user token");
            }

            return token;
        }

        private static async Task CheckSignIn()
        {
            var auth = FirebaseAuth.DefaultInstance;
            if (auth.CurrentUser == null && !sHasSignInFailed)
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
#else
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
#endif
    }
}