using System;
using System.Collections.Generic;
using System.IO;
using NuGet.Frameworks;

namespace gen.Internal
{
    internal class AssemblyFilter : IAssemblyFilter
    {
        public IEnumerable<string> Filter(IEnumerable<string> assemblyPaths, string targetFrameworkStr)
        {
            NuGetFramework targetFramework;
            if (!TryParseTargetFramework(targetFrameworkStr, out targetFramework))
                throw new ArgumentException("Invalid target framework value.");

            var compatibilityProvider = new CompatibilityProvider(new DefaultFrameworkNameProvider());

            var result = new LinkedList<string>();
            foreach (var assemblyPath in assemblyPaths)
            {
                NuGetFramework framework;
                if (!TryGetTargetFrameworkFromPath(assemblyPath, out framework))
                    continue;

                var isCompatible = compatibilityProvider.IsCompatible(targetFramework, framework);
                if (!isCompatible)
                {
                    var expandedFrameworks = ExpandTargetFramework(framework);
                    foreach (var expandedFramework in expandedFrameworks)
                    {
                        isCompatible = compatibilityProvider.IsCompatible(targetFramework, expandedFramework);
                        if (isCompatible)
                            break;
                    }
                }

                if (isCompatible)
                    result.AddLast(assemblyPath);
            }

            return result;
        }

        private static bool TryParseTargetFramework(string targetFramework, out NuGetFramework framework)
        {
            framework = NuGetFramework.ParseFolder(targetFramework);
            return !NuGetFramework.UnsupportedFramework.Equals(framework);
        }

        private static bool TryGetTargetFrameworkFromPath(string assemblyPath, out NuGetFramework framework)
        {
            var dirPath = Path.GetDirectoryName(assemblyPath);
            var dir = Path.GetFileName(dirPath);

            framework = NuGetFramework.ParseFolder(dir);
            return !NuGetFramework.UnsupportedFramework.Equals(framework);
        }

        private static ISet<NuGetFramework> ExpandTargetFramework(NuGetFramework targetFramework)
        {
            var expander = new FrameworkExpander();
            return new HashSet<NuGetFramework>(expander.Expand(targetFramework));
        }
    }
}
