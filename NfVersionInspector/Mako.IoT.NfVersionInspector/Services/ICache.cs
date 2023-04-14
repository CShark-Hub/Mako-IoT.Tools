namespace Mako.IoT.NFVersionInspector.Services;

public interface ICache
{
    Package GetOrAdd(string id, string version, Func<string, string, Package> getFunc, bool forceRefresh = false);
}