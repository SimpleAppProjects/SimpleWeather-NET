#if !WINUI
using System;
using System.IO;
using System.Threading.Tasks;
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
    }
}
#endif
