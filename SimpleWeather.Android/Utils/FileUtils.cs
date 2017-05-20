using System;
using System.Text;
using System.Threading.Tasks;
using Java.IO;
using SimpleWeather.Droid;

namespace SimpleWeather.Utils
{
    public static partial class FileUtils
    {
        public async static Task<String> ReadFile(File file)
        {
            while (IsFileLocked(file))
            {
                await Task.Delay(100);
            }

            String data;

            using (BufferedReader reader = new BufferedReader(new FileReader(file)))
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

        public static async void WriteFile(String data, File file)
        {
            while(IsFileLocked(file))
            {
                await Task.Delay(100);
            }

            await Task.Run(async () =>
            {
                using (System.IO.Stream outputStream = App.Context.OpenFileOutput(file.Name, Android.Content.FileCreationMode.Private))
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(outputStream))
                {
                    await writer.WriteAsync(data);
                    await writer.FlushAsync();
                    writer.Close();
                }
            });
        }

        public static bool IsFileLocked(File file)
        {
            if (!file.Exists())
                return false;

            FileInputStream stream = null;

            try
            {
                stream = new FileInputStream(file);
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
