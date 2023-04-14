using System.Text.RegularExpressions;
using System.Xml;
using Mako.IoT.NFVersionInspector.Extensions;

namespace Mako.IoT.NFVersionInspector.Services
{
    public class NuspecParser : INuspecParser
    {
        private static readonly Regex DescRegex =
            new Regex(@"([\w\.]+)\sv([\d\.]+)\s\(checksum\s([\dA-Fx]+)\)", RegexOptions.Compiled);
        public Package Parse(TextReader reader)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(reader);
            var nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            string? ns = xmlDoc.FirstChild?.NextSibling?.NamespaceURI; //get namespace from <package> node
            if (ns != null) nsmgr.AddNamespace("n", ns); 

            var package = new Package(
                xmlDoc.GetSingleNode("n:package/n:metadata/n:id", nsmgr).InnerText,
                xmlDoc.GetSingleNode("n:package/n:metadata/n:version", nsmgr).InnerText,
                ParseDependencies(xmlDoc.SelectSingleNode("n:package/n:metadata/n:dependencies", nsmgr), nsmgr)
                    .AddIfNotNull(ParseDescription(xmlDoc.GetSingleNode("n:package/n:metadata/n:description", nsmgr).InnerText))
                );

            return package;
        }

        private Package? ParseDescription(string description)
        {
            var m = DescRegex.Match(description);
            return m.Success ? new Package(m.Groups[1].Value, m.Groups[2].Value, m.Groups[3].Value) : null;
        }

        private IList<Package> ParseDependencies(XmlNode? node, XmlNamespaceManager nsmgr)
        {
            return node?.GetNodes("n:dependency", nsmgr).Select(n =>
                       new Package(n.GetAttribute("id").Value, n.GetAttribute("version").Value)).ToList()
                   ?? new List<Package>();
        }
    }
}
