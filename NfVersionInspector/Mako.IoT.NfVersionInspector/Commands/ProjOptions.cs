
using CommandLine;

namespace Mako.IoT.NFVersionInspector.Commands
{
    [Verb("proj", HelpText = "Display all referenced nuget packages and native assemblies used in solution.")]
    public class ProjOptions
    {
        [Option('t', "path",Default = true, Required = true, HelpText = "solution path")]
        public string Path { get; set; }
        [Option('p', "port", Required = false, HelpText = "COM port")]
        public string Port { get; set; }

        [Option('n', "name", Required = false, HelpText = "board name")]
        public string BoardName { get; set; }
        [Option('u', "upgrade", Required = false, HelpText = "check packages for upgrade")]
        public bool UpgradeCheck { get; set; }
    }

}
