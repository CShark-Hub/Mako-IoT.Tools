
using System.Xml;

namespace Mako.IoT.NFVersionInspector.Extensions
{
    public static class XmlExtensions
    {
        public static XmlNode GetSingleNode(this XmlNode node, string xpath, XmlNamespaceManager nsmgr)
        {
            return node.SelectSingleNode(xpath, nsmgr) ?? throw new Exception($"Node {xpath} not found");
        }

        public static IEnumerable<XmlNode> GetNodes(this XmlNode node, string xpath, XmlNamespaceManager nsmgr)
        {
            return node.SelectNodes(xpath, nsmgr)?.Cast<XmlNode>() ?? Array.Empty<XmlNode>();
        }

        public static XmlAttribute GetAttribute(this XmlNode node, string name)
        {
            return node.Attributes?[name] ?? throw new Exception($"Attribute {name} not found");
        }

    }
}
