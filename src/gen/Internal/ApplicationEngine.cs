using System;
using System.IO;
using System.Runtime.InteropServices;

namespace gen.Internal
{
    public class ApplicationEngine : IApplicationEngine
    {
        private const string DefaultAssemblyListWriter = "dotPeek";
        private const string DefaultOutFileName = "assemblyList";

        private readonly IAssemblyFinder _assemblyFinder;
        private readonly IAssemblyListWriterFactory _assemblyListWriterFactory;

        public ApplicationEngine(IAssemblyFinder assemblyFinder, IAssemblyListWriterFactory assemblyListWriterFactory)
        {
            _assemblyFinder = assemblyFinder;
            _assemblyListWriterFactory = assemblyListWriterFactory;
        }

        public void Run(string inDirectory, string outDirectory, string listFormat)
        {
            if (string.IsNullOrWhiteSpace(inDirectory))
            {
                var userHomePath = GetUserHomePath();
                if (string.IsNullOrWhiteSpace(userHomePath))
                    throw new InvalidOperationException("Could not get user home path from environment variables. Please provide it via the 'in' option.");

                inDirectory = Path.Combine(userHomePath, ".nuget");
            }

            if (string.IsNullOrWhiteSpace(listFormat))
                listFormat = DefaultAssemblyListWriter;

            if (string.IsNullOrWhiteSpace(outDirectory))
                outDirectory = Directory.GetCurrentDirectory();


            var assemblyPaths = _assemblyFinder.FindAssemblies(inDirectory);
            var assemblyListWriter = _assemblyListWriterFactory.Create(listFormat);

            var listContents = assemblyListWriter.Write(assemblyPaths);

            var outputFileName = DefaultOutFileName + assemblyListWriter.GetFileExtension();
            var outputPath = Path.Combine(outDirectory, outputFileName);

            File.WriteAllText(outputPath, listContents);
        }

        private static string GetUserHomePath()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var userProfile = Environment.GetEnvironmentVariable("USERPROFILE");
                if (!string.IsNullOrWhiteSpace(userProfile))
                    return userProfile;

                var homeDrive = Environment.GetEnvironmentVariable("HOMEDRIVE");
                if (string.IsNullOrWhiteSpace(homeDrive))
                    return null;

                var homePath = Environment.GetEnvironmentVariable("HOMEPATH");
                if (string.IsNullOrWhiteSpace(homePath))
                    return null;

                return homeDrive + homePath;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
                RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return Environment.GetEnvironmentVariable("HOME");

            return null;
        }
    }
}
