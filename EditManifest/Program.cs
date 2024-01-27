using System;
using System.IO;
using System.Text;

namespace EditManifest
{
    public static class Program
    {
        private enum OSVersion
        {
            Unknown = -1,
            UWP,
            MaciOS
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
                else if (FilePath.EndsWith("Info.plist", StringComparison.InvariantCultureIgnoreCase) ||
                    FilePath.EndsWith("Entitlements.plist", StringComparison.InvariantCultureIgnoreCase))
                {
                    OSVersion = OSVersion.MaciOS;
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
                            if (ConfigMode?.StartsWith("Debug") == true)
                            {
                                line = line.Replace("Name=\"49586DaveAntoine.SimpleWeather-Asimpleweatherapp\"", "Name=\"49586DaveAntoine.SimpleWeatherDebug\"");
                                line = line.Replace("S-1-15-2-2670365806-2911716598-2260801434-585721284-1716833224-1671719374-3724409756", "S-1-15-2-537857025-621772652-3741139397-2615399211-928015427-4269635983-3021255191");
                                line = line.Replace("DisplayName=\"SimpleWeather\"", "DisplayName=\"SimpleWeather (Debug)\"");
                            }
                            else
                            {
                                line = line.Replace("Name=\"49586DaveAntoine.SimpleWeatherDebug\"", "Name=\"49586DaveAntoine.SimpleWeather-Asimpleweatherapp\"");
                                line = line.Replace("S-1-15-2-537857025-621772652-3741139397-2615399211-928015427-4269635983-3021255191", "S-1-15-2-2670365806-2911716598-2260801434-585721284-1716833224-1671719374-3724409756");
                                line = line.Replace("DisplayName=\"SimpleWeather (Debug)\"", "DisplayName=\"SimpleWeather\"");
                            }
                        }
                        else if (OSVersion == OSVersion.MaciOS)
                        {
                            if (FilePath.EndsWith("Info.plist", StringComparison.InvariantCultureIgnoreCase))
                            {
                                if (ConfigMode?.StartsWith("Debug") == true)
                                {
                                    line = line.Replace("<string>com.thewizrd.simpleweather</string>", "<string>com.thewizrd.simpleweather.debug</string>");
                                    line = line.Replace("<string>SimpleWeather</string>", "<string>SimpleWeather (Debug)</string>");
                                }
                                else
                                {
                                    line = line.Replace("<string>com.thewizrd.simpleweather.debug</string>", "<string>com.thewizrd.simpleweather</string>");
                                    line = line.Replace("<string>SimpleWeather (Debug)</string>", "<string>SimpleWeather</string>");
                                }
                            }
                            else if (FilePath.EndsWith("Entitlements.plist", StringComparison.InvariantCultureIgnoreCase))
                            {
                                if (ConfigMode?.StartsWith("Debug") == true)
                                {
                                    line = line.Replace("<string>group.com.thewizrd.simpleweather</string>", "<string>group.com.thewizrd.simpleweather.debug</string>");
                                }
                                else
                                {
                                    line = line.Replace("<string>group.com.thewizrd.simpleweather.debug</string>", "<string>group.com.thewizrd.simpleweather</string>");
                                }
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
