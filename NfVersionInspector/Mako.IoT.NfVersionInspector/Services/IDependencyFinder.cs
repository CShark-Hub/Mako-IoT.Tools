namespace Mako.IoT.NFVersionInspector.Services;

public interface IDependencyFinder
{
    IEnumerable<Package> FindPackages(string rootPath);

    IEnumerable<Package> GetDependenciesFromNuget(string id, string version,
        bool refreshCache = false);

    string? FindCompatibleVersion(string id, IEnumerable<Package> nativePackages,
        bool refreshCache = false, bool silent = false);
}