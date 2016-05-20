using System;
using System.IO;
using Microsoft.Extensions.CommandLineUtils;

namespace gen
{
    public class Program
    {
        private static readonly CommandLineApplication Application = CreateCommandLineApplication();

        public static int Main(string[] args)
        {
            return Application.Execute(args);
        }

        private static int RunApplication()
        {
            string inDirectory;
            string outDirectory;
            string format;
            if (!TryGetOptionValues(out inDirectory, out outDirectory, out format))
                return -1;

            try
            {
                var applicationEngine = ServiceLocator.CreateApplicationEngine();
                applicationEngine.Run(inDirectory, outDirectory, format);
            }
            catch (Exception e)
            {
                Console.Write(e);
                return -1;
            }

            return 0;
        }

        private static bool TryGetOptionValues(out string inDirectory, out string outDirectory, out string format)
        {
            inDirectory = null;
            outDirectory = null;
            format = null;

            foreach (var option in Application.Options)
            {
                if (!option.HasValue())
                    continue;

                var optionValue = option.Values[0];
                switch (option.LongName)
                {
                    case "in":
                        if (!Directory.Exists(optionValue))
                        {
                            Console.WriteLine("Input directory does not exist.");
                            return false;
                        }
                        inDirectory = optionValue;
                        break;
                    case "out":
                        if (!Directory.Exists(optionValue))
                        {
                            Console.WriteLine("Output directory does not exist.");
                            return false;
                        }
                        outDirectory = optionValue;
                        break;
                    case "format":
                        format = optionValue;
                        break;
                }
            }

            return true;
        }

        private static CommandLineApplication CreateCommandLineApplication()
        {
            var result = new CommandLineApplication();

            result.Options.Add(new CommandOption("--in|-i <path>", CommandOptionType.SingleValue)
            {
                Description = "Input NuGet packages directory. Defaults to 'HOME_PATH\\.nuget'."
            });

            result.Options.Add(new CommandOption("--out|-o <path>", CommandOptionType.SingleValue)
            {
                Description = "Output directory. Defaults to the current directory."
            });

            result.Options.Add(new CommandOption("--format|-f <name>", CommandOptionType.SingleValue)
            {
                Description = "Output list format: dotPeek or ILSpy. Default is dotPeek."
            });

            result.Invoke = RunApplication;
            return result;
        }
    }
}
