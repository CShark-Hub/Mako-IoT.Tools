namespace Mako.IoT.NFVersionInspector.Services
{
    public class DependencyFinder : IDependencyFinder
    {
        private readonly IFileFinder _fileFinder;
        private readonly INfprojParser _nfprojParser;
        private readonly INuspecParser _nuspecParser;
        private readonly INugetClient _nugetClient;
        private readonly ICache _cache;

        public DependencyFinder(IFileFinder fileFinder, INfprojParser nfprojParser, INuspecParser nuspecParser, INugetClient nugetClient, ICache cache)
        {
            _fileFinder = fileFinder;
            _nfprojParser = nfprojParser;
            _nuspecParser = nuspecParser;
            _nugetClient = nugetClient;
            _cache = cache;
        }

        public IEnumerable<Package> FindPackages(string rootPath)
        {
            var packagePaths = new HashSet<string>();

            _fileFinder.ProcessNfprojFiles(rootPath, reader =>
            {
                foreach (var packagesPath in _nfprojParser.GetPackagesPaths(reader))
                {
                    packagePaths.Add(packagesPath);
                }
            });

            var packages = new List<Package>();

            _fileFinder.ProcessNuspecFiles(rootPath, packagePaths, reader =>
            {
                packages.Add(_nuspecParser.Parse(reader));
            });
            

            return packages;
        }

        public IEnumerable<Package> GetDependenciesFromNuget(string id, string version,
            bool refreshCache = false)
        {
            return GetDependenciesFromNuget(id, version, refreshCache, new List<Package>(),
                new Dictionary<string, string>());
        }

        private IEnumerable<Package> GetDependenciesFromNuget(string id, string version, bool refreshCache,
            IList<Package> packages, Dictionary<string, string> versionMapping)
        {

            if (versionMapping.ContainsKey($"{id}-{version}"))
                version = versionMapping[$"{id}-{version}"];


            if (packages.Any(p => p.Id == id && p.Version == version))
                return packages;


            string versionFound = version;
            var package = _cache.GetOrAdd(id, version,
                (pId, pVersion) => _nugetClient.GetFromNuspec(pId, pVersion, out versionFound), refreshCache);

            if (versionFound != version)
            {
                versionMapping.Add($"{id}-{version}", versionFound);
            }

            packages.Add(package);

            foreach (var p in package.Dependencies)
            {
                if (!MinimumVersions.IsMinimumVersion(p))
                    throw new PackageNotFoundException(
                        $"Package {p.Id} {p.Version} does not match minimum required version");


                if (!packages.Contains(p))
                {
                    if (p.IsNative)
                        packages.Add(p);
                    else
                        GetDependenciesFromNuget(p.Id, p.Version, refreshCache, packages, versionMapping);
                }
            }


            return packages;
        }

        public string? FindCompatibleVersion(string id, IEnumerable<Package> nativePackages,
            bool refreshCache = false, bool silent = false)

        {
            var versions = _nugetClient.GetPackageVersions(id);
            var np = nativePackages as Package[] ?? nativePackages.ToArray();

            foreach (var version in versions)
            {
                if (!silent)
                    Console.WriteLine($"Checking {id} {version}...");

                try
                {
                    var deps = GetDependenciesFromNuget(id, version, refreshCache).Where(d => d.IsNative);

                    if (!deps.Except(np).Any())
                        return version;

                }
                catch (PackageNotFoundException)
                {
                }

            }

            return null;
        }
    }
}
