using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KeyCheck
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            // Set default exit code as success
            Environment.ExitCode = 0;

            if (args == null || args.Length == 0)
            {
                Console.WriteLine("Missing arguments!!");
                Environment.ExitCode = 1;
            }
            else
            {
                var DirectoryPath = args[0];

                if (!Directory.Exists(DirectoryPath))
                {
                    Console.WriteLine("Directory \"{0}\" does not exist!!", DirectoryPath);
                    Environment.ExitCode = 1;
                }
                else
                {
                    var KeyFiles = Directory.EnumerateFiles(DirectoryPath, "*.cs", SearchOption.TopDirectoryOnly);
                    string[] ExcludeFiles = null;

                    if (args.Length == 2)
                    {
                        ExcludeFiles = args[1].Split(',');
                    }

                    foreach (var FilePath in KeyFiles)
                    {
                        try
                        {
                            var info = new FileInfo(FilePath);
                            if (ExcludeFiles != null && (ExcludeFiles.Contains(info.Name) || ExcludeFiles.Contains(info.FullName)))
                            {
                                Console.WriteLine("Skipping file path: {0}", FilePath);
                                continue;
                            }

                            Console.WriteLine("File path: {0}", FilePath);
                            var content = File.ReadAllText(FilePath);
                            if (content.Contains("return null;") ||
                                !Regex.Match(content, "return \".*\";", RegexOptions.IgnoreCase).Success)
                            {
                                Console.WriteLine("Key missing from file!!");
                                Environment.ExitCode = 1;
                            }
                            else
                            {
                                Console.WriteLine("Key found!!");
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

            Environment.Exit(Environment.ExitCode);
        }
    }
}
