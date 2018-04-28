using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditManifest
{
    class Program
    {
        enum OSVersion
        {
            Unknown = -1,
            Android = 0,
            UWP
        }

        static void Main(string[] args)
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

                if (FilePath.EndsWith("AndroidManifest.xml", StringComparison.InvariantCultureIgnoreCase))
                {
                    OSVersion = OSVersion.Android;
                }
                else if (FilePath.EndsWith("Package.appxmanifest", StringComparison.InvariantCultureIgnoreCase))
                {
                    OSVersion = OSVersion.UWP;
                }

                if (OSVersion == OSVersion.Unknown)
                {
                    Console.WriteLine("Unknown manifest filetype", FilePath);
                    Environment.ExitCode = 1;
                    return;
                }

                try
                {
                    var fStream = File.Open(FilePath, FileMode.Open, FileAccess.ReadWrite);
                    StringBuilder sBuilder = new StringBuilder();

                    using (StreamReader sReader = new StreamReader(fStream))
                    using (StreamWriter sWriter = new StreamWriter(fStream))
                    {
                        while (!sReader.EndOfStream)
                        {
                            var line = sReader.ReadLine();
                            if (OSVersion == OSVersion.Android)
                            {
                                if ("Debug".Equals(ConfigMode))
                                {
                                    line = line.Replace("package=\"com.thewizrd.simpleweather\"", "package=\"com.thewizrd.simpleweather_debug\"");
                                    line = line.Replace("com.thewizrd.simpleweather.", "com.thewizrd.simpleweather_debug.");
                                }
                                else
                                {
                                    line = line.Replace("package=\"com.thewizrd.simpleweather_debug\"", "package=\"com.thewizrd.simpleweather\"");
                                    line = line.Replace("com.thewizrd.simpleweather_debug.", "com.thewizrd.simpleweather.");
                                }
                            }
                            else if (OSVersion == OSVersion.UWP)
                            {
                                if ("Debug".Equals(ConfigMode))
                                {
                                    line = line.Replace("Name=\"49586DaveAntoine.SimpleWeather - Asimpleweatherapp\"", "Name=\"49586DaveAntoine.SimpleWeather - Asimpleweatherapp_debug\"");
                                }
                                else
                                {
                                    line = line.Replace("Name=\"49586DaveAntoine.SimpleWeather - Asimpleweatherapp_debug\"", "Name=\"49586DaveAntoine.SimpleWeather - Asimpleweatherapp\"");
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
            }
        }
    }
}
