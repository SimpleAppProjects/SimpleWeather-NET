using Firebase.Storage;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.Firebase
{
    public static class FirebaseStorageHelper
    {
        private static readonly Uri NetworkRequestUri = new Uri("https://firebasestorage.googleapis.com/v0/b/");
        public static DateTime LastDownloadFailTimestamp = DateTime.MinValue;

        public static Task<FirebaseStorage> GetFirebaseStorage()
        {
            return Task.Run(async () =>
            {
                var auth = await FirebaseAuthHelper.GetAuthLink();
                return new FirebaseStorage(Keys.FirebaseConfig.GetGoogleStorageBucket(), new FirebaseStorageOptions()
                {
                    AuthTokenAsyncFactory = async () =>
                    {
                        if (auth.IsExpired()) auth = await auth.GetFreshAuthAsync();
                        return auth.FirebaseToken;
                    },
                    HttpClientTimeout = TimeSpan.FromMilliseconds(Utils.Settings.READ_TIMEOUT),
                });
            });
        }

        public static FirebaseStorageReference GetReferenceFromUrl(this FirebaseStorage storage, Uri uri)
        {
            if (VerifyStorageUri(storage, uri))
            {
                var uriPath = uri.IsAbsoluteUri ? uri.AbsolutePath : uri.LocalPath;
                String encodedPath = uriPath;
                if (!uri.Scheme.Equals("gs")) 
                { 
                    int firstBSlash = encodedPath.IndexOf("/b/", 0); // /v0/b/bucket.storage
                                                                     // .firebase.com/o/child/image.png
                    int endBSlash = encodedPath.IndexOf("/", firstBSlash + 3);
                    int firstOSlash = encodedPath.IndexOf("/o/", 0);
                    if (firstBSlash != -1 && endBSlash != -1)
                    {
                        if (firstOSlash != -1)
                        {
                            encodedPath = encodedPath.Substring(firstOSlash + 3);
                        }
                        else
                        {
                            encodedPath = "";
                        }
                    }
                }

                var originalPath = Uri.UnescapeDataString(encodedPath);
                var childPaths = originalPath.Split('/');

                FirebaseStorageReference storageRef = null;
                foreach (var child in childPaths)
                {
                    if (String.IsNullOrWhiteSpace(child))
                        continue;

                    if (storageRef == null)
                    {
                        storageRef = storage.Child(child);
                    }
                    else
                    {
                        storageRef = storageRef.Child(child);
                    }
                }

                return storageRef;
            }
            else
            {
                throw new ArgumentException("Invalid uri", nameof(uri));
            }
        }

        private static bool VerifyStorageUri(FirebaseStorage storage, Uri uri)
        {
            if (uri == null || !uri.IsWellFormedOriginalString())
            {
                throw new ArgumentException("Invalid uri", nameof(uri));
            }

            if (uri != null && uri.IsWellFormedOriginalString() &&
                uri.Scheme.Equals("gs") || uri.Scheme.Equals("https") || uri.Scheme.Equals("http")) {
                if (uri.Scheme.Equals("gs"))
                {
                    return uri.Authority.Equals(storage.StorageBucket);
                }
                else
                {
                    if (NetworkRequestUri.Authority.Equals(uri.Authority))
                    {
                        return uri.IsAbsoluteUri && uri.AbsolutePath.StartsWith(NetworkRequestUri.AbsolutePath + storage.StorageBucket + "/");
                    }
                }
            }

            return false;
        }
    }
}
