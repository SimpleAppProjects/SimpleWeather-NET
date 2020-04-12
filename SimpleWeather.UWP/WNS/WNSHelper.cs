using SimpleWeather.Utils;
using SimpleWeather.UWP.BackgroundTasks;
using SimpleWeather.WeatherData.Images;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Networking.PushNotifications;
using Windows.Storage;

namespace SimpleWeather.UWP.WNS
{
    public static class WNSHelper
    {
        private static readonly ApplicationDataContainer LocalSettings =
            ApplicationData.Current.LocalSettings;
        private static readonly ApplicationDataContainer WNSSettings =
            LocalSettings.CreateContainer("wns",
                ApplicationDataCreateDisposition.Always);

        private const String KEY_WNSCHANNEL = "WNSChannel";

        public static async Task InitNotificationChannel()
        {
            try
            {
                var channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();

                // Update channel info
                await WNSHelper.UpdateChannelUri(channel);
                // Register background task
                await WNSPushBackgroundTask.RegisterBackgroundTask();
            }
            catch (Exception e)
            {
                // Could not create a channel
                Logger.WriteLine(LoggerLevel.Error, e);
            }
        }

        public static Task UpdateChannelUri(PushNotificationChannel channel)
        {
            return Task.Run(async () =>
            {
                // Check if channel id differs from uri in settings
                // if different
                //      Write channel URI to Firestore DB
                //      On success Write/Replace channel uri in settings
                //      obj { channelURI: "", expirationDate: "" }
                // else continue
                WNSChannel oldChannel = null;
                if (WNSSettings.Values.ContainsKey(KEY_WNSCHANNEL))
                {
                    oldChannel = JSONParser.Deserializer<WNSChannel>(WNSSettings.Values[KEY_WNSCHANNEL]?.ToString());
                }

                if (!Equals(channel.Uri, oldChannel?.ChannelUri))
                {
                    // Write to firestore db
                    var auth = await Firebase.FirebaseAuthHelper.GetAuthLink();
                    var firestoreDB = await Firebase.FirestoreHelper.GetFirestoreDB();
                    var userInfo = new Dictionary<String, Object>
                    {
                        { "channel_uri", channel.Uri },
                        { "expiration_time", channel.ExpirationTime.ToUnixTimeSeconds() },
                        { "package_name", Windows.ApplicationModel.Package.Current.Id.Name }
                    };
                    await firestoreDB.Collection("uwp_users")
                                     .Document(auth.User.LocalId)
                                     .SetAsync(userInfo);
                    // replace in settings
                    var newChannel = new WNSChannel()
                    {
                        ChannelUri = channel.Uri,
                        ExpirationTime = channel.ExpirationTime
                    };
                    var json = JSONParser.Serializer(newChannel);
                    WNSSettings.Values[KEY_WNSCHANNEL] = json;
                }
            });
        }
    }

    internal class WNSChannel
    {
        public String ChannelUri { get; set; }
        public DateTimeOffset ExpirationTime { get; set; }
    }
}
