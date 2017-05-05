using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace SimpleWeather.Utils
{
    public static class FileUtils
    {
        public async static Task<String> ReadFile(StorageFile file)
        {
            while (IsFileLocked(file))
            {
                await Task.Delay(100);
            }

            String data = await FileIO.ReadTextAsync(file);

            return data;
        }

        public static async void WriteFile(String data, StorageFile file)
        {
            while(IsFileLocked(file))
            {
                await Task.Delay(100);
            }

            await FileIO.WriteTextAsync(file, data);
        }

        public static async void WriteFile(Byte[] data, StorageFile file)
        {
            while (IsFileLocked(file))
            {
                await Task.Delay(100);
            }

            await FileIO.WriteBytesAsync(file, data);
        }

        public static bool IsFileLocked(StorageFile file)
        {
            if (!File.Exists(file.Path))
                return false;

            IRandomAccessStream stream = null;

            try
            {
                stream = file.OpenAsync(FileAccessMode.Read).AsTask().GetAwaiter().GetResult();
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
