using System.IO;

namespace SimpleWeather.Utils
{
    public static partial class FileUtils
    {
        public static bool IsValid(string FilePath)
        {
            FileInfo fileinfo = new FileInfo(FilePath);

            return fileinfo.Exists && fileinfo.Length > 0;
        }
    }
}
