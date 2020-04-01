using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.Firebase
{
    public static class FirestoreHelper
    {
        public static async Task<FirestoreDb> GetFirestoreDB()
        {
            var auth = await FirebaseAuthHelper.GetAuthLink();
            return await new FirestoreDbBuilder() 
            {
                ProjectId = Keys.FirebaseConfig.GetProjectID(),
                TokenAccessMethod = async (s, cts) =>
                {
                    if (auth.IsExpired()) auth = await auth.GetFreshAuthAsync();
                    return auth.FirebaseToken;
                }
            }.BuildAsync().ConfigureAwait(false);
        }
    }
}
