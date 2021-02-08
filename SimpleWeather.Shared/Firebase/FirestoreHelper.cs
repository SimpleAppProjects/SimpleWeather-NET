using Google.Apis.Firestore.v1fix;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.Firebase
{
    public static class FirestoreHelper
    {
        public static Task<FirestoreService> GetFirestoreDB()
        {
            return Task.Run(async () =>
            {
                var auth = await FirebaseAuthHelper.GetAuthLink();
                var service = new FirestoreService(new BaseClientService.Initializer()
                {
                    ApplicationName = "SimpleWeather",
                    ApiKey = Keys.FirebaseConfig.GetGoogleAPIKey()
                });

                return service;
            });
        }

        public static String GetParentPath()
        {
            return "projects/" + Keys.FirebaseConfig.GetProjectID() + "/databases/(default)/documents";
        }
    }
}
