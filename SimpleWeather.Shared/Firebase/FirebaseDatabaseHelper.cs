using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.Firebase
{
    public static class FirebaseDatabaseHelper
    {
        public static async Task<FirebaseClient> GetFirebaseDatabase()
        {
            var auth = await FirebaseAuthHelper.GetAuthLink();
            return new FirebaseClient(Keys.FirebaseConfig.GetFirebaseDatabaseUrl(), new FirebaseOptions()
            {
                AuthTokenAsyncFactory = async () =>
                {
                    if (auth.IsExpired()) auth = await auth.GetFreshAuthAsync();
                    return auth.FirebaseToken;
                }
            });
        }
    }
}
