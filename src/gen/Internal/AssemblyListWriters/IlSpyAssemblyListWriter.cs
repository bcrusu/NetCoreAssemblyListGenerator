using System;
using System.Collections.Generic;
using System.Text;

namespace gen.Internal.AssemblyListWriters
{
    internal class IlSpyAssemblyListWriter : IAssemblyListWriter
    {
        public string GetFileExtension()
        {
            return ".xml";
        }

        public string Write(IEnumerable<string> assemblyPaths)
        {
            var sb = new StringBuilder();
            sb.Append("<List name=\"Generated_");
            sb.Append(DateTime.Now.Ticks);
            sb.AppendLine("\">");

            foreach (var assemblyPath in assemblyPaths)
            {
                sb.Append('\t');
                sb.Append("<Assembly>");
                sb.Append(assemblyPath);
                sb.AppendLine("</Assembly>");
            }

            sb.Append("</List>");

            return sb.ToString();
        }
    }
}
