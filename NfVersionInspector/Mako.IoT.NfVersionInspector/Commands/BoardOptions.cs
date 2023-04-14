
using CommandLine;

namespace Mako.IoT.NFVersionInspector.Commands
{
    [Verb("board", HelpText = "Get native assemblies installed on board. If board name is specified, stores result in a file.")]
    public class BoardOptions
    {
        [Option('p', "port", Required = false, HelpText = "COM port")]
        public string Port { get; set; }

        [Option('n', "name", Required = false, HelpText = "board name")]
        public string Name { get; set; }

        [Option('l', "list", Required = false, HelpText = "list saved boards")]
        public bool List { get; set; }
    }

}
