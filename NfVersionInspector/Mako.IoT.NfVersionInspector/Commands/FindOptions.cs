using CommandLine;

namespace Mako.IoT.NFVersionInspector.Commands
{
    [Verb("find", HelpText = "Finds nuget package version compatible with installed native assemblies.")]
    public class FindOptions
    {
        [Option('p', "port", HelpText = "COM port")]
        public string Port { get; set; }
        [Option('n', "name", HelpText = "board name")]
        public string BoardName { get; set; }
        [Option('i', "id", HelpText = "nuget package ID")]
        public string PackageId { get; set; }
        [Option('r', "refresh", HelpText = "refresh package cache")]
        public bool RefreshCache { get; set; }
    }
}
