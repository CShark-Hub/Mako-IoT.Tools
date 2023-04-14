
using Mako.IoT.NFVersionInspector.Extensions;
using Mako.IoT.NFVersionInspector.Services;

namespace Mako.IoT.NFVersionInspector.Commands
{
    public class FindCommand
    {
        private readonly IStorage _storage;
        private readonly IDependencyFinder _dependencyFinder;
        private readonly IDeviceExplorer _deviceExplorer;

        public FindCommand(IStorage storage, IDependencyFinder dependencyFinder, IDeviceExplorer deviceExplorer)
        {
            _storage = storage;
            _dependencyFinder = dependencyFinder;
            _deviceExplorer = deviceExplorer;
        }

        public int Execute(FindOptions options)
        {
            IEnumerable<Package>? nativePackages = null;

            if (!String.IsNullOrWhiteSpace(options.BoardName))
            {
                nativePackages = _storage.LoadBoardInfo(options.BoardName);
            }
            else
            {
                if (String.IsNullOrWhiteSpace(options.Port))
                    throw new ArgumentNullException(nameof(options.Port));
                nativePackages = _deviceExplorer.GetBoardInfo(options.Port).NativePackages();
            }

            var version = _dependencyFinder.FindCompatibleVersion(options.PackageId, nativePackages, options.RefreshCache);
            if (version == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Compatible package {options.PackageId} not found");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Compatible package {options.PackageId} {version}");
            }

            Console.ForegroundColor = ConsoleColor.White;
            return 0;
        }
    }
}
