using Firebase.Database.Query;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Firestore.v1fix.Data;
using SimpleWeather.Utils;
using SimpleWeather.UWP.BackgroundTasks;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
                using (var cts = new CancellationTokenSource(Settings.READ_TIMEOUT))
                {
                    var channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync().AsTask(cts.Token);

                    // Update channel info
                    await WNSHelper.UpdateChannelUri(channel);
                    // Register background task
                    await WNSPushBackgroundTask.RegisterBackgroundTask();
                }
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
                    // Write to RealtimeDatabase db
                    var auth = await Firebase.FirebaseAuthHelper.GetAuthLink();
                    var db = await Firebase.FirebaseDatabaseHelper.GetFirebaseDatabase();

                    if (oldChannel?.ChannelUri != null)
                    {
                        try
                        {
                            // Delete previous entry if it exists
                            await db.Child("uwp_users").Child(oldChannel.ChannelUri).DeleteAsync();
                        } catch (Exception)
                        {
                            // ignore if does not exist
                        }
                    }

                    await db.Child("uwp_users")
                            .Child(auth.User.LocalId)
                            .PatchAsync(new FirebaseUWPUser()
                            {
                                channel_uri = channel.Uri,
                                expiration_time = channel.ExpirationTime.ToUnixTimeSeconds(),
                                package_name = Windows.ApplicationModel.Package.Current.Id.Name
                            });

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

    internal class FirebaseUWPUser
    {
        public string channel_uri { get; set; }
        public long expiration_time { get; set; }
        public string package_name { get; set; }
    }

    internal class WNSChannel
    {
        public String ChannelUri { get; set; }
        public DateTimeOffset ExpirationTime { get; set; }
    }
}