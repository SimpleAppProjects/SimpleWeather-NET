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

        public static Task<String> ReadFile(string FilePath)
        {
            return Task.Run(async () =>
            {
                if (FilePath is null || !File.Exists(FilePath))
                {
                    return null;
                }

                // Wait for file to be free
                while (IsFileLocked(FilePath))
                {
                    await Task.Delay(100);
                }

                using (StreamReader reader = new StreamReader(File.Open(FilePath, FileMode.Open)))
                {
                    String line = await reader.ReadLineAsync();
                    StringBuilder sBuilder = new StringBuilder();

                    while (line != null)
                    {
                        sBuilder.Append(line).Append("\n");
                        line = await reader.ReadLineAsync();
                    }

                    reader.Dispose();
                    return sBuilder.ToString();
                }
            });
        }

        public static Task WriteFile(String data, String filePath)
        {
            return Task.Run(async () =>
            {
                // Wait for file to be free
                while (IsFileLocked(filePath))
                {
                    await Task.Delay(100);
                }

                using (var fileStream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write))
                using (var writer = new StreamWriter(fileStream))
                {
                    // reset stream size to override the file
                    //writer.BaseStream.SetLength(0);
                    await writer.WriteAsync(data);
                    await writer.FlushAsync();
                }
            });
        }

        public static Task WriteFile(Byte[] data, String filePath)
        {
            return Task.Run(async () =>
            {
                // Wait for file to be free
                while (IsFileLocked(filePath))
                {
                    await Task.Delay(100);
                }

                using (var fileStream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write))
                using (var writer = new StreamWriter(fileStream))
                {
                    // reset stream size to override the file
                    //writer.BaseStream.SetLength(0);
                    await writer.WriteAsync(Encoding.UTF8.GetString(data));
                    await writer.FlushAsync();
                }
            });
        }

        public static bool IsFileLocked(string filePath)
        {
            if (!File.Exists(filePath))
                return false;

            Stream stream = null;

            try
            {
                stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
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

#if false
        public static Task<bool> DeleteDirectory(String path)
        {
            if (Directory.Exists(path))
            {
                return Task.Run(() =>
                {
                    var directory = new DirectoryInfo(path);
                    directory.Delete(true);
                    return true;
                });
            }

            return Task.FromResult(false);
        }
#endif

        /*
        public static Task<string> CalculateMD5Hash(string filePath)
        {
            return Task.Run(() =>
            {
                if (!File.Exists(filePath))
                {
                    return null;
                }

                while (IsFileLocked(filePath))
                {
                    Task.Delay(100);
                }

                using var md5 = MD5.Create();
                using var stream = File.OpenRead(filePath);
                var hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            });
        }
        */
    }
}