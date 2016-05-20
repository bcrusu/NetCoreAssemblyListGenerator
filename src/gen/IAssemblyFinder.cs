using System.Collections.Generic;

namespace gen
{
    public interface IAssemblyFinder
    {
        IEnumerable<string> FindAssemblies(string rootPath);
    }
}
