using Mako.IoT.NFVersionInspector.Extensions;
using Mako.IoT.NFVersionInspector.Services;

namespace Mako.IoT.NFVersionInspector.Commands
{
    public class BoardCommand
    {
        private readonly IDeviceExplorer _deviceExplorer;
        private readonly IStorage _storage;

        public BoardCommand(IDeviceExplorer deviceExplorer, IStorage storage)
        {
            _deviceExplorer = deviceExplorer;
            _storage = storage;
        }

        public int Execute(BoardOptions options)
        {
            if (options.List)
            {
                foreach (var boardName in _storage.ListBoardInfo())
                {
                    Console.WriteLine(boardName);
                }
            }

            if (!String.IsNullOrWhiteSpace(options.Port))
            {
                var info = _deviceExplorer.GetBoardInfo(options.Port);

                Console.WriteLine(info);

                if (!String.IsNullOrWhiteSpace(options.Name))
                {
                    var packages = info.NativePackages();

                    _storage.SaveBoardInfo(options.Name, packages);
                }
            }

            return 0;
        }
    }
}
