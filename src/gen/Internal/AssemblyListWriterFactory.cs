using System;
using System.Collections.Generic;
using gen.Internal.AssemblyListWriters;

namespace gen.Internal
{
    internal class AssemblyListWriterFactory : IAssemblyListWriterFactory
    {
        private static readonly IDictionary<string, IAssemblyListWriter> Registry = new Dictionary<string, IAssemblyListWriter>(StringComparer.OrdinalIgnoreCase)
        {
            {"ILSpy", new IlSpyAssemblyListWriter()},
            {"dotPeek", new IlSpyAssemblyListWriter()}
        };

        public IAssemblyListWriter Create(string name)
        {
            IAssemblyListWriter result;
            Registry.TryGetValue(name, out result);

            return result;
        }
    }
}
