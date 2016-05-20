using System.Collections.Generic;

namespace gen
{
    public interface IAssemblyFilter
    {
        IEnumerable<string> Filter(IEnumerable<string> assemblyPaths, string targetFramework);
    }
}
