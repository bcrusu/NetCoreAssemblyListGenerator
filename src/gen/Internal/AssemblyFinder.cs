using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

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
                .Where(IsNotReferenceAssemblyPath);

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

        private static bool IsNotReferenceAssemblyPath(string filePath)
        {
            var tmp = Path.GetDirectoryName(filePath);
            if (tmp != null)
            {
                tmp = Path.GetDirectoryName(tmp);
                if (tmp != null)
                {
                    tmp = Path.GetFileName(tmp);
                    if (tmp != null)
                        return !string.Equals(tmp, "ref", StringComparison.OrdinalIgnoreCase);
                }
            }

            // if path does not point to a nuget package directory
            return true;
        }
    }
}
