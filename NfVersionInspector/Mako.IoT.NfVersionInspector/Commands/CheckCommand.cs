
using Mako.IoT.NFVersionInspector.Extensions;
using Mako.IoT.NFVersionInspector.Services;

namespace Mako.IoT.NFVersionInspector.Commands
{
    public class CheckCommand
    {
        private readonly IStorage _storage;
        private readonly IDependencyFinder _dependencyFinder;
        private readonly IDeviceExplorer _deviceExplorer;

        public CheckCommand(IStorage storage, IDependencyFinder dependencyFinder, IDeviceExplorer deviceExplorer)
        {
            _storage = storage;
            _dependencyFinder = dependencyFinder;
            _deviceExplorer = deviceExplorer;
        }

        public int Execute(CheckOptions options)
        {
            Package[]? nativePackages = null;

            if (!String.IsNullOrWhiteSpace(options.BoardName))
            {
                nativePackages = _storage.LoadBoardInfo(options.BoardName).ToArray();
            }
            else
            {
                if (String.IsNullOrWhiteSpace(options.Port))
                    throw new ArgumentNullException(nameof(options.Port));
                nativePackages = _deviceExplorer.GetBoardInfo(options.Port).NativePackages().ToArray();
            }

            var proposedPackages = _dependencyFinder.GetDependenciesFromNuget(options.PackageId, options.PackageVersion, options.RefreshCache).Flatten();

            foreach (var package in proposedPackages.Where(p=>p.IsNative))
            {
                Console.ForegroundColor = nativePackages.Contains(package) ? ConsoleColor.Green : ConsoleColor.Red;
                Console.WriteLine(package);
            }

            Console.ForegroundColor = ConsoleColor.White;

            return 0;
        }
    }
}
