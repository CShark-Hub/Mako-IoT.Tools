using System.Collections.Concurrent;

namespace Mako.IoT.NFVersionInspector.Services
{
    public class Cache : ICache
    {
        private readonly IStorage _storage;

        private static readonly IDictionary<string, HashSet<Package>> Packages =
            new ConcurrentDictionary<string, HashSet<Package>>();

        public Cache(IStorage storage)
        {
            _storage = storage;
        }

        public Package GetOrAdd(string id, string version, Func<string, string, Package> getFunc, bool forceRefresh = false)
        {
            if (!Packages.ContainsKey(id))
            {
                Packages.Add(id, new HashSet<Package>(_storage.Load(id)));
            }

            if (forceRefresh)
                Packages[id].RemoveWhere(p => p.Version == version);
            
            var package = Packages[id].SingleOrDefault(p => p.Version == version);

            if (package == null)
            {
                package = getFunc(id, version);
                Packages[id].Add(package);

                _storage.Save(id, Packages[id]);
            }

            return package;
        }
    }
}
