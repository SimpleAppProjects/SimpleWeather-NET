﻿using System;
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
            if (args == null || args.Length < 1)
            {
                Console.WriteLine("Missing arguments!!");
                Environment.ExitCode = 1;
            }
            else
            {
                foreach (var FilePath in args)
                {
                    try
                    {
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
                    finally
                    {
                        if (Environment.ExitCode != 0)
                            Environment.Exit(Environment.ExitCode);
                    }
                }
            }
        }
    }
}