
using CommandLine;

namespace Mako.IoT.NFVersionInspector.Commands
{
    [Verb("check", HelpText = "Checks if nuget package is compatible with installed native packages.")]
    public class CheckOptions
    {
        [Option('p', "port", HelpText = "COM port")]
        public string Port { get; set; }
        [Option('n', "name", HelpText = "board name")]
        public string BoardName { get; set; }
        [Option('i', "id", HelpText = "nuget package ID")]
        public string PackageId { get; set; }
        [Option('v', "version", HelpText = "package version")]
        public string PackageVersion { get; set; }
        [Option('r', "refresh", HelpText = "refresh package cache")]
        public bool RefreshCache { get; set; }
    }
}
