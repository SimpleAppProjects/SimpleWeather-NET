#if !WINUI
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui;
#if __ANDROID__
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
#endif
using Microsoft.Maui.Storage;

namespace SimpleWeather.Utils
{
	public static class FileSystemUtils
	{
        public static async Task<bool> FileExistsAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return false;
            }
            else if (filePath.StartsWith("maui-appx:"))
            {
                return await FileSystem.AppPackageFileExistsAsync(filePath.ReplaceFirst("maui-appx://", ""));
            }
            else
            {
                return File.Exists(filePath);
            }
        }

        public static async Task<bool> IsAppPackageFileLockedAsync(this IFileSystem fileSystem, string filePath)
        {
            if (!await fileSystem.AppPackageFileExistsAsync(filePath))
                return false;

            Stream stream = null;

            try
            {
                stream = await fileSystem.OpenAppPackageFileAsync(filePath);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Dispose();
            }

            //file is not locked
            return false;
        }

#if __ANDROID__
        public static async Task<Stream> OpenAppPackageFileAsync(string filename)
        {
            filename = Path.GetFileName(filename);

            if (File.Exists(filename))
            {
                return File.OpenRead(filename);
            }
            else if (await FileSystem.AppPackageFileExistsAsync(filename))
            {
                return await FileSystem.OpenAppPackageFileAsync(filename);
            }
            else
            {
                var context = Android.App.Application.Context;
                var resources = context.Resources;

                var resourceId = context.GetDrawableId(filename.ReplaceFirst(".svg", ".png"));
                if (resourceId > 0)
                {
                    var imageUri = new Android.Net.Uri.Builder()
                        .Scheme(Android.Content.ContentResolver.SchemeAndroidResource)
                        .Authority(resources.GetResourcePackageName(resourceId))
                        .AppendPath(resources.GetResourceTypeName(resourceId))
                        .AppendPath(resources.GetResourceEntryName(resourceId))
                        .Build();

                    var stream = context.ContentResolver.OpenInputStream(imageUri);
                    if (stream is not null)
                        return stream;
                }

                return Stream.Null;
            }
        }
#else
        public static Task<Stream> OpenAppPackageFileAsync(string filename)
        {
            return FileSystem.OpenAppPackageFileAsync(filename);
        }
#endif
    }
}
#endif
