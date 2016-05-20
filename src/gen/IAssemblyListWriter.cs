using System.Collections.Generic;

namespace gen
{
    public interface IAssemblyListWriter
    {
        string Write(IEnumerable<string> assemblyPaths);

        string GetFileExtension();
    }
}
