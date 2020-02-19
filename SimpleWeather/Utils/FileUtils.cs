using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.Utils
{
    public static partial class FileUtils
    {
        public static bool IsValid(string FilePath)
        {
            FileInfo fileinfo = new FileInfo(FilePath);

            return fileinfo.Exists && fileinfo.Length > 0;
        }

        public async static Task<String> ReadFile(string FilePath)
        {
            if (FilePath is null || !File.Exists(FilePath))
            {
                return null;
            }

            // Wait for file to be free
            while (IsFileLocked(FilePath))
            {
                await AsyncTask.RunAsync(Task.Delay(100));
            }

            using (StreamReader reader = new StreamReader(File.Open(FilePath, FileMode.Open)))
            {
                String line = await AsyncTask.RunAsync(reader.ReadLineAsync);
                StringBuilder sBuilder = new StringBuilder();

                while (line != null)
                {
                    sBuilder.Append(line).Append("\n");
                    line = await AsyncTask.RunAsync(reader.ReadLineAsync);
                }

                reader.Dispose();
                return sBuilder.ToString();
            }
        }

        public static async Task WriteFile(String data, String filePath)
        {
            // Wait for file to be free
            while (IsFileLocked(filePath))
            {
                await AsyncTask.RunAsync(Task.Delay(100));
            }

            using (var fileStream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write))
            using (var writer = new StreamWriter(fileStream))
            {
                // reset stream size to override the file
                //writer.BaseStream.SetLength(0);
                await writer.WriteAsync(data);
                await writer.FlushAsync();
            }
        }

        public static async Task WriteFile(Byte[] data, String filePath)
        {
            // Wait for file to be free
            while (IsFileLocked(filePath))
            {
                await AsyncTask.RunAsync(Task.Delay(100));
            }

            using (var fileStream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write))
            using (var writer = new StreamWriter(fileStream))
            {
                // reset stream size to override the file
                //writer.BaseStream.SetLength(0);
                await writer.WriteAsync(Encoding.UTF8.GetString(data));
                await writer.FlushAsync();
            }
        }

        public static bool IsFileLocked(string filePath)
        {
            if (!File.Exists(filePath))
                return false;

            Stream stream = null;

            try
            {
                stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
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

#if !WINDOWS_UWP
        public static Task DeleteDirectory(String path)
        {
            var directory = new DirectoryInfo(path);
            directory.Delete(true);
        }
#endif
    }
}