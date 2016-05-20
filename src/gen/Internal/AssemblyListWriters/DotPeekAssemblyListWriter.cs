using System.Collections.Generic;
using System.Text;

namespace gen.Internal.AssemblyListWriters
{
    internal class DotPeekAssemblyListWriter : IAssemblyListWriter
    {
        public string GetFileExtension()
        {
            return ".dpl";
        }

        public string Write(IEnumerable<string> assemblyPaths)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<AssemblyExplorer>");

            foreach (var assemblyPath in assemblyPaths)
            {
                sb.Append('\t');
                sb.Append("<Assembly Path=\"");
                sb.Append(assemblyPath);
                sb.AppendLine("\" />");
            }

            sb.Append("</AssemblyExplorer>");

            return sb.ToString();
        }
    }
}
