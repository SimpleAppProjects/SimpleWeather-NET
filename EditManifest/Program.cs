﻿using System;
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
                if (args.Length > 2)
                {
                    var osVersion = args[2];
                    OSVersion = osVersion switch
                    {
                        "WinUI" or "UWP" => OSVersion.UWP,
                        "Mac" or "iOS" or "MaciOS" => OSVersion.MaciOS,
                        _ => OSVersion.Unknown
                    };
                }

                Console.WriteLine("File path: {0}", FilePath);
                Console.WriteLine("Config mode: {0}", ConfigMode);

                if (OSVersion == OSVersion.Unknown)
                {
                    if (FilePath.EndsWith("Package.appxmanifest", StringComparison.InvariantCultureIgnoreCase))
                    {
                        OSVersion = OSVersion.UWP;
                    }
                    else if (FilePath.EndsWith("Info.plist", StringComparison.InvariantCultureIgnoreCase) ||
                        FilePath.EndsWith("Entitlements.plist", StringComparison.InvariantCultureIgnoreCase))
                    {
                        OSVersion = OSVersion.MaciOS;
                    }
                }

                Console.WriteLine("OSVersion: {0}", OSVersion);

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
                    sWriter = new StreamWriter(fStream, Encoding.UTF8);

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
								// Background Task
                                line = line.Replace("E3E44B22-74AE-47CE-A507-6EBE2F832B8F", "148C5627-665B-4DAC-AB27-64397E80335A");
								// Widget Provider
                                line = line.Replace("1D453922-87B3-41AB-9D61-1A73C4360E71", "09FC0937-C728-40D3-8C0E-AAB1AA0C61C8");
                                line = line.Replace("Id=\"Weather_Widget\"", "Id=\"Weather_Widget_Debug\"");
                                line = line.Replace("Id=\"SimpleWeatherWidgetApp\"", "Id=\"SimpleWeatherDebugWidgetApp\"");
                            }
                            else
                            {
                                line = line.Replace("Name=\"49586DaveAntoine.SimpleWeatherDebug\"", "Name=\"49586DaveAntoine.SimpleWeather-Asimpleweatherapp\"");
                                line = line.Replace("S-1-15-2-537857025-621772652-3741139397-2615399211-928015427-4269635983-3021255191", "S-1-15-2-2670365806-2911716598-2260801434-585721284-1716833224-1671719374-3724409756");
                                line = line.Replace("DisplayName=\"SimpleWeather (Debug)\"", "DisplayName=\"SimpleWeather\"");
								// Background Task
                                line = line.Replace("148C5627-665B-4DAC-AB27-64397E80335A", "E3E44B22-74AE-47CE-A507-6EBE2F832B8F");
								// Widget Provider
                                line = line.Replace("09FC0937-C728-40D3-8C0E-AAB1AA0C61C8", "1D453922-87B3-41AB-9D61-1A73C4360E71");
                                line = line.Replace("Id=\"Weather_Widget_Debug\"", "Id=\"Weather_Widget\"");
                                line = line.Replace("Id=\"SimpleWeatherDebugWidgetApp\"", "Id=\"SimpleWeatherWidgetApp\"");
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
