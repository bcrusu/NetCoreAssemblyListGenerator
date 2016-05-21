using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace gen.Internal
{
    public class AssemblyFinder : IAssemblyFinder
    {
        private static readonly ISet<string> KnownAssemblyExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".dll",
            ".exe"
        };

        public IEnumerable<string> FindAssemblies(string rootPath)
        {
            return SearchDirectory(rootPath);
        }

        private IEnumerable<string> SearchDirectory(string directory)
        {
            var result = new List<string>();

            var assemblyPaths = Directory.GetFiles(directory)
                .Where(MatchAssemblyExtension)
                .Where(NotInNativeDirectory)
                .Where(NotInRuntimesDirectory)
                .Where(InLibDirectory);

            result.AddRange(assemblyPaths);

            foreach (var childDirectory in Directory.GetDirectories(directory))
                result.AddRange(SearchDirectory(childDirectory));

            return result;
        }

        private static bool MatchAssemblyExtension(string filePath)
        {
            if (!Path.HasExtension(filePath))
                return false;

            var extension = Path.GetExtension(filePath);
            return KnownAssemblyExtensions.Contains(extension);
        }

        private static bool InLibDirectory(string filePath)
        {
            var tmp = GetParentDir(filePath, 2);
            return string.Equals(tmp, "lib", StringComparison.OrdinalIgnoreCase);
        }

        private static bool NotInNativeDirectory(string filePath)
        {
            var tmp = GetParentDir(filePath, 1);
            return !string.Equals(tmp, "native", StringComparison.OrdinalIgnoreCase);
        }

        private static bool NotInRuntimesDirectory(string filePath)
        {
            var tmp = GetParentDir(filePath, 4);
            return !string.Equals(tmp, "runtimes", StringComparison.OrdinalIgnoreCase);
        }

        private static string GetParentDir(string filePath, int nthParent)
        {
            while (nthParent > 0)
            {
                filePath = Path.GetDirectoryName(filePath);
                if (filePath == null)
                    return null;

                nthParent--;
            }

            return Path.GetFileName(filePath);
        }
    }
}
