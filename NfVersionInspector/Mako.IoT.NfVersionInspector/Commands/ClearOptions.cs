using CommandLine;

namespace Mako.IoT.NFVersionInspector.Commands
{
    [Verb("clear", HelpText = "Clears saved data.")]
    public class ClearOptions
    {
        [Option('p', "packages", HelpText = "packages cache")]
        public bool ClearPackages { get; set; }

        [Option('b', "boards", HelpText = "saved boards")]
        public bool ClearBoards { get; set; }
        
    }
}
