using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EditManifest
{
    public static class Program
    {
        private enum OSVersion
        {
            Unknown = -1,
            UWP
        }

        private static void Main(string[] args)
        {
            String ConfigMode = "Release";
            String FilePath = String.Empty;
            OSVersion OSVersion = OSVersion.Unknown;

            if (args == null || args.Length < 1)
            {
                Console.WriteLine("Missing arguments!!");
                Environment.ExitCode = 1;
            }
            else
            {
                FilePath = args[0];

                if (args.Length > 1)
                {
                    ConfigMode = args[1];
                }

                Console.WriteLine("File path: {0}", FilePath);
                Console.WriteLine("Config mode: {0}", ConfigMode);

                if (FilePath.EndsWith("Package.appxmanifest", StringComparison.InvariantCultureIgnoreCase))
                {
                    OSVersion = OSVersion.UWP;
                }

                if (OSVersion == OSVersion.Unknown)
                {
                    Console.WriteLine("Unknown manifest filetype");
                    Environment.ExitCode = 1;
                    return;
                }

                StreamReader sReader = null;
                StreamWriter sWriter = null;

                try
                {
                    var sBuilder = new StringBuilder();
                    var fStream = File.Open(FilePath, FileMode.Open, FileAccess.ReadWrite);
                    sReader = new StreamReader(fStream);
                    sWriter = new StreamWriter(fStream);

                    while (!sReader.EndOfStream)
                    {
                        var line = sReader.ReadLine();
                        if (OSVersion == OSVersion.UWP)
                        {
                            if ("Debug".Equals(ConfigMode))
                            {
                                line = line.Replace("Name=\"49586DaveAntoine.SimpleWeather-Asimpleweatherapp\"", "Name=\"49586DaveAntoine.SimpleWeatherDebug\"");
                            }
                            else
                            {
                                line = line.Replace("Name=\"49586DaveAntoine.SimpleWeatherDebug\"", "Name=\"49586DaveAntoine.SimpleWeather-Asimpleweatherapp\"");
                            }
                        }

                        sBuilder.AppendLine(line);
                    }

                    // Wipe file
                    fStream.SetLength(0);

                    // Write to file
                    sWriter.Write(sBuilder.ToString());
                    sWriter.Flush();
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine("File \"{0}\" does not exist!!", FilePath);
                    Environment.ExitCode = 1;
                }
                catch (Exception)
                {
                    Console.WriteLine("Error accessing file!!");
                    Environment.ExitCode = 1;
                }
                finally
                {
                    sWriter?.Dispose();
                    sReader?.Dispose();
                }
            }
        }
    }
}
