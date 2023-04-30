using Firebase.Auth;
using Firebase.Auth.Repository;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SimpleWeather.Preferences;
using System;

namespace SimpleWeather.Firebase
{
    internal class FirebaseAuthUserRepository : IUserRepository
    {
        private const String FirebaseContainerKey = "firebase";
        private const String KEY_AUTHTOKEN = "auth_token";

        // Shared Settings
        private readonly SettingsContainer settings;

        private const string UserStorageKey = "FirebaseUser";
        private const string CredentialStorageKey = "FirebaseCredential";

        private readonly JsonSerializerSettings options;

        public FirebaseAuthUserRepository()
        {
            this.settings = new SettingsContainer(FirebaseContainerKey);
            this.options = new JsonSerializerSettings();
            this.options.Converters.Add(new StringEnumConverter());

            if (settings.ContainsKey(KEY_AUTHTOKEN))
            {
                settings.Remove(KEY_AUTHTOKEN);
            }
        }

        public void DeleteUser()
        {
            this.settings.Remove(UserStorageKey);
            this.settings.Remove(CredentialStorageKey);
        }

        public (UserInfo userInfo, FirebaseCredential credential) ReadUser()
        {
            var info = JsonConvert.DeserializeObject<UserInfo>(this.settings.GetValue<string>(UserStorageKey), this.options);
            var credential = JsonConvert.DeserializeObject<FirebaseCredential>(this.settings.GetValue<string>(CredentialStorageKey), this.options);

            return (info, credential);
        }

        public void SaveUser(User user)
        {
            this.settings.SetValue(UserStorageKey, JsonConvert.SerializeObject(user.Info, this.options));
            this.settings.SetValue(CredentialStorageKey, JsonConvert.SerializeObject(user.Credential, this.options));
        }

        public bool UserExists()
        {
            return this.settings.ContainsKey(UserStorageKey);
        }
    }
}
