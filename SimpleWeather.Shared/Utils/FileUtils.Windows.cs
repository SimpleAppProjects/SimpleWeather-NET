#if WINDOWS
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
        public static Task<String> ReadFile(StorageFile file)
        {
            return Task.Run(async () =>
            {
                if (file is null)
                {
                    return null;
                }

                // Wait for file to be free
                while (IsFileLocked(file))
                {
                    await Task.Delay(100);
                }

                String data;

                using (var fStream = (await file.OpenAsync(FileAccessMode.Read)).AsStreamForRead())
                using (StreamReader reader = new StreamReader(fStream))
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
            });
        }

        public static Task WriteFile(String data, StorageFile file)
        {
            return Task.Run(async () =>
            {
                // Wait for file to be free
                while (IsFileLocked(file))
                {
                    await Task.Delay(100);
                }

                using (StorageStreamTransaction transaction = await file.OpenTransactedWriteAsync())
                using (DataWriter writer = new DataWriter(transaction.Stream))
                {
                    writer.WriteString(data);
                    // reset stream size to override the file
                    transaction.Stream.Size = await writer.StoreAsync();
                    await transaction.CommitAsync();
                }
            });
        }

        public static Task WriteFile(Byte[] data, StorageFile file)
        {
            return Task.Run(async () =>
            {
                // Wait for file to be free
                while (IsFileLocked(file))
                {
                    await Task.Delay(100);
                }

                using (StorageStreamTransaction transaction = await file.OpenTransactedWriteAsync())
                using (DataWriter writer = new DataWriter(transaction.Stream))
                {
                    writer.WriteString(Encoding.UTF8.GetString(data));
                    // reset stream size to override the file
                    transaction.Stream.Size = await writer.StoreAsync();
                    await transaction.CommitAsync();
                }
            });
        }

        public static bool IsFileLocked(StorageFile file)
        {
            if (!File.Exists(file?.Path))
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

        public static Task<bool> DeleteDirectory(String path)
        {
            if (Directory.Exists(path))
            {
                return Task.Run(async () =>
                {
                    bool success = true;

                    try
                    {
                        var directory = await StorageFolder.GetFolderFromPathAsync(path);
                        await directory.DeleteAsync(StorageDeleteOption.PermanentDelete);
                    }
                    catch (Exception e)
                    {
                        Logger.WriteLine(LoggerLevel.Error, e);
                        success = false;
                    }

                    return success;
                });
            }

            return Task.FromResult(false);
        }
    }
}
#endif