using Mako.IoT.NFVersionInspector.Extensions;
using Mako.IoT.NFVersionInspector.Services;

namespace Mako.IoT.NFVersionInspector.Commands
{
    public class ProjCommand
    {
        private readonly IStorage _storage;
        private readonly IDependencyFinder _dependencyFinder;
        private readonly IDeviceExplorer _deviceExplorer;

        public ProjCommand(IStorage storage, IDependencyFinder dependencyFinder, IDeviceExplorer deviceExplorer)
        {
            _storage = storage;
            _dependencyFinder = dependencyFinder;
            _deviceExplorer = deviceExplorer;
        }

        public int Execute(ProjOptions options)
        {
            var existingPackages = _dependencyFinder.FindPackages(options.Path).Flatten().ToArray();
            Console.WriteLine();
            Console.WriteLine("Assemblies referenced in the solution:");
            OutputPackages(existingPackages);

            if (!String.IsNullOrWhiteSpace(options.Port) || !String.IsNullOrWhiteSpace(options.BoardName))
            {
                Console.WriteLine();
                Console.WriteLine("Checking board compatibility...");

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

                foreach (var projectPackage in existingPackages.Where(p=>!p.IsNative))
                {
                    Console.Write($"Checking {projectPackage.Id} {projectPackage.Version} - ");
                    if (!_dependencyFinder.GetDependenciesFromNuget(projectPackage.Id, projectPackage.Version).Flatten()
                        .Where(p=>p.IsNative)
                        .Except(nativePackages)
                        .Any())
                        
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("OK ");

                        if (options.UpgradeCheck)
                        {
                            var compatibleVersion =
                                _dependencyFinder.FindCompatibleVersion(projectPackage.Id, nativePackages, false, true);

                            if (compatibleVersion != null && compatibleVersion != projectPackage.Version)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.Write($"(possible upgrade to {compatibleVersion})");
                            }
                        }

                        Console.WriteLine();

                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write($"Not compatible ");
                        Console.ForegroundColor = ConsoleColor.White;

                        var compatibleVersion =
                            _dependencyFinder.FindCompatibleVersion(projectPackage.Id, nativePackages, false, true);

                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(compatibleVersion == null
                            ? "(no compatible version found)"
                            : $"(upgrade to {compatibleVersion})");
                    }

                    Console.ForegroundColor = ConsoleColor.White;

                }
            }

            return 0;
        }

        static void OutputPackages(IEnumerable<Package> packages)
        {
            foreach (var package in packages)
            {
                Console.WriteLine($"{package.Id} {package.Version} {package.Checksum}");
            }
        }
    }
}
