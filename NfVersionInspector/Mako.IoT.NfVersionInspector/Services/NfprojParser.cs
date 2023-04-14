using System.Text.RegularExpressions;
using System.Xml;
using Mako.IoT.NFVersionInspector.Extensions;

namespace Mako.IoT.NFVersionInspector.Services
{
    public class NfprojParser : INfprojParser
    {
        private static readonly Regex PathRegex = new Regex(@"^(.*\\)*packages\\([\w\.-]+)\\", RegexOptions.Compiled);

        public IEnumerable<string> GetPackagesPaths(TextReader reader)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(reader);
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("n", "http://schemas.microsoft.com/developer/msbuild/2003");

            var packages = new List<string>();
            foreach (var node in xmlDoc.GetNodes("//n:Reference/n:HintPath", nsmgr))
            {
                var m = PathRegex.Match(node.InnerText);
                if (m.Success)
                {
                    packages.Add(m.Groups[2].Value);
                }
            }

            return packages;
        }
    }
}
