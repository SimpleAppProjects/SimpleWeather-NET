using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace SimpleWeather.Utils
{
    public static class FileUtils
    {
        public static async Task<bool> IsFileLocked(StorageFile file)
        {
            if (!File.Exists(file.Path))
                return false;

            IRandomAccessStream stream = null;

            try
            {
                stream = await file.OpenAsync(FileAccessMode.Read).AsTask().ConfigureAwait(false);
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
