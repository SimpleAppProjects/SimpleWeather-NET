using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace SimpleWeather.Utils
{
    public static partial class FileUtils
    {
        public async static Task<String> ReadFile(StorageFile file)
        {
            // Wait for file to be free
            while (IsFileLocked(file))
            {
                await Task.Delay(100);
            }

            String data;

            using (StreamReader reader = new StreamReader((await file.OpenAsync(FileAccessMode.Read)).AsStreamForRead()))
            {
                String line = await reader.ReadLineAsync();
                StringBuilder sBuilder = new StringBuilder();

                while (line != null)
                {
                    sBuilder.Append(line).Append("\n");
                    line = await reader.ReadLineAsync();
                }

                reader.Dispose();
                data = sBuilder.ToString();
            }

            return data;
        }

        public static async Task WriteFile(String data, StorageFile file)
        {
            // Wait for file to be free
            while (IsFileLocked(file))
            {
                await Task.Delay(100);
            }

            using (Stream stream = (await file.OpenAsync(FileAccessMode.ReadWrite)).AsStreamForWrite())
            using (StreamWriter writer = new StreamWriter(stream))
            {
                // Clear file before writing
                stream.SetLength(0);

                await writer.WriteAsync(data);
                await writer.FlushAsync();
                writer.Dispose();
            }
        }

        public static async Task WriteFile(Byte[] data, StorageFile file)
        {
            // Wait for file to be free
            while (IsFileLocked(file))
            {
                await Task.Delay(100);
            }

            using (Stream stream = (await file.OpenAsync(FileAccessMode.ReadWrite)).AsStreamForWrite())
            using (StreamWriter writer = new StreamWriter(stream))
            {
                // Clear file before writing
                stream.SetLength(0);

                await writer.WriteAsync(Encoding.UTF8.GetString(data));
                await writer.FlushAsync();
                writer.Dispose();
            }
        }

        public static bool IsFileLocked(StorageFile file)
        {
            if (!File.Exists(file.Path))
                return false;

            IRandomAccessStream stream = null;

            try
            {
                Task<IRandomAccessStream> t = file.OpenAsync(FileAccessMode.Read).AsTask();
                t.Wait();
                stream = t.Result;
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
