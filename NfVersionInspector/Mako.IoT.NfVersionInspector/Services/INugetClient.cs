namespace Mako.IoT.NFVersionInspector.Services;

public interface INugetClient
{
    Package GetFromNuspec(string id, string version, out string versionFound);
    IEnumerable<string> GetPackageVersions(string id);
}