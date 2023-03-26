#if __IOS__ || __MACCATALYST__
using Foundation;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using UIKit;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.Maui.Updates
{
    public class IOSInAppUpdateManagerImpl : InAppUpdateManager
    {
        private readonly SettingsManager SettingsManager;

        private AppInfo appUpdateInfo = null;
        private UpdateInfo configUpdateInfo = null;

        private Version appStoreVersion = null;
        private Version currentVersion = null;

        public IOSInAppUpdateManagerImpl(SettingsManager settingsManager)
        {
            this.SettingsManager = settingsManager;
        }

        public int UpdatePriority => configUpdateInfo?.UpdatePriority ?? -1;

        public Task<bool> CheckIfUpdateAvailable()
        {
            return Task.Run(async () =>
            {
                try
                {
                    appUpdateInfo = await GetAppInfo();

                    appStoreVersion = Version.Parse(appUpdateInfo.version);
                    currentVersion = Version.Parse(GetCurrentBundleVersion());

                    if (appStoreVersion > currentVersion)
                    {
                        FeatureSettings.IsUpdateAvailable = true;

                        // Check priority of update
                        var remoteUpdateInfo = await GetRemoteUpdateInfo();
                        configUpdateInfo = remoteUpdateInfo?.FirstOrDefault(it => it.VersionCode.ToInvariantString() == appUpdateInfo?.version?.Replace(".", ""));

                        if (configUpdateInfo != null)
                        {
                            return true;
                        }
                    }
                }
                catch (Exception e)
                {
                    return await CheckIfUpdateAvailableFallback();
                }

                return false;
            });
        }

        public bool ShouldStartImmediateUpdate()
        {
            if (appUpdateInfo != null && configUpdateInfo != null)
            {
                return configUpdateInfo?.UpdatePriority > 3;
            }
            else
            {
                return false;
            }
        }

        public Task<bool> ShouldStartImmediateUpdateFlow()
        {
            return Task.Run(async () =>
            {
                return await CheckIfUpdateAvailable() && ShouldStartImmediateUpdate();
            });
        }

        public Task StartImmediateUpdateFlow()
        {
            return MainThread.InvokeOnMainThreadAsync(() =>
            {
                // Show undismissable alert
                var viewController = Platform.GetCurrentUIViewController();

                if (viewController != null)
                {
                    var alertController = UIAlertController.Create(ResStrings.prompt_update_title, ResStrings.prompt_update_available, UIAlertControllerStyle.Alert);

                    var updateButton = UIAlertAction.Create(ResStrings.ConfirmDialog_PrimaryButtonText, UIAlertActionStyle.Default, async (action) =>
                    {
                        var url = NSUrl.FromString(appUpdateInfo.trackViewUrl);

                        await UIApplication.SharedApplication.OpenUrlAsync(url, new UIApplicationOpenUrlOptions());
                    });

                    alertController.AddAction(updateButton);
                    viewController.PresentViewController(alertController, true, null);
                }
            });
        }

        private Task<IEnumerable<UpdateInfo>> GetRemoteUpdateInfo()
        {
            return Task.Run(async () =>
            {
                var db = await Firebase.FirebaseDatabaseHelper.GetFirebaseDatabase();
                var config = await db.Child("ios_updates").OnceAsync<object>();

                if (config?.Count > 0)
                {
                    return config.Select(prop =>
                    {
                        return JSONParser.Deserializer<UpdateInfo>(prop.Object.ToString());
                    });
                }

                return null;
            });
        }

        private async Task<bool> CheckIfUpdateAvailableFallback()
        {
            try
            {
                var remoteUpdateInfo = await GetRemoteUpdateInfo();
                var lastUpdate = remoteUpdateInfo?.LastOrDefault();

                if (lastUpdate != null)
                {
                    return SettingsManager.VersionCode < lastUpdate.VersionCode;
                }
            }
            catch (Exception e)
            {
                Logger.WriteLine(LoggerLevel.Error, e);
            }

            return false;
        }

        private string GetBundle(string key)
        {
            try
            {
                var path = NSBundle.MainBundle.PathForResource("Info", "plist");

                if (path == null) return null;

                var plist = new NSDictionary(path);
                var value = plist?.ObjectForKey(NSObject.FromObject(key));

                return value as NSString;
            }
            catch { }

            return null;
        }

        private async Task<AppInfo> GetAppInfo()
        {
            var identifier = GetBundle("CFBundleIdentifier");

            if (identifier == null) return null;

            var url = new Uri($"https://itunes.apple.com/us/lookup?bundleId={identifier}");

            AppInfo result = null;

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                using var response = await SharedModule.Instance.WebClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                using var stream = await response.Content.ReadAsStreamAsync();
                result = await JSONParser.DeserializerAsync<AppInfo>(stream);
            }
            catch { }

            return result;
        }

        private string GetCurrentBundleVersion()
        {
            return GetBundle("CFBundleShortVersionString");
        }

        private class Rootobject
        {
            public int resultCount { get; set; }
            public AppInfo[] results { get; set; }
        }

        private class AppInfo
        {
            /*
            public string[] screenshotUrls { get; set; }
            public string[] ipadScreenshotUrls { get; set; }
            public object[] appletvScreenshotUrls { get; set; }
            public string artworkUrl60 { get; set; }
            public string artworkUrl512 { get; set; }
            public string artworkUrl100 { get; set; }
            public string artistViewUrl { get; set; }
            public string[] features { get; set; }
            public string[] supportedDevices { get; set; }
            public string[] advisories { get; set; }
            public bool isGameCenterEnabled { get; set; }
            public string kind { get; set; }
            */
            public string trackViewUrl { get; set; }
            /*
            public string trackContentRating { get; set; }
            public string minimumOsVersion { get; set; }
            public string trackCensoredName { get; set; }
            public string[] languageCodesISO2A { get; set; }
            public string fileSizeBytes { get; set; }
            public string formattedPrice { get; set; }
            public string contentAdvisoryRating { get; set; }
            public float averageUserRatingForCurrentVersion { get; set; }
            public int userRatingCountForCurrentVersion { get; set; }
            public float averageUserRating { get; set; }
            public DateTime currentVersionReleaseDate { get; set; }
            public string releaseNotes { get; set; }
            public int artistId { get; set; }
            public string artistName { get; set; }
            public string[] genres { get; set; }
            public float price { get; set; }
            public string[] genreIds { get; set; }
            public string description { get; set; }
            public bool isVppDeviceBasedLicensingEnabled { get; set; }
            */
            public string bundleId { get; set; }
            /*
            public string sellerName { get; set; }
            public DateTime releaseDate { get; set; }
            public int trackId { get; set; }
            public string trackName { get; set; }
            public string primaryGenreName { get; set; }
            public int primaryGenreId { get; set; }
            */
            public string version { get; set; }
            /*
            public string wrapperType { get; set; }
            public string currency { get; set; }
            public int userRatingCount { get; set; }
            */
        }
    }
}
#endif
