using System.Collections.Generic;
using System.Text;

namespace gen.Internal.AssemblyListWriters
{
    internal class PlainTextAssemblyListWriter : IAssemblyListWriter
    {
        public string GetFileExtension()
        {
            return ".txt";
        }

        public string Write(IEnumerable<string> assemblyPaths)
        {
            var sb = new StringBuilder();

            foreach (var assemblyPath in assemblyPaths)
            {
                sb.AppendLine(assemblyPath);
            }

            return sb.ToString();
        }
    }
}
