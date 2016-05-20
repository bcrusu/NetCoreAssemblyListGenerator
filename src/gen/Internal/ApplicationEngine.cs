using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace gen.Internal
{
    public class ApplicationEngine : IApplicationEngine
    {
        private const string DefaultAssemblyListWriter = "dotPeek";
        private const string DefaultOutFileName = "assemblyList";
        private const string DefaultTfm = "netstandard1.5";

        private readonly IAssemblyFinder _assemblyFinder;
        private readonly IAssemblyFilter _assemblyFilter;
        private readonly IAssemblyListWriterFactory _assemblyListWriterFactory;

        public ApplicationEngine(IAssemblyFinder assemblyFinder, 
            IAssemblyFilter assemblyFilter,
            IAssemblyListWriterFactory assemblyListWriterFactory)
        {
            _assemblyFinder = assemblyFinder;
            _assemblyFilter = assemblyFilter;
            _assemblyListWriterFactory = assemblyListWriterFactory;
        }

        public void Run(string inDirectory, string outDirectory, string targetFramework, string listFormat)
        {
            if (string.IsNullOrWhiteSpace(inDirectory))
            {
                var userHomePath = GetUserHomePath();
                if (string.IsNullOrWhiteSpace(userHomePath))
                    throw new ArgumentException("Could not get user home path from environment variables. Please provide it via the 'in' option.", nameof(inDirectory));

                inDirectory = Path.Combine(userHomePath, ".nuget");
            }

            if (string.IsNullOrWhiteSpace(targetFramework))
                targetFramework = DefaultTfm;

            if (string.IsNullOrWhiteSpace(outDirectory))
                outDirectory = Directory.GetCurrentDirectory();

            if (string.IsNullOrWhiteSpace(listFormat))
                listFormat = DefaultAssemblyListWriter;

            var assemblyListWriter = _assemblyListWriterFactory.Create(listFormat);
            if (assemblyListWriter == null)
                throw new ArgumentException("Invalid output format value.", nameof(targetFramework));

            // find and filter assemblies
            var assemblyPaths = _assemblyFinder.FindAssemblies(inDirectory);
            assemblyPaths = _assemblyFilter.Filter(assemblyPaths, targetFramework);
            assemblyPaths = assemblyPaths.OrderBy(x => x).ToList();

            // render and write to disk
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
